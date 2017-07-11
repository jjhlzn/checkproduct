using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Newtonsoft.Json;

namespace checkproduct
{
    public partial class getnotchecklist : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            var resp = new
            {
                status = 0,
                errorMessage = "",
                items = new Object[] {

                    new {
                        id = "1",
                        title = "验货单1",
                        content = "这是一段测试文本"
                    },
                    new {
                        id = "2",
                        title = "验货单2",
                        content = "这是一段测试文本"
                    },
                    new {
                        id = "3",
                        title = "验货单3",
                        content = "这是一段测试文本"
                    },
                }
            };
            Response.Write(JsonConvert.SerializeObject(resp));
            Response.End();
        }
    }
}