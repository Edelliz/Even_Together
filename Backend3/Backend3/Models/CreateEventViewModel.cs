﻿using Backend3.Storage;
using System.ComponentModel.DataAnnotations;

namespace Backend3.Models
{
    public class CreateEventViewModel
    {
        public Guid Id { get; set; }
        [Required(ErrorMessage = "Обязательное поле")]
        [Display(Name = "Название")]
        public string Title { get; set; }

        [Required(ErrorMessage = "Обязательное поле")]
        [Display(Name = "Дата")]
        public DateTime Date { get; set; }

        [Required(ErrorMessage = "Обязательное поле")]
        [Display(Name = "Цена")]
        public int Price { get; set; }

        [Required(ErrorMessage = "Обязательное поле")]
        [Display(Name = "Описание")]
        public string Description { get; set; }
        [Required(ErrorMessage = "Обязательное поле")]
        [Display(Name = "Место проведения")]
        public string Place { get; set; }
        public IFormFile? Poster { get; set; }
    }

    public class ShortEventViewModel
    {
        public Guid Id { get; set; }
        public string Title { get; set; }

        public string? Poster { get; set; }
    }
    public class EventViewModel : ShortEventViewModel
    {
        public int Price { get; set; }
        public string Organizer { get; set; }
        public DateTime Date { get; set; }
        public string Description { get; set; }
        public bool IsOwner { get; set; }
        public List<ShortUserViewModel> Searching { get; set; }
        public int Grade { get; set; }
        public string Place { get; set; }
        public List<Review> Reviews { get; set; }
        public List<GroupViewModel> Groups { get; set; } 
    }

    public class GradeViewModel
    {
        public Guid EventId;
        public int Grade;
    }
    public class ReviewViewModel
    {
        [Required]
        public Guid EventId { get; set; }
        [Required]
        public string Text { get; set; }
    }
}
