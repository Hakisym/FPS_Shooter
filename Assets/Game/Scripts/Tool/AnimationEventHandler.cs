using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimationEventHandler : MonoBehaviour
{
    public Action OnAnimFinish;
    private ZombieStateController stateController;

    private void Start()
    {
        stateController = GetComponent<ZombieStateController>();
    }

    private void AnimationFinishTrigger()
    {
        OnAnimFinish?.Invoke();
    }
}
