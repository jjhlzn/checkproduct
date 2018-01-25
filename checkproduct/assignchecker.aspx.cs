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
    public partial class assignchecker : System.Web.UI.Page
    {
        private CheckOrderService checkOrderService = new CheckOrderService();

        protected void Page_Load(object sender, EventArgs e)
        {
            string ticketNo = Request.Params["ticketNo"];
            string checker = Request.Params["checker"];
            bool result = checkOrderService.AssignChecker(ticketNo, checker);

            var resp = new
            {
                status = result ? 0 : -1,
                errorMessage = ""
            };

            Response.Write(JsonConvert.SerializeObject(resp));
            Response.End();
        }
    }
}