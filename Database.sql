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
('PAT002D4E5F', 'Sana Malik',   'Female', '1985-07-22', '0321-9876543', 'Street 5, Model Town, Lahore',  'A+',  GETDATE());


INSERT INTO Doctors VALUES
('DOC001P2Q3R', 'Dr. Tariq Mahmood',   'Cardiology',   '0300-1111111', 'tariq@mediflow.com',  1, 15, 'MBBS, FCPS'),
('DOC002S4T5U', 'Dr. Ayesha Siddiqui', 'Neurology',    '0321-2222222', 'ayesha@mediflow.com', 1, 10, 'MBBS, MRCP');

INSERT INTO Appointments VALUES
('APT001E2F3G', 'PAT001A2B3C', 'DOC001P2Q3R', GETDATE(),                        'Chest pain checkup',      'Scheduled', 'First visit',          'TKN-001'),
('APT002H4I5J', 'PAT002D4E5F', 'DOC002S4T5U', DATEADD(DAY, -2,  GETDATE()),     'Headache and dizziness',  'Completed', 'Prescribed medication', 'TKN-002');


GO
