using System.ComponentModel.DataAnnotations;

namespace Core.Enums;

public enum RangeSelection
{
    [Display(Name = "Today")] Today,
    [Display(Name = "This week")] ThisWeek,
    [Display(Name = "This month")] ThisMonth,
    [Display(Name = "This year")] ThisYear,
    [Display(Name = "All time")] AllTime,
}