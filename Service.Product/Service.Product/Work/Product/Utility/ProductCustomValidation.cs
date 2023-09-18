using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;

namespace Service.Product.Work.Utility
{
    /// <summary>
    /// [CustomModelValidation] Validate unique identifier.
    /// </summary>
    /// <remarks>
    /// Criteria:
    /// <br>1. It cannot be null or whitespace.</br>
    /// <br>2. It must be numeric (0-9).</br>
    /// </remarks>
    public class ValidateUniqueIdentifierAttribute : ValidationAttribute
    {
        // Keep the expression compiled to improve performance.
        private static readonly Regex ValidationRegex = new Regex(@"^[0-9]*", RegexOptions.Compiled);

        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            if (string.IsNullOrWhiteSpace(value.ToString()))
            {
                return new ValidationResult(validationContext.MemberName + " is required.");
            }
            if (!ValidationRegex.IsMatch(value.ToString()))
            {
                return new ValidationResult(validationContext.MemberName + " must be numeric (0-9).");
            }
            return ValidationResult.Success;
        }
    }

    /// <summary>
    /// [CustomModelValidation] Validate Product name.
    /// </summary>
    /// <remarks>
    /// Criteria:
    /// <br>1. It cannot be null or whitespace.</br>
    /// <br>2. It must begin with an alphabet (A-Za-z).</br>
    /// </remarks>
    public class ValidateProductNameAttribute : ValidationAttribute
    {
        // Keep the expression compiled to improve performance.
        private static readonly Regex ValidationRegex = new Regex(@"^[A-Za-z]*", RegexOptions.Compiled);

        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            if (string.IsNullOrWhiteSpace(value.ToString()))
            {
                return new ValidationResult(validationContext.MemberName + " is required.");
            }
            if (!ValidationRegex.IsMatch(value.ToString()))
            {
                return new ValidationResult(validationContext.MemberName + " must begin wtih an alphabet (A-Za-z).");
            }
            return ValidationResult.Success;
        }
    }

    /// <summary>
    /// [CustomModelValidation] Validate Product unit in stock.
    /// </summary>
    /// <remarks>
    /// Criteria:
    /// <br>1. It cannot be null or whitespace.</br>
    /// <br>1. If not null, it must be a be a round integer (ie. no decimal).</br>
    /// </remarks>
    public class ValidateUnitInStockAttribute : ValidationAttribute
    {
        // Keep the expression compiled to improve performance.
        private static readonly Regex ValidationRegex = new Regex(@"^[0-9]*$", RegexOptions.Compiled);

        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            if (string.IsNullOrWhiteSpace(value.ToString()))
            {
                return new ValidationResult(validationContext.MemberName + " is required.");
            }
            if (!ValidationRegex.IsMatch(value.ToString()))
            {
                return new ValidationResult(validationContext.MemberName + " must be a round integer (ie. no decimal).");
            }
            return ValidationResult.Success;
        }
    }

    /// <summary>
    /// [CustomModelValidation] Validate Product unit price.
    /// </summary>
    /// <remarks>
    /// Criteria:
    /// <br>1. It cannot be null or whitespace.</br>
    /// <br>2. It must be a valid currency denomination (0-9).</br>
    /// </remarks>
    public class ValidateUnitPriceAttribute : ValidationAttribute
    {
        // Keep the expression compiled to improve performance.
        private static readonly Regex ValidationRegex = new Regex(@"^[0-9]*[.]{0,1}[0-9]{0,2}$", RegexOptions.Compiled);

        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            if (string.IsNullOrWhiteSpace(value.ToString()))
            {
                return new ValidationResult(validationContext.MemberName + " is required.");
            }
            if (!ValidationRegex.IsMatch(value.ToString()))
            {
                return new ValidationResult(validationContext.MemberName + " must be in a valid currency denomination.");
            }
            return ValidationResult.Success;
        }
    }


    /// <summary>
    /// [CustomModelValidation] Validate nullable Product unit in stock.
    /// </summary>
    /// <remarks>
    /// Criteria:
    /// <br>1. If not null, it must be a be a round integer (ie. no decimal).</br>
    /// </remarks>
    public class ValidateNullableUnitInStockAttribute : ValidationAttribute
    {
        // Keep the expression compiled to improve performance.
        private static readonly Regex ValidationRegex = new Regex(@"^[0-9]*$", RegexOptions.Compiled);

        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            if (value != null)
            {
                if (!ValidationRegex.IsMatch(value.ToString()))
                {
                    return new ValidationResult(validationContext.MemberName + " must be a positive whole number (ie. no decimal).");
                }
            }
            return ValidationResult.Success;
        }
    }

    /// <summary>
    /// [CustomModelValidation] Validate nullable Product unit price.
    /// </summary>
    /// <remarks>
    /// Criteria:
    /// <br>1. If not null, it must be a valid currency denomination (0-9).</br>
    /// </remarks>
    public class ValidateNullableUnitPriceAttribute : ValidationAttribute
    {
        // Keep the expression compiled to improve performance.
        private static readonly Regex ValidationRegex = new Regex(@"^[0-9]*[.]{0,1}[0-9]{0,2}$", RegexOptions.Compiled);

        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            if (value != null)
            {
                if (!ValidationRegex.IsMatch(value.ToString()))
                {
                    return new ValidationResult(validationContext.MemberName + " must be in a valid positive currency denomination.");
                }
            }
            return ValidationResult.Success;
        }
    }

}