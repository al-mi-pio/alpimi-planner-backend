using System.Text.Json;
using AlpimiAPI.Database;
using AlpimiAPI.Entities.EAvailability.Commands;
using AlpimiAPI.Entities.EAvailability.DTO;
using AlpimiAPI.Entities.EClassroom.Commands;
using AlpimiAPI.Entities.EClassroom.DTO;
using AlpimiAPI.Entities.EClassroomType.Commands;
using AlpimiAPI.Entities.EClassroomType.DTO;
using AlpimiAPI.Entities.ECollisionType.Commands;
using AlpimiAPI.Entities.ECollisionType.DTO;
using AlpimiAPI.Entities.EDayOff.Commands;
using AlpimiAPI.Entities.EDayOff.DTO;
using AlpimiAPI.Entities.EGroup.Commands;
using AlpimiAPI.Entities.EGroup.DTO;
using AlpimiAPI.Entities.ELesson.Commands;
using AlpimiAPI.Entities.ELesson.DTO;
using AlpimiAPI.Entities.ELessonBlock.Commands;
using AlpimiAPI.Entities.ELessonBlock.DTO;
using AlpimiAPI.Entities.ELessonPeriod.Commands;
using AlpimiAPI.Entities.ELessonPeriod.DTO;
using AlpimiAPI.Entities.ELessonType.Commands;
using AlpimiAPI.Entities.ELessonType.DTO;
using AlpimiAPI.Entities.EScheduleSettings.Commands;
using AlpimiAPI.Entities.EScheduleSettings.DTO;
using AlpimiAPI.Entities.EStudent.Commands;
using AlpimiAPI.Entities.EStudent.DTO;
using AlpimiAPI.Entities.ESubgroup.Commands;
using AlpimiAPI.Entities.ESubgroup.DTO;
using AlpimiAPI.Entities.ETeacher.Commands;
using AlpimiAPI.Entities.ETeacher.DTO;
using AlpimiAPI.Locales;
using AlpimiAPI.Responses;
using MediatR;
using Microsoft.Extensions.Localization;

namespace AlpimiAPI.Entities.EHistory.Commands
{
    public record RevertCommand(Guid ScheduleId, bool Redo, Guid FilteredId, string Role)
        : IRequest<Guid>;

    public class RevertCommandHandler : IRequestHandler<RevertCommand, Guid>
    {
        private readonly IDbService _dbService;
        private readonly IStringLocalizer<Errors> _str;
        private readonly IStringLocalizer<Fields> _strFields;

        public RevertCommandHandler(
            IDbService dbService,
            IStringLocalizer<Errors> str,
            IStringLocalizer<Fields> strFields
        )
        {
            _dbService = dbService;
            _str = str;
            _strFields = strFields;
        }

        public async Task<Guid> Handle(RevertCommand request, CancellationToken cancellationToken)
        {
            History? latestHistory;
            switch (request.Role)
            {
                case "Admin":
                    latestHistory = await _dbService.Get<History?>(
                        $@"
                            SELECT TOP 1
                            [Id], [Timestamp], [AffectedEntityId], [AffectedEntity], [Command], [ReversaleDTO], [IsUndone], [CollisionChecked], [ScheduleId]
                            FROM [History]
                            WHERE [ScheduleId] = @ScheduleId AND [IsUndone] = @Redo
                            ORDER BY [Timestamp] DESC;",
                        request
                    );
                    break;
                default:
                    latestHistory = await _dbService.Get<History?>(
                        $@"
                            SELECT TOP 1
                            h.[Id], [Timestamp], [AffectedEntityId], [AffectedEntity], [Command], [ReversaleDTO], [IsUndone], [CollisionChecked], [ScheduleId]
                            FROM [History] h
                            INNER JOIN [Schedule] s ON s.[Id] = h.[ScheduleId]
                            WHERE [ScheduleId] = @ScheduleId AND [IsUndone] = @Redo AND s.[UserId] = @FilteredId
                            ORDER BY [Timestamp] DESC;",
                        request
                    );
                    break;
            }

            if (latestHistory == null)
            {
                if (request.Redo)
                {
                    throw new ApiErrorException([new ErrorObject(_str["cantRedo"])]);
                }
                else
                {
                    throw new ApiErrorException([new ErrorObject(_str["cantUndo"])]);
                }
            }

            Guid outputId = latestHistory.AffectedEntityId;
            switch (latestHistory.Command)
            {
                case "Create":
                    switch (latestHistory.AffectedEntity)
                    {
                        case "CollisionType":
                            DeleteCollisionTypeHandler deleteCollisionTypeHandler =
                                new DeleteCollisionTypeHandler(_dbService);
                            DeleteCollisionTypeCommand deleteCollisionTypeCommand =
                                new DeleteCollisionTypeCommand(
                                    latestHistory.AffectedEntityId,
                                    request.FilteredId,
                                    request.Role
                                );
                            await deleteCollisionTypeHandler.Handle(
                                deleteCollisionTypeCommand,
                                cancellationToken
                            );
                            break;

                        case "Classroom":
                            DeleteClassroomHandler deleteClassroomHandler =
                                new DeleteClassroomHandler(_dbService);
                            DeleteClassroomCommand deleteClassroomCommand =
                                new DeleteClassroomCommand(
                                    latestHistory.AffectedEntityId,
                                    request.FilteredId,
                                    request.Role
                                );
                            await deleteClassroomHandler.Handle(
                                deleteClassroomCommand,
                                cancellationToken
                            );
                            break;

                        case "ClassroomType":
                            DeleteClassroomTypeHandler deleteClassroomTypeHandler =
                                new DeleteClassroomTypeHandler(_dbService);
                            DeleteClassroomTypeCommand deleteClassroomTypeCommand =
                                new DeleteClassroomTypeCommand(
                                    latestHistory.AffectedEntityId,
                                    request.FilteredId,
                                    request.Role
                                );
                            await deleteClassroomTypeHandler.Handle(
                                deleteClassroomTypeCommand,
                                cancellationToken
                            );
                            break;

                        case "LessonType":
                            DeleteLessonTypeHandler deleteLessonTypeHandler =
                                new DeleteLessonTypeHandler(_dbService);
                            DeleteLessonTypeCommand deleteLessonTypeCommand =
                                new DeleteLessonTypeCommand(
                                    latestHistory.AffectedEntityId,
                                    request.FilteredId,
                                    request.Role
                                );
                            await deleteLessonTypeHandler.Handle(
                                deleteLessonTypeCommand,
                                cancellationToken
                            );
                            break;

                        case "Teacher":
                            DeleteTeacherHandler deleteTeacherHandler = new DeleteTeacherHandler(
                                _dbService
                            );
                            DeleteTeacherCommand deleteTeacherCommand = new DeleteTeacherCommand(
                                latestHistory.AffectedEntityId,
                                request.FilteredId,
                                request.Role
                            );
                            await deleteTeacherHandler.Handle(
                                deleteTeacherCommand,
                                cancellationToken
                            );
                            break;

                        case "Group":
                            DeleteGroupHandler deleteGroupHandler = new DeleteGroupHandler(
                                _dbService
                            );
                            DeleteGroupCommand deleteGroupCommand = new DeleteGroupCommand(
                                latestHistory.AffectedEntityId,
                                request.FilteredId,
                                request.Role
                            );
                            await deleteGroupHandler.Handle(deleteGroupCommand, cancellationToken);
                            break;

                        case "Subgroup":
                            DeleteSubgroupHandler deleteSubgroupHandler = new DeleteSubgroupHandler(
                                _dbService
                            );
                            DeleteSubgroupCommand deleteSubgroupCommand = new DeleteSubgroupCommand(
                                latestHistory.AffectedEntityId,
                                request.FilteredId,
                                request.Role
                            );
                            await deleteSubgroupHandler.Handle(
                                deleteSubgroupCommand,
                                cancellationToken
                            );
                            break;

                        case "Student":
                            DeleteStudentHandler deleteStudentHandler = new DeleteStudentHandler(
                                _dbService
                            );
                            DeleteStudentCommand deleteStudentCommand = new DeleteStudentCommand(
                                latestHistory.AffectedEntityId,
                                request.FilteredId,
                                request.Role
                            );
                            await deleteStudentHandler.Handle(
                                deleteStudentCommand,
                                cancellationToken
                            );
                            break;

                        case "Lesson":
                            DeleteLessonHandler deleteLessonHandler = new DeleteLessonHandler(
                                _dbService
                            );
                            DeleteLessonCommand deleteLessonCommand = new DeleteLessonCommand(
                                latestHistory.AffectedEntityId,
                                request.FilteredId,
                                request.Role
                            );
                            await deleteLessonHandler.Handle(
                                deleteLessonCommand,
                                cancellationToken
                            );
                            break;

                        case "LessonBlock":
                        case "LessonBlockCluster":
                            DeleteLessonBlockHandler deleteLessonBlockHandler =
                                new DeleteLessonBlockHandler(_dbService);
                            DeleteLessonBlockCommand deleteLessonBlockCommand =
                                new DeleteLessonBlockCommand(
                                    latestHistory.AffectedEntityId,
                                    request.FilteredId,
                                    request.Role
                                );
                            await deleteLessonBlockHandler.Handle(
                                deleteLessonBlockCommand,
                                cancellationToken
                            );
                            break;

                        case "Availability":
                            DeleteAvailabilityHandler deleteAvailabilityHandler =
                                new DeleteAvailabilityHandler(_dbService);
                            DeleteAvailabilityCommand deleteAvailabilityCommand =
                                new DeleteAvailabilityCommand(
                                    latestHistory.AffectedEntityId,
                                    request.FilteredId,
                                    request.Role
                                );
                            await deleteAvailabilityHandler.Handle(
                                deleteAvailabilityCommand,
                                cancellationToken
                            );
                            break;

                        case "DayOff":
                            DeleteDayOffHandler deleteDayOffHandler = new DeleteDayOffHandler(
                                _dbService
                            );
                            DeleteDayOffCommand deleteDayOffCommand = new DeleteDayOffCommand(
                                latestHistory.AffectedEntityId,
                                request.FilteredId,
                                request.Role
                            );
                            await deleteDayOffHandler.Handle(
                                deleteDayOffCommand,
                                cancellationToken
                            );
                            break;

                        case "LessonPeriod":
                            DeleteLessonPeriodHandler deleteLessonPeriodHandler =
                                new DeleteLessonPeriodHandler(_dbService);
                            DeleteLessonPeriodCommand deleteLessonPeriodCommand =
                                new DeleteLessonPeriodCommand(
                                    latestHistory.AffectedEntityId,
                                    request.FilteredId,
                                    request.Role
                                );
                            await deleteLessonPeriodHandler.Handle(
                                deleteLessonPeriodCommand,
                                cancellationToken
                            );
                            break;
                        default:
                            throw new ApiErrorException(
                                [
                                    new ErrorObject(
                                        _str["unknownError", latestHistory.AffectedEntity]
                                    )
                                ]
                            );
                    }
                    break;
                case "Patch":
                    switch (latestHistory.AffectedEntity)
                    {
                        case "CollisionType":
                            UpdateCollisionTypeHandler updateCollisionTypeHandler =
                                new UpdateCollisionTypeHandler(_dbService, _str, _strFields);
                            UpdateCollisionTypeCommand updateCollisionTypeCommand =
                                new UpdateCollisionTypeCommand(
                                    latestHistory.AffectedEntityId,
                                    JsonSerializer.Deserialize<UpdateCollisionTypeDTO>(
                                        latestHistory.ReversaleDTO!
                                    )!,
                                    request.FilteredId,
                                    request.Role
                                );
                            await updateCollisionTypeHandler.Handle(
                                updateCollisionTypeCommand,
                                cancellationToken
                            );
                            break;

                        case "Classroom":
                            UpdateClassroomHandler updateClassroomHandler =
                                new UpdateClassroomHandler(_dbService, _str, _strFields);
                            UpdateClassroomCommand updateClassroomCommand =
                                new UpdateClassroomCommand(
                                    latestHistory.AffectedEntityId,
                                    JsonSerializer.Deserialize<UpdateClassroomDTO>(
                                        latestHistory.ReversaleDTO!
                                    )!,
                                    request.FilteredId,
                                    request.Role
                                );
                            await updateClassroomHandler.Handle(
                                updateClassroomCommand,
                                cancellationToken
                            );
                            break;

                        case "ClassroomType":
                            UpdateClassroomTypeHandler updateClassroomTypeHandler =
                                new UpdateClassroomTypeHandler(_dbService, _str, _strFields);
                            UpdateClassroomTypeCommand updateClassroomTypeCommand =
                                new UpdateClassroomTypeCommand(
                                    latestHistory.AffectedEntityId,
                                    JsonSerializer.Deserialize<UpdateClassroomTypeDTO>(
                                        latestHistory.ReversaleDTO!
                                    )!,
                                    request.FilteredId,
                                    request.Role
                                );
                            await updateClassroomTypeHandler.Handle(
                                updateClassroomTypeCommand,
                                cancellationToken
                            );
                            break;

                        case "LessonType":
                            UpdateLessonTypeHandler updateLessonTypeHandler =
                                new UpdateLessonTypeHandler(_dbService, _str, _strFields);
                            UpdateLessonTypeCommand updateLessonTypeCommand =
                                new UpdateLessonTypeCommand(
                                    latestHistory.AffectedEntityId,
                                    JsonSerializer.Deserialize<UpdateLessonTypeDTO>(
                                        latestHistory.ReversaleDTO!
                                    )!,
                                    request.FilteredId,
                                    request.Role
                                );
                            await updateLessonTypeHandler.Handle(
                                updateLessonTypeCommand,
                                cancellationToken
                            );
                            break;

                        case "Teacher":
                            UpdateTeacherHandler updateTeacherHandler = new UpdateTeacherHandler(
                                _dbService,
                                _str
                            );
                            UpdateTeacherCommand updateTeacherCommand = new UpdateTeacherCommand(
                                latestHistory.AffectedEntityId,
                                JsonSerializer.Deserialize<UpdateTeacherDTO>(
                                    latestHistory.ReversaleDTO!
                                )!,
                                request.FilteredId,
                                request.Role
                            );
                            await updateTeacherHandler.Handle(
                                updateTeacherCommand,
                                cancellationToken
                            );
                            break;

                        case "Group":
                            UpdateGroupHandler updateGroupHandler = new UpdateGroupHandler(
                                _dbService,
                                _str,
                                _strFields
                            );
                            UpdateGroupCommand updateGroupCommand = new UpdateGroupCommand(
                                latestHistory.AffectedEntityId,
                                JsonSerializer.Deserialize<UpdateGroupDTO>(
                                    latestHistory.ReversaleDTO!
                                )!,
                                request.FilteredId,
                                request.Role
                            );
                            await updateGroupHandler.Handle(updateGroupCommand, cancellationToken);
                            break;

                        case "Subgroup":
                            UpdateSubgroupHandler updateSubgroupHandler = new UpdateSubgroupHandler(
                                _dbService,
                                _str,
                                _strFields
                            );
                            UpdateSubgroupCommand updateSubgroupCommand = new UpdateSubgroupCommand(
                                latestHistory.AffectedEntityId,
                                JsonSerializer.Deserialize<UpdateSubgroupDTO>(
                                    latestHistory.ReversaleDTO!
                                )!,
                                request.FilteredId,
                                request.Role
                            );
                            await updateSubgroupHandler.Handle(
                                updateSubgroupCommand,
                                cancellationToken
                            );
                            break;

                        case "Student":
                            UpdateStudentHandler updateStudentHandler = new UpdateStudentHandler(
                                _dbService,
                                _str,
                                _strFields
                            );
                            UpdateStudentCommand updateStudentCommand = new UpdateStudentCommand(
                                latestHistory.AffectedEntityId,
                                JsonSerializer.Deserialize<UpdateStudentDTO>(
                                    latestHistory.ReversaleDTO!
                                )!,
                                request.FilteredId,
                                request.Role
                            );
                            await updateStudentHandler.Handle(
                                updateStudentCommand,
                                cancellationToken
                            );
                            break;

                        case "Lesson":
                            UpdateLessonHandler updateLessonHandler = new UpdateLessonHandler(
                                _dbService,
                                _str,
                                _strFields
                            );
                            UpdateLessonCommand updateLessonCommand = new UpdateLessonCommand(
                                latestHistory.AffectedEntityId,
                                JsonSerializer.Deserialize<UpdateLessonDTO>(
                                    latestHistory.ReversaleDTO!
                                )!,
                                request.FilteredId,
                                request.Role
                            );
                            await updateLessonHandler.Handle(
                                updateLessonCommand,
                                cancellationToken
                            );
                            break;

                        case "LessonBlock":
                        case "LessonBlockCluster":
                            UpdateLessonBlockHandler updateLessonBlockHandler =
                                new UpdateLessonBlockHandler(_dbService, _str, _strFields);
                            UpdateLessonBlockCommand updateLessonBlockCommand =
                                new UpdateLessonBlockCommand(
                                    latestHistory.AffectedEntityId,
                                    JsonSerializer.Deserialize<UpdateLessonBlockDTO>(
                                        latestHistory.ReversaleDTO!
                                    )!,
                                    request.FilteredId,
                                    request.Role
                                );
                            await updateLessonBlockHandler.Handle(
                                updateLessonBlockCommand,
                                cancellationToken
                            );
                            break;

                        case "Availability":
                            UpdateAvailabilityHandler updateAvailabilityHandler =
                                new UpdateAvailabilityHandler(_dbService, _str, _strFields);
                            UpdateAvailabilityCommand updateAvailabilityCommand =
                                new UpdateAvailabilityCommand(
                                    latestHistory.AffectedEntityId,
                                    JsonSerializer.Deserialize<UpdateAvailabilityDTO>(
                                        latestHistory.ReversaleDTO!
                                    )!,
                                    request.FilteredId,
                                    request.Role
                                );
                            await updateAvailabilityHandler.Handle(
                                updateAvailabilityCommand,
                                cancellationToken
                            );
                            break;

                        case "DayOff":
                            UpdateDayOffHandler updateDayOffHandler = new UpdateDayOffHandler(
                                _dbService,
                                _str,
                                _strFields
                            );
                            UpdateDayOffCommand updateDayOffCommand = new UpdateDayOffCommand(
                                latestHistory.AffectedEntityId,
                                JsonSerializer.Deserialize<UpdateDayOffDTO>(
                                    latestHistory.ReversaleDTO!
                                )!,
                                request.FilteredId,
                                request.Role
                            );
                            await updateDayOffHandler.Handle(
                                updateDayOffCommand,
                                cancellationToken
                            );
                            break;

                        case "LessonPeriod":
                            UpdateLessonPeriodHandler updateLessonPeriodHandler =
                                new UpdateLessonPeriodHandler(_dbService, _str, _strFields);
                            UpdateLessonPeriodCommand updateLessonPeriodCommand =
                                new UpdateLessonPeriodCommand(
                                    latestHistory.AffectedEntityId,
                                    JsonSerializer.Deserialize<UpdateLessonPeriodDTO>(
                                        latestHistory.ReversaleDTO!
                                    )!,
                                    request.FilteredId,
                                    request.Role
                                );
                            await updateLessonPeriodHandler.Handle(
                                updateLessonPeriodCommand,
                                cancellationToken
                            );
                            break;

                        case "ScheduleSettings":
                            UpdateScheduleSettingsHandler updateScheduleSettingsHandler =
                                new UpdateScheduleSettingsHandler(_dbService, _str, _strFields);
                            UpdateScheduleSettingsCommand updateScheduleSettingsCommand =
                                new UpdateScheduleSettingsCommand(
                                    latestHistory.AffectedEntityId,
                                    JsonSerializer.Deserialize<UpdateScheduleSettingsDTO>(
                                        latestHistory.ReversaleDTO!
                                    )!,
                                    request.FilteredId,
                                    request.Role
                                );
                            await updateScheduleSettingsHandler.Handle(
                                updateScheduleSettingsCommand,
                                cancellationToken
                            );
                            break;
                        default:
                            throw new ApiErrorException(
                                [
                                    new ErrorObject(
                                        _str["unknownError", latestHistory.AffectedEntity]
                                    )
                                ]
                            );
                    }
                    break;
                case "Delete":
                    Guid insertedId = Guid.NewGuid();
                    switch (latestHistory.AffectedEntity)
                    {
                        case "CollisionType":
                            CreateCollisionTypeHandler createCollisionTypeHandler =
                                new CreateCollisionTypeHandler(_dbService, _str, _strFields);
                            CreateCollisionTypeCommand createCollisionTypeCommand =
                                new CreateCollisionTypeCommand(
                                    insertedId,
                                    JsonSerializer.Deserialize<CreateCollisionTypeDTO>(
                                        latestHistory.ReversaleDTO!
                                    )!,
                                    request.FilteredId,
                                    request.Role
                                );
                            await createCollisionTypeHandler.Handle(
                                createCollisionTypeCommand,
                                cancellationToken
                            );
                            break;

                        case "Classroom":
                            CreateClassroomHandler createClassroomHandler =
                                new CreateClassroomHandler(_dbService, _str, _strFields);
                            CreateClassroomCommand createClassroomCommand =
                                new CreateClassroomCommand(
                                    insertedId,
                                    JsonSerializer.Deserialize<CreateClassroomDTO>(
                                        latestHistory.ReversaleDTO!
                                    )!,
                                    request.FilteredId,
                                    request.Role
                                );
                            await createClassroomHandler.Handle(
                                createClassroomCommand,
                                cancellationToken
                            );
                            break;

                        case "ClassroomType":
                            CreateClassroomTypeHandler createClassroomTypeHandler =
                                new CreateClassroomTypeHandler(_dbService, _str, _strFields);
                            CreateClassroomTypeCommand createClassroomTypeCommand =
                                new CreateClassroomTypeCommand(
                                    insertedId,
                                    JsonSerializer.Deserialize<CreateClassroomTypeDTO>(
                                        latestHistory.ReversaleDTO!
                                    )!,
                                    request.FilteredId,
                                    request.Role
                                );
                            await createClassroomTypeHandler.Handle(
                                createClassroomTypeCommand,
                                cancellationToken
                            );
                            break;

                        case "LessonType":
                            CreateLessonTypeHandler createLessonTypeHandler =
                                new CreateLessonTypeHandler(_dbService, _str, _strFields);
                            CreateLessonTypeCommand createLessonTypeCommand =
                                new CreateLessonTypeCommand(
                                    insertedId,
                                    JsonSerializer.Deserialize<CreateLessonTypeDTO>(
                                        latestHistory.ReversaleDTO!
                                    )!,
                                    request.FilteredId,
                                    request.Role
                                );
                            await createLessonTypeHandler.Handle(
                                createLessonTypeCommand,
                                cancellationToken
                            );
                            break;

                        case "Teacher":
                            CreateTeacherHandler createTeacherHandler = new CreateTeacherHandler(
                                _dbService,
                                _str,
                                _strFields
                            );
                            CreateTeacherCommand createTeacherCommand = new CreateTeacherCommand(
                                insertedId,
                                JsonSerializer.Deserialize<CreateTeacherDTO>(
                                    latestHistory.ReversaleDTO!
                                )!,
                                request.FilteredId,
                                request.Role
                            );
                            await createTeacherHandler.Handle(
                                createTeacherCommand,
                                cancellationToken
                            );
                            break;

                        case "Group":
                            CreateGroupHandler createGroupHandler = new CreateGroupHandler(
                                _dbService,
                                _str,
                                _strFields
                            );
                            CreateGroupCommand createGroupCommand = new CreateGroupCommand(
                                insertedId,
                                JsonSerializer.Deserialize<CreateGroupDTO>(
                                    latestHistory.ReversaleDTO!
                                )!,
                                request.FilteredId,
                                request.Role
                            );
                            await createGroupHandler.Handle(createGroupCommand, cancellationToken);
                            break;

                        case "Subgroup":
                            CreateSubgroupHandler createSubgroupHandler = new CreateSubgroupHandler(
                                _dbService,
                                _str,
                                _strFields
                            );
                            CreateSubgroupCommand createSubgroupCommand = new CreateSubgroupCommand(
                                insertedId,
                                JsonSerializer.Deserialize<CreateSubgroupDTO>(
                                    latestHistory.ReversaleDTO!
                                )!,
                                request.FilteredId,
                                request.Role
                            );
                            await createSubgroupHandler.Handle(
                                createSubgroupCommand,
                                cancellationToken
                            );
                            break;

                        case "Student":
                            CreateStudentHandler createStudentHandler = new CreateStudentHandler(
                                _dbService,
                                _str,
                                _strFields
                            );
                            CreateStudentCommand createStudentCommand = new CreateStudentCommand(
                                insertedId,
                                JsonSerializer.Deserialize<CreateStudentDTO>(
                                    latestHistory.ReversaleDTO!
                                )!,
                                request.FilteredId,
                                request.Role
                            );
                            await createStudentHandler.Handle(
                                createStudentCommand,
                                cancellationToken
                            );
                            break;

                        case "Lesson":
                            CreateLessonHandler createLessonHandler = new CreateLessonHandler(
                                _dbService,
                                _str,
                                _strFields
                            );
                            CreateLessonCommand createLessonCommand = new CreateLessonCommand(
                                insertedId,
                                JsonSerializer.Deserialize<CreateLessonDTO>(
                                    latestHistory.ReversaleDTO!
                                )!,
                                request.FilteredId,
                                request.Role
                            );
                            await createLessonHandler.Handle(
                                createLessonCommand,
                                cancellationToken
                            );
                            break;

                        case "LessonBlock":
                        case "LessonBlockCluster":
                            CreateLessonBlockDTO createLessonBlockDTO =
                                JsonSerializer.Deserialize<CreateLessonBlockDTO>(
                                    latestHistory.ReversaleDTO!
                                )!;
                            CreateLessonBlockHandler createLessonBlockHandler =
                                new CreateLessonBlockHandler(_dbService, _str, _strFields);
                            CreateLessonBlockCommand createLessonBlockCommand =
                                new CreateLessonBlockCommand(
                                    createLessonBlockDTO.WeekInterval == null
                                        ? insertedId
                                        : Guid.NewGuid(),
                                    createLessonBlockDTO.WeekInterval != null
                                        ? insertedId
                                        : Guid.NewGuid(),
                                    createLessonBlockDTO,
                                    request.FilteredId,
                                    request.Role
                                );
                            await createLessonBlockHandler.Handle(
                                createLessonBlockCommand,
                                cancellationToken
                            );
                            break;

                        case "Availability":
                            CreateAvailabilityHandler createAvailabilityHandler =
                                new CreateAvailabilityHandler(_dbService, _str, _strFields);
                            CreateAvailabilityCommand createAvailabilityCommand =
                                new CreateAvailabilityCommand(
                                    insertedId,
                                    JsonSerializer.Deserialize<CreateAvailabilityDTO>(
                                        latestHistory.ReversaleDTO!
                                    )!,
                                    request.FilteredId,
                                    request.Role
                                );
                            await createAvailabilityHandler.Handle(
                                createAvailabilityCommand,
                                cancellationToken
                            );
                            break;

                        case "DayOff":
                            CreateDayOffHandler createDayOffHandler = new CreateDayOffHandler(
                                _dbService,
                                _str,
                                _strFields
                            );
                            CreateDayOffCommand createDayOffCommand = new CreateDayOffCommand(
                                insertedId,
                                JsonSerializer.Deserialize<CreateDayOffDTO>(
                                    latestHistory.ReversaleDTO!
                                )!,
                                request.FilteredId,
                                request.Role
                            );
                            await createDayOffHandler.Handle(
                                createDayOffCommand,
                                cancellationToken
                            );
                            break;

                        case "LessonPeriod":
                            CreateLessonPeriodHandler createLessonPeriodHandler =
                                new CreateLessonPeriodHandler(_dbService, _str, _strFields);
                            CreateLessonPeriodCommand createLessonPeriodCommand =
                                new CreateLessonPeriodCommand(
                                    insertedId,
                                    JsonSerializer.Deserialize<CreateLessonPeriodDTO>(
                                        latestHistory.ReversaleDTO!
                                    )!,
                                    request.FilteredId,
                                    request.Role
                                );
                            await createLessonPeriodHandler.Handle(
                                createLessonPeriodCommand,
                                cancellationToken
                            );
                            break;
                        default:
                            throw new ApiErrorException(
                                [
                                    new ErrorObject(
                                        _str["unknownError", latestHistory.AffectedEntity]
                                    )
                                ]
                            );
                    }
                    outputId = insertedId;
                    await _dbService.Update<Guid?>(
                        $@"
                            UPDATE [History]
                            SET
                            [ReversaleDTO] = REPLACE([ReversaleDTO], '{latestHistory.AffectedEntityId}', '{insertedId}'),
                            [AffectedEntityId] = REPLACE([AffectedEntityId], '{latestHistory.AffectedEntityId}', '{insertedId}')
                            WHERE [ReversaleDTO] LIKE '%{latestHistory.AffectedEntityId}%' OR [AffectedEntityId] LIKE '%{latestHistory.AffectedEntityId}%';",
                        request
                    );
                    break;
                default:
                    throw new ApiErrorException(
                        [new ErrorObject(_str["unknownError", latestHistory.Command])]
                    );
            }

            if (!request.Redo)
            {
                await _dbService.Update<bool?>(
                    $@"
                        WITH h AS (
                        SELECT TOP 1 *
                        FROM [History]
                        WHERE [ScheduleId] = @ScheduleId
                        ORDER BY [Timestamp] DESC
                        )
                        UPDATE h
                        SET [IsUndone] = 1
                        OUTPUT INSERTED.[IsUndone];",
                    request
                );
            }
            await _dbService.Delete(
                $@"
                    DELETE [History] 
                    WHERE [Id] = '{latestHistory.Id}';",
                ""
            );

            return outputId;
        }
    }
}
