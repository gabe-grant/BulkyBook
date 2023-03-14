using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BulkyBook.Utility
{
    // static details class to store all the contants of the application
    public static class SD
    {
        // to avoid "magic strings" we place tall the roles here and then call them in the Register.cshtml file insid Identity
        public const string Role_User_Indi = "Individual";
        public const string Role_User_Comp = "Company";
        public const string Role_Admin = "Admin";
        public const string Role_Employee = "Employee";
    }
}
