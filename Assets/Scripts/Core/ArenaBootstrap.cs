using UnityEngine;
using UnityEngine.EventSystems;

/// <summary>
/// Garantiza EventSystem + módulo de entrada para que los botones de recompensa funcionen con timeScale = 0.
/// Añade un objeto con este componente a la escena (una vez).
/// </summary>
[DefaultExecutionOrder(-200)]
public class ArenaBootstrap : MonoBehaviour
{
    [SerializeField] bool createEventSystemIfMissing = true;

    void Awake()
    {
        if (!createEventSystemIfMissing)
            return;

        if (FindFirstObjectByType<EventSystem>() != null)
            return;

        var es = new GameObject("EventSystem");
        es.AddComponent<EventSystem>();
        es.AddComponent<StandaloneInputModule>();
    }
}
