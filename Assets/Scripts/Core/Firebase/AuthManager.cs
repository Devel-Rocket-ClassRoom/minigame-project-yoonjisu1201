using UnityEngine;
using Cysharp.Threading.Tasks;
using Firebase.Auth;
using System;

public class AuthManager : MonoBehaviour
{
    private static AuthManager instance;
    public static AuthManager Instance => instance;

    private FirebaseAuth _auth;
    private FirebaseUser _currentUser;

    private bool _isInitialized;
    private bool? _lastLoginState;

    public FirebaseUser CurrentUser => _currentUser;
    public bool IsLoggedIn => _currentUser != null;
    public bool IsAnonymous => _currentUser?.IsAnonymous ?? false;
    public string UserId => _currentUser?.UserId ?? string.Empty;
    public bool IsInitialized => _isInitialized;

    public event Action<bool> OnLoginStateChanged;

    private void Awake()
    {
        if (instance != null)
        {
            Destroy(this);
            return;
        }

        instance = this;
        DontDestroyOnLoad(gameObject);
    }

    private async UniTaskVoid Start()
    {
        if (FirebaseInitializer.Instance == null)
        {
            Debug.Log("[Auth] FirebaseInitializer가 씬에 없습니다.");
            return;
        }

        bool isReady = await FirebaseInitializer.Instance.WaitUntilReadyAsync();

        if (!isReady)
        {
            Debug.LogError("[Auth] Firebase 초기화 실패. 로그인 기능 사용 불가.");
            return;
        }

        _auth = FirebaseInitializer.Instance.Auth;
        _auth.StateChanged += OnAuthStateChanged;

        _currentUser = _auth.CurrentUser;
        _isInitialized = true;

        Debug.Log(IsLoggedIn ? $"[Auth] 이미 로그인됨: {UserId}" : "[Auth] 로그인 필요");

        NotifyLoginState();
    }

    private void OnAuthStateChanged(object sender, EventArgs e)
    {
        _currentUser = _auth.CurrentUser;
        NotifyLoginState();
    }
    public async UniTask<(bool success, string error)> SignInAnonymouslyAsync()
    {
        if (!CanUseAuth(out string error))
        {
            return (false, error);
        }

        if (IsLoggedIn && IsAnonymous)
        {
            Debug.Log($"[Auth] 기존 게스트 세션 사용: {UserId}");
            return (true, null);
        }

        if (IsLoggedIn && !IsAnonymous)
        {
            Debug.Log("[Auth] 이메일 세션 종료 후 게스트 로그인 전환");
            SignOutCurrentUser();
            await UniTask.Yield();
        }

        try
        {
            Debug.Log("[Auth] 게스트 로그인 시도");

            AuthResult result = await _auth.SignInAnonymouslyAsync();
            _currentUser = result.User;

            NotifyLoginState();

            Debug.Log($"[Auth] 게스트 로그인 성공: {UserId}");
            return (true, null);
        }
        catch (Exception ex)
        {
            Debug.LogError($"[Auth] 게스트 로그인 실패: {ex.Message}");
            return (false, "게스트 로그인에 실패했습니다.");
        }
    }

    public async UniTask<(bool success, string error)> SignUpWithEmailAsync(string email, string password)
    {
        if (!CanUseAuth(out string error))
        {
            return (false, error);
        }

        if (IsLoggedIn && !IsAnonymous)
        {
            return (true, null);
        }

        if (!ValidateEmailAndPassword(email, password, out error))
        {
            return (false, error);
        }

        try
        {
            Debug.Log("[Auth] 회원가입 시도");

            AuthResult result = await _auth.CreateUserWithEmailAndPasswordAsync(email, password);
            _currentUser = result.User;

            NotifyLoginState();

            Debug.Log($"[Auth] 회원가입 성공: {UserId}");
            return (true, null);
        }
        catch (Exception ex)
        {
            Debug.LogError($"[Auth] 회원가입 실패: {ex.Message}");
            return (false, "회원가입에 실패했습니다.");
        }
    }

    public async UniTask<(bool success, string error)> SignInWithEmailAsync(string email, string password)
    {
        if (!CanUseAuth(out string error))
        {
            return (false, error);
        }

        if (IsLoggedIn && !IsAnonymous)
        {
            return (true, null);
        }

        if (!ValidateEmailAndPassword(email, password, out error))
        {
            return (false, error);
        }

        try
        {
            Debug.Log("[Auth] 이메일 로그인 시도");

            AuthResult result = await _auth.SignInWithEmailAndPasswordAsync(email, password);
            _currentUser = result.User;

            NotifyLoginState();

            Debug.Log($"[Auth] 이메일 로그인 성공: {UserId}");
            return (true, null);
        }
        catch (Exception ex)
        {
            Debug.LogError($"[Auth] 이메일 로그인 실패: {ex.Message}");
            return (false, "이메일 또는 비밀번호를 확인해주세요.");
        }
    }
    public void SignOut()
    {
        if (_auth == null) return;

        if (IsAnonymous)
        {
            Debug.Log($"[Auth] 게스트 세션 유지: {UserId}");
            return;
        }

        SignOutCurrentUser();
    }

    private void SignOutCurrentUser()
    {
        Debug.Log("[Auth] 로그아웃");

        _auth.SignOut();
        _currentUser = null;

        NotifyLoginState();
    }

    private bool CanUseAuth(out string error)
    {
        if (!_isInitialized || _auth == null)
        {
            error = "Firebase 로그인이 아직 준비되지 않았습니다.";
            return false;
        }

        error = null;
        return true;
    }

    private bool ValidateEmailAndPassword(string email, string password, out string error)
    {
        if (string.IsNullOrWhiteSpace(email))
        {
            error = "이메일을 입력해주세요.";
            return false;
        }

        if (string.IsNullOrWhiteSpace(password))
        {
            error = "비밀번호를 입력해주세요.";
            return false;
        }

        if (password.Length < 6)
        {
            error = "비밀번호는 6자 이상이어야 합니다.";
            return false;
        }

        error = null;
        return true;
    }

    private void NotifyLoginState()
    {
        bool isLoggedIn = IsLoggedIn;

        if (_lastLoginState.HasValue && _lastLoginState.Value == isLoggedIn) return;

        _lastLoginState = isLoggedIn;

        Debug.Log(isLoggedIn ? $"[Auth] 로그인 상태: {UserId}" : "[Auth] 로그아웃 상태");

        OnLoginStateChanged?.Invoke(isLoggedIn);
    }
    private void OnDestroy()
    {
        if (_auth != null)
        {
            _auth.StateChanged -= OnAuthStateChanged;
        }

        if (instance == this)
        {
            instance = null;
        }
    }
}
