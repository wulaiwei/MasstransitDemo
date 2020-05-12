using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace WorkDataMassTransitSagaPersistence.Migrations
{
    public partial class _202004021714 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "OrderState",
                columns: table => new
                {
                    CorrelationId = table.Column<Guid>(nullable: false),
                    CurrentState = table.Column<int>(maxLength: 64, nullable: false),
                    ReadyEventStatus = table.Column<int>(nullable: false),
                    OrderSerialNumber = table.Column<string>(maxLength: 200, nullable: true),
                    RowVersion = table.Column<byte[]>(rowVersion: true, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OrderState", x => x.CorrelationId);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "OrderState");
        }
    }
}
