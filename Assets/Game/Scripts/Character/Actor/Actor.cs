using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Actor : MonoBehaviour
{
    public string actorName;

    public ActorManager actorManager { get; set; }

    public CharacterManager characterManager { get; set; }

    public Action<Actor> onKill;

    public bool isLocallyMine;

    public ThirdPersonController tpc;

    private void Awake()
    {
        actorManager = GetComponentInParent<ActorManager>();
        characterManager = GetComponent<CharacterManager>();
    }

    private void Start()
    {
        if (isLocallyMine)
        {
            var thirdPersonController = Instantiate(tpc, transform.parent);
            thirdPersonController.Setup(characterManager);
        }

        onKill = ConfirmKill;
        if (actorManager.actor!=this) actorManager.actor = this;
    }

    private void ConfirmKill(Actor victim)
    {
        if (actorManager) actorManager.kills++;

        if (isLocallyMine)
        {
            UIManager.instance.OnEnemyKill?.Invoke(this, victim);
        }
    }

    public void ConfirmDeath()
    {
        if (actorManager) actorManager.deaths++;
    }
}
