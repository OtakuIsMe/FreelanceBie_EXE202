using BE.src.api.domains.DTOs.User;
using BE.src.api.domains.Enum;
using BE.src.api.domains.Model;
using BE.src.api.helpers;
using BE.src.api.repositories;
using BE.src.api.shared.Type;
using Microsoft.AspNetCore.Mvc;

namespace BE.src.api.services
{
    public interface IUserServ
    {
        Task<IActionResult> RegisterUser(UserRegisterDTO user);
        Task<IActionResult> GetAllUsers();
    }

	public class UserServ : IUserServ
	{
        private readonly IUserRepo _userRepo;
        public UserServ(IUserRepo userRepo)
        {
            _userRepo = userRepo;
        }

		public async Task<IActionResult> GetAllUsers()
		{
			try
            {
                var users = await _userRepo.GetUsers();
                if(users.Count == 0)
                {
                    return ErrorResp.NotFound("No users found");
                }
                return SuccessResp.Ok(users);
            }
            catch (System.Exception ex)
            {
                return ErrorResp.BadRequest(ex.Message);
            }
		}

		public async Task<IActionResult> RegisterUser(UserRegisterDTO user)
		{
			try
            {
                var existingUser = await _userRepo.GetUserByEmail(user.Email);
                if(existingUser != null)
                {
                    return ErrorResp.BadRequest("User already exists");
                }

                var newUser = new User
                {
                    Name = user.Name,
                    Username = user.UserName,
                    Email = user.Email,
                    Password = Utils.HashObject<string>(user.Password),
                    Role = RoleEnum.Customer,
                    CreateAt = DateTime.Now,
                    UpdateAt = DateTime.Now
                };

                var result = await _userRepo.CreateUser(newUser);
                if(!result)
                {
                    return ErrorResp.BadRequest("Failed to create user");
                }
                return SuccessResp.Created("User created successfully");
            }
            catch (System.Exception ex)
            {
                return ErrorResp.BadRequest(ex.Message);
            }
		}
	}
}