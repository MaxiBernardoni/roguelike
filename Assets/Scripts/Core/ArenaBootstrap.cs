using UnityEngine;
using UnityEngine.EventSystems;

/// <summary>
/// Al inicio: cámara 2D visible (Solid Color, no cielo azul), sprites sin recurso, y EventSystem para UI.
/// </summary>
[DefaultExecutionOrder(-500)]
public class ArenaBootstrap : MonoBehaviour
{
    [SerializeField] bool fixMainCamera = true;
    [SerializeField] bool ensureSpriteFallbacks = true;
    [SerializeField] bool createEventSystemIfMissing = true;

    [SerializeField] Color cameraBackground = new Color(0.08f, 0.08f, 0.11f);
    [SerializeField] float orthographicSize = 10f;

    static Sprite _fallbackSprite;

    void Awake()
    {
        if (fixMainCamera)
            FixMainCamera();
        if (ensureSpriteFallbacks)
            EnsureSpriteRenderersHaveSprite();
        if (createEventSystemIfMissing)
            EnsureEventSystem();
    }

    void FixMainCamera()
    {
        Camera cam = Camera.main;
        if (cam == null)
        {
            cam = FindFirstObjectByType<Camera>();
            if (cam != null && !cam.CompareTag("MainCamera"))
                cam.gameObject.tag = "MainCamera";
        }

        bool createdCam = false;
        if (cam == null)
        {
            var go = new GameObject("Main Camera");
            go.tag = "MainCamera";
            cam = go.AddComponent<Camera>();
            go.AddComponent<AudioListener>();
            createdCam = true;
        }

        cam.orthographic = true;
        cam.orthographicSize = orthographicSize;
        cam.clearFlags = CameraClearFlags.SolidColor;
        cam.backgroundColor = cameraBackground;
        cam.nearClipPlane = 0.1f;
        cam.farClipPlane = 200f;

        Vector3 p = cam.transform.position;
        if (p.z >= -1f)
            cam.transform.position = new Vector3(p.x, p.y, -10f);

        if (!createdCam && cam.GetComponent<AudioListener>() == null &&
            FindFirstObjectByType<AudioListener>() == null)
            cam.gameObject.AddComponent<AudioListener>();
    }

    void EnsureSpriteRenderersHaveSprite()
    {
        var renderers = FindObjectsByType<SpriteRenderer>(FindObjectsSortMode.None);
        foreach (var sr in renderers)
        {
            if (sr.sprite != null)
                continue;
            sr.sprite = GetFallbackSprite();
            if (sr.color.a < 0.05f)
                sr.color = Color.white;
        }
    }

    static Sprite GetFallbackSprite()
    {
        if (_fallbackSprite != null)
            return _fallbackSprite;

        var tex = new Texture2D(32, 32, TextureFormat.RGBA32, false);
        Vector2 c = new Vector2(16f, 16f);
        for (int y = 0; y < 32; y++)
        for (int x = 0; x < 32; x++)
        {
            float d = Vector2.Distance(new Vector2(x, y), c);
            tex.SetPixel(x, y, d < 14f ? Color.white : Color.clear);
        }

        tex.Apply();
        _fallbackSprite = Sprite.Create(tex, new Rect(0, 0, 32, 32), new Vector2(0.5f, 0.5f), 32f);
        return _fallbackSprite;
    }

    void EnsureEventSystem()
    {
        if (FindFirstObjectByType<EventSystem>() != null)
            return;

        var es = new GameObject("EventSystem");
        es.AddComponent<EventSystem>();
        es.AddComponent<StandaloneInputModule>();
    }
}
