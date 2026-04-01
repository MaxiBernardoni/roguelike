using UnityEngine;

/// <summary>
/// Sprites de respaldo para objetos creados en runtime (enemigos, balas). Evita que queden invisibles.
/// </summary>
public static class RuntimeVisuals
{
    static Sprite _circle;

    public static Sprite GetOrCreateCircleSprite()
    {
        if (_circle != null)
            return _circle;

        var tex = new Texture2D(32, 32, TextureFormat.RGBA32, false);
        Vector2 c = new Vector2(16f, 16f);
        for (int y = 0; y < 32; y++)
        for (int x = 0; x < 32; x++)
        {
            float d = Vector2.Distance(new Vector2(x, y), c);
            tex.SetPixel(x, y, d < 14f ? Color.white : Color.clear);
        }

        tex.Apply();
        _circle = Sprite.Create(tex, new Rect(0, 0, 32, 32), new Vector2(0.5f, 0.5f), 32f);
        return _circle;
    }

    public static void EnsureSprite(SpriteRenderer sr)
    {
        if (sr == null)
            return;
        if (sr.sprite == null)
            sr.sprite = GetOrCreateCircleSprite();
        if (sr.color.a < 0.05f)
            sr.color = Color.white;
    }

    public static void EnsureSpritesUnder(GameObject root)
    {
        if (root == null)
            return;
        foreach (var sr in root.GetComponentsInChildren<SpriteRenderer>(true))
            EnsureSprite(sr);
    }
}
