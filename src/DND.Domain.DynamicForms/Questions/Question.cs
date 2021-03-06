﻿using DND.Common.Domain;
using DND.Common.Extensions;
using DND.Common.Infrastrucutre.Interfaces.Domain;
using DND.Domain.DynamicForms.LookupTables;
using DND.Domain.DynamicForms.Questions.Enums;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;

namespace DND.Domain.DynamicForms.Questions
{
    public class Question : EntityAggregateRootAuditableBase<int>
    {
        public string FieldName { get; set; }

        public string QuestionText { get; set; }

        public string QuestionTypeString
        {
            get { return QuestionType.ToString(); }
            private set { QuestionType = EnumExtensions.ParseEnum<QuestionType>(value); }
        }

        public QuestionType QuestionType = QuestionType.Text;

        public Nullable<int> LookupTableId { get; set; }

        public LookupTable LookupTable { get; set; }

        public string DefaultAnswer { get; set; }
        public string Placeholder { get; set; }
        public string HelpText { get; set; }

        public List<QuestionValidation> Validations { get; set; } = new List<QuestionValidation>();
        public List<QuestionSection> Sections { get; set; } = new List<QuestionSection>();
        public List<QuestionQuestion> Questions { get; set; } = new List<QuestionQuestion>();

        public override IEnumerable<ValidationResult> Validate(System.ComponentModel.DataAnnotations.ValidationContext validationContext)
        {
            var errors = new List<ValidationResult>();
            return errors;
        }

        public override async Task<IEnumerable<ValidationResult>> ValidateWithDbConnectionAsync(DbContext context, ValidationMode mode)
        {
            var errors = new List<ValidationResult>();
            return await Task.FromResult(errors);
        }
    }
}
