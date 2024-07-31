using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Parser.Migrations
{
    /// <inheritdoc />
    public partial class UpdateEndDateToDateTime : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Преобразование столбца "EndDate" в тип timestamp with time zone
            migrationBuilder.Sql(
                "ALTER TABLE \"Purchases\" ALTER COLUMN \"EndDate\" TYPE timestamp with time zone USING \"EndDate\"::timestamp with time zone;"
            );
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "EndDate",
                table: "Purchases",
                type: "text",
                nullable: true,
                oldClrType: typeof(DateTime),
                oldType: "timestamp with time zone",
                oldNullable: false
            );
        }
    }
}
