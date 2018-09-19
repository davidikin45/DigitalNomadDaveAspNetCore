using AutoMapper;
using DND.Common.Domain.Dtos;
using DND.Common.Domain.ModelMetadata;
using DND.Common.Infrastructure.Interfaces.Automapper;
using DND.Domain.DynamicForms.Questions.Enums;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace DND.Domain.DynamicForms.Questions.Dtos
{
    public class QuestionQuestionDto : DtoBase<int>, IMapFrom<QuestionQuestion>, IMapTo<QuestionQuestion>
    {
        [HiddenInput()]
        [ReadOnlyHiddenInput(ShowForCreate = false, ShowForEdit = false), Display(Order = 0)]
        public override int Id { get => base.Id; set => base.Id = value; }

        [ActionLink("Details", "AdminQuestions")]
        [LinkRouteValue("id", "{LogicQuestionId}")]
        [Display(Name = "Question")]
        [Required]
        [Dropdown(typeof(Question), nameof(DND.Domain.DynamicForms.Questions.Question.QuestionText))]
        public int LogicQuestionId { get; set; }

        [HiddenInput()]
        public int QuestionId { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        [Render(ShowForGrid = false, ShowForDisplay = false, ShowForEdit = false, ShowForCreate = false)]
        public QuestionDto LogicQuestion { get; set; }

        public QuestionQuestionLogicType LogicType { get; set; }

        public string Value { get; set; }

        public void CreateMappings(IMapperConfigurationExpression configuration)
        {

        }

        public override IEnumerable<ValidationResult> Validate(System.ComponentModel.DataAnnotations.ValidationContext validationContext)
        {
            yield break;
        }
    }
}
