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

            string sql = @"select top "+ pageInfo.pageSize + @" yw_mxd.mxdbh as ticketNo, yw_mxd_yhsqd.jcbh, 
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

                sql = "select count(*) from  yw_mxd with (nolock) ,yw_mxd_yhsqd where " + whereClause;
                result.totalCount = conn.QuerySingleOrDefault<int>(sql);
                return result;
            }
            
        }

        public bool AssignChecker(string ticketNo, string checker)
        {
            string sql = "update yw_mxd_yhsqd set yhy = '{0}' where mxdbh = '{1}'";
            sql = string.Format(sql, checker, ticketNo);
            logger.Debug("sql: " + sql);

            using (IDbConnection conn = ConnectionFactory.GetInstance())
            {
                //conn.Execute(sql);
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
            string sql = @"select distinct spzwmc as name, spbm as productNo, '待验货' as checkResult from yw_mxd_cmd where sghth = '{0}'";
            sql = string.Format(sql, contractNo);
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
            string sql = @"select bzjs_cy as pickCount, yhjg as checkResult, yw_mxd_cmd.djtjms as boxSize, 
                            yw_mxd_cmd.mjmz as grossWeight, yw_mxd_cmd.mjjz as netWeight, yw_mxd_cmd.yhms as checkMemo 
                            from yw_mxd_cmd where sghth = '{0}' and spbm = '{1}'";
            sql = string.Format(sql, contractNo, productNo);
            logger.Debug("sql: " + sql);

            using (IDbConnection conn = ConnectionFactory.GetInstance())
            {
                Product product = conn.QueryFirstOrDefault<Product>(sql);
                return product;
            }
        }
    }
}