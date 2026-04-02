using System.Collections;
using UnityEngine;

/// <summary>
/// Feedback visual de explosión: anillo naranja/amarillo, expansión ~0.15s, fade total ~0.28s, knockback y sacudida leve de cámara.
/// </summary>
public static class ExplosionFeedback
{
    public static void Play(Vector2 worldPos, float radius, LayerMask enemyMask)
    {
        var host = new GameObject("ExplosionFeedback");
        var run = host.AddComponent<ExplosionFeedbackRunner>();
        run.Begin(worldPos, radius, enemyMask);
    }
}

public class ExplosionFeedbackRunner : MonoBehaviour
{
    const float KnockbackForce = 4.5f;
    const float ShakeMag = 0.09f;
    const float TotalDuration = 0.28f;
    const float ExpandDuration = 0.15f;

    public void Begin(Vector2 pos, float radius, LayerMask enemyMask)
    {
        StartCoroutine(Run(pos, radius, enemyMask));
    }

    IEnumerator Run(Vector2 pos, float radius, LayerMask enemyMask)
    {
        ApplyKnockback(pos, radius, enemyMask);

        var ring = new GameObject("ExplosionRing");
        ring.transform.position = pos;
        var sr = ring.AddComponent<SpriteRenderer>();
        RuntimeVisuals.EnsureSprite(sr);
        sr.color = new Color(1f, 0.58f, 0.06f, 0.95f);
        sr.sortingOrder = CombatVisuals.SortExplosion;

        var cam = Camera.main;
        Vector3 camOrig = cam != null ? cam.transform.position : Vector3.zero;

        float t = 0f;
        float fadeStart = ExpandDuration;
        while (t < TotalDuration)
        {
            if (cam != null)
            {
                float shake = 1f - t / TotalDuration;
                cam.transform.position = camOrig + (Vector3)(Random.insideUnitCircle * (ShakeMag * shake));
            }

            if (t < ExpandDuration)
            {
                float k = t / ExpandDuration;
                ring.transform.localScale = Vector3.Lerp(Vector3.one * 0.08f, Vector3.one * (radius * 1.25f), k);
            }
            else
            {
                float fadeT = (t - fadeStart) / Mathf.Max(0.01f, TotalDuration - fadeStart);
                Color c = sr.color;
                c.a = Mathf.Lerp(0.95f, 0f, fadeT);
                sr.color = c;
            }

            t += Time.deltaTime;
            yield return null;
        }

        if (cam != null)
            cam.transform.position = camOrig;

        Destroy(ring);
        Destroy(gameObject);
    }

    void ApplyKnockback(Vector2 pos, float radius, LayerMask enemyMask)
    {
        var hits = Physics2D.OverlapCircleAll(pos, radius * 1.05f, enemyMask);
        foreach (var c in hits)
        {
            var rb = c.attachedRigidbody;
            if (rb == null)
                continue;
            if (c.GetComponent<EnemyBase>() == null)
                continue;

            Vector2 away = rb.position - pos;
            if (away.sqrMagnitude < 0.01f)
                away = Random.insideUnitCircle.normalized;
            else
                away.Normalize();

            rb.AddForce(away * KnockbackForce, ForceMode2D.Impulse);
        }
    }
}
