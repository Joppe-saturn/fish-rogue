using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Trap : MonoBehaviour
{
    [SerializeField] private float damage;
    [SerializeField] private float time;
    [SerializeField] private GameObject explosion;
    [SerializeField] private float knockback;
    [Header("audio")]
    [SerializeField] private AudioSource explosionSound;

    private void OnCollisionEnter(Collision collision)
    {
        if(collision.transform.TryGetComponent<BasePlayer>(out BasePlayer player))
        {
            StartCoroutine(DealDamage(player));
            player.GetComponent<Rigidbody>().AddForce((transform.position - player.transform.position).normalized * -knockback * 100f + new Vector3(0, knockback * 100f, 0));
            Instantiate(explosion, transform.position, transform.rotation);
            explosionSound.Play();
            transform.position = new Vector3(-1000f, -1000f, -1000f);
        }
    }

    private IEnumerator DealDamage(BasePlayer player)
    {
        for(int i = 0; i < 100f * time; i++)
        {
            player.GetDamage(0.01f / time * damage);
            yield return new WaitForSeconds(0.01f);
        }
        while(explosionSound.isPlaying)
        {
            yield return null;
        }
        Destroy(gameObject);
    }
}
