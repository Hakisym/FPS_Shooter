using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    [SerializeField] DeathCamera deathCamera;

    public DeathCamera DeathCamera { get; private set; }

    private void Awake()
    {
        if (deathCamera) 
        DeathCamera = Instantiate(deathCamera);
    }
}
