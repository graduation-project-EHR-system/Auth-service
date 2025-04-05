using Data.Layer.Dtos;

namespace UserManagementService.Helper
{
    public class ApiResponseWithData : ApiResponse
    {
        public UserDto Data { get; set; }
        public ApiResponseWithData(int Status , string? message = null) : base(Status, message)
        {
            
        }
    }
}
