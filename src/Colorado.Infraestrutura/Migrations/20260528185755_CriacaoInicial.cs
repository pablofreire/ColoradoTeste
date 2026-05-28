using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Colorado.Infraestrutura.Migrations
{
    public partial class CriacaoInicial : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Clientes",
                columns: table => new
                {
                    CodigoCliente = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    RazaoSocial = table.Column<string>(type: "TEXT", maxLength: 200, nullable: false),
                    NomeFantasia = table.Column<string>(type: "TEXT", maxLength: 200, nullable: false),
                    TipoPessoa = table.Column<string>(type: "TEXT", maxLength: 1, nullable: false),
                    Documento = table.Column<string>(type: "TEXT", maxLength: 20, nullable: false),
                    Endereco = table.Column<string>(type: "TEXT", maxLength: 200, nullable: false),
                    Complemento = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false),
                    Bairro = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false),
                    Cidade = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false),
                    Cep = table.Column<string>(type: "TEXT", maxLength: 9, nullable: false),
                    Uf = table.Column<string>(type: "TEXT", maxLength: 2, nullable: false),
                    DataInsercao = table.Column<DateTime>(type: "TEXT", nullable: false),
                    UsuarioInsercao = table.Column<string>(type: "TEXT", maxLength: 50, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Clientes", x => x.CodigoCliente);
                });

            migrationBuilder.CreateTable(
                name: "TiposTelefone",
                columns: table => new
                {
                    CodigoTipoTelefone = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    DescricaoTipoTelefone = table.Column<string>(type: "TEXT", maxLength: 50, nullable: false),
                    DataInsercao = table.Column<DateTime>(type: "TEXT", nullable: false),
                    UsuarioInsercao = table.Column<string>(type: "TEXT", maxLength: 50, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TiposTelefone", x => x.CodigoTipoTelefone);
                });

            migrationBuilder.CreateTable(
                name: "Telefones",
                columns: table => new
                {
                    CodigoCliente = table.Column<int>(type: "INTEGER", nullable: false),
                    NumeroTelefone = table.Column<string>(type: "TEXT", maxLength: 20, nullable: false),
                    CodigoTipoTelefone = table.Column<int>(type: "INTEGER", nullable: false),
                    Operadora = table.Column<string>(type: "TEXT", maxLength: 50, nullable: false),
                    Ativo = table.Column<bool>(type: "INTEGER", nullable: false),
                    DataInsercao = table.Column<DateTime>(type: "TEXT", nullable: false),
                    UsuarioInsercao = table.Column<string>(type: "TEXT", maxLength: 50, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Telefones", x => new { x.CodigoCliente, x.NumeroTelefone });
                    table.ForeignKey(
                        name: "FK_Telefones_Clientes_CodigoCliente",
                        column: x => x.CodigoCliente,
                        principalTable: "Clientes",
                        principalColumn: "CodigoCliente",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Telefones_TiposTelefone_CodigoTipoTelefone",
                        column: x => x.CodigoTipoTelefone,
                        principalTable: "TiposTelefone",
                        principalColumn: "CodigoTipoTelefone",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.InsertData(
                table: "TiposTelefone",
                columns: new[] { "CodigoTipoTelefone", "DataInsercao", "DescricaoTipoTelefone", "UsuarioInsercao" },
                values: new object[] { 1, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "RESIDENCIAL", "SISTEMA" });

            migrationBuilder.InsertData(
                table: "TiposTelefone",
                columns: new[] { "CodigoTipoTelefone", "DataInsercao", "DescricaoTipoTelefone", "UsuarioInsercao" },
                values: new object[] { 2, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "COMERCIAL", "SISTEMA" });

            migrationBuilder.InsertData(
                table: "TiposTelefone",
                columns: new[] { "CodigoTipoTelefone", "DataInsercao", "DescricaoTipoTelefone", "UsuarioInsercao" },
                values: new object[] { 3, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "WHATSAPP", "SISTEMA" });

            migrationBuilder.InsertData(
                table: "TiposTelefone",
                columns: new[] { "CodigoTipoTelefone", "DataInsercao", "DescricaoTipoTelefone", "UsuarioInsercao" },
                values: new object[] { 4, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "CELULAR", "SISTEMA" });

            migrationBuilder.CreateIndex(
                name: "IX_Clientes_Documento",
                table: "Clientes",
                column: "Documento",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Telefones_CodigoTipoTelefone",
                table: "Telefones",
                column: "CodigoTipoTelefone");

            migrationBuilder.CreateIndex(
                name: "IX_TiposTelefone_DescricaoTipoTelefone",
                table: "TiposTelefone",
                column: "DescricaoTipoTelefone",
                unique: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Telefones");

            migrationBuilder.DropTable(
                name: "Clientes");

            migrationBuilder.DropTable(
                name: "TiposTelefone");
        }
    }
}
