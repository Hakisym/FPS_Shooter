using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CenterHUD : MonoBehaviour
{
    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Mouse4))
        {
            gameObject.SetActive(!gameObject.activeSelf);
        }
    }
}
