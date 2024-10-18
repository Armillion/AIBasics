using DefaultNamespace;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour, IVehicle {
    [Header("Input")]
    [SerializeField]
    private InputActionReference _moveAction;
    
    [SerializeField]
    private InputActionReference _sprintAction;
    
    [SerializeField]
    private InputActionReference _lookAction;
    
    [Space]
    [SerializeField]
    private float _defaultMaxMoveSpeed = 5f;
    
    [SerializeField]
    private float _maxSprintMoveSpeed = 7.5f;
    
    [SerializeField]
    private float _accelerationSpeed = 10f;
    
    [Header("Aiming")]
    [SerializeField]
    private Transform _aimTransform;
    
    [SerializeField, Min(0f)]
    private float _maxAimDistance = 2f;

    public float MaxSpeed { get; private set; }
    public Vector2 Position => transform.position;
    public Vector2 Velocity { get; private set; }
    
    private Vector2 _acceleration;

    private void Start() {
        MaxSpeed = _defaultMaxMoveSpeed;
        _sprintAction.action.started += _ => MaxSpeed = _maxSprintMoveSpeed;
        _sprintAction.action.canceled += _ => MaxSpeed = _defaultMaxMoveSpeed;
    }

    private void Update() {
        Move();
        Look();
    }

    private void Move() {
        var moveInput = _moveAction.action.ReadValue<Vector2>();
        _acceleration = Vector2.Lerp(_acceleration, moveInput * MaxSpeed, _accelerationSpeed * Time.deltaTime);
        Velocity = Vector2.Lerp(Velocity, _acceleration, _accelerationSpeed * Time.deltaTime);
        transform.position += (Vector3)Velocity * Time.deltaTime;
    }

    private void Look() {
        var mousePos = _lookAction.action.ReadValue<Vector2>();
        Vector3 transformScreenPos = Camera.main!.WorldToScreenPoint(transform.position);
        Vector2 lookDir = (mousePos - (Vector2)transformScreenPos).normalized;
        float angle = Mathf.Atan2(lookDir.y, lookDir.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(0, 0, angle);
        
        Aim(transformScreenPos, mousePos);
    }

    private void Aim(Vector3 transformScreenPos, Vector2 mousePos) {
        float mousePosDistance = Vector2.Distance(transformScreenPos, mousePos);
        float aimExtensionFactor = Mathf.Lerp(0f, _maxAimDistance, mousePosDistance / (Screen.width * 0.5f));
        _aimTransform.localPosition = Vector3.right * (_maxAimDistance * aimExtensionFactor);
    }
}
