using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddMockPaymentFields : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "actual_payment_date",
                table: "payments",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "transaction_id",
                table: "payments",
                type: "character varying(100)",
                maxLength: 100,
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "unit_id",
                table: "Expenses",
                type: "uuid",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "ix_expenses_unit_id",
                table: "Expenses",
                column: "unit_id");

            migrationBuilder.AddForeignKey(
                name: "fk_expenses_units_unit_id",
                table: "Expenses",
                column: "unit_id",
                principalTable: "units",
                principalColumn: "id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "fk_expenses_units_unit_id",
                table: "Expenses");

            migrationBuilder.DropIndex(
                name: "ix_expenses_unit_id",
                table: "Expenses");

            migrationBuilder.DropColumn(
                name: "actual_payment_date",
                table: "payments");

            migrationBuilder.DropColumn(
                name: "transaction_id",
                table: "payments");

            migrationBuilder.DropColumn(
                name: "unit_id",
                table: "Expenses");
        }
    }
}
