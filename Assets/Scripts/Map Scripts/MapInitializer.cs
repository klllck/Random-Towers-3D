using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class MapInitializer
{
    private MapGrid grid;
    private bool[] obstaclesArray = null;
    private int numberOfPieces = 5;
    private Vector3 startPoint, exitPoint;
    private List<KnightPiece> knightPiecesList;
    private List<Vector3> path;

    private List<Vector3> cornersList;
    private int cornersNearEachOtherCount;

    public MapGrid Grid { get => grid; }
    public bool[] ObstaclesArray { get => obstaclesArray; }

    public MapInitializer(MapGrid grid, int numberOfPieces)
    {
        this.grid = grid;
        this.numberOfPieces = numberOfPieces;
    }

    public MapInitializer(MapInitializer testMap)
    {
        grid = testMap.grid;
        startPoint = testMap.startPoint;
        exitPoint = testMap.exitPoint;
        obstaclesArray = (bool[])testMap.obstaclesArray.Clone();
        cornersList = new List<Vector3>(testMap.cornersList);
        cornersNearEachOtherCount = testMap.cornersNearEachOtherCount;
        path = new List<Vector3>(testMap.path);
    }

    public void CreateMap(Vector3 startPositiom, Vector3 exitPositiom, bool autoRepair = false)
    {
        startPoint = startPositiom;
        exitPoint = exitPositiom;
        obstaclesArray = new bool[grid.Width * grid.Length];

        knightPiecesList = new List<KnightPiece>();
        RandomlyPlaceKnightPieces(numberOfPieces);

        PlaceObstacles();

        path = new List<Vector3>();
        FindPath();
        if (autoRepair)
        {
            Repair();
        }
    }

    public void FindPath()
    {
        path = AStar.GetPath(startPoint, exitPoint, obstaclesArray, grid);
        cornersList = GetListOfCorners(path);
        cornersNearEachOtherCount = CalculateCornersNearEachOther(cornersList);
    }

    private int CalculateCornersNearEachOther(List<Vector3> cornersList)
    {
        int cornersNearEachOther = 0;
        for (int i = 0; i < cornersList.Count - 1; i++)
        {
            if (Vector3.Distance(cornersList[i], cornersList[i + 1]) <= 1)
            {
                cornersNearEachOther++;
            }
        }
        return cornersNearEachOther;
    }

    private List<Vector3> GetListOfCorners(List<Vector3> path)
    {
        var pathWithStart = new List<Vector3>(path);
        pathWithStart.Insert(0, startPoint);
        var cornersPositions = new List<Vector3>();
        if (pathWithStart.Count <= 0)
        {
            return cornersPositions;
        }
        for (int i = 0; i < pathWithStart.Count - 2; i++)
        {
            if (pathWithStart[i + 1].x > pathWithStart[i].x || pathWithStart[i + 1].x < pathWithStart[i].x)
            {
                if (pathWithStart[i + 2].z > pathWithStart[i + 1].z || pathWithStart[i + 2].z < pathWithStart[i + 1].z)
                {
                    cornersPositions.Add(pathWithStart[i + 1]);
                }
            }
            else if (pathWithStart[i + 1].z > pathWithStart[i].z || pathWithStart[i + 1].z < pathWithStart[i].z)
            {
                if (pathWithStart[i + 2].x > pathWithStart[i + 1].x || pathWithStart[i + 2].x < pathWithStart[i + 1].x)
                {
                    cornersPositions.Add(pathWithStart[i + 1]);
                }
            }
        }
        return cornersPositions;
    }

    private void RandomlyPlaceKnightPieces(int numberOfPieces)
    {
        var count = numberOfPieces;
        var knightPlacementTryLimit = 100;

        while (count > 0 && knightPlacementTryLimit > 0)
        {
            var randomIndex = Random.Range(0, obstaclesArray.Length);
            if (obstaclesArray[randomIndex] == false)
            {
                var coordinates = grid.CalculateCoordinatesFromIndex(randomIndex);
                if (coordinates == startPoint || coordinates == exitPoint)
                {
                    continue;
                }
                obstaclesArray[randomIndex] = true;
                knightPiecesList.Add(new KnightPiece(coordinates));
                count--;
            }
            knightPlacementTryLimit--;
        }
    }

    private void PlaceObstacles()
    {
        foreach (var knight in knightPiecesList)
        {
            PlaceObstaclesForThisKnight(knight);
        }
    }

    private void PlaceObstaclesForThisKnight(KnightPiece knight)
    {
        foreach (var position in KnightPiece.listOfPossibleMoves)
        {
            var newPosition = knight.Position + position;
            if (grid.IsCellValid(newPosition.x, newPosition.z)
                && CheckIfPositionCanBeObstacle(newPosition))
            {
                obstaclesArray[grid.CalculateIndexFromCoordinates(newPosition.x, newPosition.z)] = true;
            }
        }
    }

    private bool CheckIfPositionCanBeObstacle(Vector3 position)
    {
        if (position == startPoint || position == exitPoint)
        {
            return false;
        }
        int index = grid.CalculateIndexFromCoordinates(position.x, position.z);

        return obstaclesArray[index] == false;
    }

    public List<Vector3> Repair()
    {
        int numberOfObstacles = obstaclesArray.Where(obs => obs).Count();
        var listOfObstaclesToRemove = new List<Vector3>();
        if (path.Count <= 0)
        {
            do
            {
                int obstacleIndexToRemove = Random.Range(0, numberOfObstacles);
                for (int i = 0; i < obstaclesArray.Length; i++)
                {
                    if (obstaclesArray[i])
                    {
                        if (obstacleIndexToRemove == 0)
                        {
                            obstaclesArray[i] = false;
                            listOfObstaclesToRemove.Add(grid.CalculateCoordinatesFromIndex(i));
                            break;
                        }
                        obstacleIndexToRemove--;
                    }
                }
                FindPath();

            } while (path.Count <= 0);
        }
        foreach (var obstaclePosition in listOfObstaclesToRemove)
        {
            if (path.Contains(obstaclePosition) == false)
            {
                int index = grid.CalculateIndexFromCoordinates(obstaclePosition.x, obstaclePosition.z);
                obstaclesArray[index] = true;
            }
        }
        return listOfObstaclesToRemove;
    }

    public bool IsObstacleAt(int i)
    {
        return obstaclesArray[i];
    }

    public void PlaceObstacle(int i, bool isObstacle)
    {
        obstaclesArray[i] = isObstacle;
    }

    public void AddMutation(double mutationRate)
    {
        int numItems = (int)(obstaclesArray.Length * mutationRate);
        while (numItems > 0)
        {
            int randomIndex = Random.Range(0, obstaclesArray.Length);
            obstaclesArray[randomIndex] = !obstaclesArray[randomIndex];
            numItems--;
        }
    }

    public MapInitializer DeepClone()
    {
        return new MapInitializer(this);
    }

    public MapData ReturnMapData()
    {
        return new MapData
        {
            obstacleArray = obstaclesArray,
            knightPiecesList = knightPiecesList,
            path = path,
            cornersList = cornersList,
            cornersNearEachOther = cornersNearEachOtherCount,
            startPosition = startPoint,
            exitPosition = exitPoint
        };
    }
}
