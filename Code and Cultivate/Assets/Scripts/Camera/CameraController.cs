using Unity.VisualScripting;
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

    private Vector3 _targetPosition;
    private Vector3 _moveVelocity;
    private float _targetFOV;
    private float _zoomVelocity;

    private Camera _cam;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        _cam = GetComponent<Camera>();
        _targetPosition = transform.position;

        _cam.fieldOfView = SnapToStep(Mathf.Clamp(_cam.fieldOfView, minZoom, maxZoom));
        _targetFOV = _cam.fieldOfView;
    }

    // Update is called once per frame
    void Update()
    {
        Movement();
        Zoom();
    }

    void Movement() // WASD to move camera
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

    void Zoom()
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

    float SnapToStep(float value)
    {
        return Mathf.Round(value / zoomIncrement) * zoomIncrement;
    }
}
