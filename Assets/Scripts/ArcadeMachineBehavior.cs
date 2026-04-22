using UnityEngine;

public class ArcadeMachineBehavior : CustomerDesireable
{
    public override Vector3 GetInteractionPosition()
    {
        Vector3 interactionPosition = transform.position;
        interactionPosition.y -= 2;
        return interactionPosition;
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Play() // plays animation of arcade machine being played.
    {

    }
}
