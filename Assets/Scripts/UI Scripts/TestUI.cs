using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestUI : MonoBehaviour
{
    public void StartNewGame()
    {
        ClearAll();
        MapBrain.Instance.RunAlgorithm();
    }

    public void ClearAll()
    {
        WaveManager.Instance.Reset();
        DestroyAll(GameObject.FindGameObjectsWithTag(TagManager.tower));
        DestroyAll(GameObject.FindGameObjectsWithTag(TagManager.projectile));
        DestroyAll(GameObject.FindGameObjectsWithTag(TagManager.enemy));
    }

    private void DestroyAll(GameObject[] gameObjects)
    {
        foreach (var gameObj in gameObjects)
        {
            if (gameObj.activeInHierarchy)
            {
                Destroy(gameObj);
            }
        }
    }
}
