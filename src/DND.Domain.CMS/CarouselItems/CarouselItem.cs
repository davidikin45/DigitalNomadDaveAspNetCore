using DND.Common.Enums;
using DND.Common.Implementation.Models;
using DND.Common.Interfaces.UnitOfWork;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;

namespace DND.Domain.CMS.CarouselItems
{
    public class CarouselItem : BaseEntityAggregateRootAuditable<int>
    {

		public string Title { get; set; }

        //[StringLength(200)]
        public string CarouselText
        { get; set; }

        public string Album { get; set; }

        public string ButtonText { get; set; }
        public string Link { get; set; }
        public string Image { get; set; }

        //[Required]
        public bool OpenInNewWindow
        { get; set; }

        //[Required]
        public bool Published
        { get; set; }


        public CarouselItem()
		{
			
		}

        public override IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            var errors = new List<ValidationResult>();
            return errors;
        }

        public async override Task<IEnumerable<ValidationResult>> ValidateWithDbConnectionAsync(IBaseUnitOfWorkScope unitOfWork, ValidationMode mode)
        {
            var errors = new List<ValidationResult>();
            return errors;
        }
    }
}