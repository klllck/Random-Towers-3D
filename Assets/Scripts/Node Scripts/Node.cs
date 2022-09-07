using UnityEngine;
using UnityEngine.EventSystems;

public class Node : Singleton<Node>
{
    [SerializeField] private Color hoverColor;
    [SerializeField] private Color activeColor;
    [SerializeField] private Vector3 placementOffset;
    [SerializeField] private GameObject placedTower;
    public TowerItem currentTowerItem;

    private BuildManager buildManager;
    private Renderer rend;
    private Color startColor;

    public GameObject PlacedTower { get => placedTower; set => placedTower = value; }

    private void Start()
    {
        rend = GetComponent<Renderer>();
        startColor = rend.material.color;

        buildManager = BuildManager.Instance;
    }

    private void OnMouseUp()
    {
        if (!EventSystem.current.IsPointerOverGameObject())
        {
            if (buildManager.CanSelectNode)
            {
                buildManager.SelectedNode.rend.material.color = startColor;
                buildManager.DeselectNode();
            }

            if (placedTower != null)
            {
                buildManager.SelectTower(this);
                rend.material.color = activeColor;
                return;
            }

            buildManager.SelectNode(this);
            rend.material.color = activeColor;
        }
    }

    public void BuyTower(TowerItem towerItem)
    {
        buildManager.SelectTowerToBuild(towerItem);
        if (!buildManager.CanBuildTower)
        {
            return;
        }

        if (placedTower != null)
        {
            return;
        }

        if (!buildManager.HasCoins)
        {
            return;
        }

        currentTowerItem = towerItem;
        PlayerManager.Instance.Coins -= towerItem.cost;
        GameObject towerPrefab = buildManager.TowerToBuild.tower;
        placedTower = Instantiate(towerPrefab, GetBuildPosition(), towerPrefab.transform.rotation);
        buildManager.DeselectNode();
        rend.material.color = startColor;
    }

    public void SellTower()
    {
        if (placedTower != null)
        {
            PlayerManager.Instance.Coins += currentTowerItem.SellCost;
            Destroy(placedTower);
            currentTowerItem = null;
        }
        rend.material.color = startColor;
    }

    public void HideOverlay()
    {
        rend.material.color = startColor;
    }

    public Vector3 GetBuildPosition()
    {
        return transform.position + placementOffset;
    }

    private void OnMouseEnter()
    {
        if (!EventSystem.current.IsPointerOverGameObject())
        {
            if (rend.material.color != activeColor)
            {
                rend.material.color = hoverColor;
            }
        }
    }

    private void OnMouseExit()
    {
        if (rend.material.color != activeColor)
        {
            rend.material.color = startColor;
        }
    }
}
