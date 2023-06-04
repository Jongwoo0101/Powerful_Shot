using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerHealth : LivingEntity
{
    private PlayerController playerMovement;
    private void Awake()
    {
        playerMovement = GetComponent<PlayerController>();
    }

    protected override void OnEnable()
    {
        base.OnEnable();
        playerMovement.enabled = true;
    }
    public override void OnDamage(float damage, Vector3 hitPoint, Vector3 hitDirection)
    {
        base.OnDamage(damage, hitPoint, hitDirection);
    }

    public override void Die()
    {
        base.Die();
        playerMovement.enabled = false;
    }
}
