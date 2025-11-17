using System.Xml.Linq;
using AlpimiAPI.Database;
using AlpimiAPI.Entities.EAvailability.Commands;
using AlpimiAPI.Entities.EAvailability.DTO;
using AlpimiAPI.Entities.EClassroom.Commands;
using AlpimiAPI.Entities.EClassroom.DTO;
using AlpimiAPI.Entities.EClassroomType.Commands;
using AlpimiAPI.Entities.EClassroomType.DTO;
using AlpimiAPI.Entities.EData.DTO;
using AlpimiAPI.Entities.EDayOff.Commands;
using AlpimiAPI.Entities.EDayOff.DTO;
using AlpimiAPI.Entities.EGroup.Commands;
using AlpimiAPI.Entities.EGroup.DTO;
using AlpimiAPI.Entities.ELesson.Commands;
using AlpimiAPI.Entities.ELesson.DTO;
using AlpimiAPI.Entities.ELessonPeriod.Commands;
using AlpimiAPI.Entities.ELessonPeriod.DTO;
using AlpimiAPI.Entities.ELessonType.Commands;
using AlpimiAPI.Entities.ELessonType.DTO;
using AlpimiAPI.Entities.ESchedule;
using AlpimiAPI.Entities.ESchedule.Queries;
using AlpimiAPI.Entities.EStudent.Commands;
using AlpimiAPI.Entities.EStudent.DTO;
using AlpimiAPI.Entities.ESubgroup.Commands;
using AlpimiAPI.Entities.ESubgroup.DTO;
using AlpimiAPI.Entities.ETeacher.Commands;
using AlpimiAPI.Entities.ETeacher.DTO;
using AlpimiAPI.Locales;
using AlpimiAPI.Responses;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;
using Sprache;

namespace AlpimiAPI.Entities.EData.Commands
{
    public record ImportDataCommand(ImportDataDTO dto, Guid FilteredId, string Role)
        : IRequest<ImportDataOutputDTO>;

    public class ImportDataHandler : IRequestHandler<ImportDataCommand, ImportDataOutputDTO>
    {
        private readonly IDbService _dbService;
        private readonly IStringLocalizer<Errors> _str;
        private readonly IStringLocalizer<Fields> _strFields;
        private readonly IStringLocalizer<Data> _strData;

        public ImportDataHandler(
            IDbService dbService,
            IStringLocalizer<Errors> str,
            IStringLocalizer<Fields> strFields,
            IStringLocalizer<Data> strData
        )
        {
            _dbService = dbService;
            _str = str;
            _strFields = strFields;
            _strData = strData;
        }

        public async Task<ImportDataOutputDTO> Handle(
            ImportDataCommand request,
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
                    [
                        new ErrorObject(
                            _str["resourceNotFound", _strFields["Schedule"], request.dto.ScheduleId]
                        )
                    ]
                );
            }

            XNamespace ss = "urn:schemas-microsoft-com:office:spreadsheet";
            var doc = XDocument.Parse(request.dto.Payload);

            Dictionary<string, Guid> teacherIds = new();
            Dictionary<string, Guid> classroomTypeIds = new();
            Dictionary<string, Guid> classroomIds = new();
            Dictionary<string, Guid> lessonTypeIds = new();
            Dictionary<string, Guid> groupIds = new();
            Dictionary<string, Guid> subgroupIds = new();

            int succesfulItems = 0;
            Dictionary<string, List<UnsuccsefulItem>> unsuccessfulItems = new();

            var worksheet = doc.Descendants(ss + "Worksheet")
                .FirstOrDefault(w => w.Attribute(ss + "Name")?.Value == _strData["LessonPeriods"]);

            if (worksheet != null)
            {
                var handler = new CreateLessonPeriodHandler(_dbService, _str, _strFields);
                var rows = worksheet.Descendants(ss + "Row").Skip(1);
                int rowIndex = 2;
                List<UnsuccsefulItem> failures = new();
                foreach (var row in rows)
                {
                    try
                    {
                        var cells = row.Descendants(ss + "Data").Select(d => d.Value).ToList();

                        if (cells.ElementAtOrDefault(0) == null)
                            continue;

                        var dto = new CreateLessonPeriodDTO
                        {
                            Start = TimeOnly.Parse(cells.ElementAtOrDefault(0)!),
                            ScheduleId = request.dto.ScheduleId
                        };

                        var command = new CreateLessonPeriodCommand(
                            Guid.NewGuid(),
                            dto,
                            request.FilteredId,
                            request.Role
                        );

                        await handler.Handle(command, cancellationToken);
                        succesfulItems++;
                    }
                    catch (ApiErrorException error)
                    {
                        failures.Add(
                            new UnsuccsefulItem { Reason = error.errors, RowIndex = rowIndex }
                        );
                    }
                    catch (Exception)
                    {
                        failures.Add(
                            new UnsuccsefulItem
                            {
                                Reason = [new ErrorObject(_str["badImportData"])],
                                RowIndex = rowIndex
                            }
                        );
                    }

                    rowIndex++;
                }

                if (failures.Count > 0)
                    unsuccessfulItems.Add("lessonPeriod", failures);
            }

            worksheet = doc.Descendants(ss + "Worksheet")
                .FirstOrDefault(w => w.Attribute(ss + "Name")?.Value == _strData["DaysOff"]);

            if (worksheet != null)
            {
                var handler = new CreateDayOffHandler(_dbService, _str, _strFields);
                var rows = worksheet.Descendants(ss + "Row").Skip(1);
                int rowIndex = 2;
                List<UnsuccsefulItem> failures = new();

                foreach (var row in rows)
                {
                    try
                    {
                        var cells = row.Descendants(ss + "Data").Select(d => d.Value).ToList();
                        var dto = new CreateDayOffDTO
                        {
                            Name = cells.ElementAtOrDefault(0)!,
                            From = DateOnly.Parse(cells.ElementAtOrDefault(1)!),
                            To = DateOnly.Parse(cells.ElementAtOrDefault(2)!),
                            ScheduleId = request.dto.ScheduleId
                        };

                        var command = new CreateDayOffCommand(
                            Guid.NewGuid(),
                            dto,
                            request.FilteredId,
                            request.Role
                        );

                        await handler.Handle(command, cancellationToken);
                        succesfulItems++;
                    }
                    catch (ApiErrorException error)
                    {
                        failures.Add(
                            new UnsuccsefulItem { Reason = error.errors, RowIndex = rowIndex }
                        );
                    }
                    catch (Exception)
                    {
                        failures.Add(
                            new UnsuccsefulItem
                            {
                                Reason = [new ErrorObject(_str["badImportData"])],
                                RowIndex = rowIndex
                            }
                        );
                    }
                    rowIndex++;
                }

                if (failures.Count > 0)
                    unsuccessfulItems.Add("dayOff", failures);
            }

            worksheet = doc.Descendants(ss + "Worksheet")
                .FirstOrDefault(w => w.Attribute(ss + "Name")?.Value == _strData["ClassroomTypes"]);

            if (worksheet != null)
            {
                var handler = new CreateClassroomTypeHandler(_dbService, _str, _strFields);
                var rows = worksheet.Descendants(ss + "Row").Skip(1);
                int rowIndex = 2;
                List<UnsuccsefulItem> failures = new();

                foreach (var row in rows)
                {
                    try
                    {
                        var cells = row.Descendants(ss + "Data").Select(d => d.Value).ToList();
                        var dto = new CreateClassroomTypeDTO()
                        {
                            Name = cells.ElementAtOrDefault(0)!,
                            ScheduleId = request.dto.ScheduleId
                        };
                        var command = new CreateClassroomTypeCommand(
                            Guid.NewGuid(),
                            dto,
                            request.FilteredId,
                            request.Role
                        );

                        await handler.Handle(command, cancellationToken);
                        succesfulItems++;
                        classroomTypeIds.Add(dto.Name, command.Id);
                    }
                    catch (ApiErrorException error)
                    {
                        failures.Add(
                            new UnsuccsefulItem { Reason = error.errors, RowIndex = rowIndex }
                        );
                    }
                    catch (Exception)
                    {
                        failures.Add(
                            new UnsuccsefulItem
                            {
                                Reason = [new ErrorObject(_str["badImportData"])],
                                RowIndex = rowIndex
                            }
                        );
                    }
                    rowIndex++;
                }

                if (failures.Count > 0)
                    unsuccessfulItems.Add("classroomType", failures);
            }

            worksheet = doc.Descendants(ss + "Worksheet")
                .FirstOrDefault(w => w.Attribute(ss + "Name")?.Value == _strData["LessonTypes"]);

            if (worksheet != null)
            {
                var handler = new CreateLessonTypeHandler(_dbService, _str, _strFields);
                var rows = worksheet.Descendants(ss + "Row").Skip(1);
                int rowIndex = 2;
                List<UnsuccsefulItem> failures = new();

                foreach (var row in rows)
                {
                    try
                    {
                        var cells = row.Descendants(ss + "Data").Select(d => d.Value).ToList();
                        var dto = new CreateLessonTypeDTO
                        {
                            Name = cells.ElementAtOrDefault(0)!,
                            Color = int.Parse(cells.ElementAtOrDefault(1)!),
                            ScheduleId = request.dto.ScheduleId
                        };
                        var command = new CreateLessonTypeCommand(
                            Guid.NewGuid(),
                            dto,
                            request.FilteredId,
                            request.Role
                        );

                        await handler.Handle(command, cancellationToken);
                        succesfulItems++;
                        lessonTypeIds.Add(dto.Name, command.Id);
                    }
                    catch (ApiErrorException error)
                    {
                        failures.Add(
                            new UnsuccsefulItem { Reason = error.errors, RowIndex = rowIndex }
                        );
                    }
                    catch (Exception)
                    {
                        failures.Add(
                            new UnsuccsefulItem
                            {
                                Reason = [new ErrorObject(_str["badImportData"])],
                                RowIndex = rowIndex
                            }
                        );
                    }
                    rowIndex++;
                }

                if (failures.Count > 0)
                    unsuccessfulItems.Add("lessonType", failures);
            }

            worksheet = doc.Descendants(ss + "Worksheet")
                .FirstOrDefault(w => w.Attribute(ss + "Name")?.Value == _strData["Teachers"]);

            if (worksheet != null)
            {
                var handler = new CreateTeacherHandler(_dbService, _str, _strFields);
                var rows = worksheet.Descendants(ss + "Row").Skip(1);
                int rowIndex = 2;
                List<UnsuccsefulItem> failures = new();

                foreach (var row in rows)
                {
                    try
                    {
                        var cells = row.Descendants(ss + "Data").Select(d => d.Value).ToList();
                        var dto = new CreateTeacherDTO
                        {
                            Email = cells.ElementAtOrDefault(0)!,
                            Name = cells.ElementAtOrDefault(1),
                            Surname = cells.ElementAtOrDefault(2),
                            ScheduleId = request.dto.ScheduleId
                        };
                        var command = new CreateTeacherCommand(
                            Guid.NewGuid(),
                            dto,
                            request.FilteredId,
                            request.Role
                        );

                        await handler.Handle(command, cancellationToken);
                        succesfulItems++;
                        teacherIds.Add(dto.Email, command.Id);
                    }
                    catch (ApiErrorException error)
                    {
                        failures.Add(
                            new UnsuccsefulItem { Reason = error.errors, RowIndex = rowIndex }
                        );
                    }
                    catch (Exception)
                    {
                        failures.Add(
                            new UnsuccsefulItem
                            {
                                Reason = [new ErrorObject(_str["badImportData"])],
                                RowIndex = rowIndex
                            }
                        );
                    }
                    rowIndex++;
                }

                if (failures.Count > 0)
                    unsuccessfulItems.Add("teacher", failures);
            }

            worksheet = doc.Descendants(ss + "Worksheet")
                .FirstOrDefault(w => w.Attribute(ss + "Name")?.Value == _strData["Groups"]);

            if (worksheet != null)
            {
                var handler = new CreateGroupHandler(_dbService, _str, _strFields);
                var rows = worksheet.Descendants(ss + "Row").Skip(1);
                int rowIndex = 2;
                List<UnsuccsefulItem> failures = new();

                foreach (var row in rows)
                {
                    try
                    {
                        var cells = row.Descendants(ss + "Data").Select(d => d.Value).ToList();
                        var dto = new CreateGroupDTO
                        {
                            Name = cells.ElementAtOrDefault(0)!,
                            StudentCount = int.Parse(cells.ElementAtOrDefault(1)!),
                            ScheduleId = request.dto.ScheduleId
                        };
                        var command = new CreateGroupCommand(
                            Guid.NewGuid(),
                            dto,
                            request.FilteredId,
                            request.Role
                        );

                        await handler.Handle(command, cancellationToken);
                        succesfulItems++;
                        groupIds.Add(dto.Name, command.Id);
                    }
                    catch (ApiErrorException error)
                    {
                        failures.Add(
                            new UnsuccsefulItem { Reason = error.errors, RowIndex = rowIndex }
                        );
                    }
                    catch (Exception)
                    {
                        failures.Add(
                            new UnsuccsefulItem
                            {
                                Reason = [new ErrorObject(_str["badImportData"])],
                                RowIndex = rowIndex
                            }
                        );
                    }
                    rowIndex++;
                }

                if (failures.Count > 0)
                    unsuccessfulItems.Add("group", failures);
            }

            worksheet = doc.Descendants(ss + "Worksheet")
                .FirstOrDefault(w => w.Attribute(ss + "Name")?.Value == _strData["Subgroups"]);

            if (worksheet != null)
            {
                var handler = new CreateSubgroupHandler(_dbService, _str, _strFields);
                var rows = worksheet.Descendants(ss + "Row").Skip(1);
                int rowIndex = 2;
                List<UnsuccsefulItem> failures = new();

                foreach (var row in rows)
                {
                    try
                    {
                        var cells = row.Descendants(ss + "Data").Select(d => d.Value).ToList();
                        var groupName = cells.ElementAtOrDefault(2)!;
                        var dto = new CreateSubgroupDTO
                        {
                            Name = cells.ElementAtOrDefault(0)!,
                            StudentCount = int.Parse(cells.ElementAtOrDefault(1)!),
                            GroupId = groupIds[groupName]
                        };
                        var command = new CreateSubgroupCommand(
                            Guid.NewGuid(),
                            dto,
                            request.FilteredId,
                            request.Role
                        );

                        await handler.Handle(command, cancellationToken);
                        succesfulItems++;
                        subgroupIds.Add(groupName + "/" + dto.Name, command.Id);
                    }
                    catch (ApiErrorException error)
                    {
                        failures.Add(
                            new UnsuccsefulItem { Reason = error.errors, RowIndex = rowIndex }
                        );
                    }
                    catch (Exception)
                    {
                        failures.Add(
                            new UnsuccsefulItem
                            {
                                Reason = [new ErrorObject(_str["badImportData"])],
                                RowIndex = rowIndex
                            }
                        );
                    }
                    rowIndex++;
                }

                if (failures.Count > 0)
                    unsuccessfulItems.Add("subgroup", failures);
            }

            worksheet = doc.Descendants(ss + "Worksheet")
                .FirstOrDefault(w => w.Attribute(ss + "Name")?.Value == _strData["Classrooms"]);

            if (worksheet != null)
            {
                var handler = new CreateClassroomHandler(_dbService, _str, _strFields);
                var rows = worksheet.Descendants(ss + "Row").Skip(1);
                int rowIndex = 2;
                List<UnsuccsefulItem> failures = new();

                foreach (var row in rows)
                {
                    try
                    {
                        var cells = row.Descendants(ss + "Data").Select(d => d.Value).ToList();

                        var classroomTypeNames = (cells.ElementAtOrDefault(2)!)
                            .Split(';', StringSplitOptions.RemoveEmptyEntries)
                            .Select(s => s.Trim())
                            .Where(s => !string.IsNullOrEmpty(s))
                            .ToArray();

                        var dto = new CreateClassroomDTO
                        {
                            Name = cells.ElementAtOrDefault(0)!,
                            Capacity = int.Parse(cells.ElementAtOrDefault(1)!),
                            ClassroomTypeIds = classroomTypeNames
                                .Select(tn => classroomTypeIds[tn])
                                .ToList(),
                            ScheduleId = request.dto.ScheduleId
                        };

                        var command = new CreateClassroomCommand(
                            Guid.NewGuid(),
                            dto,
                            request.FilteredId,
                            request.Role
                        );

                        await handler.Handle(command, cancellationToken);
                        succesfulItems++;
                        classroomIds.Add(dto.Name, command.Id);
                    }
                    catch (ApiErrorException error)
                    {
                        failures.Add(
                            new UnsuccsefulItem { Reason = error.errors, RowIndex = rowIndex }
                        );
                    }
                    catch (Exception)
                    {
                        failures.Add(
                            new UnsuccsefulItem
                            {
                                Reason = [new ErrorObject(_str["badImportData"])],
                                RowIndex = rowIndex
                            }
                        );
                    }
                    rowIndex++;
                }

                if (failures.Count > 0)
                    unsuccessfulItems.Add("classroom", failures);
            }

            worksheet = doc.Descendants(ss + "Worksheet")
                .FirstOrDefault(w => w.Attribute(ss + "Name")?.Value == _strData["Availability"]);

            if (worksheet != null)
            {
                var handler = new CreateAvailabilityHandler(_dbService, _str, _strFields);
                var rows = worksheet.Descendants(ss + "Row").Skip(1);
                int rowIndex = 2;
                List<UnsuccsefulItem> failures = new();

                foreach (var row in rows)
                {
                    try
                    {
                        var cells = row.Descendants(ss + "Data").Select(d => d.Value).ToList();
                        var teacherEmail = cells.ElementAtOrDefault(0)!;
                        var inputDay = cells.ElementAtOrDefault(1)!;
                        var dto = new CreateAvailabilityDTO
                        {
                            WeekDay = Utilities.DayOfWeek.dayToIntMap.ContainsKey(inputDay)
                                ? Utilities.DayOfWeek.dayToIntMap[inputDay]
                                : 0,
                            Start = Int32.Parse(cells.ElementAtOrDefault(2)!) - 1,
                            End = Int32.Parse(cells.ElementAtOrDefault(3)!) - 1,
                            TeacherId = teacherIds[teacherEmail]
                        };
                        var command = new CreateAvailabilityCommand(
                            Guid.NewGuid(),
                            dto,
                            request.FilteredId,
                            request.Role
                        );

                        await handler.Handle(command, cancellationToken);
                        succesfulItems++;
                    }
                    catch (ApiErrorException error)
                    {
                        failures.Add(
                            new UnsuccsefulItem { Reason = error.errors, RowIndex = rowIndex }
                        );
                    }
                    catch (Exception)
                    {
                        failures.Add(
                            new UnsuccsefulItem
                            {
                                Reason = [new ErrorObject(_str["badImportData"])],
                                RowIndex = rowIndex
                            }
                        );
                    }
                    rowIndex++;
                }

                if (failures.Count > 0)
                    unsuccessfulItems.Add("availability", failures);
            }

            worksheet = doc.Descendants(ss + "Worksheet")
                .FirstOrDefault(w => w.Attribute(ss + "Name")?.Value == _strData["Students"]);

            if (worksheet != null)
            {
                var handler = new CreateStudentHandler(_dbService, _str, _strFields);
                var rows = worksheet.Descendants(ss + "Row").Skip(1);
                int rowIndex = 2;
                List<UnsuccsefulItem> failures = new();

                foreach (var row in rows)
                {
                    try
                    {
                        var cells = row.Descendants(ss + "Data").Select(d => d.Value).ToList();
                        var albumNumber = cells.ElementAtOrDefault(0)!;
                        var groupName = cells.ElementAtOrDefault(1)!;
                        var subgroupNames = (cells.ElementAtOrDefault(2)!)
                            .Split(';', StringSplitOptions.RemoveEmptyEntries)
                            .Select(s => s.Trim())
                            .Where(s => !string.IsNullOrEmpty(s))
                            .ToArray();

                        var dto = new CreateStudentDTO
                        {
                            AlbumNumber = albumNumber,
                            GroupId = groupIds[groupName],
                            SubgroupIds = subgroupNames
                                .Select(sn => subgroupIds[groupName + "/" + sn])
                                .ToList()
                        };

                        var command = new CreateStudentCommand(
                            Guid.NewGuid(),
                            dto,
                            request.FilteredId,
                            request.Role
                        );

                        await handler.Handle(command, cancellationToken);
                        succesfulItems++;
                    }
                    catch (ApiErrorException error)
                    {
                        failures.Add(
                            new UnsuccsefulItem { Reason = error.errors, RowIndex = rowIndex }
                        );
                    }
                    catch (Exception)
                    {
                        failures.Add(
                            new UnsuccsefulItem
                            {
                                Reason = [new ErrorObject(_str["badImportData"])],
                                RowIndex = rowIndex
                            }
                        );
                    }
                    rowIndex++;
                }

                if (failures.Count > 0)
                    unsuccessfulItems.Add("student", failures);
            }

            worksheet = doc.Descendants(ss + "Worksheet")
                .FirstOrDefault(w => w.Attribute(ss + "Name")?.Value == _strData["Lessons"]);

            if (worksheet != null)
            {
                var handler = new CreateLessonHandler(_dbService, _str, _strFields);
                var rows = worksheet.Descendants(ss + "Row").Skip(1);
                int rowIndex = 2;
                List<UnsuccsefulItem> failures = new();

                foreach (var row in rows)
                {
                    try
                    {
                        var cells = row.Descendants(ss + "Data").Select(d => d.Value).ToList();

                        var classroomTypeNames = (cells.ElementAtOrDefault(4)!)
                            .Split(';', StringSplitOptions.RemoveEmptyEntries)
                            .Select(s => s.Trim())
                            .Where(s => !string.IsNullOrEmpty(s))
                            .ToList();

                        var subgroupNames = (cells.ElementAtOrDefault(5)!)
                            .Split(';', StringSplitOptions.RemoveEmptyEntries)
                            .Select(s => s.Trim())
                            .Where(s => !string.IsNullOrEmpty(s))
                            .ToList();

                        var dto = new CreateLessonDTO
                        {
                            Name = cells.ElementAtOrDefault(0)!,
                            AmountOfHours = int.Parse(cells.ElementAtOrDefault(1)!),
                            LessonTypeId = lessonTypeIds[cells.ElementAtOrDefault(2)!],
                            TeacherId = teacherIds[cells.ElementAtOrDefault(3)!],
                            ClassroomTypeIds = classroomTypeNames
                                .Select(tn => classroomTypeIds[tn])
                                .ToList(),
                            SubgroupIds = subgroupNames.Select(sn => subgroupIds[sn]).ToList(),
                        };

                        var command = new CreateLessonCommand(
                            Guid.NewGuid(),
                            dto,
                            request.FilteredId,
                            request.Role
                        );

                        await handler.Handle(command, cancellationToken);
                        succesfulItems++;
                    }
                    catch (ApiErrorException error)
                    {
                        failures.Add(
                            new UnsuccsefulItem { Reason = error.errors, RowIndex = rowIndex }
                        );
                    }
                    catch (Exception)
                    {
                        failures.Add(
                            new UnsuccsefulItem
                            {
                                Reason = [new ErrorObject(_str["badImportData"])],
                                RowIndex = rowIndex
                            }
                        );
                    }
                    rowIndex++;
                }

                if (failures.Count > 0)
                    unsuccessfulItems.Add("lesson", failures);
            }

            return new ImportDataOutputDTO()
            {
                successfulItems = succesfulItems,
                unsuccessfulItems = unsuccessfulItems
            };
        }
    }
}
