﻿using System.ComponentModel.DataAnnotations;

namespace Backend3.Models
{
    public class RegisterViewModel
    {
        [Required(ErrorMessage = "Email обязателен для заполнения")]
        [Display(Name = "Email")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Дата рождения обязательна для заполнения")]
        [Display(Name = "Дата рождения")]
        public DateTime BirthDate { get; set; }

        [Required(ErrorMessage = "ФИО обязательно для заполнения")]
        [Display(Name = "ФИО")]
        public string Name { get; set; }


        [Required(ErrorMessage = "Пароль обязателен для заполнения")]
        [DataType(DataType.Password)]
        [Display(Name = "Пароль")]
        public string Password { get; set; }

        [Required(ErrorMessage = "Подтверждение пароля обязательно для заполнения")]
        [Compare("Password", ErrorMessage = "Пароли не совпадают")]
        [DataType(DataType.Password)]
        [Display(Name = "Подтвердить пароль")]
        public string PasswordConfirm { get; set; }
        [Display(Name = "Вы организатор?")]
        public bool IsOrganizer { get; set; }
        public IFormFile? Avatar { get; set; }
    }
}
