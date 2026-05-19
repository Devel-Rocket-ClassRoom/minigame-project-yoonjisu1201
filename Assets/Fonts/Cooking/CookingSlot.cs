using UnityEngine;
using System.Collections.Generic;
using UnityEngine.iOS;
using System.Collections;

public enum CookingSlotState
{
    Empty,
    Filling,
    Cooking,
    Ready,
    Spoiled
}

public class CookingSlot : MonoBehaviour
{
    private CookingSlotState _state;
    private List<IngredientSO> _ingredients;
    private Coroutine _cookingCoroutine;
    [SerializeField] private float spoilTime = 10f;

    public CookingSlotState State => _state;
    public IReadOnlyList<IngredientSO> Ingredients => _ingredients;

    public event System.Action<CookingSlotState> OnStateChanged;

    private void Awake()
    {
        _ingredients = new List<IngredientSO>();
        _state = CookingSlotState.Empty;
    }

    //조리대에 재료 추가 -> 버튼 UI에서 직접참조 하던거 매니저제어로 변경
    public void AddIngredient(IngredientSO ingredient)
    {
        if (_state != CookingSlotState.Empty && _state != CookingSlotState.Filling)
            return;

        _ingredients.Add(ingredient);
        _state = CookingSlotState.Filling;
        OnStateChanged?.Invoke(_state);
        Debug.Log($"조리대 재료 추가: 현재{_ingredients.Count}개, 조리대 상태: {_state}");
    }
    public void StartCooking(float cookTime)
    {
        if (_state != CookingSlotState.Filling)
        {
            Debug.Log($"조리대 상태: {_state}");
            return;
        }
        _state = CookingSlotState.Cooking;
        OnStateChanged?.Invoke(_state);
        _cookingCoroutine = StartCoroutine(CoCookingRoutine(cookTime));
        Debug.Log($"요리 중 . . .{_state}");
    }

    private IEnumerator CoCookingRoutine(float cookTime)
    {
        yield return new WaitForSeconds(cookTime);

        //요리가 실패했을때는?
        _state = CookingSlotState.Ready;
        OnStateChanged?.Invoke(_state);
        StartCoroutine(CoSpoilRoutine(spoilTime));
    }
    private IEnumerator CoSpoilRoutine(float spoilTime)
    {
        yield return new WaitForSeconds(spoilTime);

        if (_state == CookingSlotState.Ready)
        {
            _state = CookingSlotState.Spoiled;
            OnStateChanged?.Invoke(_state);
        }
    }
    public void CancelCooking()
    {
        if (_state == CookingSlotState.Empty || _state == CookingSlotState.Cooking)
            return;
        _ingredients.Clear();
        _state = CookingSlotState.Empty;
        OnStateChanged?.Invoke(_state);
        Debug.Log($"모든 재료를 버렸습니다. 현재{_ingredients.Count}개");
    }
    //완성된 음식 서빙. 레시피 반환받아서 검증에 사용
     public List<IngredientSO> CollectAndReset()
    {
        if (_state != CookingSlotState.Ready)
            return null;
        // 원본 리스트를 복사 — Clear() 전에 새 리스트로 복사하지 않으면 반환 후 빈 리스트가 됨
        List<IngredientSO> recipe = new List<IngredientSO>(_ingredients);
        _ingredients.Clear();
        _state = CookingSlotState.Empty;
        OnStateChanged?.Invoke(_state);
        return recipe;
    }

}
