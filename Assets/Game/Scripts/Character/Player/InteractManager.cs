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
            var interactGO = raycastHit.transform.GetComponent<Interactable>();
            UIController.instance.InteractUI.gameObject.SetActive(true);
            UIController.instance.interactionName.text = interactGO.name;

            //interact
            if (Input.GetKeyDown(KeyCode.E))
            {
                interactGO.Interact();
            }
        }
        else
        {
            //hide object UI
            UIController.instance.InteractUI.gameObject.SetActive(false);
        }
    }
}
