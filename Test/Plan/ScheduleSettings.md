# ScheduleSettings entity test plan

## `ALL` `api/ScheduleSettings/*`

- [ScheduleControllerThrowsTooManyRequests()](../Entities/EScheduleSettings/ScheduleSettingsController.test.cs) - **integrity**  
  Check if returns an error when request is sent too many times

- [ScheduleSettingsControllerThrowsUnauthorized()](../Entities/EScheduleSettings/ScheduleSettingsController.test.cs) - **integrity**  
  Check if returns an error when token is not provided

## `PATCH` `api/ScheduleSettings/{scheduleId}`

- [UpdateScheduleSettingsReturnsUpdatedSchedule()](../EntitiesEScheduleSettings/ScheduleSettingsController.test.cs) - **integrity**  			
  Check if returns an updated schedule settings when provided with correct data

- [UpdateScheduleSettingsThrowsNotFoundErrorWhenWrongUserAttemptsUpdate()](../Entities/EScheduleSettings/ScheduleSettingsController.test.cs) - **integrity**  
  Check if returns an error when schedule settings are inaccessible for user

- [UpdateScheduleSettingsThrowsNotFoundErrorWhenWrongIdIsGiven()](../Entities/EScheduleSettings/ScheduleSettingsController.test.cs) - **integrity**
  Check if returns an error when schedule settings don't exists

- [ThrowsErrorWhenDateStartIsAfterDateEnd()](../Entities/EScheduleSettings/Commands/UpdateEScheduleSettingsCommand.unit.cs) - **unit**  
  Check if returns an error when incorrect dates are provided

- [ThrowsErrorDaysOffAreOutsideOfDateRange()](../Entities/EScheduleSettings/Commands/UpdateEScheduleSettingsCommand.unit.cs) - **unit**  
  Check if returns an error when a day off is outside of new range
  
## `GET` `api/ScheduleSettings/bySchedule/{scheduleId}`

- [GetScheduleSettingsByScheduleIdReturnsScheduleSettings()](../Entities/EScheduleSettings/ScheduleSettingsController.test.cs) - **integrity**
  Check if returns schedule settings when a valid token is provided

- [GetScheduleSettingsByScheduleIdThrowsNotFoundErrorWhenWrongUserAttemptsUpdate()](../Entities/EScheduleSettings/ScheduleSettingsController.test.cs) - **integrity**  
  Check if returns an error when schedule settings are inaccessible for user

- [GetScheduleSettingsByScheduleIdThrowsNotFoundErrorWhenWrongIdIsGiven()](../Entities/EScheduleSettings/ScheduleSettingsController.test.cs) - **integrity** 	
  Check if returns an error when schedule settings don't exists

## `GET` `api/ScheduleSettings/{id}`

- [GetScheduleSettingsReturnsScheduleSettings()](../Entities/EScheduleSettings/ScheduleSettingsController.test.cs) - **integrity**
  Check if returns schedule settings when a valid token is provided

- [GetScheduleSettingsThrowsNotFoundErrorWhenWrongUserAttemptsUpdate()](../Entities/EScheduleSettings/ScheduleSettingsController.test.cs) - **integrity**  
  Check if returns an error when schedule settings are inaccessible for user

- [GetScheduleSettingsThrowsNotFoundErrorWhenWrongIdIsGiven()](../Entities/EScheduleSettings/ScheduleSettingsController.test.cs) - **integrity** 	
  Check if returns an error when schedule settings don't exists


