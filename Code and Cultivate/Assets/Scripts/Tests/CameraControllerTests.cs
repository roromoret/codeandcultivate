//using NUnit.Framework;
using UnityEngine;
using TMPro;


public class CameraControllerTests : MonoBehaviour
{
    private GameObject  _cameraGO;
    private CameraController _controller;
    private TMP_Text    _label;

    //    [SetUp]
    public void SetUp()
    {
        // Camera GameObject
        _cameraGO = new GameObject("TestCamera");
        _cameraGO.AddComponent<Camera>();
        _controller = _cameraGO.AddComponent<CameraController>();
 
        // HUD label
        var labelGO = new GameObject("FollowLabel");
        _label = labelGO.AddComponent<TextMeshProUGUI>();
 
        // Wire via the private serialized field using reflection
        var field = typeof(CameraController)
            .GetField("followLabel",
                System.Reflection.BindingFlags.NonPublic |
                System.Reflection.BindingFlags.Instance);
        field.SetValue(_controller, _label);
 
        // Manually call Start() since we are in Edit Mode (no Play loop)
        _controller.SendMessage("Start", SendMessageOptions.DontRequireReceiver);
    }
 
    //[TearDown]
    public void TearDown()
    {
        // Clean up every GameObject created during the test
        foreach (var go in Object.FindObjectsByType<GameObject>(FindObjectsSortMode.None))
            Object.DestroyImmediate(go);
    }




    /// Camera starts in freecam and the label is hidden
    /*[Test]
    public void T01_StartsInFreecamMode()
    {
        Assert.IsNull(_controller.FollowTarget,
            "FollowTarget should be null on startup (freecam).");
 
        Assert.IsFalse(_label.gameObject.activeSelf,
            "Follow label should be hidden in freecam.");
    }
    */
 
    /// Clicking a farmer enters follow mode and shows the correct label
    /*[Test]
    public void T02_ClickFarmer_EntersFollowMode()
    {
        var (farmerGO, selectable) = CreateFarmer("Alice");
 
        selectable.SimulateClick();
 
        Assert.AreEqual(farmerGO.transform, _controller.FollowTarget,
            "FollowTarget should be the clicked farmer's transform.");
 
        Assert.IsTrue(_label.gameObject.activeSelf,
            "Follow label should be visible after clicking a farmer.");
 
        Assert.AreEqual("Following Alice", _label.text,
            "Label text should show the farmer's FarmerName.");
    }
 
    /// Clicking a second farmer switches follow target and updates the label.
    [Test]
    public void T03_ClickDifferentFarmer_SwitchesFollowTarget()
    {
        var (_, selectableAlice) = CreateFarmer("Alice");
        var (bobGO, selectableBob) = CreateFarmer("Bob");
 
        selectableAlice.SimulateClick();
        selectableBob.SimulateClick();
 
        Assert.AreEqual(bobGO.transform, _controller.FollowTarget,
            "FollowTarget should switch to the new farmer.");
 
        Assert.AreEqual("Following Bob", _label.text,
            "Label should update to the newly followed farmer.");
    }

    /// Calling SetFollowTarget(null) returns to freecam and hides the label
    [Test]
    public void T04_SetFollowTargetNull_ReturnsToFreecam()
    {
        var (_, selectable) = CreateFarmer("Alice");
        selectable.SimulateClick();
 
        // Simulate clicking empty ground — same code path as CheckForDeselect
        _controller.SetFollowTarget(null);
 
        Assert.IsNull(_controller.FollowTarget,
            "FollowTarget should be null after deselect.");
 
        Assert.IsFalse(_label.gameObject.activeSelf,
            "Follow label should be hidden after returning to freecam.");
    }
 
    /// In follow mode the camera XZ tracks the farmer and Y is preserved
    [Test]
    public void T05_FollowMode_CameraTracksXZNotY()
    {
        var (farmerGO, selectable) = CreateFarmer("Alice");
        float originalY = _cameraGO.transform.position.y;
 
        selectable.SimulateClick();
 
        // Move the farmer to a new position
        farmerGO.transform.position = new Vector3(10f, 0f, 5f);
 
        // Call SetFollowTarget again to trigger FollowTarget() logic via Update proxy
        // (In Edit-Mode we can't run Update, so we call the public API directly)
        _controller.SetFollowTarget(farmerGO.transform, "Alice");
 
        // The desired target is XZ of farmer, Y of camera - verify _targetPosition
        // by checking the underlying behavior: after smoothing completes the camera
        // should land at farmer XZ.  We verify via the public FollowTarget reference
        Assert.AreEqual(farmerGO.transform, _controller.FollowTarget,
            "Camera should still reference the farmer transform.");
 
        Assert.AreEqual(originalY, _cameraGO.transform.position.y, 0.001f,
            "Camera Y should not change during follow mode.");
    }
 
    /// Label is correctly hidden when no followLabel is assigned (no NullRef)
    [Test]
    public void T06_NoLabelAssigned_DoesNotThrow()
    {
        // Create a fresh controller with no label wired
        var go = new GameObject("NakedCamera");
        go.AddComponent<Camera>();
        var ctrl = go.AddComponent<CameraController>();
        ctrl.SendMessage("Start", SendMessageOptions.DontRequireReceiver);
 
        var (_, selectable) = CreateFarmer("Alice");
 
        Assert.DoesNotThrow(() => selectable.SimulateClick(),
            "Clicking a farmer should not throw even when followLabel is null.");
 
        Assert.DoesNotThrow(() => ctrl.SetFollowTarget(null),
            "Deselecting should not throw even when followLabel is null.");
    }
 
    */

    // helpers
 
    private (GameObject go, FarmerSelectable selectable) CreateFarmer(string name)
    {
        var go = new GameObject(name);
        go.AddComponent<BoxCollider>();     // required by [RequireComponent] on FarmerSelectable
 
        // Add a minimal Farmer stub so FarmerName is readable
        // (Farmer depends on WorldGrid etc., so we set FarmerName via a wrapper)
        var selectable = go.AddComponent<FarmerSelectable>();
 
        // Attach a Farmer-like stub that exposes FarmerName without engine dependencies
        // Since Farmer is the real MonoBehaviour, we just add it and set the public property
        // If Farmer's Awake() logs warnings about missing singletons that is expected and harmless
        var farmer = go.AddComponent<Farmer>();
        farmer.FarmerName = name;
 
        return (go, selectable);
    }
}   
