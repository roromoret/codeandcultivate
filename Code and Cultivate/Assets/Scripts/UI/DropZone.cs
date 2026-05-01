using UnityEngine;
using UnityEngine.EventSystems;

public class DropZone : MonoBehaviour, IDropHandler, IPointerEnterHandler
{
    public Transform blocksContent;

    //This componnent need to be added on an element with an image so the raycast sees it
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