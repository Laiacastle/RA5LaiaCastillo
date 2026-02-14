using EasyDoorSystem;
using UnityEngine;
using UnityEngine.SceneManagement;
public class DoorManager : MonoBehaviour
{

    private void OnTriggerEnter(Collider collision)
    {
        if (collision.tag == "Player")
        {
            Player player = collision.gameObject.GetComponent<Player>();
            if (player && player.hasKey)
            {
                GameManager.instance.LoadScene(1);
            }
        }
        
    }
}
