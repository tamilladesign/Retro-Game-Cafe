using UnityEngine;

public class ArcadeMachineBehavior : CustomerDesireable
{
    private const float tempTimeToAutoFix = 3f;
    private float tempTimeToFix;

    public override Vector3 GetInteractionPosition()
    {
        Vector3 interactionPosition = transform.position;
        interactionPosition.y -= 2;
        return interactionPosition;
    }

    private void Update()
    {
        switch(itemState)
        {
            case ItemState.FIXED:
                if (IsOccupied == false && timeToBreakDown <= 0)
                {
                    itemState = ItemState.BROKEN;
                    tempTimeToFix = tempTimeToAutoFix;
                    // change sprite to be broken
                    //TEMP CODE
                    GetComponent<SpriteRenderer>().color = Color.red;

                    // play sounds breaking down and stuff?
                }
                break;
            case ItemState.BROKEN:
                
                if(tempTimeToFix <= 0.0f)
                {
                    itemState = ItemState.FIXED;
                    GetComponent<SpriteRenderer>().color = Color.white;
                    timeToBreakDown = BreakDownTime;
                }
                else
                {
                    tempTimeToFix -= Time.deltaTime;
                }

                break;
            case ItemState.BEING_REPAIRED:
                break;
        }
    }

    public override float Interact(MonoBehaviour i)
    {
        // play animation, update stats and whatnot
        IsOccupied = true;

        return InteractionTime;
    }

    public override bool CanInteractNow()
    {
        return !IsOccupied && itemState == ItemState.FIXED;
    }

    public override void DoneInteracting()
    {
        IsOccupied = false;

        timeToBreakDown = Mathf.Clamp(timeToBreakDown - 1, 0, BreakDownTime);
    }
}
