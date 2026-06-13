using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace IotMonitor.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddUdpDeviceEntity : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "UdpDevices",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UdpDevices", x => x.Id);
                    table.ForeignKey(
                        name: "FK_UdpDevices_Devices_Id",
                        column: x => x.Id,
                        principalTable: "Devices",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "UdpDevices");
        }
    }
}
