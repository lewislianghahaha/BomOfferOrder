using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using BomOfferOrder.DB;
using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel;

namespace BomOfferOrder.Task
{
    //提交数据 及 导入EXCEL使用
    public class ImportDt
    {
        SqlList sqlList=new SqlList();
        DbList dbList=new DbList();
        GenerateDt generateDt=new GenerateDt();
        SearchDt searchDt=new SearchDt();

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
                    if(bomproductorderdt.Select("ProductName='" + rows[10] + "'").Length>0) continue;
                    var newrow = bomproductorderdt.NewRow();
                    newrow[0] = rows[10];
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
                            tableName = "T_OfferOrderHead";
                            GetDtToDb(funState,tableName,offerOrderHeadDt);
                            break;
                        //2:T_OfferOrderEntry
                        case 2:
                            tableName = "T_OfferOrderEntry";
                            GetDtToDb(funState,tableName,offerOrderEntryDt);
                            break;
                    }
                }
                //最后若deldt有值的话都执行删除方法
                if(deldt.Rows.Count>0)
                    DeleteRecord(deldt);
            }
            catch (Exception)
            {
                result = false;
            }
            return result;
        }

        /// <summary>
        /// 根据指定的表名等相关信息,按情况执行插入或更新操作(中转)
        /// </summary>
        /// <param name="funState">单据状态</param>
        /// <param name="tablename">表名</param>
        /// <param name="tempdt">对应临时表</param>
        private void GetDtToDb(string funState,string tablename,DataTable tempdt)
        {
            //若tablename是T_OfferOrder 或 T_OfferOrderHead,其操作中心:若单据状态为C就插入,若为R就更新
            if (tablename == "T_OfferOrder")
            {
                //执行插入
                if (funState == "C" || GlobalClasscs.Fun.RfFunctionName=="RF")
                {
                    ImportDtToDb(tablename, tempdt);
                }
                //执行更新
                else
                {
                    UpdateDbFromDt(tablename,tempdt);
                }
            }
            //使用Headid放到T_OfferOrderHead表内作判断,若有的,就作更新处理,反之按插入处理
            else if (tablename == "T_OfferOrderHead")
            {
                //获取T_OfferOrderHead表的数据
                var dt = SearchOfferOrderHeadDt();
                //分别设置 插入 更新的临时表
                var inserttemp = tempdt.Clone();
                var updatetemp = tempdt.Clone();
                //循环tempdt
                foreach (DataRow rows in tempdt.Rows)
                {
                    var dtlrows = dt.Select("Headid='"+Convert.ToInt32(rows[1])+"'").Length;
                    //将不存在的数据插入至inserttemp内
                    if (dtlrows == 0)
                    {
                        var newrow = inserttemp.NewRow();
                        for (var i = 0; i < inserttemp.Columns.Count; i++)
                        {
                            newrow[i] = rows[i];
                        }
                        inserttemp.Rows.Add(newrow);
                    }
                    //反之将存在的数据插入至updatetemp内
                    else
                    {
                        var newrow = updatetemp.NewRow();
                        for (var j = 0; j < updatetemp.Columns.Count; j++)
                        {
                            newrow[j] = rows[j];
                        }
                        updatetemp.Rows.Add(newrow);
                    }
                }

                //最后将得出的结果进行插入或更新
                if (inserttemp.Rows.Count > 0)
                    ImportDtToDb(tablename, inserttemp);
                if (updatetemp.Rows.Count > 0)
                    UpdateDbFromDt(tablename, updatetemp);
            }
            //若tablename是T_OfferOrderEntry,其操作中心:就需要使用Entryid进行判断,若为空,为插入,反之为更新
            else
            {
                //思路:将tempdt拆开两个temp,分别以EntryId为空 EntryId不为空为条件
                //创建用于存放EntryId为空的temp
                var insertdtltemp = dbList.GetOfferOrderEntryTemp();
                //创建用于存放EntryId不为空的temp
                var updatedtltemp = dbList.GetOfferOrderEntryTemp();

                //若GlobalClasscs.Fun.RfFunctionName=="RF",就直接执行插入操作,反之,才执行原来的操作
                if (GlobalClasscs.Fun.RfFunctionName == "RF")
                {
                    foreach (DataRow rows in tempdt.Rows)
                    {
                        var newrow = insertdtltemp.NewRow();
                        for (var i = 0; i < insertdtltemp.Columns.Count; i++)
                        {
                            //当检测到‘EntryID’为空时,就对其获取新ID值
                            if (i == 1 && string.IsNullOrEmpty(Convert.ToString(rows[1])))
                            {
                                newrow[i] = GetEntryidKey();
                            }
                            else
                            {
                                newrow[i] = rows[i];
                            }
                        }
                        insertdtltemp.Rows.Add(newrow);
                    }
                }
                else
                {
                    //获取EntryId为空的记录
                    var dtlnullrows = tempdt.Select("Entryid is null");
                    foreach (DataRow t in dtlnullrows)
                    {
                        var newrow = insertdtltemp.NewRow();
                        newrow[0] = t[0];             //Headid
                        newrow[1] = GetEntryidKey();  //Entryid
                        newrow[2] = t[2];             //物料编码ID
                        newrow[3] = t[3];             //物料编码
                        newrow[4] = t[4];             //物料名称
                        newrow[5] = t[5];             //配方用量
                        newrow[6] = t[6];             //占比
                        newrow[7] = t[7];             //物料单价(含税)
                        newrow[8] = t[8];             //物料成本(含税)
                        newrow[9] = t[9];             //备注
                        insertdtltemp.Rows.Add(newrow);
                    }

                    //获取EntryId不为空的记录
                    var dtlnotnullrows = tempdt.Select("Entryid is not null");
                    foreach (DataRow t in dtlnotnullrows)
                    {
                        var newrow = updatedtltemp.NewRow();
                        newrow[0] = t[0];          //Headid
                        newrow[1] = t[1];          //Entryid
                        newrow[2] = t[2];          //物料编码ID
                        newrow[3] = t[3];          //物料编码
                        newrow[4] = t[4];          //物料名称
                        newrow[5] = t[5];          //配方用量
                        newrow[6] = t[6];          //占比
                        newrow[7] = t[7];          //物料单价(含税)
                        newrow[8] = t[8];          //物料成本(含税)
                        newrow[9] = t[9];          //备注
                        updatedtltemp.Rows.Add(newrow);
                    }
                }

                //最后将得出的结果进行插入或更新
                if(insertdtltemp.Rows.Count>0)
                    ImportDtToDb(tablename, insertdtltemp);
                if(updatedtltemp.Rows.Count>0)
                    UpdateDbFromDt(tablename,updatedtltemp);
            }
        }

        /// <summary>
        /// 判断数据是否在T_OfferOrderHead表插入或更新使用
        /// </summary>
        /// <returns></returns>
        private DataTable SearchOfferOrderHeadDt()
        {
            var dt=new DataTable();
            dt = searchDt.SearchOfferHeadDt();
            return dt;
        }

        /// <summary>
        /// 针对指定表进行数据插入
        /// </summary>
        /// <param name="tableName"></param>
        /// <param name="dt"></param>
        public void ImportDtToDb(string tableName, DataTable dt)
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
        /// 根据指定条件对数据表进行更新
        /// </summary>
        public void UpdateDbFromDt(string tablename,DataTable dt)
        {
            var sqladpter = new SqlDataAdapter();
            var ds = new DataSet();

            //根据表格名称获取对应的模板表记录
            var searList = sqlList.SearchUpdateTable(tablename);

            using (sqladpter.SelectCommand = new SqlCommand(searList, searchDt.GetBomOfferConn()))
            {
                //将查询的记录填充至ds(查询表记录;后面的更新作赋值使用)
                sqladpter.Fill(ds);
                //建立更新模板相关信息(包括更新语句 以及 变量参数)
                sqladpter = GetUpdateAdapter(tablename, searchDt.GetBomOfferConn(), sqladpter);
                //开始更新(注:通过对DataSet中存在的表进行循环赋值;并进行更新)
                for (var i = 0; i < dt.Rows.Count; i++)
                {
                    for (var j = 0; j < dt.Columns.Count; j++)
                    {
                        ds.Tables[0].Rows[0].BeginEdit();
                        ds.Tables[0].Rows[0][j] = dt.Rows[i][j];
                        ds.Tables[0].Rows[0].EndEdit();
                    }
                    sqladpter.Update(ds.Tables[0]);
                }
                //完成更新后将相关内容清空
                ds.Tables[0].Clear();
                sqladpter.Dispose();
                ds.Dispose();
            }
        }

        /// <summary>
        /// 建立更新模板相关信息
        /// </summary>
        /// <param name="tablename"></param>
        /// <param name="conn"></param>
        /// <param name="da"></param>
        /// <returns></returns>
        private SqlDataAdapter GetUpdateAdapter(string tablename, SqlConnection conn, SqlDataAdapter da)
        {
            //根据tablename获取对应的更新语句
            var sqlscript = sqlList.UpdateEntry(tablename);
            da.UpdateCommand = new SqlCommand(sqlscript, conn);

            //定义所需的变量参数
            switch (tablename)
            {
                case "T_OfferOrder":
                    da.UpdateCommand.Parameters.Add("@FId", SqlDbType.Int, 8, "FId");
                    da.UpdateCommand.Parameters.Add("@OAorderno", SqlDbType.NVarChar,100, "OAorderno");
                    da.UpdateCommand.Parameters.Add("@Fstatus", SqlDbType.Int, 8, "Fstatus");
                    da.UpdateCommand.Parameters.Add("@CreateDt", SqlDbType.DateTime, 10, "CreateDt");
                    da.UpdateCommand.Parameters.Add("@ConfirmDt", SqlDbType.DateTime, 10, "ConfirmDt");
                    da.UpdateCommand.Parameters.Add("@CreateName", SqlDbType.NVarChar, 100, "CreateName");
                    da.UpdateCommand.Parameters.Add("@Useid", SqlDbType.Int, 8, "Useid");
                    da.UpdateCommand.Parameters.Add("@UserName", SqlDbType.NVarChar, 200, "UserName");
                    da.UpdateCommand.Parameters.Add("@Typeid",SqlDbType.Int,8, "Typeid");
                    break;
                case "T_OfferOrderHead":
                    da.UpdateCommand.Parameters.Add("@Headid", SqlDbType.Int, 8, "Headid");
                    da.UpdateCommand.Parameters.Add("@ProductName", SqlDbType.NVarChar, 200, "ProductName");
                    da.UpdateCommand.Parameters.Add("@Bao", SqlDbType.NVarChar, 100, "Bao");
                    da.UpdateCommand.Parameters.Add("@ProductMi", SqlDbType.Decimal, 4, "ProductMi");
                    da.UpdateCommand.Parameters.Add("@MaterialQty", SqlDbType.Decimal, 4, "MaterialQty");
                    da.UpdateCommand.Parameters.Add("@BaoQty", SqlDbType.Decimal, 4, "BaoQty");
                    da.UpdateCommand.Parameters.Add("@RenQty", SqlDbType.Decimal, 4, "RenQty");
                    da.UpdateCommand.Parameters.Add("@KGQty", SqlDbType.Decimal, 4, "KGQty");
                    da.UpdateCommand.Parameters.Add("@LQty", SqlDbType.Decimal, 4, "LQty");
                    da.UpdateCommand.Parameters.Add("@FiveQty", SqlDbType.Decimal, 4, "FiveQty");
                    da.UpdateCommand.Parameters.Add("@FourFiveQty", SqlDbType.Decimal, 4, "FourFiveQty");
                    da.UpdateCommand.Parameters.Add("@FourQty", SqlDbType.Decimal, 4, "FourQty");
                    da.UpdateCommand.Parameters.Add("@Fremark", SqlDbType.NVarChar, 500, "Fremark");
                    da.UpdateCommand.Parameters.Add("@FBomOrder", SqlDbType.NVarChar, 500, "FBomOrder");
                    da.UpdateCommand.Parameters.Add("@FPrice", SqlDbType.Decimal, 4, "FPrice");
                    da.UpdateCommand.Parameters.Add("@CustName",SqlDbType.NVarChar,300, "CustName");
                    break;
                case "T_OfferOrderEntry":
                    da.UpdateCommand.Parameters.Add("@Entryid", SqlDbType.Int, 8, "Entryid");
                    da.UpdateCommand.Parameters.Add("@MaterialID", SqlDbType.Int, 8, "MaterialID");
                    da.UpdateCommand.Parameters.Add("@MaterialCode", SqlDbType.NVarChar, 100, "MaterialCode");
                    da.UpdateCommand.Parameters.Add("@MaterialName", SqlDbType.NVarChar, 200, "MaterialName");
                    da.UpdateCommand.Parameters.Add("@PeiQty", SqlDbType.Decimal, 4, "PeiQty");
                    da.UpdateCommand.Parameters.Add("@ratioQty", SqlDbType.Decimal, 4, "ratioQty");
                    da.UpdateCommand.Parameters.Add("@MaterialPrice", SqlDbType.Decimal, 4, "MaterialPrice");
                    da.UpdateCommand.Parameters.Add("@MaterialAmount", SqlDbType.Decimal, 4, "MaterialAmount");
                    da.UpdateCommand.Parameters.Add("@Remark", SqlDbType.VarChar,500,"Remark");
                    break;
                case "T_AD_User":
                    da.UpdateCommand.Parameters.Add("@Userid", SqlDbType.Int, 8, "Userid");
                    da.UpdateCommand.Parameters.Add("@ApplyId", SqlDbType.Int, 8, "ApplyId");
                    da.UpdateCommand.Parameters.Add("@CanBackConfirm", SqlDbType.Int, 8, "CanBackConfirm");
                    da.UpdateCommand.Parameters.Add("@Readid", SqlDbType.Int, 8, "Readid");
                    da.UpdateCommand.Parameters.Add("@Addid", SqlDbType.Int, 8, "Addid");
                    da.UpdateCommand.Parameters.Add("@UserRelid", SqlDbType.Int, 8, "UserRelid");
                    break;
                case "T_BD_UserGroup":
                    da.UpdateCommand.Parameters.Add("@GroupId", SqlDbType.Int, 8, "GroupId");
                    da.UpdateCommand.Parameters.Add("@Parentid", SqlDbType.Int, 8, "Parentid");
                    da.UpdateCommand.Parameters.Add("@GroupName", SqlDbType.NVarChar, 100, "GroupName");
                    da.UpdateCommand.Parameters.Add("@CreateName", SqlDbType.NVarChar, 100, "CreateName");
                    da.UpdateCommand.Parameters.Add("@CreateDt", SqlDbType.DateTime, 10, "CreateDt");
                    break;
                case "T_BD_UserGroupDtl":
                    da.UpdateCommand.Parameters.Add("@GroupId", SqlDbType.Int, 8, "GroupId");
                    da.UpdateCommand.Parameters.Add("@Dtlid", SqlDbType.Int, 8, "Dtlid");
                    da.UpdateCommand.Parameters.Add("@UserName", SqlDbType.NVarChar, 100, "UserName");
                    da.UpdateCommand.Parameters.Add("@K3UserGroup", SqlDbType.NVarChar, 100, "K3UserGroup");
                    da.UpdateCommand.Parameters.Add("@K3UserPhone", SqlDbType.NVarChar, 100, "K3UserPhone");
                    da.UpdateCommand.Parameters.Add("@CreateName", SqlDbType.NVarChar, 100, "CreateName");
                    da.UpdateCommand.Parameters.Add("@CreateDt", SqlDbType.DateTime, 10, "CreateDt");
                    break;
            }
            return da;
        }

        /// <summary>
        /// 删除相关记录(主要针对T_OfferOrderEntry表)
        /// </summary>
        private void DeleteRecord(DataTable deldt)
        {
            var fidlist = string.Empty;
            //根据指定条件循环将记录行删除
            foreach (DataRow rows in deldt.Rows)
            {
                if (string.IsNullOrEmpty(fidlist))
                {
                    fidlist = "'"+Convert.ToString(rows[0])+"'";
                }
                else
                {
                    fidlist += "," + "'"+Convert.ToString(rows[0])+"'";
                }
            }
            searchDt.Generdt(sqlList.DelEntry(fidlist));
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
            newrow[4] = sourcerow[4];                                  //审核日期
            newrow[5] = sourcerow[5];                                  //创建人
            newrow[6] = sourcerow[7];                                  //记录当前单据使用标记
            newrow[7] = sourcerow[8];                                  //记录当前单据使用者信息
            newrow[8] = sourcerow[6];                                  //单据类型ID
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
            if (dt.Select("ProductName='"+ sourcerow[10] +"'").Length == 0)
            {
                var newrow = dt.NewRow();
                newrow[0] = fid;                                               //FID
                //注:若出现FunState为R 并且Heaid=0时,就需要通过GetHeadidKey()进行获取新的ID值,而其它情况与原来的一样
                newrow[1] = funState == "R" && Convert.ToInt32(sourcerow[9]) == 0
                    ? GetHeadidKey()
                    : (funState == "C" ? GetHeadidKey() : sourcerow[9]);       //Headid 
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
        private DataTable GetDataToOfferOrderEntryDt(int headid,DataTable dt,DataRow sourcerow)
        {
            var newrow = dt.NewRow();
            newrow[0] = headid;             //Headid
            newrow[1] = sourcerow[25];      //Entryid
            newrow[2] = sourcerow[26];      //物料编码ID
            newrow[3] = sourcerow[27];      //物料编码
            newrow[4] = sourcerow[28];      //物料名称
            newrow[5] = sourcerow[29];      //配方用量
            newrow[6] = sourcerow[30];      //占比
            newrow[7] = sourcerow[31];      //物料单价(含税)
            newrow[8] = sourcerow[32];      //物料成本(含税)
            newrow[9] =sourcerow[33];       //备注
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
        private int GetHeadidKey()
        {
            return generateDt.GetNewHeadidValue();
        }

        /// <summary>
        /// 获取最新的Entryid值
        /// </summary>
        /// <returns></returns>
        private int GetEntryidKey()
        {
            return generateDt.GetNewEntryidValue();
        }

        #region Excel模板导入

        /// <summary>
        /// EXcel导入
        /// </summary>
        /// <param name="reporttype">导入EXCEL时的类型(0:批量成本报表功能使用  1:BOM物料明细使用 2:毛利润报表使用)</param>
        /// <param name="fileAddress"></param>
        /// <returns></returns>
        public DataTable ImportExcelToDt(string reporttype,string fileAddress)
        {
            var dt=new DataTable();
            try
            {
                //使用NPOI技术进行导入EXCEL至DATATABLE
                var importExcelDt = OpenExcelToDataTable(reporttype,fileAddress);
                //将从EXCEL过来的记录集为空的行清除
                dt = RemoveEmptyRows(importExcelDt);
            }
            catch (Exception)
            {
                dt.Rows.Clear();
                dt.Columns.Clear();
            }
            return dt;
        }

        /// <summary>
        /// 打开EXCEL并获取其内容
        /// </summary>
        /// <param name="reporttype">导入EXCEL时的类型(0:批量成本报表功能使用  1:BOM物料明细使用 2:毛利润报表使用)</param>
        /// <param name="fileAddress"></param>
        /// <returns></returns>
        private DataTable OpenExcelToDataTable(string reporttype,string fileAddress)
        {
            IWorkbook wk;
            //根据reporttype判断所导入的列数
            int colnum;
            //定义TEMPDT
            var dt=new DataTable();

            //创建表标题-根据reporttype不同而改变
            switch (reporttype)
            {
                case "0":
                    dt = dbList.ImportExcelTempdt();
                    break;
                case "1":
                    dt = dbList.ImportBomExcelTempdt();
                    break;
                default:
                    dt = dbList.ImportProfitExcelTempdt();
                    break;
            }
            //dt = reporttype == "0" ? dbList.ImportExcelTempdt() : dbList.ImportBomExcelTempdt();

            using (var fsRead = File.OpenRead(fileAddress))
            {
                wk = new XSSFWorkbook(fsRead);
                //获取第一个sheet
                var sheet = wk.GetSheetAt(0);
                //获取第一行
                //var hearRow = sheet.GetRow(0);
                //定义列数
                colnum = reporttype == "0" ? 6 : 2;

                //创建完标题后,开始从第二行起读取对应列的值
                for (var r = 1; r <= sheet.LastRowNum; r++)
                {
                    var result = false;
                    var dr = dt.NewRow();
                    var row = sheet.GetRow(r);
                    if (row == null) continue;

                    for (var j = 0; j < colnum; j++)
                    {
                        //循环获取行中的单元格
                        var cell = row.GetCell(j);
                        var cellValue = GetCellValue(cell);
                        if (cellValue == string.Empty)
                        {
                            continue;
                        }
                        else
                        {
                            dr[j] = cellValue;
                        }

                        //全为空就不取
                        if (dr[j].ToString() != "")
                        {
                            result = true;
                        }
                    }

                    if (result == true)
                    {
                        //把每行增加到DataTable
                        dt.Rows.Add(dr);
                    }
                }
            }
            return dt;
        }

        /// <summary>
        /// 检查单元格的数据类型并获其中的值
        /// </summary>
        /// <param name="cell"></param>
        /// <returns></returns>
        private static string GetCellValue(ICell cell)
        {
            if (cell == null)
                return string.Empty;
            switch (cell.CellType)
            {
                case CellType.Blank: //空数据类型 这里类型注意一下，不同版本NPOI大小写可能不一样,有的版本是Blank（首字母大写)
                    return string.Empty;
                case CellType.Boolean: //bool类型
                    return cell.BooleanCellValue.ToString();
                case CellType.Error:
                    return cell.ErrorCellValue.ToString();
                case CellType.Numeric: //数字类型
                    if (DateUtil.IsCellDateFormatted(cell))//日期类型
                    {
                        return cell.DateCellValue.ToString();
                    }
                    else //其它数字
                    {
                        return cell.NumericCellValue.ToString();

                    }

                case CellType.Unknown: //无法识别类型
                default: //默认类型                    
                    return cell.ToString();
                case CellType.String: //string 类型
                    return cell.StringCellValue;
                case CellType.Formula: //带公式类型
                    try
                    {
                        var e = new XSSFFormulaEvaluator(cell.Sheet.Workbook);
                        e.EvaluateInCell(cell);
                        return cell.ToString();
                    }
                    catch
                    {
                        return cell.NumericCellValue.ToString();
                    }
            }
        }

        /// <summary>
        ///  将从EXCEL导入的DATATABLE的空白行清空
        /// </summary>
        /// <param name="dt"></param>
        /// <returns></returns>
        protected DataTable RemoveEmptyRows(DataTable dt)
        {
            var removeList = new List<DataRow>();
            for (var i = 0; i < dt.Rows.Count; i++)
            {
                var isNull = true;
                for (var j = 0; j < dt.Columns.Count; j++)
                {
                    //将不为空的行标记为False
                    if (!string.IsNullOrEmpty(dt.Rows[i][j].ToString().Trim()))
                    {
                        isNull = false;
                    }
                }
                //将整行都为空白的记录进行记录
                if (isNull)
                {
                    removeList.Add(dt.Rows[i]);
                }
            }

            //将整理出来的所有空白行通过循环进行删除
            for (var i = 0; i < removeList.Count; i++)
            {
                dt.Rows.Remove(removeList[i]);
            }
            return dt;
        }

        #endregion

    }
}
