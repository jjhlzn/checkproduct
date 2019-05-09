using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using checkproduct.Service;
using log4net;

namespace checkproduct
{
    public partial class productpicture : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            string sphh_kh = Request.Params["id"];
            string ticketNo = Request.Params["ticketNo"];
            string spbm = Request.Params["spbm"];
            byte[] image = new CheckOrderService().GetProductImage(ticketNo, sphh_kh, spbm);
            Response.ContentType = "image";
            Response.OutputStream.Write(image, 0, image.Length);
        }
    }
}