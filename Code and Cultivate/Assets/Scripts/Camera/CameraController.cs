using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;

public class CameraController : MonoBehaviour
{
    [Header("Movement")]
    public float moveSpeed = 10f;
    public float fastMoveMultiplier = 2f; // Multiplier for fast movement when holding down a key - Right now I have it binded as Left Shift

    [Header("Zoom")]    // Uses FOV
    public float minZoom = 10f;
    public float maxZoom = 40f;
    public float zoomIncrement = 5f;

    [Header("Smoothing")]
    public float moveSmoothTime = 0.4f;
    public float zoomSmoothTime = 0.4f;

    [Header("Follow Mode")]
    [SerializeField] private TMP_Text followLabel; // HUD text

    // internal state
    private Vector3 _targetPosition;
    private Vector3 _moveVelocity;
    private float _targetFOV;
    private float _zoomVelocity;

    private Camera _cam;

    // null = freecam, non-null = follow-mode
    private Transform   _followTarget;
    private string      _followTargetName;


    // Unity cycle
    void OnEnable() => FarmerSelectable.OnFarmerClicked += HandleFarmerClicked;
    void OnDisable() => FarmerSelectable.OnFarmerClicked -= HandleFarmerClicked;

    void Start()
    {
        _cam = GetComponent<Camera>();
        _targetPosition = transform.position;

        _cam.fieldOfView = SnapToStep(Mathf.Clamp(_cam.fieldOfView, minZoom, maxZoom));
        _targetFOV = _cam.fieldOfView;

        SetFollowLabel(null);
    }

    void Update()
    {
        if (_followTarget != null) TrackFollowTarget();
        else Movement();

        CheckForDeselect();
        Zoom();
    }



    // Public API also used by tests

    public Transform FollowTarget => _followTarget;

    public void SetFollowTarget(Transform target, string displayName = null)
    {
        _followTarget       = target;
        _followTargetName   = target != null ? (displayName ?? target.name) : null;
        SetFollowLabel(_followTargetName);

        if (target == null) _targetPosition = transform.position;
    }


    // private

    private void Movement() // WASD to move camera
    {
        var keyboard = Keyboard.current;
        if (keyboard == null) return;

        float speed = moveSpeed * (keyboard.leftShiftKey.isPressed ? fastMoveMultiplier : 1f);

        Vector3 input = Vector3.zero;

        if (keyboard.wKey.isPressed) input += Vector3.forward;
        if (keyboard.sKey.isPressed) input += Vector3.back;
        if (keyboard.aKey.isPressed) input += Vector3.left;
        if (keyboard.dKey.isPressed) input += Vector3.right;
 
        // Normalize so diagonal movement isn't faster
        if (input.magnitude > 1f)
            input.Normalize();

        _targetPosition += input * speed * Time.deltaTime;

        // Smooth movement
        transform.position = Vector3.SmoothDamp(
            transform.position,
            _targetPosition,
            ref _moveVelocity,
            moveSmoothTime
        );
    }

    private void Zoom()
    {
        var mouse = Mouse.current;
        if (mouse == null) return;

        float scroll = mouse.scroll.ReadValue().y;
        if (scroll != 0f)
        {  
            _targetFOV += scroll > 0f ? -zoomIncrement : zoomIncrement;
            _targetFOV = SnapToStep(Mathf.Clamp(_targetFOV, minZoom, maxZoom));
        }

        _cam.fieldOfView = Mathf.SmoothDamp(
            _cam.fieldOfView,
            _targetFOV,
            ref _zoomVelocity,
            zoomSmoothTime
        );
    }

    private float SnapToStep(float value)
    {
        return Mathf.Round(value / zoomIncrement) * zoomIncrement;
    }

    private void HandleFarmerClicked(FarmerSelectable farmer)
    {
        Farmer farmerComponent = farmer.GetComponent<Farmer>();
        string name = farmerComponent != null ? farmerComponent.FarmerName : farmer.name;

        SetFollowTarget(farmer.transform, name);
    }

    private void CheckForDeselect()
    {
        var mouse = Mouse.current;
        if (mouse == null || !mouse.leftButton.wasPressedThisFrame) return;

        Ray ray = _cam.ScreenPointToRay(mouse.position.ReadValue());
        if (Physics.Raycast(ray, out RaycastHit hit))
        {
            if (hit.collider.GetComponent<FarmerSelectable>() != null) return; // ignore farmer click
        }

        if (_followTarget != null) SetFollowTarget(null);
    }

    private void TrackFollowTarget()
    {
        Vector3 desiredPos = new Vector3(
            _followTarget.position.x,
            transform.position.y,
            _followTarget.position.z
        );

        _targetPosition = desiredPos;

        transform.position = Vector3.SmoothDamp(
            transform.position,
            _targetPosition,
            ref _moveVelocity,
            moveSmoothTime
        );
    }

    private void SetFollowLabel(string farmerName)
    {
        if (followLabel == null) return;
 
        if (string.IsNullOrEmpty(farmerName))
        {
            followLabel.gameObject.SetActive(false);
        }
        else
        {
            followLabel.text = $"Following {farmerName}";
            followLabel.gameObject.SetActive(true);
        }
    }

}
