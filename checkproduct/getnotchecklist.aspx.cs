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
                totalCount = 6,
                items = new Object[] {

                    new {
                        ticketNo = "HW17133",  
                        inHourseNo = "abcd123",
                        tracker = "张三",
                        checker = "小周",
                        outDate = "2017-10-01",
                        status = "未验货"
                    },
                    new {
                        ticketNo = "HW17134",
                        inHourseNo = "abcd124",
                        tracker = "张三",
                        checker = "未知",
                        outDate = "2017-10-20",
                        status = "未分配"
                    },
                    new {
                        ticketNo = "HW17135",
                        inHourseNo = "abcd125",
                        tracker = "张三",
                        checker = "小周",
                        outDate = "2017-10-01",
                        status = "未验货"
                    },
                    new {
                        ticketNo = "HW17136",
                        inHourseNo = "abcd126",
                        tracker = "张三",
                        checker = "小李飞刀",
                        outDate = "2017-10-11",
                        status = "未验货"
                    },
                    new {
                        ticketNo = "HW17137",
                        inHourseNo = "abcd125",
                        tracker = "乔峰",
                        checker = "李寻欢",
                        outDate = "2017-10-01",
                        status = "未验货"
                    },
                    new {
                        ticketNo = "HW17138",
                        inHourseNo = "abcd126",
                        tracker = "段誉",
                        checker = "张三丰",
                        outDate = "2017-10-11",
                        status = "未验货"
                    },
                }
            };
            Response.Write(JsonConvert.SerializeObject(resp));
            Response.End();
        }
    }
}