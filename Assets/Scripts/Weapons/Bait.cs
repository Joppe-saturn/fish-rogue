using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bait : MonoBehaviour
{
    [SerializeField] private float decelerationOnWater;
    [SerializeField] private float bionacie;
    private Rigidbody body;
    [Header("audio")]
    [SerializeField] private AudioSource bubbleSound;

    private WaterLoot water = new WaterLoot();

    private void Start()
    {
        body = GetComponent<Rigidbody>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer == 4)
        {
            water = other.GetComponent<WaterLoot>();
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.gameObject.layer == 4)
        {
            AudioSource audioInstance = bubbleSound;
            audioInstance.pitch = Random.Range(0.0f, 1.0f);
            audioInstance.volume = Random.Range(0.1f, 0.5f);
            audioInstance.Play();
            body.velocity = new Vector3(body.velocity.x / decelerationOnWater, body.velocity.y + ((bionacie * Mathf.Sqrt(Mathf.Pow(body.velocity.y, 2)) / (Mathf.Sqrt(Mathf.Pow(body.velocity.y, 2)) * decelerationOnWater)) - 1), body.velocity.z / decelerationOnWater);
            if (water != null)
            {
                int r = Random.Range(0, water.chanceToCatch);
                
                if (r < 1)
                {
                    water.GetFish(transform);
                }
            }
        }
    }
}
