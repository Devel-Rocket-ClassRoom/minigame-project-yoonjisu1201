using UnityEngine;
using UnityEngine.EventSystems;

[RequireComponent(typeof(Collider2D))]
public class DraggableFood : MonoBehaviour, IBeginDragHandler, IEndDragHandler, IDragHandler
{
    private RecipeSO _recipe;
    public Vector3 _originPosition;
    private Camera _cam;

    private void Awake()
    {
        _cam = Camera.main;
    }
    public void Setup(RecipeSO recipe)
    {
        _recipe = recipe;
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
        throw new System.NotImplementedException();
    }
}
