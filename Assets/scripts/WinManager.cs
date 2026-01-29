using UnityEngine;

public class WinManager : MonoBehaviour
{
    private void OnTriggerEnter(Collider col)
    {
        if (col.gameObject.tag == "Player")
        {
            Player player = col.gameObject.GetComponent<Player>();
            if (player)
            {
                player.OnWinDance();
            }
        }
    }
}
