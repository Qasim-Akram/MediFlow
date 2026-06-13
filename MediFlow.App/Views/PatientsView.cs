using MediFlow.App.Forms;
using MediFlow.App.Helpers;
using MediFlow.Core.Interfaces;
using MediFlow.Core.Models;

namespace MediFlow.App.Views;

public class PatientsView : UserControl
{
    private readonly IPatientService _svc = ServiceLocator.PatientService;
    private BindingSource _binding = new();
    private DataGridView _grid = null!;
    private TextBox _txtSearch = null!;
    private ComboBox _cmbGender = null!;
    private ComboBox _cmbBloodGroup = null!;
    private Label _lblCount = null!;

    public PatientsView()
    {
        InitializeComponent();
        _ = LoadDataAsync();
    }

    private void InitializeComponent()
    {
        BackColor = Theme.Background;
        Padding = new Padding(20);

        var header = BuildHeader();
        var toolbar = BuildToolbar();
        var filterBar = BuildFilterBar();
        _grid = BuildGrid();

        var layout = new TableLayoutPanel
        {
            Dock = DockStyle.Fill,
            RowCount = 4,
            ColumnCount = 1,
            BackColor = Theme.Background
        };
        layout.RowStyles.Add(new RowStyle(SizeType.Absolute, 54));
        layout.RowStyles.Add(new RowStyle(SizeType.Absolute, 50));
        layout.RowStyles.Add(new RowStyle(SizeType.Absolute, 50));
        layout.RowStyles.Add(new RowStyle(SizeType.Percent, 100));
        layout.Controls.Add(header, 0, 0);
        layout.Controls.Add(toolbar, 0, 1);
        layout.Controls.Add(filterBar, 0, 2);
        layout.Controls.Add(_grid, 0, 3);

        Controls.Add(layout);
    }

    private Panel BuildHeader()
    {
        var p = new Panel { Dock = DockStyle.Fill, BackColor = Theme.Background };
        var lbl = new Label
        {
            Text = "Patient Management",
            Font = Theme.FontTitle,
            ForeColor = Theme.TextPrimary,
            AutoSize = false,
            Dock = DockStyle.Left,
            Width = 300,
            TextAlign = ContentAlignment.MiddleLeft
        };
        _lblCount = new Label
        {
            Font = Theme.FontSmall,
            ForeColor = Theme.TextSecondary,
            AutoSize = false,
            Dock = DockStyle.Right,
            Width = 150,
            TextAlign = ContentAlignment.MiddleRight
        };
        p.Controls.Add(_lblCount);
        p.Controls.Add(lbl);
        return p;
    }

    private Panel BuildToolbar()
    {
        var p = new Panel { Dock = DockStyle.Fill, BackColor = Theme.Background };
        var btnAdd = UIHelper.MakeButton("＋ Add", Theme.Primary);
        var btnEdit = UIHelper.MakeButton("✎ Edit", Theme.Warning);
        var btnView = UIHelper.MakeButton("👁 View", Theme.Success);
        var btnDelete = UIHelper.MakeButton("✕ Delete", Theme.Danger);
        var btnRefresh = UIHelper.MakeButton("↺ Refresh", Theme.TextSecondary);

        btnAdd.Location = new Point(0, 7);
        btnEdit.Location = new Point(118, 7);
        btnView.Location = new Point(236, 7);
        btnDelete.Location = new Point(354, 7);
        btnRefresh.Location = new Point(472, 7);

        btnAdd.Click += async (s, e) => { using var f = new PatientForm(FormMode.Add); f.ShowDialog(); await LoadDataAsync(); };
        btnEdit.Click += async (s, e) => await EditSelected();
        btnView.Click += (s, e) => ViewSelected();
        btnDelete.Click += async (s, e) => await DeleteSelected();
        btnRefresh.Click += async (s, e) => await LoadDataAsync();

        p.Controls.AddRange(new Control[] { btnAdd, btnEdit, btnView, btnDelete, btnRefresh });
        return p;
    }

    private Panel BuildFilterBar()
    {
        var p = new Panel { Dock = DockStyle.Fill, BackColor = Theme.Background };

        _txtSearch = UIHelper.MakeSearchBox("🔍  Search by name, phone, ID...");
        _txtSearch.Width = 280;
        _txtSearch.Location = new Point(0, 9);
        _txtSearch.KeyUp += async (s, e) => { if (e.KeyCode == Keys.Enter) await ApplyFilter(); };

        _cmbGender = new ComboBox { Width = 110, Location = new Point(290, 9), Font = Theme.FontRegular, DropDownStyle = ComboBoxStyle.DropDownList };
        _cmbGender.Items.AddRange(new[] { "All", "Male", "Female", "Other" });
        _cmbGender.SelectedIndex = 0;
        _cmbGender.SelectedIndexChanged += async (s, e) => await ApplyFilter();

        _cmbBloodGroup = new ComboBox { Width = 110, Location = new Point(410, 9), Font = Theme.FontRegular, DropDownStyle = ComboBoxStyle.DropDownList };
        _cmbBloodGroup.Items.AddRange(new[] { "All", "A+", "A-", "B+", "B-", "AB+", "AB-", "O+", "O-" });
        _cmbBloodGroup.SelectedIndex = 0;
        _cmbBloodGroup.SelectedIndexChanged += async (s, e) => await ApplyFilter();

        var btnSearch = UIHelper.MakeButton("Search", Theme.Primary, 90);
        btnSearch.Location = new Point(530, 7);
        btnSearch.Click += async (s, e) => await ApplyFilter();

        p.Controls.AddRange(new Control[] { _txtSearch, _cmbGender, _cmbBloodGroup, btnSearch });
        return p;
    }

    private DataGridView BuildGrid()
    {
        var grid = new DataGridView { Dock = DockStyle.Fill };
        UIHelper.StyleGrid(grid);
        grid.DataSource = _binding;
        return grid;
    }

    private async Task LoadDataAsync()
    {
        var list = await _svc.GetAllAsync();
        _binding.DataSource = list;
        _grid.DataSource = _binding;
        HideColumns();
        _lblCount.Text = $"{list.Count} record(s)";
        if (ParentForm is MainForm mf)
            mf.SetStatus($"Patients loaded — {list.Count} records");
    }

    private async Task ApplyFilter()
    {
        var kw = _txtSearch.Text.Trim();
        var gender = _cmbGender.SelectedItem?.ToString() ?? "All";
        var bg = _cmbBloodGroup.SelectedItem?.ToString() ?? "All";
        var placeholder = "🔍  Search by name, phone, ID...";
        var keyword = kw == placeholder ? "" : kw;
        var list = await _svc.SearchAsync(keyword, gender, bg);
        _binding.DataSource = list;
        _grid.DataSource = _binding;
        HideColumns();
        _lblCount.Text = $"{list.Count} record(s)";
    }

    private void HideColumns()
    {
        var hide = new[] { "PatientId", "Address" };
        foreach (DataGridViewColumn col in _grid.Columns)
            if (hide.Contains(col.Name)) col.Visible = false;
    }

    private Patient? GetSelected()
    {
        if (_binding.Current is Patient p) return p;
        UIHelper.ShowError("Please select a record first.");
        return null;
    }

    private async Task EditSelected()
    {
        var p = GetSelected(); if (p == null) return;
        using var f = new PatientForm(FormMode.Edit, p.PatientId);
        f.ShowDialog();
        await LoadDataAsync();
    }

    private void ViewSelected()
    {
        var p = GetSelected(); if (p == null) return;
        using var f = new PatientForm(FormMode.View, p.PatientId);
        f.ShowDialog();
    }

    private async Task DeleteSelected()
    {
        var p = GetSelected(); if (p == null) return;
        if (!UIHelper.ConfirmDelete(p.FullName)) return;
        await _svc.DeleteAsync(p.PatientId);
        UIHelper.ShowSuccess("Patient deleted successfully.");
        await LoadDataAsync();
    }
}