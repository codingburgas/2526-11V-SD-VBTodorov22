# C++ Backend Reference

This directory contains a reference implementation for the film cover upload workflow in a C++ service. The code is intentionally framework-agnostic in the core services so it can be adapted to Crow, Pistache, or a similar REST framework.

## Included Components

- `include/HttpTypes.hpp`: lightweight request, response, and multipart file contracts
- `include/AdminAuthMiddleware.hpp`: placeholder admin authorization middleware
- `include/CoverStorageService.hpp` and `src/CoverStorageService.cpp`: validation and filesystem storage
- `include/FilmRepository.hpp` and `src/FilmRepository.cpp`: MySQL-oriented repository contract
- `src/FilmApiController.cpp`: handlers for `GET /films/{id}` and `POST /admin/films/{id}/cover`

## Covered API Endpoints

- `GET /films/{id}`
- `POST /admin/films/{id}/cover`

## Validation Rules

- Max size: 5 MB
- Allowed types: `image/jpeg`, `image/png`
- Allowed extensions: `.jpg`, `.jpeg`, `.png`

## Storage Convention

Uploaded files are saved under:

```text
/uploads/covers/
```

The database stores the relative public path returned by the storage service.
