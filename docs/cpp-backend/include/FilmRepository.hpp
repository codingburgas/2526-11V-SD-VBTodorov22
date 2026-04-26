#pragma once

#include <map>
#include <optional>
#include <string>
#include <vector>

namespace film_platform
{
struct FilmRecord
{
    int id = 0;
    std::string title;
    std::string description;
    std::string directorName;
    std::string genre;
    std::string catalogType;
    int releaseYear = 0;
    double averageRating = 0.0;
    int reviewCount = 0;
    std::vector<std::string> actors;
    std::string coverImagePath;
};

class DatabaseSession
{
public:
    virtual ~DatabaseSession() = default;

    virtual std::optional<std::map<std::string, std::string>> querySingle(
        const std::string& sql,
        const std::vector<std::string>& parameters) const = 0;

    virtual std::vector<std::string> queryColumn(
        const std::string& sql,
        const std::vector<std::string>& parameters) const = 0;

    virtual std::size_t execute(
        const std::string& sql,
        const std::vector<std::string>& parameters) const = 0;
};

class FilmRepository
{
public:
    explicit FilmRepository(const DatabaseSession& databaseSession);

    std::optional<FilmRecord> getById(int filmId) const;
    bool updateCoverImagePath(int filmId, const std::string& coverImagePath) const;

private:
    const DatabaseSession& databaseSession_;
};
}
