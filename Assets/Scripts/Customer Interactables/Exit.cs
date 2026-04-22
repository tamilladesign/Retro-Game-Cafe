using UnityEngine;

public class Exit : CustomerDesireable
{
    public override Vector3 GetInteractionPosition()
    {
        return transform.position;
    }

    public override void Interact(MonoBehaviour interactor)
    {
        Destroy(interactor.gameObject);
    }
}
