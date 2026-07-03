using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(CharacterController))]
public class PlayerController : MonoBehaviour
{
    [SerializeField] private InputActionReference _movement;
    [SerializeField] private InputActionReference _jump;
    [SerializeField] private InputActionReference _sprint;
    [SerializeField] private InputActionReference _look;

    public bool playerMode { get; set; }
    [SerializeField] public float playerSpeed = 3f;
    [SerializeField] private float gravity = -9.81f;

    [SerializeField] private float lookSpeed = 2f;

    private float _pitch;
    private float _yaw;

    private CharacterController _controller;
    private Vector3 _velocity;

    void Awake()
    {
        _controller = GetComponent<CharacterController>();
        _yaw = transform.eulerAngles.y;
    }

    void Start()
    {
        if (_movement) { _movement.action.Enable(); }
        if (_jump) { _jump.action.Enable(); }
        if (_sprint) { _sprint.action.Enable(); }
    }

    void Update()
    {
        if (!playerMode)
        {
            return;
        }

        if (_look)
        {
            Vector2 lookInput = _look.action.ReadValue<Vector2>();
            _yaw += lookInput.x * lookSpeed;
            _pitch -= lookInput.y * lookSpeed;
            _pitch = Mathf.Clamp(_pitch, -89f, 89f);

            transform.rotation = Quaternion.Euler(0f, _yaw, 0f); 
        }

        if (_movement)
        {
            Vector2 moveInput = _movement.action.ReadValue<Vector2>();
            Vector3 move = transform.right * moveInput.x + transform.forward * moveInput.y;
            _controller.Move(move * playerSpeed * Time.deltaTime);
        }

        _velocity.y += gravity * Time.deltaTime;
        _controller.Move(_velocity * Time.deltaTime);

        if (_controller.isGrounded && _velocity.y < 0)
        {
            _velocity.y = -2f;
        }
    }

    [SerializeField] private float cameraRadius = 0.2f;
    [SerializeField] private LayerMask wallMask;
    void LateUpdate()
    {
        Vector3[] checkDirections = { transform.forward, -transform.forward, transform.right, -transform.right, transform.up, -transform.up };

        foreach (var dir in checkDirections)
        {
            if (Physics.SphereCast(transform.position, cameraRadius, dir, out RaycastHit hit, cameraRadius, wallMask))
            {
                float penetration = cameraRadius - hit.distance;
                transform.position += -dir * penetration;
            }
        }
    }
}