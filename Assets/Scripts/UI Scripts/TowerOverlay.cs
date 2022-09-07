using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class TowerOverlay : MonoBehaviour
{
	[SerializeField] private GameObject towerOverlay;
	[SerializeField] private int upgradeCost;
	[SerializeField] private float damageUpgrade;
	[SerializeField] private int attackSpeedUpgrade;
	[SerializeField] private int rangeUpgrade;

	[Header("UI Elements")]
	[SerializeField] private TextMeshProUGUI towerName; 
	[SerializeField] private TextMeshProUGUI towerDamage;
	[SerializeField] private TextMeshProUGUI towerAttackSpeed;
	[SerializeField] private TextMeshProUGUI towerRange;
	[SerializeField] private TextMeshProUGUI sellCost;
	[SerializeField] private TextMeshProUGUI upgradeCostTxt;
	[SerializeField] private TextMeshProUGUI sellUpgradeCostTxt;

	private Node targetNode;
	private Tower tower;
	private GameObject currentTower;

	public void SetOverlay(Node node)
	{
		targetNode = node;
		transform.position = targetNode.transform.position;
		currentTower = node.PlacedTower;
		Show();
	}

	private void Update()
    {
        towerName.text = targetNode.currentTowerItem.tower.name;
        upgradeCostTxt.text = "$" + upgradeCost.ToString();
        sellUpgradeCostTxt.text = "$" + GetUpgradeSellCost().ToString();
        sellCost.text = "$" + targetNode.currentTowerItem.SellCost.ToString();

        tower = currentTower.GetComponent<Tower>();
        towerDamage.text = tower.Damage.ToString();
        towerAttackSpeed.text = tower.BulletsPerMinute.ToString();
        towerRange.text = tower.Range.ToString();

        CheckIfPlayerHasEnoughtCoins();
    }

    private void CheckIfPlayerHasEnoughtCoins()
    {
		if (PlayerManager.Instance.Coins < targetNode.currentTowerItem.SellCost)
		{
			if (PlayerManager.Instance.Coins < upgradeCost)
			{
				upgradeCostTxt.color = Color.red;
			}
			else
			{
				upgradeCostTxt.color = Color.yellow;
			}
		}
    }

    public void DamageUpgrade(bool isUpgrade)
	{
		if (isUpgrade)
		{
			if (PlayerManager.Instance.Coins < upgradeCost)
			{
				return;
			}

			if (tower.Damage >= float.MaxValue)
			{
				tower.Damage = float.MaxValue;
				return;
			}
			tower.Damage += damageUpgrade;
			PlayerManager.Instance.Coins -= upgradeCost;
		} 
		else
        {
			if (tower.Damage <= 0f)
			{
				tower.Damage = 0f;
				return;
			}
			tower.Damage -= damageUpgrade;
			PlayerManager.Instance.Coins += GetUpgradeSellCost();
		}
	}


	public void AttackSpeedUpgrade(bool isUpgrade)
	{
		if (isUpgrade)
        {
			if (PlayerManager.Instance.Coins < upgradeCost)
			{
				return;
			}

			if (tower.BulletsPerMinute >= float.MaxValue)
			{
				tower.BulletsPerMinute = float.MaxValue;
				return;
			}
			tower.BulletsPerMinute += attackSpeedUpgrade;
			PlayerManager.Instance.Coins -= upgradeCost;
		}
        else
        {
			if (tower.BulletsPerMinute <= 0f)
			{
				tower.BulletsPerMinute = 0f;
				return;
			}
			tower.BulletsPerMinute -= attackSpeedUpgrade;
			PlayerManager.Instance.Coins += GetUpgradeSellCost();
		}
	}

	public void RangeUpgrade(bool isUpgrade)
	{
        if (isUpgrade)
        {
			if (PlayerManager.Instance.Coins < upgradeCost)
			{
				return;
			}

			if (tower.Range >= float.MaxValue)
			{
				tower.Range = float.MaxValue;
				return;
			}
			tower.Range += rangeUpgrade;
			PlayerManager.Instance.Coins -= upgradeCost;
        }
        else
		{
			if (tower.Range <= 0f)
			{
				tower.Range = 0f;
				return;
			}
			tower.Range -= rangeUpgrade;
			PlayerManager.Instance.Coins += GetUpgradeSellCost();
		}
	}

	public void SellTower()
	{
		if (currentTower) currentTower = null;
		targetNode.SellTower();
		Hide();
	}

	private int GetUpgradeSellCost()
	{
		return upgradeCost / 2;
	}

	public void Show()
	{
		towerOverlay.SetActive(true);
	}

	public void Hide()
	{
		if (currentTower) currentTower = null;
		targetNode.HideOverlay();
		towerOverlay.SetActive(false);
	}
}
