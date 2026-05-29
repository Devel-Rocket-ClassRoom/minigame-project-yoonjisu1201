using UnityEngine;
using UnityEngine.EventSystems;

public class CookButton : MonoBehaviour, IPointerClickHandler
{
    public void OnPointerClick(PointerEventData eventData)
    {
        CookingSlotManager.Instance.OnStartCooking();
    }
}
