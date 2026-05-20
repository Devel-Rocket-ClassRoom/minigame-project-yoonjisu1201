using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class NewMonoBehaviourScript : MonoBehaviour
{
    [SerializeField] private GhostSO _ghostData;
    [SerializeField] private CookingSlot _cookingSlot;
    [SerializeField] private OrderPopup _orderPopup; 

    public RecipeSO CurrentOrder {  get; private set; }
    private void Start()
    {
        CurrentOrder = RandomOrder();
        _orderPopup.Show(CurrentOrder);
    }

    private RecipeSO RandomOrder()
    {
        //지금은 테스트로 모든 레시피 열어둠
        List<RecipeSO> candidates = _cookingSlot.AllRecipes.ToList();

        if (candidates.Count == 0)
            return null;
        return candidates[UnityEngine.Random.Range(0, candidates.Count)];
    }
}
