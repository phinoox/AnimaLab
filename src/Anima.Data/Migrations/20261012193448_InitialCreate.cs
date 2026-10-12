using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Anima.Data.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "MetaTags",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    Name = table.Column<string>(type: "TEXT", maxLength: 128, nullable: false),
                    slug = table.Column<string>(type: "TEXT", maxLength: 128, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MetaTags", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ProjectSeries",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProjectSeries", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Users",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    UserName = table.Column<string>(type: "TEXT", maxLength: 256, nullable: false),
                    Email = table.Column<string>(type: "TEXT", maxLength: 256, nullable: false),
                    RecoveryCodeHash = table.Column<string>(type: "TEXT", maxLength: 2048, nullable: true),
                    LastLogin = table.Column<DateTime>(type: "TEXT", nullable: true),
                    IsActive = table.Column<bool>(type: "INTEGER", nullable: false),
                    PasswordHash = table.Column<string>(type: "TEXT", nullable: true),
                    Provider = table.Column<int>(type: "INTEGER", nullable: false, defaultValue: 0),
                    EmailConfirmed = table.Column<bool>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Users", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Projects",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    Description = table.Column<string>(type: "TEXT", maxLength: 4096, nullable: true),
                    IsActive = table.Column<bool>(type: "INTEGER", nullable: false),
                    ProjectSeriesId = table.Column<Guid>(type: "TEXT", nullable: true),
                    EnableUserRegistration = table.Column<bool>(type: "INTEGER", nullable: false),
                    AllowManualInvites = table.Column<bool>(type: "INTEGER", nullable: false),
                    PrimaryFormat = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Projects", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Projects_ProjectSeries_ProjectSeriesId",
                        column: x => x.ProjectSeriesId,
                        principalTable: "ProjectSeries",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "ReviewStatus",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    Status = table.Column<int>(type: "INTEGER", nullable: false),
                    ReviewedByUserId = table.Column<Guid>(type: "TEXT", nullable: true),
                    ReviewComment = table.Column<string>(type: "TEXT", maxLength: 4096, nullable: true),
                    ReviewedAt = table.Column<DateTime>(type: "TEXT", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ReviewStatus", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ReviewStatus_Users_ReviewedByUserId",
                        column: x => x.ReviewedByUserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "UserMetaInfos",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    AvatarUrl = table.Column<string>(type: "TEXT", maxLength: 1024, nullable: true),
                    TypeHint = table.Column<string>(type: "TEXT", nullable: true),
                    CreatedByUserId = table.Column<Guid>(type: "TEXT", nullable: false),
                    Title = table.Column<string>(type: "TEXT", maxLength: 128, nullable: false),
                    slug = table.Column<string>(type: "TEXT", maxLength: 128, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "TEXT", nullable: false),
                    LastModifiedAt = table.Column<DateTime>(type: "TEXT", nullable: false),
                    ShortDesc = table.Column<string>(type: "TEXT", maxLength: 256, nullable: true),
                    IsPublic = table.Column<bool>(type: "INTEGER", nullable: false),
                    TagIds = table.Column<string>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserMetaInfos", x => x.Id);
                    table.ForeignKey(
                        name: "FK_UserMetaInfos_UserMetaInfos_CreatedByUserId",
                        column: x => x.CreatedByUserId,
                        principalTable: "UserMetaInfos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_UserMetaInfos_Users_Id",
                        column: x => x.Id,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ActivityLogs",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    ProjectId = table.Column<Guid>(type: "TEXT", nullable: false),
                    UserId = table.Column<Guid>(type: "TEXT", nullable: false),
                    Action = table.Column<string>(type: "TEXT", maxLength: 64, nullable: false),
                    RelatedEntityType = table.Column<string>(type: "TEXT", maxLength: 64, nullable: false),
                    RelatedEntityId = table.Column<Guid>(type: "TEXT", nullable: true),
                    Description = table.Column<string>(type: "TEXT", maxLength: 1024, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ActivityLogs", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ActivityLogs_Projects_ProjectId",
                        column: x => x.ProjectId,
                        principalTable: "Projects",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ProjectMembers",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    ProjectId = table.Column<Guid>(type: "TEXT", nullable: false),
                    UserId = table.Column<Guid>(type: "TEXT", nullable: false),
                    Role = table.Column<int>(type: "INTEGER", nullable: false),
                    JoinedAt = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProjectMembers", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ProjectMembers_Projects_ProjectId",
                        column: x => x.ProjectId,
                        principalTable: "Projects",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ProjectMembers_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "ProjectTokens",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    ProjectId = table.Column<Guid>(type: "TEXT", nullable: false),
                    IsActive = table.Column<bool>(type: "INTEGER", nullable: false),
                    TokenName = table.Column<string>(type: "TEXT", nullable: false),
                    ExpiresAt = table.Column<DateTime>(type: "TEXT", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "TEXT", nullable: false),
                    TokenHash = table.Column<string>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProjectTokens", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ProjectTokens_Projects_ProjectId",
                        column: x => x.ProjectId,
                        principalTable: "Projects",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ContentMetaInfos",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    ReviewStatusId = table.Column<Guid>(type: "TEXT", nullable: true),
                    Status = table.Column<int>(type: "INTEGER", nullable: false),
                    ViewMode = table.Column<int>(type: "INTEGER", nullable: false),
                    Version = table.Column<int>(type: "INTEGER", nullable: false),
                    OrderIndex = table.Column<int>(type: "INTEGER", nullable: false),
                    CommentSettings = table.Column<int>(type: "INTEGER", nullable: false),
                    References = table.Column<string>(type: "TEXT", maxLength: 4096, nullable: true),
                    ProjectId = table.Column<Guid>(type: "TEXT", nullable: true),
                    CreatedByUserId = table.Column<Guid>(type: "TEXT", nullable: false),
                    Title = table.Column<string>(type: "TEXT", maxLength: 128, nullable: false),
                    slug = table.Column<string>(type: "TEXT", maxLength: 128, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "TEXT", nullable: false),
                    LastModifiedAt = table.Column<DateTime>(type: "TEXT", nullable: false),
                    ShortDesc = table.Column<string>(type: "TEXT", maxLength: 4096, nullable: true),
                    IsPublic = table.Column<bool>(type: "INTEGER", nullable: false),
                    TagIds = table.Column<string>(type: "TEXT", nullable: false),
                    TypeHint = table.Column<string>(type: "TEXT", nullable: true),
                    ParentId = table.Column<Guid>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ContentMetaInfos", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ContentMetaInfos_Projects_ParentId",
                        column: x => x.ParentId,
                        principalTable: "Projects",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ContentMetaInfos_ReviewStatus_ReviewStatusId",
                        column: x => x.ReviewStatusId,
                        principalTable: "ReviewStatus",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_ContentMetaInfos_UserMetaInfos_CreatedByUserId",
                        column: x => x.CreatedByUserId,
                        principalTable: "UserMetaInfos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ProjectMetaInfos",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    Status = table.Column<int>(type: "INTEGER", nullable: false),
                    ViewMode = table.Column<int>(type: "INTEGER", nullable: false),
                    TypeHint = table.Column<string>(type: "TEXT", nullable: true),
                    Genre = table.Column<string>(type: "TEXT", maxLength: 128, nullable: true),
                    Theme = table.Column<string>(type: "TEXT", maxLength: 128, nullable: true),
                    Tone = table.Column<int>(type: "INTEGER", nullable: false),
                    Audience = table.Column<int>(type: "INTEGER", nullable: false),
                    IsDeleted = table.Column<bool>(type: "INTEGER", nullable: false),
                    DeletedAt = table.Column<DateTime>(type: "TEXT", nullable: true),
                    CreatedByUserId = table.Column<Guid>(type: "TEXT", nullable: false),
                    Title = table.Column<string>(type: "TEXT", maxLength: 128, nullable: false),
                    slug = table.Column<string>(type: "TEXT", maxLength: 128, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "TEXT", nullable: false),
                    LastModifiedAt = table.Column<DateTime>(type: "TEXT", nullable: false),
                    ShortDesc = table.Column<string>(type: "TEXT", maxLength: 4096, nullable: true),
                    IsPublic = table.Column<bool>(type: "INTEGER", nullable: false),
                    TagIds = table.Column<string>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProjectMetaInfos", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ProjectMetaInfos_Projects_Id",
                        column: x => x.Id,
                        principalTable: "Projects",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ProjectMetaInfos_UserMetaInfos_CreatedByUserId",
                        column: x => x.CreatedByUserId,
                        principalTable: "UserMetaInfos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "ProjectSeriesMetaInfos",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    Description = table.Column<string>(type: "TEXT", maxLength: 4096, nullable: true),
                    TypeHint = table.Column<string>(type: "TEXT", nullable: true),
                    CreatedByUserId = table.Column<Guid>(type: "TEXT", nullable: false),
                    Title = table.Column<string>(type: "TEXT", maxLength: 128, nullable: false),
                    slug = table.Column<string>(type: "TEXT", maxLength: 128, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "TEXT", nullable: false),
                    LastModifiedAt = table.Column<DateTime>(type: "TEXT", nullable: false),
                    ShortDesc = table.Column<string>(type: "TEXT", maxLength: 4096, nullable: true),
                    IsPublic = table.Column<bool>(type: "INTEGER", nullable: false),
                    TagIds = table.Column<string>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProjectSeriesMetaInfos", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ProjectSeriesMetaInfos_ProjectSeries_Id",
                        column: x => x.Id,
                        principalTable: "ProjectSeries",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ProjectSeriesMetaInfos_UserMetaInfos_CreatedByUserId",
                        column: x => x.CreatedByUserId,
                        principalTable: "UserMetaInfos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AssetLinks",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    EnginePath = table.Column<string>(type: "TEXT", maxLength: 2048, nullable: false),
                    EngineAssetId = table.Column<string>(type: "TEXT", maxLength: 128, nullable: true),
                    EngineFileType = table.Column<string>(type: "TEXT", maxLength: 64, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AssetLinks", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AssetLinks_ContentMetaInfos_Id",
                        column: x => x.Id,
                        principalTable: "ContentMetaInfos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "BlogPosts",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    PublishedAt = table.Column<DateTime>(type: "TEXT", nullable: true),
                    AuthorId = table.Column<Guid>(type: "TEXT", nullable: false),
                    BlogText = table.Column<string>(type: "TEXT", maxLength: 10000, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BlogPosts", x => x.Id);
                    table.ForeignKey(
                        name: "FK_BlogPosts_ContentMetaInfos_Id",
                        column: x => x.Id,
                        principalTable: "ContentMetaInfos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Comments",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    TargetId = table.Column<Guid>(type: "TEXT", nullable: false),
                    UserId = table.Column<Guid>(type: "TEXT", nullable: false),
                    Text = table.Column<string>(type: "TEXT", maxLength: 4096, nullable: false),
                    ParentCommentId = table.Column<Guid>(type: "TEXT", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "TEXT", nullable: false),
                    ContentMetaInfoId = table.Column<Guid>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Comments", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Comments_ContentMetaInfos_ContentMetaInfoId",
                        column: x => x.ContentMetaInfoId,
                        principalTable: "ContentMetaInfos",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "ContentSnapshots",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    TargetMetaInfoId = table.Column<Guid>(type: "TEXT", nullable: false),
                    Comment = table.Column<string>(type: "TEXT", maxLength: 512, nullable: true),
                    VersionNumber = table.Column<int>(type: "INTEGER", nullable: false, defaultValue: 0),
                    SnapshotType = table.Column<int>(type: "INTEGER", nullable: false, defaultValue: 0),
                    CreatedByUserId = table.Column<Guid>(type: "TEXT", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "TEXT", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP"),
                    SnapshotDataJson = table.Column<string>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ContentSnapshots", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ContentSnapshots_ContentMetaInfos_TargetMetaInfoId",
                        column: x => x.TargetMetaInfoId,
                        principalTable: "ContentMetaInfos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ContentTagRelations",
                columns: table => new
                {
                    MetaInfoId = table.Column<Guid>(type: "TEXT", nullable: false),
                    TagId = table.Column<Guid>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ContentTagRelations", x => new { x.MetaInfoId, x.TagId });
                    table.ForeignKey(
                        name: "FK_ContentTagRelations_ContentMetaInfos_MetaInfoId",
                        column: x => x.MetaInfoId,
                        principalTable: "ContentMetaInfos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_ContentTagRelations_MetaTags_TagId",
                        column: x => x.TagId,
                        principalTable: "MetaTags",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ContentVersionLogs",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    TargetMetaInfoId = table.Column<Guid>(type: "TEXT", nullable: false),
                    ChangedByUserId = table.Column<Guid>(type: "TEXT", nullable: false),
                    ChangeDescription = table.Column<string>(type: "TEXT", maxLength: 2048, nullable: true),
                    VersionNumber = table.Column<int>(type: "INTEGER", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "TEXT", nullable: false),
                    CreatedByUserId = table.Column<Guid>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ContentVersionLogs", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ContentVersionLogs_ContentMetaInfos_TargetMetaInfoId",
                        column: x => x.TargetMetaInfoId,
                        principalTable: "ContentMetaInfos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateTable(
                name: "MediaAttachments",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    FileName = table.Column<string>(type: "TEXT", nullable: false),
                    ContentType = table.Column<string>(type: "TEXT", nullable: false),
                    storage_path = table.Column<string>(type: "TEXT", maxLength: 2048, nullable: false),
                    FileSize = table.Column<long>(type: "INTEGER", nullable: false),
                    UploadedByUserId = table.Column<Guid>(type: "TEXT", nullable: false),
                    UploadedAt = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MediaAttachments", x => x.Id);
                    table.ForeignKey(
                        name: "FK_MediaAttachments_ContentMetaInfos_Id",
                        column: x => x.Id,
                        principalTable: "ContentMetaInfos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateTable(
                name: "ProjectTagRelations",
                columns: table => new
                {
                    MetaInfoId = table.Column<Guid>(type: "TEXT", nullable: false),
                    TagId = table.Column<Guid>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProjectTagRelations", x => new { x.MetaInfoId, x.TagId });
                    table.ForeignKey(
                        name: "FK_ProjectTagRelations_MetaTags_TagId",
                        column: x => x.TagId,
                        principalTable: "MetaTags",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ProjectTagRelations_ProjectMetaInfos_MetaInfoId",
                        column: x => x.MetaInfoId,
                        principalTable: "ProjectMetaInfos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ExternalReferences",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    ReferenceType = table.Column<int>(type: "INTEGER", nullable: false),
                    Url = table.Column<string>(type: "TEXT", maxLength: 2048, nullable: false),
                    Title = table.Column<string>(type: "TEXT", maxLength: 128, nullable: true),
                    Author = table.Column<string>(type: "TEXT", maxLength: 128, nullable: true),
                    Notes = table.Column<string>(type: "TEXT", maxLength: 4096, nullable: true),
                    IsActive = table.Column<bool>(type: "INTEGER", nullable: false),
                    Slug = table.Column<string>(type: "TEXT", maxLength: 128, nullable: true),
                    MediaAttachmentId = table.Column<Guid>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ExternalReferences", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ExternalReferences_ContentMetaInfos_Id",
                        column: x => x.Id,
                        principalTable: "ContentMetaInfos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ExternalReferences_MediaAttachments_MediaAttachmentId",
                        column: x => x.MediaAttachmentId,
                        principalTable: "MediaAttachments",
                        principalColumn: "Id");
                });

            migrationBuilder.InsertData(
                table: "Users",
                columns: new[] { "Id", "Email", "EmailConfirmed", "IsActive", "LastLogin", "PasswordHash", "RecoveryCodeHash", "UserName" },
                values: new object[] { new Guid("00000000-0000-0000-0000-000000000001"), "system@anima.com", false, true, null, null, null, "System" });

            migrationBuilder.InsertData(
                table: "UserMetaInfos",
                columns: new[] { "Id", "AvatarUrl", "CreatedAt", "CreatedByUserId", "IsPublic", "LastModifiedAt", "ShortDesc", "slug", "TagIds", "Title", "TypeHint" },
                values: new object[] { new Guid("00000000-0000-0000-0000-000000000001"), "/images/system-avatar.png", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), new Guid("00000000-0000-0000-0000-000000000001"), false, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, "", "[]", "", null });

            migrationBuilder.CreateIndex(
                name: "IX_ActivityLogs_ProjectId",
                table: "ActivityLogs",
                column: "ProjectId");

            migrationBuilder.CreateIndex(
                name: "IX_Comment_AuthorUserId",
                table: "Comments",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_Comments_ContentMetaInfoId",
                table: "Comments",
                column: "ContentMetaInfoId");

            migrationBuilder.CreateIndex(
                name: "IX_Comments_CreatedAt",
                table: "Comments",
                column: "CreatedAt");

            migrationBuilder.CreateIndex(
                name: "IX_Comments_ParentCommentId",
                table: "Comments",
                column: "ParentCommentId");

            migrationBuilder.CreateIndex(
                name: "IX_ContentMetaInfos_CreatedByUserId",
                table: "ContentMetaInfos",
                column: "CreatedByUserId");

            migrationBuilder.CreateIndex(
                name: "IX_ContentMetaInfos_ParentId",
                table: "ContentMetaInfos",
                column: "ParentId");

            migrationBuilder.CreateIndex(
                name: "IX_ContentMetaInfos_ReviewStatusId",
                table: "ContentMetaInfos",
                column: "ReviewStatusId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ContentMetaInfos_slug",
                table: "ContentMetaInfos",
                column: "slug",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ContentMetaInfos_Status",
                table: "ContentMetaInfos",
                column: "Status");

            migrationBuilder.CreateIndex(
                name: "IX_ContentMetaInfos_TypeHint",
                table: "ContentMetaInfos",
                column: "TypeHint");

            migrationBuilder.CreateIndex(
                name: "IX_ContentSnapshots_CreatedAt",
                table: "ContentSnapshots",
                column: "CreatedAt");

            migrationBuilder.CreateIndex(
                name: "IX_ContentSnapshots_CreatedByUserId",
                table: "ContentSnapshots",
                column: "CreatedByUserId");

            migrationBuilder.CreateIndex(
                name: "IX_ContentSnapshots_SnapshotType",
                table: "ContentSnapshots",
                column: "SnapshotType");

            migrationBuilder.CreateIndex(
                name: "IX_ContentSnapshots_TargetMetaInfoId",
                table: "ContentSnapshots",
                column: "TargetMetaInfoId");

            migrationBuilder.CreateIndex(
                name: "IX_ContentSnapshots_VersionNumber",
                table: "ContentSnapshots",
                column: "VersionNumber");

            migrationBuilder.CreateIndex(
                name: "IX_ContentTagRelations_TagId",
                table: "ContentTagRelations",
                column: "TagId");

            migrationBuilder.CreateIndex(
                name: "IX_ContentVersionLogs_TargetMetaInfoId",
                table: "ContentVersionLogs",
                column: "TargetMetaInfoId");

            migrationBuilder.CreateIndex(
                name: "IX_ContentVersionLogs_VersionNumber",
                table: "ContentVersionLogs",
                column: "VersionNumber");

            migrationBuilder.CreateIndex(
                name: "IX_ExternalReferences_MediaAttachmentId",
                table: "ExternalReferences",
                column: "MediaAttachmentId");

            migrationBuilder.CreateIndex(
                name: "IX_ExternalReferences_Title",
                table: "ExternalReferences",
                column: "Title");

            migrationBuilder.CreateIndex(
                name: "IX_ExternalReferences_Url",
                table: "ExternalReferences",
                column: "Url",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_MediaAttachments_Id",
                table: "MediaAttachments",
                column: "Id");

            migrationBuilder.CreateIndex(
                name: "IX_MetaTags_Name",
                table: "MetaTags",
                column: "Name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_MetaTags_slug",
                table: "MetaTags",
                column: "slug",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ProjectMembers_ProjectId",
                table: "ProjectMembers",
                column: "ProjectId");

            migrationBuilder.CreateIndex(
                name: "IX_ProjectMembers_UserId",
                table: "ProjectMembers",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_ProjectMetaInfos_CreatedByUserId",
                table: "ProjectMetaInfos",
                column: "CreatedByUserId");

            migrationBuilder.CreateIndex(
                name: "IX_ProjectMetaInfos_slug",
                table: "ProjectMetaInfos",
                column: "slug",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Projects_ProjectSeriesId",
                table: "Projects",
                column: "ProjectSeriesId");

            migrationBuilder.CreateIndex(
                name: "IX_ProjectSeriesMetaInfos_CreatedByUserId",
                table: "ProjectSeriesMetaInfos",
                column: "CreatedByUserId");

            migrationBuilder.CreateIndex(
                name: "IX_ProjectSeriesMetaInfos_slug",
                table: "ProjectSeriesMetaInfos",
                column: "slug",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ProjectTagRelations_TagId",
                table: "ProjectTagRelations",
                column: "TagId");

            migrationBuilder.CreateIndex(
                name: "IX_ProjectTokens_ProjectId",
                table: "ProjectTokens",
                column: "ProjectId");

            migrationBuilder.CreateIndex(
                name: "IX_ReviewStatus_ReviewedAt",
                table: "ReviewStatus",
                column: "ReviewedAt");

            migrationBuilder.CreateIndex(
                name: "IX_ReviewStatus_ReviewerId",
                table: "ReviewStatus",
                column: "ReviewedByUserId");

            migrationBuilder.CreateIndex(
                name: "IX_ReviewStatus_Status",
                table: "ReviewStatus",
                column: "Status");

            migrationBuilder.CreateIndex(
                name: "IX_UserMetaInfos_CreatedByUserId",
                table: "UserMetaInfos",
                column: "CreatedByUserId");

            migrationBuilder.CreateIndex(
                name: "IX_Users_Email",
                table: "Users",
                column: "Email",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Users_UserName",
                table: "Users",
                column: "UserName",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ActivityLogs");

            migrationBuilder.DropTable(
                name: "AssetLinks");

            migrationBuilder.DropTable(
                name: "BlogPosts");

            migrationBuilder.DropTable(
                name: "Comments");

            migrationBuilder.DropTable(
                name: "ContentSnapshots");

            migrationBuilder.DropTable(
                name: "ContentTagRelations");

            migrationBuilder.DropTable(
                name: "ContentVersionLogs");

            migrationBuilder.DropTable(
                name: "ExternalReferences");

            migrationBuilder.DropTable(
                name: "ProjectMembers");

            migrationBuilder.DropTable(
                name: "ProjectSeriesMetaInfos");

            migrationBuilder.DropTable(
                name: "ProjectTagRelations");

            migrationBuilder.DropTable(
                name: "ProjectTokens");

            migrationBuilder.DropTable(
                name: "MediaAttachments");

            migrationBuilder.DropTable(
                name: "MetaTags");

            migrationBuilder.DropTable(
                name: "ProjectMetaInfos");

            migrationBuilder.DropTable(
                name: "ContentMetaInfos");

            migrationBuilder.DropTable(
                name: "Projects");

            migrationBuilder.DropTable(
                name: "ReviewStatus");

            migrationBuilder.DropTable(
                name: "UserMetaInfos");

            migrationBuilder.DropTable(
                name: "ProjectSeries");

            migrationBuilder.DropTable(
                name: "Users");
        }
    }
}
