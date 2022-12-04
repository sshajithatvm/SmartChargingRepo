using System.Net;
using System.Net.Http.Json;
using Xunit;

public class ConnectorApiTests
{
    /// <summary>
    /// A Connector cannot exist in the domain without a Charge Station.
    /// </summary>
    [Fact]
    public async Task CreateConnectorWithoutChargeStationTest()
    {
        await using var application = new ApplicationFactory();
        var client = application.CreateClient();        

        //Create a Connector without a ChargeStation
        var model = new ConnectorModel { MaximumCurrentInAmps = 10 };
        var response = await client.PostAsJsonAsync($"/chargeStations/{1}/connectors", model);

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task CreateConnectorTest()
    {
        await using var application = new ApplicationFactory();
        var client = application.CreateClient();

        //Create a Group
        var groupModel = new GroupModel { GroupName = "GroupName 1", CapacityInAmps = 10 };
        var groupResponse = await client.PostAsJsonAsync("/groups", groupModel);
        var groupValue = groupResponse.Content.ReadFromJsonAsync<GroupModel>().Result;

        //Create a ChargeStation with a Group
        var chargeStationModel = new ChargeStationModel { GroupId = 1, ChargeStationName = "ChargeStationName 1", Connectors = { new ConnectorModel { MaximumCurrentInAmps = 2 } } };
        var chargeStationResponse = await client.PostAsJsonAsync($"/groups/{1}/chargeStations", chargeStationModel);
        var chargeStationValue = chargeStationResponse.Content.ReadFromJsonAsync<ChargeStationModel>().Result;

        //Create a Connector with a ChargeStation
        var model = new ConnectorModel { MaximumCurrentInAmps = 5 };
        var response = await client.PostAsJsonAsync($"/chargeStations/{1}/connectors", model);
        var returnValue = response.Content.ReadFromJsonAsync<ConnectorModel>().Result;

        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
        Assert.Equal($"/chargeStations/{1}/connectors/{2}", response.Headers.First().Value.First());
    }

    [Fact]
    public async Task UpdateConnectorTest()
    {
        await using var application1 = new ApplicationFactory();
        var client = application1.CreateClient();

        //Create a Group
        var groupModel = new GroupModel { GroupName = "GroupName 1", CapacityInAmps = 14 };
        var groupResponse = await client.PostAsJsonAsync("/groups", groupModel);
        var groupValue = groupResponse.Content.ReadFromJsonAsync<GroupModel>().Result;

        //Create a ChargeStation with a Group
        var chargeStationModel = new ChargeStationModel { GroupId = 1, ChargeStationName = "ChargeStationName 1", Connectors = { new ConnectorModel { MaximumCurrentInAmps = 2 } } };
        var chargeStationResponse = await client.PostAsJsonAsync($"/groups/{1}/chargeStations", chargeStationModel);
        var chargeStationValue = chargeStationResponse.Content.ReadFromJsonAsync<ChargeStationModel>().Result;

        //Create a Connector with a ChargeStation
        var model = new ConnectorModel { MaximumCurrentInAmps = 6 };
        var response = await client.PostAsJsonAsync($"/chargeStations/{1}/connectors", model);
        var returnValue = response.Content.ReadFromJsonAsync<ConnectorModel>().Result;

        //Update a Connector with a ChargeStation
        var updatedModel = new ConnectorModel { MaximumCurrentInAmps = 4 };
        var response1 = await client.PutAsJsonAsync($"/chargeStations/{1}/connectors/{1}", updatedModel);
        var returnValue1 = response1.Content.ReadFromJsonAsync<ConnectorModel>().Result;
        
        Assert.Equal(HttpStatusCode.OK, response1.StatusCode);
        Assert.Equal(updatedModel.MaximumCurrentInAmps, returnValue1.MaximumCurrentInAmps);
    }

    [Fact]
    public async Task DeleteConnectorTest()
    {

        await using var application1 = new ApplicationFactory();
        var client = application1.CreateClient();

        //Create a Group
        var groupModel = new GroupModel { GroupName = "GroupName 1", CapacityInAmps = 12 };
        var groupResponse = await client.PostAsJsonAsync("/groups", groupModel);
        var groupValue = groupResponse.Content.ReadFromJsonAsync<GroupModel>().Result;

        //Create a ChargeStation with a Group
        var chargeStationModel = new ChargeStationModel { GroupId = 1, ChargeStationName = "ChargeStationName 1", Connectors = { new ConnectorModel { MaximumCurrentInAmps = 2 } } };
        var chargeStationResponse = await client.PostAsJsonAsync($"/groups/{1}/chargeStations", chargeStationModel);
        var chargeStationValue = chargeStationResponse.Content.ReadFromJsonAsync<ChargeStationModel>().Result;

        //Create a Connector with a ChargeStation
        var model = new ConnectorModel { MaximumCurrentInAmps = 10 };
        var response = await client.PostAsJsonAsync($"/chargeStations/{1}/connectors", model);
        var returnValue = response.Content.ReadFromJsonAsync<ConnectorModel>().Result;

        //Delete a Connector
        response = await client.DeleteAsync($"/chargeStations/{1}/connectors/{1}");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

    [Fact]
    public async Task ValidateMaximumCurrentInAmpsEqualZeroTest()
    {
        await using var application1 = new ApplicationFactory();
        var client = application1.CreateClient();

        //Create a Group
        var groupModel = new GroupModel { GroupName = "GroupName 1", CapacityInAmps = 10 };
        var groupResponse = await client.PostAsJsonAsync("/groups", groupModel);
        var groupValue = groupResponse.Content.ReadFromJsonAsync<GroupModel>().Result;

        //Create a ChargeStation with a Group
        var chargeStationModel = new ChargeStationModel { GroupId = 1, ChargeStationName = "ChargeStationName 1", Connectors = { new ConnectorModel { MaximumCurrentInAmps = 2 } } };
        var chargeStationResponse = await client.PostAsJsonAsync($"/groups/{1}/chargeStations", chargeStationModel);
        var chargeStationValue = chargeStationResponse.Content.ReadFromJsonAsync<ChargeStationModel>().Result;

        //Create a Connector with a ChargeStation
        var model = new ConnectorModel { MaximumCurrentInAmps = 0 };
        var response = await client.PostAsJsonAsync($"/chargeStations/{1}/connectors", model);
        var returnValue = response.Content.ReadFromJsonAsync<ConnectorModel>().Result;

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    /// <summary>
    /// Charge station contain multiple connectors at least one, but not more than 5, Range 1 to 5
    /// </summary>
    [Fact]
    public async Task ValidateChargeStationContainsContainMultipleConnectorsTest()
    {
        await using var application1 = new ApplicationFactory();
        var client = application1.CreateClient();

        //Create a Group
        var groupModel = new GroupModel { GroupName = "GroupName 1", CapacityInAmps = 30 };
        var groupResponse = await client.PostAsJsonAsync("/groups", groupModel);
        var groupValue = groupResponse.Content.ReadFromJsonAsync<GroupModel>().Result;

        //Create a ChargeStation with Connector 1
        var chargeStationModel = new ChargeStationModel { GroupId = 1, ChargeStationName = "ChargeStationName 1", Connectors = { new ConnectorModel { MaximumCurrentInAmps = 2 } } };
        var chargeStationResponse = await client.PostAsJsonAsync($"/groups/{1}/chargeStations", chargeStationModel);
        var chargeStationValue = chargeStationResponse.Content.ReadFromJsonAsync<ChargeStationModel>().Result;

        //Create a Connector 2 with a ChargeStation
        var model = new ConnectorModel { MaximumCurrentInAmps = 2 };
        var response = await client.PostAsJsonAsync($"/chargeStations/{1}/connectors", model);
        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
        Assert.Equal($"/chargeStations/{1}/connectors/{2}", response.Headers.First().Value.First());

        //Create a Connector 3 with a ChargeStation
        var model1 = new ConnectorModel { MaximumCurrentInAmps = 1 };
        var response1 = await client.PostAsJsonAsync($"/chargeStations/{1}/connectors", model1);
        Assert.Equal(HttpStatusCode.Created, response1.StatusCode);
        Assert.Equal($"/chargeStations/{1}/connectors/{3}", response1.Headers.First().Value.First());

        //Create a Connector 4 with a ChargeStation
        var model2 = new ConnectorModel { MaximumCurrentInAmps = 3 };
        var response2 = await client.PostAsJsonAsync($"/chargeStations/{1}/connectors", model2);
        Assert.Equal(HttpStatusCode.Created, response2.StatusCode);
        Assert.Equal($"/chargeStations/{1}/connectors/{4}", response2.Headers.First().Value.First());

        //Create a Connector 5 with a ChargeStation
        var model3 = new ConnectorModel { MaximumCurrentInAmps = 4 };
        var response3 = await client.PostAsJsonAsync($"/chargeStations/{1}/connectors", model3);
        Assert.Equal(HttpStatusCode.Created, response3.StatusCode);
        Assert.Equal($"/chargeStations/{1}/connectors/{5}", response3.Headers.First().Value.First());

        //Create a Connector 6 with a ChargeStation
        var model4 = new ConnectorModel { MaximumCurrentInAmps = 5 };
        var response4 = await client.PostAsJsonAsync($"/chargeStations/{1}/connectors", model4);
        Assert.Equal(HttpStatusCode.BadRequest, response4.StatusCode);
    }

    /// <summary>
    /// The capacity in Amps of a Group should always be great or equal to 
    /// the sum of the Max current in Amps of the Connector of all Charge Stations in the Group
    /// </summary>
    [Fact]
    public async Task ValidateGroupCapacityInAmpsWhileCreatingConnectorTest()
    {
        await using var application1 = new ApplicationFactory();
        var client = application1.CreateClient();

        //Create a Group
        var groupModel = new GroupModel { GroupName = "GroupName 1", CapacityInAmps = 9 };
        var groupResponse = await client.PostAsJsonAsync("/groups", groupModel);
        var groupValue = groupResponse.Content.ReadFromJsonAsync<GroupModel>().Result;

        //Create a ChargeStation 1 with a Group 1 (Connector MaximumCurrentInAmps 2)
        var chargeStationModel = new ChargeStationModel { GroupId = 1, ChargeStationName = "ChargeStationName 1", Connectors = { new ConnectorModel { MaximumCurrentInAmps = 2 } } };
        var chargeStationResponse = await client.PostAsJsonAsync($"/groups/{1}/chargeStations", chargeStationModel);
        Assert.Equal(HttpStatusCode.Created, chargeStationResponse.StatusCode);

        //Create a ChargeStation 2 with a Group 1 (Connector MaximumCurrentInAmps 3)
        var chargeStationModel1 = new ChargeStationModel { GroupId = 1, ChargeStationName = "ChargeStationName 1", Connectors = { new ConnectorModel { MaximumCurrentInAmps = 3 } } };
        var chargeStationResponse1 = await client.PostAsJsonAsync($"/groups/{1}/chargeStations", chargeStationModel1);
        Assert.Equal(HttpStatusCode.Created, chargeStationResponse1.StatusCode);

        //Create a Connector 1 with a MaximumCurrentInAmps 2
        var connectorModel = new ConnectorModel { MaximumCurrentInAmps = 1 };
        var connectorResponse = await client.PostAsJsonAsync($"/chargeStations/{1}/connectors", connectorModel);
        Assert.Equal(HttpStatusCode.Created, connectorResponse.StatusCode);

        //Create a Connector 2 with a MaximumCurrentInAmps 6
        var connectorModel1 = new ConnectorModel { MaximumCurrentInAmps = 6 };
        var connectorResponse1 = await client.PostAsJsonAsync($"/chargeStations/{1}/connectors", connectorModel1);
        Assert.Equal(HttpStatusCode.BadRequest, connectorResponse1.StatusCode);

        //Create a Connector 3 with a with a MaximumCurrentInAmps 2
        var connectorModel2 = new ConnectorModel { MaximumCurrentInAmps = 3 };
        var connectorResponse2 = await client.PostAsJsonAsync($"/chargeStations/{1}/connectors", connectorModel2);
        Assert.Equal(HttpStatusCode.Created, connectorResponse2.StatusCode);
    }

    /// <summary>
    /// The capacity in Amps of a Group should always be great or equal to 
    /// the sum of the Max current in Amps of the Connector of all Charge Stations in the Group
    /// </summary>
    [Fact]
    public async Task ValidateGroupCapacityInAmpsWhileUpdatingConnectorTest()
    {
        await using var application1 = new ApplicationFactory();
        var client = application1.CreateClient();

        //Create a Group
        var groupModel = new GroupModel { GroupName = "GroupName 1", CapacityInAmps = 10 };
        var groupResponse = await client.PostAsJsonAsync("/groups", groupModel);
        var groupValue = groupResponse.Content.ReadFromJsonAsync<GroupModel>().Result;

        //Create a ChargeStation 1 with a Group 1 (Connector MaximumCurrentInAmps 2)
        var chargeStationModel = new ChargeStationModel { GroupId = 1, ChargeStationName = "ChargeStationName 1", Connectors = { new ConnectorModel { MaximumCurrentInAmps = 3 } } };
        var chargeStationResponse = await client.PostAsJsonAsync($"/groups/{1}/chargeStations", chargeStationModel);
        Assert.Equal(HttpStatusCode.Created, chargeStationResponse.StatusCode);

        //Create a ChargeStation 2 with a Group 1 (Connector MaximumCurrentInAmps 3)
        var chargeStationModel1 = new ChargeStationModel { GroupId = 1, ChargeStationName = "ChargeStationName 1", Connectors = { new ConnectorModel { MaximumCurrentInAmps = 4 } } };
        var chargeStationResponse1 = await client.PostAsJsonAsync($"/groups/{1}/chargeStations", chargeStationModel1);
        Assert.Equal(HttpStatusCode.Created, chargeStationResponse1.StatusCode);

        //Create a Connector 1 with a MaximumCurrentInAmps 1
        var connectorModel = new ConnectorModel { MaximumCurrentInAmps = 2 };
        var connectorResponse = await client.PostAsJsonAsync($"/chargeStations/{1}/connectors", connectorModel);
        Assert.Equal(HttpStatusCode.Created, connectorResponse.StatusCode);        

        //Create a Connector 2 with a with a MaximumCurrentInAmps 2
        var connectorModel2 = new ConnectorModel { MaximumCurrentInAmps = 1 };
        var connectorResponse2 = await client.PostAsJsonAsync($"/chargeStations/{1}/connectors", connectorModel2);
        Assert.Equal(HttpStatusCode.Created, connectorResponse2.StatusCode);

        //Update a Connector with a ChargeStation
        var updatedModel = new ConnectorModel { MaximumCurrentInAmps = 5 };
        var response1 = await client.PutAsJsonAsync($"/chargeStations/{1}/connectors/{1}", updatedModel);
        Assert.Equal(HttpStatusCode.BadRequest, response1.StatusCode);
    }
}
