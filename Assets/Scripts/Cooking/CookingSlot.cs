using UnityEngine;
using System.Collections.Generic;
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
    [SerializeField] private List<RecipeSO> _availableRecipes; //활성화된 레시피
    private RecipeSO _cookedRecipe;

    public CookingSlotState State => _state;
    public RecipeSO CookedRecipe => _cookedRecipe;
    public IReadOnlyList<IngredientSO> Ingredients => _ingredients;
    public IReadOnlyList<RecipeSO> AllRecipes => _availableRecipes;

    public event System.Action<CookingSlotState> OnStateChanged;

    private void Awake()
    {
        _ingredients = new List<IngredientSO>();
        _state = CookingSlotState.Empty;
    }

    
    public void AddIngredient(IngredientSO ingredient)
    {
        if (_state != CookingSlotState.Empty && _state != CookingSlotState.Filling)
            return;

        _ingredients.Add(ingredient);
        _state = CookingSlotState.Filling;
        OnStateChanged?.Invoke(_state);
        Debug.Log($"조리대 재료 추가: 현재{_ingredients.Count}개, 조리대 상태: {_state}");
        string ingredientList = string.Join(", ", _ingredients.ConvertAll(i => i.displayName));
        Debug.Log($"[{gameObject.name}] 재료 목록({_ingredients.Count}개): {ingredientList}");
    }
    public void StartCooking(float cookTime)
    {
        if (_state != CookingSlotState.Filling)
        {
            //Debug.Log($"조리대 상태: {_state}");
            return;
        }
        _state = CookingSlotState.Cooking;
        OnStateChanged?.Invoke(_state);
        _cookingCoroutine = StartCoroutine(CoCookingRoutine(cookTime));
        //Debug.Log($"요리 중 . . .{_state}");
    }

    private IEnumerator CoCookingRoutine(float cookTime)
    {
        yield return new WaitForSeconds(cookTime);

        //요리가 실패했을때는?
        _cookedRecipe = RecipeValidator.FindMatchingRecipe(
            new List<IngredientSO>(_ingredients), _availableRecipes);

        //Debug.Log(_cookedRecipe != null ? $"완성: {_cookedRecipe.displayName}" : "망한 음식");

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
        //Debug.Log($"모든 재료를 버렸습니다. 현재{_ingredients.Count}개");
    }
    //완성된 음식 서빙. 레시피 반환받아서 검증에 사용
     public void CollectAndReset()
    {
        if (_state != CookingSlotState.Ready)
            return;

        _ingredients.Clear();
        _cookedRecipe = null;
        _state = CookingSlotState.Empty;
        OnStateChanged?.Invoke(_state);
    }

}
