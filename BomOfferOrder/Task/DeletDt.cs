using BomOfferOrder.DB;

namespace BomOfferOrder.Task
{
    public class DeletDt
    {
        SqlList sqlList=new SqlList();
        SearchDt searchDt=new SearchDt();

        /// <summary>
        /// 删除单据记录
        /// </summary>
        /// <param name="fidlist"></param>
        /// <returns></returns>
        public bool DeleteOrderRecord(string fidlist)
        {
            var sqlscript = sqlList.Delorder(fidlist);
            return searchDt.Generdt(sqlscript);
        }

        /// <summary>
        /// 删除暂存单据记录
        /// </summary>
        /// <param name="fidlist"></param>
        /// <returns></returns>
        public bool DeleteTempOrderRecord(string fidlist)
        {
            var sqlscript = sqlList.Deltemporder(fidlist);
            return searchDt.Generdt(sqlscript);
        }
    }
}
