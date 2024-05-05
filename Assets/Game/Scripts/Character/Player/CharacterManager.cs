using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterManager : MonoBehaviour
{
    public Actor actor { get; set; }
    public WeaponController weaponController { get; set; }
    public PlayerMovement playerMovement { get; set; }
    public IDamageable damageable { get; set; }
    public Inventory inventory { get; set; }

    private void Awake()
    {
        actor = GetComponent<Actor>();
        weaponController = GetComponent<WeaponController>();
        playerMovement = GetComponent<PlayerMovement>();
        damageable = GetComponent<IDamageable>();
        inventory = GetComponentInChildren<Inventory>();
    }
}
