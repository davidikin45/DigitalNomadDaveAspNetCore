using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace DND.Data.DynamicForms.Migrations
{
    public partial class InitialMigration : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Form",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    RowVersion = table.Column<byte[]>(rowVersion: true, nullable: true),
                    DateCreated = table.Column<DateTime>(nullable: false),
                    UserOwner = table.Column<string>(nullable: true),
                    UserCreated = table.Column<string>(nullable: true),
                    DateModified = table.Column<DateTime>(nullable: true),
                    UserModified = table.Column<string>(nullable: true),
                    Published = table.Column<bool>(nullable: false),
                    Name = table.Column<string>(nullable: true),
                    UrlSlug = table.Column<string>(nullable: true),
                    ConfirmationText = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Form", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "LookupTable",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    RowVersion = table.Column<byte[]>(rowVersion: true, nullable: true),
                    DateCreated = table.Column<DateTime>(nullable: false),
                    UserOwner = table.Column<string>(nullable: true),
                    UserCreated = table.Column<string>(nullable: true),
                    DateModified = table.Column<DateTime>(nullable: true),
                    UserModified = table.Column<string>(nullable: true),
                    Name = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LookupTable", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Section",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    RowVersion = table.Column<byte[]>(rowVersion: true, nullable: true),
                    DateCreated = table.Column<DateTime>(nullable: false),
                    UserOwner = table.Column<string>(nullable: true),
                    UserCreated = table.Column<string>(nullable: true),
                    DateModified = table.Column<DateTime>(nullable: true),
                    UserModified = table.Column<string>(nullable: true),
                    Name = table.Column<string>(nullable: true),
                    UrlSlug = table.Column<string>(nullable: true),
                    ShowInMenu = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Section", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "FormNotification",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    DateCreated = table.Column<DateTime>(nullable: false),
                    UserOwner = table.Column<string>(nullable: true),
                    UserCreated = table.Column<string>(nullable: true),
                    DateModified = table.Column<DateTime>(nullable: true),
                    UserModified = table.Column<string>(nullable: true),
                    FormId = table.Column<int>(nullable: false),
                    Email = table.Column<string>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FormNotification", x => x.Id);
                    table.ForeignKey(
                        name: "FK_FormNotification_Form_FormId",
                        column: x => x.FormId,
                        principalTable: "Form",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "FormSubmission",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    RowVersion = table.Column<byte[]>(rowVersion: true, nullable: true),
                    DateCreated = table.Column<DateTime>(nullable: false),
                    UserOwner = table.Column<string>(nullable: true),
                    UserCreated = table.Column<string>(nullable: true),
                    DateModified = table.Column<DateTime>(nullable: true),
                    UserModified = table.Column<string>(nullable: true),
                    FormId = table.Column<int>(nullable: false),
                    Completed = table.Column<bool>(nullable: false),
                    Valid = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FormSubmission", x => x.Id);
                    table.ForeignKey(
                        name: "FK_FormSubmission_Form_FormId",
                        column: x => x.FormId,
                        principalTable: "Form",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "LookupTableItem",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    DateCreated = table.Column<DateTime>(nullable: false),
                    UserOwner = table.Column<string>(nullable: true),
                    UserCreated = table.Column<string>(nullable: true),
                    DateModified = table.Column<DateTime>(nullable: true),
                    UserModified = table.Column<string>(nullable: true),
                    LookupTableId = table.Column<int>(nullable: false),
                    Text = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LookupTableItem", x => x.Id);
                    table.ForeignKey(
                        name: "FK_LookupTableItem_LookupTable_LookupTableId",
                        column: x => x.LookupTableId,
                        principalTable: "LookupTable",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Question",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    RowVersion = table.Column<byte[]>(rowVersion: true, nullable: true),
                    DateCreated = table.Column<DateTime>(nullable: false),
                    UserOwner = table.Column<string>(nullable: true),
                    UserCreated = table.Column<string>(nullable: true),
                    DateModified = table.Column<DateTime>(nullable: true),
                    UserModified = table.Column<string>(nullable: true),
                    FieldName = table.Column<string>(nullable: true),
                    QuestionText = table.Column<string>(nullable: true),
                    QuestionType = table.Column<string>(nullable: true),
                    LookupTableId = table.Column<int>(nullable: true),
                    DefaultAnswer = table.Column<string>(nullable: true),
                    Placeholder = table.Column<string>(nullable: true),
                    HelpText = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Question", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Question_LookupTable_LookupTableId",
                        column: x => x.LookupTableId,
                        principalTable: "LookupTable",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "FormSection",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    DateCreated = table.Column<DateTime>(nullable: false),
                    UserOwner = table.Column<string>(nullable: true),
                    UserCreated = table.Column<string>(nullable: true),
                    DateModified = table.Column<DateTime>(nullable: true),
                    UserModified = table.Column<string>(nullable: true),
                    FormId = table.Column<int>(nullable: false),
                    SectionId = table.Column<int>(nullable: false),
                    Name = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FormSection", x => x.Id);
                    table.ForeignKey(
                        name: "FK_FormSection_Form_FormId",
                        column: x => x.FormId,
                        principalTable: "Form",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_FormSection_Section_SectionId",
                        column: x => x.SectionId,
                        principalTable: "Section",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "SectionSection",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    DateCreated = table.Column<DateTime>(nullable: false),
                    UserOwner = table.Column<string>(nullable: true),
                    UserCreated = table.Column<string>(nullable: true),
                    DateModified = table.Column<DateTime>(nullable: true),
                    UserModified = table.Column<string>(nullable: true),
                    SectionId = table.Column<int>(nullable: false),
                    Name = table.Column<string>(nullable: true),
                    ChildSectionId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SectionSection", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SectionSection_Section_ChildSectionId",
                        column: x => x.ChildSectionId,
                        principalTable: "Section",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_SectionSection_Section_SectionId",
                        column: x => x.SectionId,
                        principalTable: "Section",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "FormSectionSubmission",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    DateCreated = table.Column<DateTime>(nullable: false),
                    UserOwner = table.Column<string>(nullable: true),
                    UserCreated = table.Column<string>(nullable: true),
                    DateModified = table.Column<DateTime>(nullable: true),
                    UserModified = table.Column<string>(nullable: true),
                    FormSubmissionId = table.Column<Guid>(nullable: false),
                    UrlSlug = table.Column<string>(nullable: true),
                    Valid = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FormSectionSubmission", x => x.Id);
                    table.ForeignKey(
                        name: "FK_FormSectionSubmission_FormSubmission_FormSubmissionId",
                        column: x => x.FormSubmissionId,
                        principalTable: "FormSubmission",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "QuestionQuestion",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    DateCreated = table.Column<DateTime>(nullable: false),
                    UserOwner = table.Column<string>(nullable: true),
                    UserCreated = table.Column<string>(nullable: true),
                    DateModified = table.Column<DateTime>(nullable: true),
                    UserModified = table.Column<string>(nullable: true),
                    QuestionId = table.Column<int>(nullable: false),
                    LogicQuestionId = table.Column<int>(nullable: false),
                    LogicTypeString = table.Column<string>(nullable: true),
                    Value = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_QuestionQuestion", x => x.Id);
                    table.ForeignKey(
                        name: "FK_QuestionQuestion_Question_LogicQuestionId",
                        column: x => x.LogicQuestionId,
                        principalTable: "Question",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_QuestionQuestion_Question_QuestionId",
                        column: x => x.QuestionId,
                        principalTable: "Question",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "QuestionSection",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    DateCreated = table.Column<DateTime>(nullable: false),
                    UserOwner = table.Column<string>(nullable: true),
                    UserCreated = table.Column<string>(nullable: true),
                    DateModified = table.Column<DateTime>(nullable: true),
                    UserModified = table.Column<string>(nullable: true),
                    QuestionId = table.Column<int>(nullable: false),
                    Name = table.Column<string>(nullable: true),
                    SectionId = table.Column<int>(nullable: false),
                    LogicTypeString = table.Column<string>(nullable: true),
                    Value = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_QuestionSection", x => x.Id);
                    table.ForeignKey(
                        name: "FK_QuestionSection_Question_QuestionId",
                        column: x => x.QuestionId,
                        principalTable: "Question",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_QuestionSection_Section_SectionId",
                        column: x => x.SectionId,
                        principalTable: "Section",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "QuestionValidation",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    DateCreated = table.Column<DateTime>(nullable: false),
                    UserOwner = table.Column<string>(nullable: true),
                    UserCreated = table.Column<string>(nullable: true),
                    DateModified = table.Column<DateTime>(nullable: true),
                    UserModified = table.Column<string>(nullable: true),
                    QuestionId = table.Column<int>(nullable: false),
                    ValidationType = table.Column<string>(nullable: false),
                    CustomValidationMessage = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_QuestionValidation", x => x.Id);
                    table.ForeignKey(
                        name: "FK_QuestionValidation_Question_QuestionId",
                        column: x => x.QuestionId,
                        principalTable: "Question",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "SectionQuestion",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    DateCreated = table.Column<DateTime>(nullable: false),
                    UserOwner = table.Column<string>(nullable: true),
                    UserCreated = table.Column<string>(nullable: true),
                    DateModified = table.Column<DateTime>(nullable: true),
                    UserModified = table.Column<string>(nullable: true),
                    SectionId = table.Column<int>(nullable: false),
                    QuestionId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SectionQuestion", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SectionQuestion_Question_QuestionId",
                        column: x => x.QuestionId,
                        principalTable: "Question",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_SectionQuestion_Section_SectionId",
                        column: x => x.SectionId,
                        principalTable: "Section",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "FormSectionSubmissionQuestionAnswer",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    DateCreated = table.Column<DateTime>(nullable: false),
                    UserOwner = table.Column<string>(nullable: true),
                    UserCreated = table.Column<string>(nullable: true),
                    DateModified = table.Column<DateTime>(nullable: true),
                    UserModified = table.Column<string>(nullable: true),
                    FormSectionSubmissionId = table.Column<int>(nullable: false),
                    FieldName = table.Column<string>(nullable: true),
                    Question = table.Column<string>(nullable: true),
                    Answer = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FormSectionSubmissionQuestionAnswer", x => x.Id);
                    table.ForeignKey(
                        name: "FK_FormSectionSubmissionQuestionAnswer_FormSectionSubmission_FormSectionSubmissionId",
                        column: x => x.FormSectionSubmissionId,
                        principalTable: "FormSectionSubmission",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_FormNotification_FormId",
                table: "FormNotification",
                column: "FormId");

            migrationBuilder.CreateIndex(
                name: "IX_FormSection_FormId",
                table: "FormSection",
                column: "FormId");

            migrationBuilder.CreateIndex(
                name: "IX_FormSection_SectionId",
                table: "FormSection",
                column: "SectionId");

            migrationBuilder.CreateIndex(
                name: "IX_FormSectionSubmission_FormSubmissionId",
                table: "FormSectionSubmission",
                column: "FormSubmissionId");

            migrationBuilder.CreateIndex(
                name: "IX_FormSectionSubmissionQuestionAnswer_FormSectionSubmissionId",
                table: "FormSectionSubmissionQuestionAnswer",
                column: "FormSectionSubmissionId");

            migrationBuilder.CreateIndex(
                name: "IX_FormSubmission_FormId",
                table: "FormSubmission",
                column: "FormId");

            migrationBuilder.CreateIndex(
                name: "IX_LookupTableItem_LookupTableId",
                table: "LookupTableItem",
                column: "LookupTableId");

            migrationBuilder.CreateIndex(
                name: "IX_Question_LookupTableId",
                table: "Question",
                column: "LookupTableId");

            migrationBuilder.CreateIndex(
                name: "IX_QuestionQuestion_LogicQuestionId",
                table: "QuestionQuestion",
                column: "LogicQuestionId");

            migrationBuilder.CreateIndex(
                name: "IX_QuestionQuestion_QuestionId",
                table: "QuestionQuestion",
                column: "QuestionId");

            migrationBuilder.CreateIndex(
                name: "IX_QuestionSection_QuestionId",
                table: "QuestionSection",
                column: "QuestionId");

            migrationBuilder.CreateIndex(
                name: "IX_QuestionSection_SectionId",
                table: "QuestionSection",
                column: "SectionId");

            migrationBuilder.CreateIndex(
                name: "IX_QuestionValidation_QuestionId",
                table: "QuestionValidation",
                column: "QuestionId");

            migrationBuilder.CreateIndex(
                name: "IX_SectionQuestion_QuestionId",
                table: "SectionQuestion",
                column: "QuestionId");

            migrationBuilder.CreateIndex(
                name: "IX_SectionQuestion_SectionId",
                table: "SectionQuestion",
                column: "SectionId");

            migrationBuilder.CreateIndex(
                name: "IX_SectionSection_ChildSectionId",
                table: "SectionSection",
                column: "ChildSectionId");

            migrationBuilder.CreateIndex(
                name: "IX_SectionSection_SectionId",
                table: "SectionSection",
                column: "SectionId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "FormNotification");

            migrationBuilder.DropTable(
                name: "FormSection");

            migrationBuilder.DropTable(
                name: "FormSectionSubmissionQuestionAnswer");

            migrationBuilder.DropTable(
                name: "LookupTableItem");

            migrationBuilder.DropTable(
                name: "QuestionQuestion");

            migrationBuilder.DropTable(
                name: "QuestionSection");

            migrationBuilder.DropTable(
                name: "QuestionValidation");

            migrationBuilder.DropTable(
                name: "SectionQuestion");

            migrationBuilder.DropTable(
                name: "SectionSection");

            migrationBuilder.DropTable(
                name: "FormSectionSubmission");

            migrationBuilder.DropTable(
                name: "Question");

            migrationBuilder.DropTable(
                name: "Section");

            migrationBuilder.DropTable(
                name: "FormSubmission");

            migrationBuilder.DropTable(
                name: "LookupTable");

            migrationBuilder.DropTable(
                name: "Form");
        }
    }
}
