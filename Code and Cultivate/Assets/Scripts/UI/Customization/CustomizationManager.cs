using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class CustomizationManager : MonoBehaviour
{
    public GameObject windowPanel;
    public GameObject openMenuButton;
    public GameObject exitMenuButton;
    public GameObject extraBackground;

    public TMP_Dropdown farmerDropdown;
    public Transform slidersContainer;
    
    public GameObject partLinePrefab;

    private List<Farmer> _availableFarmers = new List<Farmer>();

    void Start()
    {
        if (windowPanel != null) windowPanel.SetActive(false);     
        if (openMenuButton != null) openMenuButton.SetActive(true); 
        if (exitMenuButton != null) exitMenuButton.SetActive(false); 
        if (extraBackground != null) extraBackground.SetActive(false);
    }

    public void OpenCustomizationMenu()
    {
        if (windowPanel != null) windowPanel.SetActive(true);       
        if (openMenuButton != null) openMenuButton.SetActive(false); 
        if (exitMenuButton != null) exitMenuButton.SetActive(true);   
        if (extraBackground != null) extraBackground.SetActive(true);

        RefreshDropdown();
    }

    public void CloseCustomizationMenu()
    {
        if (windowPanel != null) windowPanel.SetActive(false);      
        if (openMenuButton != null) openMenuButton.SetActive(true);  
        if (exitMenuButton != null) exitMenuButton.SetActive(false); 
        if (extraBackground != null) extraBackground.SetActive(false);
    }

    public void RefreshDropdown()
    {
        if (FarmerAssignment.Instance == null) return;
        _availableFarmers = FarmerAssignment.Instance.GetAvailableFarmers(null); 
        farmerDropdown.ClearOptions();
        List<string> labels = new List<string>();
        foreach (Farmer farmer in _availableFarmers) { labels.Add(farmer.gameObject.name); }
        if (labels.Count > 0)
        {
            farmerDropdown.AddOptions(labels);
            farmerDropdown.onValueChanged.RemoveAllListeners();
            farmerDropdown.onValueChanged.AddListener(OnDropdownChanged);
            farmerDropdown.value = 0;
            OnDropdownChanged(0);
        }
        else { farmerDropdown.AddOptions(new List<string> { "(No Farmer found)" }); }
    }

    private void OnDropdownChanged(int index)
    {
        if (index < 0 || index >= _availableFarmers.Count) return;
        BuildSlidersForFarmer(_availableFarmers[index]);
    }

    private void BuildSlidersForFarmer(Farmer farmer)
    {
        foreach (Transform child in slidersContainer) { Destroy(child.gameObject); }
        Transform visualContainer = farmer.transform.Find("Visual_Container");
        if (visualContainer == null) return;
        SpriteRenderer[] bodyParts = visualContainer.GetComponentsInChildren<SpriteRenderer>();
        foreach (SpriteRenderer part in bodyParts)
        {
            if (part.gameObject.name == "Visual_Container") continue; 
            GameObject newRow = Instantiate(partLinePrefab, slidersContainer);
            ColorSliderRow rowScript = newRow.GetComponent<ColorSliderRow>();
            if (rowScript != null) { rowScript.Setup(part); }
        }
    }
}