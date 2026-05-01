using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections.Generic;

public class MovementReward : MonoBehaviour
{
    private bool hasReceivedMovementReward = false;
    private bool hasReceivedArrowReward = false;
    private bool hasReceivedJReward = false;
    private bool hasReceivedTimeReward = false;
    private float timer = 0f;
    private HashSet<ResourceType> hasReceivedKeyReward = new HashSet<ResourceType>();

    void Start()
    {
        if (ResourceManager.Instance != null)
        {
            if (ResourceManager.Instance.Get(ResourceType.Money) == 0)
            {
                ResourceManager.Instance.Add(ResourceType.Money, 5);
            }

            if (ResourceManager.Instance.Get(ResourceType.Fruits) == 0)
            {
                ResourceManager.Instance.Add(ResourceType.Fruits, 5);
            }

            if (ResourceManager.Instance.Get(ResourceType.Vegetables) == 0)
            {
                ResourceManager.Instance.Add(ResourceType.Vegetables, 5);
            }

            if (ResourceManager.Instance.Get(ResourceType.Berries) == 0)
            {
                ResourceManager.Instance.Add(ResourceType.Berries, 5);
            }
        }
    }

    void Update()
    {
        if (Keyboard.current == null || ResourceManager.Instance == null) return;

        HandleTimeReward();
        HandleMovement();
        HandleNumbers();
        HandleSpecial();
    }

    void HandleTimeReward()
    {
        if (!hasReceivedTimeReward)
        {
            timer += Time.deltaTime;

            if (timer >= 30f)
            {
                ResourceManager.Instance.Add(ResourceType.Money, 10);
                ResourceManager.Instance.Add(ResourceType.Fruits, 10);
                ResourceManager.Instance.Add(ResourceType.Vegetables, 10);
                ResourceManager.Instance.Add(ResourceType.Berries, 10);
                
                hasReceivedTimeReward = true;
                Debug.Log("30 Second Time Reward Given!");
            }
        }
    }

    void HandleMovement()
    {
        bool wasd = Keyboard.current.wKey.wasPressedThisFrame || 
                    Keyboard.current.aKey.wasPressedThisFrame || 
                    Keyboard.current.sKey.wasPressedThisFrame || 
                    Keyboard.current.dKey.wasPressedThisFrame;

        bool arrows = Keyboard.current.upArrowKey.wasPressedThisFrame || 
                      Keyboard.current.downArrowKey.wasPressedThisFrame || 
                      Keyboard.current.leftArrowKey.wasPressedThisFrame || 
                      Keyboard.current.rightArrowKey.wasPressedThisFrame;

        if (wasd && !hasReceivedMovementReward)
        {
            ResourceManager.Instance.Add(ResourceType.Money, 1);
            hasReceivedMovementReward = true;
        }

        if (arrows && !hasReceivedArrowReward)
        {
            ResourceManager.Instance.Add(ResourceType.Berries, 1);
            hasReceivedArrowReward = true;
        }
    }

    void HandleNumbers()
    {
        if (Keyboard.current.digit7Key.wasPressedThisFrame && !hasReceivedKeyReward.Contains(ResourceType.Money))
        {
            ResourceManager.Instance.Add(ResourceType.Money, 1);
            hasReceivedKeyReward.Add(ResourceType.Money);
        }
        
        if (Keyboard.current.digit8Key.wasPressedThisFrame && !hasReceivedKeyReward.Contains(ResourceType.Fruits))
        {
            ResourceManager.Instance.Add(ResourceType.Fruits, 1);
            hasReceivedKeyReward.Add(ResourceType.Fruits);
        }

        if (Keyboard.current.digit9Key.wasPressedThisFrame && !hasReceivedKeyReward.Contains(ResourceType.Vegetables))
        {
            ResourceManager.Instance.Add(ResourceType.Vegetables, 1);
            hasReceivedKeyReward.Add(ResourceType.Vegetables);
        }

        if (Keyboard.current.digit0Key.wasPressedThisFrame && !hasReceivedKeyReward.Contains(ResourceType.Berries))
        {
            ResourceManager.Instance.Add(ResourceType.Berries, 1);
            hasReceivedKeyReward.Add(ResourceType.Berries);
        }
    }

    void HandleSpecial()
    {
        if (Keyboard.current.jKey.wasPressedThisFrame && !hasReceivedJReward)
        {
            ResourceManager.Instance.Add(ResourceType.Vegetables, 1);
            hasReceivedJReward = true;
        }
    }
}