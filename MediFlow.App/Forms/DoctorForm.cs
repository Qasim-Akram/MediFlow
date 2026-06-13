using MediFlow.App.Helpers;
using MediFlow.Core.Interfaces;
using MediFlow.Core.Models;

namespace MediFlow.App.Forms;

public class DoctorForm : Form
{
    private readonly FormMode _mode;
    private readonly string? _id;
    private readonly IDoctorService _svc = ServiceLocator.DoctorService;
    private Doctor? _doctor;

    private TextBox _txtName = null!, _txtSpec = null!, _txtPhone = null!, _txtEmail = null!, _txtQual = null!;
    private NumericUpDown _nudYoe = null!;
    private CheckBox _chkAvail = null!;
    private Button _btnSave = null!, _btnCancel = null!;

    public DoctorForm(FormMode mode, string? id = null)
    {
        _mode = mode; _id = id;
        InitializeComponent();
        _ = LoadAsync();
    }

    private void InitializeComponent()
    {
        Text = _mode switch { FormMode.Add => "Add Doctor", FormMode.Edit => "Edit Doctor", _ => "View Doctor" };
        Size = new Size(480, 530);
        StartPosition = FormStartPosition.CenterParent;
        FormBorderStyle = FormBorderStyle.FixedDialog;
        MaximizeBox = false; MinimizeBox = false;
        BackColor = Theme.Surface; Font = Theme.FontRegular;

        var title = new Label { Text = Text, Font = Theme.FontTitle, ForeColor = Theme.TextPrimary, Location = new Point(24, 20), AutoSize = true };

        int y = 70;
        _txtName  = AddField("Full Name *",       ref y);
        _txtSpec  = AddField("Specialization *",  ref y);
        _txtQual  = AddField("Qualification",     ref y);
        _txtPhone = AddField("Phone",             ref y);
        _txtEmail = AddField("Email",             ref y);

        Controls.Add(new Label { Text = "Years of Experience", Location = new Point(24, y), AutoSize = true, Font = Theme.FontSmall, ForeColor = Theme.TextSecondary });
        _nudYoe = new NumericUpDown { Location = new Point(24, y + 18), Width = 200, Font = Theme.FontRegular, Minimum = 0, Maximum = 60 };
        Controls.Add(_nudYoe);

        _chkAvail = new CheckBox { Text = "Available", Location = new Point(250, y + 20), Font = Theme.FontRegular, ForeColor = Theme.TextPrimary, Checked = true, AutoSize = true };
        Controls.Add(_chkAvail);
        y += 58;

        _btnSave   = UIHelper.MakeButton(_mode == FormMode.Add ? "Save" : "Update", Theme.Primary, 110, 38);
        _btnCancel = UIHelper.MakeButton("Cancel", Theme.TextSecondary, 100, 38);
        _btnSave.Location   = new Point(240, y + 10);
        _btnCancel.Location = new Point(358, y + 10);
        _btnSave.Click   += async (s, e) => await SaveAsync();
        _btnCancel.Click += (s, e) => Close();

        Controls.Add(title); Controls.Add(_btnSave); Controls.Add(_btnCancel);
        if (_mode == FormMode.View) _btnSave.Visible = false;
    }

    private TextBox AddField(string label, ref int y)
    {
        Controls.Add(new Label { Text = label, Location = new Point(24, y), AutoSize = true, Font = Theme.FontSmall, ForeColor = Theme.TextSecondary });
        var tb = new TextBox { Location = new Point(24, y + 18), Width = 410, Font = Theme.FontRegular, BorderStyle = BorderStyle.FixedSingle };
        Controls.Add(tb); y += 58; return tb;
    }

    private async Task LoadAsync()
    {
        if (_id == null) return;
        _doctor = await _svc.GetByIdAsync(_id);
        if (_doctor == null) return;
        _txtName.Text = _doctor.FullName;
        _txtSpec.Text = _doctor.Specialization;
        _txtQual.Text = _doctor.Qualification;
        _txtPhone.Text = _doctor.Phone;
        _txtEmail.Text = _doctor.Email;
        _nudYoe.Value = _doctor.YearsOfExperience;
        _chkAvail.Checked = _doctor.IsAvailable;
        if (_mode == FormMode.View)
            foreach (Control c in Controls)
            { if (c is TextBox tb) tb.ReadOnly = true; if (c is NumericUpDown n) n.ReadOnly = true; }
    }

    private async Task SaveAsync()
    {
        if (string.IsNullOrWhiteSpace(_txtName.Text)) { UIHelper.ShowError("Full Name is required."); return; }
        if (string.IsNullOrWhiteSpace(_txtSpec.Text)) { UIHelper.ShowError("Specialization is required."); return; }

        var d = new Doctor
        {
            DoctorId          = _doctor?.DoctorId ?? "",
            FullName          = _txtName.Text.Trim(),
            Specialization    = _txtSpec.Text.Trim(),
            Qualification     = _txtQual.Text.Trim(),
            Phone             = _txtPhone.Text.Trim(),
            Email             = _txtEmail.Text.Trim(),
            YearsOfExperience = (int)_nudYoe.Value,
            IsAvailable       = _chkAvail.Checked
        };

        if (_mode == FormMode.Add) await _svc.AddAsync(d);
        else await _svc.UpdateAsync(d);

        UIHelper.ShowSuccess(_mode == FormMode.Add ? "Doctor added." : "Doctor updated.");
        DialogResult = DialogResult.OK;
        Close();
    }
}
