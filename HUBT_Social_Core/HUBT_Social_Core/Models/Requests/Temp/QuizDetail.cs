using HUBT_Social_Core.Models.DTOs.ExamDTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HUBT_Social_Core.Models.Requests.Temp
{
    public class QuizDetail : ExamDTO
    {
        public QuizDetail()
        {
         
        }
        public QuizDetail(ExamDTO examDTO)
        {
            this.Id = examDTO.Id;
            this.Title = examDTO.Title;
            this.Description = examDTO.Description;
            this.Image = examDTO.Image;
            this.Major = examDTO.Major;
            this.Credits = examDTO.Credits;
        }
        public int QuestionCount
        {
            get => Questions.Length;
        }

        public Question[] Questions { get; set; } = [];
        
    }
}
