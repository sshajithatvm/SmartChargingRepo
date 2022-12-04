using FluentValidation;

namespace SmartChargingApi.Validators;

public class ConnectorValidator : AbstractValidator<Connector>
{
    public ConnectorValidator()
	{       

        RuleFor(x => x.ConnectorId).InclusiveBetween(1,5);
        RuleFor(x => x.MaximumCurrentInAmps).GreaterThan(0);

        RuleFor(x => x.MaximumCurrentInAmps)
          .Must(ValidateMaximumCurrentInAmps)
          .WithMessage("Group CapacityInAmps must be great or equal to the MaximumCurrentInAmps of the Connector of all Charge Stations");       
    }

    private bool ValidateMaximumCurrentInAmps(Connector connector, double newMaximumCurrentInAmps)
    {  
        var group = connector.ChargeStation.Group;
        var chargeStations = group.ChargeStations;

        var sumOfMaximumCurrent = (from chargeStation in chargeStations
                                   from connector1 in chargeStation.Connectors//.SkipWhile(x=>x.ConnectorId == connector.ConnectorId)
                                   select connector1.MaximumCurrentInAmps).Sum();

        return group.CapacityInAmps >= (sumOfMaximumCurrent + newMaximumCurrentInAmps);
    }

}
