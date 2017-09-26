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
    public class GetCheckListRequest : BaseRequest
    {
        public String startDate
        {
            get;
            set;
        }

        public String endDate
        {
            set;
            get;
        }

        public String ticketNo
        {
            set;
            get;
        }

        public bool hasChecked
        {
            set;
            get;
        }


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


    public partial class getchecklist : BasePage<GetCheckListRequest>
    {
        private ILog logger = LogManager.GetLogger(typeof(getchecklist));


        protected override Object handle(GetCheckListRequest req)
        {
            Object[] data = new Object[req.pageSize];


            for (int i = 0; i < req.pageSize; i++)
            {
                data[i] = makeCheckItem(req.pageNo * req.pageSize + i);
            }
            System.Threading.Thread.Sleep(500);
            var resp = new
            {
                status = 0,
                errorMessage = "",
                totalCount = 20,
                items = data

            };

            return resp;
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