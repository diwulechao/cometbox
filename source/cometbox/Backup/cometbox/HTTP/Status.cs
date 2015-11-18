using System;
using System.Collections.Generic;
using System.Text;

namespace cometbox.HTTP
{
    public enum Status
    {
        OK = 200,
        NotAuthorized = 401,
        NotFound = 404,
        InternalServerError = 500
    }
}
