# MediFlow — OPD Management System

> Semester Project — Advanced Programming (COSC-5136) | Spring 2026  
> Department of Computer Science

---

## Group Members

| Name | Roll Number |
|---|---|
| Muhammad Qasim Akram | F23BDOCS1E02152 |
| Asad Farooq | F23BDOCS1E02138 |
| Fazal Mehmood | F23BDOCS1E02074 |

---

## About the Project

MediFlow is a desktop-based Hospital OPD (Outpatient Department) Management System built using C# and WinForms on .NET 10. It was developed as our semester project for the Advanced Programming course and covers the complete workflow of an OPD — from registering patients and managing doctors to booking appointments and tracking their status.

The project follows the 3-layer architecture taught in class, with a strict separation between the business logic layer (`MediFlow.Core`) and the user interface layer (`MediFlow.App`). All data access is done through raw ADO.NET with no ORM, and all database operations are fully asynchronous.

---

## Features

### Patient Management
- Add, edit, view, and delete patient records
- Tracks full name, gender, date of birth, phone, address, and blood group
- Calculates patient age automatically from date of birth
- Search by name, phone number, or patient ID
- Filter by gender and blood group

### Doctor Management
- Add, edit, view, and delete doctor records
- Tracks name, specialization, qualification, years of experience, phone, email, and availability
- Search by name, email, or doctor ID
- Filter by specialization and availability status
- Specialization dropdown populates dynamically from existing records

### Appointment Management
- Book new appointments between any patient and doctor
- Auto-generates a token number for each appointment
- Tracks appointment date/time, reason, status, and notes
- Status options: Scheduled, Completed, Cancelled
- Search by patient name, doctor name, or token number
- Filter by status and date

### Dashboard
- Live stat cards showing total patients, total doctors, total appointments, and today's OPD count
- Pie chart — appointments broken down by status (Scheduled / Completed / Cancelled)
- Bar chart — monthly appointment volume over the last 6 months
- All charts pull from real database data and update when records change

### General
- Mode-driven forms — one form per entity handles Add, Edit, and View modes via a `FormMode` enum
- Confirm-before-delete dialog on all records
- Field validation with MessageBox feedback on required fields
- BindingSource used for all DataGridView bindings
- Search and filter on all three entity views
- Status bar at the bottom showing current section, last action, and a live clock
- Fully async data loading throughout — no UI freezing

---

## Tech Stack

| Layer | Technology |
|---|---|
| Language | C# / .NET 10 |
| UI Framework | Windows Forms (WinForms) |
| Business Logic | Class Library — `MediFlow.Core` |
| Data Access | ADO.NET — `Microsoft.Data.SqlClient` |
| Charts | LiveCharts2 — `LiveChartsCore.SkiaSharpView.WinForms` |
| Database | SQL Server LocalDB `(localdb)\MSSQLLocalDB` |
| Configuration | `appsettings.json` + `Microsoft.Extensions.Configuration` |

---

## Project Structure

```
MediFlow/
│
├── MediFlow.Core/                  # Business logic — no UI dependency
│   ├── Models/
│   │   ├── Patient.cs              # Patient entity + computed Age property
│   │   ├── Doctor.cs               # Doctor entity
│   │   └── Appointment.cs          # Appointment entity + AppointmentStatus enum
│   │
│   ├── Interfaces/
│   │   ├── IPatientService.cs      # Contract for patient operations
│   │   ├── IDoctorService.cs       # Contract for doctor operations
│   │   └── IAppointmentService.cs  # Contract for appointment operations
│   │
│   └── Services/
│       ├── DbService.cs            # Base class — connection factory + ID generator
│       ├── PatientService.cs       # ADO.NET implementation of IPatientService
│       ├── DoctorService.cs        # ADO.NET implementation of IDoctorService
│       └── AppointmentService.cs   # ADO.NET implementation of IAppointmentService
│
└── MediFlow.App/                   # WinForms UI
    ├── Views/
    │   ├── DashboardView.cs        # Stat cards + LiveCharts2 pie and bar charts
    │   ├── PatientsView.cs         # Patient grid + search + filter + toolbar
    │   ├── DoctorsView.cs          # Doctor grid + search + filter + toolbar
    │   └── AppointmentsView.cs     # Appointment grid + search + filter + toolbar
    │
    ├── Forms/
    │   ├── PatientForm.cs          # Add / Edit / View patient
    │   ├── DoctorForm.cs           # Add / Edit / View doctor
    │   └── AppointmentForm.cs      # Add / Edit / View appointment
    │
    ├── Helpers/
    │   ├── Theme.cs                # Color palette and font definitions
    │   └── UIHelper.cs             # Reusable button, grid, card styling methods
    │
    ├── MainForm.cs                 # Shell — sidebar navigation + status bar + clock
    ├── ServiceLocator.cs           # Wires up services with connection string
    ├── FormMode.cs                 # Add / Edit / View enum
    └── appsettings.json            # Connection string
```

---

## Architecture

The project strictly follows the **3-Layer Architecture** pattern:

```
┌─────────────────────────────────┐
│        MediFlow.App (UI)        │  WinForms views and forms
│   PatientsView, DoctorsView,    │  Only talks to interfaces
│   AppointmentsView, Dashboard   │
└────────────────┬────────────────┘
                 │ depends on
┌────────────────▼────────────────┐
│    MediFlow.Core (Interfaces)   │  Contracts — IPatientService etc.
│    No knowledge of UI or DB     │  Pure abstraction layer
└────────────────┬────────────────┘
                 │ implemented by
┌────────────────▼────────────────┐
│    MediFlow.Core (Services)     │  Raw ADO.NET — SqlConnection,
│    PatientService, DoctorService │  SqlCommand, SqlDataReader
│    AppointmentService           │  All operations async
└─────────────────────────────────┘
                 │
┌────────────────▼────────────────┐
│     SQL Server LocalDB          │  3 tables: Patients, Doctors,
│     Database: MediFlow          │  Appointments
└─────────────────────────────────┘
```

**Key design decisions:**
- `App.Core` has zero knowledge of the UI — it can be reused with any frontend
- The UI depends on interfaces, not concrete services — follows Dependency Inversion
- NVARCHAR primary keys (GUID-derived) — no auto-increment INT IDs
- No FK constraints at database level per project requirements
- All database calls are async (`GetAllAsync`, `SearchAsync`, etc.)
- `using` blocks on every `SqlConnection` — no resource leaks

---

## Database

**Server:** `(localdb)\MSSQLLocalDB`  
**Database name:** `MediFlow`

| Table | Primary Key | Key Columns |
|---|---|---|
| Patients | PatientId (NVARCHAR) | FullName, Gender, DateOfBirth, Phone, Address, BloodGroup, RegisteredOn |
| Doctors | DoctorId (NVARCHAR) | FullName, Specialization, Qualification, YearsOfExperience, Phone, Email, IsAvailable |
| Appointments | AppointmentId (NVARCHAR) | PatientId, DoctorId, AppointmentDate, Reason, Status, Notes, TokenNumber |

The `Database_Setup.sql` script creates the database, all three tables, and inserts sample data for testing.

---

## Getting Started

**Prerequisites**
- Visual Studio 2022 or later
- .NET 10 SDK
- SQL Server LocalDB (comes with Visual Studio by default)

**Setup steps**

1. Clone the repository
   ```bash
   git clone https://github.com/Qasim-Akram/MediFlow.git
   ```

2. Open `MediFlow.sln` in Visual Studio

3. Set up the database
   - Go to **View → SQL Server Object Explorer**
   - Expand **SQL Server → (localdb)\MSSQLLocalDB**
   - Right-click → **New Query**
   - Open `Database_Setup.sql`, paste the contents, press **Ctrl+Shift+E**
   - Wait for "Command(s) completed successfully"

4. Set the startup project
   - In Solution Explorer, right-click **MediFlow.App** → **Set as Startup Project**

5. Build and run
   - Press **Ctrl+Shift+B** to build (NuGet packages restore automatically)
   - Press **F5** to run

---

## Course Information

- **Course:** Advanced Programming (COSC-5136)
- **Semester:** Spring 2026
- **Domain:** Hospital OPD Management