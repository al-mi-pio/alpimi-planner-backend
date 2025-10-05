using System.Text.Json;
using AlpimiAPI.Database;
using AlpimiAPI.Entities.EDayOff.Commands;
using AlpimiAPI.Entities.EHistory.DTO;
using AlpimiAPI.Entities.ELessonBlock.DTO;
using AlpimiAPI.Entities.ELessonBlock.Queries;
using AlpimiAPI.Responses;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;
using Moq;

namespace AlpimiAPI.Entities.ELessonBlock.Commands
{
    public record DeleteLessonBlockCommand(Guid Id, Guid FilteredId, string Role) : IRequest;

    public class DeleteLessonBlockHandler : IRequestHandler<DeleteLessonBlockCommand>
    {
        private readonly IDbService _dbService;

        public DeleteLessonBlockHandler(IDbService dbService)
        {
            _dbService = dbService;
        }

        public async Task Handle(
            DeleteLessonBlockCommand request,
            CancellationToken cancellationToken
        )
        {
            var lessonId = await _dbService.Get<Guid?>(
                $@"
                    SELECT DISTINCT
                    [LessonId] 
                    FROM [LessonBlock] 
                    WHERE [Id] = '{request.Id}' OR [ClusterId] = '{request.Id}';",
                ""
            );

            LessonBlock? lessonBlock;
            GetLessonBlockHandler getLessonBlockHandler = new GetLessonBlockHandler(_dbService);
            GetLessonBlockQuery getLessonBlockQuery = new GetLessonBlockQuery(
                request.Id,
                request.FilteredId,
                request.Role
            );
            ActionResult<LessonBlock?> singleLessonblock = await getLessonBlockHandler.Handle(
                getLessonBlockQuery,
                cancellationToken
            );
            var emptyLocalizer = new Mock<IStringLocalizer<Locales.Errors>>();
            var emptyFieldLocalizer = new Mock<IStringLocalizer<Locales.Fields>>();
            GetAllLessonBlocksHandler getFirstLessonBlocksHandler = new GetAllLessonBlocksHandler(
                _dbService,
                emptyLocalizer.Object,
                emptyFieldLocalizer.Object
            );
            GetAllLessonBlocksQuery getFirstTwoLessonBlocksQuery = new GetAllLessonBlocksQuery(
                request.Id,
                null,
                null,
                request.FilteredId,
                request.Role,
                new PaginationParams(2, 0, "LessonDate", "ASC")
            );
            ActionResult<(IEnumerable<LessonBlock>?, int)> firstTwoLessonBlocks =
                await getFirstLessonBlocksHandler.Handle(
                    getFirstTwoLessonBlocksQuery,
                    cancellationToken
                );
            if (singleLessonblock.Value != null || firstTwoLessonBlocks.Value.Item2 != 0)
            {
                lessonBlock =
                    singleLessonblock.Value != null
                        ? singleLessonblock.Value
                        : firstTwoLessonBlocks.Value.Item1!.First();

                CreateLessonBlockDTO reversaleDTO = new CreateLessonBlockDTO
                {
                    LessonDate = lessonBlock.LessonDate,
                    LessonStart = lessonBlock.LessonStart,
                    LessonEnd = lessonBlock.LessonEnd,
                    LessonId = lessonBlock.LessonId,
                    ClassroomId = lessonBlock.ClassroomId,
                    WeekInterval =
                        firstTwoLessonBlocks.Value.Item2 >= 2
                            ? (
                                firstTwoLessonBlocks.Value.Item1!.Last().LessonDate.DayNumber
                                - firstTwoLessonBlocks.Value.Item1!.First().LessonDate.DayNumber
                            ) / 7
                            : null
                };
                AddToHistoryHandler addToHistoryHandler = new AddToHistoryHandler(_dbService);
                AddToHistoryDTO addToHistoryDTO = new AddToHistoryDTO
                {
                    Id = Guid.NewGuid(),
                    Timestamp = DateTime.Now,
                    AffectedEntityId = request.Id,
                    AffectedEntity =
                        singleLessonblock.Value != null ? "LessonBlock" : "LessonBlockCluster",
                    Command = "Delete",
                    ReversaleDTO = JsonSerializer.Serialize(reversaleDTO),
                    CollisionChecked = false,
                    ScheduleId = lessonBlock.Lesson.LessonType.ScheduleId,
                };
                AddToHistoryCommand addToHistoryCommand = new AddToHistoryCommand(addToHistoryDTO);
                await addToHistoryHandler.Handle(addToHistoryCommand, cancellationToken);
            }

            switch (request.Role)
            {
                case "Admin":
                    await _dbService.Delete(
                        @"
                            DELETE [LessonBlock] 
                            WHERE [Id] = @Id OR [ClusterId] = @Id;",
                        request
                    );
                    break;
                default:
                    await _dbService.Delete(
                        @"
                            DELETE lb
                            FROM [LessonBlock] lb
                            INNER JOIN [Lesson] l ON l.[Id] = lb.[LessonId]
                            INNER JOIN [LessonType] lt ON lt.[Id] = l.[LessonTypeId]
                            INNER JOIN [Schedule] s ON s.[Id] = lt.[ScheduleId]
                            WHERE s.[UserId] = @FilteredId AND (lb.[Id] = @Id OR lb.[ClusterId] = @Id);",
                        request
                    );
                    break;
            }
            if (lessonId != null)
            {
                await Utilities.CurrentLessonHours.Update(
                    _dbService,
                    lessonId.Value,
                    cancellationToken
                );
            }
        }
    }
}
