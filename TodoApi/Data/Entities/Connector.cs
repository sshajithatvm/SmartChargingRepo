using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

public class Connector
{
    [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int ConnectorId { get; set; }
    public double MaximumCurrentInAmps { get; set; }
    public int ChargeStationId { get; set; }
    public ChargeStation ChargeStation { get; set; }
}