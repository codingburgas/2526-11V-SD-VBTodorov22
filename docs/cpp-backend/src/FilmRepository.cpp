#include "../include/FilmRepository.hpp"

namespace film_platform
{
FilmRepository::FilmRepository(const DatabaseSession& databaseSession)
    : databaseSession_(databaseSession)
{
}

std::optional<FilmRecord> FilmRepository::getById(int filmId) const
{
    const auto filmRow = databaseSession_.querySingle(
        R"(
            SELECT
                f.id,
                f.title,
                f.description,
                f.release_year,
                f.genre,
                f.catalog_type,
                COALESCE(f.cover_image_path, '') AS cover_image_path,
                d.full_name AS director_name,
                COALESCE(AVG(r.rating), 0) AS average_rating,
                COUNT(r.id) AS review_count
            FROM films f
            INNER JOIN directors d ON d.id = f.director_id
            LEFT JOIN reviews r ON r.film_id = f.id
            WHERE f.id = ?
            GROUP BY
                f.id,
                f.title,
                f.description,
                f.release_year,
                f.genre,
                f.catalog_type,
                f.cover_image_path,
                d.full_name
        )",
        {std::to_string(filmId)});

    if (!filmRow.has_value())
    {
        return std::nullopt;
    }

    auto film = FilmRecord{};
    film.id = std::stoi(filmRow->at("id"));
    film.title = filmRow->at("title");
    film.description = filmRow->at("description");
    film.directorName = filmRow->at("director_name");
    film.genre = filmRow->at("genre");
    film.catalogType = filmRow->at("catalog_type");
    film.releaseYear = std::stoi(filmRow->at("release_year"));
    film.averageRating = std::stod(filmRow->at("average_rating"));
    film.reviewCount = std::stoi(filmRow->at("review_count"));
    film.coverImagePath = filmRow->at("cover_image_path");
    film.actors = databaseSession_.queryColumn(
        R"(
            SELECT a.full_name
            FROM film_actors fa
            INNER JOIN actors a ON a.id = fa.actor_id
            WHERE fa.film_id = ?
            ORDER BY a.full_name
        )",
        {std::to_string(filmId)});

    return film;
}

bool FilmRepository::updateCoverImagePath(int filmId, const std::string& coverImagePath) const
{
    const auto affectedRows = databaseSession_.execute(
        R"(
            UPDATE films
            SET cover_image_path = ?
            WHERE id = ?
        )",
        {coverImagePath, std::to_string(filmId)});

    return affectedRows == 1;
}
}
