﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DO
{
    public class User
    {
        public string Username { get; set; }

        public string Password { get; set; }

        public string Email { get; set; } = "";

        public bool IsAdmin { get; set; }

        public bool IsActive { get; set; }
    }
}
