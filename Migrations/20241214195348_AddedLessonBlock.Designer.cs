﻿// <auto-generated />
using System;
using AlpimiAPI.Database;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

#nullable disable

namespace alpimi_planner_backend.Migrations
{
    [DbContext(typeof(Context))]
    [Migration("20241214195348_AddedLessonBlock")]
    partial class AddedLessonBlock
    {
        /// <inheritdoc />
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "8.0.7")
                .HasAnnotation("Relational:MaxIdentifierLength", 128);

            SqlServerModelBuilderExtensions.UseIdentityColumns(modelBuilder);

            modelBuilder.Entity("AlpimiAPI.Entities.EAuth.Auth", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("Password")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Role")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Salt")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<Guid>("UserId")
                        .HasColumnType("uniqueidentifier");

                    b.HasKey("Id");

                    b.HasIndex("UserId");

                    b.ToTable("Auth");
                });

            modelBuilder.Entity("AlpimiAPI.Entities.EClassroom.Classroom", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<int>("Capacity")
                        .HasColumnType("int");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<Guid>("ScheduleId")
                        .HasColumnType("uniqueidentifier");

                    b.HasKey("Id");

                    b.HasIndex("ScheduleId");

                    b.ToTable("Classroom");
                });

            modelBuilder.Entity("AlpimiAPI.Entities.EClassroomType.ClassroomType", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<Guid>("ScheduleId")
                        .HasColumnType("uniqueidentifier");

                    b.HasKey("Id");

                    b.HasIndex("ScheduleId");

                    b.ToTable("ClassroomType");
                });

            modelBuilder.Entity("AlpimiAPI.Entities.EDayOff.DayOff", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<DateOnly>("From")
                        .HasColumnType("date");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<Guid>("ScheduleSettingsId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<DateOnly>("To")
                        .HasColumnType("date");

                    b.HasKey("Id");

                    b.HasIndex("ScheduleSettingsId");

                    b.ToTable("DayOff");
                });

            modelBuilder.Entity("AlpimiAPI.Entities.EGroup.Group", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<Guid>("ScheduleId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<int>("StudentCount")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.HasIndex("ScheduleId");

                    b.ToTable("Group");
                });

            modelBuilder.Entity("AlpimiAPI.Entities.ELesson.Lesson", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<int>("AmountOfHours")
                        .HasColumnType("int");

                    b.Property<int>("CurrentHours")
                        .HasColumnType("int");

                    b.Property<Guid>("LessonTypeId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<Guid>("SubgroupId")
                        .HasColumnType("uniqueidentifier");

                    b.HasKey("Id");

                    b.HasIndex("LessonTypeId");

                    b.HasIndex("SubgroupId");

                    b.ToTable("Lesson");
                });

            modelBuilder.Entity("AlpimiAPI.Entities.ELessonBlock.LessonBlock", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<Guid?>("ClassroomId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<Guid>("ClusterId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<DateOnly>("LessonDate")
                        .HasColumnType("date");

                    b.Property<int>("LessonEnd")
                        .HasColumnType("int");

                    b.Property<Guid>("LessonId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<int>("LessonStart")
                        .HasColumnType("int");

                    b.Property<Guid?>("TeacherId")
                        .HasColumnType("uniqueidentifier");

                    b.HasKey("Id");

                    b.HasIndex("ClassroomId");

                    b.HasIndex("LessonId");

                    b.HasIndex("TeacherId");

                    b.ToTable("LessonBlock");
                });

            modelBuilder.Entity("AlpimiAPI.Entities.ELessonPeriod.LessonPeriod", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<Guid>("ScheduleSettingsId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<TimeOnly>("Start")
                        .HasColumnType("time");

                    b.HasKey("Id");

                    b.HasIndex("ScheduleSettingsId");

                    b.ToTable("LessonPeriod");
                });

            modelBuilder.Entity("AlpimiAPI.Entities.ELessonType.LessonType", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<int>("Color")
                        .HasColumnType("int");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<Guid>("ScheduleId")
                        .HasColumnType("uniqueidentifier");

                    b.HasKey("Id");

                    b.HasIndex("ScheduleId");

                    b.ToTable("LessonType");
                });

            modelBuilder.Entity("AlpimiAPI.Entities.ESchedule.Schedule", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<Guid>("UserId")
                        .HasColumnType("uniqueidentifier");

                    b.HasKey("Id");

                    b.HasIndex("UserId");

                    b.ToTable("Schedule");
                });

            modelBuilder.Entity("AlpimiAPI.Entities.EScheduleSettings.ScheduleSettings", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<Guid>("ScheduleId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("SchoolDays")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("SchoolHour")
                        .HasColumnType("int");

                    b.Property<DateOnly>("SchoolYearEnd")
                        .HasColumnType("date");

                    b.Property<DateOnly>("SchoolYearStart")
                        .HasColumnType("date");

                    b.HasKey("Id");

                    b.HasIndex("ScheduleId");

                    b.ToTable("ScheduleSettings");
                });

            modelBuilder.Entity("AlpimiAPI.Entities.EStudent.Student", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("AlbumNumber")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<Guid>("GroupId")
                        .HasColumnType("uniqueidentifier");

                    b.HasKey("Id");

                    b.HasIndex("GroupId");

                    b.ToTable("Student");
                });

            modelBuilder.Entity("AlpimiAPI.Entities.ESubgroup.Subgroup", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<Guid>("GroupId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("StudentCount")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.HasIndex("GroupId");

                    b.ToTable("Subgroup");
                });

            modelBuilder.Entity("AlpimiAPI.Entities.ETeacher.Teacher", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<Guid>("ScheduleId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("Surname")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.HasIndex("ScheduleId");

                    b.ToTable("Teacher");
                });

            modelBuilder.Entity("AlpimiAPI.Entities.EUser.User", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("CustomURL")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Login")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.ToTable("User");
                });

            modelBuilder.Entity("AlpimiAPI.Relations.ClassroomClassroomType", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<Guid>("ClassroomId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<Guid>("ClassroomTypeId")
                        .HasColumnType("uniqueidentifier");

                    b.HasKey("Id");

                    b.HasIndex("ClassroomId");

                    b.ToTable("ClassroomClassroomType");
                });

            modelBuilder.Entity("AlpimiAPI.Relations.LessonClassroomType", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<Guid>("ClassroomTypeId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<Guid>("LessonId")
                        .HasColumnType("uniqueidentifier");

                    b.HasKey("Id");

                    b.HasIndex("LessonId");

                    b.ToTable("LessonClassroomType");
                });

            modelBuilder.Entity("AlpimiAPI.Relations.StudentSubgroup", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<Guid>("StudentId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<Guid>("SubgroupId")
                        .HasColumnType("uniqueidentifier");

                    b.HasKey("Id");

                    b.HasIndex("StudentId");

                    b.ToTable("StudentSubgroup");
                });

            modelBuilder.Entity("AlpimiAPI.Entities.EAuth.Auth", b =>
                {
                    b.HasOne("AlpimiAPI.Entities.EUser.User", "User")
                        .WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("User");
                });

            modelBuilder.Entity("AlpimiAPI.Entities.EClassroom.Classroom", b =>
                {
                    b.HasOne("AlpimiAPI.Entities.ESchedule.Schedule", "Schedule")
                        .WithMany()
                        .HasForeignKey("ScheduleId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Schedule");
                });

            modelBuilder.Entity("AlpimiAPI.Entities.EClassroomType.ClassroomType", b =>
                {
                    b.HasOne("AlpimiAPI.Entities.ESchedule.Schedule", "Schedule")
                        .WithMany()
                        .HasForeignKey("ScheduleId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Schedule");
                });

            modelBuilder.Entity("AlpimiAPI.Entities.EDayOff.DayOff", b =>
                {
                    b.HasOne("AlpimiAPI.Entities.EScheduleSettings.ScheduleSettings", "ScheduleSettings")
                        .WithMany()
                        .HasForeignKey("ScheduleSettingsId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("ScheduleSettings");
                });

            modelBuilder.Entity("AlpimiAPI.Entities.EGroup.Group", b =>
                {
                    b.HasOne("AlpimiAPI.Entities.ESchedule.Schedule", "Schedule")
                        .WithMany()
                        .HasForeignKey("ScheduleId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Schedule");
                });

            modelBuilder.Entity("AlpimiAPI.Entities.ELesson.Lesson", b =>
                {
                    b.HasOne("AlpimiAPI.Entities.ELessonType.LessonType", "LessonType")
                        .WithMany()
                        .HasForeignKey("LessonTypeId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("AlpimiAPI.Entities.ESubgroup.Subgroup", "Subgroup")
                        .WithMany()
                        .HasForeignKey("SubgroupId")
                        .OnDelete(DeleteBehavior.NoAction)
                        .IsRequired();

                    b.Navigation("LessonType");

                    b.Navigation("Subgroup");
                });

            modelBuilder.Entity("AlpimiAPI.Entities.ELessonBlock.LessonBlock", b =>
                {
                    b.HasOne("AlpimiAPI.Entities.EClassroom.Classroom", "Classroom")
                        .WithMany()
                        .HasForeignKey("ClassroomId")
                        .OnDelete(DeleteBehavior.NoAction);

                    b.HasOne("AlpimiAPI.Entities.ELesson.Lesson", "Lesson")
                        .WithMany()
                        .HasForeignKey("LessonId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("AlpimiAPI.Entities.ETeacher.Teacher", "Teacher")
                        .WithMany()
                        .HasForeignKey("TeacherId")
                        .OnDelete(DeleteBehavior.NoAction);

                    b.Navigation("Classroom");

                    b.Navigation("Lesson");

                    b.Navigation("Teacher");
                });

            modelBuilder.Entity("AlpimiAPI.Entities.ELessonPeriod.LessonPeriod", b =>
                {
                    b.HasOne("AlpimiAPI.Entities.EScheduleSettings.ScheduleSettings", "ScheduleSettings")
                        .WithMany()
                        .HasForeignKey("ScheduleSettingsId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("ScheduleSettings");
                });

            modelBuilder.Entity("AlpimiAPI.Entities.ELessonType.LessonType", b =>
                {
                    b.HasOne("AlpimiAPI.Entities.ESchedule.Schedule", "Schedule")
                        .WithMany()
                        .HasForeignKey("ScheduleId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Schedule");
                });

            modelBuilder.Entity("AlpimiAPI.Entities.ESchedule.Schedule", b =>
                {
                    b.HasOne("AlpimiAPI.Entities.EUser.User", "User")
                        .WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("User");
                });

            modelBuilder.Entity("AlpimiAPI.Entities.EScheduleSettings.ScheduleSettings", b =>
                {
                    b.HasOne("AlpimiAPI.Entities.ESchedule.Schedule", "Schedule")
                        .WithMany()
                        .HasForeignKey("ScheduleId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Schedule");
                });

            modelBuilder.Entity("AlpimiAPI.Entities.EStudent.Student", b =>
                {
                    b.HasOne("AlpimiAPI.Entities.EGroup.Group", "Group")
                        .WithMany()
                        .HasForeignKey("GroupId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Group");
                });

            modelBuilder.Entity("AlpimiAPI.Entities.ESubgroup.Subgroup", b =>
                {
                    b.HasOne("AlpimiAPI.Entities.EGroup.Group", "Group")
                        .WithMany()
                        .HasForeignKey("GroupId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Group");
                });

            modelBuilder.Entity("AlpimiAPI.Entities.ETeacher.Teacher", b =>
                {
                    b.HasOne("AlpimiAPI.Entities.ESchedule.Schedule", "Schedule")
                        .WithMany()
                        .HasForeignKey("ScheduleId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Schedule");
                });

            modelBuilder.Entity("AlpimiAPI.Relations.ClassroomClassroomType", b =>
                {
                    b.HasOne("AlpimiAPI.Entities.EClassroom.Classroom", "Classroom")
                        .WithMany()
                        .HasForeignKey("ClassroomId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Classroom");
                });

            modelBuilder.Entity("AlpimiAPI.Relations.LessonClassroomType", b =>
                {
                    b.HasOne("AlpimiAPI.Entities.ELesson.Lesson", "Lesson")
                        .WithMany()
                        .HasForeignKey("LessonId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Lesson");
                });

            modelBuilder.Entity("AlpimiAPI.Relations.StudentSubgroup", b =>
                {
                    b.HasOne("AlpimiAPI.Entities.EStudent.Student", "Student")
                        .WithMany()
                        .HasForeignKey("StudentId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Student");
                });
#pragma warning restore 612, 618
        }
    }
}
