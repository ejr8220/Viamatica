using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Viamatica.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddSoftDeleteSupport : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_usertable_email",
                table: "usertable");

            migrationBuilder.DropIndex(
                name: "IX_usertable_username",
                table: "usertable");

            migrationBuilder.DropIndex(
                name: "IX_client_email",
                table: "client");

            migrationBuilder.DropIndex(
                name: "IX_client_identification",
                table: "client");

            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "deletedat",
                table: "usertable",
                type: "datetimeoffset",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "isdeleted",
                table: "usertable",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "deletedat",
                table: "contract",
                type: "datetimeoffset",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "isdeleted",
                table: "contract",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "deletedat",
                table: "client",
                type: "datetimeoffset",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "isdeleted",
                table: "client",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.CreateIndex(
                name: "IX_usertable_email",
                table: "usertable",
                column: "email",
                unique: true,
                filter: "[isdeleted] = 0");

            migrationBuilder.CreateIndex(
                name: "IX_usertable_username",
                table: "usertable",
                column: "username",
                unique: true,
                filter: "[isdeleted] = 0");

            migrationBuilder.CreateIndex(
                name: "IX_client_email",
                table: "client",
                column: "email",
                unique: true,
                filter: "[isdeleted] = 0");

            migrationBuilder.CreateIndex(
                name: "IX_client_identification",
                table: "client",
                column: "identification",
                unique: true,
                filter: "[isdeleted] = 0");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_usertable_email",
                table: "usertable");

            migrationBuilder.DropIndex(
                name: "IX_usertable_username",
                table: "usertable");

            migrationBuilder.DropIndex(
                name: "IX_client_email",
                table: "client");

            migrationBuilder.DropIndex(
                name: "IX_client_identification",
                table: "client");

            migrationBuilder.DropColumn(
                name: "deletedat",
                table: "usertable");

            migrationBuilder.DropColumn(
                name: "isdeleted",
                table: "usertable");

            migrationBuilder.DropColumn(
                name: "deletedat",
                table: "contract");

            migrationBuilder.DropColumn(
                name: "isdeleted",
                table: "contract");

            migrationBuilder.DropColumn(
                name: "deletedat",
                table: "client");

            migrationBuilder.DropColumn(
                name: "isdeleted",
                table: "client");

            migrationBuilder.CreateIndex(
                name: "IX_usertable_email",
                table: "usertable",
                column: "email",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_usertable_username",
                table: "usertable",
                column: "username",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_client_email",
                table: "client",
                column: "email",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_client_identification",
                table: "client",
                column: "identification",
                unique: true);
        }
    }
}
