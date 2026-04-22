using UnityEngine;

public class ArcadeMachineBehavior : CustomerDesireable
{
    public override Vector3 GetInteractionPosition()
    {
        Vector3 interactionPosition = transform.position;
        interactionPosition.y -= 2;
        return interactionPosition;
    }

    public override void Interact(MonoBehaviour i)
    {
        // play animation, update stats and whatnot
    }
}
