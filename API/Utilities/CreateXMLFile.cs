using System.Xml.Linq;
using AlpimiAPI.Database;
using AlpimiAPI.Entities.EAvailability;
using AlpimiAPI.Entities.EClassroom;
using AlpimiAPI.Entities.EClassroomType;
using AlpimiAPI.Entities.EClassroomType.Queries;
using AlpimiAPI.Entities.EDayOff;
using AlpimiAPI.Entities.EGroup;
using AlpimiAPI.Entities.ELesson;
using AlpimiAPI.Entities.ELessonPeriod;
using AlpimiAPI.Entities.ELessonType;
using AlpimiAPI.Entities.EStudent;
using AlpimiAPI.Entities.ESubgroup;
using AlpimiAPI.Entities.ESubgroup.Queries;
using AlpimiAPI.Entities.ETeacher;
using AlpimiAPI.Locales;
using AlpimiAPI.Responses;
using Microsoft.Extensions.Localization;
using Moq;

namespace AlpimiAPI.Utilities
{
    public static class CreateXMLFile
    {
        public static async Task<string> create(
            IDbService dbService,
            IStringLocalizer<Data> _strData,
            IEnumerable<DayOff> daysOff,
            IEnumerable<LessonPeriod> lessonPeriods,
            IEnumerable<ClassroomType> classroomTypes,
            IEnumerable<Lesson> lessons,
            IEnumerable<Group> groups,
            IEnumerable<Subgroup> subgroups,
            IEnumerable<Availability> availabilities,
            IEnumerable<Teacher> teachers,
            IEnumerable<LessonType> lessonTypes,
            IEnumerable<Student> students,
            IEnumerable<Classroom> classrooms
        )
        {
            XNamespace ss = "urn:schemas-microsoft-com:office:spreadsheet";
            XNamespace o = "urn:schemas-microsoft-com:office:office";
            XNamespace x = "urn:schemas-microsoft-com:office:excel";
            XNamespace html = "http://www.w3.org/TR/REC-html40";

            var classroomRowElements = new List<XElement>();
            foreach (var classroom in classrooms)
            {
                classroomRowElements.Add(
                    new XElement(
                        ss + "Row",
                        new XElement(
                            ss + "Cell",
                            new XElement(
                                ss + "Data",
                                new XAttribute(ss + "Type", "String"),
                                classroom.Name
                            )
                        ),
                        new XElement(
                            ss + "Cell",
                            new XElement(
                                ss + "Data",
                                new XAttribute(ss + "Type", "Number"),
                                classroom.Capacity
                            )
                        ),
                        new XElement(
                            ss + "Cell",
                            new XElement(
                                ss + "Data",
                                new XAttribute(ss + "Type", "String"),
                                await GetSeperatedBySemicolonClassroomTypes(classroom.Id, dbService)
                            )
                        )
                    )
                );
            }

            var lessonRowElements = new List<XElement>();
            foreach (var lesson in lessons)
            {
                lessonRowElements.Add(
                    new XElement(
                        ss + "Row",
                        new XElement(
                            ss + "Cell",
                            new XElement(
                                ss + "Data",
                                new XAttribute(ss + "Type", "String"),
                                lesson.Name
                            )
                        ),
                        new XElement(
                            ss + "Cell",
                            new XElement(
                                ss + "Data",
                                new XAttribute(ss + "Type", "Number"),
                                lesson.AmountOfHours
                            )
                        ),
                        new XElement(
                            ss + "Cell",
                            new XElement(
                                ss + "Data",
                                new XAttribute(ss + "Type", "String"),
                                lesson.LessonType.Name
                            )
                        ),
                        new XElement(
                            ss + "Cell",
                            new XElement(
                                ss + "Data",
                                new XAttribute(ss + "Type", "String"),
                                lesson.Teacher.Email
                            )
                        ),
                        new XElement(
                            ss + "Cell",
                            new XElement(
                                ss + "Data",
                                new XAttribute(ss + "Type", "String"),
                                await GetSeperatedBySemicolonClassroomTypes(lesson.Id, dbService)
                            )
                        ),
                        new XElement(
                            ss + "Cell",
                            new XElement(
                                ss + "Data",
                                new XAttribute(ss + "Type", "String"),
                                await GetSeperatedBySemicolonGroupsAndSubgroups(
                                    lesson.Id,
                                    dbService
                                )
                            )
                        )
                    )
                );
            }

            var studentRowElements = new List<XElement>();
            foreach (var student in students)
            {
                studentRowElements.Add(
                    new XElement(
                        ss + "Row",
                        new XElement(
                            ss + "Cell",
                            new XElement(
                                ss + "Data",
                                new XAttribute(ss + "Type", "String"),
                                student.AlbumNumber
                            )
                        ),
                        new XElement(
                            ss + "Cell",
                            new XElement(
                                ss + "Data",
                                new XAttribute(ss + "Type", "String"),
                                student.Group.Name
                            )
                        ),
                        new XElement(
                            ss + "Cell",
                            new XElement(
                                ss + "Data",
                                new XAttribute(ss + "Type", "String"),
                                (
                                    (
                                        await GetSeperatedBySemicolonGroupsAndSubgroups(
                                            student.Id,
                                            dbService
                                        )
                                    ).Replace(student.Group.Name + "/", "")
                                )
                            )
                        )
                    )
                );
            }

            var workbook = new XDocument(
                new XDeclaration("1.0", null, null),
                new XProcessingInstruction("mso-application", "progid=\"Excel.Sheet\""),
                new XElement(
                    ss + "Workbook",
                    new XAttribute(XNamespace.Xmlns + "o", o),
                    new XAttribute(XNamespace.Xmlns + "x", x),
                    new XAttribute(XNamespace.Xmlns + "ss", ss),
                    new XAttribute(XNamespace.Xmlns + "html", html),
                    new XElement(
                        o + "DocumentProperties",
                        new XElement("Author", "Alpimi"),
                        new XElement("LastAuthor", "Export"),
                        new XElement("Created", DateTime.UtcNow.ToString("yyyy-MM-ddTHH:mm:ssZ")),
                        new XElement("LastSaved", DateTime.UtcNow.ToString("yyyy-MM-ddTHH:mm:ssZ")),
                        new XElement("Version", "16.00")
                    ),
                    new XElement(
                        x + "ExcelWorkbook",
                        new XElement("WindowHeight", "10635"),
                        new XElement("WindowWidth", "25005"),
                        new XElement("WindowTopX", "32767"),
                        new XElement("WindowTopY", "32767"),
                        new XElement("ProtectStructure", "False"),
                        new XElement("ProtectWindows", "False")
                    ),
                    new XElement(
                        ss + "Styles",
                        new XElement(
                            ss + "Style",
                            new XAttribute(ss + "ID", "Default"),
                            new XAttribute(ss + "Name", "Normal"),
                            new XElement(
                                ss + "Alignment",
                                new XAttribute(ss + "Vertical", "Bottom")
                            ),
                            new XElement(ss + "Borders"),
                            new XElement(
                                ss + "Font",
                                new XAttribute(ss + "FontName", "Arial"),
                                new XAttribute(x + "CharSet", "238"),
                                new XAttribute(x + "Family", "Swiss"),
                                new XAttribute(ss + "Size", "11"),
                                new XAttribute(ss + "Color", "#000000")
                            ),
                            new XElement(ss + "Interior"),
                            new XElement(ss + "NumberFormat"),
                            new XElement(ss + "Protection")
                        ),
                        new XElement(
                            ss + "Style",
                            new XAttribute(ss + "ID", "s18"),
                            new XElement(
                                ss + "Alignment",
                                new XAttribute(ss + "Horizontal", "Left"),
                                new XAttribute(ss + "Vertical", "Center")
                            ),
                            new XElement(
                                ss + "Font",
                                new XAttribute(ss + "FontName", "Arial"),
                                new XAttribute(x + "CharSet", "238"),
                                new XAttribute(x + "Family", "Swiss"),
                                new XAttribute(ss + "Size", "11"),
                                new XAttribute(ss + "Color", "#FFFFFF"),
                                new XAttribute(ss + "Bold", "1")
                            ),
                            new XElement(
                                ss + "Interior",
                                new XAttribute(ss + "Color", "#70AD47"),
                                new XAttribute(ss + "Pattern", "Solid")
                            )
                        ),
                        new XElement(
                            ss + "Style",
                            new XAttribute(ss + "ID", "s19"),
                            new XElement(
                                ss + "Font",
                                new XAttribute(ss + "FontName", "Arial"),
                                new XAttribute(x + "CharSet", "238"),
                                new XAttribute(x + "Family", "Swiss"),
                                new XAttribute(ss + "Size", "11"),
                                new XAttribute(ss + "Color", "#000000")
                            ),
                            new XElement(
                                ss + "NumberFormat",
                                new XAttribute(ss + "Format", "hh:mm:ss")
                            )
                        )
                    ),
                    new XElement(
                        ss + "Worksheet",
                        new XAttribute(ss + "Name", _strData["LessonPeriods"]),
                        new XElement(
                            ss + "Table",
                            new XAttribute(ss + "DefaultRowHeight", "14.25"),
                            new XElement(
                                ss + "Column",
                                new XAttribute(ss + "AutoFitWidth", "0"),
                                new XAttribute(ss + "Width", "295.5")
                            ),
                            new XElement(
                                ss + "Row",
                                new XAttribute(ss + "AutoFitHeight", "0"),
                                new XAttribute(ss + "Height", "24"),
                                new XElement(
                                    ss + "Cell",
                                    new XAttribute(ss + "StyleID", "s18"),
                                    new XElement(
                                        ss + "Data",
                                        new XAttribute(ss + "Type", "String"),
                                        _strData["LessonBlockStartHour"]
                                    )
                                )
                            ),
                            from lessonPeriod in lessonPeriods
                            select new XElement(
                                ss + "Row",
                                new XElement(
                                    ss + "Cell",
                                    new XAttribute(ss + "StyleID", "s19"),
                                    new XElement(
                                        ss + "Data",
                                        new XAttribute(ss + "Type", "DateTime"),
                                        new DateTime(1899, 12, 31)
                                            .Add(lessonPeriod.Start.ToTimeSpan())
                                            .ToString("yyyy-MM-ddTHH:mm:ss.fff")
                                    )
                                )
                            )
                        )
                    ),
                    new XElement(
                        ss + "Worksheet",
                        new XAttribute(ss + "Name", _strData["DaysOff"]),
                        new XElement(
                            ss + "Table",
                            new XAttribute(ss + "DefaultRowHeight", "14.25"),
                            new XElement(
                                ss + "Column",
                                new XAttribute(ss + "AutoFitWidth", "0"),
                                new XAttribute(ss + "Width", "171")
                            ),
                            new XElement(
                                ss + "Column",
                                new XAttribute(ss + "AutoFitWidth", "0"),
                                new XAttribute(ss + "Width", "188.25")
                            ),
                            new XElement(
                                ss + "Column",
                                new XAttribute(ss + "AutoFitWidth", "0"),
                                new XAttribute(ss + "Width", "187.5")
                            ),
                            new XElement(
                                ss + "Row",
                                new XAttribute(ss + "AutoFitHeight", "0"),
                                new XAttribute(ss + "Height", "24"),
                                new XElement(
                                    ss + "Cell",
                                    new XAttribute(ss + "StyleID", "s18"),
                                    new XElement(
                                        ss + "Data",
                                        new XAttribute(ss + "Type", "String"),
                                        _strData["DayOffName"]
                                    )
                                ),
                                new XElement(
                                    ss + "Cell",
                                    new XAttribute(ss + "StyleID", "s18"),
                                    new XElement(
                                        ss + "Data",
                                        new XAttribute(ss + "Type", "String"),
                                        _strData["StartDate"]
                                    )
                                ),
                                new XElement(
                                    ss + "Cell",
                                    new XAttribute(ss + "StyleID", "s18"),
                                    new XElement(
                                        ss + "Data",
                                        new XAttribute(ss + "Type", "String"),
                                        _strData["EndDate"]
                                    )
                                )
                            ),
                            from dayOff in daysOff
                            select new XElement(
                                ss + "Row",
                                new XElement(
                                    ss + "Cell",
                                    new XElement(
                                        ss + "Data",
                                        new XAttribute(ss + "Type", "String"),
                                        dayOff.Name
                                    )
                                ),
                                new XElement(
                                    ss + "Cell",
                                    new XElement(
                                        ss + "Data",
                                        new XAttribute(ss + "Type", "String"),
                                        dayOff.From
                                    )
                                ),
                                new XElement(
                                    ss + "Cell",
                                    new XElement(
                                        ss + "Data",
                                        new XAttribute(ss + "Type", "String"),
                                        dayOff.To
                                    )
                                )
                            )
                        )
                    ),
                    new XElement(
                        ss + "Worksheet",
                        new XAttribute(ss + "Name", _strData["ClassroomTypes"]),
                        new XElement(
                            ss + "Table",
                            new XAttribute(ss + "DefaultRowHeight", "14.25"),
                            new XElement(
                                ss + "Column",
                                new XAttribute(ss + "AutoFitWidth", "0"),
                                new XAttribute(ss + "Width", "155.25")
                            ),
                            new XElement(
                                ss + "Row",
                                new XAttribute(ss + "AutoFitHeight", "0"),
                                new XAttribute(ss + "Height", "24"),
                                new XElement(
                                    ss + "Cell",
                                    new XAttribute(ss + "StyleID", "s18"),
                                    new XElement(
                                        ss + "Data",
                                        new XAttribute(ss + "Type", "String"),
                                        _strData["ClassroomTypeName"]
                                    )
                                )
                            ),
                            from classroomType in classroomTypes
                            select new XElement(
                                ss + "Row",
                                new XElement(
                                    ss + "Cell",
                                    new XElement(
                                        ss + "Data",
                                        new XAttribute(ss + "Type", "String"),
                                        classroomType.Name
                                    )
                                )
                            )
                        )
                    ),
                    new XElement(
                        ss + "Worksheet",
                        new XAttribute(ss + "Name", _strData["LessonTypes"]),
                        new XElement(
                            ss + "Table",
                            new XAttribute(ss + "DefaultRowHeight", "14.25"),
                            new XElement(
                                ss + "Column",
                                new XAttribute(ss + "AutoFitWidth", "0"),
                                new XAttribute(ss + "Width", "138")
                            ),
                            new XElement(
                                ss + "Column",
                                new XAttribute(ss + "AutoFitWidth", "0"),
                                new XAttribute(ss + "Width", "200")
                            ),
                            new XElement(
                                ss + "Row",
                                new XAttribute(ss + "AutoFitHeight", "0"),
                                new XAttribute(ss + "Height", "24"),
                                new XElement(
                                    ss + "Cell",
                                    new XAttribute(ss + "StyleID", "s18"),
                                    new XElement(
                                        ss + "Data",
                                        new XAttribute(ss + "Type", "String"),
                                        _strData["LessonTypeName"]
                                    )
                                ),
                                new XElement(
                                    ss + "Cell",
                                    new XAttribute(ss + "StyleID", "s18"),
                                    new XElement(
                                        ss + "Data",
                                        new XAttribute(ss + "Type", "String"),
                                        _strData["Color"]
                                    )
                                )
                            ),
                            from lessonType in lessonTypes
                            select new XElement(
                                ss + "Row",
                                new XElement(
                                    ss + "Cell",
                                    new XElement(
                                        ss + "Data",
                                        new XAttribute(ss + "Type", "String"),
                                        lessonType.Name
                                    )
                                ),
                                new XElement(
                                    ss + "Cell",
                                    new XElement(
                                        ss + "Data",
                                        new XAttribute(ss + "Type", "Number"),
                                        lessonType.Color
                                    )
                                )
                            )
                        )
                    ),
                    new XElement(
                        ss + "Worksheet",
                        new XAttribute(ss + "Name", _strData["Teachers"]),
                        new XElement(
                            ss + "Table",
                            new XAttribute(ss + "DefaultRowHeight", "14.25"),
                            new XElement(
                                ss + "Column",
                                new XAttribute(ss + "AutoFitWidth", "0"),
                                new XAttribute(ss + "Width", "105")
                            ),
                            new XElement(
                                ss + "Column",
                                new XAttribute(ss + "AutoFitWidth", "0"),
                                new XAttribute(ss + "Width", "87")
                            ),
                            new XElement(
                                ss + "Column",
                                new XAttribute(ss + "AutoFitWidth", "0"),
                                new XAttribute(ss + "Width", "96.75")
                            ),
                            new XElement(
                                ss + "Row",
                                new XAttribute(ss + "AutoFitHeight", "0"),
                                new XAttribute(ss + "Height", "24"),
                                new XElement(
                                    ss + "Cell",
                                    new XAttribute(ss + "StyleID", "s18"),
                                    new XElement(
                                        ss + "Data",
                                        new XAttribute(ss + "Type", "String"),
                                        _strData["Email"]
                                    )
                                ),
                                new XElement(
                                    ss + "Cell",
                                    new XAttribute(ss + "StyleID", "s18"),
                                    new XElement(
                                        ss + "Data",
                                        new XAttribute(ss + "Type", "String"),
                                        _strData["Name"]
                                    )
                                ),
                                new XElement(
                                    ss + "Cell",
                                    new XAttribute(ss + "StyleID", "s18"),
                                    new XElement(
                                        ss + "Data",
                                        new XAttribute(ss + "Type", "String"),
                                        _strData["Surname"]
                                    )
                                )
                            ),
                            from teacher in teachers
                            select new XElement(
                                ss + "Row",
                                new XElement(
                                    ss + "Cell",
                                    new XElement(
                                        ss + "Data",
                                        new XAttribute(ss + "Type", "String"),
                                        teacher.Email
                                    )
                                ),
                                new XElement(
                                    ss + "Cell",
                                    new XElement(
                                        ss + "Data",
                                        new XAttribute(ss + "Type", "String"),
                                        teacher.Name
                                    )
                                ),
                                new XElement(
                                    ss + "Cell",
                                    new XElement(
                                        ss + "Data",
                                        new XAttribute(ss + "Type", "String"),
                                        teacher.Surname
                                    )
                                )
                            )
                        )
                    ),
                    new XElement(
                        ss + "Worksheet",
                        new XAttribute(ss + "Name", _strData["Groups"]),
                        new XElement(
                            ss + "Table",
                            new XAttribute(ss + "DefaultRowHeight", "14.25"),
                            new XElement(
                                ss + "Column",
                                new XAttribute(ss + "AutoFitWidth", "0"),
                                new XAttribute(ss + "Width", "84.75")
                            ),
                            new XElement(
                                ss + "Column",
                                new XAttribute(ss + "AutoFitWidth", "0"),
                                new XAttribute(ss + "Width", "97.5")
                            ),
                            new XElement(
                                ss + "Row",
                                new XAttribute(ss + "AutoFitHeight", "0"),
                                new XAttribute(ss + "Height", "24"),
                                new XElement(
                                    ss + "Cell",
                                    new XAttribute(ss + "StyleID", "s18"),
                                    new XElement(
                                        ss + "Data",
                                        new XAttribute(ss + "Type", "String"),
                                        _strData["GroupName"]
                                    )
                                ),
                                new XElement(
                                    ss + "Cell",
                                    new XAttribute(ss + "StyleID", "s18"),
                                    new XElement(
                                        ss + "Data",
                                        new XAttribute(ss + "Type", "String"),
                                        _strData["StudentCount"]
                                    )
                                )
                            ),
                            from @group in groups
                            select new XElement(
                                ss + "Row",
                                new XElement(
                                    ss + "Cell",
                                    new XElement(
                                        ss + "Data",
                                        new XAttribute(ss + "Type", "String"),
                                        @group.Name
                                    )
                                ),
                                new XElement(
                                    ss + "Cell",
                                    new XElement(
                                        ss + "Data",
                                        new XAttribute(ss + "Type", "Number"),
                                        @group.StudentCount
                                    )
                                )
                            )
                        )
                    ),
                    new XElement(
                        ss + "Worksheet",
                        new XAttribute(ss + "Name", _strData["Subgroups"]),
                        new XElement(
                            ss + "Table",
                            new XAttribute(ss + "DefaultRowHeight", "14.25"),
                            new XElement(
                                ss + "Column",
                                new XAttribute(ss + "AutoFitWidth", "0"),
                                new XAttribute(ss + "Width", "112.5")
                            ),
                            new XElement(
                                ss + "Column",
                                new XAttribute(ss + "AutoFitWidth", "0"),
                                new XAttribute(ss + "Width", "93.75")
                            ),
                            new XElement(
                                ss + "Column",
                                new XAttribute(ss + "AutoFitWidth", "0"),
                                new XAttribute(ss + "Width", "87.75")
                            ),
                            new XElement(
                                ss + "Row",
                                new XAttribute(ss + "AutoFitHeight", "0"),
                                new XAttribute(ss + "Height", "24"),
                                new XElement(
                                    ss + "Cell",
                                    new XAttribute(ss + "StyleID", "s18"),
                                    new XElement(
                                        ss + "Data",
                                        new XAttribute(ss + "Type", "String"),
                                        _strData["SubgroupName"]
                                    )
                                ),
                                new XElement(
                                    ss + "Cell",
                                    new XAttribute(ss + "StyleID", "s18"),
                                    new XElement(
                                        ss + "Data",
                                        new XAttribute(ss + "Type", "String"),
                                        _strData["StudentCount"]
                                    )
                                ),
                                new XElement(
                                    ss + "Cell",
                                    new XAttribute(ss + "StyleID", "s18"),
                                    new XElement(
                                        ss + "Data",
                                        new XAttribute(ss + "Type", "String"),
                                        _strData["GroupName"]
                                    )
                                )
                            ),
                            from subgroup in subgroups
                            select new XElement(
                                ss + "Row",
                                new XElement(
                                    ss + "Cell",
                                    new XElement(
                                        ss + "Data",
                                        new XAttribute(ss + "Type", "String"),
                                        subgroup.Name
                                    )
                                ),
                                new XElement(
                                    ss + "Cell",
                                    new XElement(
                                        ss + "Data",
                                        new XAttribute(ss + "Type", "Number"),
                                        subgroup.StudentCount
                                    )
                                ),
                                new XElement(
                                    ss + "Cell",
                                    new XElement(
                                        ss + "Data",
                                        new XAttribute(ss + "Type", "String"),
                                        subgroup.Group.Name
                                    )
                                )
                            )
                        )
                    ),
                    new XElement(
                        ss + "Worksheet",
                        new XAttribute(ss + "Name", _strData["Classrooms"]),
                        new XElement(
                            ss + "Table",
                            new XAttribute(ss + "DefaultRowHeight", "14.25"),
                            new XElement(
                                ss + "Column",
                                new XAttribute(ss + "AutoFitWidth", "0"),
                                new XAttribute(ss + "Width", "106.5")
                            ),
                            new XElement(
                                ss + "Column",
                                new XAttribute(ss + "AutoFitWidth", "0"),
                                new XAttribute(ss + "Width", "75.75")
                            ),
                            new XElement(
                                ss + "Column",
                                new XAttribute(ss + "AutoFitWidth", "0"),
                                new XAttribute(ss + "Width", "250")
                            ),
                            new XElement(
                                ss + "Row",
                                new XAttribute(ss + "AutoFitHeight", "0"),
                                new XAttribute(ss + "Height", "24"),
                                new XElement(
                                    ss + "Cell",
                                    new XAttribute(ss + "StyleID", "s18"),
                                    new XElement(
                                        ss + "Data",
                                        new XAttribute(ss + "Type", "String"),
                                        _strData["ClassroomName"]
                                    )
                                ),
                                new XElement(
                                    ss + "Cell",
                                    new XAttribute(ss + "StyleID", "s18"),
                                    new XElement(
                                        ss + "Data",
                                        new XAttribute(ss + "Type", "String"),
                                        _strData["Capacity"]
                                    )
                                ),
                                new XElement(
                                    ss + "Cell",
                                    new XAttribute(ss + "StyleID", "s18"),
                                    new XElement(
                                        ss + "Data",
                                        new XAttribute(ss + "Type", "String"),
                                        _strData["ClassroomTypesSeparated"]
                                    )
                                )
                            ),
                            classroomRowElements
                        )
                    ),
                    new XElement(
                        ss + "Worksheet",
                        new XAttribute(ss + "Name", _strData["Lessons"]),
                        new XElement(
                            ss + "Table",
                            new XAttribute(ss + "DefaultRowHeight", "14.25"),
                            new XElement(
                                ss + "Column",
                                new XAttribute(ss + "AutoFitWidth", "0"),
                                new XAttribute(ss + "Width", "177.75")
                            ),
                            new XElement(
                                ss + "Column",
                                new XAttribute(ss + "AutoFitWidth", "0"),
                                new XAttribute(ss + "Width", "87")
                            ),
                            new XElement(
                                ss + "Column",
                                new XAttribute(ss + "AutoFitWidth", "0"),
                                new XAttribute(ss + "Width", "185.25")
                            ),
                            new XElement(
                                ss + "Column",
                                new XAttribute(ss + "AutoFitWidth", "0"),
                                new XAttribute(ss + "Width", "206.25")
                            ),
                            new XElement(
                                ss + "Column",
                                new XAttribute(ss + "AutoFitWidth", "0"),
                                new XAttribute(ss + "Width", "250")
                            ),
                            new XElement(
                                ss + "Column",
                                new XAttribute(ss + "AutoFitWidth", "0"),
                                new XAttribute(ss + "Width", "400")
                            ),
                            new XElement(
                                ss + "Row",
                                new XAttribute(ss + "AutoFitHeight", "0"),
                                new XAttribute(ss + "Height", "24"),
                                new XElement(
                                    ss + "Cell",
                                    new XAttribute(ss + "StyleID", "s18"),
                                    new XElement(
                                        ss + "Data",
                                        new XAttribute(ss + "Type", "String"),
                                        _strData["LessonName"]
                                    )
                                ),
                                new XElement(
                                    ss + "Cell",
                                    new XAttribute(ss + "StyleID", "s18"),
                                    new XElement(
                                        ss + "Data",
                                        new XAttribute(ss + "Type", "String"),
                                        _strData["LessonHours"]
                                    )
                                ),
                                new XElement(
                                    ss + "Cell",
                                    new XAttribute(ss + "StyleID", "s18"),
                                    new XElement(
                                        ss + "Data",
                                        new XAttribute(ss + "Type", "String"),
                                        _strData["LessonType"]
                                    )
                                ),
                                new XElement(
                                    ss + "Cell",
                                    new XAttribute(ss + "StyleID", "s18"),
                                    new XElement(
                                        ss + "Data",
                                        new XAttribute(ss + "Type", "String"),
                                        _strData["TeachersEmail"]
                                    )
                                ),
                                new XElement(
                                    ss + "Cell",
                                    new XAttribute(ss + "StyleID", "s18"),
                                    new XElement(
                                        ss + "Data",
                                        new XAttribute(ss + "Type", "String"),
                                        _strData["ClassroomTypesSeparated"]
                                    )
                                ),
                                new XElement(
                                    ss + "Cell",
                                    new XAttribute(ss + "StyleID", "s18"),
                                    new XElement(
                                        ss + "Data",
                                        new XAttribute(ss + "Type", "String"),
                                        _strData["SubgroupsSeperated"]
                                    )
                                )
                            ),
                            lessonRowElements
                        )
                    ),
                    new XElement(
                        ss + "Worksheet",
                        new XAttribute(ss + "Name", _strData["Availability"]),
                        new XElement(
                            ss + "Table",
                            new XAttribute(ss + "DefaultRowHeight", "14.25"),
                            new XElement(
                                ss + "Column",
                                new XAttribute(ss + "AutoFitWidth", "0"),
                                new XAttribute(ss + "Width", "128.25")
                            ),
                            new XElement(
                                ss + "Column",
                                new XAttribute(ss + "AutoFitWidth", "0"),
                                new XAttribute(ss + "Width", "117.75")
                            ),
                            new XElement(
                                ss + "Column",
                                new XAttribute(ss + "AutoFitWidth", "0"),
                                new XAttribute(ss + "Width", "130")
                            ),
                            new XElement(
                                ss + "Column",
                                new XAttribute(ss + "AutoFitWidth", "0"),
                                new XAttribute(ss + "Width", "120")
                            ),
                            new XElement(
                                ss + "Row",
                                new XAttribute(ss + "AutoFitHeight", "0"),
                                new XAttribute(ss + "Height", "24"),
                                new XElement(
                                    ss + "Cell",
                                    new XAttribute(ss + "StyleID", "s18"),
                                    new XElement(
                                        ss + "Data",
                                        new XAttribute(ss + "Type", "String"),
                                        _strData["TeachersEmail"]
                                    )
                                ),
                                new XElement(
                                    ss + "Cell",
                                    new XAttribute(ss + "StyleID", "s18"),
                                    new XElement(
                                        ss + "Data",
                                        new XAttribute(ss + "Type", "String"),
                                        _strData["DayOfWeek"]
                                    )
                                ),
                                new XElement(
                                    ss + "Cell",
                                    new XAttribute(ss + "StyleID", "s18"),
                                    new XElement(
                                        ss + "Data",
                                        new XAttribute(ss + "Type", "String"),
                                        _strData["LessonNumberFrom"]
                                    )
                                ),
                                new XElement(
                                    ss + "Cell",
                                    new XAttribute(ss + "StyleID", "s18"),
                                    new XElement(
                                        ss + "Data",
                                        new XAttribute(ss + "Type", "String"),
                                        _strData["LessonNumberTo"]
                                    )
                                )
                            ),
                            from availability in availabilities
                            select new XElement(
                                ss + "Row",
                                new XElement(
                                    ss + "Cell",
                                    new XElement(
                                        ss + "Data",
                                        new XAttribute(ss + "Type", "String"),
                                        availability.Teacher.Email
                                    )
                                ),
                                new XElement(
                                    ss + "Cell",
                                    new XElement(
                                        ss + "Data",
                                        new XAttribute(ss + "Type", "String"),
                                        DayOfWeek.intToDayMap(availability.WeekDay, _strData)
                                    )
                                ),
                                new XElement(
                                    ss + "Cell",
                                    new XElement(
                                        ss + "Data",
                                        new XAttribute(ss + "Type", "Number"),
                                        availability.Start + 1
                                    )
                                ),
                                new XElement(
                                    ss + "Cell",
                                    new XElement(
                                        ss + "Data",
                                        new XAttribute(ss + "Type", "Number"),
                                        availability.End + 1
                                    )
                                )
                            )
                        )
                    ),
                    new XElement(
                        ss + "Worksheet",
                        new XAttribute(ss + "Name", _strData["Students"]),
                        new XElement(
                            ss + "Table",
                            new XAttribute(ss + "DefaultRowHeight", "14.25"),
                            new XElement(
                                ss + "Column",
                                new XAttribute(ss + "AutoFitWidth", "0"),
                                new XAttribute(ss + "Width", "99.75")
                            ),
                            new XElement(
                                ss + "Column",
                                new XAttribute(ss + "AutoFitWidth", "0"),
                                new XAttribute(ss + "Width", "110")
                            ),
                            new XElement(
                                ss + "Column",
                                new XAttribute(ss + "AutoFitWidth", "0"),
                                new XAttribute(ss + "Width", "270")
                            ),
                            new XElement(
                                ss + "Row",
                                new XAttribute(ss + "AutoFitHeight", "0"),
                                new XAttribute(ss + "Height", "24"),
                                new XElement(
                                    ss + "Cell",
                                    new XAttribute(ss + "StyleID", "s18"),
                                    new XElement(
                                        ss + "Data",
                                        new XAttribute(ss + "Type", "String"),
                                        _strData["AlbumNumber"]
                                    )
                                ),
                                new XElement(
                                    ss + "Cell",
                                    new XAttribute(ss + "StyleID", "s18"),
                                    new XElement(
                                        ss + "Data",
                                        new XAttribute(ss + "Type", "String"),
                                        _strData["StudentsGroup"]
                                    )
                                ),
                                new XElement(
                                    ss + "Cell",
                                    new XAttribute(ss + "StyleID", "s18"),
                                    new XElement(
                                        ss + "Data",
                                        new XAttribute(ss + "Type", "String"),
                                        _strData["StudentsSubgroupSeperated"]
                                    )
                                )
                            ),
                            studentRowElements
                        )
                    )
                )
            );
            return workbook.Declaration + Environment.NewLine + workbook.ToString();
        }

        private static async Task<string> GetSeperatedBySemicolonClassroomTypes(
            Guid id,
            IDbService dbService
        )
        {
            var getAllClassroomTypesQuery = new GetAllClassroomTypesQuery(
                id,
                new Guid(),
                "Admin",
                new PaginationParams(int.MaxValue, 0, "Id", "ASC")
            );
            var getAllClassroomTypesHandler = new GetAllClassroomTypesHandler(
                dbService,
                new Mock<IStringLocalizer<Errors>>().Object,
                new Mock<IStringLocalizer<Fields>>().Object
            );

            var classroomTypes = await getAllClassroomTypesHandler.Handle(
                getAllClassroomTypesQuery,
                new CancellationToken()
            );

            var seperatedBySemicolonClassroomTypes = "";
            foreach (var classroomType in classroomTypes.Item1!)
            {
                seperatedBySemicolonClassroomTypes += classroomType.Name + "; ";
            }

            return seperatedBySemicolonClassroomTypes.TrimEnd(' ', ';');
        }

        private static async Task<string> GetSeperatedBySemicolonGroupsAndSubgroups(
            Guid id,
            IDbService dbService
        )
        {
            var getAllSubgroupsQuery = new GetAllSubgroupsQuery(
                id,
                new Guid(),
                "Admin",
                new PaginationParams(int.MaxValue, 0, "Id", "ASC")
            );
            var getAllSubgroupsHandler = new GetAllSubgroupsHandler(
                dbService,
                new Mock<IStringLocalizer<Errors>>().Object,
                new Mock<IStringLocalizer<Fields>>().Object
            );

            var subgroups = await getAllSubgroupsHandler.Handle(
                getAllSubgroupsQuery,
                new CancellationToken()
            );

            var seperatedBySemicolonSubgroups = "";
            foreach (var subgroup in subgroups.Item1!)
            {
                seperatedBySemicolonSubgroups += $"{subgroup.Group.Name}/{subgroup.Name}; ";
            }

            return seperatedBySemicolonSubgroups.TrimEnd(' ', ';');
        }
    }
}
