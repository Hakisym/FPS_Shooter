using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CrossHair : MonoBehaviour
{
    private WeaponController weaponController;

    [Tooltip(" How much space it takes from your screen"), SerializeField]
    private float size = 10f;

    [Tooltip(" Thickness of the crosshair  "), SerializeField]
    private float width = 2f;

    [HideInInspector] public bool enableCrosshair;
    public bool showAimCrosshair;

    public Parts parts;

    [System.Serializable]
    public class Parts
    {
        public bool topPart, downPart, leftPart, rightPart, center;
    }

    private bool canShow;

    public void GetReference(WeaponController wc)
    {
        this.weaponController = wc;
        wc.OnAimFinish += () => { canShow = false; };
        wc.OnAimCancel += () => { canShow = true; };
    }

    private void OnGUI()
    {
        if ((!canShow && !showAimCrosshair) || !enableCrosshair) return;

        Texture2D texture = new Texture2D(1, 1);
        texture.SetPixel(0, 0, Color.white);
        texture.wrapMode = TextureWrapMode.Repeat;
        texture.Apply();

        if (parts.downPart) GUI.DrawTexture(new Rect(Screen.width / 2 - width / 2, (Screen.height / 2 - size / 2) + weaponController.currentSpread / 2, width, size), texture);

        if (parts.topPart) GUI.DrawTexture(new Rect(Screen.width / 2 - width / 2, (Screen.height / 2 - size / 2) - weaponController.currentSpread / 2, width, size), texture);

        if (parts.rightPart) GUI.DrawTexture(new Rect((Screen.width / 2 - size / 2) + weaponController.currentSpread / 2, Screen.height / 2 - width / 2, size, width), texture);

        if (parts.leftPart) GUI.DrawTexture(new Rect((Screen.width / 2 - size / 2) - weaponController.currentSpread / 2, Screen.height / 2 - width / 2, size, width), texture);

        if (parts.center)
        {
            float radius = Mathf.Min(width, size) / 2;
            Vector2 center = new Vector2(Screen.width / 2, Screen.height / 2);
            Rect circleRect = new Rect(center.x - radius, center.y - radius, radius * 2, radius * 2);

            GUI.DrawTexture(circleRect, texture);
        }
    }
}
