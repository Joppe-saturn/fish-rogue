using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemObject : MonoBehaviour
{
    [SerializeField] private Item item;

    private void OnCollisionEnter(Collision collision)
    {
        if(collision.gameObject.layer == 6)
        {
            FindFirstObjectByType<InventoryManager>().AddItem(item);
            Destroy(gameObject);
        }
    }
}
