using UnityEngine;


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
