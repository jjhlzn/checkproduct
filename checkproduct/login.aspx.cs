using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Newtonsoft.Json;
using log4net;

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
        private ILog logger = LogManager.GetLogger(typeof(login));

        protected override Object handle(LoginRequest req)
        {
            var resp = new
            {
                status = 0,
                errorMessage = "用户名或密码错误",
                user = new
                {
                    username = "jhjin",
                    name = "金",
                    department = "it"
                }
            };

            return resp;
        }

    }
}