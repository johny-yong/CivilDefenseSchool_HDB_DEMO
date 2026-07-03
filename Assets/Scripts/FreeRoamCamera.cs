using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;

public class FreeRoamCamera : MonoBehaviour
{
    [SerializeField] private InputActionReference _look;
    [SerializeField] private InputActionReference _move;
    [SerializeField] private InputActionReference _upDown; // optional, for vertical fly

    [Header("Movement")]
    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private float moveSmoothTime = 0.15f; // higher = more glide/inertia

    [Header("Look")]
    [SerializeField] private float lookSpeed = 2f;
    [SerializeField] private float lookSmoothTime = 0.08f; // higher = smoother but laggier

    [Header("Bounds (set to match interior)")]
    [SerializeField] private Vector3 boundsCenter;
    [SerializeField] private Vector3 boundsExtents = new Vector3(10f, 3f, 10f);

    [SerializeField] private PlayerController _pController;

    private Vector3 _initialPosition;
    private Quaternion _initialRotation;

    private float _pitch;
    private float _yaw;

    [SerializeField] private GameObject UI;

    [SerializeField] private TextMeshProUGUI QualityText;

    // Smoothing state
    private Vector2 _currentLookInput;
    private Vector2 _lookVelocity;
    private Vector3 _currentMoveVelocity;

    void Start()
    {
        _initialPosition = transform.position;
        _initialRotation = transform.rotation;
        _yaw = transform.eulerAngles.y;
        _pitch = 0f;

        if (_look) _look.action.Enable();
        if (_move) _move.action.Enable();
        if (_upDown) _upDown.action.Enable();

        int current = QualitySettings.GetQualityLevel();
        QualityText.text = "Quality Level: " + QualitySettings.names[current];
    }

    void Update()
    {
        #region QuickDebugControls
        if (Input.GetKeyDown(KeyCode.F1))
        {
            transform.position = _initialPosition;
            transform.rotation = _initialRotation;
            _yaw = _initialRotation.eulerAngles.y;
            _pitch = 0f;
            _currentLookInput = Vector2.zero;
            _lookVelocity = Vector2.zero;
            _currentMoveVelocity = Vector3.zero;
            _pController.playerMode = !_pController.playerMode; //Toggle
        }

        if (Input.GetKeyDown(KeyCode.F6))
        {
            UI.SetActive(!UI.activeSelf);
        }

        if (Input.GetKeyDown(KeyCode.F3))
        {
            CycleQuality();
        }
        #endregion


        if (_pController.playerMode)
        {
            return;
        }

        if (_look)
        {
            Vector2 targetLookInput = _look.action.ReadValue<Vector2>();

            _currentLookInput = Vector2.SmoothDamp(
                _currentLookInput, targetLookInput, ref _lookVelocity, lookSmoothTime);

            _yaw += _currentLookInput.x * lookSpeed;
            _pitch -= _currentLookInput.y * lookSpeed;
            _pitch = Mathf.Clamp(_pitch, -89f, 89f);

            transform.rotation = Quaternion.Euler(_pitch, _yaw, 0f);
        }

        Vector3 targetMoveDir = Vector3.zero;
        if (_move)
        {
            Vector2 moveInput = _move.action.ReadValue<Vector2>();
            targetMoveDir += transform.right * moveInput.x + transform.forward * moveInput.y;
        }

        Vector3 targetVelocity = targetMoveDir.normalized * moveSpeed;
        _currentMoveVelocity = Vector3.Lerp(
            _currentMoveVelocity, targetVelocity, Time.deltaTime / Mathf.Max(moveSmoothTime, 0.0001f));

        Vector3 newPos = transform.position + _currentMoveVelocity * Time.deltaTime;

        newPos.x = Mathf.Clamp(newPos.x, boundsCenter.x - boundsExtents.x, boundsCenter.x + boundsExtents.x);
        newPos.y = Mathf.Clamp(newPos.y, boundsCenter.y - boundsExtents.y, boundsCenter.y + boundsExtents.y);
        newPos.z = Mathf.Clamp(newPos.z, boundsCenter.z - boundsExtents.z, boundsCenter.z + boundsExtents.z);

        transform.position = newPos;
        transform.position = newPos;
    }



    private void CycleQuality()
    {
        int current = QualitySettings.GetQualityLevel();
        int next = (current + 1) % QualitySettings.names.Length; // wraps back to 0 after the last level

        QualitySettings.SetQualityLevel(next, true);
        QualityText.text = "Quality Level: " + QualitySettings.names[current];
        Debug.Log($"Quality set to: {QualitySettings.names[next]}");
    }
}