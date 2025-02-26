using AutoFixture;
using ErrorOr;
using Moq;
using MYInfo.Application.Features.Metadata.Commands.CreateMetadata;
using MYInfo.Domain.Models;
using MYInfo.Domain.Repositories;
using MYInfo.Domain.Services;
using MYInfo.Domain.ValueObjects;

namespace MYInfo.ApplicationUnitTests.Metadata;

public class CreateMetadataCommandHandlerTest
{
    private readonly Mock<IUserContextService> mockUserContext = new();
    private readonly Mock<IUserMetadataRepository> mockRepository = new();
    private readonly Fixture fixture = new();


    [Fact]
    public async Task CreateMetadataCommandHandler_WithUnautheticatedUser_ReturnUnautorisedResult()
    {
        // Arrange
        var dto = fixture.Create<CreateMetadaDataDto>();
        mockUserContext.Setup(x => x.GetUserIdentifier()).Returns("");
        var handler = new CreateMetadataHandler(mockRepository.Object, mockUserContext.Object);

        // Act
        var result = await handler.Handle(new CreateMetadataCommand(dto), CancellationToken.None);

        // Assert
        Assert.True(result.IsError);
        Assert.Equal(ErrorType.Unauthorized.ToString(), result.FirstError.Code);
    }

    [Fact]
    public async Task CreateMetadataCommandHandler_WithAuthenticatedUserAndNonExistingMeta_CreateMetadataAndReturnId()
    {
        // Arrange
        var dto = fixture.Create<CreateMetadaDataDto>();
        var existing = new UserMetaData()
        {
            CreatedBy = "test",
            Id = UserMetaDataId.Of(Guid.NewGuid()),
        };
        mockUserContext
            .Setup(x => x.GetUserIdentifier())
            .Returns("test");

        mockRepository.Setup(x => x.GetByUserIdAsync(It.IsAny<string>()))
            .ReturnsAsync(existing);

        mockRepository.Setup(x => x.UpdateAsync(It.IsAny<UserMetaData>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);
        var handler = new CreateMetadataHandler(mockRepository.Object, mockUserContext.Object);

        // Act
        var result = await handler.Handle(new CreateMetadataCommand(dto), CancellationToken.None);

        // Assert
        mockRepository.Verify(x => x.UpdateAsync(It.IsAny<UserMetaData>(), It.IsAny<CancellationToken>()), Times.Once);
        mockRepository.Verify(x => x.AddAsync(It.IsAny<UserMetaData>(), It.IsAny<CancellationToken>()), Times.Never);
        Assert.False(result.IsError);
        Assert.NotNull(result.Value);
        Assert.Equal(existing.Id.Value, result.Value.Id);
    }

    [Fact]
    public async Task CreateMetadataCommandHandler_WithAuthenticatedUserAndExistingMeta_UpdateMetadataAndReturnId()
    {
        // Arrange
        var dto = fixture.Create<CreateMetadaDataDto>();
        mockUserContext
            .Setup(x => x.GetUserIdentifier())
            .Returns("test");

        mockRepository.Setup(x => x.GetByUserIdAsync(It.IsAny<string>()))
            .ReturnsAsync((UserMetaData?)null);

        mockRepository.Setup(x => x.AddAsync(It.IsAny<UserMetaData>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);
        var handler = new CreateMetadataHandler(mockRepository.Object, mockUserContext.Object);

        // Act
        var result = await handler.Handle(new CreateMetadataCommand(dto), CancellationToken.None);

        // Assert
        mockRepository.Verify(x => x.UpdateAsync(It.IsAny<UserMetaData>(), It.IsAny<CancellationToken>()), Times.Never);
        mockRepository.Verify(x => x.AddAsync(It.IsAny<UserMetaData>(), It.IsAny<CancellationToken>()), Times.Once);

        Assert.False(result.IsError);
        Assert.NotNull(result.Value);
        Assert.NotEqual(Guid.Empty, result.Value.Id);

    }
}
