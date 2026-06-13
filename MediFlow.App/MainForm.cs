using MediFlow.App.Helpers;
using MediFlow.App.Views;

namespace MediFlow.App;

public class MainForm : Form
{
    private Panel _sidebar = null!;
    private Panel _contentPanel = null!;
    private StatusStrip _statusBar = null!;
    private ToolStripStatusLabel _lblStatus = null!;
    private ToolStripStatusLabel _lblTime = null!;
    private System.Windows.Forms.Timer _clockTimer = null!;
    private Button? _activeBtn;

    public MainForm()
    {
        InitializeComponent();
        ShowView(new DashboardView(), null);
    }

    private void InitializeComponent()
    {
        Text = "MediFlow — OPD Management System";
        Size = new Size(1200, 750);
        MinimumSize = new Size(1000, 650);
        StartPosition = FormStartPosition.CenterScreen;
        BackColor = Theme.Background;
        Font = Theme.FontRegular;

        BuildStatusBar();
        BuildSidebar();
        BuildContentArea();
        BuildClock();

        Controls.Add(_statusBar);
        Controls.Add(_contentPanel);
        Controls.Add(_sidebar);
    }

    private void BuildSidebar()
    {
        _sidebar = new Panel
        {
            Dock = DockStyle.Left,
            Width = 210,
            BackColor = Theme.Sidebar
        };

        var logo = new Panel
        {
            Height = 70,
            Dock = DockStyle.Top,
            BackColor = Theme.Sidebar,
            Padding = new Padding(18, 0, 0, 0)
        };

        var lblLogo = new Label
        {
            Text = "💉 MediFlow",
            Font = Theme.FontLogo,
            ForeColor = Color.White,
            Dock = DockStyle.Fill,
            TextAlign = ContentAlignment.MiddleLeft
        };
        logo.Controls.Add(lblLogo);

        var sep = new Panel { Height = 1, Dock = DockStyle.Top, BackColor = Theme.SidebarHover };

        var navPanel = new Panel { Dock = DockStyle.Fill, BackColor = Theme.Sidebar, Padding = new Padding(0, 10, 0, 0) };

        var btnDashboard = MakeSidebarButton("🏠  Dashboard", 0);
        var btnPatients = MakeSidebarButton("👤  Patients", 1);
        var btnDoctors = MakeSidebarButton("🩺  Doctors", 2);
        var btnAppointments = MakeSidebarButton("📋  Appointments", 3);

        btnDashboard.Click += (s, e) => { ShowView(new DashboardView(), btnDashboard); };
        btnPatients.Click += (s, e) => { ShowView(new PatientsView(), btnPatients); };
        btnDoctors.Click += (s, e) => { ShowView(new DoctorsView(), btnDoctors); };
        btnAppointments.Click += (s, e) => { ShowView(new AppointmentsView(), btnAppointments); };

        navPanel.Controls.AddRange(new Control[] { btnAppointments, btnDoctors, btnPatients, btnDashboard });

      

        _sidebar.Controls.Add(navPanel);
        _sidebar.Controls.Add(sep);
        _sidebar.Controls.Add(logo);
       
    }

    private Button MakeSidebarButton(string text, int index)
    {
        var btn = new Button
        {
            Text = text,
            Dock = DockStyle.Top,
            Height = 46,
            FlatStyle = FlatStyle.Flat,
            BackColor = Theme.Sidebar,
            ForeColor = Color.White,
            Font = Theme.FontSidebar,
            TextAlign = ContentAlignment.MiddleLeft,
            Padding = new Padding(20, 0, 0, 0),
            Cursor = Cursors.Hand,
            FlatAppearance = { BorderSize = 0, MouseOverBackColor = Theme.SidebarHover }
        };
        return btn;
    }

    private void BuildContentArea()
    {
        _contentPanel = new Panel
        {
            Dock = DockStyle.Fill,
            BackColor = Theme.Background,
            Padding = new Padding(0)
        };
    }

    private void BuildStatusBar()
    {
        _statusBar = new StatusStrip { BackColor = Theme.Sidebar, ForeColor = Color.White };
        _lblStatus = new ToolStripStatusLabel("Ready") { ForeColor = Color.White, Font = Theme.FontSmall };
        _lblTime = new ToolStripStatusLabel("") { ForeColor = Color.White, Font = Theme.FontSmall, Alignment = ToolStripItemAlignment.Right };
        var spacer = new ToolStripStatusLabel { Spring = true };
        _statusBar.Items.AddRange(new ToolStripItem[] { _lblStatus, spacer, _lblTime });
    }

    private void BuildClock()
    {
        _clockTimer = new System.Windows.Forms.Timer { Interval = 1000 };
        _clockTimer.Tick += (s, e) => _lblTime.Text = DateTime.Now.ToString("ddd, dd MMM yyyy  •  hh:mm:ss tt");
        _clockTimer.Start();
        _lblTime.Text = DateTime.Now.ToString("ddd, dd MMM yyyy  •  hh:mm:ss tt");
    }

    public void ShowView(UserControl view, Button? btn)
    {
        if (_activeBtn != null)
            _activeBtn.BackColor = Theme.Sidebar;

        if (btn != null)
        {
            btn.BackColor = Theme.SidebarActive;
            _activeBtn = btn;
        }

        _contentPanel.Controls.Clear();
        view.Dock = DockStyle.Fill;
        _contentPanel.Controls.Add(view);
        _lblStatus.Text = $"Viewing: {view.GetType().Name.Replace("View", "")}  •  {DateTime.Now:hh:mm tt}";
    }

    public void SetStatus(string message) => _lblStatus.Text = message;
}