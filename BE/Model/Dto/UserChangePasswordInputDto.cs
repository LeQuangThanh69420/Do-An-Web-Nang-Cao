using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace BE.Model.Dto
{
    public class UserChangePasswordInputDto
    {
        [Required] public string Username { get; set; }
        [Required][StringLength(16, MinimumLength = 8)] public string CurrentPassword { get; set; }
        [Required][StringLength(10, MinimumLength = 6)] public string NewPassword { get; set; }
    }
}