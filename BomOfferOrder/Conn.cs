using System.Configuration;

namespace BomOfferOrder
{
    public class Conn
    {
        /// <summary>
        /// 获取连接字符串
        /// </summary>
        /// <param name="connid">0:读取K3-Cloud正式库,当为1:读取BomOffer库</param>
        /// <returns></returns>
        public string GetConnectionString(int connid)
        {
            var strcon = string.Empty;

            if (connid == 0)
            {
                //读取App.Config配置文件中的Connstring节点    
                var pubs = ConfigurationManager.ConnectionStrings["Connstring"];
                strcon = pubs.ConnectionString;
            }
            else
            {
                //读取App.Config配置文件中的Connstring节点    
                var pubs = ConfigurationManager.ConnectionStrings["ConnstringBom"];
                strcon = pubs.ConnectionString;
            }

            #region Hide
            //var consplit = pubs.ConnectionString.Split(';');

            //var strcon = string.Format(consplit[0], "192.168.1.250") + ";" + string.Format(consplit[1], "RD") + ";" +
            //             consplit[2] + ";" + string.Format(consplit[3], "sa") + ";" +
            //             string.Format(consplit[4], "8990489he") + ";" + consplit[5] + ";" + consplit[6] + ";" + consplit[7];

            //var conn = new SqlConnection(strcon);
            //return conn;
            #endregion

            return strcon;
        }
    }
}
