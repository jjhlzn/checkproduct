using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using log4net;
using Newtonsoft.Json;
using System.Threading;

namespace checkproduct
{
    public partial class getchecklist : System.Web.UI.Page
    {
        private ILog logger = LogManager.GetLogger(typeof(getchecklist));

        class GetCheckListRequest
        {
            public int pageNo
            {
                get;
                set;
            }

            public int pageSize
            {
                get;
                set;
            }


        }

        protected void Page_Load(object sender, EventArgs e)
        {
            String requestJson = Request.Params["request"];
            GetCheckListRequest req = JsonConvert.DeserializeObject<GetCheckListRequest>(requestJson);
            logger.Debug("requestJson:" + requestJson);
            logger.Debug("pageNo = " + req.pageNo + ", pageSize = " + req.pageSize );

            Object[] data = new Object[req.pageSize];
            for (int i = 0; i < req.pageSize; i++)
            {
                data[i] = makeCheckItem(req.pageNo * req.pageSize + i); 
            }
            System.Threading.Thread.Sleep(1000);
            var resp = new
            {
                status = 0,
                errorMessage = "",
                totalCount = 20,
                items = data
                
            };
            Response.Write(JsonConvert.SerializeObject(resp));
            Response.End();
        }


        private Object makeCheckItem(int i)
        {
            return
            new
            {
                ticketNo = "00000000" + i,
                inHourseNo = "00000000" + i,
                tracker = "张三",
                checker = "李四",
                outDate = "2017-10-11",
                status = "已验货"
            };
        }
    }
}