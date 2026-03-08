using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class UpdateModelsAndRelationships : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "fk_contracts_renters_renter_id",
                table: "contracts");

            migrationBuilder.DropForeignKey(
                name: "fk_contracts_units_unit_id",
                table: "contracts");

            migrationBuilder.DropForeignKey(
                name: "fk_expenses_properties_property_id",
                table: "expenses");

            migrationBuilder.DropForeignKey(
                name: "fk_renters_users_user_id",
                table: "renters");

            migrationBuilder.DropForeignKey(
                name: "fk_units_properties_property_id",
                table: "units");

            migrationBuilder.DropForeignKey(
                name: "fk_users_role_role_id",
                table: "users");

            migrationBuilder.DropPrimaryKey(
                name: "pk_users",
                table: "users");

            migrationBuilder.DropColumn(
                name: "tenat_id",
                table: "users");

            migrationBuilder.RenameTable(
                name: "users",
                newName: "user");

            migrationBuilder.RenameColumn(
                name: "tenat_id",
                table: "units",
                newName: "tenant_id");

            migrationBuilder.RenameColumn(
                name: "tenat_id",
                table: "renters",
                newName: "tenant_id");

            migrationBuilder.RenameColumn(
                name: "tenat_id",
                table: "properties",
                newName: "tenant_id");

            migrationBuilder.RenameColumn(
                name: "tenat_id",
                table: "payments",
                newName: "tenant_id");

            migrationBuilder.RenameColumn(
                name: "tenat_id",
                table: "expenses",
                newName: "tenant_id");

            migrationBuilder.RenameColumn(
                name: "tenat_id",
                table: "contracts",
                newName: "tenant_id");

            migrationBuilder.RenameIndex(
                name: "ix_users_role_id",
                table: "user",
                newName: "ix_user_role_id");

            migrationBuilder.AddColumn<Guid>(
                name: "created_by",
                table: "tenants",
                type: "uuid",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "updated_at",
                table: "tenants",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<Guid>(
                name: "updated_by",
                table: "tenants",
                type: "uuid",
                nullable: true);

            migrationBuilder.AlterColumn<Guid>(
                name: "user_id",
                table: "renters",
                type: "uuid",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uuid");

            migrationBuilder.AddColumn<Guid>(
                name: "contract_id",
                table: "payments",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<Guid>(
                name: "tenant_id",
                table: "user",
                type: "uuid",
                nullable: true);

            migrationBuilder.AddPrimaryKey(
                name: "pk_user",
                table: "user",
                column: "id");

            migrationBuilder.CreateTable(
                name: "maintenance_tickets",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    unit_id = table.Column<Guid>(type: "uuid", nullable: false),
                    renter_id = table.Column<Guid>(type: "uuid", nullable: false),
                    subject = table.Column<string>(type: "text", nullable: false),
                    description = table.Column<string>(type: "text", nullable: false),
                    ticket_status = table.Column<byte>(type: "smallint", nullable: false),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    created_by = table.Column<Guid>(type: "uuid", nullable: true),
                    updated_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    updated_by = table.Column<Guid>(type: "uuid", nullable: true),
                    is_deleted = table.Column<bool>(type: "boolean", nullable: false),
                    tenant_id = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_maintenance_tickets", x => x.id);
                    table.ForeignKey(
                        name: "fk_maintenance_tickets_renters_renter_id",
                        column: x => x.renter_id,
                        principalTable: "renters",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_maintenance_tickets_units_unit_id",
                        column: x => x.unit_id,
                        principalTable: "units",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "images",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    image_path = table.Column<string>(type: "text", nullable: false),
                    ticket_id = table.Column<Guid>(type: "uuid", nullable: false),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    created_by = table.Column<Guid>(type: "uuid", nullable: true),
                    updated_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    updated_by = table.Column<Guid>(type: "uuid", nullable: true),
                    is_deleted = table.Column<bool>(type: "boolean", nullable: false),
                    tenant_id = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_images", x => x.id);
                    table.ForeignKey(
                        name: "fk_images_maintenance_tickets_ticket_id",
                        column: x => x.ticket_id,
                        principalTable: "maintenance_tickets",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "ix_payments_contract_id",
                table: "payments",
                column: "contract_id");

            migrationBuilder.CreateIndex(
                name: "ix_user_tenant_id",
                table: "user",
                column: "tenant_id");

            migrationBuilder.CreateIndex(
                name: "ix_images_ticket_id",
                table: "images",
                column: "ticket_id");

            migrationBuilder.CreateIndex(
                name: "ix_maintenance_tickets_renter_id",
                table: "maintenance_tickets",
                column: "renter_id");

            migrationBuilder.CreateIndex(
                name: "ix_maintenance_tickets_unit_id",
                table: "maintenance_tickets",
                column: "unit_id");

            migrationBuilder.AddForeignKey(
                name: "fk_contracts_renters_renter_id",
                table: "contracts",
                column: "renter_id",
                principalTable: "renters",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "fk_contracts_units_unit_id",
                table: "contracts",
                column: "unit_id",
                principalTable: "units",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "fk_expenses_properties_property_id",
                table: "expenses",
                column: "property_id",
                principalTable: "properties",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "fk_payments_contracts_contract_id",
                table: "payments",
                column: "contract_id",
                principalTable: "contracts",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "fk_renters_user_user_id",
                table: "renters",
                column: "user_id",
                principalTable: "user",
                principalColumn: "id",
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.AddForeignKey(
                name: "fk_units_properties_property_id",
                table: "units",
                column: "property_id",
                principalTable: "properties",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "fk_user_role_role_id",
                table: "user",
                column: "role_id",
                principalTable: "role",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "fk_user_tenants_tenant_id",
                table: "user",
                column: "tenant_id",
                principalTable: "tenants",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "fk_contracts_renters_renter_id",
                table: "contracts");

            migrationBuilder.DropForeignKey(
                name: "fk_contracts_units_unit_id",
                table: "contracts");

            migrationBuilder.DropForeignKey(
                name: "fk_expenses_properties_property_id",
                table: "expenses");

            migrationBuilder.DropForeignKey(
                name: "fk_payments_contracts_contract_id",
                table: "payments");

            migrationBuilder.DropForeignKey(
                name: "fk_renters_user_user_id",
                table: "renters");

            migrationBuilder.DropForeignKey(
                name: "fk_units_properties_property_id",
                table: "units");

            migrationBuilder.DropForeignKey(
                name: "fk_user_role_role_id",
                table: "user");

            migrationBuilder.DropForeignKey(
                name: "fk_user_tenants_tenant_id",
                table: "user");

            migrationBuilder.DropTable(
                name: "images");

            migrationBuilder.DropTable(
                name: "maintenance_tickets");

            migrationBuilder.DropIndex(
                name: "ix_payments_contract_id",
                table: "payments");

            migrationBuilder.DropPrimaryKey(
                name: "pk_user",
                table: "user");

            migrationBuilder.DropIndex(
                name: "ix_user_tenant_id",
                table: "user");

            migrationBuilder.DropColumn(
                name: "created_by",
                table: "tenants");

            migrationBuilder.DropColumn(
                name: "updated_at",
                table: "tenants");

            migrationBuilder.DropColumn(
                name: "updated_by",
                table: "tenants");

            migrationBuilder.DropColumn(
                name: "contract_id",
                table: "payments");

            migrationBuilder.DropColumn(
                name: "tenant_id",
                table: "user");

            migrationBuilder.RenameTable(
                name: "user",
                newName: "users");

            migrationBuilder.RenameColumn(
                name: "tenant_id",
                table: "units",
                newName: "tenat_id");

            migrationBuilder.RenameColumn(
                name: "tenant_id",
                table: "renters",
                newName: "tenat_id");

            migrationBuilder.RenameColumn(
                name: "tenant_id",
                table: "properties",
                newName: "tenat_id");

            migrationBuilder.RenameColumn(
                name: "tenant_id",
                table: "payments",
                newName: "tenat_id");

            migrationBuilder.RenameColumn(
                name: "tenant_id",
                table: "expenses",
                newName: "tenat_id");

            migrationBuilder.RenameColumn(
                name: "tenant_id",
                table: "contracts",
                newName: "tenat_id");

            migrationBuilder.RenameIndex(
                name: "ix_user_role_id",
                table: "users",
                newName: "ix_users_role_id");

            migrationBuilder.AlterColumn<Guid>(
                name: "user_id",
                table: "renters",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"),
                oldClrType: typeof(Guid),
                oldType: "uuid",
                oldNullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "tenat_id",
                table: "users",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddPrimaryKey(
                name: "pk_users",
                table: "users",
                column: "id");

            migrationBuilder.AddForeignKey(
                name: "fk_contracts_renters_renter_id",
                table: "contracts",
                column: "renter_id",
                principalTable: "renters",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "fk_contracts_units_unit_id",
                table: "contracts",
                column: "unit_id",
                principalTable: "units",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "fk_expenses_properties_property_id",
                table: "expenses",
                column: "property_id",
                principalTable: "properties",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "fk_renters_users_user_id",
                table: "renters",
                column: "user_id",
                principalTable: "users",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "fk_units_properties_property_id",
                table: "units",
                column: "property_id",
                principalTable: "properties",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "fk_users_role_role_id",
                table: "users",
                column: "role_id",
                principalTable: "role",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
