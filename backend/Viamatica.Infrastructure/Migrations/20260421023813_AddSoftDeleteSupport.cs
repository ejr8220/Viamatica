using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Viamatica.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddSoftDeleteSupport : Migration
    {
        private static void DropIndexIfExists(MigrationBuilder migrationBuilder, string tableName, string indexName)
        {
            migrationBuilder.Sql($"""
                IF EXISTS (
                    SELECT 1
                    FROM sys.indexes
                    WHERE name = N'{indexName}'
                      AND object_id = OBJECT_ID(N'[dbo].[{tableName}]')
                )
                BEGIN
                    DROP INDEX [{indexName}] ON [dbo].[{tableName}];
                END
                """);
        }

        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            DropIndexIfExists(migrationBuilder, "usertable", "IX_usertable_email");
            DropIndexIfExists(migrationBuilder, "usertable", "IX_usertable_username");
            DropIndexIfExists(migrationBuilder, "client", "IX_client_email");
            DropIndexIfExists(migrationBuilder, "client", "IX_client_identification");

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
            DropIndexIfExists(migrationBuilder, "usertable", "IX_usertable_email");
            DropIndexIfExists(migrationBuilder, "usertable", "IX_usertable_username");
            DropIndexIfExists(migrationBuilder, "client", "IX_client_email");
            DropIndexIfExists(migrationBuilder, "client", "IX_client_identification");

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
