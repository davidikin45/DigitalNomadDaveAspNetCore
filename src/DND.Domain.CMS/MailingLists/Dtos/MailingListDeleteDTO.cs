using AutoMapper;
using DND.Common.Implementation.Dtos;
using DND.Common.Implementation.Models;
using DND.Common.Interfaces.Automapper;
using DND.Common.ModelMetadataCustom.DisplayAttributes;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace DND.Domain.CMS.MailingLists.Dtos
{
    public class MailingListDeleteDto : BaseDtoAggregateRoot<int>, IMapTo<MailingList>, IMapFrom<MailingList>
    {
        public override IEnumerable<ValidationResult> Validate(System.ComponentModel.DataAnnotations.ValidationContext validationContext)
        {
            yield break;
        }
    }
}
