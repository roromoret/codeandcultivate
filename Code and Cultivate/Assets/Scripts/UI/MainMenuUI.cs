using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class MainMenuUI : MonoBehaviour
{
    [System.Serializable]
    public class SaveSlotUI
    {
        public TextMeshProUGUI infoText;
        public Button actionButton;
        public TextMeshProUGUI buttonLabel;
        public Button deleteButton;
    }

    [SerializeField] private SaveSlotUI[] slots = new SaveSlotUI[SaveManager.MAX_SLOTS];
    [SerializeField] private Button quitButton;
    [SerializeField] private Button quickLoadButton;

    private void Start()
    {
        for (int i = 0; i < slots.Length; i++)
        {
            int slotNumber = i + 1;
            slots[i].actionButton.onClick.AddListener(() => OnSlotClicked(slotNumber));
            slots[i].deleteButton.onClick.AddListener(() => OnDeleteClicked(slotNumber));
        }

if (quickLoadButton != null)
            quickLoadButton.onClick.AddListener(OnQuickLoadClicked);

        quitButton.onClick.AddListener(OnQuitClicked);
        RefreshSlotUI();
    }

    private void RefreshSlotUI()
    {
        for (int i = 0; i < slots.Length; i++)
        {
            int slotNumber = i + 1;
            bool exists = SaveManager.Instance.SlotExists(slotNumber);

            slots[i].infoText.text = SaveManager.Instance.GetSlotInfo(slotNumber);
            slots[i].buttonLabel.text = exists ? "Load" : "New Game";
            slots[i].deleteButton.gameObject.SetActive(exists);
        }

        if (quickLoadButton != null)
            quickLoadButton.gameObject.SetActive(SaveManager.Instance.AnySaveExists());
    }

    private void OnSlotClicked(int slot)
    {
        if (SaveManager.Instance.SlotExists(slot))
        {
            Debug.Log($"[MainMenuUI] Loading slot {slot}");
            MenuManager.Instance.LoadGame(slot);
        }
        else
        {
            Debug.Log($"[MainMenuUI] New game in slot {slot}");
            MenuManager.Instance.NewGame(slot);
        }
    }

        private void OnQuickLoadClicked()
    {
        int slot = SaveManager.Instance.GetMostRecentSlot();
        if (slot == -1) return;

        Debug.Log($"[MainMenuUI] Quick loading most recent slot {slot}");
        MenuManager.Instance.LoadGame(slot);
    }



    private void OnDeleteClicked(int slot)
    {
        Debug.Log($"[MainMenuUI] Deleting slot {slot}");
        SaveManager.Instance.DeleteSlot(slot);
        RefreshSlotUI();
    }

    private void OnQuitClicked()
    {
        Debug.Log("[MainMenuUI] Quit button pressed");
        MenuManager.Instance.QuitGame();
    }

    private void OnDestroy()
    {
        for (int i = 0; i < slots.Length; i++)
        {
            if (slots[i].actionButton != null) slots[i].actionButton.onClick.RemoveAllListeners();
            if (slots[i].deleteButton != null) slots[i].deleteButton.onClick.RemoveAllListeners();
        }

        if (quitButton != null) quitButton.onClick.RemoveAllListeners();
        if (quickLoadButton != null) quickLoadButton.onClick.RemoveAllListeners();
    }
}
