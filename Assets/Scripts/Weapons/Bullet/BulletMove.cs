using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletMove : MonoBehaviour
{
    [SerializeField] private float speed;
    [SerializeField] private float damage;
    [SerializeField] private float time;
    private InventoryManager inventory;

    private void Start()
    {
        inventory = FindFirstObjectByType<InventoryManager>();
        if (inventory != null)
        {
            damage += inventory.RequestEffects(Item.Effect.BulletDamage);
        }
        StartCoroutine(BulletRetirementCounter());
    }

    private void Update()
    {
        transform.position += transform.forward * speed;
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("Enemy"))
        {
            if(other.TryGetComponent<IDamigable>(out IDamigable hit))
            {
                hit.GetDamage(damage);
                Destroy(gameObject);
            }
        }
    }

    private IEnumerator BulletRetirementCounter()
    {
        yield return new WaitForSeconds(time);
        Destroy(gameObject);
    }
}