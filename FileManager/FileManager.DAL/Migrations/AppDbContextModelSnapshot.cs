﻿// <auto-generated />
using System;
using FileManager.DAL;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace FileManager.DAL.Migrations
{
    [DbContext(typeof(AppDbContext))]
    partial class AppDbContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "8.0.0")
                .HasAnnotation("Relational:MaxIdentifierLength", 63);

            NpgsqlModelBuilderExtensions.UseIdentityByDefaultColumns(modelBuilder);

            modelBuilder.HasSequence("TaskOperationSequence");

            modelBuilder.Entity("FileManager.Domain.Entity.AddresseeEntity", b =>
                {
                    b.Property<string>("PersonalNumber")
                        .HasColumnType("text");

                    b.Property<int>("AddresseeGroupId")
                        .HasColumnType("integer");

                    b.Property<string>("EMail")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<string>("Fio")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<bool>("IsActive")
                        .HasColumnType("boolean");

                    b.Property<string>("StructuralUnit")
                        .IsRequired()
                        .HasColumnType("text");

                    b.HasKey("PersonalNumber", "AddresseeGroupId");

                    b.HasIndex("AddresseeGroupId");

                    b.ToTable("Addressee");
                });

            modelBuilder.Entity("FileManager.Domain.Entity.AddresseeGroupEntity", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("Id"));

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("text");

                    b.HasKey("Id");

                    b.ToTable("AddresseeGroup");
                });

            modelBuilder.Entity("FileManager.Domain.Entity.ClientLogEntity", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("bigint");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<long>("Id"));

                    b.Property<DateTime>("Created")
                        .HasColumnType("timestamp without time zone");

                    b.Property<string>("Values")
                        .HasColumnType("jsonb");

                    b.HasKey("Id");

                    b.ToTable("ClientLog");
                });

            modelBuilder.Entity("FileManager.Domain.Entity.TaskEntity", b =>
                {
                    b.Property<string>("TaskId")
                        .HasColumnType("text");

                    b.Property<int?>("AddresseeGroupId")
                        .HasColumnType("integer");

                    b.Property<int>("DayActive")
                        .HasColumnType("integer");

                    b.Property<int>("ExecutionLeft")
                        .HasColumnType("integer");

                    b.Property<int>("ExecutionLimit")
                        .HasColumnType("integer");

                    b.Property<bool>("IsActive")
                        .HasColumnType("boolean");

                    b.Property<bool>("IsProgress")
                        .HasColumnType("boolean");

                    b.Property<DateTime>("LastModified")
                        .HasColumnType("timestamp without time zone");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<int?>("TaskGroupEntityId")
                        .HasColumnType("integer");

                    b.Property<int>("TaskGroupId")
                        .HasColumnType("integer");

                    b.Property<TimeOnly>("TimeBegin")
                        .HasColumnType("time without time zone");

                    b.Property<TimeOnly>("TimeEnd")
                        .HasColumnType("time without time zone");

                    b.HasKey("TaskId");

                    b.HasIndex("TaskGroupEntityId");

                    b.ToTable("Task");
                });

            modelBuilder.Entity("FileManager.Domain.Entity.TaskGroupEntity", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("Id"));

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("text");

                    b.HasKey("Id");

                    b.ToTable("TaskGroup");
                });

            modelBuilder.Entity("FileManager.Domain.Entity.TaskLogEntity", b =>
                {
                    b.Property<DateTime>("DateTimeLog")
                        .HasColumnType("timestamp without time zone");

                    b.Property<string>("TaskId")
                        .HasColumnType("text");

                    b.Property<string>("FileName")
                        .HasColumnType("text");

                    b.Property<int?>("OperationId")
                        .HasColumnType("integer");

                    b.Property<int?>("ResultOperation")
                        .HasColumnType("integer");

                    b.Property<string>("ResultText")
                        .HasColumnType("text");

                    b.Property<int?>("StepId")
                        .HasColumnType("integer");

                    b.HasKey("DateTimeLog", "TaskId");

                    b.ToTable("TaskLog");
                });

            modelBuilder.Entity("FileManager.Domain.Entity.TaskOperation", b =>
                {
                    b.Property<int>("OperationId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer")
                        .HasDefaultValueSql("nextval('\"TaskOperationSequence\"')");

                    NpgsqlPropertyBuilderExtensions.UseSequence(b.Property<int>("OperationId"));

                    b.Property<string>("AdditionalText")
                        .HasColumnType("text");

                    b.Property<int?>("AddresseeGroupId")
                        .HasColumnType("integer");

                    b.Property<bool>("InformSuccess")
                        .HasColumnType("boolean");

                    b.Property<int>("StepId")
                        .HasColumnType("integer");

                    b.HasKey("OperationId");

                    b.ToTable((string)null);

                    b.UseTpcMappingStrategy();
                });

            modelBuilder.Entity("FileManager.Domain.Entity.TaskStatusEntity", b =>
                {
                    b.Property<string>("TaskId")
                        .HasColumnType("text");

                    b.Property<int>("CountExecute")
                        .HasColumnType("integer");

                    b.Property<int?>("CountLeftFiles")
                        .HasColumnType("integer");

                    b.Property<int>("CountProcessedFiles")
                        .HasColumnType("integer");

                    b.Property<DateTime>("DateLastExecute")
                        .HasColumnType("timestamp without time zone");

                    b.Property<bool>("IsError")
                        .HasColumnType("boolean");

                    b.Property<bool>("IsProgress")
                        .HasColumnType("boolean");

                    b.HasKey("TaskId");

                    b.ToTable("TaskStatuse");
                });

            modelBuilder.Entity("FileManager.Domain.Entity.TaskStepEntity", b =>
                {
                    b.Property<int>("StepId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("StepId"));

                    b.Property<string>("Description")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<string>("Destination")
                        .HasColumnType("text");

                    b.Property<string>("FileMask")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<bool>("IsActive")
                        .HasColumnType("boolean");

                    b.Property<bool>("IsBreak")
                        .HasColumnType("boolean");

                    b.Property<int>("OperationId")
                        .HasColumnType("integer");

                    b.Property<int>("OperationName")
                        .HasColumnType("integer");

                    b.Property<string>("Source")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<int>("StepNumber")
                        .HasColumnType("integer");

                    b.Property<string>("TaskId")
                        .IsRequired()
                        .HasColumnType("text");

                    b.HasKey("StepId");

                    b.HasIndex("TaskId");

                    b.ToTable("TaskStep");
                });

            modelBuilder.Entity("FileManager.Domain.Entity.UserLogEntity", b =>
                {
                    b.Property<DateTime>("DateTimeLog")
                        .HasColumnType("timestamp without time zone");

                    b.Property<string>("UserName")
                        .HasColumnType("text");

                    b.Property<string>("Action")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<string>("Data")
                        .IsRequired()
                        .HasColumnType("jsonb");

                    b.HasKey("DateTimeLog", "UserName");

                    b.ToTable("UserLog");
                });

            modelBuilder.Entity("FileManager.Domain.Entity.OperationClrbufEntity", b =>
                {
                    b.HasBaseType("FileManager.Domain.Entity.TaskOperation");

                    b.HasIndex("StepId");

                    b.ToTable("OperationClrbuf");
                });

            modelBuilder.Entity("FileManager.Domain.Entity.OperationCopyEntity", b =>
                {
                    b.HasBaseType("FileManager.Domain.Entity.TaskOperation");

                    b.Property<int>("FileAttribute")
                        .HasColumnType("integer");

                    b.Property<int>("FileInDestination")
                        .HasColumnType("integer");

                    b.Property<bool>("FileInLog")
                        .HasColumnType("boolean");

                    b.Property<int>("FileInSource")
                        .HasColumnType("integer");

                    b.Property<int>("FilesForProcessing")
                        .HasColumnType("integer");

                    b.Property<int>("Sort")
                        .HasColumnType("integer");

                    b.HasIndex("StepId");

                    b.ToTable("OperationCopy");
                });

            modelBuilder.Entity("FileManager.Domain.Entity.OperationDeleteEntity", b =>
                {
                    b.HasBaseType("FileManager.Domain.Entity.TaskOperation");

                    b.HasIndex("StepId");

                    b.ToTable("OperationDelete");
                });

            modelBuilder.Entity("FileManager.Domain.Entity.OperationExistEntity", b =>
                {
                    b.HasBaseType("FileManager.Domain.Entity.TaskOperation");

                    b.Property<bool>("BreakTaskAfterError")
                        .HasColumnType("boolean");

                    b.Property<int>("ExpectedResult")
                        .HasColumnType("integer");

                    b.HasIndex("StepId");

                    b.ToTable("OperationExist");
                });

            modelBuilder.Entity("FileManager.Domain.Entity.OperationMoveEntity", b =>
                {
                    b.HasBaseType("FileManager.Domain.Entity.TaskOperation");

                    b.Property<int>("FileAttribute")
                        .HasColumnType("integer");

                    b.Property<int>("FileInDestination")
                        .HasColumnType("integer");

                    b.Property<bool>("FileInLog")
                        .HasColumnType("boolean");

                    b.Property<int>("FilesForProcessing")
                        .HasColumnType("integer");

                    b.Property<int>("Sort")
                        .HasColumnType("integer");

                    b.HasIndex("StepId");

                    b.ToTable("OperationMove");
                });

            modelBuilder.Entity("FileManager.Domain.Entity.OperationReadEntity", b =>
                {
                    b.HasBaseType("FileManager.Domain.Entity.TaskOperation");

                    b.Property<bool>("BreakTaskAfterError")
                        .HasColumnType("boolean");

                    b.Property<int>("Encode")
                        .HasColumnType("integer");

                    b.Property<int>("ExpectedResult")
                        .HasColumnType("integer");

                    b.Property<int>("FileInSource")
                        .HasColumnType("integer");

                    b.Property<string>("FindString")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<bool>("SearchRegex")
                        .HasColumnType("boolean");

                    b.HasIndex("StepId");

                    b.ToTable("OperationRead");
                });

            modelBuilder.Entity("FileManager.Domain.Entity.OperationRenameEntity", b =>
                {
                    b.HasBaseType("FileManager.Domain.Entity.TaskOperation");

                    b.Property<string>("Pattern")
                        .IsRequired()
                        .HasColumnType("text");

                    b.HasIndex("StepId");

                    b.ToTable("OperationRename");
                });

            modelBuilder.Entity("FileManager.Domain.Entity.AddresseeEntity", b =>
                {
                    b.HasOne("FileManager.Domain.Entity.AddresseeGroupEntity", "AddresseeGroup")
                        .WithMany()
                        .HasForeignKey("AddresseeGroupId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("AddresseeGroup");
                });

            modelBuilder.Entity("FileManager.Domain.Entity.TaskEntity", b =>
                {
                    b.HasOne("FileManager.Domain.Entity.TaskGroupEntity", null)
                        .WithMany("Tasks")
                        .HasForeignKey("TaskGroupEntityId");
                });

            modelBuilder.Entity("FileManager.Domain.Entity.TaskStatusEntity", b =>
                {
                    b.HasOne("FileManager.Domain.Entity.TaskEntity", "Task")
                        .WithOne("TaskStatus")
                        .HasForeignKey("FileManager.Domain.Entity.TaskStatusEntity", "TaskId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Task");
                });

            modelBuilder.Entity("FileManager.Domain.Entity.TaskStepEntity", b =>
                {
                    b.HasOne("FileManager.Domain.Entity.TaskEntity", "Task")
                        .WithMany("Steps")
                        .HasForeignKey("TaskId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Task");
                });

            modelBuilder.Entity("FileManager.Domain.Entity.OperationClrbufEntity", b =>
                {
                    b.HasOne("FileManager.Domain.Entity.TaskStepEntity", "Step")
                        .WithMany()
                        .HasForeignKey("StepId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Step");
                });

            modelBuilder.Entity("FileManager.Domain.Entity.OperationCopyEntity", b =>
                {
                    b.HasOne("FileManager.Domain.Entity.TaskStepEntity", "Step")
                        .WithMany()
                        .HasForeignKey("StepId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Step");
                });

            modelBuilder.Entity("FileManager.Domain.Entity.OperationDeleteEntity", b =>
                {
                    b.HasOne("FileManager.Domain.Entity.TaskStepEntity", "Step")
                        .WithMany()
                        .HasForeignKey("StepId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Step");
                });

            modelBuilder.Entity("FileManager.Domain.Entity.OperationExistEntity", b =>
                {
                    b.HasOne("FileManager.Domain.Entity.TaskStepEntity", "Step")
                        .WithMany()
                        .HasForeignKey("StepId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Step");
                });

            modelBuilder.Entity("FileManager.Domain.Entity.OperationMoveEntity", b =>
                {
                    b.HasOne("FileManager.Domain.Entity.TaskStepEntity", "Step")
                        .WithMany()
                        .HasForeignKey("StepId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Step");
                });

            modelBuilder.Entity("FileManager.Domain.Entity.OperationReadEntity", b =>
                {
                    b.HasOne("FileManager.Domain.Entity.TaskStepEntity", "Step")
                        .WithMany()
                        .HasForeignKey("StepId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Step");
                });

            modelBuilder.Entity("FileManager.Domain.Entity.OperationRenameEntity", b =>
                {
                    b.HasOne("FileManager.Domain.Entity.TaskStepEntity", "Step")
                        .WithMany()
                        .HasForeignKey("StepId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Step");
                });

            modelBuilder.Entity("FileManager.Domain.Entity.TaskEntity", b =>
                {
                    b.Navigation("Steps");

                    b.Navigation("TaskStatus");
                });

            modelBuilder.Entity("FileManager.Domain.Entity.TaskGroupEntity", b =>
                {
                    b.Navigation("Tasks");
                });
#pragma warning restore 612, 618
        }
    }
}
