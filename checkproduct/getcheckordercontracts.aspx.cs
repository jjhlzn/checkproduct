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
    public partial class getcheckordercontracts : System.Web.UI.Page
    {
        private CheckOrderService checkOrderService = new CheckOrderService();

        protected void Page_Load(object sender, EventArgs e)
        {
            string ticketNo = Request.Params["ticketNo"];
            List<CheckOrderContract> contracts = checkOrderService.GetCheckOrderContracts(ticketNo);

            var resp = new
            {
                status = 0,
                errorMessage = "",
                contracts = contracts
            };

            Response.Write(JsonConvert.SerializeObject(resp));
            Response.End();
        }
    }
}