using AlpimiAPI.Database;
using AlpimiAPI.Entities.EClassroomType.DTO;
using AlpimiAPI.Entities.ESchedule;
using AlpimiAPI.Entities.ESchedule.Queries;
using AlpimiAPI.Locales;
using AlpimiAPI.Responses;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;

namespace AlpimiAPI.Entities.EClassroomType.Commands
{
    public record CreateClassroomTypeCommand(
        Guid Id,
        CreateClassroomTypeDTO dto,
        Guid FilteredId,
        string Role
    ) : IRequest<Guid>;

    public class CreateClassroomTypeHandler : IRequestHandler<CreateClassroomTypeCommand, Guid>
    {
        private readonly IDbService _dbService;
        private readonly IStringLocalizer<Errors> _str;

        public CreateClassroomTypeHandler(IDbService dbService, IStringLocalizer<Errors> str)
        {
            _dbService = dbService;
            _str = str;
        }

        public async Task<Guid> Handle(
            CreateClassroomTypeCommand request,
            CancellationToken cancellationToken
        )
        {
            GetScheduleHandler getScheduleHandler = new GetScheduleHandler(_dbService);
            GetScheduleQuery getScheduleQuery = new GetScheduleQuery(
                request.dto.ScheduleId,
                request.FilteredId,
                request.Role
            );

            ActionResult<Schedule?> schedule = await getScheduleHandler.Handle(
                getScheduleQuery,
                cancellationToken
            );
            if (schedule.Value == null)
            {
                throw new ApiErrorException(
                    [new ErrorObject(_str["resourceNotFound", "Schedule", request.dto.ScheduleId])]
                );
            }

            var classroomTypeName = await _dbService.Get<ClassroomType>(
                @"
                    SELECT 
                    [Id]
                    FROM [ClassroomType] 
                    WHERE [Name] = @Name AND [ScheduleId] = @ScheduleId;",
                request.dto
            );

            if (classroomTypeName != null)
            {
                throw new ApiErrorException(
                    [new ErrorObject(_str["alreadyExists", "ClassroomType", request.dto.Name])]
                );
            }

            var insertedId = await _dbService.Post<Guid>(
                $@"
                    INSERT INTO [ClassroomType] 
                    ([Id],[Name],[ScheduleId])
                    OUTPUT 
                    INSERTED.Id                    
                    VALUES (
                    '{request.Id}',   
                    @Name,
                    @ScheduleId);",
                request.dto
            );

            return insertedId;
        }
    }
}
