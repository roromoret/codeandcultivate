using UnityEngine;

// Attach to Farmer GameObject
// Registers the farmer with ExecutableBlock so all block scripts can call from IFarmerActions
public class FarmerBlockBridge : MonoBehaviour
{
    private void Awake()
    {
        IFarmerActions farmer = GetComponent<IFarmerActions>();
        if (farmer != null) ExecutableBlock.RegisterFarmer(farmer);
        else Debug.LogError
        ("[FarmerBlockBridge] No IFarmerActions found on this GameObject. " + "Make sure Farmer.cs is attached.");
    }
}
