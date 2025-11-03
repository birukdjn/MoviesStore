using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MoviesStore.Migrations
{
    /// <inheritdoc />
    public partial class ApplyNetflixSchemaUpdates : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_MovieCategory_Categories_CategoryId",
                table: "MovieCategory");

            migrationBuilder.DropForeignKey(
                name: "FK_MovieCategory_Movies_MovieId",
                table: "MovieCategory");

            migrationBuilder.DropForeignKey(
                name: "FK_MovieGenre_Genres_GenreId",
                table: "MovieGenre");

            migrationBuilder.DropForeignKey(
                name: "FK_MovieGenre_Movies_MovieId",
                table: "MovieGenre");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Ratings",
                table: "Ratings");

            migrationBuilder.DropIndex(
                name: "IX_Ratings_ProfileId",
                table: "Ratings");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Favorites",
                table: "Favorites");

            migrationBuilder.DropIndex(
                name: "IX_Favorites_ProfileId",
                table: "Favorites");

            migrationBuilder.DropPrimaryKey(
                name: "PK_MovieGenre",
                table: "MovieGenre");

            migrationBuilder.DropPrimaryKey(
                name: "PK_MovieCategory",
                table: "MovieCategory");

            migrationBuilder.DropColumn(
                name: "Address",
                table: "Profiles");

            migrationBuilder.DropColumn(
                name: "City",
                table: "Profiles");

            migrationBuilder.DropColumn(
                name: "Country",
                table: "Profiles");

            migrationBuilder.DropColumn(
                name: "DateOfBirth",
                table: "Profiles");

            migrationBuilder.DropColumn(
                name: "Email",
                table: "Profiles");

            migrationBuilder.DropColumn(
                name: "FirstName",
                table: "Profiles");

            migrationBuilder.DropColumn(
                name: "LastName",
                table: "Profiles");

            migrationBuilder.DropColumn(
                name: "PhoneNumber",
                table: "Profiles");

            migrationBuilder.RenameTable(
                name: "MovieGenre",
                newName: "MovieGenres");

            migrationBuilder.RenameTable(
                name: "MovieCategory",
                newName: "MovieCategories");

            migrationBuilder.RenameColumn(
                name: "RatedAt",
                table: "Ratings",
                newName: "RatedDate");

            migrationBuilder.RenameColumn(
                name: "ZipCode",
                table: "Profiles",
                newName: "Name");

            migrationBuilder.RenameColumn(
                name: "State",
                table: "Profiles",
                newName: "AvatarUrl");

            migrationBuilder.RenameIndex(
                name: "IX_MovieGenre_GenreId",
                table: "MovieGenres",
                newName: "IX_MovieGenres_GenreId");

            migrationBuilder.RenameIndex(
                name: "IX_MovieCategory_CategoryId",
                table: "MovieCategories",
                newName: "IX_MovieCategories_CategoryId");

            migrationBuilder.AddColumn<string>(
                name: "Email",
                table: "Users",
                type: "nvarchar(450)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<int>(
                name: "SubscriptionPlan",
                table: "Users",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<bool>(
                name: "IsKidsProfile",
                table: "Profiles",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "AgeRating",
                table: "Movies",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<double>(
                name: "AverageRating",
                table: "Movies",
                type: "float",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<string>(
                name: "BackdropUrl",
                table: "Movies",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<bool>(
                name: "IsOriginal",
                table: "Movies",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<int>(
                name: "RuntimeMinutes",
                table: "Movies",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "ThumbnailUrl",
                table: "Movies",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AlterColumn<int>(
                name: "Id",
                table: "Favorites",
                type: "int",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int")
                .OldAnnotation("SqlServer:Identity", "1, 1");

            migrationBuilder.AddColumn<DateTime>(
                name: "AddedDate",
                table: "Favorites",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddPrimaryKey(
                name: "PK_Ratings",
                table: "Ratings",
                columns: new[] { "ProfileId", "MovieId" });

            migrationBuilder.AddPrimaryKey(
                name: "PK_Favorites",
                table: "Favorites",
                columns: new[] { "ProfileId", "MovieId" });

            migrationBuilder.AddPrimaryKey(
                name: "PK_MovieGenres",
                table: "MovieGenres",
                columns: new[] { "MovieId", "GenreId" });

            migrationBuilder.AddPrimaryKey(
                name: "PK_MovieCategories",
                table: "MovieCategories",
                columns: new[] { "MovieId", "CategoryId" });

            migrationBuilder.CreateTable(
                name: "PlaybackPositions",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ProfileId = table.Column<int>(type: "int", nullable: false),
                    MovieId = table.Column<int>(type: "int", nullable: false),
                    PositionInSeconds = table.Column<int>(type: "int", nullable: false),
                    TotalDurationInSeconds = table.Column<int>(type: "int", nullable: false),
                    LastWatchedDate = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PlaybackPositions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PlaybackPositions_Movies_MovieId",
                        column: x => x.MovieId,
                        principalTable: "Movies",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_PlaybackPositions_Profiles_ProfileId",
                        column: x => x.ProfileId,
                        principalTable: "Profiles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Users_Email",
                table: "Users",
                column: "Email",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Ratings_MovieId",
                table: "Ratings",
                column: "MovieId");

            migrationBuilder.CreateIndex(
                name: "IX_PlaybackPositions_MovieId",
                table: "PlaybackPositions",
                column: "MovieId");

            migrationBuilder.CreateIndex(
                name: "IX_PlaybackPositions_ProfileId",
                table: "PlaybackPositions",
                column: "ProfileId");

            migrationBuilder.AddForeignKey(
                name: "FK_MovieCategories_Categories_CategoryId",
                table: "MovieCategories",
                column: "CategoryId",
                principalTable: "Categories",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_MovieCategories_Movies_MovieId",
                table: "MovieCategories",
                column: "MovieId",
                principalTable: "Movies",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_MovieGenres_Genres_GenreId",
                table: "MovieGenres",
                column: "GenreId",
                principalTable: "Genres",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_MovieGenres_Movies_MovieId",
                table: "MovieGenres",
                column: "MovieId",
                principalTable: "Movies",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_MovieCategories_Categories_CategoryId",
                table: "MovieCategories");

            migrationBuilder.DropForeignKey(
                name: "FK_MovieCategories_Movies_MovieId",
                table: "MovieCategories");

            migrationBuilder.DropForeignKey(
                name: "FK_MovieGenres_Genres_GenreId",
                table: "MovieGenres");

            migrationBuilder.DropForeignKey(
                name: "FK_MovieGenres_Movies_MovieId",
                table: "MovieGenres");

            migrationBuilder.DropTable(
                name: "PlaybackPositions");

            migrationBuilder.DropIndex(
                name: "IX_Users_Email",
                table: "Users");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Ratings",
                table: "Ratings");

            migrationBuilder.DropIndex(
                name: "IX_Ratings_MovieId",
                table: "Ratings");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Favorites",
                table: "Favorites");

            migrationBuilder.DropPrimaryKey(
                name: "PK_MovieGenres",
                table: "MovieGenres");

            migrationBuilder.DropPrimaryKey(
                name: "PK_MovieCategories",
                table: "MovieCategories");

            migrationBuilder.DropColumn(
                name: "Email",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "SubscriptionPlan",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "IsKidsProfile",
                table: "Profiles");

            migrationBuilder.DropColumn(
                name: "AgeRating",
                table: "Movies");

            migrationBuilder.DropColumn(
                name: "AverageRating",
                table: "Movies");

            migrationBuilder.DropColumn(
                name: "BackdropUrl",
                table: "Movies");

            migrationBuilder.DropColumn(
                name: "IsOriginal",
                table: "Movies");

            migrationBuilder.DropColumn(
                name: "RuntimeMinutes",
                table: "Movies");

            migrationBuilder.DropColumn(
                name: "ThumbnailUrl",
                table: "Movies");

            migrationBuilder.DropColumn(
                name: "AddedDate",
                table: "Favorites");

            migrationBuilder.RenameTable(
                name: "MovieGenres",
                newName: "MovieGenre");

            migrationBuilder.RenameTable(
                name: "MovieCategories",
                newName: "MovieCategory");

            migrationBuilder.RenameColumn(
                name: "RatedDate",
                table: "Ratings",
                newName: "RatedAt");

            migrationBuilder.RenameColumn(
                name: "Name",
                table: "Profiles",
                newName: "ZipCode");

            migrationBuilder.RenameColumn(
                name: "AvatarUrl",
                table: "Profiles",
                newName: "State");

            migrationBuilder.RenameIndex(
                name: "IX_MovieGenres_GenreId",
                table: "MovieGenre",
                newName: "IX_MovieGenre_GenreId");

            migrationBuilder.RenameIndex(
                name: "IX_MovieCategories_CategoryId",
                table: "MovieCategory",
                newName: "IX_MovieCategory_CategoryId");

            migrationBuilder.AddColumn<string>(
                name: "Address",
                table: "Profiles",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "City",
                table: "Profiles",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Country",
                table: "Profiles",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<DateTime>(
                name: "DateOfBirth",
                table: "Profiles",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<string>(
                name: "Email",
                table: "Profiles",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "FirstName",
                table: "Profiles",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "LastName",
                table: "Profiles",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "PhoneNumber",
                table: "Profiles",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AlterColumn<int>(
                name: "Id",
                table: "Favorites",
                type: "int",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int")
                .Annotation("SqlServer:Identity", "1, 1");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Ratings",
                table: "Ratings",
                columns: new[] { "MovieId", "ProfileId" });

            migrationBuilder.AddPrimaryKey(
                name: "PK_Favorites",
                table: "Favorites",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_MovieGenre",
                table: "MovieGenre",
                columns: new[] { "MovieId", "GenreId" });

            migrationBuilder.AddPrimaryKey(
                name: "PK_MovieCategory",
                table: "MovieCategory",
                columns: new[] { "MovieId", "CategoryId" });

            migrationBuilder.CreateIndex(
                name: "IX_Ratings_ProfileId",
                table: "Ratings",
                column: "ProfileId");

            migrationBuilder.CreateIndex(
                name: "IX_Favorites_ProfileId",
                table: "Favorites",
                column: "ProfileId");

            migrationBuilder.AddForeignKey(
                name: "FK_MovieCategory_Categories_CategoryId",
                table: "MovieCategory",
                column: "CategoryId",
                principalTable: "Categories",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_MovieCategory_Movies_MovieId",
                table: "MovieCategory",
                column: "MovieId",
                principalTable: "Movies",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_MovieGenre_Genres_GenreId",
                table: "MovieGenre",
                column: "GenreId",
                principalTable: "Genres",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_MovieGenre_Movies_MovieId",
                table: "MovieGenre",
                column: "MovieId",
                principalTable: "Movies",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
