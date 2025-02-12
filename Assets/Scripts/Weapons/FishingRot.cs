using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FishingRot : MonoBehaviour, IWeapon
{
    [SerializeField] private GameObject bait;
    [SerializeField] private float throwForce;
    [SerializeField] private float retriefForce;
    [SerializeField] private float retriefAtDistance;
    [SerializeField] private float maxRetriefSpeed;
    [Header("audio")]
    [SerializeField] private AudioSource throwBaitSound;
    [SerializeField] private AudioSource retriefSound;

    private GameObject baitInstance;

    private bool isCast = false;

    private void Update()
    {
        if (baitInstance != null)
        {
            if ((baitInstance.transform.position - transform.position).magnitude > retriefAtDistance)
            {
                StartCoroutine(RetriefBait());
            }
        }
    }

    public void UseWeapon()
    {
        if (!isCast && baitInstance == null)
        {
            CastBait();
        }
    }

    public void UseWeaponSecondary()
    {
        if (isCast)
        {
            StartCoroutine(RetriefBait());
        }
    }

    public void WeaponSwitch()
    {
        Destroy(baitInstance);
        Destroy(gameObject);
    }

    public void ControlSwitch()
    {
        Destroy(baitInstance);
    }

    private void CastBait()
    {
        throwBaitSound.Play();
        isCast = true;
        baitInstance = Instantiate(bait, transform.position, transform.rotation);
        baitInstance.GetComponent<Rigidbody>().AddForce(baitInstance.transform.forward * throwForce, ForceMode.Impulse);
    }

    public IEnumerator RetriefBait()
    {
        retriefSound.Play();
        isCast = false;
        Rigidbody baitBody = baitInstance.GetComponent<Rigidbody>();
        while((baitInstance.transform.position - transform .position).magnitude > 1.5f)
        {
            if(baitBody.velocity.magnitude < maxRetriefSpeed)
            {
                baitBody.AddForce((baitInstance.transform.position - transform.position).normalized * -retriefForce, ForceMode.Impulse);
            } 
            else
            {
                baitBody.velocity /= new Vector3(baitBody.velocity.x / maxRetriefSpeed, baitBody.velocity.y / maxRetriefSpeed, baitBody.velocity.z / maxRetriefSpeed).magnitude;
            }
            yield return null;
        }
        Destroy(baitInstance);
    }
}
