﻿using Serilog;
using System;

namespace Lineage2Server
{
    class Program
    {
        static void Main(string[] args)
        {
            Log.Logger = new LoggerConfiguration()
                .MinimumLevel.Debug()
                .WriteTo.Console()
                .CreateLogger();

            Console.WriteLine("Hello World!");
        }
    }
}
