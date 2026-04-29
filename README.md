# 🚀 Virtual Classroom Management System

A cloud-based web application developed using **ASP.NET Core MVC** and deployed on **Microsoft Azure**.  
This system helps faculty and students manage online classrooms, assignments, and learning activities efficiently.

---

## 📌 Features

### 🔐 Authentication & Authorization
- Secure login using Email/Password
- Google Sign-In integration
- Password reset using OTP (email)
- Role-based access control (Faculty & Student)

### 👨‍🏫 Faculty Module
- Create and manage classrooms
- Invite students via email
- Upload assignments and study materials
- View student submissions

### 🎓 Student Module
- Join classrooms using invitations
- Access study materials
- Submit assignments
- Track deadlines and status

### 📩 Notification System
- Email notifications for OTP, invitations, and updates

---

## ☁️ Cloud Services Used

| Service | Type | Purpose |
|--------|------|--------|
| Azure App Service | PaaS | Hosts the web application |
| Azure SQL Database | PaaS | Stores application data |
| Azure Blob Storage | PaaS | Stores files (assignments, materials) |

---

## 🔗 External APIs

- Google Identity API (Google Sign-In)
- Twilio SendGrid API (Email Service)

---

## 🏗️ Project Structure & Folder Explanation

### 🔹 1. VirtualClassroom.Core
This layer contains **core business entities (database tables)**.

**Used for:**
- Defining database models
- Representing system data

**Key Files:**
- `TblUsers.cs` → User details
- `TblClassroom.cs` → Classroom data
- `TblAssignments.cs` → Assignment details
- `TblSubmissions.cs` → Student submissions
- `TblMaterials.cs` → Study materials
- `TblClassroomInvites.cs` → Invitations
- `TblNotifications.cs` → Notifications

---

### 🔹 2. VirtualClassroom.Infrastructure
This layer handles **database and backend services**.

**Used for:**
- Database connection (Entity Framework Core)
- Migrations and database updates
- Service implementation

**Key Files:**
- `ApplicationDbContext.cs` → Main database context
- `ApplicationDbContextFactory.cs` → Design-time DB context
- `Migrations/` → Database schema changes
- `Services/` → Business logic services

---

### 🔹 3. VirtualClassroom.Web
This is the **main application layer (UI + Controllers)**.

---

#### 📁 Controllers
Handles user requests and application logic.

- **Faculty/**
  - `ClassroomController.cs` → Manage classrooms
  - `AssignmentFacultyController.cs` → Manage assignments
  - `MaterialController.cs` → Manage materials

- **Student/**
  - `StudentController.cs` → Student dashboard
  - `AssignmentController.cs` → View assignments
  - `SubmissionController.cs` → Submit assignments
  - `MaterialController.cs` → View materials

- `AccountController.cs` → Login, Register, Authentication
- `HomeController.cs` → Landing pages

---

#### 📁 Views
Contains Razor UI pages.

- Account → Login, Register, Reset Password
- AssignmentFaculty → Faculty assignment pages
- Student → Student dashboard & actions

---

#### 📁 Services
Used for external integrations.

- `BlobService.cs` → Upload files to Azure Blob Storage
- `BlobSubmissionService.cs` → Handle assignment submissions

---

#### 📁 Middleware
Custom middleware for request handling.

- `SessionMiddleware.cs` → Manage user sessions

---

#### 📁 Filters
Custom authorization logic.

- `RoleAuthorizeAttribute.cs` → Role-based access control

---

#### 📁 wwwroot
Contains static files (frontend UI).

- CSS files for styling pages:
  - `auth.css`
  - `classroom.css`
  - `assignment.css`
  - `faculty-dashboard.css`

---

## ⚙️ Technologies Used

- ASP.NET Core MVC
- Entity Framework Core
- SQL Server (Azure SQL)
- Azure App Service
- Azure Blob Storage
- SendGrid API
- Google OAuth
- HTML, CSS, Bootstrap

---

## 🔄 System Workflow

1. User opens the web application
2. User logs in using Email or Google
3. System verifies authentication
4. User is redirected based on role (Faculty / Student)
5. Faculty creates classroom and invites students
6. Students join classroom
7. Faculty uploads assignments/materials
8. Students submit assignments
9. Files stored in Azure Blob Storage
10. Data stored in Azure SQL Database
11. Notifications sent via email

---

## 🚀 Deployment

The application is deployed using Azure App Service.

**Steps:**
1. Right-click project → Publish
2. Select Azure App Service
3. Create new instance
4. Deploy application

---

## 📚 Learning Outcomes

- Cloud service integration (Azure)
- Full-stack web development
- Role-based authentication
- File storage using Blob Storage
- Real-world project architecture

---

## 🤝 Contributors

- Shruti Hansaliya  
- Jailly Maniya  
- Hetvi Vamja  

---

## ⭐ Feedback

If you find this project useful, feel free to ⭐ the repository.
