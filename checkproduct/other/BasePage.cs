using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using log4net;
using Newtonsoft.Json;

namespace checkproduct
{
    public class BaseRequest
    {

    }


    public abstract class BasePage<T> : System.Web.UI.Page where T : BaseRequest
    {
        private static ILog logger = LogManager.GetLogger(typeof(BasePage<T>));

        protected void Page_Load(object sender, EventArgs e)
        {
            String requestJson = Request.Params["request"];
            logger.Debug("request:" + requestJson);
            T req = JsonConvert.DeserializeObject<T>(requestJson);
            

            Response.Write(JsonConvert.SerializeObject(handle(req)));
            Response.End();
        }

        protected abstract Object handle(T req);

    }
}