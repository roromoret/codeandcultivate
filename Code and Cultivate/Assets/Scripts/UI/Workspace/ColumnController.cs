using UnityEngine;

public class ColumnController : MonoBehaviour
{
    public ColumnPaginator paginator;
    
    //basic deletion of the column
    public void DeleteColumn()
    {
        this.transform.SetParent(null); 
        
        Destroy(this.gameObject);       
        
        if (paginator != null)
        {
            paginator.RefreshPagination();
        }
    }
}