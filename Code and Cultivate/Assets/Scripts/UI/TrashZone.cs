using UnityEngine;
using UnityEngine.EventSystems;

public class TrashZone : MonoBehaviour, IDropHandler, IPointerEnterHandler, IPointerExitHandler
{
    public void OnPointerEnter(PointerEventData eventData)
    {
        if (eventData.pointerDrag == null) return;

        DraggableBlock d = eventData.pointerDrag.GetComponent<DraggableBlock>();
        if (d != null)
        {
            d.isToBeDeleted = true;
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (eventData.pointerDrag == null) return;

        DraggableBlock d = eventData.pointerDrag.GetComponent<DraggableBlock>();
        if (d != null)
        {
            d.isToBeDeleted = false;
        }
    }

    public void OnDrop(PointerEventData eventData) {}
}