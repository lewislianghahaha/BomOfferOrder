using System;
using System.Data;
using BomOfferOrder.DB;

namespace BomOfferOrder.Task
{
    public class GenerateDb
    {
        SqlList sqlList=new SqlList();
        DbList dbList=new DbList();

        /// <summary>
        /// 运算-(将‘已添加’的物料记录生成其对应的BOM明细记录)
        /// </summary>
        /// <param name="sourcedt"></param>
        /// <returns></returns>
        public DataTable Generatedt(DataTable sourcedt)
        {
            var result=new DataTable();
            try
            {

            }
            catch (Exception)
            {
                result.Rows.Clear();
                result.Columns.Clear();
            }
            return result;
        }


    }
}
