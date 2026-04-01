using UnityEngine;

/// <summary>
/// Ayuda a evitar máscaras en "Nothing" cuando la capa "Enemy" no existe o no está asignada en el inspector.
/// </summary>
public static class GameLayers
{
    const string EnemyLayerName = "Enemy";

    /// <summary>Máscara para overlap de enemigos; si no hay capa Enemy, usa todas las capas.</summary>
    public static LayerMask GetEnemyMask(LayerMask configured)
    {
        if (configured.value != 0)
            return configured;

        int byName = LayerMask.GetMask(EnemyLayerName);
        if (byName != 0)
            return byName;

        return -1;
    }
}
