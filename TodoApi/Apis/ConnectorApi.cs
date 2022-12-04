using AutoMapper;
using FluentValidation;
using SmartChargingApi.Repository;
using System.Text.RegularExpressions;

namespace SmartChargingApi.Apis;

public class ConnectorApi
{
    private readonly ILogger<ConnectorApi> _logger;
    private readonly IMapper _mapper;

    public ConnectorApi(ILogger<ConnectorApi> logger, IMapper mapper)
    {
        _logger = logger;
        _mapper = mapper;
    }
    public void Register(IEndpointRouteBuilder app)
    {
        app.MapPost("/chargeStations/{chargeStationId}/connectors", CreateConnector);
        app.MapPut("/chargeStations/{chargeStationId}/connectors/{id}", UpdateConnector);
        app.MapDelete("/chargeStations/{chargeStationId}/connectors/{id}", DeleteConnector);
    }

    public async Task<IResult> CreateConnector(IValidator<Connector> validator, ISmartChargingRepository repository, int chargeStationId, ConnectorModel model)
    {
        try
        {

            model.ChargeStationId = chargeStationId;
            var existingChargeStation = await repository.GetItemAsync<ChargeStation>(x => x.ChargeStationId == chargeStationId, new[] { "Connectors", "Group" });
            if (existingChargeStation == null)
            {
                return Results.NotFound("Please provide existing chargeStation id");
            }
            
            var existingGroup = await repository.GetItemAsync<Group>(x => x.GroupId == existingChargeStation.GroupId, new[] { "ChargeStations.Connectors" });
            
            var newConnector = _mapper.Map<Connector>(model);
            newConnector.ConnectorId = FindMissingId(existingChargeStation);
            existingChargeStation.Group = existingGroup;
            newConnector.ChargeStation = existingChargeStation;

            var validationResult = await validator.ValidateAsync(newConnector);
            if (!validationResult.IsValid)
            {
                return Results.ValidationProblem(validationResult.ToDictionary());
            }
            newConnector.ChargeStation = null;

            repository.Add(newConnector);

            if (await repository.SaveAll())
            {
                return Results.Created($"/chargeStations/{chargeStationId}/connectors/{newConnector.ConnectorId}",
                  _mapper.Map<ConnectorModel>(newConnector));
            }
        }
        catch (Exception ex)
        {
            _logger.LogError("Failed while creating Connector: {ex}", ex);
        }
        return Results.BadRequest("Failed to create Connector");
    }   

    public async Task<IResult> UpdateConnector(int id, IValidator<Connector> validator, ISmartChargingRepository repository, int chargeStationId, ConnectorModel model)
    {
        try
        {
            model.ChargeStationId = chargeStationId;
            var existingConnector = await repository.GetItemAsync<Connector>(x => x.ConnectorId == id, new[] { "ChargeStation.Group" });
            if (existingConnector == null)
            {
                return Results.NotFound("Connector id not found");
            }

            var existingChargeStation = await repository.GetItemAsync<ChargeStation>(x => x.ChargeStationId == chargeStationId, new[] { "Connectors", "Group" });
            if (existingChargeStation == null)
            {
                return Results.NotFound("Please provide existing chargeStation id");
            }

            if (existingConnector.ChargeStationId != existingChargeStation.ChargeStationId)
            {
                return Results.BadRequest($"Connector alreay connected to ChargeStation : id {existingConnector.ChargeStationId}");
            }
            var existingGroup = await repository.GetItemAsync<Group>(x => x.GroupId == existingChargeStation.GroupId, new[] { "ChargeStations.Connectors" });

            _mapper.Map(model, existingConnector);
            existingChargeStation.Group = existingGroup;
            existingConnector.ChargeStation = existingChargeStation;

            var validationResult = await validator.ValidateAsync(existingConnector);
            if (!validationResult.IsValid)
            {
                return Results.ValidationProblem(validationResult.ToDictionary());
            }
            existingConnector.ChargeStation = null;

            repository.Update(existingConnector);
            if (await repository.SaveAll())
            {
                return Results.Ok(_mapper.Map<ConnectorModel>(existingConnector));
            }
        }
        catch (Exception ex)
        {
            _logger.LogError("Failed while updating Connector: {ex}", ex);
        }
        return Results.BadRequest("Failed to update Connector");
    }

    public async Task<IResult> DeleteConnector(ISmartChargingRepository repository, int id, int chargeStationId)
    {
        try
        {
            var existingConnector = await repository.GetItemAsync<Connector>(x => x.ConnectorId == id, new[] { "ChargeStation.Group" });
            if (existingConnector == null)
            {
                return Results.NotFound("Connector id not found");
            }

            var existingChargeStation = await repository.GetItemAsync<ChargeStation>(x => x.ChargeStationId == chargeStationId, new[] { "Connectors", "Group" });
            if (existingChargeStation == null)
            {
                return Results.NotFound("Please provide existing chargeStation id");
            }

            if (existingConnector.ChargeStationId != existingChargeStation.ChargeStationId)
            {
                return Results.BadRequest($"Connector alreay connected to ChargeStation : id {existingConnector.ChargeStationId}");
            }

            repository.Delete(existingConnector);
            if (await repository.SaveAll())
            {
                return Results.Ok("Successfully deleted connector");
            }
        }
        catch (Exception ex)
        {
            _logger.LogError("Failed while deleting Connector: {ex}", ex);
        }
        return Results.BadRequest("Failed to deleting Connector");
    }
    private static int FindMissingId(ChargeStation existingChargeStation)
    {
        return existingChargeStation.Connectors.Any() ? existingChargeStation.Connectors.Max(x => x.ConnectorId) + 1 : 1;
    }
}
