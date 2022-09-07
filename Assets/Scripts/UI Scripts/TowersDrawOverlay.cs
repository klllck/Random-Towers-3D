using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class TowersDrawOverlay : Singleton<TowersDrawOverlay>
{
    //[SerializeField] private GameObject closeBtn;
    [SerializeField] private LootTable lootTable;
    [SerializeField] private Transform towersSelect;
    [SerializeField] private int towerBtnsCount;

    public List<TowerItem> towerItems;

    private Node targetNode;

    private void Awake()
    {
        Instance = this;
        Hide();
    }

    public void SetOverlay()
    {
        RerollTowers();
        Show();
    }

    public void RerollTowers()
    {
        ClearOverlay();

        for (int i = 0; i < towerBtnsCount; i++)
        {
            var towerBtn = Instantiate(
                    lootTable.GetRandomItem().towerBtn,
                    towersSelect
                );
        }

        //Instantiate(closeBtn).transform.SetParent(transform);
    }

    private void ClearOverlay()
    {
        var childs = transform.GetComponentsInChildren<Transform>();

        foreach (var item in childs)
        {
            if (item.CompareTag(TagManager.button) || item.CompareTag(TagManager.towerIcon))
            {
                Destroy(item.gameObject);
            }
        }
    }

    public void SelectTower(GameObject towerObject)
    {
        targetNode = BuildManager.Instance.SelectedNode;
        targetNode.BuyTower(Instance.towerItems.First(s => s.tower.name == towerObject.name));
    }

    public void SelectBasicBlaster()
    {
        targetNode = BuildManager.Instance.SelectedNode;
        targetNode.BuyTower(Instance.towerItems.First(s => s.tower.name == "BasicBlaster"));
    }

    public void SelectDominator2000()
    {
        targetNode = BuildManager.Instance.SelectedNode;
        targetNode.BuyTower(Instance.towerItems.First(s => s.tower.name == "Dominator2000"));
    }

    public void SelectTowerBasic()
    {
        targetNode = BuildManager.Instance.SelectedNode;
        targetNode.BuyTower(Instance.towerItems.First(s => s.tower.name == "TowerBasic"));
    }

    public void SelectMissileTower()
    {
        targetNode = BuildManager.Instance.SelectedNode;
        targetNode.BuyTower(Instance.towerItems.First(s => s.tower.name == "MissleTower"));
    }

    public void SelectLaserTower()
    {
        targetNode = BuildManager.Instance.SelectedNode;
        targetNode.BuyTower(Instance.towerItems.First(s => s.tower.name == "LaserTower"));
    }

    private void Show()
    {
        gameObject.SetActive(true);
    }

    public void Hide()
    {
        gameObject.SetActive(false);
    }
}
