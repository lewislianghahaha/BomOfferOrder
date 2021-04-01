using System;
using System.Data;
using BomOfferOrder.DB;

namespace BomOfferOrder.Task
{
    public class GenerateDt
    {
        DbList dbList=new DbList();
        SqlList sqlList=new SqlList();
        SearchDt searchDt=new SearchDt();

        #region 参数

        private string _currencyname;

        #endregion

        /// <summary>
        /// 运算-(将‘已添加’的物料记录生成其对应的BOM明细记录)
        /// </summary>
        /// <param name="sourcedt">从查询窗体添加过来的DT记录</param>
        /// <param name="bomdt">BOM明细记录DT</param>
        /// <returns></returns>
        public DataTable Generatedt(DataTable sourcedt,DataTable bomdt)
        {
            var result=new DataTable();
            //保存产品ID（从1开始）
            var id = 0;

            try
            {
                //获取结果临时表
                result = dbList.MakeTemp();
                //循环sourcedt
                foreach (DataRow row in sourcedt.Rows)
                {
                    id++;
                    //获取FMATERIALID
                    var fmaterialid = Convert.ToInt32(row[0]);
                    //获取产品名称
                    var productname = Convert.ToString(row[2]);
                    //获取包装规格
                    var bao = Convert.ToString(row[3]);
                    //获取产品密度
                    var productmi = Convert.ToString(row[4]);

                    //获取BOM编号
                    var dtlrows = bomdt.Select("表头物料ID='" + Convert.ToInt32(row[0]) + "'");
                    var bomnum = Convert.ToString(dtlrows[0][1]);

                    //执行插入相关信息至临时表(使用递归)
                    result.Merge(GetdtltoDt(id,fmaterialid,productname,bao,productmi,bomnum,bomdt,result,0));
                }
            }
            catch (Exception)
            {
                result.Rows.Clear();
                result.Columns.Clear();
            }
            return result;
        }

        /// <summary>
        /// 执行循环插入(递归) 注:在同一个‘产品名称’下,各物料是唯一的,并将相同的‘物料’‘配方用量’进行累加
        /// </summary>
        /// <param name="id">产品ID（从1开始)</param>
        /// <param name="fmaterialid">表头物料ID(循环条件)</param>
        /// <param name="productname">产品名称</param>
        /// <param name="bao">包装规格</param>
        /// <param name="productmi">产品密度</param>
        /// <param name="bomnum">BOM编号</param>
        /// <param name="bomdt">初始化BOM明细DT(全部Bom内容)</param>
        /// <param name="resultdt">结果临时表</param>
        /// <param name="qty">配方用量使用</param>
        /// <returns></returns>
        private DataTable GetdtltoDt(int id,int fmaterialid,string productname,string bao,string productmi, string bomnum,DataTable bomdt,
                                     DataTable resultdt,decimal qty)
        {
            //‘用量’中转值
            decimal qtytemp = 0;

            //根据fmaterialid为‘表头物料ID’为条件,查询bomdt内的明细记录
            var dtlrows = bomdt.Select("表头物料ID='" + fmaterialid + "'");

            for (var i = 0; i < dtlrows.Length; i++)
            {
                //循环判断物料对应的“物料属性”是不是“外购”,若是就插入至resultdt内,反之,进行递归
                if (Convert.ToString(dtlrows[i][5]) == "外购")
                {
                    //判断进入的物料ID是否需要更新或是插入记录
                    //检查若此物料ID在resultdt内已存在(注:在同一个‘产品名称’下),就不需要再次插入,只需要将其‘配方用量’与已存在的记录相加即可
                    var resultrows = resultdt.Select("产品名称='" + productname + "' and 物料编码ID='" + dtlrows[i][2] + "'");

                    //使用‘产品名称’以及‘物料编码’放到resultdt内判断是否存在;若存在,就更新,不用插入新行至resultdt
                    if (resultrows.Length > 0)
                    {
                        foreach (DataRow rows in resultdt.Rows)
                        {
                            if (rows[1].ToString() == productname && Convert.ToInt32(rows[5]) == Convert.ToInt32(dtlrows[i][2]))
                            {
                                //当检查到物料在resultdt存在的话,就进行更新
                                resultdt.BeginInit();
                                //若是第一层级的‘外购’物料，其‘用量’就是取SQL内的‘用量’;反之用量的公式为:‘总用量’*分子/分母*(1+变动损耗率/100) 保留6位小数
                                qtytemp = qty == 0 ? Convert.ToDecimal(dtlrows[i][6]) :
                                          decimal.Round(qty * Convert.ToDecimal(dtlrows[i][7]) / Convert.ToDecimal(dtlrows[i][8]) * (1 + Convert.ToDecimal(dtlrows[i][9]) / 100), 6);

                                //累加‘用量’
                                rows[8] = Convert.ToDecimal(rows[8]) + qtytemp;
                                resultdt.EndInit();
                                //当修改完成后,跳出该循环
                                break;
                            }
                        }
                    }

                    #region Hide
                    //检查获取过来的‘物料编码ID’是否在第一层级的BOM明细行内存在,若存在,即不用创建新行插入,只需累加‘用量’即可
                    //var resultrows = resultdt.Select("产品名称='" + productname + "' and 明细行BOM编号='" + bomnum + "' and 物料编码ID='" + dtlrows[i][2] + "'");

                    //if (resultrows.Length > 0)
                    //{
                    //    foreach (DataRow row in resultdt.Rows)
                    //    {
                    //        //使用‘产品名称’ ‘明细行BOM编号’ ‘表体物料ID’放到resultdt内判断是否存在;若存在,就更新,不用插入新行至resultdt
                    //        if (row[1].ToString() == productname && row[9].ToString() == bomnum && Convert.ToInt32(row[5]) == Convert.ToInt32(dtlrows[i][2]))
                    //        {
                    //            //当检查到物料在resultdt存在的话,就进行更新
                    //            resultdt.BeginInit();
                    //            //若是第一层级的‘外购’物料，其‘用量’就是取SQL内的‘用量’;反之用量的公式为:‘总用量’*分子/分母*(1+变动损耗率/100) 保留6位小数
                    //            qtytemp = qty == 0 ? Convert.ToDecimal(dtlrows[i][6]) :
                    //                decimal.Round(qty * Convert.ToDecimal(dtlrows[i][7]) / Convert.ToDecimal(dtlrows[i][8]) * (1 + Convert.ToDecimal(dtlrows[i][9]) / 100), 6);
                    //            //累加‘用量’
                    //            row[8] = Convert.ToDecimal(row[8]) + qtytemp;
                    //            resultdt.EndInit();
                    //            //当修改完成后,跳出该循环
                    //            break;
                    //        }
                    //    }
                    //}
                    #endregion

                    else
                    {
                        //若是第一层级的‘外购’物料，其‘用量’就是取SQL内的‘用量’;反之用量的公式为:‘总用量’*分子/分母*(1+变动损耗率/100) 保留6位小数
                        qtytemp = qty == 0 ? Convert.ToDecimal(dtlrows[i][6]) : 
                            decimal.Round(qty * Convert.ToDecimal(dtlrows[i][7]) / Convert.ToDecimal(dtlrows[i][8])*(1+Convert.ToDecimal(dtlrows[i][9])/100),6);

                        var newrow = resultdt.NewRow();
                        newrow[0] = id;                   //产品ID
                        newrow[1] = productname;          //产品名称
                        newrow[2] = bomnum;               //BOM编号
                        newrow[3] = bao;                  //包装规格
                        newrow[4] = productmi;            //产品密度
                        newrow[5] = dtlrows[i][2];        //物料编码ID
                        newrow[6] = dtlrows[i][3];        //物料编码
                        newrow[7] = dtlrows[i][4];        //物料名称
                        newrow[8] = qtytemp;              //配方用量
                        newrow[9] = dtlrows[i][1];        //明细行BOM编号
                        newrow[10] = dtlrows[i][10];      //物料单价
                        resultdt.Rows.Add(newrow);
                    }
                }
                //递归调用
                else
                {
                    //若是第一层级的‘外购’物料，其‘用量’就是取SQL内的‘用量’;反之用量的公式为:‘总用量’*分子/分母*(1+变动损耗率/100) 保留6位小数
                    qtytemp = qty == 0 ? Convert.ToDecimal(dtlrows[i][6]) :
                        decimal.Round(qty * Convert.ToDecimal(dtlrows[i][7]) / Convert.ToDecimal(dtlrows[i][8]) * (1 + Convert.ToDecimal(dtlrows[i][9]) / 100), 6);

                    GetdtltoDt(id, Convert.ToInt32(dtlrows[i][2]),productname,bao,productmi,bomnum,bomdt,resultdt,qtytemp);
                }
            }
            return resultdt;
        }


        /// <summary>
        /// 获取最新的FID值
        /// </summary>
        /// <returns></returns>
        public int GetNewFidValue()
        {
            var sqlscript = sqlList.GetNewFidValue();
            return  Convert.ToInt32(searchDt.UseSqlSearchIntoDt(1, sqlscript).Rows[0][0]);
        }

        ///// <summary>
        ///// 获取最新的Headid值
        ///// </summary>
        ///// <returns></returns>
        public int GetNewHeadidValue()
        {
            var sqlscript = sqlList.GetNewHeadidValue();
            return Convert.ToInt32(searchDt.UseSqlSearchIntoDt(1,sqlscript).Rows[0][0]);
        }

        /// <summary>
        /// 获取最新的Entryid值
        /// </summary>
        /// <returns></returns>
        public int GetNewEntryidValue()
        {
            var sqlscript = sqlList.GetNewEntryidValue();
            return Convert.ToInt32(searchDt.UseSqlSearchIntoDt(1, sqlscript).Rows[0][0]);
        }

        /// <summary>
        /// 根据FID更新此单据占用情况
        /// </summary>
        /// <param name="fid"></param>
        /// <param name="type">type:0(更新当前用户信息) 1(清空占用记录)</param>
        /// <param name="oaorder"></param>
        /// <returns></returns>
        public void UpDateUpUseDetail(int fid, int type,string oaorder)
        {
            var sqlscript = type == 0 ? sqlList.UpUsedtl(fid) : sqlList.RemoveUsedtl(fid, oaorder);
            searchDt.Generdt(sqlscript);
        }

        /// <summary>
        /// 根据FID反审核单据状态(更新为反审核状态,并将审核日期清空)
        /// </summary>
        /// <param name="fid"></param>
        /// <returns></returns>
        public bool UpdateOrderStatus(int fid)
        {
            var sqlscript = sqlList.UpOrderStatus(fid);
            return searchDt.Generdt(sqlscript);
        }

        /// <summary>
        /// 获取最新的Userid值
        /// </summary>
        /// <returns></returns>
        public int GetNewUseridValue()
        {
            var sqlscript = sqlList.GetNewUseridValue();
            return Convert.ToInt32(searchDt.UseSqlSearchIntoDt(1, sqlscript).Rows[0][0]);
        }

        /// <summary>
        /// 根据useid更新用户占用记录
        /// </summary>
        /// <param name="userid"></param>
        /// <param name="typeid">0:更新占用 1:清空占用</param>
        public void UpAccountUseridValue(int userid, int typeid)
        {
            var sqlscript = typeid == 0 ? sqlList.UpdateAccountUseid(userid) : sqlList.RemoveAccountUseid(userid);
            searchDt.Generdt(sqlscript);
        }

        /// <summary>
        /// 更新用户新密码
        /// </summary>
        /// <param name="userid"></param>
        /// <param name="newpwd"></param>
        /// <returns></returns>
        public bool UpdateUserNewpwd(int userid, string newpwd)
        {
            var sqlscript = sqlList.UpdateUserNewpwd(userid, newpwd);
            return searchDt.Generdt(sqlscript);
        }

        /// <summary>
        /// 获取GroupID主键值
        /// </summary>
        /// <returns></returns>
        public int GetGroupidKey()
        {
            var sqlscript = sqlList.GetGroupidKey();
            return Convert.ToInt32(searchDt.UseSqlSearchIntoDt(1, sqlscript).Rows[0][0]);
        }

        /// <summary>
        ///  获取Dtlid主键值
        /// </summary>
        /// <returns></returns>
        public int GetDtlidKey()
        {
            var sqlscript = sqlList.GetDtlidKey();
            return Convert.ToInt32(searchDt.UseSqlSearchIntoDt(1, sqlscript).Rows[0][0]);
        }

        ////////////////////////////////////////////////报表运算部份////////////////////////////////////////////////////////////////

        /// <summary>
        /// 报表生成运算-毛利润报表
        /// </summary>
        /// <param name="sourcedt">从查询窗体添加过来的DT记录</param>
        /// <param name="bomdt">BOM明细记录DT</param>
        /// <param name="instockdt">保存采购入库单相关DT(报表功能-旧标准成本单价使用)</param>
        /// <param name="priceListdt">保存价目表DT(报表功能使用)</param>
        /// <param name="salespricedt">保存销售价目表相关</param>
        /// <param name="purchaseinstockdt">保存采购入库单相关-(毛利润报表生成使用)</param>
        /// <param name="rencostdt">保存人工制造费用相关</param>
        /// <returns></returns>
        public DataTable GenerateProfitReportDt(DataTable sourcedt, DataTable bomdt, DataTable instockdt, DataTable priceListdt,
                                      DataTable salespricedt, DataTable purchaseinstockdt, DataTable rencostdt)
        {
            var resultdt = new DataTable();

            try
            {
                //定义结果临时表
                resultdt = dbList.PrintProfitReportTempdt();

                #region 参数变量
                //定义‘销售价目表售价’
                decimal salesprice = 0;
                //定义‘包装罐’
                var guan = string.Empty;
                //定义‘包装罐单价’
                decimal guanprice = 0;
                //定义‘纸箱’
                var box = string.Empty;
                //定义‘纸箱单价’
                decimal boxprice = 0;
                //定义‘人工及制造费用’
                decimal rencost = 0;
                //定义‘标准成本单价’
                decimal totalamount = 0;
                //定义‘旧标准成本单价’
                decimal oldtotalamount = 0;

                //定义‘毛利润率’
                string mao = string.Empty;
                //定义‘计价成本’
                decimal zichenbin = 0;
                //定义‘每公斤含税成本小计’
                decimal kgtotal = 0;
                //定义‘计价单位成本(套/升/罐/桶)’
                decimal price = 0;
                //定义‘计价单位成本(KG)’
                decimal pricekg = 0;
                //定义‘每公斤材料成本单价’
                decimal materialprice = 0;
                //定义‘毛利润报表’行是否标红(0:是 1:否)
                var colChangeColId = 1;
                #endregion

                //调用GenerateReportDt()用于获取‘标准成本单价’以及‘旧标准成本单价’
                var tempdt = GenerateReportDt(sourcedt,bomdt,instockdt,priceListdt);

                //循环sourcedt
                foreach (DataRow rows in sourcedt.Rows)
                {
                    //计算‘销售价目表售价’
                    salesprice = GetSalesPrice(Convert.ToInt32(rows[0]), salespricedt);//* Convert.ToDecimal(rows[10]);

                    //计算‘包装罐’
                    guan = GuanBoxName(0,Convert.ToInt32(rows[0]),bomdt);

                    //计算‘包装罐单价’
                    guanprice = GuanBoxPrice(0,Convert.ToInt32(rows[0]),bomdt,purchaseinstockdt);

                    //计算‘纸箱’
                    box = GuanBoxName(1,Convert.ToInt32(rows[0]),bomdt);

                    //计算‘纸箱单价’
                    boxprice = GuanBoxPrice(1,Convert.ToInt32(rows[0]),bomdt,purchaseinstockdt);

                    //计算‘人工及制造费用’
                    rencost = GetRenCost(Convert.ToString(rows[8]),Convert.ToString(rows[7]),rencostdt);

                    //以rows.物料编码为条件,查询tempdt内的‘标准成本单价’以及‘旧标准成本单价’值
                    var dtlrows = tempdt.Select("物料编码='" + rows[1] + "'");
                    oldtotalamount = Convert.ToDecimal(dtlrows[0][5]);  //旧标准成本单价
                    totalamount = Convert.ToDecimal(dtlrows[0][6]);     //标准成本单价
                    colChangeColId = Convert.ToInt32(dtlrows[0][12]);   //定义‘毛利润报表’行是否标红(0:是 1:否) 注:当‘单价’为0时,就标红

                    //计算‘每公斤材料成本单价’=('旧标准成本单价'/'重量')
                    materialprice = rows[5] == DBNull.Value || Convert.ToDecimal(rows[5]) == 0
                        ? 0
                        : Convert.ToDecimal(
                            decimal.Round(
                                decimal.Round(oldtotalamount, 4)/decimal.Round(Convert.ToDecimal(rows[5]), 4), 4));

                    //计算‘每公斤含税成本小计’=('每公斤材料成本单价'+'人工及制造费用') 也称为:(每公斤含税成本（含材料包装人工）小计)
                    kgtotal = decimal.Round(materialprice + rencost,4);

                    //计算‘计价单位单位成本(千克)’=(若计价单位为KG,使用‘人工及制造费用’;反之为空)
                    pricekg = Convert.ToString(rows[9]) == "千克" ? rencost : Convert.ToDecimal(null);

                    //计算‘计价单位单位成本(套/升/罐/桶)’=(若计价单位为KG,即为空,反之,使用‘换算率’*‘每公斤含税成本小计’)
                    //price = Convert.ToString(rows[9]) == "千克"
                    //? 0 : decimal.Round(rows[4]==DBNull.Value ? 0 : decimal.Round(Convert.ToDecimal(rows[4]), 4) * kgtotal, 4);

                    //计算‘计价单位单位成本(套/升/罐/桶)’=(若计价单位为KG,即为空(0),若为‘罐’使用Bom成本单价+(人工制造费用*重量/换算率) 反之,使用‘换算率’*‘每公斤含税成本小计’)
                    //change date:20210401
                    switch (Convert.ToString(rows[9]))
                    {
                        case "千克":
                            price = 0;
                            break;
                        case "罐":
                            var nkg = rows[5] == DBNull.Value ? 0 : decimal.Round(Convert.ToDecimal(rows[5]), 4);
                            var tran = rows[4] == DBNull.Value ? 0 : decimal.Round(Convert.ToDecimal(rows[4]), 4);
                            price = oldtotalamount+(decimal.Round(rencost, 4)* nkg / tran);
                            break;
                        default:
                            price = decimal.Round(rows[4] == DBNull.Value ? 0 : decimal.Round(Convert.ToDecimal(rows[4]), 4)*kgtotal, 4);
                            break;
                    }

                    //计算‘计价成本’=(若计价单位为KG,就取‘每公斤含税成本小计’值;反之,取'计价单位单位成本(套/升/罐/桶)'值)
                    //change date:若计价单位为套,公式为:净重*每公斤含税成本小计
                    switch (Convert.ToString(rows[9]))
                    {
                        case "千克":
                            zichenbin = kgtotal;
                            break;
                        case "套":
                            zichenbin = kgtotal * decimal.Round(Convert.ToDecimal(rows[5]), 4);
                            break;
                        default:
                            zichenbin = decimal.Round(price,4);
                            break;
                    }

                    //zichenbin = Convert.ToString(rows[9]) == "千克" ? kgtotal : price;

                    //计算‘毛利润率’=((销售价目表售价-计价成本)/销售价目表售价*100)
                    mao = salesprice == 0
                        ? "" : Convert.ToString(decimal.Round((salesprice - zichenbin) / salesprice * 100,2)) + "%";

                    //最后将所有结果插入至resultdt内     
                    var newrow = resultdt.NewRow();
                    newrow[0] = rows[1];                                                                      //物料编码
                    newrow[1] = rows[2];                                                                      //产品名称
                    newrow[2] = rows[3];                                                                      //规格型号
                    newrow[3] = rows[6] == DBNull.Value ? 0 : decimal.Round(Convert.ToDecimal(rows[6]), 4);   //罐箱
                    newrow[4] = oldtotalamount;                                                               //旧标准成本单价(Bom成本单价)
                    newrow[5] = totalamount;                                                                  //标准成本单价(基础资料值成本单价)
                    newrow[6] = rows[9];                                                                      //销售计价单位
                    newrow[7] = decimal.Round(salesprice, 4);                                                 //销售价目表售价
                    newrow[8] = _currencyname;                                                                //币别
                    newrow[9] = mao;                                                                          //毛利润率
                    newrow[10] = zichenbin;                                                                   //计价成本
                    newrow[11] = decimal.Round(price,4);                                                      //计价单位单位成本(套/升/罐/桶)
                    newrow[12] = decimal.Round(pricekg, 4);                                                   //计价单位单位成本(千克)

                    newrow[13] = rows[4] == DBNull.Value ? 0 : decimal.Round(Convert.ToDecimal(rows[4]), 4);  //换算率
                    newrow[14] = rows[5] == DBNull.Value ? 0 : decimal.Round(Convert.ToDecimal(rows[5]), 4);  //重量(净重)
                    newrow[15] = kgtotal;                                                                     //每公斤含税成本小计(每公斤含税成本（含材料包装人工）小计)
                    newrow[16] = materialprice;                                                               //每公斤材料成本单价
                    newrow[17] = decimal.Round(rencost, 4);                                                   //人工及制造费用
                    newrow[18] = guan;                                                                        //包装罐
                    newrow[19] = decimal.Round(Convert.ToDecimal(guanprice), 4);                              //包装罐单价
                    newrow[20] = box;                                                                         //纸箱
                    newrow[21] = decimal.Round(Convert.ToDecimal(boxprice), 4);                               //纸箱单价
                    newrow[22] = rows[7];                                                                     //分类
                    newrow[23] = rows[8];                                                                     //品类
                    newrow[24] = colChangeColId;                                                              //定义‘毛利润报表’行是否标红(0:是 1:否) 注:当‘单价’为0时,就标红
                    resultdt.Rows.Add(newrow);
                }
            }
            catch (Exception)
            {
                resultdt.Rows.Clear();
                resultdt.Columns.Clear();
            }
            return resultdt;
        }

        /// <summary>
        /// 计算销售价目表售价
        /// 根据FMATERIALID查找出销售价目表‘价格’ 保留四位小数
        /// </summary>
        /// <param name="fmaterialid"></param>
        /// <param name="salespricedt"></param>
        /// <returns></returns>
        private decimal GetSalesPrice(int fmaterialid,DataTable salespricedt)
        {
            decimal result = 0;
            //以fmaterialid为条件,查找salespricedt内的‘价格’信息
            var dtlrow = salespricedt.Select("FMATERIALID='" + fmaterialid + "'");
            if (dtlrow.Length == 0)
            {
                result = 0;
                _currencyname = "";
            }
            //注:当行数超过1行时,就要取第一行的值
            else if (dtlrow.Length >=1)
            {
                result = decimal.Round(Convert.ToDecimal(dtlrow[0][2]),4);
                _currencyname = Convert.ToString(dtlrow[0][3]);
            }
            return result;
        }

        /// <summary>
        /// 获取‘罐’‘箱’对应的‘物料名称’
        /// 罐-T WSX  纸箱-WZX ZX
        /// </summary>
        /// <param name="typeid">0:罐 1:纸箱</param>
        /// <param name="fmaterialid"></param>
        /// <param name="bomdt"></param>
        /// <returns></returns>
        private string GuanBoxName(int typeid,int fmaterialid,DataTable bomdt)
        {
            var result = string.Empty;
            //typeid=0 获取罐信息 1 获取包装箱信息
            if (typeid == 0)
            {
                //以BOMDT的‘表头物料ID’为条件,使用FMATERIALID查询,若查找的‘物料编码’有'T','WSX'就获取其对应的‘物料名称’
                var dtlrows = bomdt.Select("表头物料ID='" + fmaterialid + "' and (物料编码 like '%T%' or 物料编码 like '%WSX%') and 物料名称 not like '%盖%' ");
                result = dtlrows.Length == 0 ? "" : Convert.ToString(dtlrows[0][4]);
            }
            else
            {
                //以BOMDT的‘表头物料ID’为条件,使用FMATERIALID查询,若查找的‘物料编码’有'ZX','WZX'就获取其对应的‘物料名称’
                var dtlrows = bomdt.Select("表头物料ID='" + fmaterialid + "' and (物料编码 like '%ZX%' or 物料编码 like '%WZX%') ");
                result = dtlrows.Length == 0 ? "" : Convert.ToString(dtlrows[0][4]);
            }
            return result;
        }

        /// <summary>
        /// 获取‘罐’‘箱’对应的‘净价’
        /// 纸箱-WZX ZX
        /// </summary>
        /// <param name="typeid">0:罐 1:纸箱</param>
        /// <param name="fmaterialid"></param>
        /// <param name="bomdt"></param>
        /// <param name="purchaseinstockdt"></param>
        /// <returns></returns>
        private decimal GuanBoxPrice(int typeid, int fmaterialid, DataTable bomdt, DataTable purchaseinstockdt)
        {
            decimal result = 0;
            //定义‘表体物料ID’
            var detailfmaterialid = 0;

            //typeid=0 获取罐信息 1 获取包装箱信息
            //以BOMDT的‘表头物料ID’为条件,使用FMATERIALID查询,若查找的‘物料编码’有'T','WSX'就获取其对应的‘表体物料ID’
            //以BOMDT的‘表头物料ID’为条件,使用FMATERIALID查询,若查找的‘物料编码’有'ZX','WZX'就获取其对应的‘表体物料ID’
            var dtlrows = typeid == 0 ? bomdt.Select("表头物料ID='" + fmaterialid + "' and (物料编码 like '%T%' or 物料编码 like '%WSX%' and 物料名称 not like '%盖%') ") : 
                                  bomdt.Select("表头物料ID='" + fmaterialid + "' and (物料编码 like '%ZX%' or 物料编码 like '%WZX%') ");

            if (dtlrows.Length == 0)
            {
                result = 0;
            }
            else
            {
                detailfmaterialid = Convert.ToInt32(dtlrows[0][2]);
                //将获取到的‘表体物料ID’放到purchaseinstockdt内,将获取其对应的‘净价’;注:若出现多个的话就取第一行的记录
                var dtlrow = purchaseinstockdt.Select("FMATERIALID='" + detailfmaterialid + "'");
                result = dtlrow.Length == 0 ? 0 : Convert.ToDecimal(dtlrow[0][1]);
            }
            return result;
        }

        /// <summary>
        /// 计算人工制造费用
        /// </summary>
        /// <param name="classtype">品类</param>
        /// <param name="sorttype">分类</param>
        /// <param name="rencostdt">人工及制造费用DT</param>
        /// <returns></returns>
        private decimal GetRenCost(string classtype,string sorttype,DataTable rencostdt)
        {
            decimal result = 0;
            //以‘品类’及‘分类’为条件并使用人工及制造费用DT进行查询
            //注:判断顺序=>1)将‘品类’以及‘分类’进行合并判断 2)若第一点没有的话再使用‘品类’进行判断 3)若第二点没有的话再用‘分类’进行判断,没有就为0显示
            //注:若出现多行,取第一行值
            var dtlrows = rencostdt.Select("品类='" + classtype + "' and 分类='" + sorttype + "'");
            if (dtlrows.Length == 0)
            {
                var dtclassrows = rencostdt.Select("品类='" + classtype + "'");
                if (dtclassrows.Length == 0)
                {
                    var dtfenrows = rencostdt.Select("分类='" + sorttype + "'");
                    result = dtfenrows.Length == 0 ? 0 : Convert.ToDecimal(dtfenrows[0][2]);
                }
                else
                {
                    result = Convert.ToDecimal(dtclassrows[0][2]);
                }
            }
            else
            {
                result = Convert.ToDecimal(dtlrows[0][2]);
            }
            return result;
        }


        /// <summary>
        /// 报表生成运算-批量成本报表
        /// </summary>
        /// <param name="sourcedt">从查询窗体添加过来的DT记录</param>
        /// <param name="bomdt">BOM明细记录DT</param>
        /// <param name="instockdt">保存采购入库单相关DT(报表功能-旧标准成本单价使用)</param>
        /// <param name="priceListdt">保存价目表DT(报表功能使用)</param>
        /// <returns></returns>
        public DataTable GenerateReportDt(DataTable sourcedt, DataTable bomdt, DataTable instockdt, DataTable priceListdt)
        {
            var resultdt = new DataTable();

            try
            {
                //定义结果临时表
                resultdt = dbList.ReportPrintTempdt();
                //定义Mark bool值
                var mark = 1;
                //定义‘父项金额’(标准成本单价使用)
                decimal totalamount = 0;
                //定义‘父项金额’(旧标准成本单价使用)
                decimal oldtotalamount = 0;
                //定义‘毛利润报表’行是否标红(0:是 1:否)
                var colChangeColId = 1;

                //循环sourcedt
                foreach (DataRow rows in sourcedt.Rows)
                {
                    //获取FMATERIALID
                    var fmaterialid = Convert.ToInt32(rows[0]);
                    //获取物料编码
                    var fmaterialCode = Convert.ToString(rows[1]);
                    //获取物料名称
                    var productname = Convert.ToString(rows[2]);
                    //获取规格型号
                    var spec = Convert.ToString(rows[3]);
                    //获取密度(换算率)
                    var mi = rows[4] == DBNull.Value ? 0 : decimal.Round(Convert.ToDecimal(rows[4]),4);                 
                    //获取净重(重量)
                    var weight = rows[5] == DBNull.Value? 0 : decimal.Round(Convert.ToDecimal(rows[5]),4);

                    //从bomdt内获取‘父项物料单位’
                    var dtlrows = bomdt.Select("表头物料ID='" + Convert.ToInt32(rows[0]) + "'");
                    var unitname = Convert.ToString(dtlrows[0][11]);

                    //执行将相关信息插入至tempdt内(使用递归)
                    var tempdt = GenerateReportDtlTemp(fmaterialid,productname,bomdt, dbList.ReportTempdt(), 0,instockdt,priceListdt);

                    //获取明细行后将tempdt循环-作用:获取‘父项金额’及定义‘MarkId’值
                    foreach (DataRow row in tempdt.Rows)
                    {
                        //判断若‘物料名称’不为‘拉盖’(且不为‘损耗’) 并且‘标准单价’为0,就将mark=0,反之为1 注:若mark为0即跳出循环
                        if (!Convert.ToString(row[2]).Contains("拉盖") && !Convert.ToString(row[2]).Contains("损耗"))
                        {
                            if (Convert.ToDecimal(row[4]) == 0)
                            {
                                mark = 0;
                            }
                        }
                        else
                        {
                            mark = 1;
                        }
                        if (mark == 0)
                           break;
                    }

                    //定义‘毛利润报表’行是否标红(0:是 1:否) 注:当‘单价’为0时,就标红
                    foreach (DataRow row in tempdt.Rows)
                    {
                        colChangeColId = Convert.ToDecimal(row[4]) == 0 ? 0 : 1;
                        if (colChangeColId == 0) break;
                    }

                    //累加得出‘父项金额’(标准成本单价使用)
                    foreach (DataRow row in tempdt.Rows)
                    {
                        totalamount += Convert.ToDecimal(row[6]);
                    }
                    //累加得出‘父项金额’(旧标准成本单价使用)
                    foreach (DataRow row in tempdt.Rows)
                    {
                        oldtotalamount += Convert.ToDecimal(row[7]);
                    }

                    //将相关内容插入至resultdt内
                    resultdt.Merge(MarkRecordToReportDt(fmaterialCode,productname,spec,mi,weight,unitname,mark,
                                                        oldtotalamount,totalamount,resultdt,colChangeColId));

                    //最后将tempdt行清空以及相中间变量清空,待下一次循环使用
                    tempdt.Rows.Clear();
                    totalamount = 0;
                    oldtotalamount = 0;
                    mark = 1;
                    colChangeColId = 1;
                }
            }
            catch (Exception)
            {
                resultdt.Rows.Clear();
                resultdt.Columns.Clear();
            }
            return resultdt;
        }

        /// <summary>
        /// 执行将相关信息插入至tempdt内
        /// </summary>
        /// <param name="fmaterialid">表头物料ID(循环条件)</param>
        /// <param name="productname">表头物料名称</param>
        /// <param name="bomdt">初始化BOM明细DT(全部Bom内容)</param>
        /// <param name="resultdt">结果临时表</param>
        /// <param name="qty">配方用量使用</param>
        /// <param name="instockdt">初始化入库单DT-计算旧标准单价使用</param>
        /// <param name="priceListdt">初始化价目表DT-计算旧标准单价使用</param>
        /// <returns></returns>
        public DataTable GenerateReportDtlTemp(int fmaterialid,string productname,DataTable bomdt,DataTable resultdt,
                                                decimal qty,DataTable instockdt,DataTable priceListdt)
        {
            //‘用量’中转值
            decimal qtytemp;
            //定义‘单价’变量-旧标准成本单价使用
            decimal oldprice;

            //根据fmaterialid为‘表头物料ID’为条件,查询bomdt内的明细记录
            var dtlrows = bomdt.Select("表头物料ID='" + fmaterialid + "'");

            for (var i = 0; i < dtlrows.Length; i++)
            {
                //循环判断物料对应的“物料属性”是不是“外购”,若是就插入至resultdt内,反之,进行递归
                if (Convert.ToString(dtlrows[i][5]) == "外购")
                {
                    //若是第一层级的‘外购’物料，其‘用量’就是取SQL内的‘用量’;反之用量的公式为:‘总用量’*分子/分母*(1+变动损耗率/100) 保留12位小数
                    qtytemp = qty == 0 ? Convert.ToDecimal(dtlrows[i][6]) :
                        decimal.Round(qty * Convert.ToDecimal(dtlrows[i][7]) / Convert.ToDecimal(dtlrows[i][8]) * (1 + Convert.ToDecimal(dtlrows[i][9]) / 100),12);
                  
                    //计算旧标准单价
                    oldprice = GetOldPrice(Convert.ToInt32(dtlrows[i][2]),instockdt,priceListdt);

                    var newrow = resultdt.NewRow();
                    newrow[0] = productname;                                                   //表头物料名称
                    newrow[1] = dtlrows[i][2];                                                 //FMATERIALID(表体物料ID)
                    newrow[2] = dtlrows[i][4];                                                 //物料名称
                    newrow[3] = qtytemp;                                                       //用量
                    newrow[4] = dtlrows[i][10];                                                //单价 (标准单价使用)
                    newrow[5] = oldprice;                                                      //旧标准单价 (旧标准成本单价使用)
                    newrow[6] = decimal.Round(qtytemp * Convert.ToDecimal(dtlrows[i][10]),12); //子项金额=用量*单价 (标准成本单价使用)
                    newrow[7] = decimal.Round(qtytemp * oldprice,12);                          //旧子项金额=用量*单价 (旧标准成本单价使用)
                    resultdt.Rows.Add(newrow);
                }
                //递归调用
                else
                {
                    //若是第一层级的‘外购’物料，其‘用量’就是取SQL内的‘用量’;反之用量的公式为:‘总用量’*分子/分母*(1+变动损耗率/100) 保留12位小数
                    qtytemp = qty == 0 ? Convert.ToDecimal(dtlrows[i][6]) :
                        decimal.Round(qty * Convert.ToDecimal(dtlrows[i][7]) / Convert.ToDecimal(dtlrows[i][8]) * (1 + Convert.ToDecimal(dtlrows[i][9]) / 100),12);

                    GenerateReportDtlTemp(Convert.ToInt32(dtlrows[i][2]), productname, bomdt, resultdt, qtytemp, instockdt, priceListdt);
                }
            }
            return resultdt;
        }

        /// <summary>
        /// 合并TEMP
        /// </summary>
        /// <param name="fmaterialCode">物料编码</param>
        /// <param name="productname">物料名称</param>
        /// <param name="spec">规格型号</param>
        /// <param name="mi">换算率</param>
        /// <param name="weight">重时</param>
        /// <param name="unitname">计量单位</param>
        /// <param name="mark">是否标红标记</param>
        /// <param name="oldtotalamount">父项金额-旧标准成本单价使用</param>
        /// <param name="totalamount">父项金额-标准成本单价使用</param>
        /// <param name="resultdt">整理后的临时表</param>
        /// <param name="colChangeColId">是否标红标记(毛利润报表)</param>
        /// <returns></returns>
        private DataTable MarkRecordToReportDt(string fmaterialCode, string productname,string spec,decimal mi,decimal weight,
                                               string unitname,int mark,decimal oldtotalamount,decimal totalamount,DataTable resultdt,int colChangeColId)
        {
            var newrow = resultdt.NewRow();
            newrow[0] = fmaterialCode;                                                          //物料编码
            newrow[1] = productname;                                                            //品名
            newrow[2] = spec;                                                                   //规格
            newrow[3] = unitname;                                                               //计量单位
            newrow[4] = DBNull.Value;                                                           //数量
            newrow[5] = decimal.Round(oldtotalamount,7);                                        //旧标准成本单价
            newrow[6] = decimal.Round(totalamount,7);                                           //标准成本单价
            newrow[7] = mi >= Convert.ToDecimal(0.7) && mi <= Convert.ToDecimal(1.4) ? mi : 1;  //换算率                            
            newrow[8] = weight;                                                                 //重量
            newrow[9] = weight == 0 ? totalamount : decimal.Round(totalamount/weight, 6);       //重量成本单价=标准成本单价/重量
            newrow[10] = DBNull.Value;                                                          //人工用制造费用
            newrow[11] = mark;                                                                  //Markid
            newrow[12] = colChangeColId;                                                        //ColChangeColId
            resultdt.Rows.Add(newrow);
            return resultdt;
        }

        /// <summary>
        /// 计算旧标准单价
        /// </summary>
        /// <param name="materialid">物料ID</param>
        /// <param name="instockdt">初始化入库单DT</param>
        /// <param name="priceListdt">初始化价目表DT</param>
        /// <returns></returns>
        private decimal GetOldPrice(int materialid,DataTable instockdt, DataTable priceListdt)
        {
            decimal result = 0;

            //先将materialid放到instockdt内进行检测,若没有对应‘单价’,即放到priceListdt内进行检测;
            //注:若发现两个DT都没有对应的单价,即返回0
            var instockrow = instockdt.Select("子项物料内码='" + materialid+"'");
            if (instockrow.Length > 0)
            {
                result = Convert.ToDecimal(instockrow[0][1]);
            }
            else
            {
                var pricelistrow = priceListdt.Select("子项物料内码='" + materialid+"'");
                result = pricelistrow.Length == 0 ? Convert.ToDecimal(0) : Convert.ToDecimal(pricelistrow[0][1]);
            }
            return result;
        }



    }
}
