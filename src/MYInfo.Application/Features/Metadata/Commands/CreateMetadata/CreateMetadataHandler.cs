using ErrorOr;
using Mapster;
using MYInfo.Application.CQRS;
using MYInfo.Domain.Models;
using MYInfo.Domain.Repositories;
using MYInfo.Domain.Services;
using MYInfo.Domain.ValueObjects;

namespace MYInfo.Application.Features.Metadata.Commands.CreateMetadata;

public class CreateMetadataHandler(IUserMetadataRepository repository, IUserContextService userContext)
    : ICommandHandler<CreateMetadataCommand, CreateMetadataCommandResult>
{
    public async Task<ErrorOr<CreateMetadataCommandResult>> Handle(CreateMetadataCommand request, CancellationToken cancellationToken)
    {
        var userId = userContext.GetUserIdentifier();
        if (string.IsNullOrWhiteSpace(userId))
        {
            return Error.Unauthorized("Unauthorized", "User is not authenticated.");
        }
        var exisiting = await repository.GetByUserIdAsync(userId);

        if (exisiting == null)
        {
            var data = request.Dto.Adapt<UserMetaData>();
            data.Id = UserMetaDataId.Of(Guid.CreateVersion7());
            await repository.AddAsync(data, cancellationToken);
            return new CreateMetadataCommandResult(data.Id.Value);
        }

        request.Dto.Adapt(exisiting);
        await repository.UpdateAsync(exisiting, cancellationToken);

        return new CreateMetadataCommandResult(exisiting.Id.Value);
    }
}
