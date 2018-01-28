using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using checkproduct.DomainModel;
using checkproduct.Service;
using Newtonsoft.Json;
using log4net;

namespace checkproduct
{
    public partial class getproducts : System.Web.UI.Page
    {
        private CheckOrderService service = new CheckOrderService();

        protected void Page_Load(object sender, EventArgs e)
        {
            string ticketNo = Request.Params["ticketNo"];
            string type = Request.Params["checkResult"];

            List<Product> products = service.GetProducts(ticketNo, type);

            var resp = new
            {
                status = 0,
                errorMessage = "",
                products = products
            };

            Response.Write(JsonConvert.SerializeObject(resp));
            Response.End();
        }
    }
}