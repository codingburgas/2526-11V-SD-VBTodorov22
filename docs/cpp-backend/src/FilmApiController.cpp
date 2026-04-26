#include "../include/AdminAuthMiddleware.hpp"
#include "../include/CoverStorageService.hpp"
#include "../include/FilmRepository.hpp"

#include <sstream>

namespace film_platform
{
namespace
{
std::string escapeJson(const std::string& value)
{
    std::string escaped;
    escaped.reserve(value.size());

    for (const char character : value)
    {
        switch (character)
        {
            case '\\':
                escaped += "\\\\";
                break;
            case '"':
                escaped += "\\\"";
                break;
            case '\n':
                escaped += "\\n";
                break;
            case '\r':
                escaped += "\\r";
                break;
            case '\t':
                escaped += "\\t";
                break;
            default:
                escaped += character;
                break;
        }
    }

    return escaped;
}

HttpResponse jsonMessage(int statusCode, const std::string& message)
{
    return HttpResponse{
        statusCode,
        "application/json",
        "{\"message\":\"" + escapeJson(message) + "\"}"
    };
}
}

HttpResponse getFilmById(const HttpRequest& request, const FilmRepository& repository)
{
    const auto routeValue = request.routeParameters.find("id");
    if (routeValue == request.routeParameters.end())
    {
        return jsonMessage(400, "Film id route parameter is required.");
    }

    const int filmId = std::stoi(routeValue->second);
    const auto film = repository.getById(filmId);

    if (!film.has_value())
    {
        return jsonMessage(404, "Film not found.");
    }

    std::ostringstream responseBuilder;
    responseBuilder
        << "{"
        << "\"id\":" << film->id << ","
        << "\"title\":\"" << escapeJson(film->title) << "\","
        << "\"description\":\"" << escapeJson(film->description) << "\","
        << "\"coverUrl\":\"" << escapeJson(film->coverImagePath) << "\","
        << "\"releaseYear\":" << film->releaseYear << ","
        << "\"genre\":\"" << escapeJson(film->genre) << "\","
        << "\"catalogType\":\"" << escapeJson(film->catalogType) << "\","
        << "\"directorName\":\"" << escapeJson(film->directorName) << "\","
        << "\"averageRating\":" << film->averageRating << ","
        << "\"reviewCount\":" << film->reviewCount
        << "}";

    return HttpResponse{200, "application/json", responseBuilder.str()};
}

HttpResponse uploadFilmCover(
    const HttpRequest& request,
    FilmRepository& repository,
    const CoverStorageService& storageService,
    const AdminAuthMiddleware& authMiddleware)
{
    const auto authorization = authMiddleware.authorize(request);
    if (authorization.statusCode != 200)
    {
        return authorization;
    }

    const auto routeValue = request.routeParameters.find("id");
    if (routeValue == request.routeParameters.end())
    {
        return jsonMessage(400, "Film id route parameter is required.");
    }

    if (!request.file.has_value())
    {
        return jsonMessage(400, "A cover image file is required.");
    }

    const int filmId = std::stoi(routeValue->second);
    const auto film = repository.getById(filmId);
    if (!film.has_value())
    {
        return jsonMessage(404, "Film not found.");
    }

    const auto storageResult = storageService.save(filmId, request.file.value());
    if (!storageResult.success)
    {
        return jsonMessage(400, storageResult.errorMessage);
    }

    if (!repository.updateCoverImagePath(filmId, storageResult.relativePath))
    {
        storageService.remove(storageResult.relativePath);
        return jsonMessage(500, "The film record could not be updated.");
    }

    if (!film->coverImagePath.empty())
    {
        storageService.remove(film->coverImagePath);
    }

    return HttpResponse{
        200,
        "application/json",
        "{"
        "\"filmId\":" + std::to_string(filmId) + ","
        "\"title\":\"" + escapeJson(film->title) + "\","
        "\"coverImageUrl\":\"" + escapeJson(storageResult.relativePath) + "\","
        "\"message\":\"Cover image uploaded successfully.\""
        "}"
    };
}
}
