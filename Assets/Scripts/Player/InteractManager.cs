using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InteractManager : MonoBehaviour
{
    public Camera playerCam;
    public float detectDistance;
    public LayerMask whatIsInteractable;

    private void Update()
    {
        RaycastHit raycastHit;
        if(Physics.Raycast(playerCam.transform.position,playerCam.transform.forward,out raycastHit,detectDistance, whatIsInteractable))
        {
            var weaponPickable = raycastHit.transform.GetComponent<WeaponPickable>();
            UIManager.instance.InteractUI.gameObject.SetActive(true);
            UIManager.instance.interactionName.text = weaponPickable.weaponName;
            if (Input.GetKeyDown(KeyCode.E))
            {
                weaponPickable.AddWeapon();
            }
        }
        else
        {
            UIManager.instance.InteractUI.gameObject.SetActive(false);
        }
    }
}
