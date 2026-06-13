using MediFlow.App.Helpers;
using MediFlow.Core.Interfaces;
using MediFlow.Core.Models;

namespace MediFlow.App.Forms;

public class PatientForm : Form
{
    private readonly FormMode _mode;
    private readonly string? _id;
    private readonly IPatientService _svc = ServiceLocator.PatientService;
    private Patient? _patient;

    private TextBox _txtName = null!, _txtPhone = null!, _txtAddress = null!;
    private ComboBox _cmbGender = null!, _cmbBloodGroup = null!;
    private DateTimePicker _dtpDob = null!;
    private Button _btnSave = null!, _btnCancel = null!;

    public PatientForm(FormMode mode, string? id = null)
    {
        _mode = mode;
        _id = id;
        InitializeComponent();
        _ = LoadAsync();
    }

    private void InitializeComponent()
    {
        Text = _mode switch { FormMode.Add => "Add Patient", FormMode.Edit => "Edit Patient", _ => "View Patient" };
        Size = new Size(480, 480);
        StartPosition = FormStartPosition.CenterParent;
        FormBorderStyle = FormBorderStyle.FixedDialog;
        MaximizeBox = false; MinimizeBox = false;
        BackColor = Theme.Surface;
        Font = Theme.FontRegular;

        var title = new Label { Text = Text, Font = Theme.FontTitle, ForeColor = Theme.TextPrimary, Location = new Point(24, 20), AutoSize = true };

        int y = 70;
        _txtName       = AddField("Full Name *",    ref y);
        _cmbGender     = AddCombo("Gender *",       ref y, new[] { "Male", "Female", "Other" });
        _dtpDob        = AddDatePicker("Date of Birth *", ref y);
        _txtPhone      = AddField("Phone",          ref y);
        _cmbBloodGroup = AddCombo("Blood Group",    ref y, new[] { "", "A+", "A-", "B+", "B-", "AB+", "AB-", "O+", "O-" });
        _txtAddress    = AddField("Address",        ref y);

        _btnSave = UIHelper.MakeButton(_mode == FormMode.Add ? "Save" : "Update", Theme.Primary, 110, 38);
        _btnSave.Location = new Point(240, y + 10);
        _btnSave.Click += async (s, e) => await SaveAsync();

        _btnCancel = UIHelper.MakeButton("Cancel", Theme.TextSecondary, 100, 38);
        _btnCancel.Location = new Point(358, y + 10);
        _btnCancel.Click += (s, e) => Close();

        Controls.Add(title);
        Controls.Add(_btnSave);
        Controls.Add(_btnCancel);
        if (_mode == FormMode.View) _btnSave.Visible = false;
    }

    private TextBox AddField(string label, ref int y)
    {
        Controls.Add(new Label { Text = label, Location = new Point(24, y), AutoSize = true, Font = Theme.FontSmall, ForeColor = Theme.TextSecondary });
        var tb = new TextBox { Location = new Point(24, y + 18), Width = 410, Font = Theme.FontRegular, BorderStyle = BorderStyle.FixedSingle };
        Controls.Add(tb);
        y += 58;
        return tb;
    }

    private ComboBox AddCombo(string label, ref int y, string[] items)
    {
        Controls.Add(new Label { Text = label, Location = new Point(24, y), AutoSize = true, Font = Theme.FontSmall, ForeColor = Theme.TextSecondary });
        var cb = new ComboBox { Location = new Point(24, y + 18), Width = 410, Font = Theme.FontRegular, DropDownStyle = ComboBoxStyle.DropDownList };
        cb.Items.AddRange(items);
        cb.SelectedIndex = 0;
        Controls.Add(cb);
        y += 58;
        return cb;
    }

    private DateTimePicker AddDatePicker(string label, ref int y)
    {
        Controls.Add(new Label { Text = label, Location = new Point(24, y), AutoSize = true, Font = Theme.FontSmall, ForeColor = Theme.TextSecondary });
        var dtp = new DateTimePicker { Location = new Point(24, y + 18), Width = 410, Font = Theme.FontRegular, Format = DateTimePickerFormat.Short, Value = DateTime.Today.AddYears(-25) };
        Controls.Add(dtp);
        y += 58;
        return dtp;
    }

    private async Task LoadAsync()
    {
        if (_id == null) return;
        _patient = await _svc.GetByIdAsync(_id);
        if (_patient == null) return;
        _txtName.Text = _patient.FullName;
        _cmbGender.SelectedItem = _patient.Gender;
        _dtpDob.Value = _patient.DateOfBirth;
        _txtPhone.Text = _patient.Phone;
        _cmbBloodGroup.SelectedItem = _patient.BloodGroup;
        _txtAddress.Text = _patient.Address;

        if (_mode == FormMode.View)
            foreach (Control c in Controls)
                if (c is TextBox tb) tb.ReadOnly = true;
    }

    private async Task SaveAsync()
    {
        if (string.IsNullOrWhiteSpace(_txtName.Text)) { UIHelper.ShowError("Full Name is required."); return; }
        if (_cmbGender.SelectedIndex < 0) { UIHelper.ShowError("Gender is required."); return; }

        var p = new Patient
        {
            PatientId   = _patient?.PatientId ?? "",
            FullName    = _txtName.Text.Trim(),
            Gender      = _cmbGender.SelectedItem!.ToString()!,
            DateOfBirth = _dtpDob.Value.Date,
            Phone       = _txtPhone.Text.Trim(),
            BloodGroup  = _cmbBloodGroup.SelectedItem?.ToString() ?? "",
            Address     = _txtAddress.Text.Trim(),
            RegisteredOn = _patient?.RegisteredOn ?? DateTime.Now
        };

        if (_mode == FormMode.Add)
            await _svc.AddAsync(p);
        else
            await _svc.UpdateAsync(p);

        UIHelper.ShowSuccess(_mode == FormMode.Add ? "Patient added successfully." : "Patient updated successfully.");
        DialogResult = DialogResult.OK;
        Close();
    }
}
