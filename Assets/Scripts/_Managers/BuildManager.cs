using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuildManager : Singleton<BuildManager>
{
    [SerializeField] private TowersDrawOverlay towersDrawOverlay;
    [SerializeField] private NodeOverlay nodeOverlay;
    [SerializeField] private TowerOverlay towerOverlay;

    private Node selectedNode;
    private TowerItem towerToBuild;

    public TowerItem TowerToBuild { get => towerToBuild; }
    public Node SelectedNode { get => selectedNode; set => selectedNode = value; }

    public bool CanBuildTower { get => TowerToBuild != null; }
    public bool CanSelectNode {  get => SelectedNode != null; }
    public bool HasCoins { get => PlayerManager.Instance.Coins >= towerToBuild.cost; }

    public void SelectTowerToBuild(TowerItem tower)
    {
        towerToBuild = tower;
    }

    public void SelectNode(Node node)
    {
        if (SelectedNode == node)
        {
            DeselectNode();
            return;
        }

        SelectedNode = node;
        nodeOverlay.SetOverlay(node);
    }

    public void SelectTower(Node node)
    {
        if (SelectedNode == node)
        {
            DeselectNode();
            return;
        }

        SelectedNode = node;
        towerOverlay.SetOverlay(node);
    }

    public void DeselectNode()
    {
        SelectedNode = null;
        if (nodeOverlay) nodeOverlay.Hide();
        if (towersDrawOverlay) towersDrawOverlay.Hide();
    }
}
