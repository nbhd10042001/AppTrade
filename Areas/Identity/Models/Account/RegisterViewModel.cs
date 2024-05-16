// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.ComponentModel.DataAnnotations;

namespace App.Areas.Identity.Models.AccountViewModels
{
    public class RegisterViewModel
    {
        [Required(ErrorMessage = "Phải nhập {0}")]
        [EmailAddress(ErrorMessage = "Sai định dạng Email")]
        [Display(Name = "Email", Prompt = "Địa chỉ email")]
        public string Email { get; set; }


        [Required(ErrorMessage = "Phải nhập {0}")]
        [StringLength(100, ErrorMessage = "{0} phải dài từ {2} đến {1} ký tự.", MinimumLength = 2)]
        [DataType(DataType.Password)]
        [Display(Name = "Mật khẩu", Prompt = "Mật khẩu")]
        public string Password { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "Lặp lại mật khẩu", Prompt = "Lặp lại Mật khẩu")]
        [Compare("Password", ErrorMessage = "Mật khẩu lặp lại không chính xác.")]
        public string ConfirmPassword { get; set; }


        [DataType(DataType.Text)]
        [Display(Name = "Tên tài khoản", Prompt ="Tên tài khoản")]
        [Required(ErrorMessage = "Phải nhập {0}")]
        [StringLength(100, ErrorMessage = "{0} phải dài từ {2} đến {1} ký tự.", MinimumLength = 3)]
        public string UserName { get; set; }

    }
}
