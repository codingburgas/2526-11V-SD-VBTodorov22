#pragma once

#include <cstdint>
#include <map>
#include <optional>
#include <string>
#include <vector>

namespace film_platform
{
struct UploadedFile
{
    std::string fileName;
    std::string contentType;
    std::vector<std::uint8_t> bytes;
};

struct AuthContext
{
    bool authenticated = false;
    bool isAdmin = false;
    std::string userId;
};

struct HttpRequest
{
    std::map<std::string, std::string> routeParameters;
    std::map<std::string, std::string> headers;
    std::optional<UploadedFile> file;
    AuthContext auth;
};

struct HttpResponse
{
    int statusCode = 200;
    std::string contentType = "application/json";
    std::string body;
};
}
