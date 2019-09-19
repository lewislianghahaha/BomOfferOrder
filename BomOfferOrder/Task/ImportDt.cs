using System;
using System.Data;
using BomOfferOrder.DB;

namespace BomOfferOrder.Task
{
    //提交
    public class ImportDt
    {
        DbList dbList=new DbList();
        SearchDt seraDt=new SearchDt();
        GenerateDt generateDt=new GenerateDt();

        /// <summary>
        /// 提交数据至数据表
        /// </summary>
        /// <param name="funState"></param>
        /// <param name="souredt"></param>
        /// <returns></returns>
        public bool ImportDtToDb(string funState, DataTable souredt)
        {
            var result = true;
            try
            {

            }
            catch (Exception)
            {
                
                throw;
            }
            return result;
        }

    }
}
