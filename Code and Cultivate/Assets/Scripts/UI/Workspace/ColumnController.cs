using UnityEngine;

public class ColumnController : MonoBehaviour
{
    public ColumnPaginator paginator;
    
    //basic deletion of the column
    public void DeleteColumn()
    {
        // release farmer assignment so farmer becomes available to other columns once this one has been deleted
        ColumnExecutor executor = GetComponent<ColumnExecutor>();
        if (executor != null) FarmerAssignment.Instance?.Unassign(executor);

        this.transform.SetParent(null); 
        
        Destroy(this.gameObject);       
        
        if (paginator != null)
        {
            paginator.RefreshPagination();
        }
    }
}