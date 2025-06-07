
using Data.Layer.Entities;
using Data.Layer.Dtos;

namespace UserManagementService.Interfaces
{
    public interface IUserService
    {
        Task<User> HandleUserCreatedEventAsync(KafkaUserEvent userEvent);
        Task<User> HandleUserUpdatedAsync(KafkaUserEvent userEvent);
        Task<bool> HandleUserDeletedAsync(KafkaUserEvent userEvent);
    }
}
