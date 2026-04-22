using System;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Viamatica.Infrastructure.Data;

#nullable disable

namespace Viamatica.Infrastructure.Migrations
{
    /// <inheritdoc />
    [DbContext(typeof(ViamaticaDbContext))]
    [Migration("20250101000000_InitialCreate")]
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "attentionstatus",
                columns: table => new
                {
                    statusid = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    description = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_attentionstatus", x => x.statusid);
                });

            migrationBuilder.CreateTable(
                name: "attentiontype",
                columns: table => new
                {
                    attentiontypeid = table.Column<string>(type: "nvarchar(3)", maxLength: 3, nullable: false),
                    description = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_attentiontype", x => x.attentiontypeid);
                });

            migrationBuilder.CreateTable(
                name: "cash",
                columns: table => new
                {
                    cashid = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    cashdescription = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    active = table.Column<string>(type: "nvarchar(1)", maxLength: 1, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_cash", x => x.cashid);
                });

            migrationBuilder.CreateTable(
                name: "client",
                columns: table => new
                {
                    clientid = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    name = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    lastname = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    identification = table.Column<string>(type: "nvarchar(13)", maxLength: 13, nullable: false),
                    email = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    phonenumber = table.Column<string>(type: "nvarchar(13)", maxLength: 13, nullable: false),
                    address = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    referenceaddress = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_client", x => x.clientid);
                });

            migrationBuilder.CreateTable(
                name: "methodpayment",
                columns: table => new
                {
                    methodpaymentid = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    description = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_methodpayment", x => x.methodpaymentid);
                });

            migrationBuilder.CreateTable(
                name: "rol",
                columns: table => new
                {
                    rolid = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    rolname = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_rol", x => x.rolid);
                });

            migrationBuilder.CreateTable(
                name: "service",
                columns: table => new
                {
                    serviceid = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    servicename = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    servicedescription = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                    price = table.Column<decimal>(type: "decimal(18,2)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_service", x => x.serviceid);
                });

            migrationBuilder.CreateTable(
                name: "statuscontract",
                columns: table => new
                {
                    statusid = table.Column<string>(type: "nvarchar(3)", maxLength: 3, nullable: false),
                    description = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_statuscontract", x => x.statusid);
                });

            migrationBuilder.CreateTable(
                name: "userstatus",
                columns: table => new
                {
                    statusid = table.Column<string>(type: "nvarchar(3)", maxLength: 3, nullable: false),
                    description = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_userstatus", x => x.statusid);
                });

            migrationBuilder.CreateTable(
                name: "payments",
                columns: table => new
                {
                    paymentid = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    paymentdate = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    client_clientid = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_payments", x => x.paymentid);
                    table.ForeignKey(
                        name: "FK_payments_client_client_clientid",
                        column: x => x.client_clientid,
                        principalTable: "client",
                        principalColumn: "clientid",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "device",
                columns: table => new
                {
                    deviceid = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    devicename = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    service_serviceid = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_device", x => x.deviceid);
                    table.ForeignKey(
                        name: "FK_device_service_service_serviceid",
                        column: x => x.service_serviceid,
                        principalTable: "service",
                        principalColumn: "serviceid",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "usertable",
                columns: table => new
                {
                    userid = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    username = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    email = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    password = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    rol_rolid = table.Column<int>(type: "int", nullable: false),
                    userstatus_statusid = table.Column<string>(type: "nvarchar(3)", maxLength: 3, nullable: false),
                    userapproval = table.Column<int>(type: "int", nullable: false),
                    dateapproval = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_usertable", x => x.userid);
                    table.ForeignKey(
                        name: "FK_usertable_rol_rol_rolid",
                        column: x => x.rol_rolid,
                        principalTable: "rol",
                        principalColumn: "rolid",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_usertable_userstatus_userstatus_statusid",
                        column: x => x.userstatus_statusid,
                        principalTable: "userstatus",
                        principalColumn: "statusid",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "contract",
                columns: table => new
                {
                    contractid = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    startdate = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    enddate = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    service_serviceid = table.Column<int>(type: "int", nullable: false),
                    statuscontract_statusid = table.Column<string>(type: "nvarchar(3)", maxLength: 3, nullable: false),
                    client_clientid = table.Column<int>(type: "int", nullable: false),
                    methodpayment_methodpaymentid = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_contract", x => x.contractid);
                    table.ForeignKey(
                        name: "FK_contract_client_client_clientid",
                        column: x => x.client_clientid,
                        principalTable: "client",
                        principalColumn: "clientid",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_contract_methodpayment_methodpayment_methodpaymentid",
                        column: x => x.methodpayment_methodpaymentid,
                        principalTable: "methodpayment",
                        principalColumn: "methodpaymentid",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_contract_service_service_serviceid",
                        column: x => x.service_serviceid,
                        principalTable: "service",
                        principalColumn: "serviceid",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_contract_statuscontract_statuscontract_statusid",
                        column: x => x.statuscontract_statusid,
                        principalTable: "statuscontract",
                        principalColumn: "statusid",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "turn",
                columns: table => new
                {
                    turnid = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    description = table.Column<string>(type: "nvarchar(6)", maxLength: 6, nullable: false),
                    date = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    cash_cashid = table.Column<int>(type: "int", nullable: false),
                    usergestorid = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_turn", x => x.turnid);
                    table.ForeignKey(
                        name: "FK_turn_cash_cash_cashid",
                        column: x => x.cash_cashid,
                        principalTable: "cash",
                        principalColumn: "cashid",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_turn_usertable_usergestorid",
                        column: x => x.usergestorid,
                        principalTable: "usertable",
                        principalColumn: "userid",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "usercash",
                columns: table => new
                {
                    user_userid = table.Column<int>(type: "int", nullable: false),
                    cash_cashid = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_usercash", x => new { x.user_userid, x.cash_cashid });
                    table.ForeignKey(
                        name: "FK_usercash_cash_cash_cashid",
                        column: x => x.cash_cashid,
                        principalTable: "cash",
                        principalColumn: "cashid",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_usercash_usertable_user_userid",
                        column: x => x.user_userid,
                        principalTable: "usertable",
                        principalColumn: "userid",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "attention",
                columns: table => new
                {
                    attentionid = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    turn_turnid = table.Column<int>(type: "int", nullable: false),
                    client_clientid = table.Column<int>(type: "int", nullable: false),
                    attentiontype_attentiontypeid = table.Column<string>(type: "nvarchar(3)", maxLength: 3, nullable: false),
                    attentionstatus_statusid = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_attention", x => x.attentionid);
                    table.ForeignKey(
                        name: "FK_attention_attentionstatus_attentionstatus_statusid",
                        column: x => x.attentionstatus_statusid,
                        principalTable: "attentionstatus",
                        principalColumn: "statusid",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_attention_attentiontype_attentiontype_attentiontypeid",
                        column: x => x.attentiontype_attentiontypeid,
                        principalTable: "attentiontype",
                        principalColumn: "attentiontypeid",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_attention_client_client_clientid",
                        column: x => x.client_clientid,
                        principalTable: "client",
                        principalColumn: "clientid",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_attention_turn_turn_turnid",
                        column: x => x.turn_turnid,
                        principalTable: "turn",
                        principalColumn: "turnid",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_attention_attentionstatus_statusid",
                table: "attention",
                column: "attentionstatus_statusid");

            migrationBuilder.CreateIndex(
                name: "IX_attention_attentiontype_attentiontypeid",
                table: "attention",
                column: "attentiontype_attentiontypeid");

            migrationBuilder.CreateIndex(
                name: "IX_attention_client_clientid",
                table: "attention",
                column: "client_clientid");

            migrationBuilder.CreateIndex(
                name: "IX_attention_turn_turnid",
                table: "attention",
                column: "turn_turnid");

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

            migrationBuilder.CreateIndex(
                name: "IX_contract_client_clientid",
                table: "contract",
                column: "client_clientid");

            migrationBuilder.CreateIndex(
                name: "IX_contract_methodpayment_methodpaymentid",
                table: "contract",
                column: "methodpayment_methodpaymentid");

            migrationBuilder.CreateIndex(
                name: "IX_contract_service_serviceid",
                table: "contract",
                column: "service_serviceid");

            migrationBuilder.CreateIndex(
                name: "IX_contract_statuscontract_statusid",
                table: "contract",
                column: "statuscontract_statusid");

            migrationBuilder.CreateIndex(
                name: "IX_device_service_serviceid",
                table: "device",
                column: "service_serviceid");

            migrationBuilder.CreateIndex(
                name: "IX_payments_client_clientid",
                table: "payments",
                column: "client_clientid");

            migrationBuilder.CreateIndex(
                name: "IX_turn_cash_cashid",
                table: "turn",
                column: "cash_cashid");

            migrationBuilder.CreateIndex(
                name: "IX_turn_usergestorid",
                table: "turn",
                column: "usergestorid");

            migrationBuilder.CreateIndex(
                name: "IX_usercash_cash_cashid",
                table: "usercash",
                column: "cash_cashid");

            migrationBuilder.CreateIndex(
                name: "IX_usertable_email",
                table: "usertable",
                column: "email",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_usertable_rol_rolid",
                table: "usertable",
                column: "rol_rolid");

            migrationBuilder.CreateIndex(
                name: "IX_usertable_username",
                table: "usertable",
                column: "username",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_usertable_userstatus_statusid",
                table: "usertable",
                column: "userstatus_statusid");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "attention");

            migrationBuilder.DropTable(
                name: "contract");

            migrationBuilder.DropTable(
                name: "device");

            migrationBuilder.DropTable(
                name: "payments");

            migrationBuilder.DropTable(
                name: "turn");

            migrationBuilder.DropTable(
                name: "usercash");

            migrationBuilder.DropTable(
                name: "attentionstatus");

            migrationBuilder.DropTable(
                name: "attentiontype");

            migrationBuilder.DropTable(
                name: "methodpayment");

            migrationBuilder.DropTable(
                name: "statuscontract");

            migrationBuilder.DropTable(
                name: "service");

            migrationBuilder.DropTable(
                name: "client");

            migrationBuilder.DropTable(
                name: "cash");

            migrationBuilder.DropTable(
                name: "usertable");

            migrationBuilder.DropTable(
                name: "rol");

            migrationBuilder.DropTable(
                name: "userstatus");
        }
    }
}
