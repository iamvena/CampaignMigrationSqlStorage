using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MigrationSqlStorage
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Start migration...");

            new MigrationSqlStorage().Execute();

            Console.WriteLine("Press any key to exit...");

            Console.ReadLine();
        }
    }
}
