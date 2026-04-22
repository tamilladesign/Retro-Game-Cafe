using UnityEngine;

public abstract class CustomerDesireable : MonoBehaviour
{
    [HideInInspector]public bool IsOccupied = false;

    public abstract Vector3 GetInteractionPosition();
    public abstract void Interact(MonoBehaviour interactor);
}
