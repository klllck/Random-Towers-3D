public class Cell
{
    private int x, z;
    private bool isTaken;
    private CellType cellType;

    public int X { get => x; }
    public int Z { get => z; }
    public bool IsTaken { get => isTaken; set => isTaken = value; }
    public CellType CellType { get => cellType; set => cellType = value; }

    public Cell(int x, int z)
    {
        this.x = x;
        this.z = z;
        cellType = CellType.Empty;
        isTaken = false;
    }
}
