using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Viamatica.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class FinalBackendCompliance : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_usertable_email",
                table: "usertable");

            migrationBuilder.AlterColumn<string>(
                name: "password",
                table: "usertable",
                type: "nvarchar(512)",
                maxLength: 512,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(255)",
                oldMaxLength: 255);

            migrationBuilder.AlterColumn<string>(
                name: "email",
                table: "usertable",
                type: "nvarchar(512)",
                maxLength: 512,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(100)",
                oldMaxLength: 100);

            migrationBuilder.AddColumn<string>(
                name: "emailhash",
                table: "usertable",
                type: "nvarchar(64)",
                maxLength: 64,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "identification",
                table: "usertable",
                type: "nvarchar(512)",
                maxLength: 512,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "identificationhash",
                table: "usertable",
                type: "nvarchar(64)",
                maxLength: 64,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "attentiontype_attentiontypeid",
                table: "turn",
                type: "nvarchar(3)",
                maxLength: 3,
                nullable: false,
                defaultValue: "");

            migrationBuilder.Sql("""
                UPDATE statuscontract
                SET description = CASE statusid
                    WHEN 'ACT' THEN N'Vigente'
                    WHEN 'REP' THEN N'Sustituido'
                    ELSE description
                END
                WHERE statusid IN ('ACT', 'REP');

                IF NOT EXISTS (SELECT 1 FROM statuscontract WHERE statusid = 'REN')
                BEGIN
                    INSERT INTO statuscontract (statusid, description)
                    VALUES ('REN', N'Renovación de servicio');
                END

                UPDATE turn
                SET attentiontype_attentiontypeid = CASE LEFT(description, 2)
                    WHEN 'AC' THEN 'GEN'
                    WHEN 'CT' THEN 'CTR'
                    WHEN 'PS' THEN 'PAY'
                    WHEN 'CS' THEN 'CSV'
                    WHEN 'FP' THEN 'CFP'
                    WHEN 'CA' THEN 'CAN'
                    ELSE 'GEN'
                END
                WHERE attentiontype_attentiontypeid = '';

                UPDATE usertable
                SET emailhash = CONVERT(varchar(64), HASHBYTES('SHA2_256', UPPER(LTRIM(RTRIM(email)))), 2)
                WHERE emailhash = '' AND email IS NOT NULL AND email <> '';

                UPDATE usertable
                SET identificationhash = CONVERT(varchar(64), HASHBYTES('SHA2_256', CONCAT('MIG-', userid)), 2)
                WHERE identificationhash = '';
                """);

            migrationBuilder.CreateIndex(
                name: "IX_usertable_emailhash",
                table: "usertable",
                column: "emailhash",
                unique: true,
                filter: "[isdeleted] = 0");

            migrationBuilder.CreateIndex(
                name: "IX_usertable_identificationhash",
                table: "usertable",
                column: "identificationhash",
                unique: true,
                filter: "[isdeleted] = 0");

            migrationBuilder.CreateIndex(
                name: "IX_turn_attentiontype_attentiontypeid",
                table: "turn",
                column: "attentiontype_attentiontypeid");

            migrationBuilder.AddForeignKey(
                name: "FK_turn_attentiontype_attentiontype_attentiontypeid",
                table: "turn",
                column: "attentiontype_attentiontypeid",
                principalTable: "attentiontype",
                principalColumn: "attentiontypeid",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_turn_attentiontype_attentiontype_attentiontypeid",
                table: "turn");

            migrationBuilder.DropIndex(
                name: "IX_usertable_emailhash",
                table: "usertable");

            migrationBuilder.DropIndex(
                name: "IX_usertable_identificationhash",
                table: "usertable");

            migrationBuilder.DropIndex(
                name: "IX_turn_attentiontype_attentiontypeid",
                table: "turn");

            migrationBuilder.DropColumn(
                name: "emailhash",
                table: "usertable");

            migrationBuilder.DropColumn(
                name: "identification",
                table: "usertable");

            migrationBuilder.DropColumn(
                name: "identificationhash",
                table: "usertable");

            migrationBuilder.DropColumn(
                name: "attentiontype_attentiontypeid",
                table: "turn");

            migrationBuilder.AlterColumn<string>(
                name: "password",
                table: "usertable",
                type: "nvarchar(255)",
                maxLength: 255,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(512)",
                oldMaxLength: 512);

            migrationBuilder.AlterColumn<string>(
                name: "email",
                table: "usertable",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(512)",
                oldMaxLength: 512);

            migrationBuilder.CreateIndex(
                name: "IX_usertable_email",
                table: "usertable",
                column: "email",
                unique: true,
                filter: "[isdeleted] = 0");
        }
    }
}
