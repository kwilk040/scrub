using Application.Services.Interfaces;
using BlazorBootstrap;
using Core.Enums;

namespace WebUI.Components.Features.Pulse;

public class ChartFactory(IScrobbleService scrobbleService)
{
    public async Task<(ChartData, BarChartOptions)> CreateChartBuildDataAsync(PulseRangeSelection selectionOption)
    {
        switch (selectionOption)
        {
            case PulseRangeSelection.ThisWeek: return (await CreateWeeklyChartData(), ChartOptions);
            case PulseRangeSelection.ThisMonth: return (await CreateMonthlyChartData(), ChartOptions);
            case PulseRangeSelection.ThisYear: return (await CreateYearlyChart(), ChartOptions);
            case PulseRangeSelection.LastDecade: return (await CreateLastDecadeChart(), ChartOptions);
            default: throw new ArgumentOutOfRangeException(nameof(selectionOption), selectionOption, null);
        }
    }

    private async Task<ChartData> CreateWeeklyChartData()
    {
        var data = await scrobbleService.GetWeeklyPulseAsync();

        var weekDays = Enum.GetValues<DayOfWeek>()
            .OrderBy(dayOfWeek => dayOfWeek)
            .Reverse()
            .ToDictionary(dayOfWeek => dayOfWeek, dayOfWeek => data.GetValueOrDefault(dayOfWeek, 0));

        var labels = weekDays.Select(x => x.Key);

        var dataset = new BarChartDataset
        {
            Data = [.. weekDays.Select(x => Convert.ToDouble(x.Value)).ToList()],
            BackgroundColor =
            [
                "#31748f", // love
                "#9ccfd8", // foam
            ],
            BorderColor = [ColorUtility.CategoricalTwelveColors[0]],
            BorderWidth = [0],
            CategoryPercentage = .5,
        };

        return new ChartData
        {
            Labels = [..labels.Select(l => l.ToString())],
            Datasets = [dataset]
        };
    }

    private async Task<ChartData> CreateMonthlyChartData()
    {
        // TODO: display weeks instead of days
        var data = await scrobbleService
            .GetMonthlyPulseAsync();

        var year = DateTime.Now.Year;
        var month = DateTime.Now.Month;
        var daysInMonth = DateTime.DaysInMonth(year, month);

        var map = Enumerable
            .Range(1, daysInMonth)
            .Select(day => new DateOnly(year, month, day))
            .Reverse()
            .ToDictionary(date => date, date => data.GetValueOrDefault(date, 0));

        var dataset = new BarChartDataset
        {
            Data = [.. map.Values],
            BackgroundColor =
            [
                "#31748f", // love
                "#9ccfd8", // foam
            ],
            BorderColor = [ColorUtility.CategoricalTwelveColors[0]],
            BorderWidth = [0],
            CategoryPercentage = .5,
        };

        return new ChartData
        {
            Labels = [..map.Keys.Select(l => l.ToString())],
            Datasets = [dataset]
        };
    }

    private async Task<ChartData> CreateYearlyChart()
    {
        var data = await scrobbleService.GetYearlyPulseAsync();

        IEnumerable<string> months =
        [
            "January",
            "February",
            "March",
            "April",
            "May",
            "June",
            "July",
            "August",
            "September",
            "October",
            "November",
            "December",
        ];

        var map = months.Reverse().ToDictionary(month => month, moth => data.GetValueOrDefault(moth, 0));

        var dataset = new BarChartDataset
        {
            Data = [.. map.Values],
            BackgroundColor =
            [
                "#31748f", // love
                "#9ccfd8", // foam
            ],
            BorderColor = [ColorUtility.CategoricalTwelveColors[0]],
            BorderWidth = [0],
            CategoryPercentage = .5,
        };

        return new ChartData
        {
            Labels = [..map.Keys],
            Datasets = [dataset]
        };
    }

    private async Task<ChartData> CreateLastDecadeChart()
    {
        var data = await scrobbleService.GetDecadePulseAsync();

        var year = DateTime.Now.Year;

        var map = Enumerable.Range(0, 9)
            .Select(index => year - index)
            .Reverse()
            .ToDictionary(year => year, year => data.GetValueOrDefault(year, 0));

        var dataset = new BarChartDataset
        {
            Data = [.. map.Values],
            BackgroundColor =
            [
                "#31748f", // love
                "#9ccfd8", // foam
            ],
            BorderColor = [ColorUtility.CategoricalTwelveColors[0]],
            BorderWidth = [0],
            CategoryPercentage = .5,
        };

        return new ChartData
        {
            Labels = [..map.Keys.Select(l => l.ToString())],
            Datasets = [dataset]
        };
    }

    private static BarChartOptions ChartOptions =>
        new()
        {
            MaintainAspectRatio = false,
            Responsive = true,
            IndexAxis = "y",
            Layout = new ChartLayout { Padding = 0 },
            Interaction = new Interaction { Mode = InteractionMode.Nearest },
            Scales = new Scales()
            {
                X = new ChartAxes
                {
                    Grid = new ChartAxesGrid
                    {
                        Display = false,
                    },
                },
                Y = new ChartAxes
                {
                    Grid = new ChartAxesGrid
                    {
                        Display = false,
                    },
                },
            },
            Plugins = new BarChartPlugins()
            {
                Legend =
                {
                    Display = false,
                },
            },
        };
}