using SWD_SoftwareArchitecture.DTOs;
using SWD_SoftwareArchitecture.Services;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SWD_SoftwareArchitecture.Services.Interfaces
{
    public interface IUserService
    {
        Task<IEnumerable<UserDto>> GetAllUsersAsync();
        Task<ServiceResult<UserDto>> GetUserByIdAsync(int id);
        Task<ServiceResult<UserDto>> CreateUserAsync(UserCreateDto createUserDto);
        Task<ServiceResult<UserDto>> UpdateUserAsync(UserUpdateDto updateUserDto);
        Task<ServiceResult<UserDto>> DeleteUserAsync(int id);
    }
}