using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Planner;

public class MoveAnimation : MonoBehaviour
{
    private const string WALK_UP_TRIGGER = "Walk Up";
    private const string WALK_DOWN_TRIGGER = "Walk Down";
    private const string WALK_RIGHT_TRIGGER = "Walk Right";
    private const string WALK_LEFT_TRIGGER = "Walk Left";
    private const string IDLE_UP_TRIGGER = "Idle Up";
    private const string IDLE_DOWN_TRIGGER = "Idle Down";
    private const string IDLE_RIGHT_TRIGGER = "Idle Right";
    private const string IDLE_LEFT_TRIGGER = "Idle Left";

    private Animator animator;

    // Start is called before the first frame update
    void Start()
    {
        animator = GetComponent<Animator>();
    }

    internal void walk(Direction direction)
    {
        setWalk(direction);
        resetAllIdle();
    }

    internal void idle(Direction direction)
    {
        setIdle(direction);
        resetAllWalk();
    }

    private void setIdle(Direction direction)
    {
        switch (direction)
        {
            case Direction.LEFT:
                animator.SetTrigger(IDLE_LEFT_TRIGGER);
                break;
            case Direction.RIGHT:
                animator.SetTrigger(IDLE_RIGHT_TRIGGER);
                break;
            case Direction.UP:
                animator.SetTrigger(IDLE_UP_TRIGGER);
                break;
            case Direction.DOWN:
                animator.SetTrigger(IDLE_DOWN_TRIGGER);
                break;
        }
    }

    private void resetAllIdle()
    {
        animator.ResetTrigger(IDLE_LEFT_TRIGGER);
        animator.ResetTrigger(IDLE_RIGHT_TRIGGER);
        animator.ResetTrigger(IDLE_UP_TRIGGER);
        animator.ResetTrigger(IDLE_DOWN_TRIGGER);
    }

    private void setWalk(Direction direction)
    {
        switch (direction)
        {
            case Direction.LEFT:
                animator.SetTrigger(WALK_LEFT_TRIGGER);
                break;
            case Direction.RIGHT:
                animator.SetTrigger(WALK_RIGHT_TRIGGER);
                break;
            case Direction.UP:
                animator.SetTrigger(WALK_UP_TRIGGER);
                break;
            case Direction.DOWN:
                animator.SetTrigger(WALK_DOWN_TRIGGER);
                break;
        }
    }

    private void resetAllWalk()
    {
        animator.ResetTrigger(WALK_LEFT_TRIGGER);
        animator.ResetTrigger(WALK_RIGHT_TRIGGER);
        animator.ResetTrigger(WALK_UP_TRIGGER);
        animator.ResetTrigger(WALK_DOWN_TRIGGER);
    }
}
