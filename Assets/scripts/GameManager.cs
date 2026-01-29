using System.Collections;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    [SerializeField] private Animator animator;
    [SerializeField] private float seconds = 1f;
    [SerializeField] bool UNITY_EDITOR = true;
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

        if (scene.buildIndex == 0)
        {
            Time.timeScale = 1f;
            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None;
        }

    }

    public void ExitGame()
    {
        #if UNITY_EDITOR

        UnityEditor.EditorApplication.isPlaying = false; // detiene el modo Play

        #else
           
                Application.Quit();
        #endif
    }
}
