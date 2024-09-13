using FishNet.Object;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NetComponentEnabler : MonoBehaviour
{
    [SerializeField] private NetworkObject networkObject;
    [SerializeField] private List<MonoBehaviour> disableComponents;
    [SerializeField] private List<GameObject> inactiveGameObjects;

    private void Start()
    {
        if (!networkObject.Owner.IsLocalClient)
        {
            ComponentsDisabler();
        }
    }

    private void ComponentsDisabler()
    {
        foreach (var item in disableComponents)
        {
            item.enabled = false;
        }
        foreach (var item in inactiveGameObjects)
        {
            item.SetActive(false);
        }
    }
}
