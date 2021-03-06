using DND.Common.Domain;
using DND.Common.Infrastrucutre.Interfaces.Domain;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Threading.Tasks;

namespace DND.Domain.Models
{
    public class BinaryData : EntityAuditableBase<int>
    {
        [Key, ForeignKey("File")]
        public override int Id
        {
            get { return base.Id; }
            set { base.Id = value; }
        }

        [Required]
        public byte[] Data
        { get; set; }

        public virtual File File { get; set; }

        public BinaryData()
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