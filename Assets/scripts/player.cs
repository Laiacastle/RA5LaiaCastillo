using EasyDoorSystem;
using System;
using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.InputSystem;

public class Player : MonoBehaviour, InputSystem_Actions.IPlayerActions
{
    [SerializeField] private AnimationBehaviour _aB;
    [SerializeField] private float speed = 5f;
    
    [SerializeField] private Vector3 moveInput;
    [SerializeField] private Vector3 moveDirection;
    [SerializeField] private CharacterController _Cc;
     private InputSystem_Actions inputActions;
    [SerializeField] private MoveBehaviour _mB;
    [SerializeField] public bool isSprinting = false;
    private Vector3 direction;
    public bool dancing = false;
    public bool aiming = false;
    public event Action<bool> DanceEvent = delegate { };
    public event Action<bool> AimEvent = delegate { };
    public event Action NewSceneEvent = delegate { };
    public static Player instance;
    [SerializeField] private Bullet bullet;
    [SerializeField] private CinemachineThirdPersonFollow playerCamera;

    [SerializeField] public bool hasKey;

    void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }
        instance = this;
        DontDestroyOnLoad(gameObject);
        _mB = GetComponent<MoveBehaviour>();
        _aB = GetComponent<AnimationBehaviour>();
        _Cc = GetComponent<CharacterController>();
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }
    public void OnAttack(InputAction.CallbackContext context)
    {
        if (aiming && context.canceled)
        {
            Instantiate(bullet, GameObject.FindWithTag("CollectablePos").transform.position, Quaternion.LookRotation(playerCamera.transform.forward));
        }
    }

    public void OnCrouch(InputAction.CallbackContext context)
    {
        Debug.Log("Not implemented.");
    }
    public void OnWinDance()
    {
       direction = Vector3.zero;
       _aB.Dance();
       dancing = !dancing;

       DanceEvent.Invoke(dancing);

    }

    public void OnInteract(InputAction.CallbackContext context)
    {

        if (context.canceled && !aiming)
        {
            direction = Vector3.zero;
            _aB.Dance();
            dancing = !dancing;
            
           DanceEvent.Invoke(dancing);
            
        }
    }

    public void OnJump(InputAction.CallbackContext context)
    {
        if (!context.performed || dancing) return;
        if (_Cc.isGrounded)
        {
            _mB.Jump();
            _aB.Jump(true);
        }
    }

    public void OnLook(InputAction.CallbackContext context)
    {
        Debug.Log("Not implemented.");
    }

    public void OnMove(InputAction.CallbackContext context)
    {
        if (dancing) return;

        Vector2 input = context.ReadValue<Vector2>();
        direction = new Vector3(input.x, 0f, input.y);

    }

    public void OnSprint(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            isSprinting = true;
        }
        if (context.canceled)
        {
            isSprinting = false;
        }
    }
    void OnEnable()
    {
        if (inputActions == null)
        {
            inputActions = new InputSystem_Actions();
            inputActions.Player.SetCallbacks(this);
        }
        inputActions.Player.Enable();
        if (instance != null)
            instance.DanceEvent += DanceEvent;
    }
    void OnDisable()
    {
        if (inputActions != null)
            inputActions.Player.Disable();
        if (instance != null)
            instance.DanceEvent -= DanceEvent;
    }

    // Update is called once per frame
    void Update()
    {
        
        _mB.Move(direction, isSprinting);
        _aB.Move(direction, isSprinting);
    }
    void LateUpdate()
    {
        if (_Cc.isGrounded) _aB.Jump(false);
    }

    public void OnAim(InputAction.CallbackContext context)
    {
        if (context.canceled )
        {
            _aB.Aim();
            aiming = !aiming;

            AimEvent.Invoke(aiming);

        }
    }

    public void OnTriggerEnter(Collider collision)
    {
        if (collision.gameObject.tag == "Puerta")
        {
            Puerta puerta = collision.gameObject.GetComponentInChildren<Puerta>();
            puerta.OpenDoor();
        }
    }

    public void OnUseObject(InputAction.CallbackContext context)
    {
        if (hasKey)
        {
            gameObject.GetComponentInChildren<Item>().Use(this);
        }
    }

    public void OnDrop(InputAction.CallbackContext context)
    {
        if (hasKey)
        {
            hasKey = false;
            gameObject.GetComponentInChildren<Item>().Drop();
        }
    }

    public void OnExit(InputAction.CallbackContext context)
    {
        if (context.canceled)
            GameObject.FindWithTag("Manage").GetComponent<GameManager>().TogglePause();
    }

    void OnDestroy()
    {
        if (instance != null && instance.inputActions != null)
        {
            instance.inputActions.Player.Exit.Disable();
        }
    }
}
