using AutoFixture;
using Mapster;
using Moq;
using MYInfo.Application.Features.Metadata.Queries.GetCurrentUserMetadata;
using MYInfo.Application.Features.Metadata.Queries.GetUserMetadataById;
using MYInfo.Domain.Models;
using MYInfo.Domain.Repositories;
using MYInfo.Domain.Services;

namespace MYInfo.ApplicationUnitTests.Metadata;

public class GetCurrentUserMetadataQueryHandlerTest
{
    private readonly Mock<IUserContextService> mockUserContext;
    private readonly Mock<IUserMetadataRepository> mockUserMetadataRepository;
    private readonly GetCurrentUserMetadataQueryHandler _handler;
    private readonly Fixture _fixture;

    public GetCurrentUserMetadataQueryHandlerTest()
    {
        mockUserContext = new();
        mockUserMetadataRepository = new();
        _handler = new GetCurrentUserMetadataQueryHandler(mockUserMetadataRepository.Object, mockUserContext.Object);
        _fixture = new Fixture();
    }

    [Fact]
    public async Task GetCurrentUserMetadataQueryHandler_WithAutorisedUser_ReturnsHisMetadata()
    {
        // Arrange
        var data = _fixture.Create<UserMetaData>();
        var id = Guid.CreateVersion7().ToString();
        mockUserContext.Setup(x => x.GetUserIdentifier()).Returns(id);
        mockUserMetadataRepository.Setup(x => x.GetByUserIdAsync(id)).ReturnsAsync(data);

        // Act
        var result = await _handler.Handle(new GetCurrentUserMetadataQuery(), CancellationToken.None);

        // Assert
        Assert.False(result.IsError);

        var metadata = result.Value.Value;
        var socialLinks = data.SocialLinks.Adapt<ICollection<GetUserMetadataByIdQueryDtoSocialLinks>>();

        Assert.NotNull(metadata);
        Assert.Equal(data.Id.Value, metadata.Id);
        Assert.Equal(data.Description, metadata.Description);
        Assert.Equivalent(data.Emails, metadata.Emails);
        Assert.Equivalent(data.PhoneNumbers, metadata.PhoneNumbers);
        Assert.Equivalent(data.SocialLinks, socialLinks);

        mockUserMetadataRepository.Verify(x => x.GetByUserIdAsync(id), Times.Once);
        mockUserContext.Verify(x => x.GetUserIdentifier(), Times.Once);

    }

    [Fact]
    public async Task GetCurrentUserMetadataQueryHandler_WithAutorisedUserAndNonExistingMetadata_ReturnsNotFound()
    {
        // Arrange
        var id = Guid.CreateVersion7().ToString();
        mockUserContext.Setup(x => x.GetUserIdentifier()).Returns(id);
        mockUserMetadataRepository.Setup(x => x.GetByUserIdAsync(id)).ReturnsAsync((UserMetaData?)null);


        // Act
        var result = await _handler.Handle(new GetCurrentUserMetadataQuery(), CancellationToken.None);

        // Assert
        Assert.True(result.IsError);
        Assert.Equal(ErrorOr.ErrorType.NotFound, result.FirstError.Type);
        Assert.Equal("This metadata doesn't exists!", result.FirstError.Description);
        Assert.Equal("UserMetadata.NotFound", result.FirstError.Code);

        mockUserMetadataRepository.Verify(x => x.GetByUserIdAsync(id), Times.Once);
        mockUserContext.Verify(x => x.GetUserIdentifier(), Times.Once);
    }

    [Fact]
    public async Task GetCurrentUserMetadataQueryHandler_WithUnAutorisedUser_ReturnsHisUnautorisedError()
    {
        // Arrange
        var id = Guid.CreateVersion7().ToString();
        mockUserContext.Setup(x => x.GetUserIdentifier()).Returns("");


        // Act
        var result = await _handler.Handle(new GetCurrentUserMetadataQuery(), CancellationToken.None);

        // Assert
        Assert.True(result.IsError);
        Assert.Equal(ErrorOr.ErrorType.Unauthorized, result.FirstError.Type);
        Assert.Equal("You're not authorized to read this ressource", result.FirstError.Description);
        Assert.Equal("UserMetadata.Unauthorized", result.FirstError.Code);

        mockUserMetadataRepository.Verify(x => x.GetByUserIdAsync(id), Times.Never);
        mockUserContext.Verify(x => x.GetUserIdentifier(), Times.Once);
    }
}
