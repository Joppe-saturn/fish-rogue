using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class BasePlayer : MonoBehaviour, IDamigable
{
    [SerializeField] public float health;
    [SerializeField] private bool immortal;
    private bool baitIsRetriefing = false;
    private InventoryManager inventoryManager;

    private void Start()
    {
        inventoryManager = FindFirstObjectByType<InventoryManager>();
    }

    public void GetDamage(float damage)
    {
        float realDamage = damage / (1f + inventoryManager.RequestEffects(Item.Effect.PlayerHealth));
        health -= realDamage;

        if (health <= 0 && !immortal)
        {
            Die();
        }

        if (!immortal)
        {
            Bait bait = FindFirstObjectByType<Bait>();
            if(bait != null)
            {
                if(!baitIsRetriefing)
                {
                    baitIsRetriefing = true;
                    StartCoroutine(FindFirstObjectByType<FishingRot>().RetriefBait());
                } 
            }
            else
            {
                baitIsRetriefing = false;
            }
        }
    }

    private void Die()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}
