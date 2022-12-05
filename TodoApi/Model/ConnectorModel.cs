using System.ComponentModel;
using System.Text.Json.Serialization;

public class ConnectorModel
{
    [DefaultValue(1)]
    [JsonIgnore]
    public int ChargeStationId { get; set; }

    [JsonPropertyName("Maximum Current in Amps")]
    [DefaultValue(1)]
    public double MaximumCurrentInAmps { get; set; }

    [JsonIgnore]
    public int ConnectorId { get; set; }

    [JsonIgnore]
    public ChargeStation ChargeStation { get; set; }
}
