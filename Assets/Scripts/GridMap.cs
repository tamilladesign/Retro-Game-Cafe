using System.Collections.Generic;
using UnityEngine;
using System;
using System.Collections;
using static UnityEngine.GraphicsBuffer;
using System.Linq;

public class GridMap : MonoBehaviour
{
    public bool DrawGizmos;

    private Grid grid;
    private List<Vector3Int> obstacles;

    private void Awake()
    {
        grid = GetComponent<Grid>();
        obstacles = new List<Vector3Int>();
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
        obstacles.Clear();
        for(int i = 0;  i < allInteractibles.Length; i++)
        {
            // find bounds of the object
            Bounds objectBounds;
            SpriteRenderer spriteRenderer;
            allInteractibles[i].TryGetComponent<SpriteRenderer>(out spriteRenderer);
            if(spriteRenderer == null) continue;

            objectBounds = spriteRenderer.bounds;
            for(float y = objectBounds.min.y; y < objectBounds.max.y; y += grid.cellSize.y)
            {
                for(float x = objectBounds.min.x; x < objectBounds.max.x; x += grid.cellSize.x)
                {
                    Vector3Int currentPoint = grid.WorldToCell(new Vector3(x, y, 0));
                    // add every point within the bounds of the object
                    obstacles.Add(new Vector3Int(currentPoint.x, currentPoint.y, 0));
                }
            }

            // remove the interaction point for the object
            Vector3Int interactionPoint = grid.WorldToCell(allInteractibles[i].GetInteractionPosition());
            Vector3Int match = obstacles.Find((a) => { return a.x == interactionPoint.x && a.y == interactionPoint.y; });
            obstacles.Remove(match);
        }
    }

    public Queue<Vector3> FindPathOnGrid(Vector3 startPos,  Vector3 endPos)
    {
        Queue<Vector3> path = new Queue<Vector3>();
        
        // make sure both positions are snapped to the grid
        //startPos = SnappedToGrid(startPos);
        //endPos = SnappedToGrid(endPos);

        // this is where our proper pathfinding will go, but for now let's keep it simple.
        //path.Enqueue(startPos);
        //path.Enqueue(new Vector3(endPos.x, startPos.y)); // this is basically the turn point.
        //path.Enqueue(endPos);

        //return path;

        return A_Star(startPos, endPos);
    }

    private Queue<Vector3> A_Star(Vector3 startPos, Vector3 endPos)
    {
        List<Vector3Int> openSet = new List<Vector3Int>();
        Vector3Int gridStartPos = grid.WorldToCell(startPos);
        Vector3Int gridEndPos = grid.WorldToCell(endPos);

        openSet.Add(gridStartPos);

        Dictionary<Vector3Int, float> gscore = new Dictionary<Vector3Int, float>();
        gscore.Add(gridStartPos, 0);

        Dictionary<Vector3Int, float> fscore = new Dictionary<Vector3Int, float>();
        fscore.Add(gridStartPos, distance(gridStartPos, gridEndPos));

        Dictionary<Vector3Int,Vector3Int> came_from = new Dictionary<Vector3Int, Vector3Int>();

        while (openSet.Count > 0)
        {
            Vector3Int current = FindMinF(fscore, openSet);
            if (current == gridEndPos)
            {
                // return reconstruct path
                return ReconstructPath(came_from, current);
            }

            openSet.Remove(current);
            Vector3Int[] neighbors = { 
                new Vector3Int(current.x - 1, current.y), 
                new Vector3Int(current.x + 1, current.y), 
                new Vector3Int(current.x, current.y + 1), 
                new Vector3Int(current.x, current.y - 1) };

            foreach (Vector3Int neighbor in neighbors)
            {
                float tentative_gscore = gscore.GetValueOrDefault(current, 9999) + FindWeight(neighbor);
                
                if(tentative_gscore < gscore.GetValueOrDefault(neighbor, 9999))
                {
                    // add this as a good path between current and neighbor
                    if(came_from.ContainsKey(neighbor))
                        came_from[neighbor] = current;
                    else
                        came_from.Add(neighbor, current);

                    // record this gscore for neighbor
                    if(gscore.ContainsKey(neighbor))
                        gscore[neighbor] = tentative_gscore;
                    else
                        gscore.Add(neighbor,tentative_gscore);

                    // record fscore for neighbor
                    float tentative_fscore = tentative_gscore + distance(neighbor, gridEndPos);
                    if (fscore.ContainsKey(neighbor))
                        fscore[neighbor] = tentative_fscore;
                    else
                        fscore.Add(neighbor, tentative_fscore);

                    if(!openSet.Contains(neighbor))
                        openSet.Add(neighbor);
                }
            }
        }

        // failure. Return empty path.
        return new Queue<Vector3>();
    }

    Queue<Vector3> ReconstructPath(Dictionary<Vector3Int, Vector3Int> came_from, Vector3Int current)
    {
        Stack<Vector3> path = new Stack<Vector3>();
        path.Push(current);
        while(came_from.ContainsKey(current))
        {
            current = came_from[current];
            Vector3 position = grid.CellToWorld(current);
            path.Push(current);
        }

        // this path is actually backwards. We need to flip it.
        Queue<Vector3> finalPath = new Queue<Vector3>();
        while(path.Count > 0)
        {
            finalPath.Enqueue(path.Pop());
        }

        return finalPath;
    }

    float FindWeight(Vector3Int gridPosition)
    {
        if (obstacles.Contains(gridPosition))
            return 99999;
        else
            return 1;
    }
    private Vector3Int FindMinF(Dictionary<Vector3Int, float> f_list, List<Vector3Int> openSet)
    {
        Vector3Int minLoc = new Vector3Int();
        float min = 99999;
        foreach(var pair in f_list)
        {
            if(openSet.Contains(pair.Key))
                if(pair.Value < min)
                {
                    min = pair.Value;
                    minLoc = pair.Key;
                }
        }

        Vector3Int mininumValue = openSet.Find((a) => { return a == minLoc; });
        return mininumValue;
    }

    private float distance(Vector3Int gridStartPos, Vector3Int gridEndPos)
    {
        return Mathf.Abs(gridStartPos.x- gridEndPos.x) + Mathf.Abs(gridStartPos.y - gridEndPos.y);
    }

    private void OnDrawGizmos()
    {
        if (DrawGizmos)
        {
            grid = GetComponent<Grid>();
            UpdateNavInfo();
            for (int i = 0; i < obstacles.Count; i++)
            {
                Vector3 position = grid.CellToWorld(obstacles[i]);
                position.x += grid.cellSize.x / 2.0f;
                position.y += grid.cellSize.y / 2.0f;

                Gizmos.color = Color.yellow;
                Gizmos.DrawSphere(position, 0.25f);
            }
        }
    }
}
