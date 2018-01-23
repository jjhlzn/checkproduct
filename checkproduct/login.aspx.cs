using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Newtonsoft.Json;
using log4net;
using Dapper;
using checkproduct.DomainModel;
using checkproduct.Service;

namespace checkproduct
{
    public class LoginRequest : BaseRequest
    {
        public string a
        {
            get;
            set;
        }

        public string b
        {
            get;
            set;
        }
    }

    public partial class login : BasePage<LoginRequest>
    {
        private UserService userService = new UserService();

        private ILog logger = LogManager.GetLogger(typeof(login));

        protected override Object handle(LoginRequest req)
        {
            User user = userService.Login(req.a, req.b);

            int status = 0;
            string errorMessage = "";
            
            if (user == null)
            {
                status = -1;
                errorMessage = "用户名或密码错误";
            } 

            var resp = new
            {
                status = status,
                errorMessage = errorMessage,
                user = user
            };

            return resp;

        }

    }
}