using MediFlow.App.Forms;
using MediFlow.App.Helpers;
using MediFlow.Core.Interfaces;
using MediFlow.Core.Models;

namespace MediFlow.App.Views;

public class AppointmentsView : UserControl
{
    private readonly IAppointmentService _svc = ServiceLocator.AppointmentService;
    private BindingSource _binding = new();
    private DataGridView _grid = null!;
    private TextBox _txtSearch = null!;
    private ComboBox _cmbStatus = null!;
    private DateTimePicker _dtpFilter = null!;
    private CheckBox _chkDate = null!;
    private Label _lblCount = null!;

    public AppointmentsView()
    {
        InitializeComponent();
        _ = LoadDataAsync();
    }

    private void InitializeComponent()
    {
        BackColor = Theme.Background;
        Padding = new Padding(20);

        var layout = new TableLayoutPanel
        {
            Dock = DockStyle.Fill, RowCount = 4, ColumnCount = 1, BackColor = Theme.Background
        };
        layout.RowStyles.Add(new RowStyle(SizeType.Absolute, 54));
        layout.RowStyles.Add(new RowStyle(SizeType.Absolute, 50));
        layout.RowStyles.Add(new RowStyle(SizeType.Absolute, 50));
        layout.RowStyles.Add(new RowStyle(SizeType.Percent, 100));

        layout.Controls.Add(BuildHeader(), 0, 0);
        layout.Controls.Add(BuildToolbar(), 0, 1);
        layout.Controls.Add(BuildFilterBar(), 0, 2);

        _grid = new DataGridView { Dock = DockStyle.Fill };
        UIHelper.StyleGrid(_grid);
        _grid.DataSource = _binding;
        layout.Controls.Add(_grid, 0, 3);

        Controls.Add(layout);
    }

    private Panel BuildHeader()
    {
        var p = new Panel { Dock = DockStyle.Fill, BackColor = Theme.Background };
        var lbl = new Label { Text = "Appointment Management", Font = Theme.FontTitle, ForeColor = Theme.TextPrimary, AutoSize = false, Dock = DockStyle.Left, Width = 350, TextAlign = ContentAlignment.MiddleLeft };
        _lblCount = new Label { Font = Theme.FontSmall, ForeColor = Theme.TextSecondary, AutoSize = false, Dock = DockStyle.Right, Width = 150, TextAlign = ContentAlignment.MiddleRight };
        p.Controls.Add(_lblCount); p.Controls.Add(lbl);
        return p;
    }

    private Panel BuildToolbar()
    {
        var p = new Panel { Dock = DockStyle.Fill, BackColor = Theme.Background };
        var btnAdd     = UIHelper.MakeButton("＋ Add",    Theme.Primary);
        var btnEdit    = UIHelper.MakeButton("✎ Edit",   Theme.Warning);
        var btnView    = UIHelper.MakeButton("👁 View",  Theme.Success);
        var btnDelete  = UIHelper.MakeButton("✕ Delete", Theme.Danger);
        var btnRefresh = UIHelper.MakeButton("↺ Refresh", Theme.TextSecondary);
        btnAdd.Location = new Point(0, 7); btnEdit.Location = new Point(118, 7);
        btnView.Location = new Point(236, 7); btnDelete.Location = new Point(354, 7);
        btnRefresh.Location = new Point(472, 7);
        btnAdd.Click     += async (s, e) => { using var f = new AppointmentForm(FormMode.Add); f.ShowDialog(); await LoadDataAsync(); };
        btnEdit.Click    += async (s, e) => await EditSelected();
        btnView.Click    += (s, e) => ViewSelected();
        btnDelete.Click  += async (s, e) => await DeleteSelected();
        btnRefresh.Click += async (s, e) => await LoadDataAsync();
        p.Controls.AddRange(new Control[] { btnAdd, btnEdit, btnView, btnDelete, btnRefresh });
        return p;
    }

    private Panel BuildFilterBar()
    {
        var p = new Panel { Dock = DockStyle.Fill, BackColor = Theme.Background };
        _txtSearch = UIHelper.MakeSearchBox("🔍  Search by patient, doctor, token...");
        _txtSearch.Width = 250; _txtSearch.Location = new Point(0, 9);

        _cmbStatus = new ComboBox { Width = 130, Location = new Point(260, 9), Font = Theme.FontRegular, DropDownStyle = ComboBoxStyle.DropDownList };
        _cmbStatus.Items.AddRange(new[] { "All", "Scheduled", "Completed", "Cancelled" });
        _cmbStatus.SelectedIndex = 0;
        _cmbStatus.SelectedIndexChanged += async (s, e) => await ApplyFilter();

        _chkDate = new CheckBox { Text = "Date:", Location = new Point(400, 12), Font = Theme.FontRegular, ForeColor = Theme.TextPrimary, AutoSize = true };
        _dtpFilter = new DateTimePicker { Width = 150, Location = new Point(450, 9), Font = Theme.FontRegular, Format = DateTimePickerFormat.Short, Enabled = false };
        _chkDate.CheckedChanged += (s, e) => { _dtpFilter.Enabled = _chkDate.Checked; };
        _dtpFilter.ValueChanged += async (s, e) => { if (_chkDate.Checked) await ApplyFilter(); };

        var btnSearch = UIHelper.MakeButton("Search", Theme.Primary, 90);
        btnSearch.Location = new Point(610, 7);
        btnSearch.Click += async (s, e) => await ApplyFilter();

        p.Controls.AddRange(new Control[] { _txtSearch, _cmbStatus, _chkDate, _dtpFilter, btnSearch });
        return p;
    }

    private async Task LoadDataAsync()
    {
        var list = await _svc.GetAllAsync();
        _binding.DataSource = list;
        _grid.DataSource = _binding;
        HideColumns();
        _lblCount.Text = $"{list.Count} record(s)";
        if (ParentForm is MainForm mf) mf.SetStatus($"Appointments loaded — {list.Count} records");
    }

    private async Task ApplyFilter()
    {
        var kw = _txtSearch.Text.Trim();
        var placeholder = "🔍  Search by patient, doctor, token...";
        var statusStr = _cmbStatus.SelectedItem?.ToString();
        AppointmentStatus? status = statusStr != null && statusStr != "All" ? Enum.Parse<AppointmentStatus>(statusStr) : null;
        DateTime? date = _chkDate.Checked ? _dtpFilter.Value.Date : null;
        var list = await _svc.SearchAsync(kw == placeholder ? "" : kw, status, date);
        _binding.DataSource = list;
        _grid.DataSource = _binding;
        HideColumns();
        _lblCount.Text = $"{list.Count} record(s)";
    }

    private void HideColumns()
    {
        var hide = new[] { "AppointmentId", "PatientId", "DoctorId", "Notes" };
        foreach (DataGridViewColumn col in _grid.Columns)
            if (hide.Contains(col.Name)) col.Visible = false;
    }

    private Appointment? GetSelected() { if (_binding.Current is Appointment a) return a; UIHelper.ShowError("Select a record first."); return null; }
    private async Task EditSelected() { var a = GetSelected(); if (a == null) return; using var f = new AppointmentForm(FormMode.Edit, a.AppointmentId); f.ShowDialog(); await LoadDataAsync(); }
    private void ViewSelected() { var a = GetSelected(); if (a == null) return; using var f = new AppointmentForm(FormMode.View, a.AppointmentId); f.ShowDialog(); }
    private async Task DeleteSelected()
    {
        var a = GetSelected(); if (a == null) return;
        if (!UIHelper.ConfirmDelete($"appointment for {a.PatientName}")) return;
        await _svc.DeleteAsync(a.AppointmentId);
        UIHelper.ShowSuccess("Appointment deleted.");
        await LoadDataAsync();
    }
}
