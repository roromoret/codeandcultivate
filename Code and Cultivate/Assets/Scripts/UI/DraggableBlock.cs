using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

//This object need a canvas group in order to be seen by the unity drag system

public class DraggableBlock : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    public bool isPaletteBlock = false;

    [HideInInspector] public Transform parentToReturnTo = null;
    [HideInInspector] public Transform placeholderParent = null;
    [HideInInspector] public bool isToBeDeleted = false;
    
    [Header("Drag & Drop Settings")]
    public float fixedPlaceholderHeight = 50f;

    private GameObject placeholder = null;
    private CanvasGroup canvasGroup;
    
    private Transform originalParent;
    private bool wasCloned = false;

    void Awake()
    {
        canvasGroup = GetComponent<CanvasGroup>();
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        isToBeDeleted = false;
        originalParent = this.transform.parent;
        parentToReturnTo = originalParent;
        placeholderParent = originalParent;
        wasCloned = false;

        if (isPaletteBlock)
        {
            GameObject clone = Instantiate(this.gameObject, originalParent);
            clone.transform.SetSiblingIndex(this.transform.GetSiblingIndex());
            clone.name = this.gameObject.name;

            this.isPaletteBlock = false;
            wasCloned = true;
        }

        placeholder = new GameObject("Placeholder");
        placeholder.transform.SetParent(this.transform.parent, false);
        
        LayoutElement le = placeholder.AddComponent<LayoutElement>();
        le.preferredWidth = this.GetComponent<LayoutElement>().preferredWidth;
        le.preferredHeight = fixedPlaceholderHeight;
        le.flexibleWidth = 0;
        le.flexibleHeight = 0;

        placeholder.transform.SetSiblingIndex(this.transform.GetSiblingIndex());

        // --- LA SÉCURITÉ BLINDÉE EST ICI ---
        Canvas mainCanvas = GetComponentInParent<Canvas>();
        if (mainCanvas != null)
        {
            // 1. On force l'attachement au Canvas RACINE (ignore les sous-canvas)
            this.transform.SetParent(mainCanvas.rootCanvas.transform, true);
            this.transform.SetAsLastSibling();
            
            // 2. On empêche le bloc de devenir microscopique
            this.transform.localScale = Vector3.one;
            
            // 3. On remet le Z à zéro pour éviter qu'il parte derrière la caméra
            Vector3 safePos = this.transform.localPosition;
            safePos.z = 0f;
            this.transform.localPosition = safePos;
        }
        // -----------------------------------

        canvasGroup.blocksRaycasts = false;
        canvasGroup.alpha = 0.8f;
    }

    public void OnDrag(PointerEventData eventData)
    {
        this.transform.position = eventData.position;

        if (placeholder.transform.parent != placeholderParent)
        {
            placeholder.transform.SetParent(placeholderParent);
        }

        int newSiblingIndex = placeholderParent.childCount;

        for (int i = 0; i < placeholderParent.childCount; i++)
        {
            if (this.transform.position.y > placeholderParent.GetChild(i).position.y)
            {
                newSiblingIndex = i;
                if (placeholder.transform.GetSiblingIndex() < newSiblingIndex)
                    newSiblingIndex--;
                break;
            }
        }
        placeholder.transform.SetSiblingIndex(newSiblingIndex);
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        canvasGroup.blocksRaycasts = true;
        canvasGroup.alpha = 1f;
        
        if (isToBeDeleted)
        {
            Destroy(placeholder);
            Destroy(this.gameObject);
            Canvas.ForceUpdateCanvases();
            return;
        }

        if (wasCloned && parentToReturnTo == originalParent)
        {
            Destroy(placeholder);
            Destroy(this.gameObject);
            Canvas.ForceUpdateCanvases();
            return;
        }

        this.transform.SetParent(parentToReturnTo, false);
        this.transform.SetSiblingIndex(placeholder.transform.GetSiblingIndex());
        
        this.transform.localScale = Vector3.one;

        Destroy(placeholder);
        Canvas.ForceUpdateCanvases();
    }
}