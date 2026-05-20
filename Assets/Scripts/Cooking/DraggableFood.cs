using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

[RequireComponent(typeof(Collider2D))]
public class DraggableFood : MonoBehaviour, IBeginDragHandler, IEndDragHandler, IDragHandler
{
    private RecipeSO _recipe; //조리대 슬롯에서 만든 레시피
    private CookingSlot _slot;
    public Vector3 _originPosition;
    private Camera _cam;

    private void Awake()
    {
        _cam = Camera.main;
    }
    public void Setup(RecipeSO recipe, CookingSlot slot)
    {
        _recipe = recipe;
        _slot = slot;
        gameObject.SetActive(true);
    }
    public void OnBeginDrag(PointerEventData eventData)
    {
        _originPosition = transform.position;
    }
    public void OnDrag(PointerEventData eventData)
    {
        Vector3 worldPos = _cam.ScreenToWorldPoint(eventData.position);
        worldPos.z = transform.position.z;
        transform.position = worldPos;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        if (_recipe == null)
        {
            transform.position = _originPosition;
            return;
        }
        Vector2 dropPos = _cam.ScreenToWorldPoint(eventData.position);
        Collider2D[] hits = Physics2D.OverlapPointAll(dropPos);

        Debug.Log($"드롭 위치: {dropPos}, 감지된 콜라이더 수: {hits.Length}");

        foreach (var hit in hits)
        {
            Debug.Log($"hit: {hit.gameObject.name}");
            if (hit.gameObject == gameObject) continue;

            var guest = hit.GetComponent<GuestController>();
            Debug.Log($"GuestController 있음: {guest != null}");
            if (guest == null) continue;

            if (guest.CurrentOrder != _recipe) continue;
            //_slot.Ingredients는 IReadOnlyList 이기 때문에 복사해서 사용
            bool isValid = RecipeValidator.ValidateForGuest(
                new List<IngredientSO>(_slot.Ingredients), guest.CurrentOrder, guest.GhostData);

            if (isValid)
            {
                    guest.ReceiveFood();
                    _slot.CollectAndReset();
                    gameObject.SetActive(false);
                    return;
            }
        }
        transform.position = _originPosition;
    }
}
