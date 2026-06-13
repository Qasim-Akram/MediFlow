# MediFlow — OPD Management System

A desktop application for managing hospital outpatient department operations built as a semester project for Advanced Programming (COSC-5136).

---

## What it does

- Register and manage patients with blood group tracking
- Manage doctors with specialization, qualifications, and availability
- Book, edit, and cancel appointments with auto-generated token numbers
- Dashboard with live stat cards and two charts (appointment status breakdown + monthly trends)
- Search and filter on every view
- Status bar showing current section and live clock

---

## Tech stack

| Layer | Technology |
|---|---|
| Language | C# / .NET 10 |
| UI | WinForms (`MediFlow.App`) |
| Business Logic | Class Library (`MediFlow.Core`) |
| Database | SQL Server LocalDB |
| Data Access | ADO.NET — `Microsoft.Data.SqlClient` |
| Charts | LiveCharts2 — `LiveChartsCore.SkiaSharpView.WinForms` |
| Config | `appsettings.json` + `Microsoft.Extensions.Configuration` |

---

## Project structure

```
MediFlow/
├── MediFlow.Core/
│   ├── Models/          Patient, Doctor, Appointment, AppointmentStatus
│   ├── Interfaces/      IPatientService, IDoctorService, IAppointmentService
│   └── Services/        PatientService, DoctorService, AppointmentService, DbService
│
└── MediFlow.App/
    ├── Views/           DashboardView, PatientsView, DoctorsView, AppointmentsView
    ├── Forms/           PatientForm, DoctorForm, AppointmentForm
    ├── Helpers/         Theme, UIHelper
    ├── MainForm.cs      Sidebar shell + status bar
    ├── ServiceLocator.cs
    ├── FormMode.cs      Add / Edit / View enum
    └── appsettings.json
```

---

## Getting started

**Prerequisites**
- Visual Studio 2022 or later
- .NET 10 SDK
- SQL Server LocalDB (included with Visual Studio)

**Setup**

1. Clone the repo
   ```bash
   git clone https://github.com/Qasim-Akram/MediFlow.git
   ```

2. Open `MediFlow.sln` in Visual Studio

3. Run `Database_Setup.sql` against `(localdb)\MSSQLLocalDB`
   - View → SQL Server Object Explorer
   - Expand SQL Server → (localdb)\MSSQLLocalDB → right-click → New Query
   - Paste the script and press Ctrl+Shift+E

4. Right-click `MediFlow.App` → Set as Startup Project

5. Press F5

NuGet packages restore automatically on first build.

---

## Architecture

The project follows a 3-layer architecture:

- **Models** define domain entities (Patient, Doctor, Appointment)
- **Interfaces** act as contracts between the UI and data access layers
- **Services** implement those contracts using raw ADO.NET — no ORM
- **Views** are WinForms UserControls swapped into MainForm's content panel
- **Forms** are mode-driven (one form handles Add, Edit, and View via a `FormMode` enum)

All database operations are async (`GetAllAsync`, `SearchAsync`, etc.) and all connections use `using` blocks.

---

## Database

- **Server:** `(localdb)\MSSQLLocalDB`
- **Database:** `MediFlow`
- **Tables:** `Patients`, `Doctors`, `Appointments`
- Primary keys are NVARCHAR-based (GUID-derived), no foreign key constraints at DB level