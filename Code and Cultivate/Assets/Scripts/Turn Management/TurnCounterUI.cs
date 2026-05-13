using TMPro;
using Unity.VisualScripting;
using UnityEngine;

public class TurnCounterUI : MonoBehaviour
{
    [SerializeField] private TMP_Text turnLabel;

    private void OnEnable()
    {
        TurnManager.Instance.OnTurnPassed += UpdateLabel;
        UpdateLabel(TurnManager.Instance.turnCount); // sync on enable
    }

    private void OnDisable()
    {
        TurnManager.Instance.OnTurnPassed -= UpdateLabel;
    }

    private void UpdateLabel(int turn)
    {
        turnLabel.text = $"Turn {turn}";
    }
}
