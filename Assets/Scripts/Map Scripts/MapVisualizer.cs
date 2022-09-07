using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class MapVisualizer : MonoBehaviour
{
    public Transform parent;
    public Color startColor, exitColor;
    public Dictionary<Vector3, GameObject> dictionaryOfObstacles;

    public GameObject roadStraight, roadTileCorner, tileEmpty, startTile, exitTile;
    public List<GameObject> environmentTiles;
    [Range(0, 100)]
    public int resourcesSpawnFreq;

    private GameObject[] path;

    private void Awake()
    {
        parent = transform;
        dictionaryOfObstacles = new Dictionary<Vector3, GameObject>();
    }

    public void VisualizeMap(MapGrid grid, MapData data, bool visualizeWithPrefabs)
    {
        if (visualizeWithPrefabs)
        {
            VisualizeWithPrefabs(grid, data);
        }
        else
        {
            VisualizeWithPrimitives(grid, data);
        }
    }

    private void VisualizeWithPrefabs(MapGrid grid, MapData data)
    {
        path = new GameObject[data.cornersList.Count + 2];

        for (int i = 0; i < data.path.Count; i++)
        {
            var position = data.path[i];
            if (position != data.exitPosition)
            {
                grid.SetCell(position.x, position.z, CellType.Road);
            }
        }
        for (int col = 0; col < grid.Width; col++)
        {
            for (int row = 0; row < grid.Length; row++)
            {
                var node = grid.GetCell(col, row);
                var position = new Vector3(node.X, 0, node.Z);

                var index = grid.CalculateIndexFromCoordinates(position.x, position.z);
                if (data.obstacleArray[index] && !node.IsTaken)
                {
                    node.CellType = CellType.Obstacle;
                }

                var prevDirection = Direction.None;
                var nextDirection = Direction.None;

                switch (node.CellType)
                {
                    case CellType.Empty:
                        CreateIndicator(position, tileEmpty, Quaternion.Euler(0f, 0f, 0f));
                        break;
                    case CellType.Road:
                        if (data.path.Count > 0)
                        {
                            prevDirection = GetDirectionOfPreviousNode(position, data);
                            nextDirection = GetDirectionOfNextNode(position, data);
                        }
                        if (prevDirection == Direction.Up && nextDirection == Direction.Right
                            || prevDirection == Direction.Right && nextDirection == Direction.Up)
                        {
                            CreateIndicator(position, roadTileCorner, Quaternion.Euler(0, 90, 0), data);
                        }
                        else if (prevDirection == Direction.Right && nextDirection == Direction.Down
                            || prevDirection == Direction.Down && nextDirection == Direction.Right)
                        {
                            CreateIndicator(position, roadTileCorner, Quaternion.Euler(0, 180, 0), data);
                        }
                        else if (prevDirection == Direction.Down && nextDirection == Direction.Left
                            || prevDirection == Direction.Left && nextDirection == Direction.Down)
                        {
                            CreateIndicator(position, roadTileCorner, Quaternion.Euler(0, -90, 0), data);
                        }
                        else if (prevDirection == Direction.Left && nextDirection == Direction.Up
                            || prevDirection == Direction.Up && nextDirection == Direction.Left)
                        {
                            CreateIndicator(position, roadTileCorner, Quaternion.identity, data);
                        }
                        else if (prevDirection == Direction.Right && nextDirection == Direction.Left
                            || prevDirection == Direction.Left && nextDirection == Direction.Right)
                        {
                            CreateIndicator(position, roadStraight, Quaternion.Euler(0, 90, 0));
                        }
                        else
                        {
                            CreateIndicator(position, roadStraight);
                        }
                        break;
                    case CellType.Obstacle:
                        var randomIndex = Random.Range(0, environmentTiles.Count);
                        CreateIndicator(position, environmentTiles[randomIndex]);
                        break;
                    case CellType.Spawn:
                        if (data.path.Count > 0)
                        {
                            nextDirection = GetDirectionFromVectors(data.path[0], position);
                        }
                        switch (nextDirection)
                        {
                            case Direction.Right:
                                CreateIndicator(position, startTile, Quaternion.Euler(0, 90, 0), data);
                                break;
                            case Direction.Left:
                                CreateIndicator(position, startTile, Quaternion.Euler(0, -90, 0), data);
                                break;
                            case Direction.Down:
                                CreateIndicator(position, startTile, Quaternion.Euler(0, 180, 0), data);
                                break;
                            default:
                                CreateIndicator(position, startTile, Quaternion.identity, data);
                                break;
                        }
                        break;
                    case CellType.Exit:
                        if (data.path.Count > 0)
                        {
                            prevDirection = GetDirectionOfPreviousNode(position, data);
                        }
                        switch (prevDirection)
                        {
                            case Direction.Right:
                                CreateIndicator(position, exitTile, Quaternion.Euler(0, 90, 0), data);
                                break;
                            case Direction.Left:
                                CreateIndicator(position, exitTile, Quaternion.Euler(0, -90, 0), data);
                                break;
                            case Direction.Down:
                                CreateIndicator(position, exitTile, Quaternion.Euler(0, 180, 0), data);
                                break;
                            default:
                                CreateIndicator(position, exitTile, Quaternion.identity, data);
                                break;
                        }
                        break;
                    default:
                        break;
                }
            }
        }
        WaveManager.Instance.CreatePaths(path);
    }
    private void CreateIndicator(Vector3 position, GameObject prefab, Quaternion rotation = new Quaternion(), MapData data = null)
    {
        if (prefab.CompareTag(TagManager.resource))
        {
            var random = Random.Range(0, 100);
            if (random > resourcesSpawnFreq)
            {
                var environment = environmentTiles.FindAll(go => !go.CompareTag(TagManager.resource));
                prefab = environment[Random.Range(0, environment.Count)];
            }
        }

        var placementPosition = position + new Vector3(.5f, .5f, .5f);
        var gameObject = Instantiate(prefab, placementPosition, rotation);
        gameObject.transform.parent = parent;

        if (data != null)
        {
            if (position == data.startPosition)
            {
                path[0] = gameObject;
            }
            else if (position == data.exitPosition)
            {
                path[path.Length - 1] = gameObject;
            }
            else
            {
                path[data.cornersList.FindIndex(a => a == position) + 1] = gameObject;
            }
        }

        dictionaryOfObstacles.Add(position, gameObject);
    }

    private Direction GetDirectionOfNextNode(Vector3 position, MapData data)
    {
        int index = data.path.FindIndex(a => a == position);
        var nextNodePosition = data.path[index + 1];
        return GetDirectionFromVectors(nextNodePosition, position);
    }

    private Direction GetDirectionOfPreviousNode(Vector3 position, MapData data)
    {
        var index = data.path.FindIndex(a => a == position);
        var prevNodePosition = Vector3.zero;
        if (index > 0)
        {
            prevNodePosition = data.path[index - 1];
        }
        else
        {
            prevNodePosition = data.startPosition;
        }
        return GetDirectionFromVectors(prevNodePosition, position);
    }

    private Direction GetDirectionFromVectors(Vector3 positionToGo, Vector3 position)
    {
        if (positionToGo.x > position.x)
        {
            return Direction.Right;
        }
        else if (positionToGo.x < position.x)
        {
            return Direction.Left;
        }
        else if (positionToGo.z < position.z)
        {
            return Direction.Down;
        }
        return Direction.Up;
    }

    private void VisualizeWithPrimitives(MapGrid grid, MapData data)
    {
        PlaceStartAndExitPoints(data);
        PlaceObstacles(grid, data);
        PlacePath(data);
    }

    private void PlaceObstacles(MapGrid grid, MapData data)
    {
        for (int i = 0; i < data.obstacleArray.Length; i++)
        {
            if (data.obstacleArray[i])
            {
                var positionOnGrid = grid.CalculateCoordinatesFromIndex(i);
                if (positionOnGrid == data.startPosition || positionOnGrid == data.exitPosition)
                {
                    continue;
                }
                grid.SetCell(positionOnGrid.x, positionOnGrid.z, CellType.Obstacle);
                if (PlaceKnightObstacle(data, positionOnGrid))
                {
                    continue;
                }
                if (dictionaryOfObstacles.ContainsKey(positionOnGrid) == false)
                {
                    CreateIndicator(positionOnGrid, Color.white, PrimitiveType.Cube);
                }
            }
        }
    }

    private void PlacePath(MapData data)
    {
        foreach (var vertex in data.path)
        {
            if (vertex != data.exitPosition)
            {
                CreateIndicator(vertex, Color.yellow, PrimitiveType.Sphere, true);
            }
        }
    }

    private bool PlaceKnightObstacle(MapData data, Vector3 positionOnGrid)
    {
        foreach (var knight in data.knightPiecesList)
        {
            if (knight.Position == positionOnGrid)
            {
                CreateIndicator(positionOnGrid, Color.red, PrimitiveType.Cube);
                return true;
            }
        }
        return false;
    }

    private void PlaceStartAndExitPoints(MapData data)
    {
        CreateIndicator(data.startPosition, startColor, PrimitiveType.Capsule);
        CreateIndicator(data.exitPosition, exitColor, PrimitiveType.Capsule);
    }

    private void CreateIndicator(Vector3 position, Color color, PrimitiveType primitive, bool isScaled = false)
    {
        var gameObject = GameObject.CreatePrimitive(primitive);
        dictionaryOfObstacles.Add(position, gameObject);

        gameObject.transform.position = position + new Vector3(.5f, .5f, .5f);
        gameObject.transform.parent = parent;
        if (isScaled)
        {
            gameObject.transform.localScale = new Vector3(.5f, .5f, .5f);
        }

        var renderer = gameObject.GetComponent<Renderer>();
        renderer.material.SetColor("_Color", color);
    }

    public void ClearMap()
    {
        foreach (var obstacle in dictionaryOfObstacles.Values)
        {
            Destroy(obstacle);
        }
        dictionaryOfObstacles.Clear();
    }
}
