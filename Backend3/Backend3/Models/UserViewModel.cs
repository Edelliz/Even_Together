﻿using System.ComponentModel.DataAnnotations;

namespace Backend3.Models
{
    public class ShortUserViewModel
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string? Avatar { get; set; }
    }
    public class UserViewModel: ShortUserViewModel
    {
        public string Email { get; set; }
        public DateTime BirthDate { get; set; }
    }
    public class EditUserViewModel
    {
        public Guid Id { get; set; }
        [Required(ErrorMessage = "Email обязателен для заполнения")]
        [Display(Name = "Email")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Дата рождения обязательна для заполнения")]
        [Display(Name = "Дата рождения")]
        public DateTime BirthDate { get; set; }

        [Required(ErrorMessage = "ФИО обязательно для заполнения")]
        [Display(Name = "ФИО")]
        public string Name { get; set; }

        public IFormFile Avatar { get; set; }

    }
}
