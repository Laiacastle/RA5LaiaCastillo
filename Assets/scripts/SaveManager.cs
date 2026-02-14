using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SaveManager : MonoBehaviour
{
    [SerializeField]private Player playerScript;
    [SerializeField] private GameManager _gM;
    [SerializeField] public bool hasHat = false;
    [SerializeField] public GameObject _hat;
    [SerializeField] private GameObject playerPrefab;
    private bool isPaused;

    const string SAVE_KEY = "SAVE_DATA";

    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;

        if (_gM != null)
            _gM.PauseEvent += TogglePause;
    }
    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;

        if (_gM != null)
            _gM.PauseEvent -= TogglePause;
    }
    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        playerScript = Player.instance?.GetComponent<Player>();
    }
    private void Start()
    {
        _gM = gameObject.GetComponentInParent<GameManager>();
        playerScript =Player.instance?.GetComponent<Player>();

    }

    public void SaveGame()
    {
        Debug.Log("Intentando guardar...");
        Player playerScript = Player.instance?.GetComponent<Player>();
        if (playerScript == null) return;

        if (playerScript == null) return;
        SaveData data = new SaveData();

        // Posición
        data.posX = playerScript.transform.position.x;
        data.posY = playerScript.transform.position.y;
        data.posZ = playerScript.transform.position.z;
        Debug.Log("Guardando posición: " + playerScript.transform.position);

        // Escena
        data.sceneIndex = SceneManager.GetActiveScene().buildIndex;

        // Objeto
        data.hasKey = playerScript.hasKey;

        string json = JsonUtility.ToJson(data);
        PlayerPrefs.SetString(SAVE_KEY, json);
        PlayerPrefs.Save();

    }

    public void SaveSecondScene()
    {
        Debug.Log("Intentando guardar...");
        Player playerScript = Player.instance?.GetComponent<Player>();
        GameObject spawn = GameObject.FindWithTag("Spawn");
        if (playerScript == null) return;

        if (playerScript == null) return;
        SaveData data = new SaveData();

        // Posición
        data.posX = spawn.transform.position.x;
        data.posY = spawn.transform.position.y;
        data.posZ = spawn.transform.position.z;
        Debug.Log("Guardando posición: " + spawn.transform.position);

        // Escena
        data.sceneIndex = 2;

        // Objeto
        data.hasKey = true;

        string json = JsonUtility.ToJson(data);
        PlayerPrefs.SetString(SAVE_KEY, json);
        PlayerPrefs.Save();
        StartCoroutine(LoadAfterScene(data));
    }


    public void LoadGame()
    {
        if (!PlayerPrefs.HasKey("SAVE_DATA")) return;

        string json = PlayerPrefs.GetString("SAVE_DATA");
        SaveData data = JsonUtility.FromJson<SaveData>(json);

        if (SceneManager.GetActiveScene().buildIndex == 0) return;

        StartCoroutine(LoadAfterScene(data));
    }

    public IEnumerator LoadSceneAndApply(SaveData data)
    {
        _gM.LoadScene(data.sceneIndex); //caga la escena guardada

        while (SceneManager.GetActiveScene().buildIndex != data.sceneIndex)
            yield return null;

        yield return null;

        ApplyData(data); //mueveo
    }
    public void ApplyData(SaveData data)
    {
        GameObject playerObj = Player.instance.gameObject;
        
        if (playerObj == null)
        {
            Debug.LogError("Player no encontrado al cargar");
            return;
        }

        CharacterController cc = playerObj.GetComponent<CharacterController>();
        cc.enabled = false;

        playerObj.transform.position = new Vector3(data.posX, data.posY, data.posZ);

        cc.enabled = true;

        Player p = playerObj.GetComponent<Player>();
        p.hasKey = data.hasKey;

        if (data.hasKey)
        {
            if (GameObject.FindWithTag("Hat")) Destroy(GameObject.FindWithTag("Hat"));
            GameObject hat = Instantiate(_hat);

            hat.transform.SetParent(p.transform);

            hat.GetComponent<Hat>()._pos =
                GameObject.FindWithTag("CollectablePos").transform;

            hasHat = true;
        }
        else
        {
            hasHat = false;
            Instantiate(_hat);
        }

        Debug.Log("Datos aplicados correctamente");
    }
    private IEnumerator LoadAfterScene(SaveData data)
    {
        yield return new WaitUntil(() => Player.instance != null);

        Player p = Player.instance;

        p.transform.position = new Vector3(data.posX, data.posY, data.posZ);
        p.hasKey = data.hasKey;

        if (data.hasKey && GameObject.FindWithTag("Hat").GetComponentInParent<Player>() == null)
        {
            GameObject hat = GameObject.FindWithTag("Hat");
            if (hat == null) hat = Instantiate(_hat);
            if (hat != null)
            {
                hat.transform.SetParent(p.transform);
                hat.GetComponent<Hat>()._pos = GameObject.FindWithTag("CollectablePos")?.transform;
            }
            
        }
        else if (SceneManager.GetActiveScene().buildIndex == 1)
        {
            Instantiate(_hat);
        }

        Debug.Log("Datos cargados correctamente");
    }


    public void ResetGame()
    {
        //Borrar datos
        if (PlayerPrefs.HasKey(SAVE_KEY))
            PlayerPrefs.DeleteKey(SAVE_KEY);
        PlayerPrefs.Save();

        Debug.Log("Datos de guardado borrados");

        if (Player.instance != null)
            Destroy(Player.instance.gameObject);

        if (playerPrefab != null)
        {
            GameObject newPlayer = Instantiate(playerPrefab);
            Player.instance = newPlayer.GetComponent<Player>();
        }

        if (_hat != null)
        {
            Instantiate(_hat);
            hasHat = false;
        }

        //Cargar la escena
        _gM.LoadScene(1);
    }

    private void TogglePause(bool pause)
    {
        isPaused = pause;
    }

}