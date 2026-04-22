using UnityEngine;

public class Exit : CustomerDesireable
{
    public override bool CanInteractNow()
    {
        return true;
    }

    public override void DoneInteracting()
    {
        return;
    }

    public override Vector3 GetInteractionPosition()
    {
        return transform.position;
    }

    public override float Interact(MonoBehaviour interactor)
    {
        Destroy(interactor.gameObject);
        return 0;
    }
}
