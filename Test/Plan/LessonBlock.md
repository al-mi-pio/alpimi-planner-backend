# LessonBlock entity test plan

## `ALL` `api/LessonBlock/*`

- [LessonBlockControllerThrowsTooManyRequests()](../Entities/ELessonBlock/LessonBlockController.test.cs) - **integrity**  
  Check if returns an error when request is sent too many times

- [LessonBlockSettingsControllerThrowsUnauthorized()](../Entities/ELessonBlock/LessonBlockController.test.cs) - **integrity**  
  Check if returns an error when token is not provided

## `POST` `api/LessonBlock`

- [LessonBlockIsCreated()](../Entities/ELessonBlock/LessonBlockController.test.cs) - **integrity**  
  Check if lesson block is created when provided with correct data

- [LessonBlockClusterIsCreated()](../Entities/ELessonBlock/LessonBlockController.test.cs) - **integrity**  
  Check if multiple lesson blocks are created when a week interval is provided

- [CreateLessonBlockUpdatesLessonsCurrentHours()](../Entities/ELessonBlock/LessonBlockController.test.cs) - **integrity**  
  Check if creating a lesson block updates the current hours of the lesson

- [ThrowsErrorWhenWrongLessonIdIsGiven()](../Entities/ELessonBlock/CreateLessonBlockCommand.unit.cs) - **unit** 
  Check if returns an error when incorrect lesson id is provided

- [ThrowsErrorWhenWrongTeacherIdIsGiven()](../Entities/ELessonBlock/CreateLessonBlockCommand.unit.cs) - **unit** 
  Check if returns an error when incorrect teacher id is provided

- [ThrowsErrorWhenScheduleIdsFromLessonAndTeacherDontMatch()](../Entities/ELessonBlock/CreateLessonBlockCommand.unit.cs) - **unit** 
  Check if returns an error when lesson and teacher are from different schedules

- [ThrowsErrorWhenWrongClassroomIdIsGiven()](../Entities/ELessonBlock/CreateLessonBlockCommand.unit.cs) - **unit** 
  Check if returns an error when incorrect classroom id is provided

- [ThrowsErrorWhenScheduleIdsFromLessonAndClassroomDontMatch()](../Entities/ELessonBlock/CreateLessonBlockCommand.unit.cs) - **unit** 
  Check if returns an error when lesson and classroom are from different schedules

- [ThrowsErrorWhenLessonStartIsAfterLessonEnd()](../Entities/ELessonBlock/Commands/CreateLessonBlockCommand.unit.cs) - **unit** 
  Check if returns an error when incorrect lesson start and end times are provided

- [ThrowsErrorWhenLessonStartIsLessThan1()](../Entities/ELessonBlock/Commands/CreateLessonBlockCommand.unit.cs) - **unit** 
  Check if returns an error when lesson start is less than one

- [ThrowsErrorWhenLessonStartIsMoreThanTheAmountOfLessonPeriods()](../Entities/ELessonBlock/Commands/CreateLessonBlockCommand.unit.cs) - **unit** 
  Check if returns an error when lesson start is more than the amount of lesson periods

- [ThrowsErrorWhenLessonEndIsLessThan1()](../Entities/ELessonBlock/Commands/CreateLessonBlockCommand.unit.cs) - **unit** 
  Check if returns an error when lesson end is less than one

- [ThrowsErrorWhenLessonEndIsMoreThanTheAmountOfLessonPeriods()](../Entities/ELessonBlock/Commands/CreateLessonBlockCommand.unit.cs) - **unit** 
  Check if returns an error when lesson end is more than the amount of lesson periods

- [ThrowsErrorWhenLessonDateIsBeforeSchoolYearStart()](../Entities/ELessonBlock/Commands/CreateLessonBlockCommand.unit.cs) - **unit** 
  Check if returns an error when lesson date is before the start of the school year

- [ThrowsErrorWhenLessonDateIsAfterSchoolYearEnd()](../Entities/ELessonBlock/Commands/CreateLessonBlockCommand.unit.cs) - **unit** 
  Check if returns an error when lesson date is after the end of the school year

- [ThrowsErrorWhenLessonOccursOnADayOfTheWeekThatIsNotAllowedByScheduleSettings()](../Entities/ELessonBlock/Commands/CreateLessonBlockCommand.unit.cs) - **unit** 
  Check if returns an error when lesson occurs on a day of the week that is not allowed by schedule settings

- [ThrowsErrorWhenWeekIntervalIsLessThan1()](../Entities/ELessonBlock/Commands/CreateLessonBlockCommand.unit.cs) - **unit** 
  Check if returns an error when lesson end is more than the amount of lesson periods


## `GET` `api/LessonBlock`

- [GetAllLessonBlocksReturnsLessonBlocksFromGroupIfGroupIdIsProvided()](../Entities/ELessonBlock/LessonBlockController.test.cs) - **integrity**  
  Check if returns lesson blocks when a valid student id provided

- [GetAllLessonBlocksReturnsLessonBlocksFromSubgroupIfSubgroupIdIsProvided()](../Entities/ELessonBlock/LessonBlockController.test.cs) - **integrity**  
  Check if returns lesson blocks when a valid group id provided

- [GetAllLessonBlocksReturnsLessonBlocksFromScheduleIfScheduleIdIsProvided()](../Entities/ELessonBlock/LessonBlockController.test.cs) - **integrity**  
  Check if returns lesson blocks when a valid schedule id provided

- [GetAllLessonBlocksReturnsLessonBlocksFromClassroomIfClassroomIdIsProvided()](../Entities/ELessonBlock/LessonBlockController.test.cs) - **integrity**  
  Check if returns lesson blocks when a valid classroom id provided

- [GetAllLessonBlocksReturnsLessonBlocksFromLessonIfLessonIdIsProvided()](../Entities/ELessonBlock/LessonBlockController.test.cs) - **integrity**  
  Check if returns lesson blocks when a valid lesson id provided

- [GetAllLessonBlocksReturnsLessonBlocksFromTeacherIfTeacherIdIsProvided()](../Entities/ELessonBlock/LessonBlockController.test.cs) - **integrity**  
  Check if returns lesson blocks when a valid teacher id provided

- [GetAllLessonBlocksReturnsLessonBlocksFromClsterIfClusterIdIsProvided()](../Entities/ELessonBlock/LessonBlockController.test.cs) - **integrity**  
  Check if returns lesson blocks when a valid classroom id provided

- [GetAllLessonBlocksReturnsLessonBlocksFromProvidedDateRange()](../Entities/ELessonBlock/LessonBlockController.test.cs) - **integrity**  
  Check if returns lesson blocks when a valid date range is provided

- [GetAllLessonBlocksReturnsEmptyContentWhenWrongUserAttemptsGet()](../Entities/ELessonBlock/LessonBlockController.test.cs) - **integrity**  
  Check if returns no lesson blocks when other user's token is provided

- [GetAllLessonBlocksReturnsEmptyContentWhenWrongIdIsGiven()](../Entities/ELessonBlock/LessonBlockController.test.cs) - **integrity**  
  Check if returns no lesson blocks when wrong group id is provided

- [ThrowsErrorWhenIncorrectPerPageIsGiven()](../Entities/ELessonBlock/Queries/GetAllLessonBlock.unit.cs) - **unit**  
  Check if returns an error when provided with invalid perPage

- [ThrowsErrorWhenIncorrectPageIsGiven()](../Entities/ELessonBlock/Queries/GetAllLessonBlock.unit.cs) - **unit**  
  Check if returns an error when provided with invalid page

- [ThrowsErrorWhenIncorrectSortByIsGiven()](../Entities/ELessonBlock/Queries/GetAllLessonBlock.unit.cs) - **unit**  
  Check if returns an error when provided with invalid sortBy

- [ThrowsErrorWhenIncorrectSortOrderIsGiven()](../Entities/ELessonBlock/Queries/GetAllLessonBlock.unit.cs) - **unit**  
  Check if returns an error when provided with invalid sortOrder

- [ThrowsMultipleErrorMessages()](../Entities/ELessonBlock/Queries/GetAllLessonBlock.unit.cs) - **unit**  
  Check if returns multiple errors when provided with multiple invalid parameters

- [ThrowsErrorWhenFromDateIsAfterToDate()](../Entities/ELessonBlock/Queries/GetAllLessonBlock.unit.cs) - **unit**  
  Check if returns an error when incorrect date range is provided

	
## `GET` `api/LessonBlock{id}`

- [GetLessonBlockReturnsLessonBlock()](../Entities/ELessonBlock/LessonBlockController.test.cs) - **integrity**  
  Check if returns a lesson block when a valid token is provided

- [GetLessonBlockThrowsNotFoundErrorWhenWrongUserTokenIsGiven()](../Entities/ELessonBlock/LessonBlockController.test.cs) - **integrity** 
  Check if returns an error when a group is inaccessible for user

- [GetLessonBlockThrowsNotFoundWhenWrongIdIsGiven()](../Entities/ELessonBlock/LessonBlockController.test.cs) - **integrity**  
  Check if returns no days off when wrong id is provided

## `DELETE` `api/LessonBlock/{id}`

- [LessonBlockIsDeleted()](../Entities/ELessonBlock/LessonBlockController.test.cs) - **integrity**  
  Check if lesson block is deleted when a valid token is provided

- [LessonBlockClusterIsDeleted()](../Entities/ELessonBlock/LessonBlockController.test.cs) - **integrity**  
  Check if multiple lesson blocks in a cluster are deleted when a cluster id is provided

- [DeleteLessonBlockUpdatesLessonsCurrentHours()](../Entities/ELessonBlock/LessonBlockController.test.cs) - **integrity**  
  Check if deleting a lesson block updates the current hours of the lesson

## `PATCH` `api/LessonBlock/{id}`

- [UpdateLessonBlockReturnsId()](../Entities/ELessonBlock/LessonBlockController.test.cs) - **integrity**  
  Check if returns an id when provided with correct data

- [UpdateLessonBlockUpdatesLessonBlockCluster()](../Entities/EClassroom/ClassroomController.test.cs) - **integrity**  
  Check if updates lesson block cluster  when provided with cluster id and update cluster is true

- [UpdateLessonBlockUpdatesLessonsCurrentHours()](../Entities/EClassroom/ClassroomController.test.cs) - **integrity**  
   Check if updating a lesson block updates the current hours of the lesson

- [UpdateLessonBlockThrowsNotFoundErrorWhenWrongIdIsGiven()](../Entities/ELessonBlock/LessonBlockController.test.cs) - **integrity**  
  Check if returns an error when day off doesn't exists

- [UpdateLessonBlockThrowsNotFoundErrorWhenWrongUserAttemptsUpdate()](../Entities/ELessonBlock/LessonBlockController.test.cs) - **integrity**  
  Check if returns an error when day off is inaccessible for user

- [ThrowsErrorWhenWrongTeacherIdIsGiven()](../Entities/ELessonBlock/UpdateLessonBlockCommand.unit.cs) - **unit** 
  Check if returns an error when incorrect teacher id is provided

- [ThrowsErrorWhenScheduleIdsFromLessonAndTeacherDontMatch()](../Entities/ELessonBlock/UpdateLessonBlockCommand.unit.cs) - **unit** 
  Check if returns an error when lesson and teacher are from different schedules

- [ThrowsErrorWhenWrongClassroomIdIsGiven()](../Entities/ELessonBlock/UpdateLessonBlockCommand.unit.cs) - **unit** 
  Check if returns an error when incorrect classroom id is provided

- [ThrowsErrorWhenScheduleIdsFromLessonAndClassroomDontMatch()](../Entities/ELessonBlock/UpdateLessonBlockCommand.unit.cs) - **unit** 
  Check if returns an error when lesson and classroom are from different schedules

- [ThrowsErrorWhenLessonStartIsAfterLessonEnd()](../Entities/ELessonBlock/Commands/UpdateLessonBlockCommand.unit.cs) - **unit** 
  Check if returns an error when incorrect lesson start and end times are provided

- [ThrowsErrorWhenLessonStartIsLessThan1()](../Entities/ELessonBlock/Commands/UpdateLessonBlockCommand.unit.cs) - **unit** 
  Check if returns an error when lesson start is less than one

- [ThrowsErrorWhenLessonStartIsMoreThanTheAmountOfLessonPeriods()](../Entities/ELessonBlock/Commands/UpdateLessonBlockCommand.unit.cs) - **unit** 
  Check if returns an error when lesson start is more than the amount of lesson periods

- [ThrowsErrorWhenLessonEndIsLessThan1()](../Entities/ELessonBlock/Commands/UpdateLessonBlockCommand.unit.cs) - **unit** 
  Check if returns an error when lesson end is less than one

- [ThrowsErrorWhenLessonEndIsMoreThanTheAmountOfLessonPeriods()](../Entities/ELessonBlock/Commands/UpdateLessonBlockCommand.unit.cs) - **unit** 
  Check if returns an error when lesson end is more than the amount of lesson periods

- [ThrowsErrorWhenLessonOccursOnADayOfTheWeekThatIsNotAllowedByScheduleSettings()](../Entities/ELessonBlock/Commands/UpdateLessonBlockCommand.unit.cs) - **unit** 
  Check if returns an error when lesson occurs on a day of the week that is not allowed by schedule settings

- [ThrowsErrorWhenUpdatedLessonDateWouldOccurBeforeSchoolYearStart()](../Entities/ELessonBlock/Commands/UpdateLessonBlockCommand.unit.cs) - **unit** 
  Check if returns an error when updated lesson date would occur before the start of the school year

- [ThrowsErrorWhenUpdatedLessonDateWouldOccurAfterSchoolYearEnd()](../Entities/ELessonBlock/Commands/UpdateLessonBlockCommand.unit.cs) - **unit** 
  Check if returns an error when updated lesson date would occur after the end of the school year




