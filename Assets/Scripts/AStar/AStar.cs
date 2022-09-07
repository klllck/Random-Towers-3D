using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class AStar
{
    public static List<Vector3> GetPath(Vector3 start, Vector3 exit, bool[] obstaclesArray, MapGrid grid)
    {
        //
        var startVertex = new VertexPosition(start);
        var exitVertex = new VertexPosition(exit);

        var path = new List<Vector3>();

        var openList = new List<VertexPosition>();
        var closedList = new HashSet<VertexPosition>();

        startVertex.estimatedCost = ManhattanDistance(startVertex, exitVertex);
        openList.Add(startVertex);
        VertexPosition currentVertex = null;

        while (openList.Count > 0)
        {
            openList.Sort();
            currentVertex = openList[0];

            if (currentVertex.Equals(exitVertex))
            {
                while (currentVertex != startVertex)
                {
                    path.Add(currentVertex.Position);
                    currentVertex = currentVertex.previousVertex;
                }
                path.Reverse();
                break;
            }
            var arrayOfNeighbours = FindNeighbours(currentVertex, grid, obstaclesArray);
            foreach (var neighbour in arrayOfNeighbours)
            {
                if (neighbour == null || closedList.Contains(neighbour))
                {
                    continue;
                }
                if (neighbour.IsTaken == false)
                {
                    var totalCost = currentVertex.totalCost + 1;
                    var neighbourEstimatedCost = ManhattanDistance(neighbour, exitVertex);
                    neighbour.totalCost = totalCost;
                    neighbour.previousVertex = currentVertex;
                    neighbour.estimatedCost = totalCost + neighbour.estimatedCost;
                    if (openList.Contains(neighbour) == false)
                    {
                        openList.Add(neighbour);
                    }
                }
            }
            closedList.Add(currentVertex);
            openList.Remove(currentVertex);
        }

        return path;
    }

    private static VertexPosition[] FindNeighbours(VertexPosition currentVertex, MapGrid grid, bool[] obstaclesArray)
    {
        var arrayOfNeighbours = new VertexPosition[4];

        int arrayIndex = 0;
        foreach (var possibleNeighbour in VertexPosition.possibleNeighbours)
        {
            var position = new Vector3(
                currentVertex.X + possibleNeighbour.x,
                0,
                currentVertex.Z + possibleNeighbour.y);

            if (grid.IsCellValid(position.x, position.z)) 
            {
                int index = grid.CalculateIndexFromCoordinates(position.x, position.z);
                arrayOfNeighbours[arrayIndex] = new VertexPosition(position, obstaclesArray[index]);
                arrayIndex++;
            }
        }
        return arrayOfNeighbours;
    }

    private static float ManhattanDistance(VertexPosition startVertex, VertexPosition exitVertex)
    {
        return Mathf.Abs(startVertex.X - exitVertex.X) + Mathf.Abs(startVertex.Z - exitVertex.Z);
    }
}
