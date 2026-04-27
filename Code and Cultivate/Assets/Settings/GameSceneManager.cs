using UnityEngine;

public class GameSceneManager : MonoBehaviour
{
    private void Start()
    {
        // Check if there's pending save data to load
        if (SaveManager.Instance.HasPendingLoadData())
        {
            Debug.Log("[GameSceneManager] Applying loaded save data");
            SaveManager.Instance.ApplyLoadedData();
        }
        else
        {
            Debug.Log("[GameSceneManager] Starting new game with fresh resources");
            // ResourceManager will initialize with default values
        }
    }
}