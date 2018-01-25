using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using checkproduct.DomainModel;
using checkproduct.Service;
using Newtonsoft.Json;

namespace checkproduct
{
    public partial class getallcheckers : System.Web.UI.Page
    {
        private UserService userService = new UserService();

        protected void Page_Load(object sender, EventArgs e)
        {
            List<User> users = userService.GetAllCheckers();

            var resp = new
            {
                status = 0,
                errorMessage = "",
                checkers = users
            };

            Response.Write(JsonConvert.SerializeObject(resp));
            Response.End();
       }

    }
}