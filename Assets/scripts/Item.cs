using UnityEngine;

public abstract class Item : MonoBehaviour
{

    [SerializeField] public int amount;
    [SerializeField] public string nameItem;
    [SerializeField] public string description;
    [SerializeField] public Sprite icon;
    [SerializeField] public Mesh obj;


    public abstract void Use(Player player);

}

