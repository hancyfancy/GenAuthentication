using GenCommon.Shared.Extensions;
using GenCore.Data.Models;
using GenCore.Data.Repositories.Interface;
using GenCryptography.Service.Utilities.Interface;
using GenNotification.Service.Utilities.Interface;
using GenTokenization.Service.Utilities.Interface;
using GenValidation.Service.Utilities.Interface;
using Microsoft.AspNetCore.Mvc;
using static Dapper.SqlMapper;

namespace GenAuthentication.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]/[action]")]
    public class LoginController : ControllerBase
    {
        private readonly IUserVerificationRepository _userVerificationRepo;
        private readonly IUserEncryptionRepository _userEncryptionRepo;
        private readonly IUserTokensRepository _userTokensRepo;
        private readonly IKeyGenerator _keyGenerator;
        private readonly IEncryptor _encryptor;
        private readonly ITokenizer _alphanumericTokenizer;
        private readonly ITokenizer _numericTokenizer;
        private readonly IEmailDespatcher _emailDespatcher;
        private readonly ISmsDespatcher _smsDespatcher;

        public LoginController(IKeyGenerator keyGenerator, IEncryptor encryptor, ITokenizer tokenizer, IUserVerificationRepository userVerificationRepository, IUserEncryptionRepository userEncryptionRepository, IUserTokensRepository userTokensRepository)
        {
            _keyGenerator = keyGenerator;
            _encryptor = encryptor;
            _alphanumericTokenizer = _numericTokenizer = tokenizer;
            _userVerificationRepo = userVerificationRepository;
            _userEncryptionRepo = userEncryptionRepository;
            _userTokensRepo = userTokensRepository;
        }

        [HttpPost(Name = "Individual")]
        public IActionResult Individual([FromBody] User user)
        {
            UserVerification userVerification = _userVerificationRepo.Get(user.Username);

            if (userVerification is null)
            {
                return BadRequest("User not found");
            }

            userVerification.Username = user.Username;

            string userVerificationJson = userVerification.ToJson();

            if (userVerificationJson.IsEmpty())
            {
                return BadRequest("User could not be verified");
            }

            byte[] encryptionKey = _keyGenerator.GenerateEncryptionKey();

            if ((encryptionKey is null) || (encryptionKey.Count() == 0))
            {
                return BadRequest("Failed to generate encryption key");
            }

            _userEncryptionRepo.InsertOrUpdate(userVerification.UserId, encryptionKey);

            string encryptedUserVerificationJson = _encryptor.Encrypt(encryptionKey, userVerificationJson);

            if (encryptedUserVerificationJson.IsEmpty())
            {
                return BadRequest("User verification could not be processed");
            }

            if (!userVerification.EmailVerified)
            {
                _emailDespatcher.SendEmail(GenCommon.Shared.Settings.SmtpHost, GenCommon.Shared.Settings.SmtpPort, GenCommon.Shared.Settings.SmtpUseSsl, userVerification.Email, GenCommon.Shared.Settings.SmtpSender, GenCommon.Shared.Settings.SmtpPassword, "Verification required", $"Please verify email at https://localhost:7138/api/Authentication/Verify?mode=email&user={encryptedUserVerificationJson}");
            }

            if (!userVerification.PhoneVerified)
            {
                _smsDespatcher.SendSms(GenCommon.Shared.Settings.ClickSendUsername, GenCommon.Shared.Settings.ClickSendApiKey, userVerification.Phone, GenCommon.Shared.Settings.SmsSender, $"Please verify phone number at https://localhost:7138/api/Authentication/Verify?mode=phone&user={encryptedUserVerificationJson}");
            }

            if (!(userVerification.EmailVerified || userVerification.PhoneVerified))
            {
                return BadRequest("Either email or phone needs to be verified to access content");
            }

            UserToken userToken = new UserToken()
            { 
                UserId = userVerification.UserId,
                Username = userVerification.Username,
                Email = userVerification.Email,
                Phone = userVerification.Phone,
                Role = userVerification.Role,
                LastActive = userVerification.LastActive
            };

            if (userVerification.EmailVerified)
            {
                userToken.Token = _alphanumericTokenizer.GetUniqueKey(GenCommon.Shared.Settings.EmailValidationSize);

                if (userToken.Token.IsEmpty())
                {
                    return BadRequest("Error while generating token");
                }

                string validationMessage = $"Please use the following token, which expires in 24 hours, to login: {userToken.Token}";

                _emailDespatcher.SendEmail(GenCommon.Shared.Settings.SmtpHost, GenCommon.Shared.Settings.SmtpPort, GenCommon.Shared.Settings.SmtpUseSsl, userVerification.Email, GenCommon.Shared.Settings.SmtpSender, GenCommon.Shared.Settings.SmtpPassword, "Validate login attempt", validationMessage);
            }
            else if (userVerification.PhoneVerified)
            {
                userToken.Token = _numericTokenizer.GetUniqueKey(GenCommon.Shared.Settings.PhoneValidationSize);

                if (userToken.Token.IsEmpty())
                {
                    return BadRequest("Error while generating token");
                }

                string validationMessage = $"Please use the following token, which expires in 24 hours, to login: {userToken.Token}";

                _smsDespatcher.SendSms(GenCommon.Shared.Settings.ClickSendUsername, GenCommon.Shared.Settings.ClickSendApiKey, userVerification.Phone, GenCommon.Shared.Settings.SmsSender, validationMessage);
            }

            _userTokensRepo.InsertOrUpdate(userVerification.UserId, userToken.Token);

            return Ok(userToken.Token);
        }
    }
}
