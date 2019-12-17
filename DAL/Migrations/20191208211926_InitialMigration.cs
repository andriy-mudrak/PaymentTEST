using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace DAL.Migrations
{
    public partial class InitialMigration : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Transactions",
                columns: table => new
                {
                    TransactionID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    ExternalID = table.Column<string>(maxLength: 50, nullable: true),
                    TransactionType = table.Column<string>(maxLength: 20, nullable: true),
                    Amount = table.Column<long>(nullable: false),
                    Status = table.Column<string>(maxLength: 30, nullable: true),
                    Metadata = table.Column<string>(maxLength: 1000, nullable: true),
                    TransactionTime = table.Column<DateTime>(type: "datetime", nullable: false),
                    UserID = table.Column<int>(nullable: false),
                    OrderID = table.Column<int>(nullable: false),
                    VendorID = table.Column<int>(nullable: false),
                    Instrument = table.Column<string>(maxLength: 20, nullable: true),
                    Response = table.Column<string>(maxLength: 4000, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__Transacctions_$Id", x => x.TransactionID);
                });

            migrationBuilder.CreateTable(
                name: "Users",
                columns: table => new
                {
                    UserID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    ExternalID = table.Column<string>(maxLength: 50, nullable: true),
                    Email = table.Column<string>(maxLength: 50, nullable: true),
                    UserToken = table.Column<string>(maxLength: 50, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__Users__Id", x => x.UserID);
                });

            migrationBuilder.CreateIndex(
                name: "in_Transactions_$orderId$userId$vendorId$externalId",
                table: "Transactions",
                columns: new[] { "UserID", "VendorID", "ExternalID", "OrderID" });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Transactions");

            migrationBuilder.DropTable(
                name: "Users");
        }
    }
}
