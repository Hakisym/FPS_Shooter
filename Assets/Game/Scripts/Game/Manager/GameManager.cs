using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;
    [SerializeField] DeathCamera deathCamera;

    public DeathCamera DeathCamera { get; private set; }

    private void Awake()
    {
        instance = this;
        if (deathCamera) DeathCamera = Instantiate(deathCamera);
    }
}
