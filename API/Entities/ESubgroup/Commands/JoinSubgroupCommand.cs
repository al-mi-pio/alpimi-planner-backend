using AlpimiAPI.Database;
using AlpimiAPI.Entities.EGroup;
using AlpimiAPI.Entities.EGroup.Queries;
using AlpimiAPI.Entities.ESubgroup.Queries;
using AlpimiAPI.Locales;
using AlpimiAPI.Responses;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;

namespace AlpimiAPI.Entities.ESubgroup.Commands
{
    public record JoinSubgroupCommand(
        Guid Id,
        IEnumerable<Guid> SubgroupIds,
        Guid FilteredId,
        string Role
    ) : IRequest<Guid>;

    public class JoinSubgroupHandler : IRequestHandler<JoinSubgroupCommand, Guid>
    {
        private readonly IDbService _dbService;
        private readonly IStringLocalizer<Errors> _str;

        public JoinSubgroupHandler(IDbService dbService, IStringLocalizer<Errors> str)
        {
            _dbService = dbService;
            _str = str;
        }

        public async Task<Guid> Handle(
            JoinSubgroupCommand request,
            CancellationToken cancellationToken
        )
        {
            var duplicates = request
                .SubgroupIds.GroupBy(g => g)
                .Where(g => g.Count() > 1)
                .Select(g => g.Key);

            if (duplicates.Any())
            {
                List<ErrorObject> duplicateErrors = new List<ErrorObject>();
                foreach (var duplicate in duplicates)
                {
                    duplicateErrors.Add(
                        new ErrorObject(_str["duplicateData", "Subgroup", duplicate])
                    );
                }
                throw new ApiErrorException(duplicateErrors);
            }

            List<ErrorObject> errors = new List<ErrorObject>();
            foreach (var subgroupId in request.SubgroupIds)
            {
                GetSubgroupHandler getSubgroupHandler = new GetSubgroupHandler(_dbService);
                GetSubgroupQuery getSubgroupQuery = new GetSubgroupQuery(
                    request.Id,
                    request.FilteredId,
                    request.Role
                );
                ActionResult<Subgroup?> originalSubgroup = await getSubgroupHandler.Handle(
                    getSubgroupQuery,
                    cancellationToken
                );
                if (originalSubgroup.Value == null)
                {
                    errors.Add(new ErrorObject(_str["resourceNotFound", "Subgroup", subgroupId]));
                }
            }
            if (errors.Count != 0)
            {
                throw new ApiErrorException(errors);
            }

            foreach (var subgroupId in request.SubgroupIds)
            {
                await _dbService.Update<Guid?>(
                    $@"
                    UPDATE [Subgroup] 
                    SET
                    [JointSubgroupId] = @Id
                    OUTPUT
                    INSERTED.[Id],
                    WHERE [Id] = '{subgroupId}';",
                    request
                );
            }
            return request.Id;
        }
    }
}
