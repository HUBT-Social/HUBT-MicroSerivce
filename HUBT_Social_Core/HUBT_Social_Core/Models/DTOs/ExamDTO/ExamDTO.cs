using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HUBT_Social_Core.Models.DTOs.ExamDTO
{
    public class ExamDTO
    {
        public string Id { get; set; } = string.Empty;
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public int DurationMinutes { get; set; } = 0;
        public string Image { get; set; } = string.Empty;
        public string Major { get; set; } = string.Empty;
        public int Credits { get; set; } = 0;
        public int QuestionCount { get; set; } = 0;

        public Question[] Questions { get; set; } = [];
    }
    public class Question
    {
        public string Title { get; set; } = string.Empty;
        public Answer[] Answers { get; set; } = [];

        public int CorrectAnswer { get; set; } = 0;
    }

    public class Answer
    {
        public string Content { get; set; } = string.Empty;

    }
}
