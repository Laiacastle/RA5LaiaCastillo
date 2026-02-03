using UnityEngine;


public class Hat : Item
{
    private Transform _pos = null;
    private void OnTriggerEnter(Collider other)
    {
        Debug.Log("Trigger");
        if (other.tag == "Player" && _pos == null)
        {
            Debug.Log("TriggerP");
            other.GetComponent<Player>().hasKey = true;
            transform.SetParent(other.transform);
        }
        _pos = GameObject.FindWithTag("CollectablePos").transform;
    }
    public override void Use(Player player)
    {
        _pos = _pos == GameObject.FindWithTag("HatPos").transform ? GameObject.FindWithTag("CollectablePos").transform : GameObject.FindWithTag("HatPos").transform;
    }

    private void Update()
    {
        if (_pos != null)
        {
            transform.position = _pos.position;
        }
    }
}
