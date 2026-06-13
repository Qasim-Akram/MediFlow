using MediFlow.App.Forms;
using MediFlow.App.Helpers;
using MediFlow.Core.Interfaces;
using MediFlow.Core.Models;

namespace MediFlow.App.Views;

public class DoctorsView : UserControl
{
    private readonly IDoctorService _svc = ServiceLocator.DoctorService;
    private BindingSource _binding = new();
    private DataGridView _grid = null!;
    private TextBox _txtSearch = null!;
    private ComboBox _cmbSpec = null!;
    private ComboBox _cmbAvail = null!;
    private Label _lblCount = null!;

    public DoctorsView()
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
        var lbl = new Label { Text = "Doctor Management", Font = Theme.FontTitle, ForeColor = Theme.TextPrimary, AutoSize = false, Dock = DockStyle.Left, Width = 300, TextAlign = ContentAlignment.MiddleLeft };
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
        btnAdd.Click     += async (s, e) => { using var f = new DoctorForm(FormMode.Add); f.ShowDialog(); await LoadDataAsync(); };
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
        _txtSearch = UIHelper.MakeSearchBox("Search by name, email, ID...");
        _txtSearch.Width = 260; _txtSearch.Location = new Point(0, 9);
        _txtSearch.KeyUp += async (s, e) => { if (e.KeyCode == Keys.Enter) await ApplyFilter(); };

        _cmbSpec = new ComboBox { Width = 160, Location = new Point(270, 9), Font = Theme.FontRegular, DropDownStyle = ComboBoxStyle.DropDownList };
        _cmbSpec.Items.Add("All");
        _cmbSpec.SelectedIndex = 0;
        _cmbSpec.SelectedIndexChanged += async (s, e) => await ApplyFilter();

        _cmbAvail = new ComboBox { Width = 120, Location = new Point(440, 9), Font = Theme.FontRegular, DropDownStyle = ComboBoxStyle.DropDownList };
        _cmbAvail.Items.AddRange(new[] { "All", "Available", "Unavailable" });
        _cmbAvail.SelectedIndex = 0;
        _cmbAvail.SelectedIndexChanged += async (s, e) => await ApplyFilter();

        var btnSearch = UIHelper.MakeButton("Search", Theme.Primary, 90);
        btnSearch.Location = new Point(570, 7);
        btnSearch.Click += async (s, e) => await ApplyFilter();

        p.Controls.AddRange(new Control[] { _txtSearch, _cmbSpec, _cmbAvail, btnSearch });
        return p;
    }

    private async Task LoadDataAsync()
    {
        var list = await _svc.GetAllAsync();
        _binding.DataSource = list;
        _grid.DataSource = _binding;
        HideColumns();
        _lblCount.Text = $"{list.Count} records";

        var specs = await _svc.GetSpecializationsAsync();
        var current = _cmbSpec.SelectedItem?.ToString();
        _cmbSpec.Items.Clear();
        _cmbSpec.Items.Add("All");
        foreach (var s in specs) _cmbSpec.Items.Add(s);
        _cmbSpec.SelectedItem = current ?? "All";
        if (_cmbSpec.SelectedIndex < 0) _cmbSpec.SelectedIndex = 0;

        if (ParentForm is MainForm mf) mf.SetStatus($"Doctors loaded — {list.Count} records");
    }

    private async Task ApplyFilter()
    {
        var kw    = _txtSearch.Text.Trim();
        var spec  = _cmbSpec.SelectedItem?.ToString() ?? "All";
        var avail = _cmbAvail.SelectedItem?.ToString();
        bool? isAvail = avail == "Available" ? true : avail == "Unavailable" ? false : null;
        var placeholder = "Search by name, email, ID...";
        var list = await _svc.SearchAsync(kw == placeholder ? "" : kw, spec, isAvail);
        _binding.DataSource = list;
        _grid.DataSource = _binding;
        HideColumns();
        _lblCount.Text = $"{list.Count} records";
    }

    private void HideColumns() { if (_grid.Columns["DoctorId"] != null) _grid.Columns["DoctorId"]!.Visible = false; }

    private Doctor? GetSelected() { if (_binding.Current is Doctor d) return d; UIHelper.ShowError("Select a record first."); return null; }

    private async Task EditSelected() { var d = GetSelected(); if (d == null) return; using var f = new DoctorForm(FormMode.Edit, d.DoctorId); f.ShowDialog(); await LoadDataAsync(); }
    private void ViewSelected() { var d = GetSelected(); if (d == null) return; using var f = new DoctorForm(FormMode.View, d.DoctorId); f.ShowDialog(); }
    private async Task DeleteSelected()
    {
        var d = GetSelected(); if (d == null) return;
        if (!UIHelper.ConfirmDelete(d.FullName)) return;
        await _svc.DeleteAsync(d.DoctorId);
        UIHelper.ShowSuccess("Doctor deleted.");
        await LoadDataAsync();
    }
}
