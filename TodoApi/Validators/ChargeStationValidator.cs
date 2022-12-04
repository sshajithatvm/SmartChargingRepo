using FluentValidation;

namespace SmartChargingApi.Validators;
public class ChargeStationValidator : AbstractValidator<ChargeStation>
{
	public ChargeStationValidator()
	{
        RuleFor(x => x.ChargeStationName).NotNull().NotEmpty();
        //RuleForEach(x => x.Connectors).SetValidator(new ConnectorValidator()).When(m => m.Connectors.Count() > 0);
        RuleFor(x => x.Connectors)
          .Must(ValidateMaximumCurrentInAmps)
          .WithMessage("Group CapacityInAmps must be great or equal to the MaximumCurrentInAmps of the Connector of all Charge Stations");
        RuleFor(x => x.Connectors.Count()).GreaterThanOrEqualTo(1).LessThanOrEqualTo(5);
    }
    private bool ValidateMaximumCurrentInAmps(ChargeStation chargeStation, List<Connector> newConnectors)
    {
        var group = chargeStation.Group;
        var chargeStations = group.ChargeStations;

        var sumOfMaximumCurrent = (from chargeStation1 in chargeStations
                                   from connector1 in chargeStation1.Connectors
                                   select connector1.MaximumCurrentInAmps).Sum();

        var newSumOfMaximumCurrent = newConnectors.Sum(x => x.MaximumCurrentInAmps);

        return group.CapacityInAmps >= (sumOfMaximumCurrent + newSumOfMaximumCurrent);
    }
}
