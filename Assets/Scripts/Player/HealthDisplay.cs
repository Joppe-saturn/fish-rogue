using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HealthDisplay : MonoBehaviour
{
    [SerializeField] private Vector2 scale;
    private BasePlayer player;
    private Image me;
    private float maxHealth;

    private void Start()
    {
        player = FindFirstObjectByType<BasePlayer>();
        me = GetComponent<Image>();
        maxHealth = player.health;
    }

    private void Update()
    {
        me.rectTransform.sizeDelta = new Vector2(scale.x * player.health / maxHealth, scale.y);
    }
}
