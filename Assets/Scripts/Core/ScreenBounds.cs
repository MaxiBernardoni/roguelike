using UnityEngine;

/// <summary>
/// Limita posiciones 2D al rectángulo visible de la cámara (viewport → mundo) con margen.
/// </summary>
public static class ScreenBounds
{
    public static bool TryGetWorldBounds(Camera cam, float marginWorld, out float minX, out float maxX, out float minY, out float maxY)
    {
        minX = maxX = minY = maxY = 0f;
        if (cam == null)
            return false;

        float dist = Mathf.Abs(cam.transform.position.z);
        if (dist < 0.01f)
            dist = 10f;

        Vector3 bl = cam.ViewportToWorldPoint(new Vector3(0f, 0f, dist));
        Vector3 tr = cam.ViewportToWorldPoint(new Vector3(1f, 1f, dist));

        minX = Mathf.Min(bl.x, tr.x) + marginWorld;
        maxX = Mathf.Max(bl.x, tr.x) - marginWorld;
        minY = Mathf.Min(bl.y, tr.y) + marginWorld;
        maxY = Mathf.Max(bl.y, tr.y) - marginWorld;

        if (minX > maxX || minY > maxY)
        {
            float cx = (minX + maxX) * 0.5f;
            float cy = (minY + maxY) * 0.5f;
            minX = maxX = cx;
            minY = maxY = cy;
        }

        return true;
    }

    public static Vector2 ClampPosition(Vector2 pos, float marginWorld)
    {
        Camera cam = Camera.main != null ? Camera.main : UnityEngine.Object.FindFirstObjectByType<Camera>();
        if (!TryGetWorldBounds(cam, marginWorld, out float minX, out float maxX, out float minY, out float maxY))
            return pos;

        pos.x = Mathf.Clamp(pos.x, minX, maxX);
        pos.y = Mathf.Clamp(pos.y, minY, maxY);
        return pos;
    }

    public static void ClampRigidbody(Rigidbody2D rb, float marginWorld)
    {
        if (rb == null)
            return;
        rb.position = ClampPosition(rb.position, marginWorld);
    }
}
