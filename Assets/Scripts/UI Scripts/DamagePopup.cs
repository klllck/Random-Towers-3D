using UnityEngine;
using TMPro;
using System;
using Random = UnityEngine.Random;

public class DamagePopup : Singleton<DamagePopup>
{
    [SerializeField] private GameObject damagePopupPrefab;
    private TextMeshPro textMesh;
    private float disappearTimer;
    private Color textColor;
    private Vector3 moveVector;
    private const float DISAPPEAR_TIMER_MAX = 0.8f;
    private Action<DamagePopup> destroyAction;

    private Color defaultColor;

    public void Delegate(Action<DamagePopup> destroyDp)
    {
        destroyAction = destroyDp;
    }

    void Awake()
    {
        textMesh = transform.GetComponent<TextMeshPro>();
        defaultColor = textMesh.color;
    }

    void Update()
    {
        transform.position += moveVector * Time.deltaTime;
        moveVector -= 3.2f * Time.deltaTime * moveVector;

        if (disappearTimer > DISAPPEAR_TIMER_MAX * 0.5f)
        {
            transform.localScale += 0.4f * Time.deltaTime * Vector3.one;
        }
        else
        {
            transform.localScale -= 1.2f * Time.deltaTime * Vector3.one;
        }

        disappearTimer -= Time.deltaTime;
        if (disappearTimer < 0)
        {
            float disappearSpeed = 30f;
            textColor.a -= disappearSpeed * Time.deltaTime;
            textMesh.color = textColor;
            if (textColor.a < 0)
            {
                //Need to fix this
                //destroyAction(this);
                Destroy(gameObject);
            }
        }
    }

    public void Setup(int damage)
    {
        textMesh.SetText(damage.ToString());

        textMesh.color = defaultColor;
        textColor = defaultColor;

        disappearTimer = DISAPPEAR_TIMER_MAX;

        moveVector = new Vector3(Random.Range(-4, 4), 5);
    }
}
