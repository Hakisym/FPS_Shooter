using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimationEventHandler : MonoBehaviour
{
    private StateController stateController;
    private void Start()
    {
        stateController = GetComponent<StateController>();
    }

    private void AnimationFinishTrigger()
    {
        stateController.attackFinished = true;
        stateController.attackEndTime = Time.time;
        stateController.animator.SetInteger("attack", -1);
    }
}
