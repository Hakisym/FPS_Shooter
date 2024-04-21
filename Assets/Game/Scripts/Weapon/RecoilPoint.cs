using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RecoilPoint : MonoBehaviour
{
    public CameraRecoil cameraRecoil;

    private void Update()
    {
        transform.localRotation = cameraRecoil.transform.localRotation;
    }
}
