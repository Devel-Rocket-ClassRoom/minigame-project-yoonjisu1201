using System.Collections.Generic;
using System.Linq;
using UnityEngine;

//손님 주문 담당, 주문팝업 보이고 숨기는 메서드 호출
public class GuestController : MonoBehaviour
{
    [SerializeField] private GhostSO _ghostData;
    [SerializeField] private CookingSlot _cookingSlot;
    [SerializeField] private OrderPopup _orderPopup; 

    public GhostSO GhostData => _ghostData;

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
    //호출은 DraggableFood에서
    public void ReceiveFood()
    {
        CurrentOrder = null;
        _orderPopup.Hide();
    }

}
