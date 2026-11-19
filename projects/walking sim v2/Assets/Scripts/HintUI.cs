using UnityEngine;

// one place for all the on-screen hints so they all look the same.
// a dark rounded box near the bottom with the key in its own little key cap, like [E] pick up the axe
// call these from OnGUI
public static class HintUI
{
    static GUIStyle textStyle;
    static GUIStyle keyStyle;
    static GUIStyle panelStyle;
    static GUIStyle smallStyle;
    static Texture2D dot;
    static int builtForHeight;

    static readonly Color textColour = new Color(0.93f, 0.89f, 0.8f);
    static readonly Color keyTextColour = new Color(0.12f, 0.1f, 0.08f);

    // a key prompt in the middle bottom of the screen
    public static void Prompt(string key, string action)
    {
        Setup();
        var keyContent = new GUIContent(key);
        var textContent = new GUIContent(action);
        Vector2 keySize = keyStyle.CalcSize(keyContent);
        keySize.x = Mathf.Max(keySize.x, keySize.y);   // at least square
        Vector2 textSize = textStyle.CalcSize(textContent);

        float pad = Screen.height * 0.012f;
        float gap = Screen.height * 0.012f;
        float width = pad + keySize.x + gap + textSize.x + pad * 1.5f;
        float height = Mathf.Max(keySize.y, textSize.y) + pad * 1.2f;
        var box = new Rect((Screen.width - width) / 2f, Screen.height * 0.74f, width, height);

        GUI.Box(box, GUIContent.none, panelStyle);
        float middle = box.y + box.height / 2f;
        GUI.Box(new Rect(box.x + pad, middle - keySize.y / 2f, keySize.x, keySize.y), keyContent, keyStyle);
        GUI.Label(new Rect(box.x + pad + keySize.x + gap, middle - textSize.y / 2f, textSize.x, textSize.y), textContent, textStyle);
    }

    // small line of text in the bottom left corner, stacked by row (0 = lowest)
    public static void Corner(string text, int row)
    {
        Setup();
        float lineHeight = smallStyle.fontSize * 1.6f;
        var rect = new Rect(Screen.height * 0.03f, Screen.height - Screen.height * 0.03f - lineHeight * (row + 1), Screen.width * 0.5f, lineHeight);
        GUI.Label(rect, text, smallStyle);
    }

    // little dot in the middle so you know what you're looking at
    public static void Crosshair()
    {
        Setup();
        float size = Mathf.Max(3f, Screen.height * 0.004f);
        GUI.DrawTexture(new Rect((Screen.width - size) / 2f, (Screen.height - size) / 2f, size, size), dot);
    }

    static void Setup()
    {
        // remake the styles if the window size changed, so everything scales with the screen
        if (textStyle != null && builtForHeight == Screen.height) return;
        builtForHeight = Screen.height;
        int size = Mathf.Max(14, Screen.height / 42);

        textStyle = new GUIStyle(GUI.skin.label) { fontSize = size, alignment = TextAnchor.MiddleLeft };
        textStyle.normal.textColor = textColour;

        keyStyle = new GUIStyle(GUI.skin.box) { fontSize = Mathf.RoundToInt(size * 0.85f), fontStyle = FontStyle.Bold, alignment = TextAnchor.MiddleCenter };
        keyStyle.normal.background = Rounded(new Color(0.9f, 0.85f, 0.74f, 0.95f));
        keyStyle.normal.textColor = keyTextColour;
        keyStyle.border = new RectOffset(6, 6, 6, 6);
        keyStyle.padding = new RectOffset(size / 2, size / 2, size / 5, size / 5);

        panelStyle = new GUIStyle(GUI.skin.box);
        panelStyle.normal.background = Rounded(new Color(0.05f, 0.05f, 0.06f, 0.6f));
        panelStyle.border = new RectOffset(8, 8, 8, 8);

        smallStyle = new GUIStyle(GUI.skin.label) { fontSize = Mathf.Max(12, Mathf.RoundToInt(size * 0.7f)) };
        smallStyle.normal.textColor = new Color(textColour.r, textColour.g, textColour.b, 0.75f);

        dot = new Texture2D(1, 1);
        dot.SetPixel(0, 0, new Color(1f, 1f, 1f, 0.55f));
        dot.Apply();
    }

    // a small square with rounded corners, stretched out using its border
    static Texture2D Rounded(Color colour)
    {
        const int n = 16;
        const float radius = 6f;
        var tex = new Texture2D(n, n) { wrapMode = TextureWrapMode.Clamp };
        for (int y = 0; y < n; y++)
        for (int x = 0; x < n; x++)
        {
            // how far outside the rounded corner this pixel is
            float dx = Mathf.Max(0f, Mathf.Max(radius - x - 0.5f, x + 0.5f - (n - radius)));
            float dy = Mathf.Max(0f, Mathf.Max(radius - y - 0.5f, y + 0.5f - (n - radius)));
            float alpha = Mathf.Clamp01(radius - Mathf.Sqrt(dx * dx + dy * dy) + 0.5f);
            tex.SetPixel(x, y, new Color(colour.r, colour.g, colour.b, colour.a * alpha));
        }
        tex.Apply();
        return tex;
    }
}
