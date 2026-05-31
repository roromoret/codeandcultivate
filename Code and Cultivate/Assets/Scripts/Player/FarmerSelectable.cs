using UnityEngine;

// Attach to every Farmer prefab (req. a Collider for OnMouseDown)
// Raises OnFarmerClicked with itself or OnDeselect when the camera raycast lands on nothing
[RequireComponent(typeof(Collider))]
public class FarmerSelectable : MonoBehaviour
{
    public static event System.Action<FarmerSelectable> OnFarmerClicked;

    private void OnMouseDown() => OnFarmerClicked?.Invoke(this);

    public void SimulateClick() => OnFarmerClicked?.Invoke(this); // call from tests to sim a click
}
