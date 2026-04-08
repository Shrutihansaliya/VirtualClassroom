using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace VirtualClassroom.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "TblUsers",
                columns: table => new
                {
                    UserId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    FullName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Email = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    PasswordHash = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    AuthProvider = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ProviderUserId = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ProfilePicture = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Role = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETDATE()")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TblUsers", x => x.UserId);
                });

            migrationBuilder.CreateTable(
                name: "TblClassroom",
                columns: table => new
                {
                    ClassroomId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ClassName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CreatedBy = table.Column<int>(type: "int", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TblClassroom", x => x.ClassroomId);
                    table.ForeignKey(
                        name: "FK_TblClassroom_TblUsers_CreatedBy",
                        column: x => x.CreatedBy,
                        principalTable: "TblUsers",
                        principalColumn: "UserId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "TblNotifications",
                columns: table => new
                {
                    NotificationId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<int>(type: "int", nullable: false),
                    Title = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Message = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    IsRead = table.Column<bool>(type: "bit", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TblNotifications", x => x.NotificationId);
                    table.ForeignKey(
                        name: "FK_TblNotifications_TblUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "TblUsers",
                        principalColumn: "UserId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "TblUserLogins",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<int>(type: "int", nullable: false),
                    Provider = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ProviderUserId = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TblUserLogins", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TblUserLogins_TblUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "TblUsers",
                        principalColumn: "UserId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "TblAssignments",
                columns: table => new
                {
                    AssignmentId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ClassroomId = table.Column<int>(type: "int", nullable: false),
                    Title = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    DueDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<int>(type: "int", nullable: false),
                    FilePath = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    FileType = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TblAssignments", x => x.AssignmentId);
                    table.ForeignKey(
                        name: "FK_TblAssignments_TblClassroom_ClassroomId",
                        column: x => x.ClassroomId,
                        principalTable: "TblClassroom",
                        principalColumn: "ClassroomId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_TblAssignments_TblUsers_CreatedBy",
                        column: x => x.CreatedBy,
                        principalTable: "TblUsers",
                        principalColumn: "UserId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "TblClassroomMembers",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ClassroomId = table.Column<int>(type: "int", nullable: false),
                    UserId = table.Column<int>(type: "int", nullable: false),
                    JoinedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TblClassroomMembers", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TblClassroomMembers_TblClassroom_ClassroomId",
                        column: x => x.ClassroomId,
                        principalTable: "TblClassroom",
                        principalColumn: "ClassroomId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_TblClassroomMembers_TblUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "TblUsers",
                        principalColumn: "UserId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "TblLectures",
                columns: table => new
                {
                    LectureId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ClassroomId = table.Column<int>(type: "int", nullable: false),
                    Title = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    StartTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    EndTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    LiveLink = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Platform = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Status = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    RecordingUrl = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CreatedBy = table.Column<int>(type: "int", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TblLectures", x => x.LectureId);
                    table.ForeignKey(
                        name: "FK_TblLectures_TblClassroom_ClassroomId",
                        column: x => x.ClassroomId,
                        principalTable: "TblClassroom",
                        principalColumn: "ClassroomId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_TblLectures_TblUsers_CreatedBy",
                        column: x => x.CreatedBy,
                        principalTable: "TblUsers",
                        principalColumn: "UserId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "TblMaterials",
                columns: table => new
                {
                    MaterialId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ClassroomId = table.Column<int>(type: "int", nullable: false),
                    Title = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    FilePath = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    FileType = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    UploadedBy = table.Column<int>(type: "int", nullable: false),
                    UploadedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    IsVisible = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TblMaterials", x => x.MaterialId);
                    table.ForeignKey(
                        name: "FK_TblMaterials_TblClassroom_ClassroomId",
                        column: x => x.ClassroomId,
                        principalTable: "TblClassroom",
                        principalColumn: "ClassroomId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_TblMaterials_TblUsers_UploadedBy",
                        column: x => x.UploadedBy,
                        principalTable: "TblUsers",
                        principalColumn: "UserId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "TblSubmissions",
                columns: table => new
                {
                    SubmissionId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    AssignmentId = table.Column<int>(type: "int", nullable: false),
                    StudentId = table.Column<int>(type: "int", nullable: false),
                    FilePath = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    SubmittedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Marks = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TblSubmissions", x => x.SubmissionId);
                    table.ForeignKey(
                        name: "FK_TblSubmissions_TblAssignments_AssignmentId",
                        column: x => x.AssignmentId,
                        principalTable: "TblAssignments",
                        principalColumn: "AssignmentId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_TblSubmissions_TblUsers_StudentId",
                        column: x => x.StudentId,
                        principalTable: "TblUsers",
                        principalColumn: "UserId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_TblAssignments_ClassroomId",
                table: "TblAssignments",
                column: "ClassroomId");

            migrationBuilder.CreateIndex(
                name: "IX_TblAssignments_CreatedBy",
                table: "TblAssignments",
                column: "CreatedBy");

            migrationBuilder.CreateIndex(
                name: "IX_TblClassroom_CreatedBy",
                table: "TblClassroom",
                column: "CreatedBy");

            migrationBuilder.CreateIndex(
                name: "IX_TblClassroomMembers_ClassroomId_UserId",
                table: "TblClassroomMembers",
                columns: new[] { "ClassroomId", "UserId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_TblClassroomMembers_UserId",
                table: "TblClassroomMembers",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_TblLectures_ClassroomId",
                table: "TblLectures",
                column: "ClassroomId");

            migrationBuilder.CreateIndex(
                name: "IX_TblLectures_CreatedBy",
                table: "TblLectures",
                column: "CreatedBy");

            migrationBuilder.CreateIndex(
                name: "IX_TblMaterials_ClassroomId",
                table: "TblMaterials",
                column: "ClassroomId");

            migrationBuilder.CreateIndex(
                name: "IX_TblMaterials_UploadedBy",
                table: "TblMaterials",
                column: "UploadedBy");

            migrationBuilder.CreateIndex(
                name: "IX_TblNotifications_UserId",
                table: "TblNotifications",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_TblSubmissions_AssignmentId",
                table: "TblSubmissions",
                column: "AssignmentId");

            migrationBuilder.CreateIndex(
                name: "IX_TblSubmissions_StudentId",
                table: "TblSubmissions",
                column: "StudentId");

            migrationBuilder.CreateIndex(
                name: "IX_TblUserLogins_UserId",
                table: "TblUserLogins",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_TblUsers_Email",
                table: "TblUsers",
                column: "Email",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "TblClassroomMembers");

            migrationBuilder.DropTable(
                name: "TblLectures");

            migrationBuilder.DropTable(
                name: "TblMaterials");

            migrationBuilder.DropTable(
                name: "TblNotifications");

            migrationBuilder.DropTable(
                name: "TblSubmissions");

            migrationBuilder.DropTable(
                name: "TblUserLogins");

            migrationBuilder.DropTable(
                name: "TblAssignments");

            migrationBuilder.DropTable(
                name: "TblClassroom");

            migrationBuilder.DropTable(
                name: "TblUsers");
        }
    }
}
