using UnityEngine;
using System;

public class TurnManager : MonoBehaviour
{
    public static TurnManager Instance { get; private set; }
    
    public int turnCount { get; private set; } = 0;

    public event Action<int> OnTurnPassed;

    private void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;
        Debug.Log("[TurnManager] Instance set successfully");
    }

    public void PassTurn()
    {
        turnCount++;
        Debug.Log($"[TurnManagher] Passed turn: currently turn {turnCount}");
        OnTurnPassed?.Invoke(turnCount);
    }
}
