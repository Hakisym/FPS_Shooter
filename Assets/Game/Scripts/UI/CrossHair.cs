using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CrossHair : MonoBehaviour
{
    [Tooltip("Attach your PlayerMovement player "), SerializeField]
    private PlayerMovement player;

    private WeaponController weaponController;

    [Tooltip(" How much space it takes from your screen"), SerializeField]
    private float size = 10f;

    [Tooltip(" Thickness of the crosshair  "), SerializeField]
    private float width = 2f;


    [Tooltip(" Original spread you want to start with "), SerializeField]
    public float defaultSpread = 10f;

    [SerializeField] private float walkSpread, runSpread, crouchSpread, jumpSpread;

    [SerializeField] private float resizeSpeed = 3f;
    [HideInInspector]
    public float spread;
    private float locomotionSpread;
    private float bulletSpread;

    public Parts parts;

    [System.Serializable]
    public class Parts
    {
        public bool topPart, downPart, leftPart, rightPart, center;
    }

    private void Awake()
    {
        weaponController = player.GetComponent<WeaponController>();
    }

    void Update()
    {
        // Manage different sizes
        if (player.isGrounded)
        {
            if (player.currentSpeed == player.sprintSpeed) locomotionSpread = runSpread;
            else
            {
                if (player.currentSpeed == player.walkSpeed)
                {
                    if (player.moveVel < .2f) locomotionSpread = defaultSpread;
                    else locomotionSpread = walkSpread;
                }

                if (player.currentSpeed == player.crouchSpeed) locomotionSpread = crouchSpread;
            }
        }
        else locomotionSpread = jumpSpread;

        bulletSpread = Mathf.Lerp(bulletSpread, 0f, weaponController.wd.crosshairResizeSpeed * Time.deltaTime);

        spread = Mathf.Lerp(spread, locomotionSpread + bulletSpread, resizeSpeed * Time.deltaTime);
    }

    public void Fire()
    {
        bulletSpread += weaponController.wd.crosshairResize;
    }

    private void OnGUI()
    {
        Texture2D texture = new Texture2D(1, 1);
        texture.SetPixel(0, 0, Color.white);
        texture.wrapMode = TextureWrapMode.Repeat;
        texture.Apply();

        if (parts.downPart) GUI.DrawTexture(new Rect(Screen.width / 2 - width / 2, (Screen.height / 2 - size / 2) + spread / 2, width, size), texture);

        if (parts.topPart) GUI.DrawTexture(new Rect(Screen.width / 2 - width / 2, (Screen.height / 2 - size / 2) - spread / 2, width, size), texture);

        if (parts.rightPart) GUI.DrawTexture(new Rect((Screen.width / 2 - size / 2) + spread / 2, Screen.height / 2 - width / 2, size, width), texture);

        if (parts.leftPart) GUI.DrawTexture(new Rect((Screen.width / 2 - size / 2) - spread / 2, Screen.height / 2 - width / 2, size, width), texture);

        if (parts.center)
        {
            float radius = Mathf.Min(width, size) / 2;
            Vector2 center = new Vector2(Screen.width / 2, Screen.height / 2);
            Rect circleRect = new Rect(center.x - radius, center.y - radius, radius * 2, radius * 2);

            GUI.DrawTexture(circleRect, texture);
        }
    }
}
