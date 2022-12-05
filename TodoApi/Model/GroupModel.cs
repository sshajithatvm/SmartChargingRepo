using System.ComponentModel;
using System.Text.Json.Serialization;

public class GroupModel
{
    [JsonPropertyName("Group Name")]
    [DefaultValue("Group 1")]
    public string GroupName { get; set; }

    [JsonPropertyName("Capacity in Amps")]
    [DefaultValue(1)]
    public double CapacityInAmps { get; set; }

    [JsonIgnore]
    public List<ChargeStationModel> ChargeStations { get; set; } = new List<ChargeStationModel>();
}
