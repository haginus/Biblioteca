using Biblioteca.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Biblioteca.Validators
{
    public class CopyIdValidatorAttribute : ValidationAttribute
    {
        public CopyIdValidatorAttribute() { }

        public string GetErrorMessage() =>
            $"This book copy is already in a rent.";

        protected override ValidationResult IsValid(object value,
            ValidationContext validationContext)
        { // value is CopyID
            var rent = (Rent) validationContext.ObjectInstance;
            var ctx = new LibraryEntities();
            var bc = ctx.BookCopies.Find(value);
            if(bc == null)
            {
                return new ValidationResult("Copy does not exist.");
            }
            var isAvailable = bc.isAvailable();
            if (!isAvailable)
            {
                return new ValidationResult(GetErrorMessage());
            }

            return ValidationResult.Success;
        }
    }
}