using UnityEngine;

// This is what the code block execution engine will call
public interface IFarmerActions
{
    void MoveNorth();
    void MoveSouth();
    void MoveWest();
    void MoveEast();
    
    // updated for the dropdown on the plant block
    void Plant(int cropDropdownIndex); 
    
    void Harvest();
    bool IsBusy { get; } // wait if an action is already in progress
}