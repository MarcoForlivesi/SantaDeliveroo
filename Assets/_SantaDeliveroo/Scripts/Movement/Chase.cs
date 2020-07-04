
using UnityEngine;

public class Chase : BaseMovement
{
    private Transform target;

    private void FixedUpdate()
    {
        if (target == null)
        {
            return;
        }

        MoveTowards(target.position);
    }

    private void TargetReached()
    {

    }

    public void SetTarget(Transform target)
    {
        this.target = target;
    }
}
