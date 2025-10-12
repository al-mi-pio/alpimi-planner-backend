using AlpimiAPI.Database;
using AlpimiAPI.Entities.ETeacher;
using AlpimiAPI.Entities.ETeacher.Queries;
using AlpimiAPI.Locales;
using AlpimiAPI.Responses;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;

namespace AlpimiAPI.Entities.EAvailability.Queries
{
    public record GetAllAvailabilityByTeacherQuery(
        Guid TeacherId,
        Guid FilteredId,
        string Role,
        PaginationParams Pagination
    ) : IRequest<(IEnumerable<Availability>?, int)>;

    public class GetAllAvailabilityByTeacherHandler
        : IRequestHandler<GetAllAvailabilityByTeacherQuery, (IEnumerable<Availability>?, int)>
    {
        private readonly IDbService _dbService;
        private readonly IStringLocalizer<Errors> _str;
        private readonly IStringLocalizer<Fields> _strFields;

        public GetAllAvailabilityByTeacherHandler(
            IDbService dbService,
            IStringLocalizer<Errors> str,
            IStringLocalizer<Fields> strFields
        )
        {
            _dbService = dbService;
            _str = str;
            _strFields = strFields;
        }

        public async Task<(IEnumerable<Availability>?, int)> Handle(
            GetAllAvailabilityByTeacherQuery request,
            CancellationToken cancellationToken
        )
        {
            List<ErrorObject> errors = new List<ErrorObject>();
            if (request.Pagination.PerPage < 0)
            {
                errors.Add(
                    new FieldErrorObject("perPage", _str["badParameter", _strFields["PerPage"]])
                );
            }
            if (request.Pagination.Offset < 0)
            {
                errors.Add(new FieldErrorObject("page", _str["badParameter", "Page"]));
            }
            if (
                request.Pagination.SortOrder.ToLower() != "asc"
                && request.Pagination.SortOrder.ToLower() != "desc"
            )
            {
                errors.Add(
                    new FieldErrorObject("sortOrder", _str["badParameter", _strFields["SortOrder"]])
                );
            }
            if (
                request.Pagination.SortBy != "Id"
                && request.Pagination.SortBy != "WeekDay"
                && request.Pagination.SortBy != "Start"
                && request.Pagination.SortBy != "End"
            )
            {
                errors.Add(
                    new FieldErrorObject("sortBy", _str["badParameter", _strFields["SortBy"]])
                );
            }

            if (errors.Count != 0)
            {
                throw new ApiErrorException(errors);
            }

            IEnumerable<Availability>? availability;
            int count;
            switch (request.Role)
            {
                case "Admin":
                    count = await _dbService.Get<int>(
                        @"
                            SELECT 
                            COUNT(*)
                            FROM [Availability] a
                            INNER JOIN [Teacher] t ON t.[Id] = a.[TeacherId]
                            WHERE t.[Id] = @TeacherId;",
                        request
                    );
                    availability = await _dbService.GetAll<Availability>(
                        $@"
                            SELECT
                            a.[Id], a.[WeekDay], a.[Start], a.[End], a.[TeacherId] 
                            FROM [Availability] a
                            INNER JOIN [Teacher] t ON t.[Id] = a.[TeacherId]
                            WHERE t.[Id] = @TeacherId
                            ORDER BY
                            {request.Pagination.SortBy}
                            {request.Pagination.SortOrder}
                            OFFSET
                            {request.Pagination.Offset} ROWS
                            FETCH NEXT
                            {request.Pagination.PerPage} ROWS ONLY;",
                        request
                    );
                    break;
                default:
                    count = await _dbService.Get<int>(
                        @"
                            SELECT 
                            COUNT(*)
                            FROM [Availability] a
                            INNER JOIN [Teacher] t ON t.[Id] = a.[TeacherId]
                            INNER JOIN [Schedule] s ON s.[Id]= t.[ScheduleId]
                            INNER JOIN [ScheduleSettings] ss ON ss.[ScheduleId] = s.[Id]
                            WHERE s.[UserId] = @FilteredId AND t.[Id] = @TeacherId;",
                        request
                    );
                    availability = await _dbService.GetAll<Availability>(
                        $@"
                            SELECT 
                            a.[Id], a.[WeekDay], a.[Start], a.[End], a.[TeacherId]
                            FROM [Availability] a
                            INNER JOIN [Teacher] t ON t.[Id] = a.[TeacherId]
                            INNER JOIN [Schedule] s ON s.[Id]= t.[ScheduleId]
                            INNER JOIN [ScheduleSettings] ss ON ss.[ScheduleId] = s.[Id]
                            WHERE s.[UserId] = @FilteredId AND t.[Id] = @TeacherId 
                            ORDER BY
                            {request.Pagination.SortBy}
                            {request.Pagination.SortOrder}
                            OFFSET
                            {request.Pagination.Offset} ROWS
                            FETCH NEXT
                            {request.Pagination.PerPage} ROWS ONLY;",
                        request
                    );
                    break;
            }

            if (availability != null)
            {
                Dictionary<Guid, Teacher> scheduleSettingsMap = new Dictionary<Guid, Teacher>();
                foreach (var slot in availability)
                {
                    if (!scheduleSettingsMap.ContainsKey(slot.TeacherId))
                    {
                        GetTeacherHandler getTeacherHandler = new GetTeacherHandler(_dbService);
                        GetTeacherQuery getTeacherQuery = new GetTeacherQuery(
                            slot.TeacherId,
                            new Guid(),
                            "Admin"
                        );
                        ActionResult<Teacher?> scheduleSettings = await getTeacherHandler.Handle(
                            getTeacherQuery,
                            cancellationToken
                        );

                        scheduleSettingsMap.Add(slot.TeacherId, scheduleSettings.Value!);
                    }
                    slot.Teacher = scheduleSettingsMap[slot.TeacherId];
                }
            }

            return (availability, count);
        }
    }
}
