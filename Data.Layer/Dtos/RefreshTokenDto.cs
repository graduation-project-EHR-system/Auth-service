﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Data.Layer.Dtos
{
    public class RefreshTokenDto
    {
        [Required(ErrorMessage = "Refresh Token is Required")] 
        public string RefreshToken { get; set; }
    }
}
