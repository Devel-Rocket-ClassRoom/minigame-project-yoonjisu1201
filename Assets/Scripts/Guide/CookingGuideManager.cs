using Cysharp.Threading.Tasks;
using UnityEngine;

// 첫 영업 한정 튜토리얼 가이드. 6단계: 손님도착 → 주문확인 →
// 재료투입 → 실패 → 다시만들기 -> 성공 -> 서빙
public class CookingGuideManager : MonoBehaviour
{
    public static CookingGuideManager instance;

    [SerializeField] private GuideUI guideUI;

    [SerializeField] private Transform cookingSlotTarget;
    [SerializeField] private Transform cancelButtonTarget;
    [SerializeField] private Transform guestTarget;
    [SerializeField] private IngredientObject[] ingredientTargets;
    [SerializeField] private IngredientObject[] retryIngredientTargets;
    [SerializeField] private GuestSpawner[] _pausedSpawners;

    [SerializeField] private GameObject goldUI;
    [SerializeField] private GameObject timerUI;
    [SerializeField] private GameObject pauseButton;
    [SerializeField] private GameObject collectionbook;

    private GuestController _currentGuest;
    private int _ingredientIndex = 0;
    private int _retryIngredientIndex = 0;
    private int _stepIndex = -1;
    private int _subStep = 0;

    private bool _guideDone = false;
    public int StepIndex => _stepIndex;
    public int SubStep => _subStep;
    // 각 단계별 원 크기 (주문팝업, 재료, 조리대, 쓰레기통, 손님)
    private readonly float[] _circleSizes = { 600f, 200f, 200f, 270f, 200f, 600f };

    private System.Action<CookingSlot> _onAnyIngredientAdded;

    private const string PREF_KEY = "guide_cooking_done";
    public static string PrefKey => AuthManager.Instance != null && AuthManager.Instance.IsLoggedIn
        ? $"{PREF_KEY}_{AuthManager.Instance.UserId}"
        : PREF_KEY;

    public static bool IsGuideDone()
    {
        if (TruckRankManager.instance != null && TruckRankManager.instance.CurrentRank > 1)
            return true;

        return PlayerPrefs.GetInt(PrefKey, 0) == 1;
    }

    public static void MarkGuideDone()
    {
        PlayerPrefs.SetInt(PrefKey, 1);
    }

    public static void ResetGuideDone()
    {
        PlayerPrefs.DeleteKey(PrefKey);
    }
    private void Awake()
    {
        instance = this;
    }
    private void Start()
    {
        if (IsGuideDone()) { _guideDone = true; return; }

        goldUI.SetActive(false);
        timerUI.SetActive(false);
        pauseButton.SetActive(false);
        collectionbook.SetActive(false);

        foreach (var s in _pausedSpawners)
            s.StopSpawning();

        SessionManager.instance.PauseTimer();

        _onAnyIngredientAdded = _ => HandleIngredientAdded();

        guideUI.OnClicked += HandleTouch;
        GuestSpawner.OnGuestSpawned += HandleGuestSpawned;
        GuestController.OnGuestOrdering += HandleGuestOrdering;
        CookingSlot.OnAnyIngredientAdded += _onAnyIngredientAdded;
        CookingSlot.OnAnyStateChanged += HandleCookingStateChanged;
        DraggableFood.OnAnyServeSuccess += HandleServeSuccess;
    }
    private void HandleTouch()
    {
        if (_stepIndex == 0)
        {
            if (_subStep == 0)
            {
                _subStep = 1;
                guideUI.ShowSub(1);
            }
            else
            {
                _subStep = 0;
                ShowStep(1, ingredientTargets[0].transform);
            }
        }
        if (_stepIndex == 3)
        {
            if (_subStep == 0)
            {
                _subStep = 1;
                guideUI.ShowSub(1);
                guideUI.SetBlocksRaycast(false);
            }
        }
        if (_stepIndex == 4)
        {
            if (_subStep == 0)
            {
                _subStep = 1;
                guideUI.ShowSub(1);
                guideUI.SetBlocksRaycast(false);
            }
        }
        if (_stepIndex == 5 && _subStep == 1)
        {
            _guideDone = true;
            guideUI.Hide();
            MarkGuideDone();
            SaveManager.Instance?.SaveWithBackupAsync().Forget();

            SessionManager.instance.ResumeTimer();
            _currentGuest?.Resume();

            foreach (var s in _pausedSpawners)
                s.StopSpawning();
            foreach (var s in _pausedSpawners)
                s.StartSpawning();

            goldUI.SetActive(true);
            timerUI.SetActive(true);
            pauseButton.SetActive(true);
            collectionbook.SetActive(true);
        }
    }
    private void OnDestroy()
    {
        guideUI.OnClicked -= HandleTouch;
        GuestSpawner.OnGuestSpawned -= HandleGuestSpawned;
        GuestController.OnGuestOrdering -= HandleGuestOrdering;
        CookingSlot.OnAnyIngredientAdded -= _onAnyIngredientAdded;
        CookingSlot.OnAnyStateChanged -= HandleCookingStateChanged;
        DraggableFood.OnAnyServeSuccess -= HandleServeSuccess;
    }
    private void ShowStep(int index, Transform circleTarget = null)
    {
        _stepIndex = index;
        _subStep = 0;
        guideUI.Display(_stepIndex + 1, circleTarget, _circleSizes[index]);

        guideUI.SetBlocksRaycast(index == 0 || index == 3 || index == 4);
    }
    private void HandleGuestSpawned(GuestController guest)
    {
        _currentGuest = guest;
    }
    //step1 손님도착 → 주문확인
    private void HandleGuestOrdering(GuestController guest)
    {
        if (_stepIndex >= 0) return;
        guest.Pause();
        ShowStep(0, guestTarget);
    }
    //step2 재료선택 → 조리대
    private void HandleIngredientAdded()
    {
        if (_stepIndex == 1)
        {
            _ingredientIndex++;

            if (_ingredientIndex == 1)
                guideUI.ShowSub(1);

            if (_ingredientIndex < ingredientTargets.Length)
                guideUI.MoveCircle(ingredientTargets[_ingredientIndex].transform);
            else
                ShowStep(2, cookingSlotTarget);
        }
        else if (_stepIndex == 4 && _subStep == 1)
        {
            _retryIngredientIndex++;

            if (_retryIngredientIndex < retryIngredientTargets.Length)
                guideUI.MoveCircle(retryIngredientTargets[_retryIngredientIndex].transform);
            else
            {
                _subStep = 2;
                guideUI.MoveCircle(cookingSlotTarget);
            }
        }
    }
    // Step 3(조리시작) / Step 4(실패) / Step 5(성공)
    private void HandleCookingStateChanged(CookingSlot slot)
    {
        if (slot.State == CookingSlotState.Empty && _stepIndex == 3 && _subStep == 1)
        {
            ShowStep(4, retryIngredientTargets[0].transform);
            return;
        }
        if (slot.State == CookingSlotState.Cooking && _stepIndex == 2)
        {
            guideUI.ShowSub(1); // 조리 시작하면 sub2 표시
            return;
        }

        if (slot.State == CookingSlotState.Ready)
        {
            bool failed = slot.CookedRecipe == null;

            if (failed && _stepIndex == 2)
                ShowStep(3, cancelButtonTarget);           // Step 4: 실패
            else if (!failed && _stepIndex == 4)
            {
                slot.CancelSpoil();
                ShowStep(5, guestTarget);
            }
        }
    }
    private void HandleServeSuccess()
    {
        if (_stepIndex != 5) return;
        _subStep = 1;
        guideUI.ShowSub(1);
        guideUI.SetBlocksRaycast(true);
    }
    public bool IsIngredientAllowed(IngredientSO ingredient)
    {
        if (_guideDone) return true;
        if (_stepIndex == 1)
        {
            if (_ingredientIndex >= ingredientTargets.Length) return true;
            return ingredientTargets[_ingredientIndex].Ingredient == ingredient;
        }
        if (_stepIndex == 4 && _subStep == 1)
        {
            if (_retryIngredientIndex >= retryIngredientTargets.Length) return true;
            return retryIngredientTargets[_retryIngredientIndex].Ingredient == ingredient;
        }
        return false;
    }
    public bool IsCancelAllowed()
    {
        if (_guideDone) return true;
        if (_stepIndex == 3 && _subStep == 1) return true;
        return false;
    }
}
