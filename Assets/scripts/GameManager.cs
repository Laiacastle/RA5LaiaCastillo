using System;
using System.Collections;
using System.Linq;
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
    private bool hasLoadedSave = false;
    private InputSystem_Actions inputActions;

    private void Awake()
    {
        // Patrón singleton
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }
        instance = this;
        DontDestroyOnLoad(gameObject);

        _sM = GetComponentInChildren<SaveManager>();
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (scene.buildIndex == 0)
        {
            // Menú
            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None;
            Player.instance?.gameObject.SetActive(false);
            _pauseCanvas?.SetActive(false);
        }
        else
        {
            // Escena de juego
            Cursor.visible = false;
            Cursor.lockState = CursorLockMode.Locked;
            SearchPJ();

            // Solo autoload si es escena de juego
            if (!hasLoadedSave)
            {
                StartCoroutine(AutoLoad());
                hasLoadedSave = true;
            }
        }
    }



    private void SearchPJ()
    {
        if (Player.instance != null)
        {
            Player.instance.gameObject.SetActive(true);
        }

        else
        {
            Player p = Resources.FindObjectsOfTypeAll<Player>().FirstOrDefault(obj => obj.gameObject.scene.name != null);
            if (p != null)
            {
                p.gameObject.SetActive(true);
                Player.instance = p;
            }
        }
    }
    private void OnEnable()
    {
        if (inputActions == null)
        {
            inputActions = new InputSystem_Actions();
            inputActions.Global.SetCallbacks(this);
        }
        inputActions.Global.Enable();

        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDisable()
    {
        if (inputActions != null)
            inputActions.Global.Disable();

        SceneManager.sceneLoaded -= OnSceneLoaded;
    }


    private IEnumerator AutoLoad()
    {
        if (!PlayerPrefs.HasKey("SAVE_DATA")) yield break;

        string json = PlayerPrefs.GetString("SAVE_DATA");
        SaveData data = JsonUtility.FromJson<SaveData>(json);

        if (SceneManager.GetActiveScene().buildIndex != data.sceneIndex)
        {
            Debug.Log("Cargando escena guardada: " + data.sceneIndex);
            yield return _sM.LoadSceneAndApply(data);
        }
        else
        {
            Debug.Log("Aplicando datos en la misma escena");
            _sM.ApplyData(data);
        }
        if (data.sceneIndex == 0)
        {
            Debug.Log("AutoLoad detectó menú, no hacemos nada");
            yield break;
        }

        hasLoadedSave = true;
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
        Time.timeScale = 1f;
        if (isPaused) TogglePause();

        // ... resto de tu código
        StartCoroutine(SceneLoadCoroutine(sceneIndex));
    }

    private IEnumerator SceneLoadCoroutine(int sceneIndex)
    {
        if (animator != null)
            animator.SetTrigger("StartTransition");

        yield return new WaitForSecondsRealtime(seconds);

        hasLoadedSave = false;

        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(sceneIndex);
        asyncLoad.allowSceneActivation = true;

        while (!asyncLoad.isDone)
        {
            yield return null;
        }
    }

    private void PauseGame()
    {
        isPaused = true;
        PauseEvent.Invoke(isPaused);

        if (_pauseCanvas != null)
            _pauseCanvas.SetActive(true);

        Time.timeScale = 0f;
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        _sM?.SaveGame();
    }

    private void ResumeGame()
    {
        isPaused = false;
        PauseEvent.Invoke(isPaused);

        if (_pauseCanvas != null)
            _pauseCanvas.SetActive(false);

        Time.timeScale = 1f;
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    public void TogglePause()
    {
        if (SceneManager.GetActiveScene().buildIndex == 0) return;
        if (isPaused)
            ResumeGame();
        else
            PauseGame();
    }

    public void OnPause(InputAction.CallbackContext context)
    {
        if (context.started)
        {
            TogglePause();
        }

    
    }

}
