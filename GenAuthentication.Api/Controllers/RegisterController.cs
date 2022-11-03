using GenCore.Data.Models;
using GenCore.Data.Repositories.Implementation;
using GenCore.Data.Repositories.Interface;
using GenValidation.Service.Utilities.Implementation;
using GenValidation.Service.Utilities.Interface;
using Microsoft.AspNetCore.Mvc;

namespace GenAuthentication.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]/[action]")]
    public class RegisterController : ControllerBase
    {
        private readonly IUserRepository _userRepo;
        private readonly IUserVerificationRepository _userVerificationRepo;
        private readonly IUserRolesRepository _userRolesRepo;
        private readonly IValidator _emailValidator;
        private readonly IValidator _phoneValidator;
        private readonly string _connectionString;

        public RegisterController()
        {
            _connectionString = "Data Source=localhost;Initial Catalog=CwRetail;Persist Security Info=true;User ID=TestLogin; Password = ABC123";
            _emailValidator = new EmailValidator();
            _phoneValidator = new PhoneValidator();
            _userRepo = new UserRepository(_connectionString);
            _userVerificationRepo = new UserVerificationRepository(_connectionString);
            _userRolesRepo = new UserRolesRepository(_connectionString);
        }

        [HttpPost(Name = "New")]
        public IActionResult New([FromBody] User user)
        {
            if (!_emailValidator.IsValid(user.Email))
            {
                return BadRequest("Invalid email");
            }

            if (!_phoneValidator.IsValid(user.Phone))
            {
                return BadRequest("Invalid phone");
            }

            long userId = _userRepo.Insert(user);

            _userVerificationRepo.Insert(new UserVerification()
            {
                UserId = userId,
                EmailVerified = false,
                PhoneVerified = false
            });

            _userRolesRepo.Insert(userId);

            return Ok();
        }
    }
}
