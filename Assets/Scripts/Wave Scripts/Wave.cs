using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Wave : MonoBehaviour {
    public Difficulties difficulty;
    public PathTypes pathType;
    public EnemyBuilder[] enemies;

    public int EnemiesCount {
        get {
            int count = 0;
            foreach (var enemy in enemies) {
                count += enemy.amount;
            }

            return count;
        }
    }
}
