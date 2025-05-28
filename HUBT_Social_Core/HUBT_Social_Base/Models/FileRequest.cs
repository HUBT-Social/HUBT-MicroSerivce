using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HUBT_Social_Base.Models
{
    public class FileRequest
    {
        public IFormFile file { set; get; }

    }
}
