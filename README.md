# 🎬 Movie Series Catalog

> An ASP.NET Core MVC web application for browsing a catalog of movies and series, managing actors and directors, and collecting user reviews.

![GitHub repo](https://img.shields.io/badge/CodingBurgas-2526--11V--SD--VBTodorov22-blue)
![.NET](https://img.shields.io/badge/.NET-8.0-purple)
![C#](https://img.shields.io/badge/C%23-67%25-green)
![HTML](https://img.shields.io/badge/HTML-32%25-orange)

---

## 📖 About

**Movie Series Catalog** is a school project built for CodingBurgas GitHub Classroom. It allows users to browse catalog entries for movies and series, view actor and director information, and leave reviews — with authentication controlling who can interact with the content.

---

## ✨ Features

- 📋 Browse a catalog of movies and series
- 🎭 View and manage actors and directors
- ⭐ Authenticated users can submit one review per catalog item
- 🔒 Once a review is submitted, the add-review action is hidden for that user
- 👤 User registration, login, and identity management via ASP.NET Identity

---

## 🛠️ Tech Stack

| Layer | Technology |
|---|---|
| Framework | ASP.NET Core 8.0 MVC |
| Language | C# |
| ORM | Entity Framework Core 8 |
| Database | SQLite |
| Auth | ASP.NET Core Identity |
| Frontend | Razor Views + HTML/CSS |

---

## 📁 Project Structure

```
MovieSeriesCatalog/
├── Areas/
│   └── Identity/
│       └── Pages/          # Identity UI pages (login, register, etc.)
├── Controllers/            # MVC Controllers
├── DTOs/                   # Data Transfer Objects
├── Data/                   # DbContext and migrations
├── Models/                 # Domain models
├── Services/               # Business logic / service layer
├── Views/                  # Razor view templates
├── wwwroot/                # Static files (CSS, JS, images)
├── Program.cs              # App entry point & DI configuration
├── appsettings.json        # Application configuration
└── MovieSeriesCatalog.csproj
```

---

## 🚀 Getting Started

### Prerequisites

- [.NET 8.0 SDK](https://dotnet.microsoft.com/download/dotnet/8.0)
- Git

### Installation

1. **Clone the repository**
   ```bash
   git clone https://github.com/codingburgas/2526-11V-SD-VBTodorov22.git
   cd 2526-11V-SD-VBTodorov22
   ```

2. **Restore dependencies**
   ```bash
   dotnet restore
   ```

3. **Apply database migrations**
   ```bash
   dotnet ef database update
   ```

4. **Run the application**
   ```bash
   dotnet run
   ```

5. Open your browser and navigate to `https://localhost:5001`

---

## 👤 Usage

- **Guest users** can browse the catalog, movies, series, actors, and directors.
- **Registered users** can log in and submit a review for any catalog item they haven't reviewed yet.
- **After submitting a review**, the review form is hidden — one review per user per item.

---

## 📦 NuGet Packages

| Package | Version |
|---|---|
| Microsoft.AspNetCore.Diagnostics.EntityFrameworkCore | 8.0.26 |
| Microsoft.AspNetCore.Identity.EntityFrameworkCore | 8.0.26 |
| Microsoft.AspNetCore.Identity.UI | 8.0.26 |
| Microsoft.EntityFrameworkCore.Sqlite | 8.0.26 |
| Microsoft.EntityFrameworkCore.Tools | 8.0.26 |

---

## 🎓 Author

**VBTodorov** — CodingBurgas, Class 11V, 2025–2026

---

## 📄 License

This project was created as part of a GitHub Classroom assignment for [CodingBurgas](https://github.com/codingburgas).
