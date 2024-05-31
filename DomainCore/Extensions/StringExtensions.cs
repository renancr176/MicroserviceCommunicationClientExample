using System.Globalization;
using System.Text.RegularExpressions;

namespace DomainCore.Extensions;

public static class StringExtensions
{

    public static bool IsValidEmail(this string email)
    {
        if (string.IsNullOrWhiteSpace(email))
            return false;

        if (email.Contains(";"))
        {
            var results = new List<bool>();
            foreach (var e in email.Split(";"))
            {
                results.Add(e.IsValidEmail());
            }

            return results.All(result => result);
        }

        try
        {
            // Normalize the domain
            email = Regex.Replace(email, @"(@)(.+)$", DomainMapper,
                RegexOptions.None, TimeSpan.FromMilliseconds(200));

            // Examines the domain part of the email and normalizes it.
            string DomainMapper(Match match)
            {
                // Use IdnMapping class to convert Unicode domain names.
                var idn = new IdnMapping();

                // Pull out and process domain name (throws ArgumentException on invalid)
                string domainName = idn.GetAscii(match.Groups[2].Value);

                return match.Groups[1].Value + domainName;
            }
        }
        catch (RegexMatchTimeoutException e)
        {
            return false;
        }
        catch (ArgumentException e)
        {
            return false;
        }

        try
        {
            return Regex.IsMatch(email,
                @"^[^@\s]+@[^@\s]+\.[^@\s]+$",
                RegexOptions.IgnoreCase, TimeSpan.FromMilliseconds(250));
        }
        catch (RegexMatchTimeoutException)
        {
            return false;
        }
    }

    public static bool ValueExistsInEnum<TEnum>(this string value) where TEnum : struct, Enum
    {
        var enumValues = Enum.GetValues<TEnum>().ToList();
        return enumValues != null && enumValues.Any(e => e.ToString().Trim().ToLower() == value.Trim().ToLower());
    }

    public static TEnum? StringToEnum<TEnum>(this string value) where TEnum : struct, Enum
    {
        var enumValues = Enum.GetValues<TEnum>().ToList();
        return enumValues != null ? enumValues.FirstOrDefault(e => e.ToString().Trim().ToLower() == value.Trim().ToLower()) : null;
    }
}