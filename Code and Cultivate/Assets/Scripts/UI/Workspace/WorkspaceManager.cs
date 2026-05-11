using UnityEngine;

public class WorkspaceManager : MonoBehaviour
{
    public GameObject columnPrefab;
    public Transform workspaceContent;
    public GameObject addColumnButton;
    public ColumnPaginator paginator;
    
    //Method to instantiate a colum prefab 
    public void AddNewColumn()
    {
        GameObject newColumn = Instantiate(columnPrefab, workspaceContent);
        
        int lastIndex = workspaceContent.childCount - 1; 
        newColumn.transform.SetSiblingIndex(lastIndex - 1);
        
        this.transform.SetAsLastSibling(); 

        ColumnController scriptColonne = newColumn.GetComponent<ColumnController>();
        if (scriptColonne != null)
        {
            scriptColonne.paginator = this.paginator;
        }

        if (paginator != null)
        {
            paginator.RefreshPagination();
        }
    }
}