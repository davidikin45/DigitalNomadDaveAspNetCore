using DND.Common.Implementation.Models;
using DND.Common.Implementation.Validation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace DND.Domain.Models
{
    //var result = Result.Combine(Email.Create("a"), Email.Create("b"));
    //if(result.IsFailure)
    //{
    //    return BadRequest(result.Error);
    //}

    public class Email : BaseValueObject<Email>
    {
        public string Value { get; }

        private Email(string value)
        {
            Value = value;
        }

        public static Result<Email> Create(string email)
        {


            email = (email ?? string.Empty).Trim();

            if (email.Length == 0)
                return Result.ObjectValidationFail<Email>("Email should not be empty");

            if (!Regex.IsMatch(email, @"^(.+)@(.+)$"))
                return Result.ObjectValidationFail<Email>("Email is invalid");

            return Result.Ok(new Email(email));
        }

        public static explicit operator Email(string email)
        {
            return Create(email).Value;
        }

        public static implicit operator string(Email email)
        {
            return email.Value;
        }
    }
}
