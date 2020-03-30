using System;
using System.Data;
using System.Data.SqlClient;
using System.Windows.Forms;
using BomOfferOrder.DB;

namespace BomOfferOrder.Task
{
    //提交‘暂存’数据
    public class ImportTempDt
    {
        SqlList sqlList=new SqlList();
        DbList dbList = new DbList();
        GenerateDt generateDt=new GenerateDt();
        SearchDt searchDt=new SearchDt();

        /// <summary>
        /// 将暂存信息上传-注:只有插入效果
        /// </summary>
        /// <param name="souredt"></param>
        /// <returns></returns>
        public bool ImportTempDtToDb(DataTable souredt)
        {
            var result = true;
            var tableName = string.Empty;

            try
            {
                //获取三个临时表(分别针对对应的数据表)
                var tempOrderDt = dbList.GetTempOrderTemp();
                var tempOrderHeadDt = dbList.GetTempOrderHeadTemp();
                var tempOrderEntryDt = dbList.GetTempOrderEntryTemp();

                //创建产成品名称临时表
                var bomproductorderdt = dbList.CreateBomProductTemp();

                //过滤得出不相同的‘产品名称’临时表
                foreach (DataRow rows in souredt.Rows)
                {
                    if (bomproductorderdt.Select("ProductName='" + rows[10] + "'").Length > 0) continue;
                    var newrow = bomproductorderdt.NewRow();
                    newrow[0] = rows[10];
                    bomproductorderdt.Rows.Add(newrow);
                }

                //将数据插入至TempOrderDt临时表
                tempOrderDt = GetDataToTempOrderDt(tempOrderDt, souredt.Rows[0]);

                foreach (DataRow rows in bomproductorderdt.Rows)
                {
                    var dtlrows = souredt.Select("ProductName='" + rows[0] + "'");
                    //根据‘产品名称’为条件,所获得的数据进行循环插入至对应的临时表内
                    for (var i = 0; i < dtlrows.Length; i++)
                    {
                        //先插入信息至offerOrderHeadDt临时表内
                        tempOrderHeadDt.Merge(GetDataToTempOrderHeadDt(Convert.ToInt32(tempOrderDt.Rows[0][0]), tempOrderHeadDt, dtlrows[i]));
                        //根据‘产品名称’获取对应的Headid值
                        var headid = Convert.ToInt32(tempOrderHeadDt.Select("ProductName='" + rows[0] + "'")[0][1]);
                        //然后再将相关信息插入至offerOrderEntryDt临时表内
                        tempOrderEntryDt.Merge(GetDataToTempOrderEntryDt(headid, tempOrderEntryDt, dtlrows[i]));
                    }
                }

                //将整理好的三个临时表通过循环分别放到‘插入’ ‘更新’方法内执行;
                for (var i = 0; i < 3; i++)
                {
                    switch (i)
                    {
                        //0:T_TempOrder
                        case 0:
                            tableName = "T_TempOrder";
                            ImportDtToDb(tableName, tempOrderDt);
                            break;
                        //1:T_TempOrderHead
                        case 1:
                            tableName = "T_TempOrderHead";
                            ImportDtToDb(tableName, tempOrderHeadDt);
                            break;
                        //2:T_TempOrderEntry
                        case 2:
                            tableName = "T_TempOrderEntry";
                            ImportDtToDb(tableName, tempOrderEntryDt);
                            break;
                    }
                }
            }
            catch (Exception)
            {
                result = false;
            }
            return result;
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
        /// <param name="dt">临时表</param>
        /// <param name="sourcerow"></param>
        private DataTable GetDataToTempOrderDt(DataTable dt, DataRow sourcerow)
        {
            var newrow = dt.NewRow();
            newrow[0] = GetFidKey();           //FID 
            newrow[1] = sourcerow[1];          //流水号
            newrow[2] = sourcerow[2];          //单据状态
            newrow[3] = sourcerow[3];          //创建日期
            newrow[4] = sourcerow[4];          //审核日期
            newrow[5] = sourcerow[5];          //创建人
            newrow[6] = sourcerow[7];          //记录当前单据使用标记
            newrow[7] = sourcerow[8];          //记录当前单据使用者信息
            newrow[8] = sourcerow[6];          //单据类型ID
            dt.Rows.Add(newrow);
            return dt;
        }

        /// <summary>
        /// 根据条件将sourerow的数据插入至TempOrderHeadDt临时表内
        /// </summary>
        /// <param name="fid"></param>
        /// <param name="dt"></param>
        /// <param name="sourcerow"></param>
        /// <returns></returns>
        private DataTable GetDataToTempOrderHeadDt(int fid,DataTable dt, DataRow sourcerow)
        {
            //必须‘产成品名称’不在dt内,才将数据插入
            if (dt.Select("ProductName='" + sourcerow[10] + "'").Length == 0)
            {
                var newrow = dt.NewRow();
                newrow[0] = fid;                                               //FID
                newrow[1] = GetHeadidKey();                                    //Headid 
                newrow[2] = sourcerow[10];                                     //产品名称(物料名称)
                newrow[3] = sourcerow[11];                                     //包装规格
                newrow[4] = sourcerow[12];                                     //产品密度(KG/L)
                newrow[5] = sourcerow[13];                                     //材料成本(不含税)
                newrow[6] = sourcerow[14];                                     //包装成本
                newrow[7] = sourcerow[15];                                     //人工及制造费用
                newrow[8] = sourcerow[16];                                     //成本(元/KG)
                newrow[9] = sourcerow[17];                                     //成本(元/L)
                newrow[10] = sourcerow[18];                                    //50%报价
                newrow[11] = sourcerow[19];                                    //45%报价
                newrow[12] = sourcerow[20];                                    //40%报价
                newrow[13] = sourcerow[21];                                    //备注
                newrow[14] = sourcerow[22];                                    //对应BOM版本编号
                newrow[15] = sourcerow[23];                                    //物料单价
                newrow[16] = sourcerow[24];                                    //客户
                dt.Rows.Add(newrow);
            }
            return dt;
        }

        /// <summary>
        /// 根据条件将sourerow的数据插入至OfferOrderEntryDt临时表内
        /// </summary>
        /// <returns></returns>
        private DataTable GetDataToTempOrderEntryDt(int headid, DataTable dt, DataRow sourcerow)
        {
            var newrow = dt.NewRow();
            newrow[0] = headid;             //Headid
            newrow[1] = GetEntryidKey();    //Entryid
            newrow[2] = sourcerow[26];      //物料编码ID
            newrow[3] = sourcerow[27];      //物料编码
            newrow[4] = sourcerow[28];      //物料名称
            newrow[5] = sourcerow[29];      //配方用量
            newrow[6] = sourcerow[30];      //占比
            newrow[7] = sourcerow[31];      //物料单价(含税)
            newrow[8] = sourcerow[32];      //物料成本(含税)
            newrow[9] = sourcerow[33];      //备注
            dt.Rows.Add(newrow);
            return dt;
        }

        /// <summary>
        /// 获取FID主键值-(注:使用T_OfferOrder_KEY生成)
        /// </summary>
        /// <returns></returns>
        private int GetFidKey()
        {
            return generateDt.GetNewFidValue();
        }

        /// <summary>
        /// 获取Headid主键值-(注:使用T_OfferOrderHead_KEY生成)
        /// </summary>
        /// <returns></returns>
        private int GetHeadidKey()
        {
            return generateDt.GetNewHeadidValue();
        }

        /// <summary>
        /// 获取最新的Entryid值-(注:使用T_OfferOrderEntry_KEY生成)
        /// </summary>
        /// <returns></returns>
        private int GetEntryidKey()
        {
            return generateDt.GetNewEntryidValue();
        }

        /// <summary>
        /// 删除相关暂存表记录
        /// </summary>
        /// <param name="delvalue"></param>
        public bool DeleteRecord(string delvalue)
        {
            var result = true;
            try
            {
                searchDt.Generdt(sqlList.DelTempOrder(delvalue));
            }
            catch (Exception)
            {
                result = false;
            }
            return result;
        }

    }
}
