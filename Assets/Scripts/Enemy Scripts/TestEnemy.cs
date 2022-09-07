using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestEnemy : Enemy
{
    //[Header("Effects")]
    //public GameObject echo;
    //public float echoFrequency = .1f;
    //float residualEchoFrequency;

    //void Awake()
    //{
    //    health = 200;
    //    speed = 1;
    //    SetStats();
    //}

    //void Update()
    //{
    //    if (isMoving)
    //    {
    //        if (residualEchoFrequency <= 0)
    //        {
    //            GameObject echo = Instantiate(this.echo, transform.position, Quaternion.identity);
    //            Destroy(echo, 1f);
    //            residualEchoFrequency = echoFrequency;
    //        }
    //        else
    //        {
    //            residualEchoFrequency -= Time.deltaTime;
    //        }
    //    }
    //}

    protected override void Die()
    {
        Destroy(gameObject);
    }
}
