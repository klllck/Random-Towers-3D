using System;
using System.Collections;
using UnityEngine;

public abstract class Projectile : Singleton<Projectile>
{
    [Header("Projectile Setup")]
    [SerializeField] protected MeshRenderer meshRenderer;

    protected float damage;
    protected float speed;
    protected float radius; //?
    protected float deathDelay;
    protected bool isEnabled;

    protected Tower tower;
    protected Transform target;
    protected Enemy enemy;

    protected Action<Projectile> destroyAction;

    public virtual void Update()
    {
        if (!isEnabled)
        {
            return;
        }

        if (target == null || !enemy.isActiveAndEnabled)
        {
            destroyAction(this);
            return;
        }

        Vector3 shootDirection = target.position - transform.position;
        float currentDistance = speed * Time.deltaTime;

        if (shootDirection.magnitude <= currentDistance)
        {
            HitEnemy();
            return;
        }

        transform.Translate(shootDirection.normalized * currentDistance, Space.World);
        transform.LookAt(target);
    }

    public void Delegate(Action<Projectile> destroyPt)
    {
        destroyAction = destroyPt;
    }

    public virtual void FindEnemyToShoot(Enemy enemy, float towerDamage, float towerSpeed, float towerDeathDelay)
    {
        isEnabled = true;
        target = enemy.targetPoint;
        if (meshRenderer != null)
        {
            meshRenderer.enabled = true;
        }

        this.enemy = enemy;

        damage = towerDamage;
        speed = towerSpeed;
        deathDelay = towerDeathDelay;
    }

    protected virtual void HitEnemy() 
    {
        DoDamage(target);
        if (meshRenderer != null)
        {
            meshRenderer.enabled = false;
        }
        StartCoroutine(DeathDelay());
    }

    protected virtual IEnumerator DeathDelay()
    {
        yield return new WaitForSeconds(deathDelay);
        destroyAction(this);
    }

    protected virtual void DoDamage(Transform enemy)
    {
        var targetEnemy = target.parent.GetComponent<Enemy>();
        isEnabled = false;
        if (targetEnemy != null)
        {
            targetEnemy.TakeDamage(damage);
        }
    }

    protected void OnDrawGizmosSelected() 
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, radius);
    }
}
