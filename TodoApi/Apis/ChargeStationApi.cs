using AutoMapper;
using FluentValidation;
using SmartChargingApi.Repository;

namespace SmartChargingApi.Apis;

public class ChargeStationApi
{
    private readonly ILogger<ChargeStationApi> _logger;
    private readonly IMapper _mapper;
    
    public ChargeStationApi(ILogger<ChargeStationApi> logger, IMapper mapper)
    {
        _logger = logger;
        _mapper = mapper;
    }
    public void Register(IEndpointRouteBuilder app)
    {
        app.MapPost("/groups/{groupId}/chargeStations", CreateChargeStation);
        app.MapPut("/groups/{groupId}/chargeStations/{id}", UpdateChargeStation);
        app.MapDelete("/groups/{groupId}/chargeStations/{id}", DeleteChargeStation);
        app.MapGet("/groups/{groupId}/chargeStations/{id}", GetChargeStation).ExcludeFromDescription();
    }

    public async Task<IResult> CreateChargeStation(IValidator<ChargeStation> validator, ISmartChargingRepository repository, int groupId, ChargeStationModel model)
    {
        try
        {
            model.GroupId = groupId;
            var existingGroup = await repository.GetItemAsync<Group>(x => x.GroupId == groupId, new[] { "ChargeStations.Connectors" });
            if (existingGroup == null)
            {
                return Results.NotFound("Please provide existing group id");
            }

            var newChargeStation = _mapper.Map<ChargeStation>(model);
            newChargeStation.Group = existingGroup;

            var validationResult = await validator.ValidateAsync(newChargeStation);
            if (!validationResult.IsValid)
            {
                return Results.ValidationProblem(validationResult.ToDictionary());
            }
            newChargeStation.Group = null;

            repository.Add(newChargeStation);

            if (await repository.SaveAll())
            {
                return Results.Created($"/groups/{groupId}/chargeStations/{newChargeStation.ChargeStationId}",
                  _mapper.Map<ChargeStationModel>(newChargeStation));
            }
        }
        catch (Exception ex)
        {
            _logger.LogError("Failed while creating ChargeStation: {ex}", ex);
        }
        return Results.BadRequest("Failed to create ChargeStation");
    }
    public async Task<IResult> UpdateChargeStation(IValidator<ChargeStation> validator, ISmartChargingRepository repository, int id, int groupId, ChargeStationModel model)
    {
        try
        {
            model.GroupId= groupId;
            var existingChargeStation = await repository.GetItemAsync<ChargeStation>(x => x.ChargeStationId == id, new[] { "Connectors" });
            if (existingChargeStation == null)
            {
                return Results.NotFound("ChargeStation id not found");
            }           

            var existingGroup = await repository.GetItemAsync<Group>(x => x.GroupId == groupId, new[] { "ChargeStations.Connectors" });
            if (existingGroup == null)
            {
                return Results.NotFound("Please provide existing group id");
            }

            if(existingChargeStation.GroupId != existingGroup.GroupId)
            {
                return Results.BadRequest($"ChargeStation alreay connected to Groupd : id {existingChargeStation.GroupId}");
            }

            _mapper.Map(model, existingChargeStation);
            existingChargeStation.Group = existingGroup;

            var validationResult = await validator.ValidateAsync(existingChargeStation);
            if (!validationResult.IsValid)
            {
                return Results.ValidationProblem(validationResult.ToDictionary());
            }
            existingChargeStation.Group = null;

            repository.Update(existingChargeStation);
            if (await repository.SaveAll())
            {
                return Results.Ok(_mapper.Map<ChargeStationModel>(existingChargeStation));
            }
        }
        catch (Exception ex)
        {
            _logger.LogError("Failed while updating ChargeStation: {ex}", ex);
        }
        return Results.BadRequest("Bad request : Failed to update ChargeStation");
    }
    public async Task<IResult> DeleteChargeStation(ISmartChargingRepository repository, int id, int groupId)
    {
        try
        {
            var existingChargeStation = await repository.GetItemAsync<ChargeStation>(x => x.ChargeStationId == id, new[] { "Connectors" });
            if (existingChargeStation == null)
            {
                return Results.NotFound("ChargeStation id not found");
            }

            var existingGroup = await repository.GetItemAsync<Group>(x => x.GroupId == groupId, new[] { "ChargeStations.Connectors" });
            if (existingGroup == null)
            {
                return Results.NotFound("Please provide existing group id");
            }

            if (existingChargeStation.GroupId != existingGroup.GroupId)
            {
                return Results.BadRequest($"ChargeStation connected to Groupd : id {existingChargeStation.GroupId}");
            }

            repository.Delete(existingChargeStation);
            if (await repository.SaveAll())
            {
                return Results.Ok("Successfully deleted ChargeStation");
            }
        }
        catch (Exception ex)
        {
            _logger.LogError("Failed while deleting ChargeStation: {ex}", ex);
        }
        return Results.BadRequest("Failed to deleting ChargeStation");
    }

    /// <summary>
    /// Added for testing purpose
    /// </summary>
    public async Task<IResult> GetChargeStation(ISmartChargingRepository repository, int id, int groupId)
    {
        var chargeStation = await repository.GetItemAsync<ChargeStation>(x => x.ChargeStationId == id, new[] { "Connectors" });
        if (chargeStation == null)
        {
            return Results.NotFound("ChargeStation id not found");
        }
        return Results.Ok(_mapper.Map<ChargeStationModel>(chargeStation));

    }
}
