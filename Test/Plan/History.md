# History entity test plan

## `ALL` `api/History/*`

- [HistorySettingsControllerThrowsUnauthorized()](../Entities/EHistory/HistoryController.test.cs) - **integrity**  
  Check if returns an error when token is not provided

- [HistoryControllerThrowsTooManyRequests()](../Entities/EHistory/HistoryController.test.cs) - **integrity**  
  Check if returns an error when request is sent too many times

- [ReplacesGuidsIfANewEntityIsCreated()](../Entities/EHistory/HistoryController.test.cs) - **integrity**  
  Check if previous entries in history are updated when a entity is created after undo or redo

## `PATCH` `api/History/undo/{scheduleId}`

- [ThrowsErrorWhenThereAreNoChangesToUndo()](../Entities/EHistory/RevertController.test.cs) - **unit**  
  Check if returns an error when there are no changes to undo

- [UndoThrowsErrorWhenBadScheduleIdIsGiven()](../Entities/EHistory/HistoryController.test.cs) - **integrity** 
  Check if returns an error when bad schedule id is given

- [UndoThrowsErrorWhenWrongUserAttemptsToUndo()](../Entities/EHistory/HistoryController.test.cs) - **integrity** 
  Check if returns an error when wrong user attempts to undo

## `PATCH` `api/History/redo/{scheduleId}`

- [ThrowsErrorWhenThereAreNoChangesToredo()](../Entities/EHistory/RevertController.test.cs) - **unit**  
  Check if returns an error when there are no changes to redo

- [RedoThrowsErrorWhenBadScheduleIdIsGiven()](../Entities/EHistory/HistoryController.test.cs) - **integrity** 
  Check if returns an error when bad schedule id is given

- [RedoThrowsErrorWhenWrongUserAttemptsToUndo()](../Entities/EHistory/HistoryController.test.cs) - **integrity** 
  Check if returns an error when wrong user attempts to redo



