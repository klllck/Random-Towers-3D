using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class MoneyPopup : Singleton<MoneyPopup>
{
    [SerializeField] private GameObject moneyPopup;
    [SerializeField] private float disappearTime;
    [SerializeField] private float transformSpeed;

    private TextMeshPro textMesh;
    private float disappearTimer;
    private Vector3 moveVector;
    private Action<MoneyPopup> destroyAction;

    public void Delegate(Action<MoneyPopup> destroyDp)
    {
        destroyAction = destroyDp;
    }

    void Awake()
    {
        textMesh = transform.GetComponent<TextMeshPro>();
    }

    void Update()
    {
        transform.position += moveVector * Time.deltaTime;
        moveVector += transformSpeed * Time.deltaTime * moveVector;

        disappearTimer -= Time.deltaTime;

        if (disappearTimer < 0)
        {
            destroyAction(this);
            //Destroy(gameObject);
        }
    }

    public void Setup(int damage)
    {
        textMesh.SetText("+" + damage.ToString());
        disappearTimer = disappearTime;
        moveVector = new Vector3(0, 1);
    }
}
