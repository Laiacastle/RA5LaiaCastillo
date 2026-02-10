using UnityEngine;
using UnityEngine.InputSystem;
public class ThirdPCameraManager : MonoBehaviour
{
    [Header("Target")]
    public Transform target;
    public Vector3 offset = new Vector3(0f, 2f, -4f);

    [Header("Mouse")]
    public float sensitivity = 0.1f;
    public float minPitch = -30f;
    public float maxPitch = 70f;

    [Header("Rotation")]
    public float characterRotationSpeed = 10f;

    private float yaw;
    private float pitch;

    [SerializeField] GameManager _gameMan;
    private bool isPaused;

    private void Awake()
    {
        _gameMan = GameObject.FindWithTag("Manage").GetComponent<GameManager>();
    }

    private void OnEnable()
    {
        if(_gameMan != null)
        {
            _gameMan.PauseEvent += TogglePause;
        }
    }
    private void OnDisable()
    {
        if (_gameMan != null)
        {
            _gameMan.PauseEvent -= TogglePause;
        }
    }
    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        
        yaw = target.eulerAngles.y;
    }

    void LateUpdate()
    {
        if (isPaused) return;
        HandleMouse();
        UpdateCameraPosition();
        RotateCharacter();
    }

    void HandleMouse()
    {
        
        if (Mouse.current == null) return;

        Vector2 mouseDelta = Mouse.current.delta.ReadValue();

        yaw += mouseDelta.x * sensitivity;
        pitch -= mouseDelta.y * sensitivity;
        pitch = Mathf.Clamp(pitch, minPitch, maxPitch);
    }

    void UpdateCameraPosition()
    {
        Quaternion rotation = Quaternion.Euler(pitch, yaw, 0f);
        Vector3 desiredPosition = target.position + rotation * offset;

        transform.position = desiredPosition;
        transform.LookAt(target.position + Vector3.up * 1.5f);
    }

    void RotateCharacter()
    {
        Quaternion targetRotation = Quaternion.Euler(0f, yaw, 0f);
        target.rotation = Quaternion.Slerp(
            target.rotation,
            targetRotation,
            Time.deltaTime * characterRotationSpeed
        );
    }

    private void TogglePause(bool pause)
    {
        isPaused = pause;
    }
}
