using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

public class ChargeStation
{
    [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int ChargeStationId { get; set; }
    public string ChargeStationName { get; set; }

    public List<Connector> Connectors { get; set; } = new List<Connector>();

    public int GroupId { get; set; }
    public Group Group { get; set; }   
}
