using Data.Layer.Dtos;

namespace UserManagementService.Helper
{
    public class ApiResponseWithData : ApiResponse
    {
        public UserDto Data { get; set; }
        public ApiResponseWithData(int statusCode , string? message = null) : base(statusCode, message)
        {
            
        }
    }
}
