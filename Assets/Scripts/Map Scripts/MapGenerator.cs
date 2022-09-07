using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapGenerator : MonoBehaviour
{
    public GridVisualizer gridVisualizer;
    public MapVisualizer mapVisualizer;
    public Direction startEdge, exitEdge;
    public bool randomPlacement;
    public bool visualizePrefabs = false;
    public bool autoRepair = true;

    [Range(1, 10)]
    public int numberOfPieces;

    [Range(3, 30)]
    public int width, length;

    private MapGrid grid;
    private MapInitializer map;
    private Vector3 startPosition, exitPosition;

    private void Start()
    {
        gridVisualizer.VisualizeGrid(width, length);
        GenerateNewMap();
    }

    public void GenerateNewMap()
    {
        mapVisualizer.ClearMap();
        grid = new MapGrid(width, length);
        MapHelper.RandomlyChooseAndSetStartAndExit(grid, ref startPosition, ref exitPosition, randomPlacement, startEdge, exitEdge);
        map = new MapInitializer(grid, numberOfPieces);
        map.CreateMap(startPosition, exitPosition, autoRepair);
        mapVisualizer.VisualizeMap(grid, map.ReturnMapData(), visualizePrefabs);
    }

    public void RepairMap()
    {
        if(map != null)
        {
            var listOfObstaclesToRemove = map.Repair();
            if(listOfObstaclesToRemove.Count > 0)
            {
                mapVisualizer.ClearMap();
                mapVisualizer.VisualizeMap(grid, map.ReturnMapData(), visualizePrefabs);
            }
        }
    }
}
