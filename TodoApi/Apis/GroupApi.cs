using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using FluentValidation;
using SmartChargingApi.Repository;

namespace SmartChargingApi.Apis;
public class GroupApi
{
    private readonly ILogger<GroupApi> _logger;
    private readonly IMapper _mapper;
    public GroupApi(ILogger<GroupApi> logger, IMapper mapper)
    {
        _logger = logger;
        _mapper = mapper;
    }
    public void Register(IEndpointRouteBuilder app)
    {
        app.MapPost("/groups", CreateGroup);
        app.MapPut("/groups/{id}", UpdateGroup);
        app.MapDelete("/groups/{id}", DeleteGroup);
    }

    public async Task<IResult> CreateGroup(IValidator<Group> validator, ISmartChargingRepository repository, GroupModel model)
    {
        try
        {
            var newGroup = _mapper.Map<Group>(model);

            var validationResult = await validator.ValidateAsync(newGroup);
            if (!validationResult.IsValid)
            {
                return Results.ValidationProblem(validationResult.ToDictionary());
            }
            
            repository.Add(newGroup);

            if (await repository.SaveAll())
            {
                return Results.Created($"/groups/{newGroup.GroupId}",
                  _mapper.Map<GroupModel>(newGroup));
            }
        }
        catch (Exception ex)
        {
            _logger.LogError("Failed while creating group: {ex}", ex);
        }
        return Results.BadRequest("Failed to create group");
    }
    public async Task<IResult> UpdateGroup(IValidator<Group> validator, ISmartChargingRepository repository, int id, GroupModel model)
    {
        try
        {
            var existingGroup = await repository.GetItemAsync<Group>(x => x.GroupId == id, new[] { "ChargeStations.Connectors" });
            if (existingGroup == null)
            {
                return Results.NotFound("Group id not found");
            }

            _mapper.Map(model, existingGroup);

            var validationResult = await validator.ValidateAsync(existingGroup);
            if (!validationResult.IsValid)
            {
                return Results.ValidationProblem(validationResult.ToDictionary());
            }

            repository.Update(existingGroup);
            if (await repository.SaveAll())
            {
                return Results.Ok(_mapper.Map<GroupModel>(existingGroup));
            }
        }
        catch (Exception ex)
        {
            _logger.LogError("Failed while updating group: {ex}", ex);
        }
        return Results.BadRequest("Failed to update group");
    }

    public async Task<IResult> DeleteGroup(ISmartChargingRepository repository, int id)
    {
        try
        {
            var existingGroup = await repository.GetItemAsync<Group>(x => x.GroupId == id, new[] { "ChargeStations.Connectors" });
            if (existingGroup == null)
            {
                return Results.NotFound("Group id not found");
            }

            repository.Delete(existingGroup);
            if (await repository.SaveAll())
            {
                return Results.Ok($"Group deteted successfully");
            }
        }
        catch (Exception ex)
        {
            _logger.LogError("Failed while deleting group: {ex}", ex);
        }
        return Results.BadRequest("Failed to deleting group");
    }    
}
