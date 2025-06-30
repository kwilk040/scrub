using System.ComponentModel.DataAnnotations;

namespace Core.Enums;

public enum PulseRangeSelection
{
    [Display(Name = "This week")] ThisWeek,
    [Display(Name = "This month")] ThisMonth,
    [Display(Name = "This year")] ThisYear,
    [Display(Name = "Last decade")] LastDecade,
}