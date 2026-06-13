using MediFlow.App.Helpers;
using MediFlow.Core.Interfaces;
using MediFlow.Core.Models;

namespace MediFlow.App.Forms;

public class AppointmentForm : Form
{
    private readonly FormMode _mode;
    private readonly string? _id;
    private readonly IAppointmentService _svc    = ServiceLocator.AppointmentService;
    private readonly IPatientService     _patSvc = ServiceLocator.PatientService;
    private readonly IDoctorService      _docSvc = ServiceLocator.DoctorService;
    private Appointment? _appointment;

    private ComboBox _cmbPatient = null!, _cmbDoctor = null!, _cmbStatus = null!;
    private DateTimePicker _dtpDate = null!;
    private TextBox _txtReason = null!, _txtNotes = null!, _txtToken = null!;
    private Button _btnSave = null!, _btnCancel = null!;
    private List<Patient> _patients = new();
    private List<Doctor> _doctors = new();

    public AppointmentForm(FormMode mode, string? id = null)
    {
        _mode = mode; _id = id;
        InitializeComponent();
        _ = LoadDropdownsAsync();
    }

    private void InitializeComponent()
    {
        Text = _mode switch { FormMode.Add => "New Appointment", FormMode.Edit => "Edit Appointment", _ => "View Appointment" };
        Size = new Size(500, 560);
        StartPosition = FormStartPosition.CenterParent;
        FormBorderStyle = FormBorderStyle.FixedDialog;
        MaximizeBox = false; MinimizeBox = false;
        BackColor = Theme.Surface; Font = Theme.FontRegular;

        Controls.Add(new Label { Text = Text, Font = Theme.FontTitle, ForeColor = Theme.TextPrimary, Location = new Point(24, 20), AutoSize = true });

        int y = 70;
        _cmbPatient = AddComboField("Patient *", ref y);
        _cmbDoctor  = AddComboField("Doctor *",  ref y);
        _dtpDate    = AddDateTimePicker("Appointment Date & Time *", ref y);
        _txtToken   = AddTextField("Token Number", ref y);
        _txtReason  = AddTextField("Reason", ref y);
        _cmbStatus  = AddComboField("Status *", ref y);
        _cmbStatus.Items.AddRange(new[] { "Scheduled", "Completed", "Cancelled" });
        _cmbStatus.SelectedIndex = 0;
        _txtNotes   = AddTextField("Notes", ref y);

        _btnSave   = UIHelper.MakeButton(_mode == FormMode.Add ? "Save" : "Update", Theme.Primary, 110, 38);
        _btnCancel = UIHelper.MakeButton("Cancel", Theme.TextSecondary, 100, 38);
        _btnSave.Location   = new Point(260, y + 10);
        _btnCancel.Location = new Point(378, y + 10);
        _btnSave.Click   += async (s, e) => await SaveAsync();
        _btnCancel.Click += (s, e) => Close();
        Controls.Add(_btnSave); Controls.Add(_btnCancel);
        if (_mode == FormMode.View) _btnSave.Visible = false;
    }

    private ComboBox AddComboField(string label, ref int y)
    {
        Controls.Add(new Label { Text = label, Location = new Point(24, y), AutoSize = true, Font = Theme.FontSmall, ForeColor = Theme.TextSecondary });
        var cb = new ComboBox { Location = new Point(24, y + 18), Width = 430, Font = Theme.FontRegular, DropDownStyle = ComboBoxStyle.DropDownList };
        Controls.Add(cb); y += 58; return cb;
    }

    private TextBox AddTextField(string label, ref int y)
    {
        Controls.Add(new Label { Text = label, Location = new Point(24, y), AutoSize = true, Font = Theme.FontSmall, ForeColor = Theme.TextSecondary });
        var tb = new TextBox { Location = new Point(24, y + 18), Width = 430, Font = Theme.FontRegular, BorderStyle = BorderStyle.FixedSingle };
        Controls.Add(tb); y += 58; return tb;
    }

    private DateTimePicker AddDateTimePicker(string label, ref int y)
    {
        Controls.Add(new Label { Text = label, Location = new Point(24, y), AutoSize = true, Font = Theme.FontSmall, ForeColor = Theme.TextSecondary });
        var dtp = new DateTimePicker { Location = new Point(24, y + 18), Width = 430, Font = Theme.FontRegular, Format = DateTimePickerFormat.Custom, CustomFormat = "dd/MM/yyyy  hh:mm tt", Value = DateTime.Now };
        Controls.Add(dtp); y += 58; return dtp;
    }

    private async Task LoadDropdownsAsync()
    {
        _patients = await _patSvc.GetAllAsync();
        _doctors  = await _docSvc.GetAllAsync();
        _cmbPatient.Items.Clear();
        _cmbDoctor.Items.Clear();
        foreach (var p in _patients) _cmbPatient.Items.Add(p.FullName);
        foreach (var d in _doctors)  _cmbDoctor.Items.Add($"{d.FullName} — {d.Specialization}");
        if (_patients.Count > 0) _cmbPatient.SelectedIndex = 0;
        if (_doctors.Count > 0)  _cmbDoctor.SelectedIndex  = 0;

        if (_id != null)
        {
            _appointment = await _svc.GetByIdAsync(_id);
            if (_appointment == null) return;
            var pi = _patients.FindIndex(p => p.PatientId == _appointment.PatientId);
            var di = _doctors.FindIndex(d => d.DoctorId == _appointment.DoctorId);
            if (pi >= 0) _cmbPatient.SelectedIndex = pi;
            if (di >= 0) _cmbDoctor.SelectedIndex  = di;
            _dtpDate.Value    = _appointment.AppointmentDate;
            _txtToken.Text    = _appointment.TokenNumber;
            _txtReason.Text   = _appointment.Reason;
            _cmbStatus.SelectedItem = _appointment.Status.ToString();
            _txtNotes.Text    = _appointment.Notes;
            if (_mode == FormMode.View)
                foreach (Control c in Controls)
                    if (c is TextBox tb) tb.ReadOnly = true;
        }
    }

    private async Task SaveAsync()
    {
        if (_cmbPatient.SelectedIndex < 0) { UIHelper.ShowError("Select a patient."); return; }
        if (_cmbDoctor.SelectedIndex < 0)  { UIHelper.ShowError("Select a doctor."); return; }

        var token = string.IsNullOrWhiteSpace(_txtToken.Text)
            ? $"TKN-{DateTime.Now:HHmmss}"
            : _txtToken.Text.Trim();

        var a = new Appointment
        {
            AppointmentId   = _appointment?.AppointmentId ?? "",
            PatientId       = _patients[_cmbPatient.SelectedIndex].PatientId,
            DoctorId        = _doctors[_cmbDoctor.SelectedIndex].DoctorId,
            AppointmentDate = _dtpDate.Value,
            TokenNumber     = token,
            Reason          = _txtReason.Text.Trim(),
            Status          = Enum.Parse<AppointmentStatus>(_cmbStatus.SelectedItem!.ToString()!),
            Notes           = _txtNotes.Text.Trim()
        };

        if (_mode == FormMode.Add) await _svc.AddAsync(a);
        else await _svc.UpdateAsync(a);

        UIHelper.ShowSuccess(_mode == FormMode.Add ? "Appointment booked." : "Appointment updated.");
        DialogResult = DialogResult.OK;
        Close();
    }
}
