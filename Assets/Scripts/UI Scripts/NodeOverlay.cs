using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NodeOverlay : MonoBehaviour
{
	[SerializeField] private GameObject nodeOverlay;

	private Node targetNode;

	public void SetOverlay(Node node)
	{
		targetNode = node;

		//transform.position = targetNode.GetBuildPosition();
		transform.position = targetNode.transform.position;

		Show();
	}

	public void Show()
    {
		nodeOverlay.SetActive(true);
    }

	public void Hide()
	{
		targetNode.HideOverlay();
		nodeOverlay.SetActive(false);
	}

	public void Roll()
    {
		TowersDrawOverlay.Instance.SetOverlay();
		Hide();
	}

	public void Sell()
	{
		targetNode.SellTower();
		BuildManager.Instance.DeselectNode();
	}
}
