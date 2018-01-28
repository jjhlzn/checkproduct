using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data;
using Dapper;
using checkproduct.DomainModel;
using log4net;

namespace checkproduct.Service
{
    public class GetCheckOrdersResult
    {
        public List<CheckOrder> checkOrders;
        public int totalCount;
    }
    public class CheckOrderService
    {
        private static ILog logger = LogManager.GetLogger(typeof(CheckOrderService));

        public GetCheckOrdersResult GetCheckOrders(DateTime startDate, DateTime endDate, string status, string username, PageInfo pageInfo, string keyword = "")
        {
            int skipCount = pageInfo.pageSize * pageInfo.pageNo;
            string whereClause = @"  ( yw_mxd.mxdbh = yw_mxd_yhsqd.mxdbh ) and  
                                     ( yw_mxd.bbh = yw_mxd_yhsqd.bbh ) and
                                     ( yw_mxd.bb_flag = 'Y' ) ";

            if (status == CheckOrder.Status_Not_Assign)
            {
                whereClause += " and (yw_mxd_yhsqd.yhy is null or yw_mxd_yhsqd.yhy = '')";
            } else
            {
                whereClause += " and (yw_mxd_yhsqd.yhy is not null and yw_mxd_yhsqd.yhy != '')";
            }

            string sql = "";
            
            sql = @"select top "+ pageInfo.pageSize + @" yw_mxd.mxdbh as ticketNo, yw_mxd_yhsqd.jcbh, 
                            (select name from rs_employee where e_no = yw_mxd.zdr) as tracker, 
                            (select name from rs_employee where e_no = yw_mxd_yhsqd.yhy) as checker, 
                            yw_mxd_yhsqd.yjckrq as outDate, yw_mxd_yhsqd.yhrq as checkDate
                                FROM yw_mxd with (nolock) ,yw_mxd_yhsqd      
                                WHERE " + whereClause + @" and yw_mxd.mxdbh not in (select top "+ skipCount + @" yw_mxd.mxdbh from yw_mxd with (nolock), yw_mxd_yhsqd

                              where " + whereClause + @" order by yw_mxd.mxdbh ) order by yw_mxd.mxdbh";


            logger.Debug("sql: " + sql);

            using (IDbConnection conn = ConnectionFactory.GetInstance())
            {
                var orders = conn.Query<CheckOrder>(sql);
                foreach(CheckOrder order in orders)
                {
                    order.status = status;
                }
                GetCheckOrdersResult result = new GetCheckOrdersResult();
                result.checkOrders = orders.AsList<CheckOrder>();

                if (status == CheckOrder.Status_Has_Checked)
                {
                    SetCheckResultStatus(result.checkOrders);
                }

                sql = "select count(*) from  yw_mxd with (nolock) ,yw_mxd_yhsqd where " + whereClause;
                result.totalCount = conn.QuerySingleOrDefault<int>(sql);
                return result;
            }
        }


        private class C
        {
            public string ticketNo;
            public string checkResult;
            public int checkCount = 0;
        }
        private void SetCheckResultStatus(List<CheckOrder> orders)
        {
            if (orders.Count == 0)
                return;

            string ordersStr = "(";
            foreach(CheckOrder order in orders)
            {
                ordersStr += string.Format("'{0}',", order.ticketNo);
            }
            ordersStr = ordersStr.Substring(0, ordersStr.Length - 1);
            ordersStr += ")";
            using (IDbConnection conn = ConnectionFactory.GetInstance())
            {
                string sql = @"select mxdbh as ticketNo, yhjg as checkResult, COUNT(*) as checkCount from (select distinct mxdbh, spbm, yhjg from yw_mxd_cmd 
                               where mxdbh in " + ordersStr + " ) as a group by yhjg, mxdbh";

                var resultSet = conn.Query<C>(sql);
                foreach(C result in resultSet)
                {
                    foreach(CheckOrder order in orders)
                    {
                        if (order.ticketNo == result.ticketNo)
                        {
                            if (string.IsNullOrEmpty(result.checkResult)) {
                                order.notCheckCount += result.checkCount;
                            } else
                            {
                                switch(result.checkResult)
                                {
                                    case "合格":
                                        order.qualifiedCount = result.checkCount;
                                        break;
                                    case "不合格":
                                        order.notQualifiedCount = result.checkCount;
                                        break;
                                    case "未验":
                                        order.notCheckCount += result.checkCount;
                                        break;
                                    case "待定":
                                        order.tbdCount += result.checkCount;
                                        break;
                                }
                            }
                            break;
                        }
                    }
                }

            }
        }

        public bool AssignChecker(string ticketNo, string checker)
        {
            string sql = "update yw_mxd_yhsqd set yhy = '{0}' where mxdbh = '{1}'";
            sql = string.Format(sql, checker, ticketNo);
            logger.Debug("sql: " + sql);

            using (IDbConnection conn = ConnectionFactory.GetInstance())
            {
                conn.Execute(sql);
                return true;
            }
        }  

        public List<CheckOrderContract> GetCheckOrderContracts(string ticketNo)
        {
            string sql = @"select distinct sghth as contractNo, 
                        (select name from rs_employee where e_no in (select top 1 yhy from yw_mxd_yhsqd where mxdbh = '{0}')) as checker,
                        (select name from rs_employee where e_no in (select top 1 zdr from yw_mxd where mxdbh = '{0}')) as tracker
                          from yw_mxd_cmd where mxdbh = '{0}'";
            sql = string.Format(sql, ticketNo);
            logger.Debug("sql: " + sql);

            using (IDbConnection conn = ConnectionFactory.GetInstance())
            {
                var contracts = conn.Query<CheckOrderContract>(sql);
                return contracts.AsList<CheckOrderContract>();
            }
        }

        public List<Product> GetContractProducts(string ticketNo, string contractNo)
        {
            string sql = @"select distinct spzwmc as name, spbm as productNo, yhjg as checkResult from yw_mxd_cmd where sghth = '{0}'";
            sql = string.Format(sql, contractNo);
            logger.Debug("sql: " + sql);

            using (IDbConnection conn = ConnectionFactory.GetInstance())
            {
                var products = conn.Query<Product>(sql);
                return products.AsList<Product>();
            }
            
        }

        public List<Product> GetProducts(string ticketNo, string type)
        {
            
            string sql = @"select distinct mxdbh as ticketNo, spzwmc as name, spbm as productNo, yhjg as checkResult, sghth as contractNo,
                           (select name from rs_employee where e_no = (select top 1 zdr from yw_mxd where mxdbh = '" + ticketNo + @"')) as tracker, 
                           (select name from rs_employee where e_no = (select top 1 yhy from yw_mxd_yhsqd where mxdbh = '" + ticketNo + @"')) as checker
                           from yw_mxd_cmd where mxdbh = '{0}' ";
            if (type != "全部")
            {
                sql += " and yhjg = '" + type + "' ";
            }
            sql = string.Format(sql, ticketNo);
            logger.Debug("sql: " + sql);

            using (IDbConnection conn = ConnectionFactory.GetInstance())
            {
                var products = conn.Query<Product>(sql);
                return products.AsList<Product>();
            }

        }


        public CheckOrderContract GetContractInfo(string ticketNo, string contractNo)
        {
            CheckOrderContract contract = new CheckOrderContract();

            string sql = @"select yw_mxd.mxdbh as ticketNo, 
                           (select name from rs_employee where e_no = yw_mxd.zdr) as tracker, 
                           (select name from rs_employee where e_no = yw_mxd_yhsqd.yhy) as checker, 
                            yw_mxd_yhsqd.jcbh as jinCangNo, yjckrq as deadlineDate from yw_mxd, yw_mxd_yhsqd 
                            where yw_mxd.mxdbh = yw_mxd_yhsqd.mxdbh and yw_mxd.mxdbh = '{0}'";

            sql = string.Format(sql, ticketNo);
            logger.Debug("sql: " + sql);

            using (IDbConnection conn = ConnectionFactory.GetInstance())
            {
                contract = conn.QueryFirstOrDefault<CheckOrderContract>(sql);
                if (contract == null)
                {
                    return null;
                }
            }

            List<Product> products = GetContractProducts(ticketNo, contractNo);
            contract.products = products;
            contract.ticketNo = ticketNo;
            contract.contractNo = contractNo;
            
            return contract;
        }

        public Product GetProductInfo(string contractNo, string productNo)
        {
            string sql = @"select mxdbh as ticketNo, mxd_spid as spid, bzjs_cy as pickCount, yhjg as checkResult, yw_mxd_cmd.djtjms as boxSize, 
                            yw_mxd_cmd.mjmz as grossWeight, yw_mxd_cmd.mjjz as netWeight, yw_mxd_cmd.yhms as checkMemo 
                            from yw_mxd_cmd where sghth = '{0}' and spbm = '{1}'";
            sql = string.Format(sql, contractNo, productNo);
            logger.Debug("sql: " + sql);

            using (IDbConnection conn = ConnectionFactory.GetInstance())
            {
                Product product = conn.QueryFirstOrDefault<Product>(sql);

                if (product != null) {
                    //获取验货的图片
                    sql = @"select picture_sourcefile from yw_mxd_yhmx_picture where mxdbh = '{0}' and mxd_spid = '{1}' order by sqrq ";
                    sql = string.Format(sql, product.ticketNo, product.spid);

                    var pictureUrls = conn.Query<string>(sql);
                    product.pictureUrls = pictureUrls.AsList<string>();
                }
                return product;
            }
        }

        public bool CheckProduct(string ticketNo, string contractNo, string productNo, CheckProductResult checkResult)
        {
            logger.Debug("check product is called");
            //设置值
            string sql = @"update yw_mxd_cmd set bzjs_cy = '{0}', yhjg = '{1}', djtjms = '{2}', mjmz = '{3}', mjjz = '{4}', yhms = '{5}'
                            where sghth = '{6}' and spbm = '{7}'";

            sql = string.Format(sql, checkResult.pickCount, checkResult.checkResult, checkResult.boxSize, checkResult.grossWeight,
                checkResult.netWeight, checkResult.checkMemo, contractNo, productNo);
            logger.Debug("sql: " + sql);

            string spid = "";
            using (IDbConnection conn = ConnectionFactory.GetInstance())
            {
                conn.Execute(sql);

                //获取mxd_spid
                sql = @"select mxd_spid from yw_mxd_cmd where sghth = '{0}' and spbm = '{1}'";
                sql = string.Format(sql, contractNo, productNo);
                logger.Debug("sql: " + sql);

                var result = conn.Query<string>(sql);
                if (result.Count<string>() == 0)
                {
                    return false;
                }
                spid = result.First<string>();

                //设置图片
                foreach (string url in checkResult.addImages)
                {
                    sql = @"insert into yw_mxd_yhmx_picture (mxdbh, bbh, mxd_spid, sqrq, picture_filepath, picture_sourcefile, picture_lx, picture_xz)
                        values ('{0}', '{1}', '{2}', '{3}', '{4}',  '{5}', '{6}', '{7}')";
                    sql = string.Format(sql, ticketNo, 1, spid, DateTime.Now, url, url, "辅图", 1);
                    logger.Debug("sql: " + sql);

                    conn.Execute(sql);
                }

                foreach (string url in checkResult.deleteImages)
                {
                    sql = @"delete from yw_mxd_yhmx_picture where picture_filepath = '{0}' and mxdbh = '{1}'";
                    int index = url.IndexOf("uploads/");
                    string fileName = url;
                    if (index != -1)
                    {
                        fileName = url.Substring(index + "uploads/".Length);
                    }
                    sql = string.Format(sql, fileName, ticketNo);
                    logger.Debug("sql: " + sql);

                    conn.Execute(sql);
                }

                return true;
            }
           
        }
    }
}