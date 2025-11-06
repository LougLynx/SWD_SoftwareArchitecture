using SWD_SoftwareArchitecture.DTOs;
using SWD_SoftwareArchitecture.Models;
using SWD_SoftwareArchitecture.Repositories.Interfaces;
using SWD_SoftwareArchitecture.Services.Interfaces;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SWD_SoftwareArchitecture.Services
{
    public class UserService : IUserService
    {
        private readonly IUserRepository _userRepository;

        public UserService(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        // Ánh xạ từ Model sang DTO
        private UserDto MapToUserDto(User user)
        {
            return new UserDto
            {
                Id = user.UserId,
                Username = user.FullName,
                Email = user.Email,
                Role = int.TryParse(user.Role, out var roleInt) ? roleInt : 0 // Convert string to int, fallback to 0 if invalid
            };
        }

        public async Task<IEnumerable<UserDto>> GetAllUsersAsync()
        {
            var users = await _userRepository.GetAllAsync();
            return users.Select(MapToUserDto);
        }

        public async Task<ServiceResult<UserDto>> GetUserByIdAsync(int id)
        {
            var user = await _userRepository.GetByIdAsync(id);
            if (user == null)
            {
                return ServiceResult<UserDto>.Failure("User not found.");
            }
            return ServiceResult<UserDto>.Success(MapToUserDto(user));
        }

        public async Task<ServiceResult<UserDto>> CreateUserAsync(UserCreateDto createUserDto)
        {
            // 1. Kiểm tra logic nghiệp vụ: Username không được trùng
            var existingUser = await _userRepository.GetByUsernameAsync(createUserDto.Username);
            if (existingUser != null)
            {
                return ServiceResult<UserDto>.Failure("Username already exists.");
            }

            // 2. Xử lý logic (ví dụ: băm mật khẩu)
            // TODO: Băm mật khẩu bằng một thư viện an toàn như BCrypt.Net
            // Tạm thời lưu mật khẩu (đây là hành vi KHÔNG an toàn, chỉ dùng cho demo)
            var hashedPassword = createUserDto.Password; // Thay thế bằng logic băm thật

            // 3. Tạo Model
            var user = new User
            {
                FullName = createUserDto.Username,
                Email = createUserDto.Email,
                PasswordHash = hashedPassword, // Sẽ lưu vào cột PasswordHash
                Role = createUserDto.Role.ToString(), // Convert int to string
                CreatedAt = System.DateTime.Now
            };

            // 4. Lưu vào DB
            await _userRepository.AddAsync(user);

            // 5. Trả về DTO
            return ServiceResult<UserDto>.Success(MapToUserDto(user));
        }

        public async Task<ServiceResult<UserDto>> UpdateUserAsync(UserUpdateDto updateUserDto)
        {
            // 1. Tìm user
            var user = await _userRepository.GetByIdAsync(updateUserDto.Id);
            if (user == null)
            {
                return ServiceResult<UserDto>.Failure("User not found.");
            }

            // 2. Kiểm tra logic nghiệp vụ: Username không được trùng (trừ chính nó)
            var existingUser = await _userRepository.GetByUsernameAsync(updateUserDto.Username);
            if (existingUser != null && existingUser.UserId != updateUserDto.Id)
            {
                return ServiceResult<UserDto>.Failure("Username already exists.");
            }

            // 3. Cập nhật thông tin
            user.FullName = updateUserDto.Username;
            user.Email = updateUserDto.Email;
            user.Role = updateUserDto.Role.ToString();

            // 4. Lưu vào DB
            await _userRepository.UpdateAsync(user);

            // 5. Trả về DTO
            return ServiceResult<UserDto>.Success(MapToUserDto(user));
        }

        public async Task<ServiceResult<UserDto>> DeleteUserAsync(int id)
        {
            // 1. Tìm user
            var user = await _userRepository.GetByIdAsync(id);
            if (user == null)
            {
                return ServiceResult<UserDto>.Failure("User not found.");
            }

            // 2. Xóa khỏi DB
            await _userRepository.DeleteAsync(user);

            // 3. Trả về DTO của user đã xóa
            return ServiceResult<UserDto>.Success(MapToUserDto(user));
        }
    }
}