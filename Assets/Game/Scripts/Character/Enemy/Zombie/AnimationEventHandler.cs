using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class AnimationEventHandler : MonoBehaviour
{
    public UnityEvent OnAnimFinish;
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
