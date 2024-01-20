﻿using System.ComponentModel.DataAnnotations;

namespace Neo4Fredis.Models.Entities
{
    public class Login
    {
        public string Login_Kao { get; set; } = string.Empty;
       
        [EmailAddress]
        public string Email { get; set; } = string.Empty;

        [DataType(DataType.Password)]
        public string Lozinka { get; set; } = string.Empty;
    }
}
