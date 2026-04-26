#pragma once

#include "HttpTypes.hpp"

#include <filesystem>
#include <string>

namespace film_platform
{
class CoverStorageService
{
public:
    struct Result
    {
        bool success = false;
        std::string relativePath;
        std::string errorMessage;
    };

    explicit CoverStorageService(std::filesystem::path uploadRoot);

    Result save(int filmId, const UploadedFile& file) const;
    void remove(const std::string& relativePath) const;

private:
    static constexpr std::size_t MaxBytes = 5U * 1024U * 1024U;
    std::filesystem::path uploadRoot_;

    static bool isAllowedExtension(const std::string& fileName);
    static bool isAllowedContentType(const std::string& contentType);
};
}
