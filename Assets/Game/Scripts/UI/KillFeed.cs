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

    public void Show()
    {
        animator.Play("Show", 0, 0f);
    }
}
