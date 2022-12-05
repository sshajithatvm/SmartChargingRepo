using FluentValidation;

namespace SmartChargingApi.Validators;
public class ChargeStationValidator : AbstractValidator<ChargeStationModel>
{
	public ChargeStationValidator()
	{
        RuleFor(x => x.ChargeStationName).NotNull().NotEmpty();
        RuleFor(x => x.Connectors)
          .Must(ValidateMaximumCurrentInAmps)
          .WithMessage("Group CapacityInAmps must be great or equal to the MaximumCurrentInAmps of the Connector of all Charge Stations");
        RuleFor(x => x.Connectors.Count()).GreaterThanOrEqualTo(1).LessThanOrEqualTo(5);
    }
    private bool ValidateMaximumCurrentInAmps(ChargeStationModel chargeStation, List<ConnectorModel> newConnectors)
    {
        var group1 = chargeStation.Group;
        var chargeStations = group1.ChargeStations.Where(x => x.ChargeStationId != chargeStation.ChargeStationId);

        var sumOfMaximumCurrent = (from chargeStation1 in chargeStations
                                   from connector1 in chargeStation1.Connectors
                                   select connector1.MaximumCurrentInAmps).Sum();

        var newSumOfMaximumCurrent = newConnectors.Sum(x => x.MaximumCurrentInAmps);

        return group1.CapacityInAmps >= (sumOfMaximumCurrent + newSumOfMaximumCurrent);
    }
}
