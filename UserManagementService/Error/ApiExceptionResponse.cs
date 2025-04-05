using UserManagementService.Helper;

namespace UserManagementService.Error
{
    public class ApiExceptionResponse : ApiResponse
    {
        public string Details { get; set; }
        public ApiExceptionResponse(int Status  , string? message = null , string? details = null ) :base(Status,message)   
        {
            Details = details;
        }
    }
}
