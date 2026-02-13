using System.Collections;
using System.Security.Cryptography;
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
        playerScript = GameObject.FindGameObjectWithTag("Player")?.GetComponent<Player>();
    }
    private void Start()
    {
        _gM = gameObject.GetComponentInParent<GameManager>();
        playerScript = GameObject.FindGameObjectWithTag("Player")?.GetComponent<Player>();

    }

    public void SaveGame()
    {
        Debug.Log("Intentando guardar...");
        Player playerScript = GameObject.FindGameObjectWithTag("Player")?.GetComponent<Player>();
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
        GameObject playerObj = GameObject.FindWithTag("Player");
        if (playerObj == null)
        {
            Debug.LogError("Player no encontrado al cargar");
            return;
        }

        playerObj.transform.position = new Vector3(data.posX, data.posY, data.posZ);

        Player p = playerObj.GetComponent<Player>();
        p.hasKey = data.hasKey;

        if (data.hasKey)
        {
            GameObject[] worldHat = GameObject.FindGameObjectsWithTag("Hat");

            if (worldHat.Length > 1)
                Destroy(worldHat[1]);

            worldHat[0].transform.SetParent(p.transform);
            worldHat[0].GetComponent<Hat>()._pos =
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
        //Espera a que Player.instance esté
        yield return new WaitUntil(() => Player.instance != null);

        Player p = Player.instance;

        p.transform.position = new Vector3(data.posX, data.posY, data.posZ);
        p.hasKey = data.hasKey;

        if (data.hasKey)
        {
            GameObject hat = GameObject.FindWithTag("Hat");
            if (hat != null)
            {
                hat.transform.SetParent(p.transform);
                hat.GetComponent<Hat>()._pos = GameObject.FindWithTag("CollectablePos")?.transform;
            }
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
            Vector3 hatSpawnPos = new Vector3(3.01494f, 1.64f, 0.83f);
            Instantiate(_hat, hatSpawnPos, Quaternion.identity);
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