using System;
using System.Collections;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    [SerializeField] private Animator animator;
    [SerializeField] private float seconds = 1f;
    [SerializeField] bool UNITY_EDITOR = true;
    [SerializeField] private SaveManager _sM;
    [SerializeField] private GameObject _pauseCanvas;
    [SerializeField] private PlayerInput playerInput;
    public bool isPaused = false;
    public event Action<bool> PauseEvent = delegate { };
    public static GameManager instance;

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
        playerInput = Player.instance.GetComponent<PlayerInput>();
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
        if (isPaused) TogglePause();
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
        playerInput = Player.instance.GetComponent<PlayerInput>();
        playerInput.DeactivateInput();
        playerInput.ActivateInput();

        if (Player.instance != null)
            playerInput = Player.instance.GetComponent<PlayerInput>();

        if (scene.buildIndex == 0)
        {
            Time.timeScale = 1f;
            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None;
        }

    }

    private void PauseGame()
    {
        isPaused = true;
        PauseEvent.Invoke(isPaused);
        playerInput.actions.FindActionMap("Player").Disable();
        playerInput.actions.FindActionMap("UI").Enable();
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
        playerInput.actions.FindActionMap("Player").Enable();
        playerInput.actions.FindActionMap("UI").Disable();
        _pauseCanvas?.SetActive(false);
        Time.timeScale = 1f;

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        Debug.Log("Continuar");
    }

    public void TogglePause()
    {
        if (isPaused)
            ResumeGame();
        else
            PauseGame();
    }

    public void ExitGame()
    {
        _sM.SaveGame();
        #if UNITY_EDITOR

        UnityEditor.EditorApplication.isPlaying = false; // detiene el modo Play

        #else
           
                Application.Quit();
        #endif
    }
}
