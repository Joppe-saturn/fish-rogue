using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerWeapon : MonoBehaviour
{
    [SerializeField] private GameObject fishingRot;
    [SerializeField] private GameObject weapon;

    private GameObject currentObj;

    public enum State
    {
        Empty,
        Fishing,
        Shooting
    }

    private State state = State.Empty;

    public void SwitchWeapon()
    {
        if (currentObj != null)
        {
            if (currentObj.TryGetComponent<IWeapon>(out IWeapon weapon))
            {
                weapon.WeaponSwitch();
            }
        }

        switch (state)
        {
            case State.Empty:
                state = State.Fishing;
                break;
            case State.Fishing:
                state = State.Shooting;
                break;
            case State.Shooting:
                state = State.Fishing;
                break;
        }
        HoldWeapon();
    }

    private void HoldWeapon()
    {
        Transform holdingItem = transform.GetChild(0).GetChild(0);
        switch (state)
        {
            case State.Fishing:
                currentObj = Instantiate(fishingRot, holdingItem.position, holdingItem.rotation);
                break;
            case State.Shooting:
                currentObj = Instantiate(weapon, holdingItem.position, holdingItem.rotation);
                break;
        }
        currentObj.transform.SetParent(holdingItem);
    }

    public void UseWeapon()
    {
        if (currentObj != null)
        {
            if (currentObj.TryGetComponent<IWeapon>(out IWeapon weapon))
            {
                weapon.UseWeapon();
            }
        }
    }

    public void UseWeaponSecondary()
    {
        if (currentObj != null)
        {
            if (currentObj.TryGetComponent<IWeapon>(out IWeapon weapon))
            {
                weapon.UseWeaponSecondary();
            }
        }
    }

    public void ChangeControls()
    {
        if (currentObj != null)
        {
            if (currentObj.TryGetComponent<IWeapon>(out IWeapon weapon))
            {
                weapon.ControlSwitch();
            }
        }
    }
}
