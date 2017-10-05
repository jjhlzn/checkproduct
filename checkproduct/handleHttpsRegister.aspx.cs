using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace checkproduct
{
    public partial class handleHttpsRegister : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            string text = System.IO.File.ReadAllText(MapPath("~/data.txt"));
            Response.Write(text);
            Response.End();
        }
    }
}