#pragma once

#include "HttpTypes.hpp"

namespace film_platform
{
class AdminAuthMiddleware
{
public:
    HttpResponse authorize(const HttpRequest& request) const
    {
        if (!request.auth.authenticated)
        {
            return HttpResponse{401, "application/json", R"({"message":"Authentication is required to upload a cover image."})"};
        }

        if (!request.auth.isAdmin)
        {
            return HttpResponse{403, "application/json", R"({"message":"Only admins can upload cover images."})"};
        }

        return HttpResponse{200, "application/json", {}};
    }
};
}
