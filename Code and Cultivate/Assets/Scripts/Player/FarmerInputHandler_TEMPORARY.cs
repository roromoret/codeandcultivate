using UnityEngine;
using UnityEngine.InputSystem;

public class FarmerInputHandler_TEMPORARY : MonoBehaviour
{
    private IFarmerActions _farmer;

    void Awake()
    {
        _farmer = GetComponent<IFarmerActions>();
    }

    void Update()
    {
        InputHandler();
    }

    // arrow keys to move farmer, 1 and 2 to plant and harvest
    void InputHandler()
    {
        var keyboard = Keyboard.current;
        if (keyboard == null) return;

        if (keyboard.upArrowKey.isPressed)      _farmer.MoveNorth();
        if (keyboard.leftArrowKey.isPressed)    _farmer.MoveWest();
        if (keyboard.downArrowKey.isPressed)    _farmer.MoveSouth();
        if (keyboard.rightArrowKey.isPressed)   _farmer.MoveEast();
        if (keyboard.digit1Key.isPressed)       _farmer.Plant();
        if (keyboard.digit2Key.isPressed)       _farmer.Harvest();
    }
}
