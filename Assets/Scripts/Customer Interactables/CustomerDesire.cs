using UnityEngine;

public abstract class CustomerDesireable : MonoBehaviour
{
    public enum ItemState
    {
        FIXED,
        BROKEN,
        BEING_REPAIRED
    }

    protected bool IsOccupied;
    protected ItemState itemState;

    public float InteractionTime; // time it takes to interact with this machine
    public int BreakDownTime; // number of times it can be interacted with before it breaks down.

    protected int timeToBreakDown; // in-game breakdown timer

    private void Start()
    {
        IsOccupied = false;
        itemState = ItemState.FIXED;
        timeToBreakDown = BreakDownTime;
    }

    public abstract Vector3 GetInteractionPosition();
    public abstract float Interact(MonoBehaviour interactor); // Runs on successful interaction, returns interaction time
    public abstract void DoneInteracting();
    public abstract bool CanInteractNow();
}
