using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace Backend6.Data.Migrations
{
    public partial class AddForumAndForumCategory : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ForumCategories_Forums_ForumId",
                table: "ForumCategories");

            migrationBuilder.DropForeignKey(
                name: "FK_ForumTopics_AspNetUsers_CreatorId",
                table: "ForumTopics");

            migrationBuilder.DropIndex(
                name: "IX_ForumCategories_ForumId",
                table: "ForumCategories");

            migrationBuilder.DropColumn(
                name: "ForumId",
                table: "ForumCategories");

            migrationBuilder.AddColumn<Guid>(
                name: "ForumCategoryId",
                table: "Forums",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.CreateIndex(
                name: "IX_Forums_ForumCategoryId",
                table: "Forums",
                column: "ForumCategoryId");

            migrationBuilder.AddForeignKey(
                name: "FK_Forums_ForumCategories_ForumCategoryId",
                table: "Forums",
                column: "ForumCategoryId",
                principalTable: "ForumCategories",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_ForumTopics_AspNetUsers_CreatorId",
                table: "ForumTopics",
                column: "CreatorId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Forums_ForumCategories_ForumCategoryId",
                table: "Forums");

            migrationBuilder.DropForeignKey(
                name: "FK_ForumTopics_AspNetUsers_CreatorId",
                table: "ForumTopics");

            migrationBuilder.DropIndex(
                name: "IX_Forums_ForumCategoryId",
                table: "Forums");

            migrationBuilder.DropColumn(
                name: "ForumCategoryId",
                table: "Forums");

            migrationBuilder.AddColumn<Guid>(
                name: "ForumId",
                table: "ForumCategories",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.CreateIndex(
                name: "IX_ForumCategories_ForumId",
                table: "ForumCategories",
                column: "ForumId");

            migrationBuilder.AddForeignKey(
                name: "FK_ForumCategories_Forums_ForumId",
                table: "ForumCategories",
                column: "ForumId",
                principalTable: "Forums",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_ForumTopics_AspNetUsers_CreatorId",
                table: "ForumTopics",
                column: "CreatorId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
