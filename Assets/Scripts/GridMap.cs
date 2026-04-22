using System.Collections.Generic;
using UnityEngine;
using System;
using System.Collections;
using static UnityEngine.GraphicsBuffer;

public class GridMap : MonoBehaviour
{
    private Grid grid;
    private List<Vector2Int> obstacles;

    private void Awake()
    {
        grid = GetComponent<Grid>();
        obstacles = new List<Vector2Int>();
    }
    public Vector3 SnappedToGrid(Vector3 position)
    {
        Vector3Int gridPosition = grid.WorldToCell(position);
        Vector3 snappedPosition = grid.CellToWorld(gridPosition);
        snappedPosition.x += grid.cellSize.x / 2.0f;
        snappedPosition.y += grid.cellSize.y / 2.0f;

        return snappedPosition;
    }

    public void UpdateNavInfo()
    {
        CustomerDesireable[] allInteractibles = FindObjectsByType<CustomerDesireable>(FindObjectsSortMode.None);
        for(int i = 0;  i < allInteractibles.Length; i++)
        {
            // find bounds of the object
            Bounds objectBounds = allInteractibles[i].GetComponent<SpriteRenderer>().bounds;
            for(float y = objectBounds.min.y; y < objectBounds.max.y; y += grid.cellSize.y)
            {
                for(float x = objectBounds.min.x; x < objectBounds.max.x; x += grid.cellSize.x)
                {
                    Vector3Int currentPoint = grid.WorldToCell(new Vector3(x, y, 0));
                    // add every point within the bounds of the object
                    obstacles.Add(new Vector2Int(currentPoint.x, currentPoint.y));
                }
            }

            // remove the interaction point for the object
            Vector2Int interactionPoint = (Vector2Int) grid.WorldToCell(allInteractibles[i].GetInteractionPosition());
            Vector2Int match = obstacles.Find((a) => { return a.x == interactionPoint.x && a.y == interactionPoint.y; });
            obstacles.Remove(match);
        }
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
