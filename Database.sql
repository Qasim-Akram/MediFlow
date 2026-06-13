IF NOT EXISTS (SELECT name FROM sys.databases WHERE name = 'MediFlow')
    CREATE DATABASE MediFlow;
GO

USE MediFlow;
GO

IF OBJECT_ID('Appointments', 'U') IS NOT NULL DROP TABLE Appointments;
IF OBJECT_ID('Patients',     'U') IS NOT NULL DROP TABLE Patients;
IF OBJECT_ID('Doctors',      'U') IS NOT NULL DROP TABLE Doctors;
GO

CREATE TABLE Patients (
    PatientId    NVARCHAR(20)  NOT NULL PRIMARY KEY,
    FullName     NVARCHAR(150) NOT NULL,
    Gender       NVARCHAR(10)  NOT NULL,
    DateOfBirth  DATE          NOT NULL,
    Phone        NVARCHAR(20)  NULL,
    Address      NVARCHAR(300) NULL,
    BloodGroup   NVARCHAR(5)   NULL,
    RegisteredOn DATETIME      NOT NULL DEFAULT GETDATE()
);

CREATE TABLE Doctors (
    DoctorId          NVARCHAR(20)  NOT NULL PRIMARY KEY,
    FullName          NVARCHAR(150) NOT NULL,
    Specialization    NVARCHAR(100) NOT NULL,
    Phone             NVARCHAR(20)  NULL,
    Email             NVARCHAR(150) NULL,
    IsAvailable       BIT           NOT NULL DEFAULT 1,
    YearsOfExperience INT           NOT NULL DEFAULT 0,
    Qualification     NVARCHAR(150) NULL
);

CREATE TABLE Appointments (
    AppointmentId   NVARCHAR(20)  NOT NULL PRIMARY KEY,
    PatientId       NVARCHAR(20)  NOT NULL,
    DoctorId        NVARCHAR(20)  NOT NULL,
    AppointmentDate DATETIME      NOT NULL,
    Reason          NVARCHAR(300) NULL,
    Status          NVARCHAR(20)  NOT NULL DEFAULT 'Scheduled',
    Notes           NVARCHAR(500) NULL,
    TokenNumber     NVARCHAR(20)  NOT NULL
);
GO

INSERT INTO Patients VALUES
('PAT001A2B3C', 'Ahmed Raza',   'Male',   '1990-03-15', '0300-1234567', 'House 12, Block B, Bahawalpur', 'B+',  GETDATE()),
('PAT002D4E5F', 'Sana Malik',   'Female', '1985-07-22', '0321-9876543', 'Street 5, Model Town, Lahore',  'A+',  GETDATE()),
('PAT003G6H7I', 'Usman Khan',   'Male',   '1978-11-05', '0333-4567890', 'Flat 3, City Centre, Karachi',  'O+',  GETDATE()),
('PAT004J8K9L', 'Ayesha Bibi',  'Female', '1995-02-10', '0311-1122334', 'Street 9, Gulberg III, Lahore', 'AB+', GETDATE()),
('PAT005M0N1O', 'Bilal Hussain','Male',   '2000-06-25', '0345-9988776', 'House 5, F-7, Islamabad',       'A-',  GETDATE());

INSERT INTO Doctors VALUES
('DOC001P2Q3R', 'Dr. Tariq Mahmood',   'Cardiology',   '0300-1111111', 'tariq@mediflow.com',  1, 15, 'MBBS, FCPS'),
('DOC002S4T5U', 'Dr. Ayesha Siddiqui', 'Neurology',    '0321-2222222', 'ayesha@mediflow.com', 1, 10, 'MBBS, MRCP'),
('DOC003V6W7X', 'Dr. Bilal Ahmed',     'Orthopedics',  '0333-3333333', 'bilal@mediflow.com',  0, 8,  'MBBS, MS Ortho'),
('DOC004Y8Z9A', 'Dr. Sara Qureshi',    'Dermatology',  '0312-4444444', 'sara@mediflow.com',   1, 5,  'MBBS, DDVL'),
('DOC005B0C1D', 'Dr. Hamza Farooq',    'Pediatrics',   '0322-5555555', 'hamza@mediflow.com',  1, 12, 'MBBS, DCH');

INSERT INTO Appointments VALUES
('APT001E2F3G', 'PAT001A2B3C', 'DOC001P2Q3R', GETDATE(),                        'Chest pain checkup',      'Scheduled', 'First visit',          'TKN-001'),
('APT002H4I5J', 'PAT002D4E5F', 'DOC002S4T5U', DATEADD(DAY, -2,  GETDATE()),     'Headache and dizziness',  'Completed', 'Prescribed medication', 'TKN-002'),
('APT003K6L7M', 'PAT003G6H7I', 'DOC003V6W7X', DATEADD(DAY, -5,  GETDATE()),     'Knee pain',               'Cancelled', 'Patient rescheduled',   'TKN-003'),
('APT004N8O9P', 'PAT004J8K9L', 'DOC004Y8Z9A', DATEADD(DAY, 1,   GETDATE()),     'Skin rash',               'Scheduled', NULL,                    'TKN-004'),
('APT005Q0R1S', 'PAT005M0N1O', 'DOC005B0C1D', DATEADD(MONTH, -1, GETDATE()),    'Fever and cough',         'Completed', 'Fully recovered',        'TKN-005'),
('APT006T2U3V', 'PAT001A2B3C', 'DOC001P2Q3R', DATEADD(MONTH, -2, GETDATE()),    'Follow-up checkup',       'Completed', 'Stable condition',       'TKN-006'),
('APT007W4X5Y', 'PAT002D4E5F', 'DOC002S4T5U', DATEADD(MONTH, -3, GETDATE()),    'MRI discussion',          'Completed', 'Results reviewed',       'TKN-007');
GO
