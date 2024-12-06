using AlpimiAPI.Database;
using AlpimiAPI.Entities.ELesson.DTO;
using AlpimiAPI.Entities.ELessonType;
using AlpimiAPI.Entities.ELessonType.Queries;
using AlpimiAPI.Entities.ESubgroup;
using AlpimiAPI.Entities.ESubgroup.Queries;
using AlpimiAPI.Locales;
using AlpimiAPI.Responses;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;

namespace AlpimiAPI.Entities.ELesson.Commands
{
    public record CreateLessonCommand(Guid Id, CreateLessonDTO dto, Guid FilteredId, string Role)
        : IRequest<Guid>;

    public class CreateLessonHandler : IRequestHandler<CreateLessonCommand, Guid>
    {
        private readonly IDbService _dbService;
        private readonly IStringLocalizer<Errors> _str;

        public CreateLessonHandler(IDbService dbService, IStringLocalizer<Errors> str)
        {
            _dbService = dbService;
            _str = str;
        }

        public async Task<Guid> Handle(
            CreateLessonCommand request,
            CancellationToken cancellationToken
        )
        {
            if (request.dto.AmountOfHours < 1)
            {
                throw new ApiErrorException(
                    [new ErrorObject(_str["badParameter", "AmountOfHours"])]
                );
            }

            GetLessonTypeHandler getLessonTypeHandler = new GetLessonTypeHandler(_dbService);
            GetLessonTypeQuery getLessonTypeQuery = new GetLessonTypeQuery(
                request.dto.LessonTypeId,
                request.FilteredId,
                request.Role
            );
            ActionResult<LessonType?> lessonType = await getLessonTypeHandler.Handle(
                getLessonTypeQuery,
                cancellationToken
            );

            if (lessonType.Value == null)
            {
                throw new ApiErrorException(
                    [
                        new ErrorObject(
                            _str["resourceNotFound", "LessonType", request.dto.LessonTypeId]
                        )
                    ]
                );
            }

            GetSubgroupHandler getSubgroupHandler = new GetSubgroupHandler(_dbService);
            GetSubgroupQuery getSubgroupQuery = new GetSubgroupQuery(
                request.dto.SubgroupId,
                request.FilteredId,
                request.Role
            );
            ActionResult<Subgroup?> subgroup = await getSubgroupHandler.Handle(
                getSubgroupQuery,
                cancellationToken
            );

            if (subgroup.Value == null)
            {
                throw new ApiErrorException(
                    [new ErrorObject(_str["resourceNotFound", "Subgroup", request.dto.SubgroupId])]
                );
            }

            var lessonName = await _dbService.GetAll<Lesson>(
                @"
                    SELECT 
                    [Id]
                    FROM [Lesson] 
                    WHERE [Name] = @Name AND [SubgroupId] = @SubgroupId;",
                request.dto
            );

            if (lessonName!.Any())
            {
                throw new ApiErrorException(
                    [new ErrorObject(_str["alreadyExists", "Lesson", request.dto.Name])]
                );
            }

            var insertedId = await _dbService.Post<Guid>(
                $@"
                    INSERT INTO [Lesson] 
                    ([Id],[Name],[AmountOfHours],[LessonTypeId],[SubgroupId])
                    OUTPUT 
                    INSERTED.Id                    
                    VALUES (
                    '{request.Id}',   
                    @Name,
                    @AmountOfHours,
                    @LessonTypeId,
                    @SubgroupId);",
                request.dto
            );

            return insertedId;
        }
    }
}
