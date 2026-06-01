using UnityEngine;

// 첫 영업 한정 튜토리얼 가이드. 5단계: 손님도착 → 주문확인 → 재료투입 → 실패 → 서빙
public class CookingGuideManager : MonoBehaviour
{
    public static CookingGuideManager instance;

    [SerializeField] private GuideUI guideUI;

    [SerializeField] private Transform cookingSlotTarget;
    [SerializeField] private Transform cancelButtonTarget;
    [SerializeField] private Transform guestTarget;
    [SerializeField] private IngredientObject[] ingredientTargets;
    [SerializeField] private GuestSpawner[] _pausedSpawners;

    [SerializeField] private GameObject goldUI;
    [SerializeField] private GameObject timerUI;
    [SerializeField] private GameObject lobbyButton;

    private GuestController _currentGuest;
    private int _subStep = 0;
    private int _ingredientIndex = 0;
    private int _stepIndex = -1;

    private bool _guideDone = false;
    public int StepIndex => _stepIndex;
    // 각 단계별 원 크기 (주문팝업, 재료, 조리대, 쓰레기통, 손님)
    private readonly float[] _circleSizes = { 600f, 160f, 260f, 270f, 200f };

    private System.Action<CookingSlot> _onAnyIngredientAdded;

    private const string PREF_KEY = "guide_cooking_done";
    private void Awake()
    {
        instance = this;
    }
    private void Start()
    {
        goldUI.SetActive(false);
        timerUI.SetActive(false);
        lobbyButton.SetActive(false);

        PlayerPrefs.DeleteKey(PREF_KEY); // 테스트 끝나면 꼭 지울 것

        if (PlayerPrefs.GetInt(PREF_KEY, 0) == 1) return;

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

        // Step 1(index 0)만 터치 차단, 나머지는 통과
        guideUI.SetBlocksRaycast(index == 0 || index == 3);
    }
    private void HandleGuestSpawned(GuestController guest)
    {
        _currentGuest = guest;
    }
    //step1 손님도착 → 주문확인
    private void HandleGuestOrdering(GuestController guest)
    {
        if (_stepIndex >= 0) return;
        guest.PausePatience();
        ShowStep(0, guestTarget);
    }
    //step2 재료선택 → 조리대
    private void HandleIngredientAdded()
    {
        if (_stepIndex != 1) return;

        _ingredientIndex++;

        if (_ingredientIndex == 1)
            guideUI.ShowSub(1);

        if (_ingredientIndex < ingredientTargets.Length)
            guideUI.MoveCircle(ingredientTargets[_ingredientIndex].transform);
        else
            ShowStep(2, cookingSlotTarget);
    }
    // Step 3(조리시작) / Step 4(실패) / Step 5(성공)
    private void HandleCookingStateChanged(CookingSlot slot)
    {
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
            else if (!failed && (_stepIndex == 2 || _stepIndex == 3))
                ShowStep(4, guestTarget);    // Step 5: 성공
        }
    }
    private void HandleServeSuccess()
    {
        if (_stepIndex != 4) return;
        _guideDone = true;
        guideUI.Hide();
        PlayerPrefs.SetInt(PREF_KEY, 1);

        SessionManager.instance.ResumeTimer();
        _currentGuest?.ResumePatience();

        foreach (var s in _pausedSpawners)
            s.StartSpawning();

        goldUI.SetActive(true);
        timerUI.SetActive(true);
        lobbyButton.SetActive(true);
    }
    public bool IsIngredientAllowed(IngredientSO ingredient)
    {
        if (_guideDone) return true;
        if (_stepIndex != 1) return false;
        if (_ingredientIndex >= ingredientTargets.Length) return true;
        return ingredientTargets[_ingredientIndex].Ingredient == ingredient;
    }
    public bool IsCancelAllowed()
    {
        if (_guideDone) return true;
        if (_stepIndex == 3 && _subStep == 1) return true;
        return false;
    }
}
