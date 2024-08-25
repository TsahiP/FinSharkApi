using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace api.Dtos.Comments
{
    public class UpdateCommentDto
    {
        [Required]
        [MinLength(5, ErrorMessage = "Title must be at least 5 Chareavters")]
        [MaxLength(280, ErrorMessage = "Title must be  less then 280 Chareacters")]
        public string Title { get; set; } = string.Empty;
        [Required]
        [MinLength(5, ErrorMessage = "Content must be at least 5 Chareavters")]
        [MaxLength(280, ErrorMessage = "Content must be  less then 280 Chareacters")]
        public string Content { get; set; } = string.Empty;
    }
}