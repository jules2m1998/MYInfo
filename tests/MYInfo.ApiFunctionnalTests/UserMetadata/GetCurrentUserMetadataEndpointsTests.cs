using Mapster;
using Microsoft.EntityFrameworkCore;
using MYInfo.API.Endpoints.UserMetadata;
using MYInfo.ApiFunctionnalTests.Abstractions;
using MYInfo.ApiFunctionnalTests.TestData;
using MYInfo.Application.Features.Metadata.Queries.GetCurrentUserMetadata;
using MYInfo.Domain.ValueObjects;
using System.Net;
using System.Net.Http.Json;
using Metadata = MYInfo.Domain.Models.UserMetaData;

namespace MYInfo.ApiFunctionnalTests.UserMetadata;

public class GetCurrentUserMetadataEndpointsTests(IntegrationTestWebAppFactory factory)
    : BaseIntegrationTest(factory)
{

    private DbSet<Metadata> UserMetadata => DbContext.Set<Metadata>();

    [Fact]
    public async Task GetCurrentUser_UnauthorizedUser_Returns401()
    {
        // Arrange
        await ClearTableAsync<Metadata>();

        // Act
#pragma warning disable CA2234 // Passer des objets d'URI système à la place de chaînes
        var response = await Client.GetAsync("api/v1/userMetadata/current");
#pragma warning restore CA2234 // Passer des objets d'URI système à la place de chaînes

        // Assert
        Assert.NotNull(response);
        Assert.False(response.IsSuccessStatusCode);
        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);

    }

    [Fact]
    public async Task GetCurrentUser_AuthorizedUserButNonExistingMetadata_Returns404()
    {
        // Arrange
        await ClearTableAsync<Metadata>();
        ApplyAutorisation();

        // Act
#pragma warning disable CA2234 // Passer des objets d'URI système à la place de chaînes
        var response = await Client.GetAsync("api/v1/userMetadata/current");
#pragma warning restore CA2234 // Passer des objets d'URI système à la place de chaînes

        // Assert
        Assert.NotNull(response);
        Assert.False(response.IsSuccessStatusCode);
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Theory]
    [ClassData(typeof(GetUserMetadataByIdTestData))]
    public async Task GetCurrentUser_AuthorizedUserButExistingMetadata_ReturnsMetadata(GetCurrentUserMetadataDto data)
    {
        // Arrange
        await ClearTableAsync<Metadata>();
        ApplyAutorisation();

        var userId = Configuration["JwtTest:UserId"];
        var userMetadata = new Metadata
        {
            Id = UserMetaDataId.Of(data.Id),
            Description = data.Description,
            Emails = [.. data.Emails],
            PhoneNumbers = [.. data.PhoneNumbers],
            SocialLinks = data.SocialLinks.Adapt<List<SocialLink>>(),
            CreatedBy = userId
        };

        UserMetadata.Add(userMetadata);
#pragma warning disable CA1849 // Appeler des méthodes async dans une méthode async
#pragma warning disable S6966 // Awaitable method should be used
        DbContext.SaveChanges();
#pragma warning restore S6966 // Awaitable method should be used
#pragma warning restore CA1849 // Appeler des méthodes async dans une méthode async

        // Act
#pragma warning disable CA2234 // Passer des objets d'URI système à la place de chaînes
        var response = await Client.GetAsync("api/v1/userMetadata/current");
#pragma warning restore CA2234 // Passer des objets d'URI système à la place de chaînes

        // Assert
        Assert.NotNull(response);
        response.EnsureSuccessStatusCode();

        var result = await response.Content.ReadFromJsonAsync<GetUserMetadataByIdResponse>();
        Assert.NotNull(result);
        Assert.Equal(data.Id, result.Id);
        Assert.Equal(data.Description, result.Description);
        Assert.Equivalent(data.Emails, result.Emails);
        Assert.Equivalent(data.PhoneNumbers, result.PhoneNumbers);
        Assert.Equivalent(data.SocialLinks, result.SocialLinks);
    }
}
