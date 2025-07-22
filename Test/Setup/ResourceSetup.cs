using AlpimiAPI.Locales;
using Microsoft.Extensions.Localization;
using Moq;

namespace AlpimiTest.TestSetup
{
    public static class ResourceSetup
    {
        private static readonly Mock<IStringLocalizer<Errors>> _str = new();
        private static readonly Mock<IStringLocalizer<Fields>> _strFields = new();

        private static readonly Dictionary<string, string> locolaizedFieldErrors = new Dictionary<
            string,
            string
        >
        {
            { "SchoolDays", "School Days" },
            { "WeekInterval", "Week Interval" },
            { "LessonEnd", "Lesson End" },
            { "LessonStart", "Lesson Start" },
            { "AmountOfHours", "Amount Of Hours" },
            { "SortOrder", "Sort Order" },
            { "Page", "Page" },
            { "PerPage", "Per Page" },
            { "SortBy", "Sort By" },
            { "OrderBy", "Oder by" },
            { "Description", "Description" },
            { "Login", "Login" },
            { "Password", "Password" },
            { "CustomUrl", "Custom URL" },
            { "Timestamp", "Timestamp" },
            { "AffectedEntity", "Affected Entity" },
            { "Command", "Command" },
            { "ReversalId", "Reversal ID" },
            { "IsUndone", "Is Undone" },
            { "IsUndoneChecked", "Is Undone Checked" },
            { "SchoolHour", "School Hour" },
            { "SchoolYearStart", "School Year Start" },
            { "SchoolYearEnd", "School Year End" },
            { "From", "From" },
            { "To", "To" },
            { "StartTime", "Start Time" },
            { "Weight", "Weight" },
            { "Filter", "Filter" },
            { "Category", "Category" },
            { "Capacity", "Capacity" },
            { "Color", "Color" },
            { "WeekDay", "Week Day" },
            { "Start", "Start" },
            { "End", "End" },
            { "Surname", "Surname" },
            { "StudentCount", "Student Count" },
            { "AlbumNumber", "Album Number" },
            { "CollidingObject1", "Colliding Object 1" },
            { "CollidingObject2", "Colliding Object 2" },
            { "Ignored", "Ignored" },
            { "Subgroup", "Subgroup" },
            { "ClassroomType", "Classroom Type" },
            { "LessonPeriod", "Lesson Period" },
            { "Auth", "Auth" },
            { "User", "User" },
            { "Schedule", "Schedule" },
            { "History", "History" },
            { "ScheduleSettings", "Schedule Settings" },
            { "DayOff", "Day Off" },
            { "CollisionType", "Collision Type" },
            { "Collision", "Collision" },
            { "Classroom", "Classroom" },
            { "LessonType", "Lesson Type" },
            { "Lesson", "Lesson" },
            { "Teacher", "Teacher" },
            { "Availability", "Availability" },
            { "Group", "Group" },
            { "Student", "Student" },
            { "LessonBlock", "Lesson Block" }
        };
        private static readonly Dictionary<string, string> localizedErrorsWithoutArgs =
            new Dictionary<string, string>
            {
                { "invalidLoginOrPassword", "Invalid login or password" },
                { "invalidPassword", "Invalid password" },
                { "scheduleDate", "The end date cannot happen before the start date" },
                { "scheduleTime", "The end time cannot happen before the start time" },
                {
                    "tooManyStudents",
                    "Student count in a subgroup cannot be greater than the student count in a group"
                },
                { "cantUndo", "There is nothing to undo" },
                { "cantRedo", "There is nothing to redo" }
            };
        private static readonly Dictionary<
            string,
            Func<object[], LocalizedString>
        > localizedErrorsWithArgs = new Dictionary<string, Func<object[], LocalizedString>>
        {
            {
                "badParameter",
                args => new LocalizedString(
                    "badParameter",
                    string.Format("{0} parameter is invalid", args[0])
                )
            },
            {
                "outOfRange",
                args => new LocalizedString(
                    "outOfRange",
                    string.Format(
                        "There are {0} outside of provided range. Please change them first",
                        args[0]
                    )
                )
            },
            {
                "resourceNotFound",
                args => new LocalizedString(
                    "resourceNotFound",
                    string.Format("{0} with id {1} was not found", args[0], args[1])
                )
            },
            {
                "longPassword",
                args => new LocalizedString(
                    "longPassword",
                    string.Format("Password cannot be longer than {0} characters", args[0])
                )
            },
            {
                "shortPassword",
                args => new LocalizedString(
                    "shortPassword",
                    string.Format("Password cannot be shorter than {0} characters", args[0])
                )
            },
            {
                "timeOverlap",
                args => new LocalizedString(
                    "timeOverlap",
                    string.Format("{0}s cannot overlap", args[0])
                )
            },
            {
                "alreadyExists",
                args => new LocalizedString(
                    "alreadyExists",
                    string.Format("There is already a {0} with the name {1}", args[0], args[1])
                )
            },
            {
                "passwordMustContain",
                args => new LocalizedString(
                    "passwordMustContain",
                    string.Format(
                        "Password must contain at least one of the following: {0}",
                        args[0]
                    )
                )
            },
            {
                "badWeekDay",
                args => new LocalizedString(
                    "badWeekDay",
                    string.Format("Lessons cannot occur on {0}", args[0])
                )
            },
            {
                "scheduleDuration",
                args => new LocalizedString(
                    "scheduleDuration",
                    string.Format("The school year cannot be longer than {0} month(s)", args[0])
                )
            },
            {
                "dateOutOfRange",
                args => new LocalizedString(
                    "dateOutofRange",
                    string.Format("Date must be in between {0} and {1}", args[0], args[1])
                )
            },
            {
                "duplicateData",
                args => new LocalizedString(
                    "duplicateData",
                    string.Format("Cannot add multiple {0} with the value {1}", args[0], args[1])
                )
            },
            {
                "wrongSet",
                args => new LocalizedString(
                    "wrongSet",
                    string.Format("{0} must be in the same {1} as {2}", args[0], args[1], args[2])
                )
            },
            {
                "cantContain",
                args => new LocalizedString(
                    "cantContain",
                    string.Format("{0} can only contain the following: {1}", args[0], args[1])
                )
            }
        };

        public static Mock<IStringLocalizer<Errors>> Setup()
        {
            _str.Setup(localizer => localizer[It.IsAny<string>(), It.IsAny<object[]>()])
                .Returns(
                    (string key, object[] args) =>
                    {
                        return localizedErrorsWithArgs[key](args);
                    }
                );

            _str.Setup(localizer => localizer[It.IsAny<string>()])
                .Returns(
                    (string key) =>
                    {
                        return new LocalizedString(key, localizedErrorsWithoutArgs[key]);
                    }
                );

            return _str;
        }

        public static Mock<IStringLocalizer<Fields>> FieldSetup()
        {
            _strFields
                .Setup(localizer => localizer[It.IsAny<string>()])
                .Returns(
                    (string key) =>
                    {
                        return new LocalizedString(key, locolaizedFieldErrors[key]);
                    }
                );

            return _strFields;
        }
    }
}
