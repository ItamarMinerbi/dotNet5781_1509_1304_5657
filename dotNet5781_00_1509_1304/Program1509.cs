﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace dotNet5781_00_1509_1304
{
    partial class Program
    {
        static void Main(string[] args)
        {
            Welcome1509();
            Welcome1304();
            Console.ReadKey();
        }

        static partial void Welcome1304();

        private static void Welcome1509()
        {
            Console.Write("Enter your name please: ");
            string name = Console.ReadLine();
            Console.WriteLine($"Hello {name}!");
        }
    }
}
