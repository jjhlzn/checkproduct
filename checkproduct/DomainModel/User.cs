using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace checkproduct.DomainModel
{
    public class User
    {
        public static string Role_Checker_Manager = "checker_manager";
        public static string Role_Checker = "checker";

        public string username;
        public string name;
        public string password;
        public string department;
        public string role;
    }

}