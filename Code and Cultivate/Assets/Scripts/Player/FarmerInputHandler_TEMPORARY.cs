using UnityEngine;
using UnityEngine.InputSystem;

// TEMPORARY - broadcasts input to ALL farmers simultaneously
// replace with proper per-farmer input routing when farmer assignment is implemented in the codeblocks
public class FarmerInputHandler_TEMPORARY : MonoBehaviour
{
    private IFarmerActions[] _farmers;

    private void Start()
    {
        RefreshFarmers();
    }

    public void RefreshFarmers() // Call this whenever a farmer is added or removed from the scene
    {
        _farmers = FindObjectsByType<Farmer>(FindObjectsSortMode.None);
        Debug.Log($"[FarmerInputHandler] Tracking {_farmers.Length} farmer(s).");
    }
    private void Update()
    {
        InputHandler();
    }

    // arrow keys to move farmer, 1 and 2 to plant and harvest, T to pass turn, F spawn new farmer
    private void InputHandler()
    {
        var keyboard = Keyboard.current;
        if (keyboard == null || _farmers == null) return;

        foreach (IFarmerActions farmer in _farmers)
        {
            if (keyboard.upArrowKey.isPressed) farmer.MoveNorth();
            if (keyboard.leftArrowKey.isPressed) farmer.MoveWest();
            if (keyboard.downArrowKey.isPressed) farmer.MoveSouth();
            if (keyboard.rightArrowKey.isPressed) farmer.MoveEast();
            if (keyboard.digit1Key.isPressed) farmer.Plant(0);
            if (keyboard.digit2Key.isPressed) farmer.Harvest();
        }

        // Fusion : On conserve l'ajout de ton collègue pour faire spawn de nouveaux fermiers
        if (keyboard.fKey.wasPressedThisFrame) FarmerSpawner.Instance?.TrySpawnAdditionalFarmer(WorldGenerator_GetCenter());
    }

    // temp private helper - asks WorldGenerator for the center position rather than duplicating the calculation
    private static Vector3 WorldGenerator_GetCenter()
    {
        var wg = Object.FindFirstObjectByType<WorldGenerator>();
        if (wg == null)
        {
            Debug.LogError("[FarmerInputHandler] WorldGenerator not found in scene.");
            return Vector3.zero;
        }
        return wg.GetCenterWorldPosition();
    }
}