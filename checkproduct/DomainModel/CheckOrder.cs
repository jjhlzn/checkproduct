using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace checkproduct.DomainModel
{
    public class CheckOrder
    {
        public static string Status_All = "全部";
        public static string Status_Not_Assign = "未分配";
        public static string Status_Not_Check = "未验货";
        public static string Status_Not_Complete = "未完成";
        public static string Status_Has_Checked = "已验货";

        public string ticketNo;
        public string tracker; //业务员
        public string checker;
        public string outDate;
        public string status;
        public string jinCangNo; //进仓编号
        public string announcements;

        public int productCount; //验货单的所有合同下面的货物的种数之和
        public int checkedProductCount; //验货单的所有合同下面的货物的种数之和, 并且已经验货的货物种数之和

        public int qualifiedCount;
        public int notQualifiedCount;
        public int notCheckCount;
        public int tbdCount;
        public int hasZlkImageProductCount;

        public string checkMemo; //验货备注
        public string checkResult;
        public string checkTime;
        public List<string> pictureUrls = new List<string>(); //验货图片链接
    }

    public class CheckOrderContract
    {
        public string ticketNo;
        public string contractNo;
        public string jinCangNo;
        public string tracker; //跟单员
        public string checker;
        public string deadlineDate;
        public int productCount;
        public int checkedProductCount;
        public List<Product> products = new List<Product>();
    }

    public class Product
    {
        public string ticketNo;
        public string contractNo;
        public string spid;
        public string productNo;
        public string sphh; //客户货号
        public string spbm;
        public string name;
        public string totalCount = "0"; //总箱数
        public string pickCount; //抽箱数
        public string boxSize; //外箱尺寸
        public string grossWeight; //单件毛重
        public string netWeight; //单件净重
        public string checkMemo; //验货备注
        public string checkResult;
        public string checkTime;
        public List<string> pictureUrls = new List<string>(); //验货图片链接
        public List<string> zlkUrls = new List<string>(); //资料库图片链接
        public List<string> productUrls = new List<string>(); //商品图片链接
        public int zlkImagesCount;

        public string description;
        public string package;

        public string tracker;
        public string checker;
    }

    public class CheckProductResult
    {
        public string checkResult;

        public string pickCount; //抽箱数
        public string boxSize; //外箱尺寸
        public string grossWeight; //单件毛重
        public string netWeight; //单件净重
        public string checkMemo; //验货备注

        public List<string> addImages = new List<string>(); //验货图片链接
        public List<string> deleteImages = new List<string>();

        public override string ToString()
        {
            string result = "checkResult = {0}, pickCount = {1}, boxSize = {2}, grossWeight = {3}, netWeight = {4}, checkMemo = {5}";

            return string.Format(result, checkResult, pickCount, boxSize, grossWeight, netWeight, checkMemo);
        }
    }

}