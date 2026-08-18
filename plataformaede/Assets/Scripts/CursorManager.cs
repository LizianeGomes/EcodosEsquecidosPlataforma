using UnityEngine;

public class CursorManager : MonoBehaviour
{
    [Header("Cursor")]
    public Texture2D cursorTexture;

    [Header("Hotspot")]
    public Vector2 hotspot = Vector2.zero;

    void Start()
    {
        Cursor.visible = true;

        Cursor.SetCursor(
            cursorTexture,
            hotspot,
            CursorMode.Auto
        );
    }

    void OnDestroy()
    {
        Cursor.SetCursor(null, Vector2.zero, CursorMode.Auto);
    }
}