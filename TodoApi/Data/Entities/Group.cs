using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
public class Group
{
    [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int GroupId { get; set; }

    public string GroupName { get; set; }

    public double CapacityInAmps { get; set; }

    public List<ChargeStation> ChargeStations { get; set; } = new List<ChargeStation>();
}