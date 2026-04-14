using UnityEngine;

[CreateAssetMenu(fileName = "ResourceConfig", menuName = "Code and Cultivate/Resource Config")]
public class ResourceConfig : ScriptableObject
{
    [System.Serializable]
    public class ResourceEntry
    {
        public ResourceType type;
        public string       displayName;
        public Sprite       icon;
        public int          startingAmount;
    }

    public ResourceEntry[] resources;

    public System.Collections.Generic.Dictionary<ResourceType, ResourceEntry> BuildLookup()
    {
        var dict = new System.Collections.Generic.Dictionary<ResourceType, ResourceEntry>();
        foreach (ResourceEntry entry in resources) dict[entry.type] = entry;
        return dict;
    }
}