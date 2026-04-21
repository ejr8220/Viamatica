using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Viamatica.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddOperationalModules : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<decimal>(
                name: "amount",
                table: "payments",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<int>(
                name: "contract_contractid",
                table: "payments",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "description",
                table: "payments",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<int>(
                name: "cashier_userid",
                table: "attention",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "completedat",
                table: "attention",
                type: "datetimeoffset",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "contract_contractid",
                table: "attention",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "createdat",
                table: "attention",
                type: "datetimeoffset",
                nullable: false,
                defaultValue: new DateTimeOffset(new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)));

            migrationBuilder.AddColumn<string>(
                name: "notes",
                table: "attention",
                type: "nvarchar(300)",
                maxLength: 300,
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateTable(
                name: "cashsession",
                columns: table => new
                {
                    cashsessionid = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    cash_cashid = table.Column<int>(type: "int", nullable: false),
                    user_userid = table.Column<int>(type: "int", nullable: false),
                    startedat = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    endedat = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_cashsession", x => x.cashsessionid);
                    table.ForeignKey(
                        name: "FK_cashsession_cash_cash_cashid",
                        column: x => x.cash_cashid,
                        principalTable: "cash",
                        principalColumn: "cashid",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_cashsession_usertable_user_userid",
                        column: x => x.user_userid,
                        principalTable: "usertable",
                        principalColumn: "userid",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_payments_contract_contractid",
                table: "payments",
                column: "contract_contractid");

            migrationBuilder.CreateIndex(
                name: "IX_attention_cashier_userid",
                table: "attention",
                column: "cashier_userid");

            migrationBuilder.CreateIndex(
                name: "IX_attention_contract_contractid",
                table: "attention",
                column: "contract_contractid");

            migrationBuilder.CreateIndex(
                name: "IX_cashsession_cash_cashid",
                table: "cashsession",
                column: "cash_cashid",
                unique: true,
                filter: "[endedat] IS NULL");

            migrationBuilder.CreateIndex(
                name: "IX_cashsession_user_userid",
                table: "cashsession",
                column: "user_userid");

            migrationBuilder.AddForeignKey(
                name: "FK_attention_contract_contract_contractid",
                table: "attention",
                column: "contract_contractid",
                principalTable: "contract",
                principalColumn: "contractid",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_attention_usertable_cashier_userid",
                table: "attention",
                column: "cashier_userid",
                principalTable: "usertable",
                principalColumn: "userid",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_payments_contract_contract_contractid",
                table: "payments",
                column: "contract_contractid",
                principalTable: "contract",
                principalColumn: "contractid",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_attention_contract_contract_contractid",
                table: "attention");

            migrationBuilder.DropForeignKey(
                name: "FK_attention_usertable_cashier_userid",
                table: "attention");

            migrationBuilder.DropForeignKey(
                name: "FK_payments_contract_contract_contractid",
                table: "payments");

            migrationBuilder.DropTable(
                name: "cashsession");

            migrationBuilder.DropIndex(
                name: "IX_payments_contract_contractid",
                table: "payments");

            migrationBuilder.DropIndex(
                name: "IX_attention_cashier_userid",
                table: "attention");

            migrationBuilder.DropIndex(
                name: "IX_attention_contract_contractid",
                table: "attention");

            migrationBuilder.DropColumn(
                name: "amount",
                table: "payments");

            migrationBuilder.DropColumn(
                name: "contract_contractid",
                table: "payments");

            migrationBuilder.DropColumn(
                name: "description",
                table: "payments");

            migrationBuilder.DropColumn(
                name: "cashier_userid",
                table: "attention");

            migrationBuilder.DropColumn(
                name: "completedat",
                table: "attention");

            migrationBuilder.DropColumn(
                name: "contract_contractid",
                table: "attention");

            migrationBuilder.DropColumn(
                name: "createdat",
                table: "attention");

            migrationBuilder.DropColumn(
                name: "notes",
                table: "attention");
        }
    }
}
