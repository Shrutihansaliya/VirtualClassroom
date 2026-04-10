using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace VirtualClassroom.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class UpdateModelClassroomMember : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Email",
                table: "TblClassroomMembers",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Email",
                table: "TblClassroomMembers");
        }
    }
}
