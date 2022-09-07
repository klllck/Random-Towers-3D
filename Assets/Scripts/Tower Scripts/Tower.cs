using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;
using UnityEngine.VFX;

public class Tower : Singleton<Tower>
{
    [Header("Tower Stats")]
    [SerializeField] private float damage;
    [SerializeField] private float bulletsPerMinute;
    [SerializeField] private float turnSpeed;
    [SerializeField] private float range;
    [SerializeField] private float projectileSpeed;
    [SerializeField] private float projectileDeathDelay;

    [Header("Tower Setup")]
    [SerializeField] private GameObject projectilePrefab;
    [SerializeField] private Animator animator;
    [SerializeField] private Transform partToRotate;
    [SerializeField] private List<Transform> shootPoints;

    private Transform target;
    private Enemy targetEnemy;
    private float attackCountdown = 0f;
    private List<VisualEffect> visualEffects = new List<VisualEffect>();

    public float Damage { get => damage; set => damage = value; }
    public float BulletsPerMinute { get => bulletsPerMinute; set => bulletsPerMinute = value; }
    public float Range { get => range; set => range = value; }

    private void Start()
    {
        InvokeRepeating(nameof(FindTarget), 0f, 0.1f);

        foreach (var shootPoint in shootPoints)
        {
            visualEffects.Add(shootPoint.GetComponent<VisualEffect>());
        }
    }

    private void Update()
    {
        if (target != null && targetEnemy.isActiveAndEnabled)
        {
            LookAtTarget();

            if (attackCountdown <= 0f)
            {
                attackCountdown = 60f / bulletsPerMinute;
                Shoot();
            }
        }

        attackCountdown -= Time.deltaTime;
    }

    private void FindTarget()
    {
        if (target != null && Vector3.Distance(transform.position, target.position) <= range && targetEnemy.isActiveAndEnabled)
        {
            return;
        }

        GameObject[] enemies = GameObject.FindGameObjectsWithTag(TagManager.enemy);
        float shortestDistance = Mathf.Infinity;
        GameObject nearestEnemy = null;

        foreach (GameObject enemy in enemies)
        {
            float distanceToEnemy = Vector3.Distance(transform.position, enemy.transform.position);
            if (distanceToEnemy < shortestDistance)
            {
                shortestDistance = distanceToEnemy;
                nearestEnemy = enemy;
            }
        }

        if (nearestEnemy != null && shortestDistance <= range)
        {
            targetEnemy = nearestEnemy.GetComponent<Enemy>();
            target = targetEnemy.targetPoint;
        }
        else
        {
            target = null;
        }
    }

    private void Shoot()
    {
        foreach (var shootPoint in shootPoints)
        {
            var projectileGO = PoolManager.Instance.Get(projectilePrefab.name);
            projectileGO.transform.position = shootPoint.position;
            var projectile = projectileGO.GetComponent<Projectile>();

            if (projectile != null)
            {
                projectile.FindEnemyToShoot(targetEnemy, damage, projectileSpeed, projectileDeathDelay);
                projectile.Delegate(DestroyProjectile);
            }
        }

        animator.SetFloat("Cooldown", 1f / attackCountdown);
        animator.Play("Shoot");

        foreach (var visualEffect in visualEffects)
        {
            if (visualEffect != null)
            {
                visualEffect.Play();
            }
        }
    }

    private void DestroyProjectile(Projectile projectile)
    {
        PoolManager.Instance.Release(projectile.gameObject);
    }

    private void LookAtTarget()
    {
        Vector3 dir = transform.position - target.position;
        Quaternion lookRotation = Quaternion.LookRotation(dir);
        Vector3 rotation = Quaternion.Lerp(partToRotate.rotation, lookRotation, Time.deltaTime * turnSpeed).eulerAngles;
        partToRotate.rotation = Quaternion.Euler(0f, rotation.y, 0f);
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, range);
    }
}
