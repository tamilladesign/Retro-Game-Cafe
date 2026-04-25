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

    private List<CustomerDesire> desires; // actually, maybe this could be a priority queue or something. We'll figure that out at some point.
    private CustomerDesire currentDesire;
    private float patience;
    #endregion

    #region Movement Information

    private GridMap MovementGrid;
    public int MovementSpeed = 2; // in cells/second
    public float MaxPatience;

    private Queue<Vector3> pathToCurrentDesire;

    #endregion

    private Coroutine currentAction; // we'll store our currently running coroutine here

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

        patience = MaxPatience;
    }

    // Update is called once per frame
    void Update()
    {
        if(currentDesire == null)
        {
            // find what we want to do (this is super unoptimized but we'll fix that later)
            // this is where we'd look through the list of possible things to do and pick something
            currentDesire = desires.Find((x) => { return !x.satisfied; });
            
            
            if(currentDesire == null) // if nothing satisfactory is found
            {
                currentDesire = new CustomerDesire(FindFirstObjectByType<Exit>()); // get out of here
            }

            //TEMP CODE
            GetComponent<SpriteRenderer>().color = Color.white;
        }

        if(currentDesire == null) // just to catch an empty situation
        {
            return;
        }

        // here we execute moving to the destination
        if (pathToCurrentDesire == null && currentAction == null) // first get a path if we don't have one yet
        {
            Vector3 target = currentDesire.desire.GetInteractionPosition();

            // snap target location to grid
            target = MovementGrid.SnappedToGrid(target);
            // find path to our target
            pathToCurrentDesire = MovementGrid.FindPathOnGrid(transform.position, target);
        }
        else if (pathToCurrentDesire.Count > 0) // we have a path and it's not yet empty
        {
            if (currentAction == null) // dequeue the next point in the path and start moving there.
            {
                Vector3 currentTargetPosition = pathToCurrentDesire.Dequeue();
                currentAction = StartCoroutine(MoveTowardsTarget(currentTargetPosition)); // we set this so it's called only once for this target
            }
        }
        else if (currentAction == null) // we have reached our destination
        {
            pathToCurrentDesire = null;

            // This is where we execute doing the task.
            if(currentDesire.desire.CanInteractNow())
            {
                currentAction = StartCoroutine(Interact(currentDesire.desire));
            }
            else
            {
                patience -= Time.deltaTime;

                //TEMP CODE
                GetComponent<SpriteRenderer>().color = Color.tomato;
            }
        }

        if(patience <= 0)
        {
            desires.Clear(); // we don't want anything anymore
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

    IEnumerator Interact(CustomerDesireable desireable)
    {
        //TEMP CODE
        GetComponent<SpriteRenderer>().color = Color.white;

        desireable.Interact(this); // start interaction

        yield return new WaitForSeconds(desireable.InteractionTime); // wait for the interaction to finish

        float coinsSpent = desireable.DoneInteracting(); // end interaction

        // Update coins for player.
        coinsSpent = coinsSpent * (patience / MaxPatience); // we'll scale the coins this machine gave by how much we've had to wait.
        PlayerStats.AddCoins((int)Mathf.Round(coinsSpent)); // then round that to the nearest in and say that's how much we got.

        // set that we're done with this task
        currentDesire.satisfied = true;
        currentDesire = null;

        currentAction = null;
    }

    private void OnDrawGizmos()
    {
        if(pathToCurrentDesire.Count > 0)
        {
            Vector3[] path = pathToCurrentDesire.ToArray();
            for(int i = 0; i < path.Length - 1; i++)
            {
                Gizmos.color = Color.red;
                Gizmos.DrawLine(path[i], path[i+1]);
            }    
        }
    }
}
