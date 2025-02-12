using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

[CreateAssetMenu]
public class Item : ScriptableObject
{
    public string itemName;
    public string description;
    public Sprite icon;
    public GameObject item;

    public enum Effect
    {
        BulletDamage,
        PlayerHealth
    }

    [Header("Algie")]
    public Sprite algieIcon;
    public Effect effect;
    public float power;
    public float maxHealth;
    public float algieSpreadChance;
}
