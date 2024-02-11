using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimationEventHandler : MonoBehaviour
{
    private void AnimationFinishTrigger()
    {
        GetComponent<StateController>().attackFinished = true;
        GetComponent<StateController>().attackEndTime = Time.time;
        GetComponent<Animator>().SetInteger("attack", -1);
    }
}
