using NUnit.Framework;
using UnityEngine;
using System.Collections.Generic;
using System;
using System.Collections;

public class CustomerMovement : MonoBehaviour
{
    public GameObject[] animationPrefabs;

    #region AI and state information
    private class CustomerDesire
    {
        public CustomerDesireable desire;
        public bool satisfied;

        public CustomerDesire(CustomerDesireable desire)
        {
            this.desire = desire;
            satisfied = false;
        }
    }

    private List<CustomerDesire> desires;
    #endregion

    #region Movement Information
    private GridMap MovementGrid;
    public int MovementSpeed = 2; // in cells/second

    private float movementTimer;
    private float timeBetweenMoves;
    private Queue<Vector3> pathToCurrentTarget;
    #endregion

    private Coroutine currentAction;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        // initialize customer desires
        ArcadeMachineBehavior[] machines = FindObjectsByType<ArcadeMachineBehavior>(FindObjectsSortMode.None); // none for now, we'll sort by interest or something later
        desires = new List<CustomerDesire>();
        for(int i = 0; i < machines.Length; i++)
        {
            desires.Add(new CustomerDesire(machines[i]));
        }

        // get map information
        MovementGrid = FindFirstObjectByType<GridMap>();

        // initialize movement info
        timeBetweenMoves = 1.0f / MovementSpeed;
        movementTimer = timeBetweenMoves;
    }

    // Update is called once per frame
    void Update()
    {
        // find target destination (this is super unoptimized but we'll fix that later)
        CustomerDesire currentDesire = desires.Find((x) => { return !x.satisfied; });
        Vector3 target = GameObject.FindWithTag("Entrance").transform.position;
        if (currentDesire != null)
        {
            target = currentDesire.desire.GetInteractionPosition();
        }
        
        // snap target location to grid
        target = MovementGrid.SnappedToGrid(target);

        // move to destination
        if(pathToCurrentTarget == null) // first get a path if we don't have one yet
        {
            pathToCurrentTarget = MovementGrid.FindPathOnGrid(transform.position, target);
        }
        else if(pathToCurrentTarget.Count > 0) // if we haven't emptied our path
        {
            if(currentAction == null) // next, dequeue the first point in the path and start moving there.
            {
                Vector3 currentTargetPosition = pathToCurrentTarget.Dequeue();
                currentAction = StartCoroutine(MoveTowardsTarget(currentTargetPosition)); // we set this so it's called only once for this target
            }
        }
        else if(currentAction == null) // here we'll handle selecting our next action. Or something. But for now it's gonna be just unsetting pathToCurrentTarget.
        {
            pathToCurrentTarget = null;
        }
    }

    IEnumerator MoveTowardsTarget(Vector3 target)
    {
        while(Vector3.Distance(transform.position, target) > 0.1f)
        {
            Vector3 directionVector = (target - transform.position).normalized;
            transform.position = transform.position + directionVector * MovementSpeed * Time.deltaTime;
            yield return null;
        }
        transform.position = target;
        
        currentAction = null; // clear this so we can move to the next path
    }
}
