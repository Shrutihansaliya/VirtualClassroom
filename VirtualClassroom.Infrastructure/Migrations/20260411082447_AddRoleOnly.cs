//using System;
//using Microsoft.EntityFrameworkCore.Migrations;

//#nullable disable

//namespace VirtualClassroom.Infrastructure.Migrations
//{
//    /// <inheritdoc />
//    public partial class AddRoleOnly : Migration
//    {
//        /// <inheritdoc />
//        protected override void Up(MigrationBuilder migrationBuilder)
//        {
//            migrationBuilder.AlterColumn<DateTime>(
//                name: "SubmittedAt",
//                table: "TblSubmissions",
//                type: "datetime2",
//                nullable: true,
//                oldClrType: typeof(DateTime),
//                oldType: "datetime2");

//            migrationBuilder.AddColumn<string>(
//                name: "Status",
//                table: "TblSubmissions",
//                type: "nvarchar(max)",
//                nullable: false,
//                defaultValue: "");

//            migrationBuilder.AddColumn<string>(
//                name: "Role",
//                table: "TblClassroomMembers",
//                type: "nvarchar(max)",
//                nullable: false,
//                defaultValue: "");

//            migrationBuilder.AddColumn<DateTime>(
//                name: "CreatedAt",
//                table: "TblAssignments",
//                type: "datetime2",
//                nullable: false,
//                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));
//        }

//        /// <inheritdoc />
//        protected override void Down(MigrationBuilder migrationBuilder)
//        {
//            migrationBuilder.DropColumn(
//                name: "Status",
//                table: "TblSubmissions");

//            migrationBuilder.DropColumn(
//                name: "Role",
//                table: "TblClassroomMembers");

//            migrationBuilder.DropColumn(
//                name: "CreatedAt",
//                table: "TblAssignments");

//            migrationBuilder.AlterColumn<DateTime>(
//                name: "SubmittedAt",
//                table: "TblSubmissions",
//                type: "datetime2",
//                nullable: false,
//                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified),
//                oldClrType: typeof(DateTime),
//                oldType: "datetime2",
//                oldNullable: true);
//        }
//    }
//}


using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace VirtualClassroom.Infrastructure.Migrations
{
    public partial class AddRoleOnly : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Role",
                table: "TblClassroomMembers",
                type: "nvarchar(max)",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Role",
                table: "TblClassroomMembers");
        }
    }
}