# 🎬 Movie Series Catalog — Wiki

Welcome to the official wiki for **Movie Series Catalog**, an ASP.NET Core 8 MVC application built for CodingBurgas GitHub Classroom.

Use the sections below to navigate the documentation.

---

## 📚 Table of Contents

- [Project Overview](#project-overview)
- [Architecture](#architecture)
- [Folder Structure](#folder-structure)
- [Data Models](#data-models)
- [Controllers](#controllers)
- [Services](#services)
- [Authentication & Identity](#authentication--identity)
- [Views & Frontend](#views--frontend)
- [Database](#database)
- [Setup & Installation](#setup--installation)
- [Review System](#review-system)

---

## Project Overview

Movie Series Catalog is a web application that lets users:

- Browse a catalog of movies and TV series
- View details about actors and directors linked to each title
- Register an account and submit personal reviews
- See all existing reviews per catalog item

The application follows the standard **MVC (Model-View-Controller)** pattern using ASP.NET Core 8, with Entity Framework Core for data access and SQLite as the database.

---

## Architecture

The project uses the **ASP.NET Core MVC** architecture with a service layer:

```
Request → Controller → Service → DbContext (EF Core) → SQLite
                ↓
             View (Razor)
```

- **Controllers** handle HTTP requests and return views or redirect.
- **Services** contain the business logic and interact with the database via `DbContext`.
- **DTOs** are used to transfer data between layers without exposing domain models directly.
- **Models** represent the database entities.
- **Views** are Razor templates that render the HTML.

---

## Folder Structure

| Folder | Purpose |
|---|---|
| `Areas/Identity/Pages` | Auto-generated and customized Identity pages (login, register, manage account) |
| `Controllers/` | MVC controllers for each feature area |
| `DTOs/` | Data Transfer Objects for passing data between layers |
| `Data/` | EF Core `DbContext` and database migrations |
| `Models/` | Domain entity classes (Movie, Series, Actor, Director, Review, etc.) |
| `Services/` | Business logic services consumed by controllers |
| `Views/` | Razor `.cshtml` view templates |
| `wwwroot/` | Static assets — CSS, JavaScript, images |
| `Program.cs` | Application entry point, dependency injection setup, middleware pipeline |
| `appsettings.json` | App configuration (connection string, etc.) |

---

## Data Models

The core domain models used in the application:

**CatalogItem** — Represents a movie or series entry.
- Title, Description, Release Year, Type (Movie/Series)
- Related Actors, Directors, Reviews

**Actor** — A person who appears in catalog items.
- Name, Biography, Date of Birth

**Director** — A person who directed catalog items.
- Name, Biography

**Review** — A user-submitted review for a catalog item.
- Rating, Comment, Timestamp
- Linked to a `CatalogItem` and an `ApplicationUser`

**ApplicationUser** — Extends ASP.NET Identity's `IdentityUser`.

---

## Controllers

Each controller corresponds to a feature area:

| Controller | Responsibility |
|---|---|
| `HomeController` | Landing page and general navigation |
| `CatalogController` | CRUD for catalog entries (movies/series) |
| `ActorsController` | Actor listing and detail pages |
| `DirectorsController` | Director listing and detail pages |
| `ReviewsController` | Submitting and displaying reviews |

---

## Services

The service layer sits between controllers and the database:

| Service | Responsibility |
|---|---|
| `CatalogService` | Fetching, creating, and updating catalog items |
| `ActorService` | Actor data operations |
| `DirectorService` | Director data operations |
| `ReviewService` | Review submission logic including one-review-per-user enforcement |

Services are registered in `Program.cs` via dependency injection and injected into controllers via their constructors.

---

## Authentication & Identity

The app uses **ASP.NET Core Identity** with Entity Framework Core for user management.

- Identity pages live under `Areas/Identity/Pages/` (Razor Pages).
- The `ApplicationUser` class extends `IdentityUser` for any custom user fields.
- Routes like `/Identity/Account/Login` and `/Identity/Account/Register` are auto-scaffolded.
- Authorization is applied to review submission — only signed-in users can post reviews.

---

## Views & Frontend

Views are **Razor templates** (`.cshtml`) located in the `Views/` folder, organized by controller:

```
Views/
├── Home/
├── Catalog/
├── Actors/
├── Directors/
├── Reviews/
└── Shared/
    ├── _Layout.cshtml       ← Main layout template
    └── _ValidationScriptsPartial.cshtml
```

Static assets (CSS, JS, images) are in `wwwroot/`.

---

## Database

- **Engine:** SQLite (via `Microsoft.EntityFrameworkCore.Sqlite`)
- **ORM:** Entity Framework Core 8
- **Migrations:** Managed with `dotnet ef migrations`

To apply migrations and create/update the database:

```bash
dotnet ef database update
```

The connection string is configured in `appsettings.json`.

---

## Setup & Installation

### Requirements

- [.NET 8.0 SDK](https://dotnet.microsoft.com/download/dotnet/8.0)
- Git

### Steps

```bash
# 1. Clone the repo
git clone https://github.com/codingburgas/2526-11V-SD-VBTodorov22.git
cd 2526-11V-SD-VBTodorov22

# 2. Restore NuGet packages
dotnet restore

# 3. Apply EF Core migrations
dotnet ef database update

# 4. Run the app
dotnet run
```

Then open `https://localhost:5001` in your browser.

---

## Review System

The review system enforces a **one review per user per catalog item** rule:

1. A signed-in user navigates to a catalog item's detail page.
2. If they have not yet reviewed it, a "Add Review" form is displayed.
3. After submitting a review, the form is hidden — the user will no longer see the add action for that item.
4. All reviews for an item are visible to all visitors.

This logic is enforced in `ReviewService` and checked in the Reviews view using the current user's identity.

---

*Wiki maintained by VBTodorov — CodingBurgas Class 11V, 2025–2026*
