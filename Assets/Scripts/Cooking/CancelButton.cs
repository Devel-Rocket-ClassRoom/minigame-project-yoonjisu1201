using UnityEngine;
using UnityEngine.EventSystems;

[RequireComponent(typeof(Collider2D))]
public class CancelButton : MonoBehaviour, IPointerClickHandler
{
    public void OnPointerClick(PointerEventData eventData)
    {
        if (CookingGuideManager.instance != null &&
        !CookingGuideManager.instance.IsCancelAllowed())
            return;

        CookingSlotManager.Instance.OnCancelCooking();
    }
}
