using LiveChartsCore;
using LiveChartsCore.SkiaSharpView;
using LiveChartsCore.SkiaSharpView.WinForms;
using LiveChartsCore.SkiaSharpView.Painting;
using SkiaSharp;
using MediFlow.App.Helpers;
using MediFlow.Core.Interfaces;

namespace MediFlow.App.Views;

public class DashboardView : UserControl
{
    private readonly IPatientService     _patSvc  = ServiceLocator.PatientService;
    private readonly IDoctorService      _docSvc  = ServiceLocator.DoctorService;
    private readonly IAppointmentService _apptSvc = ServiceLocator.AppointmentService;

    private Label _lblPatients      = null!;
    private Label _lblDoctors       = null!;
    private Label _lblAppointments  = null!;
    private Label _lblToday         = null!;
    private PieChart _pieChart      = null!;
    private CartesianChart _barChart = null!;

    public DashboardView()
    {
        InitializeComponent();
        _ = LoadAsync();
    }

    private void InitializeComponent()
    {
        BackColor = Theme.Background;
        Padding = new Padding(24);

        var title = new Label
        {
            Text = "Dashboard",
            Font = Theme.FontTitle,
            ForeColor = Theme.TextPrimary,
            AutoSize = true,
            Location = new Point(0, 0)
        };

        var subtitle = new Label
        {
            Text = $"Welcome back  •  {DateTime.Now:dddd, MMMM dd yyyy}",
            Font = Theme.FontSubtitle,
            ForeColor = Theme.TextSecondary,
            AutoSize = true,
            Location = new Point(0, 36)
        };

        var statPanel = new FlowLayoutPanel
        {
            Location = new Point(0, 80),
            Size = new Size(820, 100),
            BackColor = Theme.Background,
            FlowDirection = FlowDirection.LeftToRight,
            WrapContents = false
        };

        var cardPatients     = UIHelper.MakeStatCard("Total Patients",     "...", Theme.Primary);
        var cardDoctors      = UIHelper.MakeStatCard("Total Doctors",      "...", Theme.Success);
        var cardAppointments = UIHelper.MakeStatCard("Total Appointments", "...", Theme.Warning);
        var cardToday        = UIHelper.MakeStatCard("Today's OPD",        "...", Theme.Danger);

        _lblPatients     = (Label)cardPatients.Controls[1];
        _lblDoctors      = (Label)cardDoctors.Controls[1];
        _lblAppointments = (Label)cardAppointments.Controls[1];
        _lblToday        = (Label)cardToday.Controls[1];

        statPanel.Controls.AddRange(new Control[] { cardPatients, cardDoctors, cardAppointments, cardToday });

        var chartTitle1 = new Label { Text = "Appointments by Status", Font = Theme.FontMedium, ForeColor = Theme.TextPrimary, AutoSize = true, Location = new Point(0, 200) };
        var chartTitle2 = new Label { Text = "Monthly Appointments (Last 6 Months)", Font = Theme.FontMedium, ForeColor = Theme.TextPrimary, AutoSize = true, Location = new Point(420, 200) };

        _pieChart = new PieChart
        {
            Location = new Point(0, 228),
            Size = new Size(380, 260),
            BackColor = Theme.Surface
        };

        _barChart = new CartesianChart
        {
            Location = new Point(420, 228),
            Size = new Size(560, 260),
            BackColor = Theme.Surface
        };

        Controls.AddRange(new Control[] { title, subtitle, statPanel, chartTitle1, chartTitle2, _pieChart, _barChart });
    }

    private async Task LoadAsync()
    {
        var patCount  = await _patSvc.GetTotalCountAsync();
        var docCount  = await _docSvc.GetTotalCountAsync();
        var apptCount = await _apptSvc.GetTotalCountAsync();
        var todayCount = await _apptSvc.GetTodaysCountAsync();

        _lblPatients.Text     = patCount.ToString();
        _lblDoctors.Text      = docCount.ToString();
        _lblAppointments.Text = apptCount.ToString();
        _lblToday.Text        = todayCount.ToString();

        await LoadPieChartAsync();
        await LoadBarChartAsync();
    }

    private async Task LoadPieChartAsync()
    {
        var stats = await _apptSvc.GetStatusStatsAsync();

        var colors = new Dictionary<string, SKColor>
        {
            ["Scheduled"]  = SKColor.Parse("#2563EB"),
            ["Completed"]  = SKColor.Parse("#16A34A"),
            ["Cancelled"]  = SKColor.Parse("#DC2626")
        };

        var series = stats.Select(kvp => new PieSeries<double>
        {
            Values = new double[] { kvp.Value },
            Name = kvp.Key,
            Fill = new SolidColorPaint(colors.TryGetValue(kvp.Key, out var c) ? c : SKColors.Gray),
            DataLabelsSize = 13,
            DataLabelsPaint = new SolidColorPaint(SKColors.White),
            DataLabelsFormatter = p => $"{kvp.Key}\n{kvp.Value}"
        }).ToArray();

        _pieChart.Series = series;
        _pieChart.LegendPosition = LiveChartsCore.Measure.LegendPosition.Right;
    }

    private async Task LoadBarChartAsync()
    {
        var stats = await _apptSvc.GetMonthlyStatsAsync();

        var labels = stats.Keys.ToArray();
        var values = stats.Values.Select(v => (double)v).ToArray();

        _barChart.Series = new ISeries[]
        {
            new ColumnSeries<double>
            {
                Values = values,
                Name = "Appointments",
                Fill = new SolidColorPaint(SKColor.Parse("#2563EB")),
                DataLabelsPaint = new SolidColorPaint(SKColor.Parse("#0F172A")),
                DataLabelsSize = 11,
                DataLabelsPosition = LiveChartsCore.Measure.DataLabelsPosition.Top,
                DataLabelsFormatter = p => p.Coordinate.PrimaryValue.ToString("0")
            }
        };

        _barChart.XAxes = new[]
        {
            new Axis
            {
                Labels = labels,
                LabelsPaint = new SolidColorPaint(SKColor.Parse("#64748B")),
                TextSize = 11
            }
        };

        _barChart.YAxes = new[]
        {
            new Axis
            {
                LabelsPaint = new SolidColorPaint(SKColor.Parse("#64748B")),
                TextSize = 11
            }
        };
    }
}
