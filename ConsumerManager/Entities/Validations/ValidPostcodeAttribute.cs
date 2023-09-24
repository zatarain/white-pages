using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;

namespace ConsumerManager.Entities.Validations
{
  public class ValidPostcodeAttribute: ValidationAttribute
  {
    private readonly Dictionary<string, string> patterns = new() {
      {"GB", "^[A-Z]{1,2}[0-9][0-9A-Z]? ?[0-9][A-Z]{2}$" },
      {"MX", "^[0-9]{5}$" },
      {"US", "^[0-9]{5}$" },
    };
    protected override ValidationResult? IsValid(object? value, ValidationContext context)
    {
      var request = context.ObjectInstance as CreateAddressRequest;
      var country = request?.Country ?? "GB";
      if (request is not null && patterns.ContainsKey(country))
      {
        var postcode = value?.ToString() ?? string.Empty;
        if (!Regex.Match(postcode, patterns[country], RegexOptions.IgnoreCase).Success)
        {
          return new ValidationResult($"Invalid postcode for country '{country}'");
        }
      }
      return ValidationResult.Success;
    }
  }
}
