using System.ComponentModel;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.InputSystem;

public class ResourceInputHandler_TEMPORARY : MonoBehaviour
{
    private void Update()
    {
        if (ResourceManager.Instance == null) return;

        bool adding   = Keyboard.current.leftBracketKey.isPressed;
        bool removing = Keyboard.current.rightBracketKey.isPressed;

        if (!adding && !removing) return;

        if (Keyboard.current.digit7Key.wasPressedThisFrame) ApplyResource(ResourceType.Money, adding);
        if (Keyboard.current.digit8Key.wasPressedThisFrame) ApplyResource(ResourceType.Fruits, adding);
        if (Keyboard.current.digit9Key.wasPressedThisFrame) ApplyResource(ResourceType.Vegetables, adding);
        if (Keyboard.current.digit0Key.wasPressedThisFrame) ApplyResource(ResourceType.Berries, adding);
    }

    private void ApplyResource(ResourceType type, bool adding)
    {
        if (adding)
        {
            ResourceManager.Instance.Add(type, 1);
            Debug.Log($"[ResourceInputHandler] Added 1 {type} to resources");
        }

        else
        {
            bool success = ResourceManager.Instance.Spend(type, 1);
            if (success) Debug.Log($"[ResourceInputHandler] Removed 1 {type} from resources");
            else         Debug.Log($"[ResourceInputHandler] Tried to remove 1 {type} from resources - already at 0");
        }
    }
}
