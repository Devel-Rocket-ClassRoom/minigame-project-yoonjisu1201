using UnityEngine;
using Cysharp.Threading.Tasks;
using Firebase;
using Firebase.Auth;
using Firebase.Database;

public enum InitState
{
    Pending,
    Ready,
    Failed
}
public class FirebaseInitializer : MonoBehaviour
{
    private static FirebaseInitializer instance;

    public static FirebaseInitializer Instance => instance;

    [SerializeField] private FirebaseConfig _config;

    public InitState State { get; private set; } = InitState.Pending;
    public bool IsReady => State == InitState.Ready;
    public string LastError { get; private set; }

    public FirebaseApp App { get; private set; }
    public FirebaseAuth Auth { get; private set; }
    public FirebaseDatabase Database { get; private set; }
    public DatabaseReference RootReference => Database?.RootReference;

    public void Awake()
    {
        if (instance != null)
        {
            Destroy(gameObject);
            return;
        }

        instance = this;
        DontDestroyOnLoad(gameObject);

        InitializeAsync().Forget();
    }

    private async UniTaskVoid InitializeAsync()
    {
        Debug.Log("[Firebase] 초기화 시작");

        try
        {
            DependencyStatus status =
                await FirebaseApp.CheckAndFixDependenciesAsync().AsUniTask();
            Debug.Log($"[Firebase] 의존성 검사 결과: {status}");

            if (status != DependencyStatus.Available)
            {
                Fail($"Firebase 의존성 오류: {status}");
                return;
            }

            App = FirebaseApp.DefaultInstance;
            Auth = FirebaseAuth.GetAuth(App);
            Database = GetDatabase(App);

            State = InitState.Ready;
            Debug.Log("[Firebase] 초기화 성공");
        }
        catch (System.Exception ex)
        {
            Fail(ex.Message);
        }
    }

    private FirebaseDatabase GetDatabase(FirebaseApp app)
    {
        if (_config != null && _config.IsValid)
        {
            return FirebaseDatabase.GetInstance(app, _config.databaseUrl);
        }

        return FirebaseDatabase.GetInstance(app);
    }
    private void Fail(string error)
    {
        LastError = error;
        State = InitState.Failed;
        Debug.LogError($"[Firebase] 초기화 실패: {error}");
    }
    
    public async UniTask<bool> WaitUntilReadyAsync()
    {
        await UniTask.WaitUntil(() => State != InitState.Pending);
        return IsReady;
    }

    private void OnDestroy()
    {
        if (Instance == this)
        {
            instance = null;
        }
    }
}
