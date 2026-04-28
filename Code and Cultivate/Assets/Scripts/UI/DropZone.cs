using UnityEngine;
using UnityEngine.EventSystems;

public class DropZone : MonoBehaviour, IDropHandler, IPointerEnterHandler
{
    public Transform blocksContent;

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (eventData.pointerDrag == null) return;

        DraggableBlock d = eventData.pointerDrag.GetComponent<DraggableBlock>();
        if (d != null && blocksContent != null)
        {
            d.placeholderParent = blocksContent;
        }
    }

    public void OnDrop(PointerEventData eventData)
    {
        if (eventData.pointerDrag == null) return;

        DraggableBlock d = eventData.pointerDrag.GetComponent<DraggableBlock>();
        if (d != null && blocksContent != null)
        {
            d.parentToReturnTo = blocksContent;
        }
    }
}