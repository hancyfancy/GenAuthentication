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

        public RegisterController(IValidator validator, IUserRepository userRepository, IUserVerificationRepository userVerificationRepository, IUserRolesRepository userRolesRepository)
        {
            _emailValidator = _phoneValidator = validator;
            _userRepo = userRepository;
            _userVerificationRepo = userVerificationRepository;
            _userRolesRepo = userRolesRepository;
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
