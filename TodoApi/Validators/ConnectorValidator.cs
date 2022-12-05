using FluentValidation;

namespace SmartChargingApi.Validators;

public class ConnectorValidator : AbstractValidator<ConnectorModel>
{
    public ConnectorValidator()
	{    

        RuleFor(x => x.ConnectorId).InclusiveBetween(1,5);
        RuleFor(x => x.MaximumCurrentInAmps).GreaterThan(0);

        RuleFor(x => x.MaximumCurrentInAmps)
          .Must(ValidateMaximumCurrentInAmps)
          .WithMessage("Group CapacityInAmps must be great or equal to the MaximumCurrentInAmps of the Connector of all Charge Stations");       
    }

    private bool ValidateMaximumCurrentInAmps(ConnectorModel connector, double newMaximumCurrentInAmps)
    {  
        var group1 = connector.ChargeStation.Group;

        var sumOfMaximumCurrent = (from chargeStation in group1.ChargeStations
                                   from connector1 in chargeStation.Connectors.Where(x=>x.ConnectorId != connector.ConnectorId)
                                   select connector1.MaximumCurrentInAmps).Sum();

        return group1.CapacityInAmps >= (sumOfMaximumCurrent + newMaximumCurrentInAmps);
    }

}
