using UnityEngine;
using UnityEngine.EventSystems;

//Just a class for deleting the blocks when you slide them back in the block spawn

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