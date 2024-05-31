using System.ComponentModel.DataAnnotations;

namespace CustomerApi.Attributes;

public class MinDateTimeAttribute : ValidationAttribute
{
    public DateTime MinDate { get; set; }

    public MinDateTimeAttribute(string minDate)
    {
        MinDate = DateTime.Parse(minDate);
    }

    public override bool IsValid(object? value)
    {
        return value.GetType() == MinDate.GetType() && value != null && ((DateTime)value) > MinDate;
    }
}