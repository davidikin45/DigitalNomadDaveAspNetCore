using DND.Domain.DTOs;
using DND.Common.Implementation.Models;
using DND.Common.Interfaces.Automapper;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace DND.Domain.ViewModels
{
    public class LocationForm : BaseObjectValidatable, IMapTo<LocationRequestDTO>
    {
        [Required]
        public string Locale { get; set; }
        [Required]
        public string[] Market { get; set; }
        [Required]
        public string Currency { get; set; }
    
        public string Id { get; set; }

        public override IEnumerable<ValidationResult> Validate(System.ComponentModel.DataAnnotations.ValidationContext validationContext)
        {
            yield break;
        }
    }
}