using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IControllable
{
    public Action<bool> OnControllableChanged { get; set; }

    public void HandleControl(bool controllable);
}
