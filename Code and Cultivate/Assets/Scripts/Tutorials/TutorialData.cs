using System.IO.Enumeration;
using System.Security.AccessControl;
using UnityEngine;

[CreateAssetMenu(fileName = "Tutorial_New", menuName = "Code and Cultivate/Tutorial Data")]
public class TutorialData : ScriptableObject
{
    [Header("Identity")]
    public string id;   // eg: "tutorial_gamestart", "tutorial_shop"
    public string title;

    [TextArea(3,8)]
    public string body;

    [Header("Trigger")]
    public TutorialTrigger trigger;

    [Header("Threshold Trigger")] // for resource thresholds
    public ResourceType thresholdResource;
    public int          thresholdAmount;
}