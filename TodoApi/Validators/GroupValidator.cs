using FluentValidation;

namespace SmartChargingApi.Validators;
public class GroupValidator :AbstractValidator<GroupModel>
{
    public GroupValidator()
    {
        RuleFor(x => x.GroupName).NotNull().NotEmpty();
        RuleFor(x => x.CapacityInAmps).GreaterThan(0);

        RuleFor(x => x.CapacityInAmps)
        .Must(ValidateGroupCapacityInAmps)
        .When(m => m.ChargeStations.Count() > 0)
        .WithMessage($"Group CapacityInAmps must be great or equal to the MaximumCurrentInAmps of the Connector of all Charge Stations");
    }

    private bool ValidateGroupCapacityInAmps(GroupModel group, double capacityInAmps)
    {
        var chargeStations = group.ChargeStations;
        var sumOfMaximumCurrent = (from chargeStation in chargeStations
                                   from connector in chargeStation.Connectors
                                   select connector.MaximumCurrentInAmps).Sum();

        return capacityInAmps >= sumOfMaximumCurrent;
    }
}
