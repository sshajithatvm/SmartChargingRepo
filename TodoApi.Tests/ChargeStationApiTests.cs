using System.Net;
using System.Net.Http.Json;
using Xunit;

public class ChargeStationApiTests
{
    /// <summary>
    /// //The Charge Station cannot exist in the domain without Group.
    /// </summary>
    [Fact]
    public async Task CreateChargeStationWithoutGroupTest()
    {
        await using var application = new ApplicationFactory();
        var client = application.CreateClient();

        //Create a ChargeStation without a Group
        var model = new ChargeStationModel { ChargeStationName = "ChargeStationName 1" };
        var response = await client.PostAsJsonAsync($"/groups/{1}/chargeStations", model);

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    } 

    [Fact]
    public async Task CreateChargeStationTest()
    {
        await using var application = new ApplicationFactory();
        var client = application.CreateClient();

        //Create a Group
        var groupModel = new GroupModel { GroupName = "GroupName 1", CapacityInAmps = 10 };        
        var groupResponse = await client.PostAsJsonAsync("/groups", groupModel);

        //Create a ChargeStation with a Group
        var model = new ChargeStationModel { GroupId = 1, ChargeStationName = "ChargeStationName 1" , Connectors = { new ConnectorModel { MaximumCurrentInAmps = 2 } } };
        var response = await client.PostAsJsonAsync($"/groups/{1}/chargeStations", model);
        var returnValue = response.Content.ReadFromJsonAsync<ChargeStationModel>().Result;

        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
        Assert.Equal($"/groups/{1}/chargeStations/{1}", response.Headers.First().Value.First());
    }

    /// <summary>
    /// Group can contain multiple charge stations
    /// </summary>
    [Fact]
    public async Task ValidateGroupContainsMultipleChargeStationsTest()
    {
        await using var application = new ApplicationFactory();
        var client = application.CreateClient();

        //Create a Group
        var groupModel = new GroupModel { GroupName = "GroupName 1", CapacityInAmps = 10 };
        var groupResponse = await client.PostAsJsonAsync("/groups", groupModel);
        var groupValue = groupResponse.Content.ReadFromJsonAsync<GroupModel>().Result;

        //Create a ChargeStation 1 with a Group 1
        var chargeStationModel = new ChargeStationModel { GroupId = 1, ChargeStationName = "ChargeStationName 1", Connectors = { new ConnectorModel { MaximumCurrentInAmps = 2 } } };
        var chargeStationResponse = await client.PostAsJsonAsync($"/groups/{1}/chargeStations", chargeStationModel);
        Assert.Equal(HttpStatusCode.Created, chargeStationResponse.StatusCode);
        Assert.Equal($"/groups/{1}/chargeStations/{1}", chargeStationResponse.Headers.First().Value.First());

        //Create a ChargeStation 2 with a Group 1
        var chargeStationModel1 = new ChargeStationModel { GroupId = 1, ChargeStationName = "ChargeStationName 2", Connectors = { new ConnectorModel { MaximumCurrentInAmps = 2 } } };
        var chargeStationResponse1 = await client.PostAsJsonAsync($"/groups/{1}/chargeStations", chargeStationModel1);
        Assert.Equal(HttpStatusCode.Created, chargeStationResponse1.StatusCode);
        Assert.Equal($"/groups/{1}/chargeStations/{2}", chargeStationResponse1.Headers.First().Value.First());

        //Create a ChargeStation 3 with a Group 1
        var chargeStationModel2 = new ChargeStationModel { GroupId = 1, ChargeStationName = "ChargeStationName 3", Connectors = { new ConnectorModel { MaximumCurrentInAmps = 2 } } };
        var chargeStationResponse2 = await client.PostAsJsonAsync($"/groups/{1}/chargeStations", chargeStationModel2);
        Assert.Equal(HttpStatusCode.Created, chargeStationResponse2.StatusCode);
        Assert.Equal($"/groups/{1}/chargeStations/{3}", chargeStationResponse2.Headers.First().Value.First());
    }

    [Fact]
    public async Task UpdateChargeStationTest()
    {
        await using var application = new ApplicationFactory();
        var client = application.CreateClient();

        //Create a Group
        var groupModel = new GroupModel { GroupName = "GroupName 1", CapacityInAmps = 10 };
        var groupResponse = await client.PostAsJsonAsync("/groups", groupModel);
        var groupValue = groupResponse.Content.ReadFromJsonAsync<GroupModel>().Result;

        //Create a ChargeStation with a Group
        var model = new ChargeStationModel { GroupId = 1, ChargeStationName = "ChargeStationName 1" , Connectors = { new ConnectorModel { MaximumCurrentInAmps = 2 } } };
        var response = await client.PostAsJsonAsync($"/groups/{1}/chargeStations", model);
        var returnValue = response.Content.ReadFromJsonAsync<ChargeStationModel>().Result;

        //Update a ChargeStation
        var updatedModel = new ChargeStationModel { ChargeStationName = "ChargeStationName 2", Connectors = { new ConnectorModel { MaximumCurrentInAmps = 4 } } };
        response = await client.PutAsJsonAsync($"/groups/{1}/chargeStations/{1}", updatedModel);

        returnValue = response.Content.ReadFromJsonAsync<ChargeStationModel>().Result;
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.Equal(updatedModel.ChargeStationName, returnValue.ChargeStationName);
    }

    /// <summary>
    /// The Charge Station can be only in one Group at the same time.
    /// </summary>    
    [Fact]
    public async Task UpdateChargeStationWithAnotherGroupTest()
    {
        await using var application = new ApplicationFactory();
        var client = application.CreateClient();

        //Create a Group 1
        var groupModel = new GroupModel { GroupName = "GroupName 1", CapacityInAmps = 10 };
        var groupResponse = await client.PostAsJsonAsync("/groups", groupModel);
        var groupValue = groupResponse.Content.ReadFromJsonAsync<GroupModel>().Result;

        //Create a Group 2
        var groupModel1 = new GroupModel { GroupName = "GroupName 1", CapacityInAmps = 10 };
        var groupResponse1 = await client.PostAsJsonAsync("/groups", groupModel1);
        var groupValue1 = groupResponse1.Content.ReadFromJsonAsync<GroupModel>().Result;

        //Create a ChargeStation with a Group 1
        var model = new ChargeStationModel { GroupId = 1, ChargeStationName = "ChargeStationName 1" , Connectors = { new ConnectorModel { MaximumCurrentInAmps = 2 } } };
        var response = await client.PostAsJsonAsync($"/groups/{1}/chargeStations", model);
        var returnValue = response.Content.ReadFromJsonAsync<ChargeStationModel>().Result;

        //Update a ChargeStation with a Group 2
        var updatedModel = new ChargeStationModel { ChargeStationName = "ChargeStationName 2", Connectors = { new ConnectorModel { MaximumCurrentInAmps = 5 } } };
        response = await client.PutAsJsonAsync($"/groups/{2}/chargeStations/{1}", updatedModel);

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task DeleteChargeStationTest()
    {
        await using var application = new ApplicationFactory();
        var client = application.CreateClient();

        //Create a Group
        var groupModel = new GroupModel { GroupName = "GroupName 1", CapacityInAmps = 10 };
        var groupResponse = await client.PostAsJsonAsync("/groups", groupModel);
        var groupValue = groupResponse.Content.ReadFromJsonAsync<GroupModel>().Result;

        //Create a ChargeStation with a Group
        var model = new ChargeStationModel { GroupId = 1, ChargeStationName = "ChargeStationName 1" , Connectors = { new ConnectorModel { MaximumCurrentInAmps = 2 } } };
        var response = await client.PostAsJsonAsync($"/groups/{1}/chargeStations", model);
        var returnValue = response.Content.ReadFromJsonAsync<ChargeStationModel>().Result;

        // Delete a ChargeStation
        response = await client.DeleteAsync($"/groups/{1}/chargeStations/{1}");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

    [Fact]
    public async Task DeleteGroupAllChargeStationsRemovedTest()
    {
        await using var application1 = new ApplicationFactory();
        var client = application1.CreateClient();

        //Create a Group
        var groupModel = new GroupModel { GroupName = "GroupName 1", CapacityInAmps = 20 };
        var groupResponse = await client.PostAsJsonAsync("/groups", groupModel);
        var groupValue = groupResponse.Content.ReadFromJsonAsync<GroupModel>().Result;

        //Create a ChargeStation 1 with a Group 1
        var chargeStationModel = new ChargeStationModel { GroupId = 1, ChargeStationName = "ChargeStationName 1" , Connectors = { new ConnectorModel { MaximumCurrentInAmps = 2 } } };
        var chargeStationResponse = await client.PostAsJsonAsync($"/groups/{1}/chargeStations", chargeStationModel);
        Assert.Equal($"/groups/{1}/chargeStations/{1}", chargeStationResponse.Headers.First().Value.First());

        //Create a ChargeStation 2 with a Group 1
        var chargeStationModel1 = new ChargeStationModel { GroupId = 1, ChargeStationName = "ChargeStationName 1" , Connectors = { new ConnectorModel { MaximumCurrentInAmps = 2 } } };
        var chargeStationResponse1 = await client.PostAsJsonAsync($"/groups/{1}/chargeStations", chargeStationModel1);
        Assert.Equal($"/groups/{1}/chargeStations/{2}", chargeStationResponse1.Headers.First().Value.First());

        //Create a Connector 1 with a ChargeStation 1
        var connectorModel = new ConnectorModel { MaximumCurrentInAmps = 5 };
        var connectorResponse = await client.PostAsJsonAsync($"/chargeStations/{1}/connectors", connectorModel);
        var connectorRreturnValue = connectorResponse.Content.ReadFromJsonAsync<ConnectorModel>().Result;

        //Create a Connector 2 with a ChargeStation 1
        var connectorModel1 = new ConnectorModel { MaximumCurrentInAmps = 6 };
        var connectorResponse1 = await client.PostAsJsonAsync($"/chargeStations/{1}/connectors", connectorModel1);
        var connectorRreturnValue1 = connectorResponse1.Content.ReadFromJsonAsync<ConnectorModel>().Result;

        //Create a Connector 3 with a ChargeStation 2
        var connectorModel2 = new ConnectorModel { MaximumCurrentInAmps = 4 };
        var connectorResponse2 = await client.PostAsJsonAsync($"/chargeStations/{2}/connectors", connectorModel2);
        var connectorRreturnValue2 = connectorResponse2.Content.ReadFromJsonAsync<ConnectorModel>().Result;

        //Delete a Group
        var response = await client.DeleteAsync($"/groups/{1}");
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        //All charge stations removed from db
        var chargeStation1 = await client.GetAsync($"/groups/{1}/chargeStations/{1}");
        Assert.Equal(HttpStatusCode.NotFound, chargeStation1.StatusCode);
        var chargeStation2 = await client.GetAsync($"/groups/{1}/chargeStations/{2}");
        Assert.Equal(HttpStatusCode.NotFound, chargeStation1.StatusCode);
    }    

    [Fact]
    public async Task ValidateGroupCapacityInAmps1WhileUpdatingChargeStationTest()
    {
        await using var application1 = new ApplicationFactory();
        var client = application1.CreateClient();

        //Create a Group
        var groupModel = new GroupModel { GroupName = "GroupName 1", CapacityInAmps = 5 };
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

        //Update a ChargeStation 1 with a Group 1 (Connector MaximumCurrentInAmps 1)
        var chargeStationModel2 = new ChargeStationModel { GroupId = 1, ChargeStationName = "ChargeStationName 1", Connectors = { new ConnectorModel { MaximumCurrentInAmps = 2 } } };
        var chargeStationResponse2 = await client.PutAsJsonAsync($"/groups/{1}/chargeStations/{2}", chargeStationModel2);
        Assert.Equal(HttpStatusCode.OK, chargeStationResponse2.StatusCode);       
    }
}
