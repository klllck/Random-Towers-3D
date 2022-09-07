using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridVisualizer : MonoBehaviour
{
    public GameObject ground;

    public void VisualizeGrid(int width, int length)
    {
        var position = new Vector3(width / 2f, 0, length / 2f);
        var rotation = Quaternion.Euler(90, 0, 0);
        var board = Instantiate(ground, position, rotation);
        board.transform.localScale = new Vector3(width, length, 1);
    }
}
