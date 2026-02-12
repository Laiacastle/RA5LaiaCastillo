using System;
using System.Collections;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour, InputSystem_Actions.IGlobalActions
{
    [SerializeField] private Animator animator;
    [SerializeField] private float seconds = 1f;
    [SerializeField] private bool UNITY_EDITOR = true;
    [SerializeField] private SaveManager _sM;
    [SerializeField] private GameObject _pauseCanvas;
    public bool isPaused = false;
    public event Action<bool> PauseEvent = delegate { };
    public static GameManager instance;
    private InputSystem_Actions inputActions;

    void Awake()
    {
        GameObject transitionObj = GameObject.Find("Transition");
        if (transitionObj != null)
        {
            animator = transitionObj.GetComponent<Animator>();
        }
        else
        {
            Debug.LogWarning("Transition no encontrado en la escena");
        }
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }

        instance = this;
        DontDestroyOnLoad(gameObject);
        SceneManager.sceneLoaded += OnSceneLoaded;
        _sM = GetComponentInChildren<SaveManager>();
    }
    void Start()
    {
        StartCoroutine(AutoLoad());
    }

    IEnumerator AutoLoad()
    {
        yield return new WaitForSeconds(0.1f);

        if (PlayerPrefs.HasKey("SAVE_DATA"))
        {
            _sM.LoadGame();
        }
    }
    public void Update()
    {
        if (animator == null)
        {
            GameObject transitionObj = GameObject.Find("Transition");
            if (transitionObj != null)
            {
                animator = transitionObj.GetComponent<Animator>();
            }
            else
            {
                Debug.LogWarning("Transition no encontrado en la escena");
            }
        }
        
    }
    public void LoadNextScene()
    {
        LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }

    public void LoadScene(int sceneIndex)
    {
        _sM.SaveGame();
        if (isPaused) TogglePause();
        if (!GameObject.FindWithTag("Hat") && sceneIndex == 1)
        {
            Instantiate(_sM._hat);
        }
        StartCoroutine(SceneLoadCoroutine(sceneIndex));
    }

    private IEnumerator SceneLoadCoroutine(int sceneIndex)
    {
        animator.SetTrigger("StartTransition");
        yield return new WaitForSecondsRealtime(seconds);

        SceneManager.sceneLoaded += OnSceneLoaded;

        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(sceneIndex);
        asyncLoad.allowSceneActivation = true;

        while (!asyncLoad.isDone)
        {
            yield return null;
        }
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        
        SceneManager.sceneLoaded -= OnSceneLoaded;
        _pauseCanvas = GetComponentInChildren<Canvas>(true).gameObject;


        if (scene.buildIndex == 0)
        {
            Time.timeScale = 1f;
            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None;
        }
        else
        {
            GameObject.FindWithTag("Player").transform.position = GameObject.FindWithTag("Spawn").transform.position;
        }

    }

    private void PauseGame()
    {
        isPaused = true;
        PauseEvent.Invoke(isPaused);
        _pauseCanvas.SetActive(true);
        Time.timeScale = 0f;

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        Debug.Log("Pausa");
    }

    private void ResumeGame()
    {
        isPaused = false;
        PauseEvent.Invoke(isPaused);
        _pauseCanvas?.SetActive(false);
        Time.timeScale = 1f;

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        Debug.Log("Continuar");
    }

    public void TogglePause()
    {
        Debug.Log("Toggle");
        if (isPaused)
            ResumeGame();
        else
            PauseGame();
    }

    public void OnPause(InputAction.CallbackContext context)
    {
        Debug.Log("Input");
        if (context.started)
        {
            Debug.Log("InputCanceled");
            GameObject.FindWithTag("Manage").GetComponent<GameManager>().TogglePause();
        }

    
    }
    void OnEnable()
    {
        if (inputActions == null)
        {
            inputActions = new InputSystem_Actions();
            inputActions.Global.SetCallbacks(this);
        }
        inputActions.Global.Enable();
    }
    void OnDisable()
    {
        if (inputActions != null)
            inputActions.Global.Disable();
    }
}
