using UnityEngine;
using System;

public class TurnManager : MonoBehaviour
{
    public static TurnManager Instance { get; private set; }
    
    public int turnCount { get; private set; } = 0;

    public event Action<int> OnTurnPassed;

    public float timeBetweenTurns = 10f;
    
    private float _timer = 0f;

    private void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;
        Debug.Log("[TurnManager] Instance set successfully");
    }

    private void Update()
    {
        _timer += Time.deltaTime;

        if (_timer >= timeBetweenTurns)
        {
            _timer -= timeBetweenTurns; 
            PassTurn();
        }
    }

    public void PassTurn()
    {
        turnCount++;
        Debug.Log($"[TurnManager] Passed turn: currently turn {turnCount}");
        OnTurnPassed?.Invoke(turnCount);
    }
}