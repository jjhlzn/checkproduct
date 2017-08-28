using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Newtonsoft.Json;

namespace checkproduct
{
    public partial class getcheckimages : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            var resp = new
            {
                images = new String[] {

                   "animal.jpg",
                   "test.jpg",
                   "test.jpg",
                    "test.jpg",
                   "test.jpg",
                   "test.jpg",
                    "test.jpg",
                   "test.jpg",
                   "test.jpg",
                    "test.jpg",
                   "test.jpg",
                   "test.jpg",
                    "test.jpg",
                   "test.jpg",
                   "test.jpg",
                    "test.jpg",
                   "test.jpg",
                   "test.jpg",
                    "test.jpg",
                   "test.jpg",
                   "test.jpg",
                }
            };
            Response.Write(JsonConvert.SerializeObject(resp));
            Response.End();
        }
    }
}