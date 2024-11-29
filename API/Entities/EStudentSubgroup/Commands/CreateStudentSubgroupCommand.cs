using AlpimiAPI.Database;
using AlpimiAPI.Entities.EStudent;
using AlpimiAPI.Entities.EStudent.Queries;
using AlpimiAPI.Entities.EStudentSubgroup.DTO;
using AlpimiAPI.Entities.ESubgroup;
using AlpimiAPI.Entities.ESubgroup.Queries;
using AlpimiAPI.Locales;
using AlpimiAPI.Responses;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;

namespace AlpimiAPI.Entities.EStudentSubgroup.Commands
{
    public record CreateStudentSubgroupCommand(
        CreateStudentSubgroupDTO dto,
        Guid FilteredId,
        string Role
    ) : IRequest;

    public class CreateStudentSubgroupHandler : IRequestHandler<CreateStudentSubgroupCommand>
    {
        private readonly IDbService _dbService;
        private readonly IStringLocalizer<Errors> _str;

        public CreateStudentSubgroupHandler(IDbService dbService, IStringLocalizer<Errors> str)
        {
            _dbService = dbService;
            _str = str;
        }

        public async Task Handle(
            CreateStudentSubgroupCommand request,
            CancellationToken cancellationToken
        )
        {
            var duplicates = request
                .dto.SubgroupIds.GroupBy(g => g)
                .Where(g => g.Count() > 1)
                .Select(g => g.Key);

            if (duplicates.Any())
            {
                List<ErrorObject> duplicateErrors = new List<ErrorObject>();
                foreach (var duplicate in duplicates)
                {
                    duplicateErrors.Add(
                        new ErrorObject(_str["duplicateData", "SubgroupId", duplicate])
                    );
                }
                throw new ApiErrorException(duplicateErrors);
            }

            List<ErrorObject> errors = new List<ErrorObject>();
            GetStudentHandler getStudentHandler = new GetStudentHandler(_dbService);
            GetStudentQuery getStudentQuery = new GetStudentQuery(
                request.dto.StudentId,
                request.FilteredId,
                request.Role
            );
            ActionResult<Student?> student = await getStudentHandler.Handle(
                getStudentQuery,
                cancellationToken
            );

            if (student.Value == null)
            {
                errors.Add(new ErrorObject(_str["notFound", "Student"]));
            }

            foreach (Guid subgroupId in request.dto.SubgroupIds)
            {
                GetSubgroupHandler getSubgroupHandler = new GetSubgroupHandler(_dbService);
                GetSubgroupQuery getSubgroupQuery = new GetSubgroupQuery(
                    subgroupId,
                    request.FilteredId,
                    request.Role
                );
                ActionResult<Subgroup?> subgroup = await getSubgroupHandler.Handle(
                    getSubgroupQuery,
                    cancellationToken
                );

                if (subgroup.Value == null)
                {
                    errors.Add(new ErrorObject(_str["notFound", $"Subgroup Id = {subgroupId}"]));
                }
                else if (subgroup.Value.GroupId != student.Value!.GroupId)
                {
                    errors.Add(new ErrorObject(_str["notFound", $"Subgroup Id = {subgroupId}"]));
                }
                else
                {
                    var studentSubgroup = await _dbService.Get<StudentSubgroup>(
                        $@"
                            SELECT 
                            [Id]
                            FROM [StudentSubgroup] 
                            WHERE [StudentId] = @StudentId AND [SubgroupId] = '{subgroupId}';",
                        request.dto
                    );

                    if (studentSubgroup != null)
                    {
                        errors.Add(new ErrorObject(_str["studentInGroup", subgroupId]));
                    }
                }
            }
            if (errors.Count != 0)
            {
                throw new ApiErrorException(errors);
            }

            foreach (Guid subgroupId in request.dto.SubgroupIds)
            {
                await _dbService.Post<Guid>(
                    $@"
                        INSERT INTO [StudentSubgroup] 
                        ([Id],[StudentId],[SubgroupId])
                        OUTPUT 
                        INSERTED.Id                    
                        VALUES (
                        '{Guid.NewGuid()}',   
                        @StudentId,
                        '{subgroupId}');",
                    request.dto
                );
            }
        }
    }
}
