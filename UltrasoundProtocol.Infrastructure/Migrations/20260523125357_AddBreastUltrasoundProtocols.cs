using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace UltrasoundProtocol.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddBreastUltrasoundProtocols : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "BreastUltrasoundProtocols",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    UltrasoundExamId = table.Column<Guid>(type: "uuid", nullable: false),
                    PatientId = table.Column<Guid>(type: "uuid", nullable: false),
                    DoctorUsername = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    DoctorName = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true),
                    MedicalInstitutionName = table.Column<string>(type: "character varying(300)", maxLength: 300, nullable: true),
                    MedicalInstitutionAddress = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    ProtocolNumber = table.Column<string>(type: "character varying(80)", maxLength: 80, nullable: true),
                    ExamDate = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    UltrasoundMachine = table.Column<string>(type: "character varying(150)", maxLength: 150, nullable: true),
                    Probe = table.Column<string>(type: "character varying(150)", maxLength: 150, nullable: true),
                    UltrasoundExamNumber = table.Column<string>(type: "character varying(80)", maxLength: 80, nullable: true),
                    PatientWeightKg = table.Column<decimal>(type: "numeric", nullable: true),
                    PatientHeightCm = table.Column<decimal>(type: "numeric", nullable: true),
                    Symmetry = table.Column<string>(type: "character varying(120)", maxLength: 120, nullable: true),
                    RightBreastJson = table.Column<string>(type: "jsonb", nullable: false),
                    LeftBreastJson = table.Column<string>(type: "jsonb", nullable: false),
                    LesionJson = table.Column<string>(type: "jsonb", nullable: false),
                    CystsJson = table.Column<string>(type: "jsonb", nullable: false),
                    RegionalLymphNodesJson = table.Column<string>(type: "jsonb", nullable: false),
                    AdditionalInfo = table.Column<string>(type: "text", nullable: true),
                    Findings = table.Column<string>(type: "text", nullable: false),
                    Conclusion = table.Column<string>(type: "text", nullable: false),
                    Birads = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: true),
                    Recommendations = table.Column<string>(type: "text", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp without time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BreastUltrasoundProtocols", x => x.Id);
                    table.ForeignKey(
                        name: "FK_BreastUltrasoundProtocols_UltrasoundExams_UltrasoundExamId",
                        column: x => x.UltrasoundExamId,
                        principalTable: "UltrasoundExams",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_BreastUltrasoundProtocols_Users_PatientId",
                        column: x => x.PatientId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_BreastUltrasoundProtocols_PatientId",
                table: "BreastUltrasoundProtocols",
                column: "PatientId");

            migrationBuilder.CreateIndex(
                name: "IX_BreastUltrasoundProtocols_UltrasoundExamId",
                table: "BreastUltrasoundProtocols",
                column: "UltrasoundExamId",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "BreastUltrasoundProtocols");
        }
    }
}
