
using UnityEngine;
using UnityEngine.SceneManagement;


public class Hat : Item
{
    public Transform _pos = null;

    [SerializeField] private float rotationSpeed = 90f;
    [SerializeField] private float bobSpeed = 2f;
    [SerializeField] private float bobHeight = 0.25f;
    

    private Vector3 groundStartPos;


    private void Start()
    {
        groundStartPos = transform.position;
        
    }
    private void OnEnable()
    {
        if (SceneManager.GetActiveScene().buildIndex == 0 )
        {
            Destroy(gameObject);
        }
        else if (GameObject.FindWithTag("Hat").GetComponent<Hat>() != this)
        {
            Destroy(GameObject.FindWithTag("Hat"));

        }

    }

    private void OnTriggerEnter(Collider other)
    {
        
        if (other.tag == "Player" && _pos == null && !gameObject.GetComponentInParent<Player>())
        {
            Debug.Log("Equipar");
            other.GetComponent<Player>().hasKey = true;
            transform.SetParent(other.transform);
            _pos = GameObject.FindWithTag("CollectablePos").transform;
        }
    }

    public override void Use(Player player)
    {
        _pos = _pos == GameObject.FindWithTag("HatPos").transform
            ? GameObject.FindWithTag("CollectablePos").transform
            : GameObject.FindWithTag("HatPos").transform;
    }

    private void Update()
    {
        if (GameObject.FindWithTag("Hat").GetComponent<Hat>() != this && SceneManager.GetActiveScene().buildIndex == 2)
        {
            Destroy(GameObject.FindWithTag("Hat"));

        }
        if (_pos != null)
        {
            transform.position = _pos.position;
        }
        else
        {
            transform.Rotate(Vector3.up * rotationSpeed * Time.deltaTime, Space.World);

            float bobOffset = Mathf.Sin(Time.time * bobSpeed) * bobHeight;
            transform.position = groundStartPos + Vector3.up * bobOffset;
        }
    }

    public override void Drop()
    {
        _pos = null;
        transform.SetParent(null);
        groundStartPos = transform.position; 
    }
}
