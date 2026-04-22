using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

public class GridMap : MonoBehaviour
{
    private Grid grid;
    private void Awake()
    {
        grid = GetComponent<Grid>();
    }
    public Vector3 SnappedToGrid(Vector3 position)
    {
        Vector3Int gridPosition = grid.WorldToCell(position);
        Vector3 snappedPosition = grid.CellToWorld(gridPosition);
        snappedPosition.x += grid.cellSize.x / 2.0f;
        snappedPosition.y += grid.cellSize.y / 2.0f;

        return snappedPosition;
    }

    public Queue<Vector3> FindPathOnGrid(Vector3 startPos,  Vector3 endPos)
    {
        Queue<Vector3> path = new Queue<Vector3>();
        
        // make sure both positions are snapped to the grid
        startPos = SnappedToGrid(startPos);
        endPos = SnappedToGrid(endPos);

        // this is where our proper pathfinding will go, but for now let's keep it simple.
        path.Enqueue(startPos);
        path.Enqueue(new Vector3(endPos.x, startPos.y)); // this is basically the turn point.
        path.Enqueue(endPos);

        return path;
    }
}
