using UnityEngine;

public class WorkspaceManager : MonoBehaviour
{
    [Header("Configuration")]
    public GameObject columnPrefab;
    public Transform workspaceContent;
    public GameObject addColumnButton;

    //Method to instantiate a colum prefab 
    public void AddNewColumn()
    {
        GameObject newColumn = Instantiate(columnPrefab, workspaceContent);
        
        int lastIndex = workspaceContent.childCount - 1; 
        newColumn.transform.SetSiblingIndex(lastIndex - 1);
        
    }
}