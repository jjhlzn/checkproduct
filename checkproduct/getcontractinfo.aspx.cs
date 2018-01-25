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
    public partial class getcontractinfo : System.Web.UI.Page
    {
        private CheckOrderService checkOrderService = new CheckOrderService();
        protected void Page_Load(object sender, EventArgs e)
        {
            string ticketNo = Request.Params["ticketNo"];
            string contractNo = Request.Params["contractNo"];
            CheckOrderContract contract = checkOrderService.GetContractInfo(ticketNo, contractNo);

            int status = 0;
            if (contract == null)
            {
                status = -1;
            }

            var resp = new
            {
                status = status,
                errorMessage = "",
                contract = contract
            };

            Response.Write(JsonConvert.SerializeObject(resp));
            Response.End();
        }
    }
}