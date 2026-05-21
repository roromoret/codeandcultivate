using System.Collections.Generic;
using UnityEngine;

//Class used for the creation and call of fucntions
public static class FunctionRegistry
{
    private static Dictionary<string, ColumnExecutor> registry = new Dictionary<string, ColumnExecutor>();
    
    public static int currentCallDepth = 0;
    public const int MAX_CALL_DEPTH = 50;

    public static event System.Action OnRegistryChanged;

    public static bool RegisterFunction(string name, ColumnExecutor executor)
    {
        if (string.IsNullOrEmpty(name)) return false;
        
        if (registry.ContainsKey(name)) return false;

        registry[name] = executor;
        OnRegistryChanged?.Invoke();
        return true;
    }

    public static void UnregisterFunction(string name)
    {
        if (registry.ContainsKey(name))
        {
            registry.Remove(name);
            OnRegistryChanged?.Invoke();
        }
    }

    public static ColumnExecutor GetExecutor(string name)
    {
        if (registry.ContainsKey(name)) return registry[name];
        return null;
    }

    public static List<string> GetFunctionNames()
    {
        return new List<string>(registry.Keys);
    }
}