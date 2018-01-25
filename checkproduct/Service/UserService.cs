using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Dapper;
using checkproduct.DomainModel;
using System.Data;
using log4net;

namespace checkproduct.Service
{
    public class UserService
    {
        private static ILog logger = LogManager.GetLogger(typeof(UserService));

        public User Login(string userName, string password)
        {
            using (IDbConnection conn = ConnectionFactory.GetInstance())
            {
                string sql = "select e_no as username, name from rs_employee where e_no = @userName";
                var users = conn.Query<User>(sql, new { userName });

                if (users.Count<User>() == 0)
                    return null;

                User user = users.First<User>();

                sql = @"select role_no from t_operator_role_base where o_no = '{0}' and qy_flag = 'Y' 
                        and role_no like 'yh%' order by role_no desc";
                sql = string.Format(sql, userName);
                logger.Debug("sql: " + sql);
                var roleNames = conn.Query<string>(sql);

                if (roleNames.Count<string>() == 0)
                {
                    logger.Debug("userName: " + userName + " 找不到任何角色");
                    return null;
                }

                foreach(string roleName in roleNames)
                {
                    if (roleName == "yhzz")
                    {
                        user.role = User.Role_Checker_Manager;
                        break;
                    }

                    if (roleName == "yhy")
                    {
                        user.role = User.Role_Checker;
                        break;
                    }
                }

                return user;
            }
        }

        public List<User> GetAllCheckers()
        {
            string sql = @"select e_no as username, name from rs_employee where e_no in (select o_no from t_operator_role_base where role_no = 'yhy')";
            using (IDbConnection conn = ConnectionFactory.GetInstance())
            {
                var checkers = conn.Query<User>(sql);
                return checkers.AsList<User>();
            }
        }
    }
}