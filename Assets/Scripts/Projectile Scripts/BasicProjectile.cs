using UnityEngine;

public class BasicProjectile : Projectile
{
    [Header("Projectile Stats")]
    public float tickTime;
    public float tickDamage;
    public int splashDamage;

    public override void Update()
    {
        base.Update();
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
