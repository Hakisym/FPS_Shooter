using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CamShake : MonoBehaviour
{
    public WeaponController wc;
    [Space(10)]
    public float effectSpeed = 100f;
    public float returnSpeed = 10f;
    private float effectIntensity;

    private float sinTime;

    private static CamShake _instance;
    private float trauma;

    public static CamShake instance { get { return _instance; } }

    private void Awake() => _instance = this;

    private void Update()
    {
        trauma = Mathf.Clamp01(trauma);

        trauma = Mathf.Lerp(trauma, 0, Time.deltaTime * returnSpeed);

        sinTime += Time.deltaTime * effectSpeed * trauma;

        float sinAmount = Mathf.Sin(sinTime) * effectIntensity;

        Vector3 finalRotation = Vector3.forward * sinAmount;

        transform.localRotation = Quaternion.Euler(finalRotation);
    }

    public void ShootShake(float intensity)
    {
        trauma += 1;
        effectIntensity = intensity;
    }
}
