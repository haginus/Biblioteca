using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;

namespace Biblioteca.Validators
{
    public class CnpValidatorAttribute : ValidationAttribute
    {
        public CnpValidatorAttribute() { }

        public string GetErrorMessage(string e) =>
            $"Invalid CNP: {e}";

        protected override ValidationResult IsValid(object value,
            ValidationContext validationContext)
        {
            string CNP; //SYYMMDDJJNNNC
            try
            {
                CNP = ((string)value);
            } catch
            {
                return new ValidationResult(GetErrorMessage("Not a string."));
            }

            if(CNP.Length != 13)
                return new ValidationResult(GetErrorMessage("Should be 13 characters long."));

            foreach(var letter in CNP)  // all digits
            {
                var r = letter - '0';
                if(!(r >= 0 && r <=9))
                    return new ValidationResult(GetErrorMessage("Should only contain digits."));
            }

            int firstDigit = CNP[0] - '0';  // first digit
            if(!(firstDigit >= 1 && firstDigit <= 8)) {
                return new ValidationResult(GetErrorMessage("First letter is incorrect."));
            }
            string year = "";
            if (1 <= firstDigit && firstDigit <= 2) year = "19";
            if (3 <= firstDigit && firstDigit <= 4) year = "18";
            if (5 <= firstDigit && firstDigit <= 8) year = "20";
            year += "" + CNP[1] + CNP[2];
            string month = "" + CNP[3] + CNP[4];
            string day = "" + CNP[5] + CNP[6];
            string date = $"{year}-{month}-{day}";
            DateTime t;
            if(!DateTime.TryParse(date, out t))
            {
                return new ValidationResult(GetErrorMessage("Invalid date of birth."));
            }

            int county = int.Parse("" + CNP[7] + CNP[8]);
            if(!( (1 <= county && county <= 46) || (county == 51 || county == 52) ))
            {
                return new ValidationResult(GetErrorMessage("Invalid county."));
            }

            int secv = int.Parse("" + CNP[9] + CNP[10] + CNP[11]);
            if(!(1 <= secv && secv <= 999)) {
                return new ValidationResult(GetErrorMessage("Invalid sequential number."));
            }

            const string mockCnp = "279146358279";
            var sum = 0;

            for (var i = 0; i < CNP.Length - 1; i++)
            {
                sum += (CNP[i] - '0') * (mockCnp[i] - '0');
            }
            sum %= 11;
            sum = sum == 10 ? 1 : sum;

            if((CNP[12] - '0') != sum)
            {
                return new ValidationResult(GetErrorMessage("Invalid control digit."));
            }

            return ValidationResult.Success;
        }
    }
}