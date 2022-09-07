using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class TowerItem
{
    public GameObject towerBtn;
    public GameObject tower;

    public int tier;
    public int cost;

    public int SellCost => cost / 2;
    

    private float weight;

    public float Weight { get => weight; set => weight = value; }
}
