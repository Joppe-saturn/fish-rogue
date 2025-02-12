using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShotGun : MonoBehaviour, IWeapon
{
    [SerializeField] private GameObject bullet;
    [SerializeField] private int bulletQuantity;
    [SerializeField] private float spread;
    [SerializeField] private GameObject spreadIndicator;
    [SerializeField] private float focusSpeed;
    [SerializeField] private float reloadTime;
    [SerializeField] private float recoil;
    [Header("audio")]
    [SerializeField] private AudioSource loadGunSound;
    [SerializeField] private AudioSource shotSound;

    private Canvas ui;
    private GameObject indicator;

    private float range;
    
    private bool hasShot = true;
    private bool reLoading = true;


    private void Start()
    {
        ui = FindFirstObjectByType<Canvas>();
        StartCoroutine(ReLoad());
    }

    public void UseWeapon()
    {
        StartCoroutine(LoadGun());
    }

    public void UseWeaponSecondary()
    {
        hasShot = true;
    }

    public void WeaponSwitch()
    {
        Destroy(indicator);
        StopCoroutine(LoadGun());
        Destroy(gameObject);
    }

    private IEnumerator LoadGun()
    {
        if (hasShot && !reLoading)
        {
            hasShot = false;
            indicator = Instantiate(spreadIndicator);
            indicator.transform.SetParent(ui.transform.GetChild(0).GetChild(0));
            indicator.transform.position = new Vector3(Screen.width / 2.0f, Screen.height / 2.0f, 0.0f);
            range = 1;
            bool canShoot = true;
            StartCoroutine(PlayEffect());
            while (!hasShot)
            {
                if(indicator == null)
                {
                    canShoot = false;
                    hasShot = true;
                    break;
                }
                range /= focusSpeed;
                indicator.transform.localScale = new Vector3(range, range, range);
                yield return new WaitForSeconds(0.02f);
            }
            Destroy(indicator);
            for(int i = 0; i < ui.transform.GetChild(0).GetChild(0).childCount; i++)
            {
                Destroy(ui.transform.GetChild(0).GetChild(0).GetChild(i).gameObject);
            }
            for(int i = 0; i < bulletQuantity; i++)
            {
                if(canShoot)
                {
                    float x = Random.Range(-range, range) * spread;
                    float y = Random.Range(-range, range) * spread;
                    float z = Random.Range(-spread / 10.0f, spread / 10.0f);

                    Instantiate(bullet, transform.parent.parent.position + new Vector3(x, y, z), Quaternion.Euler(transform.rotation.eulerAngles.x + (x * 10.0f), transform.rotation.eulerAngles.y + (y * 10.0f), 0));
                }
            }
            if(canShoot)
            {
                transform.parent.parent.parent.GetComponent<Rigidbody>().AddForce(transform.parent.parent.forward * -10.0f * (0.025f / (range + 0.01f)) * recoil, ForceMode.Impulse);
            }
            reLoading = true;
            StartCoroutine(ReLoad());
        }
    }

    private IEnumerator ReLoad()
    {
        yield return new WaitForSeconds(reloadTime);
        reLoading = false;
    }

    public void ControlSwitch()
    {
        Destroy(indicator);
        StopCoroutine(LoadGun());
    }

    private IEnumerator PlayEffect()
    {
        loadGunSound.Play();
        while(!hasShot)
        {
            yield return null;
        }
        loadGunSound.Stop();
        shotSound.Play();
    }
}
