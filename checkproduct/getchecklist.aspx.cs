using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Newtonsoft.Json;

namespace checkproduct
{
    public partial class getchecklist : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            var resp = new
            {
                items = new Object[] {

                    new {
                        id = "1",
                        title = "已验货单1",
                        content = "这是一段测试文本",
                        checkResult = true
                    },
                    new {
                        id = "2",
                        title = "已验货单2",
                        content = "这是一段测试文本",
                        checkResult = false
                    },
                    new {
                        id = "3",
                        title = "已验货单3",
                        content = "这是一段测试文本",
                        checkResult = true
                    },
                }
            };
            Response.Write(JsonConvert.SerializeObject(resp));
            Response.End();
        }
    }
}