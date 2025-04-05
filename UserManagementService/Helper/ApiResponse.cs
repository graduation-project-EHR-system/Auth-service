namespace UserManagementService.Helper
{
    public class ApiResponse
    {
        public int Status { get; set; }
        public string Message { get; set; }
        public ApiResponse(int Status, string? message = null)
        {
            Status = Status;
            Message = message ?? GetDefaultForErrorResponse(Status);
        }

        private string GetDefaultForErrorResponse(int Status)
        {
            return Status switch
            {
                400 => "Error 400: Bad Request.",
                401 => "Error 401: Unauthorized.",
                403 => "Error 403: Forbidden.",
                404 => "Error 404: Not Found.",
                408 => "Error 408: Request Timeout.",
                409 => "Error 409: Conflict.",
                410 => "Error 410: Gone.",
                413 => "Error 413: Payload Too Large.",
                415 => "Error 415: Unsupported Media Type.",
                418 => "Error 418: I'm a teapot. ☕",
                422 => "Error 422: Unprocessable Entity.",
                429 => "Error 429: Too Many Requests.",
                500 => "Error 500: Internal Server Error.",
                501 => "Error 501: Not Implemented.",
                502 => "Error 502: Bad Gateway.",
                503 => "Error 503: Service Unavailable.",
                504 => "Error 504: Gateway Timeout.",
                _ => "Unknown error code."
            };
        }
    }
}
