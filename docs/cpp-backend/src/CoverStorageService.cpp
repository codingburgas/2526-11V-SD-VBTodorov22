#include "../include/CoverStorageService.hpp"

#include <algorithm>
#include <cctype>
#include <chrono>
#include <fstream>
#include <sstream>

namespace film_platform
{
namespace
{
std::string toLower(std::string value)
{
    std::transform(value.begin(), value.end(), value.begin(), [](unsigned char character)
    {
        return static_cast<char>(std::tolower(character));
    });

    return value;
}

std::string fileExtension(const std::string& fileName)
{
    const auto dotPosition = fileName.find_last_of('.');
    if (dotPosition == std::string::npos)
    {
        return {};
    }

    return toLower(fileName.substr(dotPosition));
}
}

CoverStorageService::CoverStorageService(std::filesystem::path uploadRoot)
    : uploadRoot_(std::move(uploadRoot))
{
}

CoverStorageService::Result CoverStorageService::save(int filmId, const UploadedFile& file) const
{
    if (file.bytes.empty())
    {
        return Result{false, {}, "The uploaded file is empty."};
    }

    if (file.bytes.size() > MaxBytes)
    {
        return Result{false, {}, "Cover images must be 5 MB or smaller."};
    }

    if (!isAllowedExtension(file.fileName))
    {
        return Result{false, {}, "Only JPG and PNG cover images are allowed."};
    }

    if (!isAllowedContentType(file.contentType))
    {
        return Result{false, {}, "The uploaded file content type is not supported."};
    }

    std::filesystem::create_directories(uploadRoot_);

    const auto extension = fileExtension(file.fileName);
    const auto timestamp = std::chrono::system_clock::now().time_since_epoch().count();

    std::ostringstream fileNameBuilder;
    fileNameBuilder << "film-" << filmId << "-" << timestamp << extension;

    const auto storedFileName = fileNameBuilder.str();
    const auto absolutePath = uploadRoot_ / storedFileName;

    std::ofstream outputStream(absolutePath, std::ios::binary);
    if (!outputStream.is_open())
    {
        return Result{false, {}, "The cover image could not be saved to disk."};
    }

    outputStream.write(reinterpret_cast<const char*>(file.bytes.data()), static_cast<std::streamsize>(file.bytes.size()));
    outputStream.close();

    if (!outputStream)
    {
        std::filesystem::remove(absolutePath);
        return Result{false, {}, "The cover image could not be saved to disk."};
    }

    return Result{true, "/uploads/covers/" + storedFileName, {}};
}

void CoverStorageService::remove(const std::string& relativePath) const
{
    if (relativePath.empty())
    {
        return;
    }

    const auto fileName = std::filesystem::path(relativePath).filename();
    const auto absolutePath = uploadRoot_ / fileName;

    if (std::filesystem::exists(absolutePath))
    {
        std::filesystem::remove(absolutePath);
    }
}

bool CoverStorageService::isAllowedExtension(const std::string& fileName)
{
    const auto extension = fileExtension(fileName);
    return extension == ".jpg" || extension == ".jpeg" || extension == ".png";
}

bool CoverStorageService::isAllowedContentType(const std::string& contentType)
{
    const auto normalizedContentType = toLower(contentType);
    return normalizedContentType == "image/jpeg" || normalizedContentType == "image/png";
}
}
