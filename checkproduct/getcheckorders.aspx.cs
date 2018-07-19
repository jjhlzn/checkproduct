using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Newtonsoft.Json;
using checkproduct.DomainModel;
using checkproduct.Service;
using log4net;


namespace checkproduct
{
    public partial class getcheckorders : System.Web.UI.Page
    {
        private CheckOrderService checkOrderService = new CheckOrderService();
        private static ILog logger = LogManager.GetLogger(typeof(getcheckorders));

        protected void Page_Load(object sender, EventArgs e)
        {
            PageInfo pageInfo = new PageInfo();
            string username = Request.Params["username"];

            string ticketNo = Request.Params["ticketNo"] ?? "";
            ticketNo = ticketNo.Trim();
            
            string startDate = Request.Params["startDate"];
            string endDate = Request.Params["endDate"];
            string status = Request.Params["status"];
            string pageNo = Request.Params["pageNo"];
            string pageSize = Request.Params["pageSize"];
            string checker = Request.Params["checker"];

            string str = string.Format(@"ticketNo = {0}, startDate = {1}, endDate = {2}, status = {3}, pageNo = {4}, username = {5}, checker = {6}",
                ticketNo, startDate, endDate, status, pageNo, username, checker);
            logger.Debug("params: " + str);

            pageInfo.pageNo = int.Parse(pageNo);
            pageInfo.pageSize = int.Parse(pageSize);

            GetCheckOrdersResult checkOrdersResult = checkOrderService.GetCheckOrders(DateTime.ParseExact(startDate, "yyyy-MM-dd",
                                       System.Globalization.CultureInfo.InvariantCulture), DateTime.ParseExact(endDate, "yyyy-MM-dd",
                                       System.Globalization.CultureInfo.InvariantCulture), status, username,checker, pageInfo, ticketNo);

            var resp = new
            {
                status = 0,
                errorMessage = "",
                totalCount = checkOrdersResult.totalCount,
                items = checkOrdersResult.checkOrders
            };

            Response.Write(JsonConvert.SerializeObject(resp));
            Response.End();
        }
    }
}