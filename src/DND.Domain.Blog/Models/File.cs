using DND.Common.Domain;
using DND.Common.Enums;
using DND.Common.Extensions;
using DND.Common.Infrastrucutre.Interfaces.Domain;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Threading.Tasks;

namespace DND.Domain.Models
{
    public class File : EntityAggregateRootAuditableBase<int>
    {
        [Required, StringLength(255)]
        public string Description
        { get; set; }

        [Required, StringLength(255)]
        public string FileName
        { get; set; }

        [StringLength(255)]
        public string FilePath
        { get; set; }

        [Required]
        public int FileSize
        { get; set; }

        [Required, StringLength(100)]
        public string ContentType
        { get; set; }

        [Required]
        public bool Published
        { get; set; }

        [Required]
        public bool IsImage
        { get; set; }

        [Column("CropType")]
        public string CropTypeString
        {
            get { return CropType.ToString(); }
            private set { CropType = EnumExtensions.ParseEnum<CropType>(value); }
        }

        [NotMapped]
        public CropType CropType = CropType.None;

        public virtual BinaryData Content { get; set; }

        public File()
        {

        }

        public override IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            var errors = new List<ValidationResult>();
            return errors;
        }

        public async override Task<IEnumerable<ValidationResult>> ValidateWithDbConnectionAsync(DbContext context, ValidationMode mode)
        {
            var errors = new List<ValidationResult>();
            return await Task.FromResult(errors);
        }
    }
}