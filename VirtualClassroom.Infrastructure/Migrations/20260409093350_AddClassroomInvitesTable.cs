using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace VirtualClassroom.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddClassroomInvitesTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_TblAssignments_TblClassroom_ClassroomId",
                table: "TblAssignments");

            migrationBuilder.DropForeignKey(
                name: "FK_TblClassroom_TblUsers_CreatedBy",
                table: "TblClassroom");

            migrationBuilder.DropForeignKey(
                name: "FK_TblClassroomMembers_TblClassroom_ClassroomId",
                table: "TblClassroomMembers");

            migrationBuilder.DropForeignKey(
                name: "FK_TblLectures_TblClassroom_ClassroomId",
                table: "TblLectures");

            migrationBuilder.DropForeignKey(
                name: "FK_TblMaterials_TblClassroom_ClassroomId",
                table: "TblMaterials");

            migrationBuilder.DropPrimaryKey(
                name: "PK_TblClassroom",
                table: "TblClassroom");

            migrationBuilder.DropColumn(
                name: "Email",
                table: "TblClassroomMembers");

            migrationBuilder.RenameTable(
                name: "TblClassroom",
                newName: "TblClassrooms");

            migrationBuilder.RenameIndex(
                name: "IX_TblClassroom_CreatedBy",
                table: "TblClassrooms",
                newName: "IX_TblClassrooms_CreatedBy");

            migrationBuilder.AddPrimaryKey(
                name: "PK_TblClassrooms",
                table: "TblClassrooms",
                column: "ClassroomId");

            migrationBuilder.AddForeignKey(
                name: "FK_TblAssignments_TblClassrooms_ClassroomId",
                table: "TblAssignments",
                column: "ClassroomId",
                principalTable: "TblClassrooms",
                principalColumn: "ClassroomId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_TblClassroomMembers_TblClassrooms_ClassroomId",
                table: "TblClassroomMembers",
                column: "ClassroomId",
                principalTable: "TblClassrooms",
                principalColumn: "ClassroomId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_TblClassrooms_TblUsers_CreatedBy",
                table: "TblClassrooms",
                column: "CreatedBy",
                principalTable: "TblUsers",
                principalColumn: "UserId",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_TblLectures_TblClassrooms_ClassroomId",
                table: "TblLectures",
                column: "ClassroomId",
                principalTable: "TblClassrooms",
                principalColumn: "ClassroomId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_TblMaterials_TblClassrooms_ClassroomId",
                table: "TblMaterials",
                column: "ClassroomId",
                principalTable: "TblClassrooms",
                principalColumn: "ClassroomId",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_TblAssignments_TblClassrooms_ClassroomId",
                table: "TblAssignments");

            migrationBuilder.DropForeignKey(
                name: "FK_TblClassroomMembers_TblClassrooms_ClassroomId",
                table: "TblClassroomMembers");

            migrationBuilder.DropForeignKey(
                name: "FK_TblClassrooms_TblUsers_CreatedBy",
                table: "TblClassrooms");

            migrationBuilder.DropForeignKey(
                name: "FK_TblLectures_TblClassrooms_ClassroomId",
                table: "TblLectures");

            migrationBuilder.DropForeignKey(
                name: "FK_TblMaterials_TblClassrooms_ClassroomId",
                table: "TblMaterials");

            migrationBuilder.DropPrimaryKey(
                name: "PK_TblClassrooms",
                table: "TblClassrooms");

            migrationBuilder.RenameTable(
                name: "TblClassrooms",
                newName: "TblClassroom");

            migrationBuilder.RenameIndex(
                name: "IX_TblClassrooms_CreatedBy",
                table: "TblClassroom",
                newName: "IX_TblClassroom_CreatedBy");

            migrationBuilder.AddColumn<string>(
                name: "Email",
                table: "TblClassroomMembers",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddPrimaryKey(
                name: "PK_TblClassroom",
                table: "TblClassroom",
                column: "ClassroomId");

            migrationBuilder.AddForeignKey(
                name: "FK_TblAssignments_TblClassroom_ClassroomId",
                table: "TblAssignments",
                column: "ClassroomId",
                principalTable: "TblClassroom",
                principalColumn: "ClassroomId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_TblClassroom_TblUsers_CreatedBy",
                table: "TblClassroom",
                column: "CreatedBy",
                principalTable: "TblUsers",
                principalColumn: "UserId",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_TblClassroomMembers_TblClassroom_ClassroomId",
                table: "TblClassroomMembers",
                column: "ClassroomId",
                principalTable: "TblClassroom",
                principalColumn: "ClassroomId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_TblLectures_TblClassroom_ClassroomId",
                table: "TblLectures",
                column: "ClassroomId",
                principalTable: "TblClassroom",
                principalColumn: "ClassroomId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_TblMaterials_TblClassroom_ClassroomId",
                table: "TblMaterials",
                column: "ClassroomId",
                principalTable: "TblClassroom",
                principalColumn: "ClassroomId",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
