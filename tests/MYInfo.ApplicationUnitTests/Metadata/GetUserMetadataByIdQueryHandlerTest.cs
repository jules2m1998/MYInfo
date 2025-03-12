using AutoFixture;
using Mapster;
using Moq;
using MYInfo.Application.Features.Metadata.Queries.GetUserMetadataById;
using MYInfo.Domain.Models;
using MYInfo.Domain.Repositories;
using MYInfo.Domain.ValueObjects;

namespace MYInfo.ApplicationUnitTests.Metadata;

public class GetUserMetadataByIdQueryHandlerTest
{
    private readonly Mock<IUserMetadataRepository> _mockRepository;
    private readonly Fixture _fixture;
    private readonly GetUserMetadataByIdHandler _handler;

    public GetUserMetadataByIdQueryHandlerTest()
    {
        _mockRepository = new Mock<IUserMetadataRepository>();
        _fixture = new Fixture();
        _handler = new GetUserMetadataByIdHandler(_mockRepository.Object);
    }

    [Fact]
    public async Task Handle_WithExistingMetadata_ReturnsMetadata()
    {
        // Arrange
        var data = _fixture.Create<UserMetaData>();
        _mockRepository.Setup(x => x.GetByIdAsync(It.IsAny<UserMetaDataId>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(data);

        // Act
        var result = await _handler.Handle(new GetUserMetadataByIdQuery(data.Id.Value), CancellationToken.None);

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

        _mockRepository.Verify(x => x.GetByIdAsync(It.IsAny<UserMetaDataId>(), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Handle_WithNonExistingMetadata_ReturnsNotFoundError()
    {
        // Arrange
        var nonExistentId = UserMetaDataId.Of(Guid.NewGuid());
        _mockRepository.Setup(x => x.GetByIdAsync(It.IsAny<UserMetaDataId>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((UserMetaData?)null);

        // Act
        var result = await _handler.Handle(new GetUserMetadataByIdQuery(nonExistentId.Value), CancellationToken.None);

        // Assert
        Assert.True(result.IsError);
        Assert.Equal(ErrorOr.ErrorType.NotFound, result.FirstError.Type);
        Assert.Equal("This metadata doesn't exists!", result.FirstError.Description);
        Assert.Equal("UserMetadata.NotFound", result.FirstError.Code);

        _mockRepository.Verify(x => x.GetByIdAsync(It.IsAny<UserMetaDataId>(), It.IsAny<CancellationToken>()), Times.Once);
    }
}