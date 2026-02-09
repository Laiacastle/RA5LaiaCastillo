using UnityEngine;
using UnityEngine.SceneManagement;

public class SaveManager : MonoBehaviour
{
    [SerializeField]private Player playerScript;

    const string SAVE_KEY = "SAVE_DATA";

    private void Start()
    {
        playerScript = GameObject.FindGameObjectWithTag("Player").GetComponent<Player>();

    }

    public void SaveGame()
    {
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
}