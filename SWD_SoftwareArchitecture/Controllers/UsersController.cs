using Microsoft.AspNetCore.Mvc;
using SWD_SoftwareArchitecture.DTOs;
using SWD_SoftwareArchitecture.Services.Interfaces;
using System.Threading.Tasks;

namespace SWD_SoftwareArchitecture.Controllers
{
    public class UsersController : Controller
    {
        private readonly IUserService _userService;

        public UsersController(IUserService userService)
        {
            _userService = userService;
        }

        // GET: /Users
        public async Task<IActionResult> Index()
        {
            var users = await _userService.GetAllUsersAsync();
            return View(users); // Cần tạo View/Users/Index.cshtml
        }

        // GET: /Users/Details/5
        public async Task<IActionResult> Details(int id)
        {
            var result = await _userService.GetUserByIdAsync(id);
            if (!result.IsSuccess)
            {
                return NotFound(); // Hoặc hiển thị thông báo lỗi
            }
            return View(result.Data); // Cần tạo View/Users/Details.cshtml
        }

        // GET: /Users/Create
        public IActionResult Create()
        {
            return View(); // Cần tạo View/Users/Create.cshtml
        }

        // POST: /Users/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(UserCreateDto userCreateDto)
        {
            if (!ModelState.IsValid)
            {
                return View(userCreateDto);
            }

            var result = await _userService.CreateUserAsync(userCreateDto);
            if (!result.IsSuccess)
            {
                ModelState.AddModelError(string.Empty, result.ErrorMessage);
                return View(userCreateDto);
            }

            return RedirectToAction(nameof(Index));
        }

        // GET: /Users/Edit/5
        public async Task<IActionResult> Edit(int id)
        {
            var result = await _userService.GetUserByIdAsync(id);
            if (!result.IsSuccess)
            {
                return NotFound();
            }

            // Map từ UserDto sang UserUpdateDto
            var userUpdateDto = new UserUpdateDto
            {
                Id = result.Data.Id,
                Username = result.Data.Username,
                Email = result.Data.Email,
                Role = result.Data.Role
            };

            return View(userUpdateDto); // Cần tạo View/Users/Edit.cshtml
        }

        // POST: /Users/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, UserUpdateDto userUpdateDto)
        {
            if (id != userUpdateDto.Id)
            {
                return BadRequest();
            }

            if (!ModelState.IsValid)
            {
                return View(userUpdateDto);
            }

            var result = await _userService.UpdateUserAsync(userUpdateDto);
            if (!result.IsSuccess)
            {
                ModelState.AddModelError(string.Empty, result.ErrorMessage);
                return View(userUpdateDto);
            }

            return RedirectToAction(nameof(Index));
        }

        // GET: /Users/Delete/5
        public async Task<IActionResult> Delete(int id)
        {
            var result = await _userService.GetUserByIdAsync(id);
            if (!result.IsSuccess)
            {
                return NotFound();
            }
            return View(result.Data); // Cần tạo View/Users/Delete.cshtml
        }

        // POST: /Users/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var result = await _userService.DeleteUserAsync(id);
            if (!result.IsSuccess)
            {
                // Có thể thêm thông báo lỗi vào TempData
                return RedirectToAction(nameof(Index));
            }
            return RedirectToAction(nameof(Index));
        }
    }
}