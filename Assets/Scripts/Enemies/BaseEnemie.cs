using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseEnemie : MonoBehaviour, IDamigable
{
    public float health;
    public int damage;
    public float speed;
    public float minDistanceFromPlayer;
    public float maxDistanceFromPlayer;
    public float playerSaveTime;

    public Rigidbody rb;
    public GameObject player;

    public enum EnemyState
    {
        idle,
        attacking
    }

    public EnemyState enemyState = EnemyState.idle;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        player = FindFirstObjectByType<PlayerMovement>().gameObject;

        StartCoroutine(MoveToPlayer());
    }

    private IEnumerator MoveToPlayer()
    {
        while (true)
        {
            if ((transform.position - player.transform.position).magnitude < maxDistanceFromPlayer)
            {
                enemyState = EnemyState.attacking;
            } else
            {
                enemyState = EnemyState.idle;
            }

            switch(enemyState)
            {
                case EnemyState.idle:
                    EnemyIdle(); 
                    break;
                case EnemyState.attacking:
                    EnemyAttacking();
                    break;
            }
            yield return null;
        }
    }

    private void EnemyIdle()
    {
        rb.velocity /= 2.0f;
    }

    private void EnemyAttacking()
    {
        if ((transform.position - player.transform.position).magnitude > minDistanceFromPlayer)
        {
            rb.velocity = (transform.position - player.transform.position).normalized * -speed;
        } 
        else if (player.transform.TryGetComponent<IDamigable>(out IDamigable hit))
        {
            hit.GetDamage(damage * Time.deltaTime);
        }


        transform.LookAt(player.transform.position);
    }

    public void GetDamage(float damage)
    {
        health -= damage;

        if(health <= 0)
        {
            Die();
        }
    }

    private void Die()
    {
        Destroy(gameObject);
    }
}
