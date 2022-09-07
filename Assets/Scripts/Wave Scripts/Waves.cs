using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Waves : MonoBehaviour
{
    public static List<Wave> waves;

    private void Awake() {
        waves = new List<Wave>();

        for (int i = 0; i < transform.childCount; i++) {
            waves.Add(transform.GetChild(i).GetComponent<Wave>());
        }
    }
}
