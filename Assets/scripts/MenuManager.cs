using System.Collections;
using System.Linq;
using TMPro.EditorUtilities;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Users;
using UnityEngine.SceneManagement;

public class MenuManager : MonoBehaviour
{
    [SerializeField] private float seconds = 1f;
    [SerializeField] private GameObject firstSelectedButton;
    [SerializeField] private GameManager _gM;

    private InputSystem_Actions inputActions;

    private void Awake()
    {
        if (inputActions == null)
        {
            inputActions = new InputSystem_Actions();
        }

        inputActions.UI.Enable();
    }
    private void Start()
    {
        inputActions.UI.Enable();

        if (Keyboard.current != null)
            InputUser.PerformPairingWithDevice(Keyboard.current);
        if (Mouse.current != null)
            InputUser.PerformPairingWithDevice(Mouse.current);

        EventSystem.current.SetSelectedGameObject(firstSelectedButton);
        _gM = GameObject.FindWithTag("Manage").GetComponent<GameManager>();
    }

    private void OnDestroy()
    {
        inputActions.UI.Disable();
    }

    public void LoadScene(int sceneIndex)
    {
        _gM.LoadScene(sceneIndex);
        //StartCoroutine(SceneLoadCoroutine(sceneIndex));
    }

    private IEnumerator SceneLoadCoroutine(int sceneIndex)
    {
        
        yield return new WaitForSecondsRealtime(seconds);

        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(sceneIndex);
        asyncLoad.allowSceneActivation = true;

        while (!asyncLoad.isDone)
        {
            yield return null;
        }
    }

    public void ExitGame()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }
}