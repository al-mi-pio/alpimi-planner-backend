using AlpimiAPI.Database;
using AlpimiAPI.Entities.ELesson;
using AlpimiAPI.Entities.ELessonBlock;
using AlpimiAPI.Entities.ELessonBlock.Queries;
using AlpimiAPI.Locales;
using AlpimiAPI.Responses;
using Azure.Core;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;
using Moq;

namespace AlpimiAPI.Utilities
{
    public static class CurrentLessonHours
    {
        public static async Task Update(
            IDbService _dbService,
            Guid lessonId,
            CancellationToken cancellationToken
        )
        {
            Mock<IStringLocalizer<Errors>> _str = new();
            int amountOfHoursToInsert = 0;

            GetAllLessonBlocksHandler getAllLessonBlocksHandler = new GetAllLessonBlocksHandler(
                _dbService,
                _str.Object
            );
            GetAllLessonBlocksQuery getAllLessonBlocksQuery = new GetAllLessonBlocksQuery(
                lessonId,
                null,
                null,
                new Guid(),
                "Admin",
                new PaginationParams(int.MaxValue, 0, "LessonDate", "ASC")
            );
            ActionResult<(IEnumerable<LessonBlock>?, int)> lessonBlocks =
                await getAllLessonBlocksHandler.Handle(getAllLessonBlocksQuery, cancellationToken);

            if (lessonBlocks.Value.Item2 > 0)
            {
                foreach (var lessonBlock in lessonBlocks.Value.Item1!)
                {
                    amountOfHoursToInsert += lessonBlock.LessonEnd - lessonBlock.LessonStart + 1;
                }
            }

            await _dbService.Update<Lesson?>(
                $@"
                    UPDATE [Lesson] 
                    SET
                    [CurrentHours] = {amountOfHoursToInsert}
                    OUTPUT
                    INSERTED.[Id]
                    WHERE [Id] = '{lessonId}'; ",
                ""
            );
        }
    }
}
