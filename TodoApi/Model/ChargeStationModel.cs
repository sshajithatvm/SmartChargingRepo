using System.ComponentModel;
using System.Text.Json.Serialization;

public class ChargeStationModel
{
    [DefaultValue(1)]
    [JsonIgnore]
    public int GroupId { get; set; }

    [JsonPropertyName("Charge Station Name")]
    [DefaultValue("ChargeStation 1")]
    public string ChargeStationName { get; set; }

    public List<ConnectorModel> Connectors { get; set; } = new List<ConnectorModel>();
}
