using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotationEffect : MonoBehaviour
{
    [Range(1, 10)]
    [SerializeField] private float rotationSpeed;

    void Update()
    {
        transform.Rotate(0f, 0f, rotationSpeed * Time.deltaTime);
    }
}
