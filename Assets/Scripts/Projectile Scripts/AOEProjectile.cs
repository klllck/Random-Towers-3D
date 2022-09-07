using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AOEProjectile : Projectile
{
    public override void Update()
    {
        base.Update();
    }

    protected override void HitEnemy()
    {
        Collider[] hitColliders = Physics.OverlapSphere(transform.position, radius);
        foreach (var collider in hitColliders)
        {
            if (collider.CompareTag("Enemy"))
            {
                DoDamage(collider.transform);
            }
        }

        destroyAction(this);
    }

    protected override void DoDamage(Transform enemyCollider) 
    {
        base.DoDamage(enemyCollider);
    }

    public override void FindEnemyToShoot(Enemy targetEnemy, float towerDamage, float towerSpeed, float towerDeathDelay)
    {
        base.FindEnemyToShoot(targetEnemy, towerDamage, towerSpeed, towerDeathDelay);
    }
}
