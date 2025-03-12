using FluentValidation.Results;
using Mapster;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MYInfo.API.Endpoints.UserMetadata;
using MYInfo.ApiFunctionnalTests.Abstractions;
using MYInfo.ApiFunctionnalTests.TestData;
using MYInfo.Application.Features.Metadata.Commands.CreateMetadata;
using MYInfo.Domain.ValueObjects;
using Newtonsoft.Json;
using System.Net;
using System.Net.Http.Json;
using Metadata = MYInfo.Domain.Models.UserMetaData;

namespace MYInfo.ApiFunctionnalTests.UserMetadata;

public class CreateMetadataEndpointsTests(IntegrationTestWebAppFactory factory)
    : BaseIntegrationTest(factory)
{
    private DbSet<Metadata> _userMetadata => DbContext.Set<Metadata>();
    [Theory]
    [ClassData(typeof(CreateMetadataTestData))]
    public async Task CreateMetadata_ValidDataAndUnauthorizedUser_Returns401(CreateMetadataDataDto dto)
    {
        // Arrange
        await ClearTableAsync<Metadata>();
        var request = new CreateMetadataRequest(dto);

        // Act
        var response = await Client.PostAsJsonAsync("api/v1/userMetadata", request);

        // Assert
        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Theory]
    [ClassData(typeof(CreateMetadataTestData))]
    public async Task CreateMetadata_ValidData_ReturnsCreated(CreateMetadataDataDto dto)
    {
        // Arrange
        await ClearTableAsync<Metadata>();
        ApplyAutorisation();
        var request = new CreateMetadataRequest(dto);

        // Act
        var response = await Client.PostAsJsonAsync("api/v1/userMetadata", request);

        // Transform
        var data = await response.Content.ReadFromJsonAsync<CreateMetadataResponse>();

        // Assert
        response.EnsureSuccessStatusCode();
        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
        Assert.NotNull(data);
        Assert.NotEqual(Guid.Empty, data.Id);
    }

    [Theory]
    [ClassData(typeof(CreateMetadataInvalidTestData))]
    public async Task CreateMetadata_InvalidData_Returns400(CreateMetadataDataDto dto, string[] messages)
    {
        // Arrange
        await ClearTableAsync<Metadata>();
        ApplyAutorisation();
        var request = new CreateMetadataRequest(dto);

        // Act
        var response = await Client.PostAsJsonAsync("api/v1/userMetadata", request);

        // Transform
        var result = await response.Content.ReadFromJsonAsync<ProblemDetails>();
        var errors = result!.Extensions["ValidationErrors"]!.ToString();

        var validationErrors = JsonConvert.DeserializeObject<List<ValidationFailure>>(errors!);
        var errorMsgs = validationErrors!.Select(x => x.ErrorMessage).ToArray();

        // Assert
        Assert.False(response.IsSuccessStatusCode);
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        Assert.NotNull(errors);
        Assert.Equivalent(messages, errorMsgs);
    }

    [Theory]
    [ClassData(typeof(CreateMetadataTestData))]
    public async Task CreateMetadata_WithExinstingMetadata_JustUpdate(CreateMetadataDataDto dto)
    {
        // Arrange
        await ClearTableAsync<Metadata>();
        ApplyAutorisation();
        var userId = Configuration["JwtTest:UserId"];
        var userMetadata = new Metadata
        {
            Id = UserMetaDataId.Of(Guid.CreateVersion7()),
            Description = dto.Description,
            Emails = dto.Emails,
            PhoneNumbers = dto.PhoneNumbers,
            SocialLinks = dto.SocialLinks.Adapt<List<SocialLink>>(),
            CreatedBy = userId
        };
        _userMetadata.Add(userMetadata);
#pragma warning disable CA1849 // Appeler des méthodes async dans une méthode async
#pragma warning disable S6966 // Awaitable method should be used
        DbContext.SaveChanges();
#pragma warning restore S6966 // Awaitable method should be used
#pragma warning restore CA1849 // Appeler des méthodes async dans une méthode async


        // Act
        var request = new CreateMetadataRequest(dto);
        var response = await Client.PostAsJsonAsync("api/v1/userMetadata", request);

        // Transform
        var data = await response.Content.ReadFromJsonAsync<CreateMetadataResponse>();
        var metadata = await _userMetadata.AsNoTracking().Where(x => x.CreatedBy == userId).SingleAsync();

        // Assert
        response.EnsureSuccessStatusCode();
        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
        Assert.NotNull(data);
        Assert.NotEqual(Guid.Empty, data.Id);

        Assert.NotNull(metadata);
        Assert.Equal(dto.Description, metadata.Description);
        Assert.Equivalent(dto.Emails, metadata.Emails);
        Assert.Equivalent(dto.PhoneNumbers, metadata.PhoneNumbers);
        Assert.Equivalent(dto.SocialLinks.Adapt<List<SocialLink>>(), metadata.SocialLinks);

    }

}
