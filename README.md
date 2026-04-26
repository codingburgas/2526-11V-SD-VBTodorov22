# Film Streaming Platform

Film Streaming Platform is a catalog and administration solution for managing films, people, reviews, and cover artwork. This repository contains a working ASP.NET Core MVC application for browsing and administration, plus a reference C++ REST backend design for teams that want to implement the service layer with Crow or Pistache.

## Project Overview

The platform is designed around three responsibilities:

- Public catalog browsing for films, metadata, ratings, and reviews
- Role-based administration for catalog maintenance and cover image management
- A REST-oriented backend contract that can be implemented in C++ and backed by MySQL with filesystem-based media storage

The latest update adds admin-only cover upload support. Administrators can now upload or replace a film poster, store it on the local filesystem, and expose the stored URL through the film API and the UI.

## Features

- Browse films and series by type, genre, and release year
- View detailed film pages with cast, crew, ratings, reviews, and cover artwork
- Manage actors, directors, films, reviews, and statistics from the MVC application
- Upload and replace film cover images from the admin edit screen
- Admin-only cover upload API with type and size validation
- Local filesystem storage for uploaded covers in `wwwroot/uploads/covers/`
- Seeded identity roles for admin and standard users
- MySQL schema reference for a C++ backend deployment
- GitHub wiki pages ready to publish from the `wiki/` directory

## Tech Stack

- Backend reference architecture: C++ REST API using Crow or Pistache style routing
- Current repository application: ASP.NET Core MVC (.NET 8, C#, Razor Views)
- Authentication and authorization: ASP.NET Core Identity with role-based admin access
- Data access: Entity Framework Core with SQLite in the local MVC app
- Database reference for service deployments: MySQL 8
- Storage: local filesystem for cover images
- Frontend: Razor Views, Bootstrap 5, vanilla JavaScript for async cover upload

## Installation

### Prerequisites

- .NET 8 SDK
- Git
- Optional: MySQL 8 if you want to apply the reference SQL schema
- Optional: a C++17 or newer toolchain if you want to adapt the reference backend in `docs/cpp-backend/`

### Run the MVC application

```bash
git clone <repository-url>
cd project-VBTodorov22
dotnet restore
dotnet run
```

The application seeds demo accounts on first start:

- Admin: `admin@movieseriescatalog.local` / `Admin123!`
- User: `user@movieseriescatalog.local` / `User123!`

### Reference backend assets

- MySQL schema: [docs/sql/film_streaming_platform_mysql.sql](docs/sql/film_streaming_platform_mysql.sql)
- C++ backend reference: [docs/cpp-backend/README.md](docs/cpp-backend/README.md)
- GitHub wiki pages: [wiki/Home.md](wiki/Home.md)

## Usage

### Catalog Browsing

1. Start the application with `dotnet run`.
2. Open the site in a browser.
3. Browse the catalog from `Movies`, apply filters, and open film details.

### Admin Cover Upload

1. Sign in with the seeded admin account.
2. Open `Movies`, then create a new catalog item or edit an existing one.
3. In the `Cover Image` section of the edit page, select a `.jpg`, `.jpeg`, or `.png` file.
4. Click `Upload Cover`.
5. The file is stored under `wwwroot/uploads/covers/`, the film record is updated, and the new image appears on the film pages.

### REST API

- `GET /films/{id}` returns film data including `coverUrl`
- `POST /admin/films/{id}/cover` accepts multipart form data with a `file` field and updates the cover image for that film

## Project Structure

```text
.
|-- Controllers/             # MVC controllers and REST endpoints
|-- Data/                    # EF Core context, migrations, seeding, roles
|-- DTOs/                    # View models and API contracts
|-- Middleware/              # API authorization middleware
|-- Models/                  # Domain models
|-- Options/                 # Cover upload configuration
|-- Services/                # Business logic, repositories, storage services
|-- Views/                   # Razor UI
|-- wwwroot/                 # Static assets and uploaded cover images
|-- docs/
|   |-- cpp-backend/         # Reference C++ REST implementation
|   |-- examples/            # Example client-side upload logic
|   `-- sql/                 # MySQL schema
`-- wiki/                    # GitHub wiki markdown pages
```

## Contributing

1. Create a feature branch from `main`.
2. Keep changes scoped and documented.
3. Run the application and verify the affected workflows.
4. Update the wiki or README when you change setup, API behavior, or schema.
5. Open a pull request with a short summary, verification notes, and screenshots for UI changes when relevant.

## Wiki

The repository includes wiki-ready pages:

- [Home](wiki/Home.md)
- [Installation](wiki/Installation.md)
- [API](wiki/API.md)
- [Database](wiki/Database.md)
- [Admin Guide](wiki/Admin-Guide.md)
- [User Guide](wiki/User-Guide.md)
