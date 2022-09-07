using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;
using UnityEngine.UI;

public abstract class Enemy : MonoBehaviour
{

    [Header("Stats")]
    [SerializeField] protected float health;
    [SerializeField] protected float speed;
    [SerializeField] protected float damage;
    [SerializeField] protected int gold;

    protected float currentHealth;
    protected float currentSpeed;
    protected float currentDamage;
    protected int currentGold;

    protected int waypointIndex;
    protected Vector3 waypointPos;
    protected Vector3 moveDirection;

    [Header("Setup")]
    [SerializeField] private Image healthBar;
    [SerializeField] private GameObject damagePopupPrefab;
    [SerializeField] private GameObject rewardPopupPrefab;
    [SerializeField] private Outline outline;
    [SerializeField] private Animator animator;
    public Transform targetPoint;

    protected virtual void Start()
    {
        outline = GetComponent<Outline>();
        outline.OutlineColor = Color.red;
        outline.OutlineWidth = 0;
    }

    private void OnMouseOver()
    {
        outline.OutlineWidth = 3f;
    }

    private void OnMouseExit()
    {
        outline.OutlineWidth = 0f;

    }

    public void SetStats(int waypointIndex)
    {
        this.waypointIndex = waypointIndex;
        currentHealth = health;
        currentSpeed = speed;
        currentDamage = damage;
        currentGold = gold;
        healthBar.fillAmount = health;
        animator.SetFloat("Move Speed", speed);
        FindNextWaypoint();
    }

    private void Update()
    {
        if (!isActiveAndEnabled)
        {
            return;
        }

        LookAtDirection();
        transform.Translate(moveDirection * currentSpeed * Time.deltaTime, Space.World);

        if (Vector3.Distance(transform.position, waypointPos) <= 0.2f)
        {
            FindNextWaypoint();
        }
    }

    protected void FindNextWaypoint()
    {
        if (waypointIndex >= WaveManager.Instance.paths.transform.childCount - 1)
        {
            DealDamage();
            return;
        }
        waypointIndex++;
        waypointPos = WaveManager.Instance.paths.transform.GetChild(waypointIndex).position;
        moveDirection = (waypointPos - transform.position).normalized;
    }

    protected void LookAtDirection()
    {
        Quaternion lookRotation = Quaternion.LookRotation(moveDirection);
        Vector3 rotation = Quaternion.Lerp(transform.rotation, lookRotation, Time.deltaTime * speed * 2).eulerAngles;
        transform.rotation = Quaternion.Euler(0f, rotation.y, 0f);
    }

    public virtual void TakeDamage(float damage)
    {
        if (!isActiveAndEnabled)
        {
            return;
        }
        currentHealth -= damage;
        healthBar.fillAmount = currentHealth / health;

        if (damagePopupPrefab != null && damage >= 1)
        {
            var damagePopupGO = PoolManager.Instance.Get(damagePopupPrefab.name);
            damagePopupGO.transform.SetPositionAndRotation(transform.position + new Vector3(0, .6f, 0), damagePopupGO.transform.rotation);
            var damagePopup = damagePopupGO.GetComponent<DamagePopup>();
            damagePopup.Setup((int)damage);
            damagePopup.Delegate(DestroyDamagePopup);
        }
        if (currentHealth <= 0)
        {
            Die();
        }
    }

    protected virtual void DealDamage()
    {
        PlayerManager.Instance.Lives -= (int)currentDamage;
        PoolManager.Instance.Release(gameObject);
    }

    protected virtual void Die()
    {
        PlayerManager.Instance.Coins += currentGold;

        var rewardPopupGO = PoolManager.Instance.Get(rewardPopupPrefab.name);
        rewardPopupGO.transform.SetPositionAndRotation(transform.position + new Vector3(0, 1f, 0), rewardPopupGO.transform.rotation);
        var rewardPopup = rewardPopupGO.GetComponent<MoneyPopup>();
        rewardPopup.Setup(gold);
        rewardPopup.Delegate(DestroyMoneyPopup);

        PoolManager.Instance.Release(gameObject);
    }

    private void DestroyDamagePopup(DamagePopup damagePopup)
    {
        PoolManager.Instance.Release(damagePopup.gameObject);
    }

    private void DestroyMoneyPopup(MoneyPopup moneyPopup)
    {
        PoolManager.Instance.Release(moneyPopup.gameObject);
    }
}
