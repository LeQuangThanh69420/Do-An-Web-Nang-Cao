using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using BE.Context;
using BE.InterfaceController;
using BE.Model.Dto;
using BE.Model.Entity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BE.Controllers
{
    public class UserController : BaseApiController, IUserController
    {
        private readonly DataContext _context;
        private readonly EmailController _email;
        public UserController(DataContext context, EmailController email)
        {
            _context = context;
            _email = email;
        }

        private async Task<bool> UserExists(string Username)
        {
            return await _context.User.AnyAsync(x => x.Username == Username.ToLower());
        }
        private async Task<bool> EmailExists(string Email)
        {
            return await _context.User.AnyAsync(x => x.Email == Email.ToLower());
        }

        [HttpPost("Register")]
        public async Task<ActionResult> Register([FromBody] UserRegisterInputDto input)
        {
            if (await UserExists(input.Username))
            {
                return BadRequest(new {message = "Tên tài khoản đã có người sử dụng!"});
            }
            if (await EmailExists(input.Email))
            {
                return BadRequest(new {message = "Email đã có người sử dụng!"});
            }
            var activeCode = new Random().Next(1000, 9999);
            var rs = await _email.SendEmail(new EmailModel()
            {
                To = input.Email,
                Subject = "Kích hoạt tài khoản YuGhiOh TCG",
                Body = "<h3>Bấm nút để kích hoạt</h3><a href='http://localhost:5233/api/User/ActiveUser" + "/" + input.Username + "/" + activeCode + "'><button style='width: 200px; height: 40px; background-color: #008cff; color: white; border-radius: 6px; border: none;'>Bấm tôi</button></a>",
            });
            if ((int)rs.GetType().GetProperty("StatusCode").GetValue(rs, null) == 200)
            {
                var newUser = new User()
                {
                    Username = input.Username.ToLower(),
                    Password = input.Password,
                    Email = input.Email,
                    Money = 0,
                    Actived = false,
                    ActiveCode = activeCode,
                    AvatarUrl = "https://res.cloudinary.com/dslzbnfu8/image/upload/v1699185130/samples/DuRiu.png",
                };
                _context.User.Add(newUser);
                await _context.SaveChangesAsync();
            }
            return rs;
        }

        [HttpGet("ActiveUser/{username}/{activeCode}")]
        public async Task<ActionResult> ActiveUser(string username, int activeCode)
        {
            var user = await _context.User.SingleOrDefaultAsync(u => u.Username == username);
            if (user == null) return NotFound(new {message = "URL không tồn tại!"});
            if (user.ActiveCode == activeCode)
            {
                user.Actived = true;
                user.ActiveCode = null;
                await _context.SaveChangesAsync();
                return Ok(new {message = "Kích hoạt tài khoản thành công!"});
            }
            else return NotFound(new {message = "URL không tồn tại!"});
        }

        [HttpPost("Login")]
        public async Task<ActionResult<UserLoginOutputDto>> Login([FromBody] UserLoginInputDto input)
        {
            var user = await _context.User.SingleOrDefaultAsync(x => x.Username == input.Username);
            if (user == null) return BadRequest(new {message = "Tài khoản không tồn tại!"});
            if (input.Password != user.Password) return BadRequest(new {message = "Sai mật khẩu, vui lòng kiểm tra lại!"});
            if (user.Actived != true) return BadRequest(new {message = "Tài khoản chưa kích hoạt, vui lòng kiểm tra Email!"});
            else return Ok(new UserLoginOutputDto()
            {
                Username = user.Username,
                AvatarURL = user.AvatarUrl,
                Token = "daylatoken",
            });
        }

        [HttpPost("ForgetPassword")]
        public async Task<ActionResult> ForgetPassword([FromBody] UserForgetPasswordInputDto input)
        {
            var user = await _context.User.SingleOrDefaultAsync(x => x.Username == input.Username);
            if (user == null) return BadRequest(new {message = "Tài khoản không tồn tại!"});
            if (user.Email != input.Email) return BadRequest(new {message = "Email không đúng"});
            else
            {
                return await _email.SendEmail(new EmailModel()
                {
                    To = user.Email,
                    Subject = "Mật khẩu bạn quên!",
                    Body = "<h2>Vui lòng không chia sẻ mật khẩu cho bất kỳ ai, kể cả ADMIN!</h2><h3>Mật khẩu của bạn là:</h3>" + user.Password,
                });
            }
        }

        [HttpPost("ChangePassword")]
        public async Task<ActionResult> ChangePassword([FromBody] UserChangePasswordInputDto input)
        {
            var user = await _context.User.SingleOrDefaultAsync(x => x.Username == input.Username);
            if (user == null) return BadRequest(new {message = "Tài khoản không tồn tại!"});
            if (user.Password != input.CurrentPassword) return BadRequest(new {message = "Sai Mật Khẩu Cũ"});
            else
            {
                user.Password = input.NewPassword;
                await _context.SaveChangesAsync();
                return Ok(new {message = "Thay đổi mật khẩu thành công"});
            }
        }

        [HttpPost("ChangeEmail")]
        public async Task<ActionResult> ChangeEmail([FromBody] UserChangeEmailInputDto input)
        {
            var user = await _context.User.SingleOrDefaultAsync(x => x.Username == input.Username);
            if (user == null) return BadRequest(new {message = "Tài khoản không tồn tại!"});
            if (user.Password != input.CurrentPassword) return BadRequest(new {message = "Sai Mật Khẩu Cũ"});
            if (user.Email != input.CurrentEmail) return BadRequest(new {message = "Sai Email Cũ"});
            if (await EmailExists(input.NewEmail))
            {
                return BadRequest(new {message = "Email đã có người sử dụng!"});
            }
            else
            {
                user.Email = input.NewEmail;
                await _context.SaveChangesAsync();
                return Ok(new {message = "Thay đổi Email thành công"});
            }
        }
        
        [HttpPost("ChangeAvatar")]
        public async Task<ActionResult> ChangeAvatar([FromBody] UserChangeAvatarInputDto input)
        {
            var user = await _context.User.SingleOrDefaultAsync(x => x.Username == input.Username);
            if (user == null) return BadRequest(new {message = "Tài khoản không tồn tại!"});
            user.AvatarUrl = input.NewAvatar;
            await _context.SaveChangesAsync();
            return Ok(new {message = "Thay đổi Avatar thành công"});
        }
    }
}