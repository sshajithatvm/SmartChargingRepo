using AutoMapper.Configuration.Annotations;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel;
using System.Text.Json.Serialization;
using System.Runtime.Serialization;

public class GroupModel
{
    [JsonPropertyName("Group Name")]
    [DefaultValue("Group 1")]
    public string GroupName { get; set; }

    [JsonPropertyName("Capacity in Amps")]
    [DefaultValue(1)]
    public double CapacityInAmps { get; set; }
}
