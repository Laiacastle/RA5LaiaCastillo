using System.Security.Cryptography;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SaveManager : MonoBehaviour
{
    [SerializeField]private Player playerScript;
    [SerializeField] private GameManager _gM;

    private bool isPaused;

    const string SAVE_KEY = "SAVE_DATA";

    private void OnEnable()
    {
        if (_gM != null)
        {
            _gM.PauseEvent += TogglePause;
        }
    }
    private void OnDisable()
    {
        if (_gM != null) 
        {
            _gM.PauseEvent -= TogglePause;
        }
    }
    private void Start()
    {
        _gM = gameObject.GetComponentInParent<GameManager>();
        playerScript = GameObject.FindGameObjectWithTag("Player").GetComponent<Player>();

    }

    public void SaveGame()
    {
        if (isPaused) return;
        if (playerScript == null)
            playerScript = GameObject.FindGameObjectWithTag("Player")?.GetComponent<Player>();
        SaveData data = new SaveData();

        // Posición
        data.posX = playerScript.transform.position.x;
        data.posY = playerScript.transform.position.y;
        data.posZ = playerScript.transform.position.z;
        Debug.Log("Guardando posición: " + playerScript.transform.position);

        // Escena
        data.sceneName = SceneManager.GetActiveScene().name;

        // Objeto
        data.hasKey = playerScript.hasKey;

        string json = JsonUtility.ToJson(data);
        PlayerPrefs.SetString(SAVE_KEY, json);
        PlayerPrefs.Save();

        Debug.Log("Guardado");
    }

    public void LoadGame()
    {
        if (!PlayerPrefs.HasKey(SAVE_KEY))
        {
            Debug.Log("No hay datos");
            return;
        }

        string json = PlayerPrefs.GetString(SAVE_KEY);
        SaveData data = JsonUtility.FromJson<SaveData>(json);

        SceneManager.LoadScene(data.sceneName);

        StartCoroutine(LoadAfterScene(data));
    }

    private System.Collections.IEnumerator LoadAfterScene(SaveData data)
    {
        yield return new WaitForEndOfFrame();

        GameObject playerObj = GameObject.FindWithTag("Player");
        
        playerObj.transform.position = new Vector3(data.posX, data.posY, data.posZ);
        Debug.Log(new Vector3(data.posX, data.posY, data.posZ));
        Player p = playerObj.GetComponent<Player>();
        p.hasKey = data.hasKey;
        if (data.hasKey)
        {
            Hat item = GameObject.FindWithTag("Hat").GetComponent<Hat>();
            item.transform.SetParent(p.transform);
            item._pos = GameObject.FindWithTag("CollectablePos").transform;
        }
        Debug.Log("Se han cargado los datos");
    }

    public void ResetGame()
    {
        
        if (PlayerPrefs.HasKey(SAVE_KEY))
            PlayerPrefs.DeleteKey(SAVE_KEY);

        PlayerPrefs.Save();

        Debug.Log("Datos de guardado borrados");

        if (Player.instance != null)
        {
            Destroy(Player.instance.gameObject);
        }



        _gM.LoadScene(0);
    }

    private void TogglePause(bool pause)
    {
        isPaused = pause;
    }
}