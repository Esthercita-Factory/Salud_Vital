namespace SaludVital.Models;

public sealed class ConsultaDashboardItem
{
    public required Consulta Consulta { get; init; }

    public required Mascota Mascota { get; init; }
}
