using System;

/// <summary>
/// Definición de una mejora: nombre, descripción y efecto aplicado al jugador (vía arma, stats, etc.).
/// </summary>
public class Upgrade
{
    public string Name;
    public string Description;
    public Action<PlayerController> ApplyEffect;
}
