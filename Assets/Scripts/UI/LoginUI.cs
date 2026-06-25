using Cysharp.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LoginUI : MonoBehaviour
{
    [Header("Panels")]
    [SerializeField] private GameObject loginPanel;
    [SerializeField] private GameObject menuPanel;

    [Header("Input")]
    [SerializeField] private TMP_InputField _emailInput;
    [SerializeField] private TMP_InputField _passwordInput;

    [Header("Button")]
    [SerializeField] private Button _loginButton;
    [SerializeField] private Button _signUpButton;
    [SerializeField] private Button _guestLoginButton;
    [SerializeField] private Button _logoutButton;

    [Header("Text")]
    [SerializeField] private TextMeshProUGUI _statusText;

    private void Start()
    {
        _loginButton.onClick.AddListener(() => OnLoginClickedAsync().Forget());
        _signUpButton.onClick.AddListener(() => OnSignUpClickedAsync().Forget());
        _guestLoginButton.onClick.AddListener(() => OnGuestLoginClickedAsync().Forget());
        _logoutButton.onClick.AddListener(OnLogoutClicked);

        SetLoginState(false);
    }

    private void SetStatus(string message)
    {
        if (_statusText != null)
        {
            _statusText.text = message;
        }

        Debug.Log($"[AuthUI] {message}");
    }

    private async UniTaskVoid OnLoginClickedAsync()
    {
        SetButtonsInteractable(false);
        SetStatus("로그인 중...");

        var result = await AuthManager.Instance.SignInWithEmailAsync(
            _emailInput.text,
            _passwordInput.text);

        if (result.success)
        {
            SetStatus("로그인 성공!");
            SetLoginState(true);
            RefreshTitleContinueButton();
        }
        else
        {
            SetStatus(result.error);
        }

        SetButtonsInteractable(true);
    }

    private async UniTaskVoid OnSignUpClickedAsync()
    {
        SetButtonsInteractable(false);
        SetStatus("회원가입 중...");

        var result = await AuthManager.Instance.SignUpWithEmailAsync(
            _emailInput.text,
            _passwordInput.text);

        if (result.success)
        {
            SetStatus("회원가입 성공!");
            SetLoginState(true);
            RefreshTitleContinueButton();
        }
        else
        {
            SetStatus(result.error);
        }

        SetButtonsInteractable(true);
    }

    private async UniTaskVoid OnGuestLoginClickedAsync()
    {
        SetButtonsInteractable(false);
        SetStatus("게스트 로그인 중...");

        var result = await AuthManager.Instance.SignInAnonymouslyAsync();

        if (result.success)
        {
            SetStatus("게스트 로그인 성공!");
            SetLoginState(true);
            RefreshTitleContinueButton();
        }
        else
        {
            SetStatus(result.error);
        }

        SetButtonsInteractable(true);
    }


    private void OnLogoutClicked()
    {
        bool wasAnonymous = AuthManager.Instance != null && AuthManager.Instance.IsAnonymous;

        AuthManager.Instance.SignOut();

        SetStatus(wasAnonymous ? "게스트 계정은 이 기기에 유지됩니다." : "로그아웃되었습니다.");
        SetLoginState(false);
        RefreshTitleContinueButton();
    }
    private void RefreshTitleContinueButton()
    {
        TitleSceneManager titleSceneManager = FindFirstObjectByType<TitleSceneManager>();
        titleSceneManager?.RefreshContinueButton();
    }

    private void SetButtonsInteractable(bool interactable)
    {
        if (_loginButton != null)
            _loginButton.interactable = interactable;

        if (_signUpButton != null)
            _signUpButton.interactable = interactable;

        if (_guestLoginButton != null)
            _guestLoginButton.interactable = interactable;

        if (_logoutButton != null)
            _logoutButton.interactable = interactable;
    }
    private void SetLoginState(bool isLoggedIn)
    {
        if (loginPanel != null)
        {
            loginPanel.SetActive(!isLoggedIn);
        }

        if (menuPanel != null)
        {
            menuPanel.SetActive(isLoggedIn);
        }

        if (isLoggedIn)
        {
            SetStatus("로그인되었습니다.");
        }
        else
        {
            SetStatus("로그인해주세요.");
        }
    }
}
