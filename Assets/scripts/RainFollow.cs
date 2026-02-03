using UnityEngine;

public class RainFollow : MonoBehaviour
{
    [SerializeField] private Transform target;

    private void Awake()
    {
        if(target == null)
        {
            target = GameObject.FindWithTag("Player").transform;
        }
    }

    void Update()
    {
        if (target == null) return;

        transform.position = new Vector3(
            target.position.x,
            transform.position.y,
            target.position.z
        );
    }
}
