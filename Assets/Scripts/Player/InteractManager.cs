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
        //cast ray
        RaycastHit raycastHit;
        if(Physics.Raycast(playerCam.transform.position,playerCam.transform.forward,out raycastHit,detectDistance, whatIsInteractable))
        {
            //show object UI
            var weaponPickable = raycastHit.transform.GetComponent<WeaponPickable>();
            UIManager.instance.InteractUI.gameObject.SetActive(true);
            UIManager.instance.interactionName.text = weaponPickable.weaponData.gunName;

            //interact
            if (Input.GetKeyDown(KeyCode.E))
            {
                weaponPickable.Interact();
            }
        }
        else
        {
            //hide object UI
            UIManager.instance.InteractUI.gameObject.SetActive(false);
        }
    }
}
