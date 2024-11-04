using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CrossHair : MonoBehaviour
{
    [Tooltip(" How much space it takes from your screen"), SerializeField]
    private float size = 10f;

    [Tooltip(" Thickness of the crosshair  "), SerializeField]
    private float width = 2f;

    [SerializeField] private float spreadRatio = 1f;
    [HideInInspector] public bool enableCrosshair;
    public bool showAimCrosshair;

    public Parts parts;

    [System.Serializable]
    public class Parts
    {
        public bool topPart, downPart, leftPart, rightPart, center;
    }

    private bool canShow = true;
    private float spread;
    public void DisplayCrosshair(float _spread, bool show)
    {
        if (show)
        {
            spread = _spread;
            if (!canShow) canShow = true;
        }
        else
        {
            if (canShow) canShow = false;
        }
    }

    private void OnGUI()
    {
        if ((!canShow && !showAimCrosshair) || !enableCrosshair) return;

        Texture2D texture = new Texture2D(1, 1);
        texture.SetPixel(0, 0, Color.white);
        texture.wrapMode = TextureWrapMode.Repeat;
        texture.Apply();

        float scaledSpread = spread * Screen.width / 2560 * spreadRatio;

        if (parts.downPart) GUI.DrawTexture(ResizeGUI(new Rect(Screen.width / 2 - width / 2, Screen.height / 2 + scaledSpread, width, size)), texture);

        if (parts.topPart) GUI.DrawTexture(ResizeGUI(new Rect(Screen.width / 2 - width / 2, Screen.height / 2 - scaledSpread, width, -size)), texture);

        if (parts.rightPart) GUI.DrawTexture(ResizeGUI(new Rect(Screen.width / 2 + scaledSpread, Screen.height / 2 - width / 2, size, width)), texture);

        if (parts.leftPart) GUI.DrawTexture(ResizeGUI(new Rect(Screen.width / 2 - scaledSpread, Screen.height / 2 - width / 2, -size, width)), texture);

        if (parts.center)
        {
            float radius = Mathf.Min(width, size) / 2;
            Vector2 center = new Vector2(Screen.width / 2, Screen.height / 2);
            Rect circleRect = new Rect(center.x - radius, center.y - radius, radius * 2, radius * 2);

            GUI.DrawTexture(circleRect, texture);
        }
    }

    Rect ResizeGUI(Rect _rect)
    {
        float FilScreenWidth = _rect.width / 2560;
        float rectWidth = FilScreenWidth * Screen.width;
        float FilScreenHeight = _rect.height / 1440;
        float rectHeight = FilScreenHeight * Screen.height;
        float rectX = (_rect.x / 2560) * Screen.width;
        float rectY = (_rect.y / 1440) * Screen.height;

        return new Rect(_rect.x, _rect.y, rectWidth, rectHeight);
    }
}
