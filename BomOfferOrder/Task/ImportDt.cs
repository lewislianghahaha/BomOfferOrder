using System;
using System.Data;
using System.Data.SqlClient;
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
            var tableName = string.Empty;

            try
            {
                //获取三个临时表(分别针对对应的数据表)
                var offerOrderDt = dbList.GetOfferOrderTemp();
                var offerOrderHeadDt = dbList.GetOfferOrderHeadTemp();
                var offerOrderEntryDt = dbList.GetOfferOrderEntryTemp();
                //创建产成品名称临时表
                var bomproductorderdt = dbList.CreateBomProductTemp();

                //过滤得出不相同的‘产品名称’临时表
                foreach (DataRow rows in souredt.Rows)
                {
                    if(bomproductorderdt.Select("ProductName='" + rows[7] + "'").Length>0) continue;
                    var newrow = bomproductorderdt.NewRow();
                    newrow[0] = rows[7];
                    bomproductorderdt.Rows.Add(newrow);
                }

                //将数据插入至offerOrderDt临时表
                offerOrderDt = GetDataToOfferOrderDt(funState,offerOrderDt, souredt.Rows[0]);

                foreach (DataRow rows in bomproductorderdt.Rows)
                {
                    var dtlrows = souredt.Select("ProductName='" + rows[0] + "'");
                    //根据‘产品名称’为条件,所获得的数据进行循环插入至对应的临时表内
                    for (var i = 0; i < dtlrows.Length; i++)
                    {
                        //先插入信息至offerOrderHeadDt临时表内
                        offerOrderHeadDt.Merge(GetDataToOfferOrderHeadDt(Convert.ToInt32(offerOrderDt.Rows[0][0]),funState,offerOrderHeadDt, dtlrows[i]));
                        //根据‘产品名称’获取对应的Headid值
                        var headid = Convert.ToInt32(offerOrderHeadDt.Select("ProductName='" + rows[0] + "'")[0][1]);
                        //然后再将相关信息插入至offerOrderEntryDt临时表内
                        offerOrderEntryDt.Merge(GetDataToOfferOrderEntryDt(headid,offerOrderEntryDt, dtlrows[i]));
                    }
                }

                //将整理好的三个临时表通过循环分别放到‘插入’ ‘更新’方法内执行;
                for (var i = 0; i < 3; i++)
                {
                    switch (i)
                    {
                        //0:T_OfferOrder
                        case 0:
                            tableName = "T_OfferOrder";
                            GetDtToDb(funState,tableName,offerOrderDt);
                            break;
                        //1:T_OfferOrderHead
                        case 1:
                            tableName = "T_OfferOrderEntry";
                            GetDtToDb(funState,tableName,offerOrderHeadDt);
                            break;
                        //2:T_OfferOrderEntry
                        case 2:
                            tableName = "T_OfferOrderEntry";
                            GetDtToDb(funState,tableName,offerOrderEntryDt);
                            break;
                    }
                }
                //最后若_deldt有值的话都执行删除方法
                if(deldt.Rows.Count>0)
                { }
            }
            catch (Exception)
            {
                result = false;
            }
            return result;
        }

        /// <summary>
        /// 根据指定的表名等相关信息,按情况执行插入或其它操作
        /// </summary>
        /// <param name="funState">单据状态</param>
        /// <param name="tabname">表名</param>
        /// <param name="tempdt">对应临时表</param>
        private void GetDtToDb(string funState,string tabname,DataTable tempdt)
        {
            //
            
        }

        /// <summary>
        /// 针对指定表进行数据插入
        /// </summary>
        /// <param name="tableName"></param>
        /// <param name="dt"></param>
        private void ImportDtToDb(string tableName, DataTable dt)
        {
            var conn = new Conn();
            var sqlcon = conn.GetConnectionString(1);
            // sqlcon.Open(); 若返回一个SqlConnection的话,必须要显式打开 
            //注:1)要插入的DataTable内的字段数据类型必须要与数据库内的一致;并且要按数据表内的字段顺序 2)SqlBulkCopy类只提供将数据写入到数据库内
            using (var sqlBulkCopy = new SqlBulkCopy(sqlcon))
            {
                sqlBulkCopy.BatchSize = 1000;                    //表示以1000行 为一个批次进行插入
                sqlBulkCopy.DestinationTableName = tableName;  //数据库中对应的表名
                sqlBulkCopy.NotifyAfter = dt.Rows.Count;      //赋值DataTable的行数
                sqlBulkCopy.WriteToServer(dt);               //数据导入数据库
                sqlBulkCopy.Close();                        //关闭连接 
            }
            // sqlcon.Close();
        }




        /// <summary>
        /// 根据条件将sourerow的数据插入至offerOrderDt临时表内
        /// </summary>
        /// <param name="funState"></param>
        /// <param name="dt">临时表</param>
        /// <param name="sourcerow"></param>
        private DataTable GetDataToOfferOrderDt(string funState,DataTable dt,DataRow sourcerow)
        {
            var newrow = dt.NewRow();
            newrow[0] = funState == "C" ? GetFidKey() : sourcerow[0];  //FID 
            newrow[1] = sourcerow[1];                                  //流水号
            newrow[2] = sourcerow[2];                                  //单据状态
            newrow[3] = sourcerow[3];                                  //创建日期
            newrow[4] = sourcerow[4];                                  //记录当前单据使用标记
            newrow[5] = sourcerow[5];                                  //记录当前单据使用者信息
            dt.Rows.Add(newrow);
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
            //必须‘产成品名称’不在dt内,才将数据插入
            if (dt.Select("ProductName='"+ sourcerow[7] +"'").Length == 0)
            {
                var newrow = dt.NewRow();
                newrow[0] = fid;                                               //FID
                newrow[1] = funState == "C" ? GetHeadidKey() : sourcerow[6];   //Headid 
                newrow[2] = sourcerow[7];                                      //产品名称(物料名称)
                newrow[3] = sourcerow[8];                                      //包装规格
                newrow[4] = sourcerow[9];                                      //产品密度(KG/L)
                newrow[5] = sourcerow[10];                                     //材料成本(不含税)
                newrow[6] = sourcerow[11];                                     //包装成本
                newrow[7] = sourcerow[12];                                     //人工及制造费用
                newrow[8] = sourcerow[13];                                     //成本(元/KG)
                newrow[9] = sourcerow[14];                                     //成本(元/L)
                newrow[10] = sourcerow[15];                                    //50%报价
                newrow[11] = sourcerow[16];                                    //45%报价
                newrow[12] = sourcerow[17];                                    //40%报价
                newrow[13] = sourcerow[18];                                    //备注
                newrow[14] = sourcerow[19];                                    //对应BOM版本编号
                newrow[15] = sourcerow[20];                                    //物料单价
                dt.Rows.Add(newrow);
            }
            return dt;
        }

        /// <summary>
        /// 根据条件将sourerow的数据插入至OfferOrderEntryDt临时表内
        /// </summary>
        /// <returns></returns>
        private DataTable GetDataToOfferOrderEntryDt(int headid,DataTable dt,DataRow sourcerow)
        {
            var newrow = dt.NewRow();
            newrow[0] = headid;             //Headid
            newrow[1] = sourcerow[21];      //Entryid
            newrow[2] = sourcerow[22];      //物料编码ID
            newrow[3] = sourcerow[23];      //物料编码
            newrow[4] = sourcerow[24];      //物料名称
            newrow[5] = sourcerow[25];      //配方用量
            newrow[6] = sourcerow[26];      //物料单价(含税)
            newrow[7] = sourcerow[27];      //物料成本(含税)
            dt.Rows.Add(newrow);
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
