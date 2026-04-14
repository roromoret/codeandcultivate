using UnityEngine;
using System.Collections;

// SPRINT 1 ONLY attach to any GameObject, run the scene, check Console, then delete
// Tests ResourceManager.Add, Spend, Get, CanAfford and the OnResourceChanged event

public class ResourceManagerTest : MonoBehaviour
{
    private int _eventFireCount = 0;
    private ResourceType _lastEventType;
    private int _lastEventAmount;

    private void Start()
    {
        StartCoroutine(RunTests());
    }

    private IEnumerator RunTests()
    {
        yield return new WaitUntil(() => ResourceManager.Instance != null);
        if (ResourceManager.Instance == null)
        {
            Debug.LogError("[ResourceManagerTest] ResourceManager.Instance is still null.");
            yield break;
        }

        ResourceManager.Instance.OnResourceChanged += OnResourceChangedHandler;

        Debug.Log("========== ResourceManager Tests ==========");

        TestInitialState();
        TestAddSingleResource();
        TestAddMultipleResources();
        TestAddInvalidAmount();
        TestSpendSuccess();
        TestSpendInsufficientFunds();
        TestSpendExactAmount();
        TestCanAfford();
        TestEventFires();
        TestSpendReturnValue();
        TestAllResourceTypesExist();

        Debug.Log("========== Tests Complete ==========");

        ResourceManager.Instance.OnResourceChanged -= OnResourceChangedHandler;
    }

    private void OnResourceChangedHandler(ResourceType type, int newAmount)
    {
        _eventFireCount++;
        _lastEventType   = type;
        _lastEventAmount = newAmount;
    }

    // Tests

    // All resources should exist and start at their configured amounts
    // If starting amounts are 0 in your ResourceConfig, all should log 0
    private void TestInitialState()
    {
        Debug.Log("--- TestInitialState ---");
        foreach (ResourceType type in System.Enum.GetValues(typeof(ResourceType)))
        {
            int amount = ResourceManager.Instance.Get(type);
            Debug.Log($"  Initial {type}: {amount}");
        }
        Pass("Initial state readable for all resource types.");
    }

    // Adding a positive amount should increase the total correctly
    private void TestAddSingleResource()
    {
        Debug.Log("--- TestAddSingleResource ---");

        int before = ResourceManager.Instance.Get(ResourceType.Fruits);
        ResourceManager.Instance.Add(ResourceType.Fruits, 5);
        int after = ResourceManager.Instance.Get(ResourceType.Fruits);

        AssertEqual("Add 5 Fruits", before + 5, after);
    }

    // Adding to multiple resources should not cross-contaminate values
    private void TestAddMultipleResources()
    {
        Debug.Log("--- TestAddMultipleResources ---");

        int beforeCoins = ResourceManager.Instance.Get(ResourceType.Money);
        int beforeBerry = ResourceManager.Instance.Get(ResourceType.Berries);

        ResourceManager.Instance.Add(ResourceType.Money,  10);
        ResourceManager.Instance.Add(ResourceType.Berries, 3);

        AssertEqual("Money unaffected by Berry add",
            beforeCoins + 10, ResourceManager.Instance.Get(ResourceType.Money));

        AssertEqual("Berries unaffected by Money add",
            beforeBerry + 3, ResourceManager.Instance.Get(ResourceType.Berries));
    }

    // Adding zero or a negative amount should be rejected with a warning
    // The resource total should not change
    private void TestAddInvalidAmount()
    {
        Debug.Log("--- TestAddInvalidAmount ---");

        int before = ResourceManager.Instance.Get(ResourceType.Vegetables);
        ResourceManager.Instance.Add(ResourceType.Vegetables, 0);
        ResourceManager.Instance.Add(ResourceType.Vegetables, -5);
        int after = ResourceManager.Instance.Get(ResourceType.Vegetables);

        AssertEqual("Invalid Add does not change total", before, after);
    }

    // Spending an amount the player can afford should reduce the total
    private void TestSpendSuccess()
    {
        Debug.Log("--- TestSpendSuccess ---");

        ResourceManager.Instance.Add(ResourceType.Money, 20);
        int before = ResourceManager.Instance.Get(ResourceType.Money);

        bool result = ResourceManager.Instance.Spend(ResourceType.Money, 8);

        AssertTrue ("Spend returns true when affordable", result);
        AssertEqual("Money reduced correctly", before - 8,
            ResourceManager.Instance.Get(ResourceType.Money));
    }

    // Spending more than available should fail and leave the total unchanged
    private void TestSpendInsufficientFunds()
    {
        Debug.Log("--- TestSpendInsufficientFunds ---");

        // Force a known state
        int current = ResourceManager.Instance.Get(ResourceType.Fruits);
        if (current > 0)
            ResourceManager.Instance.Spend(ResourceType.Fruits, current);

        ResourceManager.Instance.Add(ResourceType.Fruits, 3);
        int before = ResourceManager.Instance.Get(ResourceType.Fruits);

        bool result = ResourceManager.Instance.Spend(ResourceType.Fruits, 99);

        AssertFalse("Spend returns false when unaffordable", result);
        AssertEqual("Fruits unchanged after failed spend", before,
            ResourceManager.Instance.Get(ResourceType.Fruits));
    }

    // Spending exactly the full amount should leave the resource at zero (not negative)
    private void TestSpendExactAmount()
    {
        Debug.Log("--- TestSpendExactAmount ---");

        ResourceManager.Instance.Add(ResourceType.Berries, 5);
        int before = ResourceManager.Instance.Get(ResourceType.Berries);

        bool result = ResourceManager.Instance.Spend(ResourceType.Berries, before);

        AssertTrue ("Spend exact amount returns true", result);
        AssertEqual("Berries at zero after exact spend", 0,
            ResourceManager.Instance.Get(ResourceType.Berries));
    }

    // CanAfford should return true when funds are sufficient and false when not & without modifying the resource total
    private void TestCanAfford()
    {
        Debug.Log("--- TestCanAfford ---");

        ResourceManager.Instance.Add(ResourceType.Money, 10);
        int before = ResourceManager.Instance.Get(ResourceType.Money);

        AssertTrue ("CanAfford true when sufficient",
            ResourceManager.Instance.CanAfford(ResourceType.Money, 5));

        AssertFalse("CanAfford false when insufficient",
            ResourceManager.Instance.CanAfford(ResourceType.Money, 9999));

        AssertEqual("CanAfford does not modify total", before,
            ResourceManager.Instance.Get(ResourceType.Money));
    }

    // OnResourceChanged should fire exactly once per Add or successful Spend and carry the correct type and new value
    private void TestEventFires()
    {
        Debug.Log("--- TestEventFires ---");

        _eventFireCount = 0;

        ResourceManager.Instance.Add(ResourceType.Vegetables, 4);
        AssertEqual("Event fires once on Add", 1, _eventFireCount);
        AssertTrue ("Event carries correct type",
            _lastEventType == ResourceType.Vegetables);
        AssertEqual("Event carries correct new amount",
            ResourceManager.Instance.Get(ResourceType.Vegetables), _lastEventAmount);

        _eventFireCount = 0;
        ResourceManager.Instance.Spend(ResourceType.Vegetables, 2);
        AssertEqual("Event fires once on successful Spend", 1, _eventFireCount);

        _eventFireCount = 0;
        ResourceManager.Instance.Spend(ResourceType.Vegetables, 9999); // will fail
        AssertEqual("Event does not fire on failed Spend", 0, _eventFireCount);
    }

    // Spend's bool return value should correctly reflect success and failure
    private void TestSpendReturnValue()
    {
        Debug.Log("--- TestSpendReturnValue ---");

        ResourceManager.Instance.Add(ResourceType.Money, 50);

        AssertTrue ("Spend returns true when funds available",
            ResourceManager.Instance.Spend(ResourceType.Money, 10));

        AssertFalse("Spend returns false when funds unavailable",
            ResourceManager.Instance.Spend(ResourceType.Money, 99999));
    }

    // Get should return 0 for every ResourceType without throwing confirming the dictionary is seeded for all enum values
    private void TestAllResourceTypesExist()
    {
        Debug.Log("--- TestAllResourceTypesExist ---");

        bool allExist = true;
        foreach (ResourceType type in System.Enum.GetValues(typeof(ResourceType)))
        {
            int val = ResourceManager.Instance.Get(type);
            if (val < 0)
            {
                Fail($"{type} returned negative value: {val}");
                allExist = false;
            }
        }

        if (allExist)
            Pass("All ResourceType entries exist in dictionary and are non-negative.");
    }

    // Assertion helpers
    private void AssertEqual(string label, int expected, int actual)
    {
        if (expected == actual)
            Pass($"{label} — expected {expected}, got {actual}");
        else
            Fail($"{label} — expected {expected}, got {actual}");
    }

    private void AssertTrue(string label, bool condition)
    {
        if (condition) Pass(label);
        else           Fail($"{label} — expected true, got false");
    }

    private void AssertFalse(string label, bool condition)
    {
        if (!condition) Pass(label);
        else            Fail($"{label} — expected false, got true");
    }

    private void Pass(string label) =>
        Debug.Log($"  <color=green>PASS</color> {label}");

    private void Fail(string label) =>
        Debug.LogError($"  FAIL {label}");
}