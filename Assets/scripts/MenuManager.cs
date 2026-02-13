using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.UI;
using UnityEngine.InputSystem.Users;
using UnityEngine.SceneManagement;

public class MenuManager : MonoBehaviour
{
    [Header("UI")]
    [SerializeField] private GameObject firstSelectedButton;

    [Header("References")]
    [SerializeField] private GameManager _gM;

    private InputSystem_Actions inputActions;
    private InputSystemUIInputModule uiInputModule;

    private void Awake()
    {
        // Inicializamos acciones
        inputActions ??= new InputSystem_Actions();
        inputActions.UI.Enable();

        // Referencia al GameManager si no está asignado
        if (_gM == null)
            _gM = GameObject.FindWithTag("Manage").GetComponent<GameManager>();

        // Aseguramos que haya EventSystem con InputSystemUIInputModule
        SetupUIInputModule();
    }

    private void Start()
    {
        // Aseguramos que el cursor sea visible y desbloqueado
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;

        // Seleccionamos el primer botón
        if (firstSelectedButton != null)
            EventSystem.current.SetSelectedGameObject(firstSelectedButton);

        // Forzamos la posición del mouse al centro de la pantalla
        if (Mouse.current != null)
            Mouse.current.WarpCursorPosition(new Vector2(Screen.width / 2f, Screen.height / 2f));
    }

    private void OnDestroy()
    {
        inputActions.UI.Disable();
    }

    private void SetupUIInputModule()
    {
        var es = EventSystem.current;
        if (es == null)
        {
            // Creamos un EventSystem si no existe
            var esGO = new GameObject("EventSystem", typeof(EventSystem));
            es = esGO.GetComponent<EventSystem>();
        }

        // Aseguramos que tenga InputSystemUIInputModule
        uiInputModule = es.GetComponent<InputSystemUIInputModule>();
        if (uiInputModule == null)
            uiInputModule = es.gameObject.AddComponent<InputSystemUIInputModule>();

        // Asignamos las acciones
        uiInputModule.actionsAsset = inputActions.asset;
        uiInputModule.point = InputActionReference.Create(inputActions.UI.Point);
        uiInputModule.leftClick = InputActionReference.Create(inputActions.UI.Click);
        uiInputModule.submit = InputActionReference.Create(inputActions.UI.Submit);
        uiInputModule.cancel = InputActionReference.Create(inputActions.UI.Cancel);
        uiInputModule.move = InputActionReference.Create(inputActions.UI.Navigate);
    }

    // Función para cargar escena
    public void LoadScene(int sceneIndex)
    {
        if (_gM != null)
            _gM.LoadScene(sceneIndex);
        else
            SceneManager.LoadScene(sceneIndex); // fallback si no hay GameManager
    }

    // Función para salir del juego
    public void ExitGame()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }
}