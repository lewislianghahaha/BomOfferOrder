using System;
using System.Data;
using BomOfferOrder.DB;

namespace BomOfferOrder.Task
{
    //提交
    public class ImportDt
    {
        DbList dbList=new DbList();
        GenerateDt generateDt=new GenerateDt();

        /// <summary>
        /// 提交数据至数据表
        /// </summary>
        /// <param name="funState">单据状态 R:读取 C:创建</param>
        /// <param name="souredt">Tab Pages收集过来的DT</param>
        /// <param name="deldt">需要删除的记录DT(单据状态为R时使用)</param>
        /// <returns></returns>
        public bool ImportDtToDb(string funState, DataTable souredt,DataTable deldt)
        {
            var result = true;

            try
            {
                //获取三个临时表(分别针对对应的数据表)
                var offerOrderDt = dbList.GetOfferOrderTemp();
                var offerOrderHeadDt = dbList.GetOfferOrderHeadTemp();
                var offerOrderEntryDt = dbList.GetOfferOrderEntryTemp();


                //过滤得出不相同的‘产品名称’临时表

                //循环将sourcedt进行拆分,并放置到三个临时表内
                var dtlrows = souredt.Select("OAorderno='" + sourcerow[1] + "'");








                foreach (DataRow rows in souredt.Rows)
                {
                    //


                    //将数据插入至offerOrderDt临时表
                    offerOrderDt = GetDataToOfferOrderDt(funState, offerOrderDt, rows);
                    //将数据插入至offerOrderHeadDt临时表
                    offerOrderHeadDt.Merge(GetDataToOfferOrderHeadDt(0,funState,offerOrderHeadDt,rows));
                    //将数据插入至offerOrderEntryDt临时表

                }
                var a = offerOrderDt;

            }
            catch (Exception)
            {
                result = false;
            }
            return result;
        }

        /// <summary>
        /// 根据条件将sourerow的数据插入至offerOrderDt临时表内
        /// </summary>
        /// <param name="funState">单据状态</param>
        /// <param name="dt">临时表</param>
        /// <param name="sourcerow"></param>
        private DataTable GetDataToOfferOrderDt(string funState,DataTable dt,DataRow sourcerow)
        {
            //根据‘OA流水号’判断是否已在临时表内存在,若不存在,才进行插入
            if(dt.Select("OAorderno='" + sourcerow[1] + "'").Length==0)
            {
                var newrow = dt.NewRow();
                newrow[0] = funState == "C" ? GetFidKey() :sourcerow[0];  //FID
                newrow[1] = sourcerow[1];                                 //流水号
                newrow[2] = sourcerow[2];                                 //单据状态
                newrow[3] = sourcerow[3];                                 //创建日期
                newrow[4] = sourcerow[4];                                 //记录当前单据使用标记
                newrow[5] = sourcerow[5];                                 //记录当前单据使用者信息
                dt.Rows.Add(newrow);
            }
            return dt;
        }

        /// <summary>
        /// 根据条件将sourerow的数据插入至offerOrderHeadDt临时表内
        /// </summary>
        /// <param name="fid"></param>
        /// <param name="funState"></param>
        /// <param name="dt"></param>
        /// <param name="sourcerow"></param>
        /// <returns></returns>
        private DataTable GetDataToOfferOrderHeadDt(int fid,string funState, DataTable dt, DataRow sourcerow)
        {
            //根据‘产品名称’判断是否已在临时表内存在,若不存在,才进行插入
            if (dt.Select("MaterialName='" + sourcerow[7] + "'").Length == 0)
            {
                var newrow = dt.NewRow();
                newrow[0] = fid;//FID
                newrow[1] = funState=="C" ? GetHeadidKey() : sourcerow[0]; //Headid
                newrow[2] =;//产品名称(物料名称)
                newrow[3] =;//包装规格
                newrow[4] =;//产品密度(KG/L)
                newrow[5] =;//材料成本(不含税)
                newrow[6] =;//包装成本
                newrow[7] =;//人工及制造费用
                newrow[8] =;//成本(元/KG)
                newrow[9] =;//成本(元/L)
                newrow[10] =;//50%报价
                newrow[11] =;//45%报价
                newrow[12] =;//40%报价
                newrow[13] =;//备注
                newrow[14] =;//对应BOM版本编号
                newrow[15] =;//物料单价
                dt.Rows.Add(newrow);
            }
            return dt;
        }

        


        /// <summary>
        /// 获取FID主键值
        /// </summary>
        /// <returns></returns>
        private int GetFidKey()
        {
            return generateDt.GetNewFidValue();
        }

        /// <summary>
        /// 获取Headid主键值
        /// </summary>
        /// <returns></returns>
        public int GetHeadidKey()
        {
            return generateDt.GetNewHeadidValue();
        }

        /// <summary>
        /// 获取最新的Entryid值
        /// </summary>
        /// <returns></returns>
        public int GetEntryidKey()
        {
            return generateDt.GetNewEntryidValue();
        }

    }
}
