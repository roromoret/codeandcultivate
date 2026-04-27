using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;

public class TutorialManager : MonoBehaviour
{
    public static TutorialManager Instance { get; private set; }

    [Header("All tutorials in the game - assign assets here")]
    [SerializeField] private List<TutorialData> allTutorials;

    [Header("Popup prefab")]
    [SerializeField] private TutorialPopup popupPrefab;
    [SerializeField] private Transform     uiParent;   // assign HUD Canvas

    // Seen state - persisted via SaveSystem in future 
    private HashSet<string> _seenIds = new();

    // Queue so tutorials never overlap
    private Queue<TutorialData> _queue = new();
    private bool                _isShowing;
    
    /*
        Init
    */
    private void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;
    }

    private void Start()
    {
        TriggerAllMatching(TutorialTrigger.OnGameStart);
    }

    /* 
        PUBLIC API
    */
    // Manually show a tutorial by ID (e.g. from a help button)
    public void Show(string tutorialId)
    {
        TutorialData data = allTutorials.Find(t => t.id == tutorialId);
        if (data == null)
        {
            Debug.LogWarning($"[TutorialManager] No tutorial found with id '{tutorialId}'.");
            return;
        }
        Enque(data);
    }

    // Show again regardless of seen state - future "view again" feature
    public void ShowAgain(string tutorialId)
    {
        TutorialData data = allTutorials.Find(t => t.id == tutorialId);
        if (data != null) Enque(data);
    }

    // Check and show a tutorial if its condition is met and it hasnt been seen
    public void TryTrigger(TutorialTrigger trigger, ResourceType resource = default, int amount = 0)
    {
        foreach (TutorialData tutorial in allTutorials)
        {
            if (tutorial.trigger != trigger)     continue;
            if (HasSeen(tutorial.id))            continue;

            if (trigger == TutorialTrigger.OnResourceThreshold)
            {
                if (tutorial.thresholdResource != resource) continue;
                if (amount < tutorial.thresholdAmount)      continue;
            }

            Enque(tutorial);
        }
    }

    public bool HasSeen(string tutorialId) => _seenIds.Contains(tutorialId);

    // Called by TutorialPopup
    public void MarkSeen(string tutorialId)
    {
        if (_seenIds.Add(tutorialId))
        {
            // TODO: Persist seen state in save system
            // and some other stuff too ig
        }
    }

    /*
        PRIVATE HELPERS
    */
    private void TriggerAllMatching(TutorialTrigger trigger)
    {
        foreach (TutorialData tutorial in allTutorials)
        {
            if (tutorial.trigger == trigger && !HasSeen(tutorial.id)) Enque(tutorial);
        }
    }

    private void Enque(TutorialData data)
    {
        _queue.Enqueue(data);
        if (!_isShowing) StartCoroutine(ProcessQueue());
    }

    private IEnumerator ProcessQueue()
    {
        _isShowing = true;

        while (_queue.Count > 0)
        {
            TutorialData next = _queue.Dequeue();

            TutorialPopup popup = Instantiate(popupPrefab, uiParent);
            popup.Show(next);

            // Wait until popup is closed before showing the next
            yield return new WaitUntil(() => popup == null);
        }

        _isShowing = false;
    }
}