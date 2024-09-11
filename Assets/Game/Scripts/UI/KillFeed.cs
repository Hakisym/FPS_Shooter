using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class KillFeed : MonoBehaviour
{
    public TextMeshProUGUI counter;
    public TextMeshProUGUI tag;
    private Animator animator;

    private void Awake()
    {
        animator = GetComponent<Animator>();
    }

    public void Show(Actor killer, Actor victim)
    {
        if (!UIManager.instance.KillFeed) return;

        int kills = killer.actorManager.kills;
        counter.text = "TOTAL KILLS " + kills;
        tag.text = "YOU KILLED " + victim.actorName;
        animator.Play("Show", 0, 0f);
    }
}
