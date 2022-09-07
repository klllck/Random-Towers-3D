using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class LootTable : Singleton<LootTable>
{
    [SerializeField, Range(1, 5)]
    private int lvl;
    private List<TowerItem> towerItems;

    private List<TowerItem> eachTierTowers;
    private float totalWeight;
    private float lootChance;

    //column = main level
    //row = tower's tier
    private readonly float[,] chanceTable =
    {
        { 65f, 50f,   42f,   37.5f, 30f },
        { 30f, 42.5f, 45.5f, 47f,   50f },
        { 5f,  7.5f,  12.5f, 15.5f, 20f }
    };

    public float TotalWeight { get => totalWeight; }
    public float LootChance { get => lootChance; set => lootChance = value; }

    public TowerItem GetRandomItem()
    {
        towerItems = TowersDrawOverlay.Instance.towerItems;
        eachTierTowers = towerItems.GroupBy(g => g.tier).Select(i => i.First()).ToList();
        CalculateWeights();
        totalWeight = eachTierTowers.Sum(item => item.Weight);

        lootChance = Random.Range(0, maxInclusive: totalWeight);

        foreach (var item in eachTierTowers)
        {
            if (item.Weight >= lootChance)
            {
                var currnetTowerTier = item.tier;
                var towersWithSameTier = towerItems.Where(t => t.tier == currnetTowerTier).ToArray();

                return towersWithSameTier[Random.Range(0, towersWithSameTier.Length)];
            }
            lootChance -= item.Weight;
        }

        throw new System.Exception("Random item generation failed!");
    }

    private void CalculateWeights()
    {
        foreach (var item in eachTierTowers)
        {
            if (item.tier == 1)
            {
                item.Weight = GetLootChanseForCurrentLevel(item.tier, lvl);
            }
            if (item.tier == 2)
            {
                item.Weight = GetLootChanseForCurrentLevel(item.tier, lvl);
            }
            if (item.tier == 3)
            {
                item.Weight = GetLootChanseForCurrentLevel(item.tier, lvl);
            }
        }
    }

    private float GetLootChanseForCurrentLevel(int tier, int lvl)
    {
        return chanceTable[tier - 1, lvl - 1];
    }
}
