using System.Text;
using UnityEngine;

public class MapGrid 
{
    private int width, length;
    private Cell[,] grid;

    public int Width { get => width; }
    public int Length { get => length; }

    public MapGrid(int width, int length)
    {
        this.width = width;
        this.length = length;
        CreateGrid();
    }

    private void CreateGrid()
    {
        grid = new Cell[width, length];
        for (int row = 0; row < length; row++)
        {
            for (int col = 0; col < width; col++)
            {
                grid[row, col] = new Cell(col, row);
            }
        }
    }

    public void SetCell(int x, int z, CellType nodeType, bool isTaken = false)
    {
        grid[z, x].CellType = nodeType;
        grid[z, x].IsTaken = isTaken;
    }

    public void SetCell(float x, float z, CellType nodeType, bool isTaken = false)
    {
        SetCell((int)x, (int)z, nodeType, isTaken);
    }

    public void SetCell(Vector3 vector, CellType nodeType, bool isTaken = false)
    {
        SetCell((int)vector.x, (int)vector.z, nodeType, isTaken);
    }

    public bool IsCellTaken(int x, int z)
    {
        return grid[x, z].IsTaken;
    }

    public bool IsCellTaken(float x, float z)
    {
        return grid[(int)x, (int)z].IsTaken;
    }

    public bool IsCellTaken(Vector3 vector)
    {
        return grid[(int)vector.x, (int)vector.z].IsTaken;
    }

    public bool IsCellValid(int x, int z)
    {
        if (x >= width || x < 0 || z >= length || z < 0) return false;
        return true;
    }

    public bool IsCellValid(float x, float z)
    {
        if (x >= width || x < 0 || z >= length || z < 0) return false;
        return true;
    }

    public bool IsCellValid(Vector3 vector)
    {
        if (vector.x >= width || vector.x < 0 || vector.z >= length || vector.z < 0) return false;
        return true;
    }

    public Cell GetCell(int x, int z)
    {
        if (!IsCellValid(x, z)) return null;
        return grid[x, z];
    }

    public Cell GetCell(float x, float z)
    {
        if (!IsCellValid(x, z)) return null;
        return grid[(int)x, (int)z];
    }

    public Cell GetCell(Vector3 vector)
    {
        if (!IsCellValid(vector)) return null;
        return grid[(int)vector.x, (int)vector.z];
    }

    public int CalculateIndexFromCoordinates(int x, int z)
    {
        return x + z * width;
    }

    public int CalculateIndexFromCoordinates(float x, float z)
    {
        return (int)x + (int)z * width;
    }

    public int CalculateIndexFromCoordinates(Vector3 vector)
    {
        return (int)vector.x + (int)vector.z * width;
    }

    public Vector3 CalculateCoordinatesFromIndex(int randomIndex)
    {
        int x = randomIndex % width;
        int z = randomIndex / width;
        return new Vector3(x, 0, z);
    }

    public void CheckCoordinates()
    {
        for (int i = 0; i < grid.GetLength(0); i++)
        {
            var sb = new StringBuilder();
            for (int j = 0; j < grid.GetLength(1); j++)
            {
                sb.Append(j + "," + i + " ");
            }
            Debug.Log(sb.ToString());
        }
    }
}
