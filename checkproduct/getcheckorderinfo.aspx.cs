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
    public partial class getcheckorderinfo : System.Web.UI.Page
    {
        private CheckOrderService service = new CheckOrderService();

        protected void Page_Load(object sender, EventArgs e)
        {
            string ticketNo = Request.Params["ticketNo"];
            CheckOrder checkOrder = service.GetCheckOrderInfo(ticketNo);

            var resp = new
            {
                status = checkOrder == null ? -1 : 0,
                errorMessage = "",
                checkOrder = checkOrder
            };

            Response.Write(JsonConvert.SerializeObject(resp));
            Response.End();
        }
    }
}