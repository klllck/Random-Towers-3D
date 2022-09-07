using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class MapHelper
{
    public static void RandomlyChooseAndSetStartAndExit(
        MapGrid grid,
        ref Vector3 startPosition,
        ref Vector3 exitPosition,
        bool randomPlacement,
        Direction startPositionEdge = Direction.Left,
        Direction exitPositionEdge = Direction.Right)
    {
        if (randomPlacement)
        {
            startPosition = RandomlyChoosePositionOnTheEdgeOfTheMap(grid, startPosition);
            exitPosition = RandomlyChoosePositionOnTheEdgeOfTheMap(grid, startPosition);
        }
        else
        {
            startPosition = RandomlyChoosePositionOnTheEdgeOfTheMap(grid, startPosition, startPositionEdge);
            exitPosition = RandomlyChoosePositionOnTheEdgeOfTheMap(grid, startPosition, exitPositionEdge);
        }
        grid.SetCell(startPosition.x, startPosition.z, CellType.Spawn, true);
        grid.SetCell(exitPosition.x, exitPosition.z, CellType.Exit, true);
    }

    private static Vector3 RandomlyChoosePositionOnTheEdgeOfTheMap(
        MapGrid grid,
        Vector3 startPosition,
        Direction direction = Direction.None)
    {
        if (direction == Direction.None)
        {
            direction = (Direction)Random.Range(1, 5);
        }
        Vector3 position = Vector3.zero;
        switch (direction)
        {
            case Direction.Right:
                do
                {
                    position = new Vector3(grid.Width - 1, 0, Random.Range(0, grid.Length));
                } while (Vector3.Distance(position, startPosition) <= 1);
                break;
            case Direction.Left:
                do
                {
                    position = new Vector3(0, 0, Random.Range(0, grid.Length));
                } while (Vector3.Distance(position, startPosition) <= 1);
                break;
            case Direction.Up:
                do
                {
                    position = new Vector3(Random.Range(0, grid.Length), 0, grid.Length - 1);
                } while (Vector3.Distance(position, startPosition) <= 1);
                break;
            case Direction.Down:
                do
                {
                    position = new Vector3(Random.Range(0, grid.Length), 0, 0);
                } while (Vector3.Distance(position, startPosition) <= 1);
                break;
            default:
                break;
        }
        return position;
    }
}
