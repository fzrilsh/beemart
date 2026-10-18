using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BeeMart.Api.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "LtPaymentMethod",
                columns: table => new
                {
                    IdPaymentMethod = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    MethodName = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Stsrc = table.Column<bool>(type: "bit", nullable: false),
                    Datein = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Userin = table.Column<string>(type: "nvarchar(36)", maxLength: 36, nullable: false),
                    Dateup = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Userup = table.Column<string>(type: "nvarchar(36)", maxLength: 36, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LtPaymentMethod", x => x.IdPaymentMethod);
                });

            migrationBuilder.CreateTable(
                name: "MsProduct",
                columns: table => new
                {
                    IdProduct = table.Column<string>(type: "nvarchar(36)", maxLength: 36, nullable: false),
                    Name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Weight = table.Column<decimal>(type: "decimal(10,2)", precision: 10, scale: 2, nullable: false),
                    Stock = table.Column<int>(type: "int", nullable: false),
                    Price = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    Discount = table.Column<decimal>(type: "decimal(5,2)", precision: 5, scale: 2, nullable: false),
                    Stsrc = table.Column<bool>(type: "bit", nullable: false),
                    Datein = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Userin = table.Column<string>(type: "nvarchar(36)", maxLength: 36, nullable: false),
                    Dateup = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Userup = table.Column<string>(type: "nvarchar(36)", maxLength: 36, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MsProduct", x => x.IdProduct);
                });

            migrationBuilder.CreateTable(
                name: "MsUser",
                columns: table => new
                {
                    IdUser = table.Column<string>(type: "nvarchar(5)", maxLength: 5, nullable: false),
                    Username = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    FullName = table.Column<string>(type: "nvarchar(300)", maxLength: 300, nullable: false),
                    PasswordHash = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    Role = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    Stsrc = table.Column<bool>(type: "bit", nullable: false),
                    Datein = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Userin = table.Column<string>(type: "nvarchar(36)", maxLength: 36, nullable: false),
                    Dateup = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Userup = table.Column<string>(type: "nvarchar(36)", maxLength: 36, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MsUser", x => x.IdUser);
                });

            migrationBuilder.CreateTable(
                name: "TrInvoiceLock",
                columns: table => new
                {
                    IdInvoiceLock = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    InvoiceNo = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    IdUser = table.Column<string>(type: "nvarchar(5)", maxLength: 5, nullable: false),
                    LockDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    LockDateOnly = table.Column<DateTime>(type: "date", nullable: false),
                    RunningNumber = table.Column<int>(type: "int", nullable: false),
                    Stsrc = table.Column<bool>(type: "bit", nullable: false),
                    Datein = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Userin = table.Column<string>(type: "nvarchar(36)", maxLength: 36, nullable: false),
                    Dateup = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Userup = table.Column<string>(type: "nvarchar(36)", maxLength: 36, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TrInvoiceLock", x => x.IdInvoiceLock);
                    table.ForeignKey(
                        name: "FK_TrInvoiceLock_MsUser_IdUser",
                        column: x => x.IdUser,
                        principalTable: "MsUser",
                        principalColumn: "IdUser",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "TrTransaction",
                columns: table => new
                {
                    IdTransaction = table.Column<string>(type: "nvarchar(36)", maxLength: 36, nullable: false),
                    InvoiceNo = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    TransactionDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    IdUser = table.Column<string>(type: "nvarchar(5)", maxLength: 5, nullable: false),
                    IdPaymentMethod = table.Column<int>(type: "int", nullable: false),
                    TotalPrice = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    Stsrc = table.Column<bool>(type: "bit", nullable: false),
                    Datein = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Userin = table.Column<string>(type: "nvarchar(36)", maxLength: 36, nullable: false),
                    Dateup = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Userup = table.Column<string>(type: "nvarchar(36)", maxLength: 36, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TrTransaction", x => x.IdTransaction);
                    table.ForeignKey(
                        name: "FK_TrTransaction_LtPaymentMethod_IdPaymentMethod",
                        column: x => x.IdPaymentMethod,
                        principalTable: "LtPaymentMethod",
                        principalColumn: "IdPaymentMethod");
                    table.ForeignKey(
                        name: "FK_TrTransaction_MsUser_IdUser",
                        column: x => x.IdUser,
                        principalTable: "MsUser",
                        principalColumn: "IdUser");
                });

            migrationBuilder.CreateTable(
                name: "TrTransactionDetail",
                columns: table => new
                {
                    IdTransactionDetail = table.Column<string>(type: "nvarchar(36)", maxLength: 36, nullable: false),
                    IdTransaction = table.Column<string>(type: "nvarchar(36)", maxLength: 36, nullable: false),
                    IdProduct = table.Column<string>(type: "nvarchar(36)", maxLength: 36, nullable: false),
                    Qty = table.Column<int>(type: "int", nullable: false),
                    Price = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    Discount = table.Column<decimal>(type: "decimal(5,2)", precision: 5, scale: 2, nullable: false),
                    SubTotal = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    Stsrc = table.Column<bool>(type: "bit", nullable: false),
                    Datein = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Userin = table.Column<string>(type: "nvarchar(36)", maxLength: 36, nullable: false),
                    Dateup = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Userup = table.Column<string>(type: "nvarchar(36)", maxLength: 36, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TrTransactionDetail", x => x.IdTransactionDetail);
                    table.ForeignKey(
                        name: "FK_TrTransactionDetail_MsProduct_IdProduct",
                        column: x => x.IdProduct,
                        principalTable: "MsProduct",
                        principalColumn: "IdProduct");
                    table.ForeignKey(
                        name: "FK_TrTransactionDetail_TrTransaction_IdTransaction",
                        column: x => x.IdTransaction,
                        principalTable: "TrTransaction",
                        principalColumn: "IdTransaction",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_MsProduct_Name",
                table: "MsProduct",
                column: "Name");

            migrationBuilder.CreateIndex(
                name: "IX_MsUser_Username",
                table: "MsUser",
                column: "Username",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_TrInvoiceLock_IdUser_LockDateOnly",
                table: "TrInvoiceLock",
                columns: new[] { "IdUser", "LockDateOnly" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_TrInvoiceLock_InvoiceNo",
                table: "TrInvoiceLock",
                column: "InvoiceNo",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_TrInvoiceLock_LockDateOnly",
                table: "TrInvoiceLock",
                column: "LockDateOnly");

            migrationBuilder.CreateIndex(
                name: "IX_TrTransaction_IdPaymentMethod",
                table: "TrTransaction",
                column: "IdPaymentMethod");

            migrationBuilder.CreateIndex(
                name: "IX_TrTransaction_IdUser",
                table: "TrTransaction",
                column: "IdUser");

            migrationBuilder.CreateIndex(
                name: "IX_TrTransaction_InvoiceNo",
                table: "TrTransaction",
                column: "InvoiceNo",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_TrTransaction_TransactionDate",
                table: "TrTransaction",
                column: "TransactionDate");

            migrationBuilder.CreateIndex(
                name: "IX_TrTransactionDetail_IdProduct",
                table: "TrTransactionDetail",
                column: "IdProduct");

            migrationBuilder.CreateIndex(
                name: "IX_TrTransactionDetail_IdTransaction",
                table: "TrTransactionDetail",
                column: "IdTransaction");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "TrInvoiceLock");

            migrationBuilder.DropTable(
                name: "TrTransactionDetail");

            migrationBuilder.DropTable(
                name: "MsProduct");

            migrationBuilder.DropTable(
                name: "TrTransaction");

            migrationBuilder.DropTable(
                name: "LtPaymentMethod");

            migrationBuilder.DropTable(
                name: "MsUser");
        }
    }
}
