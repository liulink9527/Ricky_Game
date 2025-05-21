using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Character : MonoBehaviour {
    [Header("基本属性")] public float maxHealth;
    public float currentHealth;

    [Header("受伤无敌")] public float invulnerableDuration;
    private float invulnerableCounter;
    public bool invulnerable;

    public UnityEvent<Transform> onTakeDamage;
    public UnityEvent onDead;

    private void Start() {
        currentHealth = maxHealth;
    }

    private void Update() {
        if (invulnerable) {
            invulnerableCounter -= Time.deltaTime;
            if (invulnerableCounter <= 0) {
                invulnerable = false;
            }
        }
    }

    public void TakeDamage(Attack attacker) {
        if (invulnerable) {
            return;
        }

        // 如果伤害大于生命值 则减血 触发无敌时间
        if (currentHealth - attacker.damage > 0) {
            currentHealth -= attacker.damage;
            TriggerInvulnerable();
            // 人物受伤逻辑
            onTakeDamage?.Invoke(attacker.transform);
            
        } else {
            currentHealth = 0;
            // 触发死亡
            onDead?.Invoke();
        }
    }

    // 触发无敌时间
    private void TriggerInvulnerable() {
        if (!invulnerable) {
            invulnerable = true;
            invulnerableCounter = invulnerableDuration;
        }
    }
}
