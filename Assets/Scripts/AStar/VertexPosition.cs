using System;
using System.Collections.Generic;
using UnityEngine;

public class VertexPosition : IEquatable<VertexPosition>, IComparable<VertexPosition>
{
    public static List<Vector2Int> possibleNeighbours = new List<Vector2Int>()
    {
        new Vector2Int(0, -1),
        new Vector2Int(0, 1),
        new Vector2Int(1, 0),
        new Vector2Int(-1, 0)
    };

    public float totalCost, estimatedCost;
    public VertexPosition previousVertex = null;
    private Vector3 position;
    private bool isTaken;

    public Vector3 Position { get => position; }
    public int X { get => (int)position.x; }
    public int Z { get => (int)position.z; }
    public bool IsTaken { get => isTaken; }

    public VertexPosition(Vector3 position, bool isTaken = false)
    {
        this.position = position;
        this.isTaken = isTaken;
        estimatedCost = 0;
        totalCost = 1;
    }

    public int GetHashCode(VertexPosition vertex)
    {
        return vertex.position.GetHashCode();
    }

    public override int GetHashCode()
    {
        return position.GetHashCode();
    }

    public int CompareTo(VertexPosition other)
    {
        if (estimatedCost < other.estimatedCost) return -1;
        if (estimatedCost > other.estimatedCost) return 1;
        return 0;
    }

    public bool Equals(VertexPosition other)
    {
        return Position == other.Position;
    }
}
