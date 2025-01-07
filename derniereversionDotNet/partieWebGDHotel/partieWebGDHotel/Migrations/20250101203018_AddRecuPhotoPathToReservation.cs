using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace partieWebGDHotel.Migrations
{
    /// <inheritdoc />
    public partial class AddRecuPhotoPathToReservation : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ValidationReservations");

            migrationBuilder.AddColumn<byte[]>(
                name: "RecuPhotoPath",
                table: "Reservation",
                type: "longblob",
                nullable: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "RecuPhotoPath",
                table: "Reservation");

            migrationBuilder.CreateTable(
                name: "ValidationReservations",
                columns: table => new
                {
                    ID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    ReservationId = table.Column<int>(type: "int", nullable: false),
                    RecuPhotoPath = table.Column<string>(type: "longtext", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ValidationReservations", x => x.ID);
                    table.ForeignKey(
                        name: "FK_ValidationReservations_Reservation_ReservationId",
                        column: x => x.ReservationId,
                        principalTable: "Reservation",
                        principalColumn: "ID_Reservation",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateIndex(
                name: "IX_ValidationReservations_ReservationId",
                table: "ValidationReservations",
                column: "ReservationId");
        }
    }
}
