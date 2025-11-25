# 🎓 MiniLMS - Learning Management System

A full-stack Learning Management System built with **ASP.NET Core 8**, **React**, and **SQL Server**. Features JWT authentication, role-based access control, file uploads, and real-time progress tracking.

![Build Status](https://img.shields.io/badge/build-passing-brightgreen)
![.NET](https://img.shields.io/badge/.NET-8.0-blue)
![React](https://img.shields.io/badge/React-18-61dafb)

---

## ✨ Features

### 🔐 **Authentication & Authorization**
- JWT-based authentication
- Role-based access control (Admin, Teacher, Student)
- BCrypt password hashing
- Protected API endpoints

### 👥 **User Management**
- **Admin**: Full system control
- **Teacher**: Create courses, upload content, grade assignments
- **Student**: Enroll in courses, submit assignments, track progress

### 📚 **Course Management**
- Create and manage courses
- Assign teachers to courses
- Student enrollment system
- Many-to-many relationships

### 📂 **Content Delivery**
- Teachers upload videos and documents
- File storage with unique naming
- Students access course materials
- Enrollment verification for content access

### 📝 **Assignments & Grading**
- Teachers create assignments with due dates
- Students submit files (PDFs, documents)
- Teachers grade submissions (0-100 scale)
- Feedback system

### 📊 **Progress Tracking**
- Mark lessons as complete
- Visual progress bars
- Real-time percentage calculation
- Per-course progress tracking

---

## 🛠️ Tech Stack

### Backend
- **ASP.NET Core 8** - Web API
- **Entity Framework Core** - ORM
- **SQL Server** - Database
- **JWT** - Authentication
- **BCrypt** - Password hashing

### Frontend
- **React 18** - UI Library
- **Vite** - Build tool
- **CSS3** - Styling

### DevOps
- **Docker** - Containerization
- **Docker Compose** - Multi-container orchestration

---

## 🚀 Getting Started

### Prerequisites
- [.NET 8 SDK](https://dotnet.microsoft.com/download)
- [Node.js 18+](https://nodejs.org/)
- [SQL Server](https://www.microsoft.com/sql-server) or Docker

### Installation

1. **Clone the repository**
   ```bash
   git clone https://github.com/YOUR_USERNAME/MiniLMS.git
   cd MiniLMS
   ```

2. **Backend Setup**
   ```bash
   # Restore dependencies
   dotnet restore

   # Update appsettings.json with your connection string
   # Navigate to DemoMvc/appsettings.json and update:
   # "DefaultConnection": "YOUR_CONNECTION_STRING"

   # Run migrations
   dotnet ef database update --project DemoMvc.Infrastructure --startup-project DemoMvc

   # Run the API
   dotnet run --project DemoMvc
   ```
   API will be available at `https://localhost:7015`

3. **Frontend Setup**
   ```bash
   cd clientapp
   npm install
   npm run dev
   ```
   Frontend will be available at `http://localhost:5173`

### Docker Setup (Alternative)

```bash
docker-compose up --build
```

---

## 📖 Usage

### First-Time Setup
1. Register an **Admin** user via Swagger (`https://localhost:7015/swagger`)
2. Login to get your JWT token
3. Use the React UI to manage the system

### User Roles

| Role | Capabilities |
|------|-------------|
| **Admin** | Create/delete teachers, students, courses. Full access. |
| **Teacher** | Create courses, upload content, create assignments, grade submissions. |
| **Student** | Enroll in courses, view content, submit assignments, track progress. |

---

## 🏗️ Architecture

```
MiniLMS/
├── DemoMvc.Core/           # Domain entities and interfaces
├── DemoMvc.Infrastructure/ # Data access and repositories
├── DemoMvc/                # Web API and controllers
└── clientapp/              # React frontend
```

**Design Pattern**: Clean Architecture with Repository Pattern

---

## 🔒 Security

- Passwords are hashed using **BCrypt**
- JWT tokens expire after 60 minutes
- Role-based authorization on all endpoints
- CORS configured for frontend origin
- Secrets managed via `appsettings.json` (use environment variables in production)

---

## 📸 Screenshots

> Add screenshots of your application here

---

## 🤝 Contributing

Contributions are welcome! Please open an issue or submit a pull request.

---

## 📄 License

This project is licensed under the MIT License.

---

## 👨‍💻 Author

**Sandesh Kandel**  
[GitHub](https://github.com/007sandesh) | [LinkedIn](https://www.linkedin.com/in/sandesh-kandel-7b6a04397/)

---

## 🙏 Acknowledgments

- Built as a portfolio project to demonstrate full-stack development skills
- Inspired by modern LMS platforms like Moodle and Canvas
