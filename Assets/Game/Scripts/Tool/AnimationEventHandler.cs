using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimationEventHandler : MonoBehaviour
{
    public Action OnAnimFinish;
    private StateController stateController;

    private void Start()
    {
        stateController = GetComponent<StateController>();
    }

    private void AnimationFinishTrigger()
    {
        OnAnimFinish?.Invoke();
    }
}
