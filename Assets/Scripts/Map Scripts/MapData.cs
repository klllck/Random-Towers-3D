using System.Collections.Generic;
using UnityEngine;

public class MapData
{
    public bool[] obstacleArray;
    public List<KnightPiece> knightPiecesList;
    public List<Vector3> path;
    public List<Vector3> cornersList;
    public int cornersNearEachOther;
    public Vector3 startPosition;
    public Vector3 exitPosition;
}
