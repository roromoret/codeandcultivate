using UnityEngine;
using UnityEngine.InputSystem;

public class MovementReward : MonoBehaviour
{
    private bool hasReceivedMovementReward = false;

    void Update()
    {
        if (!hasReceivedMovementReward)
        {
            if (Keyboard.current.wKey.wasPressedThisFrame || 
                Keyboard.current.aKey.wasPressedThisFrame || 
                Keyboard.current.sKey.wasPressedThisFrame || 
                Keyboard.current.dKey.wasPressedThisFrame)
            {
                GiveReward();
            }
        }
    }

    void GiveReward()
    {
        if (ResourceManager.Instance != null)
        {
            ResourceManager.Instance.Add(ResourceType.Money, 1);
            hasReceivedMovementReward = true;
            Debug.Log("Movement reward added to ResourceManager!");
        }
    }
}