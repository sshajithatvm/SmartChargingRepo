using Microsoft.AspNetCore.Http;
using System.Net;
using System.Net.Http.Json;
using Xunit;


public class GroupApiTests
{
    [Fact]
    public async Task CreateGroupTest()
    {
        await using var application = new ApplicationFactory();
        var client = application.CreateClient();

        //Create a Group 
        var model = new GroupModel { GroupName = "GroupName 1", CapacityInAmps = 10 };
        var response = await client.PostAsJsonAsync("/groups", model);

        var returnValue = response.Content.ReadFromJsonAsync<GroupModel>().Result;
        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
        Assert.Equal($"/groups/{1}", response.Headers.First().Value.First());
    }

    [Fact]
    public async Task UpdateGroupTest()
    {
        await using var application1 = new ApplicationFactory();
        var client = application1.CreateClient();

        //Create a Group 
        var model = new GroupModel { GroupName = "GroupName 1", CapacityInAmps = 10 };
        var response = await client.PostAsJsonAsync("/groups", model);
        var returnValue = response.Content.ReadFromJsonAsync<GroupModel>().Result;

        //Update a Group 
        var updatedModel = new GroupModel { GroupName = "GroupName 1 Updated", CapacityInAmps = 20 };
        response = await client.PutAsJsonAsync($"/groups/{1}", updatedModel);

        returnValue = response.Content.ReadFromJsonAsync<GroupModel>().Result;
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.Equal(updatedModel.GroupName, returnValue.GroupName);
        Assert.Equal(updatedModel.CapacityInAmps, returnValue.CapacityInAmps);
    }

    [Fact]
    public async Task DeleteGroupTest()
    {

        await using var application1 = new ApplicationFactory();
        var client = application1.CreateClient();

        //Create a Group 
        var model = new GroupModel { GroupName = "GroupName 1", CapacityInAmps = 10 };
        var response = await client.PostAsJsonAsync("/groups", model);
        var returnValue = response.Content.ReadFromJsonAsync<GroupModel>().Result;

        //Delete a Group
        response = await client.DeleteAsync($"/groups/{1}");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }


    [Fact]
    public async Task ValidateGroupCapacityInAmpsEqualZeroTest()
    {
        await using var application1 = new ApplicationFactory();
        var client = application1.CreateClient();

        //Create a Group 
        var groupModel = new GroupModel { GroupName = "GroupName 1", CapacityInAmps = 0 };
        var groupResponse = await client.PostAsJsonAsync("/groups", groupModel);

        Assert.Equal(HttpStatusCode.BadRequest, groupResponse.StatusCode);
    }

}

