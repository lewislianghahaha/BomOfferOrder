namespace BomOfferOrder.DB
{
    public class SqlList
    {
        //根据SQLID返回对应的SQL语句  
        private string _result;

        /// <summary>
        /// 查询窗体使用-成本Bom报价单窗体查询使用
        /// </summary>
        /// <param name="searchid">查询ID</param>
        /// <param name="searchvalue">查询值</param>
        /// <returns></returns>
        public string Get_Search(int searchid,string searchvalue)
        {
            //按照‘物料名称’进行查询
            if (searchid == 1)
            {
                _result = $@"
                                SELECT a.FMATERIALID,a.FNUMBER 物料编码,d.FNAME 物料名称,c.FDATAVALUE 物料分组,
                                       d.FSPECIFICATION '规格型号',g.FNAME '基本单位',a.F_YTC_DECIMAL1 '密度(KG/L)',a.F_YTC_REMARK '包装规格'

                                FROM dbo.T_BD_MATERIAL a
                                INNER JOIN dbo.T_BAS_ASSISTANTDATAENTRY b ON a.F_YTC_ASSISTANT5=b.FENTRYID
                                INNER JOIN dbo.T_BAS_ASSISTANTDATAENTRY_L c ON b.FENTRYID=c.FENTRYID
                                INNER JOIN dbo.T_BD_MATERIAL_L d ON a.FMATERIALID=d.FMATERIALID

                                INNER JOIN dbo.T_BD_MATERIALBASE e ON a.FMATERIALID=e.FMATERIALID
                                INNER JOIN dbo.T_BD_UNIT f ON e.FBASEUNITID=f.FUNITID
                                INNER JOIN dbo.T_BD_UNIT_L g ON f.FUNITID=g.FUNITID

                                WHERE c.FDATAVALUE IN('产成品','原漆半成品','原漆')
                                AND a.FDOCUMENTSTATUS='C'
                                AND a.FFORBIDSTATUS='A' --物料禁用状态:否
                                AND d.FLOCALEID=2052
                                AND g.FLOCALEID=2052
                                AND D.FNAME LIKE '%{searchvalue}%'
                                AND EXISTS (
												SELECT NULL FROM T_ENG_BOM A1
												WHERE A1.FMATERIALID=A.FMATERIALID
                                                AND a1.FFORBIDSTATUS='A'   --BOM禁用状态:否
												AND a1.FDOCUMENTSTATUS='C' --BOM审核状态:已审核
											) --必须要在‘成本BOM’内存在
                                --AND a.FNUMBER='010-0025(P-2)-4L-00-43-CK'
                            ";
            }
            //按照‘物料编码’进行查询
            else
            {
                _result = $@"
                                SELECT a.FMATERIALID,a.FNUMBER 物料编码,d.FNAME 物料名称,c.FDATAVALUE 物料分组,
                                        d.FSPECIFICATION '规格型号',g.FNAME '基本单位',a.F_YTC_DECIMAL1 '密度(KG/L)',a.F_YTC_REMARK '包装规格'

                                FROM dbo.T_BD_MATERIAL a
                                INNER JOIN dbo.T_BAS_ASSISTANTDATAENTRY b ON a.F_YTC_ASSISTANT5=b.FENTRYID
                                INNER JOIN dbo.T_BAS_ASSISTANTDATAENTRY_L c ON b.FENTRYID=c.FENTRYID
                                INNER JOIN dbo.T_BD_MATERIAL_L d ON a.FMATERIALID=d.FMATERIALID

                                INNER JOIN dbo.T_BD_MATERIALBASE e ON a.FMATERIALID=e.FMATERIALID
                                INNER JOIN dbo.T_BD_UNIT f ON e.FBASEUNITID=f.FUNITID
                                INNER JOIN dbo.T_BD_UNIT_L g ON f.FUNITID=g.FUNITID

                                WHERE c.FDATAVALUE IN('产成品','原漆半成品','原漆')
                                AND a.FDOCUMENTSTATUS='C'
                                AND a.FFORBIDSTATUS='A' --物料禁用状态:否
                                AND d.FLOCALEID=2052
                                AND g.FLOCALEID=2052
                                AND a.FNUMBER LIKE '%{searchvalue}%'
                                AND EXISTS (
												SELECT NULL FROM T_ENG_BOM A1
												WHERE A1.FMATERIALID=A.FMATERIALID
                                                AND a1.FFORBIDSTATUS='A'   --BOM禁用状态:否
												AND a1.FDOCUMENTSTATUS='C' --BOM审核状态:已审核
											)  --必须要在‘成本BOM’内存在
                                --AND a.FNUMBER='010-0025(P-2)-4L-00-43-CK'
                            ";
            }
            return _result;
        }

        /// <summary>
        /// 获取BOM明细记录信息(生成时使用) 注:获取目前K3-CLOUD BOM相关明细的所有记录;在初始化时获取
        /// </summary>
        /// <returns></returns>
        public string Get_Bomdtl()
        {
            #region Hide
            //_result = $@"
            //                    SELECT x.FMATERIALID,x.BOM编号,x.修改日期,x.物料编码id,x.物料编码,x.物料名称,x.markid
            //                    FROM (
            //                       --不计算原漆的SQL
            //                       SELECT a.FMATERIALID,A.FNUMBER 'BOM编号' ,CONVERT(varchar(100), a.FMODIFYDATE, 23) '修改日期',
            //                           c.FMATERIALID 物料编码id,c.FNUMBER 物料编码,d.FNAME 物料名称,0 markid

            //                       FROM T_ENG_BOM A
            //                       INNER JOIN dbo.T_ENG_BOMCHILD B ON A.FID=B.FID
            //                       INNER JOIN dbo.T_BD_MATERIAL c ON b.FMATERIALID=c.FMATERIALID
            //                       INNER JOIN dbo.T_BD_MATERIAL_L d ON c.FMATERIALID=d.FMATERIALID

            //                       WHERE A.FMATERIALID IN({valuelist})
            //                       AND a.FFORBIDSTATUS='A'   --禁止状态:否
            //                       AND a.FDOCUMENTSTATUS='C' --审核状态:已审核
            //                       AND c.FDOCUMENTSTATUS='C'
            //                       AND d.FLOCALEID='2052'
            //                       AND d.FNAME NOT LIKE '%原漆%'

            //                       UNION ALL

            //                       --计算原漆的SQL
            //                       SELECT x.FMATERIALID,x.BOM编号,CONVERT(varchar(100), a.FMODIFYDATE, 23) '修改日期',
            //                            c.FMATERIALID 物料编码id,C.FNUMBER 物料编码,d.FNAME 物料名称,1 markid
            //                       FROM (
            //                         SELECT a.FMATERIALID,A.FNUMBER 'BOM编号' ,b.FMATERIALID 明细物料ID

            //                         FROM T_ENG_BOM A
            //                         INNER JOIN dbo.T_ENG_BOMCHILD B ON A.FID=B.FID
            //                         INNER JOIN dbo.T_BD_MATERIAL c ON b.FMATERIALID=c.FMATERIALID
            //                         INNER JOIN dbo.T_BD_MATERIAL_L d ON c.FMATERIALID=d.FMATERIALID

            //                         WHERE A.FMATERIALID IN({valuelist})
            //                         AND a.FFORBIDSTATUS='A'   --禁止状态:否
            //                         AND a.FDOCUMENTSTATUS='C' --审核状态:已审核
            //                         AND c.FDOCUMENTSTATUS='C'
            //                         AND d.FLOCALEID='2052'
            //                         AND d.FNAME LIKE '%原漆%')x
            //                       INNER JOIN dbo.T_ENG_BOM a ON x.明细物料ID=a.FMATERIALID
            //                       INNER JOIN dbo.T_ENG_BOMCHILD b ON a.FID=b.FID
            //                       INNER JOIN dbo.T_BD_MATERIAL C ON B.FMATERIALID=C.FMATERIALID
            //                       INNER JOIN dbo.T_BD_MATERIAL_L D ON C.FMATERIALID=D.FMATERIALID

            //                       WHERE a.FFORBIDSTATUS='A' --禁止状态:否
            //                       AND A.FDOCUMENTSTATUS='C'
            //                       AND c.FDOCUMENTSTATUS='C'
            //                       AND D.FLOCALEID='2052'
            //                         )x
            //                    ORDER BY x.FMATERIALID,x.markid,x.修改日期 DESC
            //            ";
            #endregion

            _result = $@"
                            SELECT A.FMATERIALID 表头物料ID,A.FNUMBER 'BOM编号',
	                               b.FMATERIALID 表体物料ID,c.FNUMBER 物料编码,d.FNAME 物料名称,
                                   CASE E.FERPCLSID WHEN 1 THEN '外购' WHEN 2 THEN '自制' ELSE '其它' END 物料属性,
                                   cast(b.FNUMERATOR/b.FDENOMINATOR*(1+b.FSCRAPRATE/100) as nvarchar(250)) 用量,
                                   b.FNUMERATOR 分子,b.FDENOMINATOR 分母,b.FSCRAPRATE 变动损耗率,c.F_YTC_DECIMAL8 物料单价,
                                   G.FNAME 父项物料单位

                            FROM T_ENG_BOM A
                            INNER JOIN dbo.T_ENG_BOMCHILD b ON a.FID=b.FID

                            INNER JOIN dbo.T_BD_MATERIAL C ON B.FMATERIALID=C.FMATERIALID
                            INNER JOIN dbo.T_BD_MATERIAL_L D ON C.FMATERIALID=D.FMATERIALID
                            INNER JOIN dbo.T_BD_MATERIALBASE E ON D.FMATERIALID=E.FMATERIALID

                            INNER JOIN dbo.T_BD_UNIT_L g ON a.FUNITID=g.FUNITID AND g.FLOCALEID <>1033

                            WHERE A.FFORBIDSTATUS='A' --BOM禁用状态:否
                            AND A.FDOCUMENTSTATUS='C' --BOM审核状态:已审核
                            AND C.FDOCUMENTSTATUS='C' --物料审核状态:已审核
                            AND C.FFORBIDSTATUS='A'   --物料禁用状态:否
                            AND D.FLOCALEID='2052'
                            AND CONVERT(varchar(100), a.FMODIFYDATE, 20)= (
												                            SELECT CONVERT(varchar(100), MAX(A1.FMODIFYDATE), 20)
												                            FROM T_ENG_BOM A1
												                            WHERE A1.FMATERIALID=A.FMATERIALID
                                                                            AND a1.FFORBIDSTATUS='A'   --BOM禁用状态:否
																			AND a1.FDOCUMENTSTATUS='C' --BOM审核状态:已审核
											                               )  --获取最大的‘修改日期’记录
                            --检测获取最大的BOM编码
							AND A.FNUMBER= (
											SELECT  MAX(X.FNUMBER)
											FROM T_ENG_BOM X
											WHERE X.FMATERIALID=A.FMATERIALID
											AND X.FFORBIDSTATUS='A'     --BOM禁用状态:否
											AND x.FDOCUMENTSTATUS='C'   --BOM审核状态:已审核
										)
                            --AND A.FMATERIALID='136357'
                            --AND A.FNUMBER='QQ-G5-0001_V1.7'
                            ORDER BY a.FMATERIALID,e.FERPCLSID--,a.FMODIFYDATE DESC
                        ";

            return _result;
        }

        /// <summary>
        /// 获取原材料物料记录(Bom明细调用新物料时使用) 注:材料来源为‘原材料’
        /// </summary>
        /// <param name="searchid">0:全部查询</param>
        /// <param name="searchvalue">查询值</param>
        /// <returns></returns>
        public string Get_MaterialDtl(int searchid, string searchvalue)
        {
            //全部查询
            if (searchid == 0 || searchvalue=="")
            {
                _result = $@"
                            SELECT a.FMATERIALID,a.FNUMBER 物料编码,
                                   d.FNAME 物料名称,
                                   c.FDATAVALUE 物料分组,
                                   d.FSPECIFICATION '规格型号',g.FNAME '基本单位',a.F_YTC_DECIMAL8 最新采购价格,
                                   CASE CHARINDEX('%',d.FNAME) WHEN 0 THEN d.FNAME ELSE REPLACE(D.FNAME,'%','') END 物料名称1

                            FROM dbo.T_BD_MATERIAL a
                            INNER JOIN dbo.T_BAS_ASSISTANTDATAENTRY b ON a.F_YTC_ASSISTANT5=b.FENTRYID
                            INNER JOIN dbo.T_BAS_ASSISTANTDATAENTRY_L c ON b.FENTRYID=c.FENTRYID
                            INNER JOIN dbo.T_BD_MATERIAL_L d ON a.FMATERIALID=d.FMATERIALID

                            INNER JOIN dbo.T_BD_MATERIALBASE e ON a.FMATERIALID=e.FMATERIALID
                            INNER JOIN dbo.T_BD_UNIT f ON e.FBASEUNITID=f.FUNITID
                            INNER JOIN dbo.T_BD_UNIT_L g ON f.FUNITID=g.FUNITID

                            WHERE c.FDATAVALUE IN('原漆半成品','原材料','原漆')
                            AND a.FDOCUMENTSTATUS='C'
                            AND a.FFORBIDSTATUS='A' --物料禁用状态:否
                            AND d.FLOCALEID=2052
                            AND g.FLOCALEID=2052
                            ORDER BY A.FMATERIALID
                        ";
            }
            else
            {
                //按照‘物料名称’进行查询
                if (searchid == 1)
                {
                    _result = $@"
                            SELECT a.FMATERIALID,a.FNUMBER 物料编码,
                                   d.FNAME 物料名称,
                                   c.FDATAVALUE 物料分组,
                                   d.FSPECIFICATION '规格型号',g.FNAME '基本单位',a.F_YTC_DECIMAL8 最新采购价格,
                                   CASE CHARINDEX('%',d.FNAME) WHEN 0 THEN d.FNAME ELSE REPLACE(D.FNAME,'%','') END 物料名称1

                            FROM dbo.T_BD_MATERIAL a
                            INNER JOIN dbo.T_BAS_ASSISTANTDATAENTRY b ON a.F_YTC_ASSISTANT5=b.FENTRYID
                            INNER JOIN dbo.T_BAS_ASSISTANTDATAENTRY_L c ON b.FENTRYID=c.FENTRYID
                            INNER JOIN dbo.T_BD_MATERIAL_L d ON a.FMATERIALID=d.FMATERIALID

                            INNER JOIN dbo.T_BD_MATERIALBASE e ON a.FMATERIALID=e.FMATERIALID
                            INNER JOIN dbo.T_BD_UNIT f ON e.FBASEUNITID=f.FUNITID
                            INNER JOIN dbo.T_BD_UNIT_L g ON f.FUNITID=g.FUNITID

                            WHERE c.FDATAVALUE IN('原漆半成品','原材料','原漆')
                            AND D.FNAME LIKE '%{searchvalue}%'
                            AND a.FDOCUMENTSTATUS='C'
                            AND a.FFORBIDSTATUS='A' --物料禁用状态:否
                            AND d.FLOCALEID=2052
                            AND g.FLOCALEID=2052
                            ORDER BY A.FMATERIALID
                        ";
                }
                //按照'物料编码'进行查询
                else
                {
                    _result = $@"
                            SELECT a.FMATERIALID,a.FNUMBER 物料编码,
                                   d.FNAME 物料名称,
                                   c.FDATAVALUE 物料分组,
                                   d.FSPECIFICATION '规格型号',g.FNAME '基本单位',a.F_YTC_DECIMAL8 最新采购价格,
                                   CASE CHARINDEX('%',d.FNAME) WHEN 0 THEN d.FNAME ELSE REPLACE(D.FNAME,'%','') END 物料名称1

                            FROM dbo.T_BD_MATERIAL a
                            INNER JOIN dbo.T_BAS_ASSISTANTDATAENTRY b ON a.F_YTC_ASSISTANT5=b.FENTRYID
                            INNER JOIN dbo.T_BAS_ASSISTANTDATAENTRY_L c ON b.FENTRYID=c.FENTRYID
                            INNER JOIN dbo.T_BD_MATERIAL_L d ON a.FMATERIALID=d.FMATERIALID

                            INNER JOIN dbo.T_BD_MATERIALBASE e ON a.FMATERIALID=e.FMATERIALID
                            INNER JOIN dbo.T_BD_UNIT f ON e.FBASEUNITID=f.FUNITID
                            INNER JOIN dbo.T_BD_UNIT_L g ON f.FUNITID=g.FUNITID

                            WHERE c.FDATAVALUE IN('原漆半成品','原材料','原漆')
                            AND a.FNUMBER LIKE '%{searchvalue}%'
                            AND a.FDOCUMENTSTATUS='C'
                            AND a.FFORBIDSTATUS='A' --物料禁用状态:否
                            AND d.FLOCALEID=2052
                            AND g.FLOCALEID=2052
                            ORDER BY A.FMATERIALID
                        ";
                }
            }

            return _result;
        }

        /// <summary>
        /// 查询‘新产品报价单历史记录’ (ShowMaterialDeatailFrm.cs使用)
        /// </summary>
        /// <param name="searchid"></param>
        /// <param name="searchvalue"></param>
        /// <returns></returns>
        public string SearchBomHistory(int searchid, string searchvalue)
        {
            //全部查询
            if (searchid == 0 || searchvalue == "")
            {
                _result = $@"
                                SELECT DISTINCT A.MaterialID,A.MaterialCode 物料编码,A.MaterialName 物料名称,A.MaterialPrice 物料单价,c.OAorderno 对应OA流水号,b.ProductName 对应产品名称
                                FROM dbo.T_OfferOrderEntry A
                                INNER JOIN dbo.T_OfferOrderHead b ON a.Headid=b.Headid
                                INNER JOIN dbo.T_OfferOrder c ON b.FId=c.FId
                                WHERE c.Fstatus=0
                                ORDER BY c.OAorderno,a.MaterialID
                            ";
            }
            //按‘物料名称’进行查询
            else if (searchid == 1)
            {
                _result = $@"
                                SELECT DISTINCT A.MaterialID,A.MaterialCode 物料编码,A.MaterialName 物料名称,A.MaterialPrice 物料单价,c.OAorderno 对应OA流水号,b.ProductName 对应产品名称
                                FROM dbo.T_OfferOrderEntry A
                                INNER JOIN dbo.T_OfferOrderHead b ON a.Headid=b.Headid
                                INNER JOIN dbo.T_OfferOrder c ON b.FId=c.FId
                                WHERE c.Fstatus = 0
                                AND A.MaterialName LIKE '%{searchvalue}%'
                                ORDER BY c.OAorderno,a.MaterialID
                            ";
            }
            //按‘物料编码’进行查询
            else if (searchid == 2)
            {
                _result = $@"
                                SELECT DISTINCT A.MaterialID,A.MaterialCode 物料编码,A.MaterialName 物料名称,A.MaterialPrice 物料单价,c.OAorderno 对应OA流水号,b.ProductName 对应产品名称
                                FROM dbo.T_OfferOrderEntry A
                                INNER JOIN dbo.T_OfferOrderHead b ON a.Headid=b.Headid
                                INNER JOIN dbo.T_OfferOrder c ON b.FId=c.FId
                                WHERE c.Fstatus = 0
                                AND A.MaterialCode LIKE '%{searchvalue}%'
                                ORDER BY c.OAorderno,a.MaterialID
                            ";
            }
            //按‘OA流水号’进行查询
            else if(searchid == 3)
            {
                _result = $@"
                                SELECT DISTINCT A.MaterialID,A.MaterialCode 物料编码,A.MaterialName 物料名称,A.MaterialPrice 物料单价,c.OAorderno 对应OA流水号,b.ProductName 对应产品名称
                                FROM dbo.T_OfferOrderEntry A
                                INNER JOIN dbo.T_OfferOrderHead b ON a.Headid=b.Headid
                                INNER JOIN dbo.T_OfferOrder c ON b.FId=c.FId
                                WHERE c.Fstatus = 0
                                AND c.OAorderno LIKE '%{searchvalue}%'
                                ORDER BY c.OAorderno,a.MaterialID
                            ";
            }
            return _result;
        }

        /// <summary>
        /// 查询K3客户信息
        /// </summary>
        /// <returns></returns>
        public string SearchK3CustomerList(int searchid, string searchvalue)
        {
            //全部查询
            if (searchid == 0 || searchvalue == "")
            {
                _result = $@"
                                SELECT A.FNUMBER 客户编码,B.FNAME 客户名称,a.FADDRESS 客户地址,a.f_ytc_text45 联系电话
                                FROM dbo.T_BD_CUSTOMER A
                                INNER JOIN dbo.T_BD_CUSTOMER_L B ON A.FCUSTID=B.FCUSTID
                                WHERE a.FDOCUMENTSTATUS='C'
                            ";
            }
            //按‘客户编码’进行查询
            else if (searchid == 1)
            {
                _result = $@"
                                SELECT A.FNUMBER 客户编码,B.FNAME 客户名称,a.FADDRESS 客户地址,a.f_ytc_text45 联系电话
                                FROM dbo.T_BD_CUSTOMER A
                                INNER JOIN dbo.T_BD_CUSTOMER_L B ON A.FCUSTID=B.FCUSTID
                                WHERE a.FDOCUMENTSTATUS='C'
                                AND A.FNUMBER LIKE '%{searchvalue}%'
                            ";
            }
            //按‘客户名称’进行查询
            else if (searchid == 2)
            {
                _result = $@"
                                SELECT A.FNUMBER 客户编码,B.FNAME 客户名称,a.FADDRESS 客户地址,a.f_ytc_text45 联系电话
                                FROM dbo.T_BD_CUSTOMER A
                                INNER JOIN dbo.T_BD_CUSTOMER_L B ON A.FCUSTID=B.FCUSTID
                                WHERE a.FDOCUMENTSTATUS='C'
                                AND B.FNAME LIKE '%{searchvalue}%'
                            ";
            }
            return _result;
        }

        /// <summary>
        /// 检测OA流水号是否存在
        /// </summary>
        /// <param name="orderno"></param>
        /// <returns></returns>
        public string SearchOaOrderInclud(string orderno)
        {
            _result = $@"
                           SELECT COUNT(*) FROM dbo.T_OfferOrder a
                           WHERE a.OAorderno='{orderno}' 
                        ";
            return _result;
        }

        /// <summary>
        /// 获取最新的FID值
        /// </summary>
        /// <returns></returns>
        public string GetNewFidValue()
        {
            _result = @"
                           DECLARE
	                            @id INT;
                            BEGIN
	                            INSERT INTO dbo.T_OfferOrder_KEY( Column1 )
	                            VALUES  (1)

	                            SELECT @id=Id FROM dbo.T_OfferOrder_KEY

	                            DELETE FROM dbo.T_OfferOrder_KEY

	                            SELECT @id
                            END
                       ";
            return _result;
        }

        /// <summary>
        /// 获取最新的Headid值
        /// </summary>
        /// <returns></returns>
        public string GetNewHeadidValue()
        {
            _result = @"
                            DECLARE
	                            @id INT;
                            BEGIN
	                            INSERT INTO dbo.T_OfferOrderHead_KEY( Column1 )
	                            VALUES  (1)

	                            SELECT @id=Id FROM dbo.T_OfferOrderHead_KEY

	                            DELETE FROM dbo.T_OfferOrderHead_KEY

	                            SELECT @id
                            END
                       ";
            return _result;
        }

        /// <summary>
        /// 获取最新的Entryid值
        /// </summary>
        /// <returns></returns>
        public string GetNewEntryidValue()
        {
            _result = @"
                            DECLARE
	                            @id INT;
                            BEGIN
	                            INSERT INTO dbo.T_OfferOrderEntry_KEY( Column1 )
	                            VALUES  (1)

	                            SELECT @id=Id FROM dbo.T_OfferOrderEntry_KEY

	                            DELETE FROM dbo.T_OfferOrderEntry_KEY

	                            SELECT @id
                            END
                       ";
            return _result;
        }

        /// <summary>
        /// 根据表名获取查询表体语句(更新时使用) 只显示TOP 1记录
        /// </summary>
        /// <param name="tableName"></param>
        /// <returns></returns>
        public string SearchUpdateTable(string tableName)
        {
            _result =$@"
                          SELECT Top 1 a.*
                          FROM {tableName} a
                        ";
            return _result;
        }

        /// <summary>
        /// 更新语句
        /// </summary>
        /// <param name="tablename"></param>
        /// <returns></returns>
        public string UpdateEntry(string tablename)
        {
            switch (tablename)
            {
                case "T_OfferOrder":
                    _result = @"UPDATE dbo.T_OfferOrder SET OAorderno=@OAorderno,Fstatus=@Fstatus,ConfirmDt=@ConfirmDt,CreateDt=@CreateDt,
                                                            CreateName=@CreateName,Useid=@Useid,UserName=@UserName,Typeid=@Typeid,DevGroupid=@DevGroupid
                                WHERE FId=@FId";
                    break;
                case "T_OfferOrderHead":
                    _result = @"UPDATE dbo.T_OfferOrderHead SET ProductName=@ProductName,Bao=@Bao,ProductMi=@ProductMi,MaterialQty=@MaterialQty,BaoQty=@BaoQty,RenQty=@RenQty,KGQty=@KGQty,
                                                                LQty=@LQty,FiveQty=@FiveQty,FourFiveQty=@FourFiveQty,FourQty=@FourQty,Fremark=@Fremark,FBomOrder=@FBomOrder,FPrice=@FPrice,
                                                                CustName=@CustName
                                WHERE Headid=@Headid
                                ";
                    break;
                case "T_OfferOrderEntry":
                    _result = @"UPDATE dbo.T_OfferOrderEntry SET MaterialID=@MaterialID,MaterialCode=@MaterialCode,MaterialName=@MaterialName,
                                                                 PeiQty=@PeiQty,ratioQty=@ratioQty,MaterialPrice=@MaterialPrice,MaterialAmount=@MaterialAmount,
                                                                 Remark=@Remark,LastChangeUser=@LastChangeUser,LastChanageDt=@LastChanageDt
                                WHERE Entryid=@Entryid
                               ";
                    break;
                case "T_AD_User":
                    _result = @"
                                    UPDATE dbo.T_AD_User SET ApplyId=@ApplyId,CanBackConfirm=@CanBackConfirm,Readid=@Readid,Addid=@Addid,UserRelid=@UserRelid
                                    WHERE Userid=@Userid
                                ";
                    break;
                case "T_BD_UserGroup":
                    _result = @"
                                    UPDATE dbo.T_BD_UserGroup SET GroupId=@Groupid, Parentid=@Parentid, GroupName=@GroupName,CreateName=@CreateName,CreateDt=@CreateDt
                                    WHERE GroupId=@Groupid
                               ";
                    break;
                case "T_BD_UserGroupDtl":
                    _result = @"
                                    UPDATE dbo.T_BD_UserGroupDtl SET Groupid=@Groupid,Dtlid=@Dtlid,UserName=@UserName,K3UserGroup=@K3UserGroup,
                                                                     K3UserPhone=@K3UserPhone,CreateName=@CreateName,CreateDt=@CreateDt
                                    WHERE Dtlid=@Dtlid
                               ";
                    break;
            }
            return _result;
        }

        /// <summary>
        /// 删除指定行记录(主要针对T_OfferOrderEntry表)
        /// </summary>
        /// <returns></returns>
        public string DelEntry(string entryid)
        {
            _result = $@"
                            DELETE FROM dbo.T_OfferOrderEntry WHERE Entryid IN ({entryid})
                       ";
            return _result;
        }

        /// <summary>
        /// (根据Headid为条件 T_OfferOrderHead 以及 T_OfferOrderEntry 对应的记录)
        /// </summary>
        /// <param name="headid"></param>
        /// <returns></returns>
        public string DelOrderHeadAndEntry(string headid)
        {
            _result = $@"
                           DELETE FROM dbo.T_OfferOrderHead WHERE Headid IN ({headid})

                           DELETE FROM dbo.T_OfferOrderEntry WHERE Headid IN ({headid})
                        ";
            return _result;
        }

        /// <summary>
        /// 删除指定记录-基础资料:用户组别使用
        /// </summary>
        /// <param name="id">0:表头删除 1:表体删除</param>
        /// <param name="fidlist"></param>
        /// <returns></returns>
        public string DelGroupRecord(int id,string fidlist)
        {
            if (id == 0)
            {
                _result = $@"
                            DELETE FROM dbo.T_BD_UserGroup WHERE Groupid IN ({fidlist})
                            ";
            }
            else
            {
                _result = $@"
                            DELETE FROM dbo.T_BD_UserGroupDtl WHERE Dtlid IN ({fidlist})
                            ";
            }
            return _result;
        }

        #region 暂存功能使用-(注:使用FID值)

        public string DelTempOrder(int fid)
        {
            _result = $@"
                            DELETE FROM dbo.T_TempOrderEntry
                            WHERE EXISTS (
			                                 SELECT * FROM dbo.T_TempOrderHead A
				                             INNER JOIN dbo.T_TempOrder B ON A.FId=B.FId
				                             WHERE A.Headid=dbo.T_TempOrderEntry.Headid
				                             AND B.FId ='{fid}'
			                              )

                            DELETE FROM dbo.T_TempOrderHead
                            WHERE EXISTS (
                                            SELECT * FROM dbo.T_TempOrder A
				                            WHERE dbo.T_TempOrderHead.FId=A.FId
				                            AND A.FId ='{fid}'
			                             )

                            DELETE FROM dbo.T_TempOrder WHERE FId ='{fid}'
                        ";
            return _result;
        }
        #endregion


        /////////////////////////////////////查询端使用//////////////////////////////////////////////////////

        /// <summary>
        /// 查询T_OfferOrderHead表的Headid值(ImportDt.cs判断是否插入时使用)
        /// </summary>
        /// <returns></returns>
        public string SearchOfferHeadDt()
        {
            _result = @"SELECT Headid FROM dbo.T_OfferOrderHead";
            return _result;
        }

        /// <summary>
        /// Main查询及查询端使用(注:当createname为空时,就表示查询端使用,反之是Main主窗体页使用)
        /// 当Createname为空时,添加Userid为条件=>主要根据Userid带出与它用户关联的单据记录(结合用户组别等权限表)
        /// 注:找出除登入用户外关联的用户单据记录(前提:Createname为空时)
        /// </summary>
        /// <returns></returns>
        public string SearchBomList(int typeid,string value,string cratename,string dpstart,string dpend)
        {
            if (cratename == "")
            {
                //OA流水号
                if (typeid == 0)
                {
                    _result =
                        $@"
                                SELECT Y.FId,Y.OA流水号,
                                        Y.研发类别,
                                        Y.产品名称,Y.单据状态,Y.创建日期,
                                        Y.审核日期,Y.创建人,
                                        Y.单据类型,
                                        Y.产品成本含税小计,Y.包装规格,Y.包装成本,Y.人工制造费用,
	                                    Y.[30%],Y.[38%],
	                                    Y.[40%],Y.[45%],Y.[50%] 
                                FROM (
                                --查询只有表头信息的记录
                                /*SELECT A.FId,A.OAorderno OA流水号,
                                        CASE A.DevGroupid WHEN 1 THEN '地坪漆' WHEN 2 THEN '高温漆' WHEN 3 THEN '水性大巴轨交' WHEN 4 THEN '水性工程机械' WHEN 5 THEN '水性头盔' 
		                                        WHEN 6 THEN '水性小工业' WHEN 7 THEN '水性修补' WHEN 8 THEN '油性保险杠' WHEN 9 THEN '油性标识标牌' WHEN 10 THEN '油性大巴轨交' WHEN 11 THEN '油性汽车配件'
			                                    WHEN 12 THEN '油性小工业' WHEN 13 THEN '油性修补' WHEN 14 THEN '原子灰'
		                                ELSE '' END 研发类别,
                                        null 产品名称,CASE A.Fstatus WHEN 0 THEN '已审核' ELSE '反审核' END 单据状态,CONVERT(varchar(100), A.CreateDt, 23)  创建日期,
                                        CONVERT(VARCHAR(100),A.ConfirmDt,23) 审核日期,A.CreateName 创建人,
                                        CASE a.Typeid WHEN 0 THEN 'BOM成本报价单' WHEN 1 THEN '新产品成本报价单' WHEN 2 THEN '空白报价单' END 单据类型,
                                        NULL '产品成本含税小计',null '包装规格',null '包装成本',null '人工制造费用',
		                                null [30%],null [38%],
		                                null [40%],null [45%],null [50%]

                                FROM dbo.T_OfferOrder a
                                WHERE NOT EXISTS (
                                                    SELECT * FROM dbo.T_OfferOrderHead b
					                                WHERE a.FId=b.FId
                                                    )

                                UNION*/

                                SELECT A.FId,A.OAorderno OA流水号,
                                        CASE A.DevGroupid WHEN 1 THEN '地坪漆' WHEN 2 THEN '高温漆' WHEN 3 THEN '水性大巴轨交' WHEN 4 THEN '水性工程机械' WHEN 5 THEN '水性头盔' 
		                                        WHEN 6 THEN '水性小工业' WHEN 7 THEN '水性修补' WHEN 8 THEN '油性保险杠' WHEN 9 THEN '油性标识标牌' WHEN 10 THEN '油性大巴轨交' WHEN 11 THEN '油性汽车配件'
			                                    WHEN 12 THEN '油性小工业' WHEN 13 THEN '油性修补' WHEN 14 THEN '原子灰'
		                                ELSE '' END 研发类别,
                                        b.ProductName 产品名称,CASE A.Fstatus WHEN 0 THEN '已审核' ELSE '反审核' END 单据状态,CONVERT(varchar(100), A.CreateDt, 23)  创建日期,
                                        CONVERT(VARCHAR(100),A.ConfirmDt,23) 审核日期,A.CreateName 创建人,
                                        CASE a.Typeid WHEN 0 THEN 'BOM成本报价单' WHEN 1 THEN '新产品成本报价单' WHEN 2 THEN '空白报价单' END 单据类型,
                                        x.物料成本和 '产品成本含税小计',b.Bao '包装规格',b.BaoQty '包装成本',b.RenQty '人工制造费用',
		                                ROUND(b.KGQty/(1-30/100),4) [30%],ROUND(b.KGQty/(1-38/100),4) [38%],
		                                b.FourQty [40%],b.FourFiveQty [45%],b.FiveQty [50%]

                                FROM dbo.T_OfferOrder A
                                INNER JOIN dbo.T_OfferOrderHead b ON a.FId=b.FId
                                INNER JOIN (
				                                SELECT x.FId,SUM(x2.MaterialAmount) 物料成本和
				                                FROM dbo.T_OfferOrder x
				                                INNER JOIN dbo.T_OfferOrderHead x1 ON x.FId=x1.FId
				                                INNER JOIN dbo.T_OfferOrderEntry x2 ON x1.Headid=x2.Headid
				                                GROUP BY x.FId
			                                )x ON x.FId=a.FId
                                --WHERE a.OAorderno LIKE '%{value}%'
                                --用户权限关联
                                AND  A.CreateName IN(
						                                SELECT DISTINCT x3.UserName
						                                FROM dbo.T_AD_User X
						                                INNER JOIN dbo.T_AD_RelUser X1 ON X.Userid=X1.Userid
						                                INNER JOIN dbo.T_BD_UserGroup X2 ON X1.Groupid=X2.GroupId
						                                INNER JOIN dbo.T_BD_UserGroupDtl X3 ON X2.GroupId=X3.Groupid
                                                        --排除不包含的用户信息
						                                WHERE NOT EXISTS (
											                                    SELECT NULL
											                                    FROM dbo.T_AD_RelUserDtl X4
											                                    WHERE X4.Userid=X.Userid
											                                    AND X4.Groupid=X3.Groupid
											                                    AND X4.Dtlid=X3.Dtlid
									                                        )                                                        
					                                    AND X.UserRelid=1                                    --表示需关联用户
						                                AND X.Userid='{GlobalClasscs.User.UserId}'           --以登录用户ID作为条件   
						                                AND X3.UserName<>'{GlobalClasscs.User.StrUsrName}'   --不包含登入用户名
					                                    )
                                --1)包含T_OfferOrder.DevGroupid=0的记录 2)根据Userid找到其对应的关联‘研发类别’信息
                                AND EXISTS(
			                                    SELECT NULL 
			                                    FROM (
			                                            SELECT X1.UserName,0 DevGroupid
					                                    FROM dbo.T_BD_UserGroupDtl X1

					                                    UNION

					                                    SELECT x1.UserName,x2.DevGroupid
					                                    FROM dbo.T_BD_UserGroupDtl X1
					                                    INNER JOIN T_AD_RelUserDevGroup X2 ON X1.Groupid=X2.Groupid AND x1.Dtlid=x2.Dtlid 
																			                                        AND x2.Userid='{GlobalClasscs.User.UserId}'
			                                        )X 
			                                    WHERE A.CreateName=X.UserName AND A.DevGroupid=X.DevGroupid
                                            )            

                                )Y
                                WHERE Y.OA流水号 LIKE '%{value}%'--'%19500%'
                                ORDER BY Y.创建日期 DESC
                            ";
                }
                //产品名称
                else if (typeid == 1)
                {
                    _result =
                        $@"
                                SELECT Y.FId,Y.OA流水号,
                                       Y.研发类别,
                                       Y.产品名称,Y.单据状态,Y.创建日期,
                                       Y.审核日期,Y.创建人,
                                       Y.单据类型,
                                       Y.产品成本含税小计,Y.包装规格,Y.包装成本,Y.人工制造费用,
	                                   Y.[30%],Y.[38%],
	                                   Y.[40%],Y.[45%],Y.[50%] 
                                FROM (
                                SELECT A.FId,A.OAorderno OA流水号,
                                        CASE A.DevGroupid WHEN 1 THEN '地坪漆' WHEN 2 THEN '高温漆' WHEN 3 THEN '水性大巴轨交' WHEN 4 THEN '水性工程机械' WHEN 5 THEN '水性头盔' 
		                                     WHEN 6 THEN '水性小工业' WHEN 7 THEN '水性修补' WHEN 8 THEN '油性保险杠' WHEN 9 THEN '油性标识标牌' WHEN 10 THEN '油性大巴轨交' WHEN 11 THEN '油性汽车配件'
			                                 WHEN 12 THEN '油性小工业' WHEN 13 THEN '油性修补' WHEN 14 THEN '原子灰'
		                                ELSE '' END 研发类别,
                                        b.ProductName 产品名称,CASE A.Fstatus WHEN 0 THEN '已审核' ELSE '反审核' END 单据状态,CONVERT(varchar(100), A.CreateDt, 23)  创建日期,
                                        CONVERT(VARCHAR(100),A.ConfirmDt,23) 审核日期,A.CreateName 创建人,
                                        CASE a.Typeid WHEN 0 THEN 'BOM成本报价单' WHEN 1 THEN '新产品成本报价单' WHEN 2 THEN '空白报价单' END 单据类型,
                                        x.物料成本和 '产品成本含税小计',b.Bao '包装规格',b.BaoQty '包装成本',b.RenQty '人工制造费用',
		                                ROUND(b.KGQty/(1-30/100),4) [30%],ROUND(b.KGQty/(1-38/100),4) [38%],
		                                b.FourQty [40%],b.FourFiveQty [45%],b.FiveQty [50%]

                                FROM dbo.T_OfferOrder A
                                INNER JOIN dbo.T_OfferOrderHead b ON a.FId=b.FId
                                INNER JOIN (
				                                SELECT x.FId,SUM(x2.MaterialAmount) 物料成本和
				                                FROM dbo.T_OfferOrder x
				                                INNER JOIN dbo.T_OfferOrderHead x1 ON x.FId=x1.FId
				                                INNER JOIN dbo.T_OfferOrderEntry x2 ON x1.Headid=x2.Headid
				                                GROUP BY x.FId
			                                )x ON x.FId=a.FId
                                --用户权限关联
                                AND  A.CreateName IN(
						                                SELECT DISTINCT x3.UserName
						                                FROM dbo.T_AD_User X
						                                INNER JOIN dbo.T_AD_RelUser X1 ON X.Userid=X1.Userid
						                                INNER JOIN dbo.T_BD_UserGroup X2 ON X1.Groupid=X2.GroupId
						                                INNER JOIN dbo.T_BD_UserGroupDtl X3 ON X2.GroupId=X3.Groupid
                                                        --排除不包含的用户信息
						                                WHERE NOT EXISTS (
											                                    SELECT NULL
											                                    FROM dbo.T_AD_RelUserDtl X4
											                                    WHERE X4.Userid=X.Userid
											                                    AND X4.Groupid=X3.Groupid
											                                    AND X4.Dtlid=X3.Dtlid
									                                        )                                                        
					                                    AND X.UserRelid=1                                    --表示需关联用户
						                                AND X.Userid='{GlobalClasscs.User.UserId}'           --以登录用户ID作为条件   
						                                AND X3.UserName<>'{GlobalClasscs.User.StrUsrName}'   --不包含登入用户名
					                                    )
                                --1)包含T_OfferOrder.DevGroupid=0的记录 2)根据Userid找到其对应的关联‘研发类别’信息
                                AND EXISTS(
			                                 SELECT NULL 
			                                 FROM (
			                                         SELECT X1.UserName,0 DevGroupid
					                                 FROM dbo.T_BD_UserGroupDtl X1

					                                 UNION

					                                 SELECT x1.UserName,x2.DevGroupid
					                                 FROM dbo.T_BD_UserGroupDtl X1
					                                 INNER JOIN T_AD_RelUserDevGroup X2 ON X1.Groupid=X2.Groupid AND x1.Dtlid=x2.Dtlid 
																			                                     AND x2.Userid='{GlobalClasscs.User.UserId}'
			                                      )X 
			                                 WHERE A.CreateName=X.UserName AND A.DevGroupid=X.DevGroupid
                                          )            

                                )Y
                                WHERE Y.产品名称 LIKE '%{value}%'
                                ORDER BY Y.创建日期 DESC
                            ";
                }
                //创建日期
                else if (typeid == 2)
                {
                    _result =
                        $@"
                                SELECT Y.FId,Y.OA流水号,
                                       Y.研发类别,
                                       Y.产品名称,Y.单据状态,Y.创建日期,
                                       Y.审核日期,Y.创建人,
                                       Y.单据类型,
                                       Y.产品成本含税小计,Y.包装规格,Y.包装成本,Y.人工制造费用,
	                                   Y.[30%],Y.[38%],
	                                   Y.[40%],Y.[45%],Y.[50%] 
                                FROM (
                                SELECT A.FId,A.OAorderno OA流水号,
                                        CASE A.DevGroupid WHEN 1 THEN '地坪漆' WHEN 2 THEN '高温漆' WHEN 3 THEN '水性大巴轨交' WHEN 4 THEN '水性工程机械' WHEN 5 THEN '水性头盔' 
		                                     WHEN 6 THEN '水性小工业' WHEN 7 THEN '水性修补' WHEN 8 THEN '油性保险杠' WHEN 9 THEN '油性标识标牌' WHEN 10 THEN '油性大巴轨交' WHEN 11 THEN '油性汽车配件'
			                                 WHEN 12 THEN '油性小工业' WHEN 13 THEN '油性修补' WHEN 14 THEN '原子灰'
		                                ELSE '' END 研发类别,
                                        b.ProductName 产品名称,CASE A.Fstatus WHEN 0 THEN '已审核' ELSE '反审核' END 单据状态,CONVERT(varchar(100), A.CreateDt, 23)  创建日期,
                                        CONVERT(VARCHAR(100),A.ConfirmDt,23) 审核日期,A.CreateName 创建人,
                                        CASE a.Typeid WHEN 0 THEN 'BOM成本报价单' WHEN 1 THEN '新产品成本报价单' WHEN 2 THEN '空白报价单' END 单据类型,
                                        x.物料成本和 '产品成本含税小计',b.Bao '包装规格',b.BaoQty '包装成本',b.RenQty '人工制造费用',
		                                ROUND(b.KGQty/(1-30/100),4) [30%],ROUND(b.KGQty/(1-38/100),4) [38%],
		                                b.FourQty [40%],b.FourFiveQty [45%],b.FiveQty [50%]

                                FROM dbo.T_OfferOrder A
                                INNER JOIN dbo.T_OfferOrderHead b ON a.FId=b.FId
                                INNER JOIN (
				                                SELECT x.FId,SUM(x2.MaterialAmount) 物料成本和
				                                FROM dbo.T_OfferOrder x
				                                INNER JOIN dbo.T_OfferOrderHead x1 ON x.FId=x1.FId
				                                INNER JOIN dbo.T_OfferOrderEntry x2 ON x1.Headid=x2.Headid
				                                GROUP BY x.FId
			                                )x ON x.FId=a.FId
                                --用户权限关联
                                AND  A.CreateName IN(
						                                SELECT DISTINCT x3.UserName
						                                FROM dbo.T_AD_User X
						                                INNER JOIN dbo.T_AD_RelUser X1 ON X.Userid=X1.Userid
						                                INNER JOIN dbo.T_BD_UserGroup X2 ON X1.Groupid=X2.GroupId
						                                INNER JOIN dbo.T_BD_UserGroupDtl X3 ON X2.GroupId=X3.Groupid
                                                        --排除不包含的用户信息
						                                WHERE NOT EXISTS (
											                                    SELECT NULL
											                                    FROM dbo.T_AD_RelUserDtl X4
											                                    WHERE X4.Userid=X.Userid
											                                    AND X4.Groupid=X3.Groupid
											                                    AND X4.Dtlid=X3.Dtlid
									                                        )                                                        
					                                    AND X.UserRelid=1                                    --表示需关联用户
						                                AND X.Userid='{GlobalClasscs.User.UserId}'           --以登录用户ID作为条件   
						                                AND X3.UserName<>'{GlobalClasscs.User.StrUsrName}'   --不包含登入用户名
					                                    )
                                --1)包含T_OfferOrder.DevGroupid=0的记录 2)根据Userid找到其对应的关联‘研发类别’信息
                                AND EXISTS(
			                                 SELECT NULL 
			                                 FROM (
			                                         SELECT X1.UserName,0 DevGroupid
					                                 FROM dbo.T_BD_UserGroupDtl X1

					                                 UNION

					                                 SELECT x1.UserName,x2.DevGroupid
					                                 FROM dbo.T_BD_UserGroupDtl X1
					                                 INNER JOIN T_AD_RelUserDevGroup X2 ON X1.Groupid=X2.Groupid AND x1.Dtlid=x2.Dtlid 
																			                                     AND x2.Userid='{GlobalClasscs.User.UserId}'
			                                      )X 
			                                 WHERE A.CreateName=X.UserName AND A.DevGroupid=X.DevGroupid
                                          )            

                                )Y
                                WHERE Y.创建日期 >= CONVERT(VARCHAR(100),CONVERT(DATETIME,'{dpstart}'),23)
                                AND Y.创建日期 <= CONVERT(VARCHAR(100),CONVERT(DATETIME,'{dpend}'),23)
                                ORDER BY Y.创建日期 DESC
                            ";
                }
                //审核日期
                else if (typeid == 3)
                {
                    _result =
                        $@"
                                SELECT Y.FId,Y.OA流水号,
                                       Y.研发类别,
                                       Y.产品名称,Y.单据状态,Y.创建日期,
                                       Y.审核日期,Y.创建人,
                                       Y.单据类型,
                                       Y.产品成本含税小计,Y.包装规格,Y.包装成本,Y.人工制造费用,
	                                   Y.[30%],Y.[38%],
	                                   Y.[40%],Y.[45%],Y.[50%] 
                                FROM (
                                SELECT A.FId,A.OAorderno OA流水号,
                                        CASE A.DevGroupid WHEN 1 THEN '地坪漆' WHEN 2 THEN '高温漆' WHEN 3 THEN '水性大巴轨交' WHEN 4 THEN '水性工程机械' WHEN 5 THEN '水性头盔' 
		                                     WHEN 6 THEN '水性小工业' WHEN 7 THEN '水性修补' WHEN 8 THEN '油性保险杠' WHEN 9 THEN '油性标识标牌' WHEN 10 THEN '油性大巴轨交' WHEN 11 THEN '油性汽车配件'
			                                 WHEN 12 THEN '油性小工业' WHEN 13 THEN '油性修补' WHEN 14 THEN '原子灰'
		                                ELSE '' END 研发类别,
                                        b.ProductName 产品名称,CASE A.Fstatus WHEN 0 THEN '已审核' ELSE '反审核' END 单据状态,CONVERT(varchar(100), A.CreateDt, 23)  创建日期,
                                        CONVERT(VARCHAR(100),A.ConfirmDt,23) 审核日期,A.CreateName 创建人,
                                        CASE a.Typeid WHEN 0 THEN 'BOM成本报价单' WHEN 1 THEN '新产品成本报价单' WHEN 2 THEN '空白报价单' END 单据类型,
                                        x.物料成本和 '产品成本含税小计',b.Bao '包装规格',b.BaoQty '包装成本',b.RenQty '人工制造费用',
		                                ROUND(b.KGQty/(1-30/100),4) [30%],ROUND(b.KGQty/(1-38/100),4) [38%],
		                                b.FourQty [40%],b.FourFiveQty [45%],b.FiveQty [50%]

                                FROM dbo.T_OfferOrder A
                                INNER JOIN dbo.T_OfferOrderHead b ON a.FId=b.FId
                                INNER JOIN (
				                                SELECT x.FId,SUM(x2.MaterialAmount) 物料成本和
				                                FROM dbo.T_OfferOrder x
				                                INNER JOIN dbo.T_OfferOrderHead x1 ON x.FId=x1.FId
				                                INNER JOIN dbo.T_OfferOrderEntry x2 ON x1.Headid=x2.Headid
				                                GROUP BY x.FId
			                                )x ON x.FId=a.FId
                                --用户权限关联
                                AND  A.CreateName IN(
						                                SELECT DISTINCT x3.UserName
						                                FROM dbo.T_AD_User X
						                                INNER JOIN dbo.T_AD_RelUser X1 ON X.Userid=X1.Userid
						                                INNER JOIN dbo.T_BD_UserGroup X2 ON X1.Groupid=X2.GroupId
						                                INNER JOIN dbo.T_BD_UserGroupDtl X3 ON X2.GroupId=X3.Groupid
                                                        --排除不包含的用户信息
						                                WHERE NOT EXISTS (
											                                    SELECT NULL
											                                    FROM dbo.T_AD_RelUserDtl X4
											                                    WHERE X4.Userid=X.Userid
											                                    AND X4.Groupid=X3.Groupid
											                                    AND X4.Dtlid=X3.Dtlid
									                                        )                                                        
					                                    AND X.UserRelid=1                                    --表示需关联用户
						                                AND X.Userid='{GlobalClasscs.User.UserId}'           --以登录用户ID作为条件   
						                                AND X3.UserName<>'{GlobalClasscs.User.StrUsrName}'   --不包含登入用户名
					                                    )
                                --1)包含T_OfferOrder.DevGroupid=0的记录 2)根据Userid找到其对应的关联‘研发类别’信息
                                AND EXISTS(
			                                 SELECT NULL 
			                                 FROM (
			                                         SELECT X1.UserName,0 DevGroupid
					                                 FROM dbo.T_BD_UserGroupDtl X1

					                                 UNION

					                                 SELECT x1.UserName,x2.DevGroupid
					                                 FROM dbo.T_BD_UserGroupDtl X1
					                                 INNER JOIN T_AD_RelUserDevGroup X2 ON X1.Groupid=X2.Groupid AND x1.Dtlid=x2.Dtlid 
																			                                     AND x2.Userid='{GlobalClasscs.User.UserId}'
			                                      )X 
			                                 WHERE A.CreateName=X.UserName AND A.DevGroupid=X.DevGroupid
                                          )            

                                )Y
                                WHERE Y.审核日期 >= CONVERT(VARCHAR(100),CONVERT(DATETIME,'{dpstart}'),23)
                                AND Y.审核日期 <= CONVERT(VARCHAR(100),CONVERT(DATETIME,'{dpend}'),23)
                                ORDER BY Y.创建日期 DESC
                            ";
                }
                //单据状态
                else if (typeid == 4)
                {
                    _result =
                        $@"
                                SELECT Y.FId,Y.OA流水号,
                                        Y.研发类别,
                                        Y.产品名称,Y.单据状态,Y.创建日期,
                                        Y.审核日期,Y.创建人,
                                        Y.单据类型,
                                        Y.产品成本含税小计,Y.包装规格,Y.包装成本,Y.人工制造费用,
	                                    Y.[30%],Y.[38%],
	                                    Y.[40%],Y.[45%],Y.[50%] 
                                FROM (
                                SELECT A.FId,A.OAorderno OA流水号,
                                        CASE A.DevGroupid WHEN 1 THEN '地坪漆' WHEN 2 THEN '高温漆' WHEN 3 THEN '水性大巴轨交' WHEN 4 THEN '水性工程机械' WHEN 5 THEN '水性头盔' 
		                                        WHEN 6 THEN '水性小工业' WHEN 7 THEN '水性修补' WHEN 8 THEN '油性保险杠' WHEN 9 THEN '油性标识标牌' WHEN 10 THEN '油性大巴轨交' WHEN 11 THEN '油性汽车配件'
			                                    WHEN 12 THEN '油性小工业' WHEN 13 THEN '油性修补' WHEN 14 THEN '原子灰'
		                                ELSE '' END 研发类别,
                                        b.ProductName 产品名称,CASE A.Fstatus WHEN 0 THEN '已审核' ELSE '反审核' END 单据状态,CONVERT(varchar(100), A.CreateDt, 23)  创建日期,
                                        CONVERT(VARCHAR(100),A.ConfirmDt,23) 审核日期,A.CreateName 创建人,
                                        CASE a.Typeid WHEN 0 THEN 'BOM成本报价单' WHEN 1 THEN '新产品成本报价单' WHEN 2 THEN '空白报价单' END 单据类型,
                                        x.物料成本和 '产品成本含税小计',b.Bao '包装规格',b.BaoQty '包装成本',b.RenQty '人工制造费用',
		                                ROUND(b.KGQty/(1-30/100),4) [30%],ROUND(b.KGQty/(1-38/100),4) [38%],
		                                b.FourQty [40%],b.FourFiveQty [45%],b.FiveQty [50%]

                                FROM dbo.T_OfferOrder A
                                INNER JOIN dbo.T_OfferOrderHead b ON a.FId=b.FId
                                INNER JOIN (
				                                SELECT x.FId,SUM(x2.MaterialAmount) 物料成本和
				                                FROM dbo.T_OfferOrder x
				                                INNER JOIN dbo.T_OfferOrderHead x1 ON x.FId=x1.FId
				                                INNER JOIN dbo.T_OfferOrderEntry x2 ON x1.Headid=x2.Headid
				                                GROUP BY x.FId
			                                )x ON x.FId=a.FId
                                --用户权限关联
                                AND  A.CreateName IN(
						                                SELECT DISTINCT x3.UserName
						                                FROM dbo.T_AD_User X
						                                INNER JOIN dbo.T_AD_RelUser X1 ON X.Userid=X1.Userid
						                                INNER JOIN dbo.T_BD_UserGroup X2 ON X1.Groupid=X2.GroupId
						                                INNER JOIN dbo.T_BD_UserGroupDtl X3 ON X2.GroupId=X3.Groupid
                                                        --排除不包含的用户信息
						                                WHERE NOT EXISTS (
											                                    SELECT NULL
											                                    FROM dbo.T_AD_RelUserDtl X4
											                                    WHERE X4.Userid=X.Userid
											                                    AND X4.Groupid=X3.Groupid
											                                    AND X4.Dtlid=X3.Dtlid
									                                        )                                                        
					                                    AND X.UserRelid=1                                    --表示需关联用户
						                                AND X.Userid='{GlobalClasscs.User.UserId}'           --以登录用户ID作为条件   
						                                AND X3.UserName<>'{GlobalClasscs.User.StrUsrName}'   --不包含登入用户名
					                                    )
                                --1)包含T_OfferOrder.DevGroupid=0的记录 2)根据Userid找到其对应的关联‘研发类别’信息
                                AND EXISTS(
			                                    SELECT NULL 
			                                    FROM (
			                                            SELECT X1.UserName,0 DevGroupid
					                                    FROM dbo.T_BD_UserGroupDtl X1

					                                    UNION

					                                    SELECT x1.UserName,x2.DevGroupid
					                                    FROM dbo.T_BD_UserGroupDtl X1
					                                    INNER JOIN T_AD_RelUserDevGroup X2 ON X1.Groupid=X2.Groupid AND x1.Dtlid=x2.Dtlid 
																			                                        AND x2.Userid='{GlobalClasscs.User.UserId}'
			                                        )X 
			                                    WHERE A.CreateName=X.UserName AND A.DevGroupid=X.DevGroupid
                                            )            

                                )Y
                                WHERE Y.单据状态 = CASE '{value}' WHEN 0 THEN '已审核' ELSE '反审核' END
                                ORDER BY Y.创建日期 DESC
                            ";
                }
                //研发类别
                else if (typeid == 5)
                {
                    _result = $@"
                               SELECT Y.FId,Y.OA流水号,
                                        Y.研发类别,
                                        Y.产品名称,Y.单据状态,Y.创建日期,
                                        Y.审核日期,Y.创建人,
                                        Y.单据类型,
                                        Y.产品成本含税小计,Y.包装规格,Y.包装成本,Y.人工制造费用,
	                                    Y.[30%],Y.[38%],
	                                    Y.[40%],Y.[45%],Y.[50%] 
                                FROM (
                                SELECT A.FId,A.OAorderno OA流水号,
                                        CASE A.DevGroupid WHEN 1 THEN '地坪漆' WHEN 2 THEN '高温漆' WHEN 3 THEN '水性大巴轨交' WHEN 4 THEN '水性工程机械' WHEN 5 THEN '水性头盔' 
		                                        WHEN 6 THEN '水性小工业' WHEN 7 THEN '水性修补' WHEN 8 THEN '油性保险杠' WHEN 9 THEN '油性标识标牌' WHEN 10 THEN '油性大巴轨交' WHEN 11 THEN '油性汽车配件'
			                                    WHEN 12 THEN '油性小工业' WHEN 13 THEN '油性修补' WHEN 14 THEN '原子灰'
		                                ELSE '' END 研发类别,
                                        b.ProductName 产品名称,CASE A.Fstatus WHEN 0 THEN '已审核' ELSE '反审核' END 单据状态,CONVERT(varchar(100), A.CreateDt, 23)  创建日期,
                                        CONVERT(VARCHAR(100),A.ConfirmDt,23) 审核日期,A.CreateName 创建人,
                                        CASE a.Typeid WHEN 0 THEN 'BOM成本报价单' WHEN 1 THEN '新产品成本报价单' WHEN 2 THEN '空白报价单' END 单据类型,
                                        x.物料成本和 '产品成本含税小计',b.Bao '包装规格',b.BaoQty '包装成本',b.RenQty '人工制造费用',
		                                ROUND(b.KGQty/(1-30/100),4) [30%],ROUND(b.KGQty/(1-38/100),4) [38%],
		                                b.FourQty [40%],b.FourFiveQty [45%],b.FiveQty [50%]

                                FROM dbo.T_OfferOrder A
                                INNER JOIN dbo.T_OfferOrderHead b ON a.FId=b.FId
                                INNER JOIN (
				                                SELECT x.FId,SUM(x2.MaterialAmount) 物料成本和
				                                FROM dbo.T_OfferOrder x
				                                INNER JOIN dbo.T_OfferOrderHead x1 ON x.FId=x1.FId
				                                INNER JOIN dbo.T_OfferOrderEntry x2 ON x1.Headid=x2.Headid
				                                GROUP BY x.FId
			                                )x ON x.FId=a.FId
                                --用户权限关联
                                AND  A.CreateName IN(
						                                SELECT DISTINCT x3.UserName
						                                FROM dbo.T_AD_User X
						                                INNER JOIN dbo.T_AD_RelUser X1 ON X.Userid=X1.Userid
						                                INNER JOIN dbo.T_BD_UserGroup X2 ON X1.Groupid=X2.GroupId
						                                INNER JOIN dbo.T_BD_UserGroupDtl X3 ON X2.GroupId=X3.Groupid
                                                        --排除不包含的用户信息
						                                WHERE NOT EXISTS (
											                                    SELECT NULL
											                                    FROM dbo.T_AD_RelUserDtl X4
											                                    WHERE X4.Userid=X.Userid
											                                    AND X4.Groupid=X3.Groupid
											                                    AND X4.Dtlid=X3.Dtlid
									                                        )                                                        
					                                    AND X.UserRelid=1                                    --表示需关联用户
						                                AND X.Userid='{GlobalClasscs.User.UserId}'           --以登录用户ID作为条件   
						                                AND X3.UserName<>'{GlobalClasscs.User.StrUsrName}'   --不包含登入用户名
					                                    )
                                --1)包含T_OfferOrder.DevGroupid=0的记录 2)根据Userid找到其对应的关联‘研发类别’信息
                                AND EXISTS(
			                                    SELECT NULL 
			                                    FROM (
			                                            SELECT X1.UserName,0 DevGroupid
					                                    FROM dbo.T_BD_UserGroupDtl X1

					                                    UNION

					                                    SELECT x1.UserName,x2.DevGroupid
					                                    FROM dbo.T_BD_UserGroupDtl X1
					                                    INNER JOIN T_AD_RelUserDevGroup X2 ON X1.Groupid=X2.Groupid AND x1.Dtlid=x2.Dtlid 
																			                                        AND x2.Userid='{GlobalClasscs.User.UserId}'
			                                        )X 
			                                    WHERE A.CreateName=X.UserName AND A.DevGroupid=X.DevGroupid
                                            )            

                                )Y
                                WHERE Y.研发类别 = CASE '{value}' WHEN 1 THEN '地坪漆' WHEN 2 THEN '高温漆' WHEN 3 THEN '水性大巴轨交' WHEN 4 THEN '水性工程机械' WHEN 5 THEN '水性头盔' 
		                                        WHEN 6 THEN '水性小工业' WHEN 7 THEN '水性修补' WHEN 8 THEN '油性保险杠' WHEN 9 THEN '油性标识标牌' WHEN 10 THEN '油性大巴轨交' WHEN 11 THEN '油性汽车配件'
			                                    WHEN 12 THEN '油性小工业' WHEN 13 THEN '油性修补' WHEN 14 THEN '原子灰'
		                                        ELSE '' END
                                ORDER BY Y.创建日期 DESC
                        ";
                }
            }
            //当‘创建人’不为空时,执行以登入用户为条件的查询,注:此为‘主窗体’查询使用
            else
            {
                //OA流水号
                if (typeid == 0)
                {
                    _result =
                        $@"
                            SELECT Y.FId,Y.OA流水号,
                                    Y.研发类别,
                                    Y.产品名称,Y.单据状态,Y.创建日期,
                                    Y.审核日期,Y.创建人,
                                    Y.单据类型,
                                    Y.产品成本含税小计,Y.包装规格,Y.包装成本,Y.人工制造费用,
	                                Y.[30%],Y.[38%],
	                                Y.[40%],Y.[45%],Y.[50%] 
                            FROM (
                                    --查询只有表头信息的记录
                            /*SELECT A.FId,A.OAorderno OA流水号,
                            CASE A.DevGroupid WHEN 1 THEN '地坪漆' WHEN 2 THEN '高温漆' WHEN 3 THEN '水性大巴轨交' WHEN 4 THEN '水性工程机械' WHEN 5 THEN '水性头盔' 
		                            WHEN 6 THEN '水性小工业' WHEN 7 THEN '水性修补' WHEN 8 THEN '油性保险杠' WHEN 9 THEN '油性标识标牌' WHEN 10 THEN '油性大巴轨交' WHEN 11 THEN '油性汽车配件'
		                            WHEN 12 THEN '油性小工业' WHEN 13 THEN '油性修补' WHEN 14 THEN '原子灰'
                            ELSE '' END 研发类别,
                            null 产品名称,CASE A.Fstatus WHEN 0 THEN '已审核' ELSE '反审核' END 单据状态,CONVERT(varchar(100), A.CreateDt, 23)  创建日期,
                            CONVERT(VARCHAR(100),A.ConfirmDt,23) 审核日期,A.CreateName 创建人,
                            CASE a.Typeid WHEN 0 THEN 'BOM成本报价单' WHEN 1 THEN '新产品成本报价单' WHEN 2 THEN '空白报价单' END 单据类型,
                            NULL '产品成本含税小计',null '包装规格',null '包装成本',null '人工制造费用',
                            null [30%],null [38%],
                            null [40%],null [45%],null [50%]

                            FROM dbo.T_OfferOrder a
                            WHERE NOT EXISTS (
                                        SELECT * FROM dbo.T_OfferOrderHead b
			                            WHERE a.FId=b.FId
                                        )
				 
                            UNION*/

                            SELECT A.FId,A.OAorderno OA流水号,
                            CASE A.DevGroupid WHEN 1 THEN '地坪漆' WHEN 2 THEN '高温漆' WHEN 3 THEN '水性大巴轨交' WHEN 4 THEN '水性工程机械' WHEN 5 THEN '水性头盔' 
	                            WHEN 6 THEN '水性小工业' WHEN 7 THEN '水性修补' WHEN 8 THEN '油性保险杠' WHEN 9 THEN '油性标识标牌' WHEN 10 THEN '油性大巴轨交' WHEN 11 THEN '油性汽车配件'
	                            WHEN 12 THEN '油性小工业' WHEN 13 THEN '油性修补' WHEN 14 THEN '原子灰'
	                            ELSE '' END 研发类别,
                            b.ProductName 产品名称,CASE A.Fstatus WHEN 0 THEN '已审核' ELSE '反审核' END 单据状态,CONVERT(varchar(100), A.CreateDt, 23)  创建日期,
                            CONVERT(VARCHAR(100),A.ConfirmDt,23) 审核日期,A.CreateName 创建人,
                            CASE a.Typeid WHEN 0 THEN 'BOM成本报价单' WHEN 1 THEN '新产品成本报价单' WHEN 2 THEN '空白报价单' END 单据类型,
                            x.物料成本和 '产品成本含税小计',b.Bao '包装规格',b.BaoQty '包装成本',b.RenQty '人工制造费用',
                            ROUND(b.KGQty/(1-30/100),4) '30%',ROUND(b.KGQty/(1-38/100),4) '38%',
                            b.FourQty '40%',b.FourFiveQty '45%',b.FiveQty '50%' 
                            FROM dbo.T_OfferOrder A
                            INNER JOIN dbo.T_OfferOrderHead b ON a.FId=b.FId
                            INNER JOIN (
		                            SELECT x.FId,SUM(x2.MaterialAmount) 物料成本和
		                            FROM dbo.T_OfferOrder x
		                            INNER JOIN dbo.T_OfferOrderHead x1 ON x.FId=x1.FId
		                            INNER JOIN dbo.T_OfferOrderEntry x2 ON x1.Headid=x2.Headid
		                            GROUP BY x.FId
	                            )x ON x.FId=a.FId
                            )Y
                            WHERE Y.创建人='{cratename}' 
                            and Y.OA流水号 like '%{value}%'
                            order by Y.创建日期 desc  
                            ";
                }
                //产品名称
                else if (typeid == 1)
                {
                    _result =
                        $@"
                            SELECT Y.FId,Y.OA流水号,
                                    Y.研发类别,
                                    Y.产品名称,Y.单据状态,Y.创建日期,
                                    Y.审核日期,Y.创建人,
                                    Y.单据类型,
                                    Y.产品成本含税小计,Y.包装规格,Y.包装成本,Y.人工制造费用,
	                                Y.[30%],Y.[38%],
	                                Y.[40%],Y.[45%],Y.[50%] 
                            FROM (
                            SELECT A.FId,A.OAorderno OA流水号,
                            CASE A.DevGroupid WHEN 1 THEN '地坪漆' WHEN 2 THEN '高温漆' WHEN 3 THEN '水性大巴轨交' WHEN 4 THEN '水性工程机械' WHEN 5 THEN '水性头盔' 
	                            WHEN 6 THEN '水性小工业' WHEN 7 THEN '水性修补' WHEN 8 THEN '油性保险杠' WHEN 9 THEN '油性标识标牌' WHEN 10 THEN '油性大巴轨交' WHEN 11 THEN '油性汽车配件'
	                            WHEN 12 THEN '油性小工业' WHEN 13 THEN '油性修补' WHEN 14 THEN '原子灰'
	                            ELSE '' END 研发类别,
                            b.ProductName 产品名称,CASE A.Fstatus WHEN 0 THEN '已审核' ELSE '反审核' END 单据状态,CONVERT(varchar(100), A.CreateDt, 23)  创建日期,
                            CONVERT(VARCHAR(100),A.ConfirmDt,23) 审核日期,A.CreateName 创建人,
                            CASE a.Typeid WHEN 0 THEN 'BOM成本报价单' WHEN 1 THEN '新产品成本报价单' WHEN 2 THEN '空白报价单' END 单据类型,
                            x.物料成本和 '产品成本含税小计',b.Bao '包装规格',b.BaoQty '包装成本',b.RenQty '人工制造费用',
                            ROUND(b.KGQty/(1-30/100),4) '30%',ROUND(b.KGQty/(1-38/100),4) '38%',
                            b.FourQty '40%',b.FourFiveQty '45%',b.FiveQty '50%' 
                            FROM dbo.T_OfferOrder A
                            INNER JOIN dbo.T_OfferOrderHead b ON a.FId=b.FId
                            INNER JOIN (
		                            SELECT x.FId,SUM(x2.MaterialAmount) 物料成本和
		                            FROM dbo.T_OfferOrder x
		                            INNER JOIN dbo.T_OfferOrderHead x1 ON x.FId=x1.FId
		                            INNER JOIN dbo.T_OfferOrderEntry x2 ON x1.Headid=x2.Headid
		                            GROUP BY x.FId
	                            )x ON x.FId=a.FId
                            )Y
                            WHERE Y.创建人='{cratename}'
                            and Y.产品名称 like '%{value}%'
                            order by Y.创建日期 desc  
                            ";
                }
                //创建日期
                else if (typeid == 2)
                {
                    _result =
                        $@"
                            SELECT Y.FId,Y.OA流水号,
                                    Y.研发类别,
                                    Y.产品名称,Y.单据状态,Y.创建日期,
                                    Y.审核日期,Y.创建人,
                                    Y.单据类型,
                                    Y.产品成本含税小计,Y.包装规格,Y.包装成本,Y.人工制造费用,
	                                Y.[30%],Y.[38%],
	                                Y.[40%],Y.[45%],Y.[50%] 
                            FROM (
                            SELECT A.FId,A.OAorderno OA流水号,
                            CASE A.DevGroupid WHEN 1 THEN '地坪漆' WHEN 2 THEN '高温漆' WHEN 3 THEN '水性大巴轨交' WHEN 4 THEN '水性工程机械' WHEN 5 THEN '水性头盔' 
	                            WHEN 6 THEN '水性小工业' WHEN 7 THEN '水性修补' WHEN 8 THEN '油性保险杠' WHEN 9 THEN '油性标识标牌' WHEN 10 THEN '油性大巴轨交' WHEN 11 THEN '油性汽车配件'
	                            WHEN 12 THEN '油性小工业' WHEN 13 THEN '油性修补' WHEN 14 THEN '原子灰'
	                            ELSE '' END 研发类别,
                            b.ProductName 产品名称,CASE A.Fstatus WHEN 0 THEN '已审核' ELSE '反审核' END 单据状态,CONVERT(varchar(100), A.CreateDt, 23)  创建日期,
                            CONVERT(VARCHAR(100),A.ConfirmDt,23) 审核日期,A.CreateName 创建人,
                            CASE a.Typeid WHEN 0 THEN 'BOM成本报价单' WHEN 1 THEN '新产品成本报价单' WHEN 2 THEN '空白报价单' END 单据类型,
                            x.物料成本和 '产品成本含税小计',b.Bao '包装规格',b.BaoQty '包装成本',b.RenQty '人工制造费用',
                            ROUND(b.KGQty/(1-30/100),4) '30%',ROUND(b.KGQty/(1-38/100),4) '38%',
                            b.FourQty '40%',b.FourFiveQty '45%',b.FiveQty '50%' 
                            FROM dbo.T_OfferOrder A
                            INNER JOIN dbo.T_OfferOrderHead b ON a.FId=b.FId
                            INNER JOIN (
		                            SELECT x.FId,SUM(x2.MaterialAmount) 物料成本和
		                            FROM dbo.T_OfferOrder x
		                            INNER JOIN dbo.T_OfferOrderHead x1 ON x.FId=x1.FId
		                            INNER JOIN dbo.T_OfferOrderEntry x2 ON x1.Headid=x2.Headid
		                            GROUP BY x.FId
	                            )x ON x.FId=a.FId
                            )Y
                            WHERE Y.创建人='{cratename}'
                            and Y.创建日期 >= CONVERT(VARCHAR(100),CONVERT(DATETIME,'{value}'),23)
                            order by Y.创建日期 desc  
                            ";
                }
                //审核日期
                else if (typeid == 3)
                {
                    _result =
                        $@"
                            SELECT Y.FId,Y.OA流水号,
                                    Y.研发类别,
                                    Y.产品名称,Y.单据状态,Y.创建日期,
                                    Y.审核日期,Y.创建人,
                                    Y.单据类型,
                                    Y.产品成本含税小计,Y.包装规格,Y.包装成本,Y.人工制造费用,
	                                Y.[30%],Y.[38%],
	                                Y.[40%],Y.[45%],Y.[50%] 
                            FROM (
                            SELECT A.FId,A.OAorderno OA流水号,
                            CASE A.DevGroupid WHEN 1 THEN '地坪漆' WHEN 2 THEN '高温漆' WHEN 3 THEN '水性大巴轨交' WHEN 4 THEN '水性工程机械' WHEN 5 THEN '水性头盔' 
	                            WHEN 6 THEN '水性小工业' WHEN 7 THEN '水性修补' WHEN 8 THEN '油性保险杠' WHEN 9 THEN '油性标识标牌' WHEN 10 THEN '油性大巴轨交' WHEN 11 THEN '油性汽车配件'
	                            WHEN 12 THEN '油性小工业' WHEN 13 THEN '油性修补' WHEN 14 THEN '原子灰'
	                            ELSE '' END 研发类别,
                            b.ProductName 产品名称,CASE A.Fstatus WHEN 0 THEN '已审核' ELSE '反审核' END 单据状态,CONVERT(varchar(100), A.CreateDt, 23)  创建日期,
                            CONVERT(VARCHAR(100),A.ConfirmDt,23) 审核日期,A.CreateName 创建人,
                            CASE a.Typeid WHEN 0 THEN 'BOM成本报价单' WHEN 1 THEN '新产品成本报价单' WHEN 2 THEN '空白报价单' END 单据类型,
                            x.物料成本和 '产品成本含税小计',b.Bao '包装规格',b.BaoQty '包装成本',b.RenQty '人工制造费用',
                            ROUND(b.KGQty/(1-30/100),4) '30%',ROUND(b.KGQty/(1-38/100),4) '38%',
                            b.FourQty '40%',b.FourFiveQty '45%',b.FiveQty '50%' 
                            FROM dbo.T_OfferOrder A
                            INNER JOIN dbo.T_OfferOrderHead b ON a.FId=b.FId
                            INNER JOIN (
		                            SELECT x.FId,SUM(x2.MaterialAmount) 物料成本和
		                            FROM dbo.T_OfferOrder x
		                            INNER JOIN dbo.T_OfferOrderHead x1 ON x.FId=x1.FId
		                            INNER JOIN dbo.T_OfferOrderEntry x2 ON x1.Headid=x2.Headid
		                            GROUP BY x.FId
	                            )x ON x.FId=a.FId
                            )Y
                            WHERE Y.创建人='{cratename}'
                            and Y.审核日期 >= CONVERT(VARCHAR(100),CONVERT(DATETIME,'{value}'),23)
                            order by Y.创建日期 desc  
                            ";
                }
                //单据状态
                else if (typeid == 4)
                {
                    _result =
                        $@"
                            SELECT Y.FId,Y.OA流水号,
                                    Y.研发类别,
                                    Y.产品名称,Y.单据状态,Y.创建日期,
                                    Y.审核日期,Y.创建人,
                                    Y.单据类型,
                                    Y.产品成本含税小计,Y.包装规格,Y.包装成本,Y.人工制造费用,
	                                Y.[30%],Y.[38%],
	                                Y.[40%],Y.[45%],Y.[50%] 
                            FROM (
                            SELECT A.FId,A.OAorderno OA流水号,
                            CASE A.DevGroupid WHEN 1 THEN '地坪漆' WHEN 2 THEN '高温漆' WHEN 3 THEN '水性大巴轨交' WHEN 4 THEN '水性工程机械' WHEN 5 THEN '水性头盔' 
	                            WHEN 6 THEN '水性小工业' WHEN 7 THEN '水性修补' WHEN 8 THEN '油性保险杠' WHEN 9 THEN '油性标识标牌' WHEN 10 THEN '油性大巴轨交' WHEN 11 THEN '油性汽车配件'
	                            WHEN 12 THEN '油性小工业' WHEN 13 THEN '油性修补' WHEN 14 THEN '原子灰'
	                            ELSE '' END 研发类别,
                            b.ProductName 产品名称,CASE A.Fstatus WHEN 0 THEN '已审核' ELSE '反审核' END 单据状态,CONVERT(varchar(100), A.CreateDt, 23)  创建日期,
                            CONVERT(VARCHAR(100),A.ConfirmDt,23) 审核日期,A.CreateName 创建人,
                            CASE a.Typeid WHEN 0 THEN 'BOM成本报价单' WHEN 1 THEN '新产品成本报价单' WHEN 2 THEN '空白报价单' END 单据类型,
                            x.物料成本和 '产品成本含税小计',b.Bao '包装规格',b.BaoQty '包装成本',b.RenQty '人工制造费用',
                            ROUND(b.KGQty/(1-30/100),4) '30%',ROUND(b.KGQty/(1-38/100),4) '38%',
                            b.FourQty '40%',b.FourFiveQty '45%',b.FiveQty '50%' 
                            FROM dbo.T_OfferOrder A
                            INNER JOIN dbo.T_OfferOrderHead b ON a.FId=b.FId
                            INNER JOIN (
		                            SELECT x.FId,SUM(x2.MaterialAmount) 物料成本和
		                            FROM dbo.T_OfferOrder x
		                            INNER JOIN dbo.T_OfferOrderHead x1 ON x.FId=x1.FId
		                            INNER JOIN dbo.T_OfferOrderEntry x2 ON x1.Headid=x2.Headid
		                            GROUP BY x.FId
	                            )x ON x.FId=a.FId
                            )Y
                            WHERE Y.创建人='{cratename}'
                            and Y.单据状态 = CASE '{value}' WHEN 0 THEN '已审核' ELSE '反审核' END
                            order by Y.创建日期 desc  
                            ";
                }
                //研发类别
                else if (typeid==5)
                {
                    _result = $@"
                            SELECT Y.FId,Y.OA流水号,
                                    Y.研发类别,
                                    Y.产品名称,Y.单据状态,Y.创建日期,
                                    Y.审核日期,Y.创建人,
                                    Y.单据类型,
                                    Y.产品成本含税小计,Y.包装规格,Y.包装成本,Y.人工制造费用,
	                                Y.[30%],Y.[38%],
	                                Y.[40%],Y.[45%],Y.[50%] 
                            FROM (
                            SELECT A.FId,A.OAorderno OA流水号,
                            CASE A.DevGroupid WHEN 1 THEN '地坪漆' WHEN 2 THEN '高温漆' WHEN 3 THEN '水性大巴轨交' WHEN 4 THEN '水性工程机械' WHEN 5 THEN '水性头盔' 
	                            WHEN 6 THEN '水性小工业' WHEN 7 THEN '水性修补' WHEN 8 THEN '油性保险杠' WHEN 9 THEN '油性标识标牌' WHEN 10 THEN '油性大巴轨交' WHEN 11 THEN '油性汽车配件'
	                            WHEN 12 THEN '油性小工业' WHEN 13 THEN '油性修补' WHEN 14 THEN '原子灰'
	                            ELSE '' END 研发类别,
                            b.ProductName 产品名称,CASE A.Fstatus WHEN 0 THEN '已审核' ELSE '反审核' END 单据状态,CONVERT(varchar(100), A.CreateDt, 23)  创建日期,
                            CONVERT(VARCHAR(100),A.ConfirmDt,23) 审核日期,A.CreateName 创建人,
                            CASE a.Typeid WHEN 0 THEN 'BOM成本报价单' WHEN 1 THEN '新产品成本报价单' WHEN 2 THEN '空白报价单' END 单据类型,
                            x.物料成本和 '产品成本含税小计',b.Bao '包装规格',b.BaoQty '包装成本',b.RenQty '人工制造费用',
                            ROUND(b.KGQty/(1-30/100),4) '30%',ROUND(b.KGQty/(1-38/100),4) '38%',
                            b.FourQty '40%',b.FourFiveQty '45%',b.FiveQty '50%' 
                            FROM dbo.T_OfferOrder A
                            INNER JOIN dbo.T_OfferOrderHead b ON a.FId=b.FId
                            INNER JOIN (
		                            SELECT x.FId,SUM(x2.MaterialAmount) 物料成本和
		                            FROM dbo.T_OfferOrder x
		                            INNER JOIN dbo.T_OfferOrderHead x1 ON x.FId=x1.FId
		                            INNER JOIN dbo.T_OfferOrderEntry x2 ON x1.Headid=x2.Headid
		                            GROUP BY x.FId
	                            )x ON x.FId=a.FId
                            )Y
                            WHERE Y.创建人='{cratename}'
                            and Y.研发类别 = CASE '{value}' WHEN 1 THEN '地坪漆' WHEN 2 THEN '高温漆' WHEN 3 THEN '水性大巴轨交' WHEN 4 THEN '水性工程机械' WHEN 5 THEN '水性头盔' 
		                                        WHEN 6 THEN '水性小工业' WHEN 7 THEN '水性修补' WHEN 8 THEN '油性保险杠' WHEN 9 THEN '油性标识标牌' WHEN 10 THEN '油性大巴轨交' WHEN 11 THEN '油性汽车配件'
			                                    WHEN 12 THEN '油性小工业' WHEN 13 THEN '油性修补' WHEN 14 THEN '原子灰'
		                                        ELSE '' END
                            order by Y.创建日期 desc  
                        ";
                }
            }

            return _result;
        }


        /// <summary>
        /// 根据FID查询BOM报价单明细
        /// </summary>
        /// <returns></returns>
        public string SearchBomDtl(int fid)
        {
            _result = $@"
                            SELECT a.FId,a.OAorderno,a.Fstatus,a.CreateDt,a.ConfirmDt,a.CreateName,a.Typeid,a.DevGroupid,

	                               b.Headid,b.ProductName,b.Bao,b.ProductMi,b.MaterialQty,b.BaoQty,
	                               b.RenQty,b.KGQty,b.LQty,b.FiveQty,b.FourFiveQty,b.FourQty,
	                               b.Fremark,b.FBomOrder,b.FPrice,b.CustName,

	                               c.Entryid,c.MaterialID,c.MaterialCode,c.MaterialName,c.PeiQty,c.ratioQty,c.MaterialPrice,c.MaterialAmount,
                                   c.Remark,c.LastChangeUser,c.LastChanageDt
	    
                            FROM dbo.T_OfferOrder a
                            INNER JOIN dbo.T_OfferOrderHead b ON a.FId=b.FId
                            INNER JOIN dbo.T_OfferOrderEntry c ON b.Headid=c.Headid

                            WHERE a.FId='{fid}'
                        ";
            return _result;
        }

        /// <summary>
        /// 根据FID查询此单据占用情况
        /// </summary>
        /// <param name="fid"></param>
        /// <returns></returns>
        public string SearchUseInfo(int fid)
        {
            _result = $@"
                            SELECT a.Useid,a.UserName FROM dbo.T_OfferOrder a
                            WHERE a.FId='{fid}'
                        ";
            return _result;
        }

        /// <summary>
        /// 更新单据占用情况
        /// </summary>
        /// <returns></returns>
        public string UpUsedtl(int fid)
        {
            _result = $@"
                           UPDATE dbo.T_OfferOrder SET Useid=0,UserName='{GlobalClasscs.User.StrUsrName}'
                           where fid='{fid}' 
                        ";
            return _result;
        }

        /// <summary>
        /// 清空单据占用情况
        /// </summary>
        /// <param name="fid"></param>
        /// <param name="oaorder"></param>
        /// <returns></returns>
        public string RemoveUsedtl(int fid,string oaorder)
        {
            _result = oaorder == ""
                ? $@"
                           UPDATE dbo.T_OfferOrder SET Useid=1,UserName=''
                           where fid='{fid}' 
                        "
                : $@"
                           UPDATE dbo.T_OfferOrder SET Useid=1,UserName=''
                           where OAOrderno='{oaorder}' 
                        ";
            return _result;
        }

        /// <summary>
        /// 更新单据状态(更新为反审核状态,并将审核日期清空)
        /// </summary>
        /// <returns></returns>
        public string UpOrderStatus(int fid)
        {
            _result = $@"
                            UPDATE dbo.T_OfferOrder SET Fstatus=1,ConfirmDt=NULL WHERE FId='{fid}'
                        ";
            return _result;
        }

        /// <summary>
        /// 批量成本查询及产品成本毛利润表-报表使用
        /// </summary>
        /// <param name="searchid"></param>
        /// <param name="searchvalue"></param>
        /// <returns></returns>
        public string SearchMaterial(int searchid,string searchvalue)
        {
            //按照‘物料名称’进行查询
            if (searchid == 1)
            {
                _result = $@"
                                SELECT a.FMATERIALID,a.FNUMBER 物料编码,d.FNAME 物料名称,
                                       d.FSPECIFICATION '规格型号',
                                       CASE c.FDATAVALUE WHEN '产成品' THEN (
											SELECT x.FCONVERTDENOMINATOR/x.FCONVERTNUMERATOR FROM dbo.T_BD_UNITCONVERTRATE x
											WHERE a.FMATERIALID=x.FMATERIALID AND x.FCURRENTUNITID='10095'
											) ELSE 1 END '密度(KG/L)' ,  --来源“物料单位换算列表”功能
                                       --CASE A.F_YTC_DECIMAL7 WHEN 0 THEN a.F_YTC_DECIMAL1 ELSE E.FNETWEIGHT/A.F_YTC_DECIMAL7 END '密度(KG/L)', /*a.F_YTC_DECIMAL1*/
                                       e.FNETWEIGHT '净重',
                                       a.F_YTC_DECIMAL '罐/箱',x1.FDATAVALUE 分类,y1.FDATAVALUE 品类,
                                       z2.FNAME 销售计价单位,
                                       a.F_YTC_DECIMAL7 U订货计价规格

                                FROM dbo.T_BD_MATERIAL a
                                INNER JOIN dbo.T_BAS_ASSISTANTDATAENTRY b ON a.F_YTC_ASSISTANT5=b.FENTRYID
                                INNER JOIN dbo.T_BAS_ASSISTANTDATAENTRY_L c ON b.FENTRYID=c.FENTRYID
                                INNER JOIN dbo.T_BD_MATERIAL_L d ON a.FMATERIALID=d.FMATERIALID

                                INNER JOIN dbo.T_BD_MATERIALBASE e ON a.FMATERIALID=e.FMATERIALID

                                LEFT JOIN dbo.T_BAS_ASSISTANTDATAENTRY x0 ON a.F_YTC_ASSISTANT=x0.FENTRYID
                                LEFT JOIN dbo.T_BAS_ASSISTANTDATAENTRY_L x1 ON x0.FENTRYID=x1.FENTRYID

                                LEFT JOIN T_BAS_ASSISTANTDATAENTRY y0 ON a.f_ytc_assistant1=y0.FENTRYID
                                LEFT JOIN dbo.T_BAS_ASSISTANTDATAENTRY_L y1 ON y0.FENTRYID=y1.FENTRYID

                                INNER JOIN dbo.T_BD_MATERIALSALE z0 ON a.FMATERIALID=z0.FMATERIALID
								INNER JOIN dbo.T_BD_UNIT z1 ON z0.FSALEPRICEUNITID=z1.FUNITID
								INNER JOIN dbo.T_BD_UNIT_L z2 ON z1.FUNITID=z2.FUNITID

                                WHERE c.FDATAVALUE IN('产成品','原漆半成品','原漆')
                                AND a.FDOCUMENTSTATUS='C'
                                --AND a.FFORBIDSTATUS='A' --物料禁用状态:否
                                AND d.FLOCALEID=2052
                                AND z2.FLOCALEID=2052
                                AND D.FNAME LIKE '%{searchvalue}%'
                                AND EXISTS (
												SELECT NULL FROM T_ENG_BOM A1
												WHERE A1.FMATERIALID=A.FMATERIALID
                                                AND a1.FFORBIDSTATUS='A'   --BOM禁用状态:否
												AND a1.FDOCUMENTSTATUS='C' --BOM审核状态:已审核
											) --必须要在‘成本BOM’内存在
                                order by a.FMATERIALID
                            ";
            }
            //按照‘物料编码’进行查询
            else
            {
                _result = $@"
                                 SELECT a.FMATERIALID,a.FNUMBER 物料编码,d.FNAME 物料名称,
                                        d.FSPECIFICATION '规格型号',
                                        CASE c.FDATAVALUE WHEN '产成品' THEN (
										SELECT x.FCONVERTDENOMINATOR/x.FCONVERTNUMERATOR FROM dbo.T_BD_UNITCONVERTRATE x
										WHERE a.FMATERIALID=x.FMATERIALID AND x.FCURRENTUNITID='10095'
										) ELSE 1 END '密度(KG/L)' ,
										--CASE A.F_YTC_DECIMAL7 WHEN 0 THEN a.F_YTC_DECIMAL1 ELSE E.FNETWEIGHT/A.F_YTC_DECIMAL7 END '密度(KG/L)', /*a.F_YTC_DECIMAL1*/
                                        e.FNETWEIGHT '净重',
                                        a.F_YTC_DECIMAL '罐/箱',x1.FDATAVALUE 分类,y1.FDATAVALUE 品类,
                                        z2.FNAME 销售计价单位,
                                        a.F_YTC_DECIMAL7 U订货计价规格

                                FROM dbo.T_BD_MATERIAL a
                                INNER JOIN dbo.T_BAS_ASSISTANTDATAENTRY b ON a.F_YTC_ASSISTANT5=b.FENTRYID
                                INNER JOIN dbo.T_BAS_ASSISTANTDATAENTRY_L c ON b.FENTRYID=c.FENTRYID
                                INNER JOIN dbo.T_BD_MATERIAL_L d ON a.FMATERIALID=d.FMATERIALID

                                INNER JOIN dbo.T_BD_MATERIALBASE e ON a.FMATERIALID=e.FMATERIALID

                                LEFT JOIN dbo.T_BAS_ASSISTANTDATAENTRY x0 ON a.F_YTC_ASSISTANT=x0.FENTRYID
                                LEFT JOIN dbo.T_BAS_ASSISTANTDATAENTRY_L x1 ON x0.FENTRYID=x1.FENTRYID

                                LEFT JOIN T_BAS_ASSISTANTDATAENTRY y0 ON a.f_ytc_assistant1=y0.FENTRYID
                                LEFT JOIN dbo.T_BAS_ASSISTANTDATAENTRY_L y1 ON y0.FENTRYID=y1.FENTRYID

                                INNER JOIN dbo.T_BD_MATERIALSALE z0 ON a.FMATERIALID=z0.FMATERIALID
								INNER JOIN dbo.T_BD_UNIT z1 ON z0.FSALEPRICEUNITID=z1.FUNITID
								INNER JOIN dbo.T_BD_UNIT_L z2 ON z1.FUNITID=z2.FUNITID

                                WHERE c.FDATAVALUE IN('产成品','原漆半成品','原漆')
                                AND a.FDOCUMENTSTATUS='C'
                                --AND a.FFORBIDSTATUS='A' --物料禁用状态:否
                                AND d.FLOCALEID=2052
								AND z2.FLOCALEID=2052
                                AND a.FNUMBER LIKE '%{searchvalue}%'
                                AND EXISTS (
												SELECT NULL FROM T_ENG_BOM A1
												WHERE A1.FMATERIALID=A.FMATERIALID
                                                AND a1.FFORBIDSTATUS='A'   --BOM禁用状态:否
												AND a1.FDOCUMENTSTATUS='C' --BOM审核状态:已审核
											)  --必须要在‘成本BOM’内存在
                                order by a.FMATERIALID
                            ";
            }
            return _result;
        }

        /// <summary>
        /// 采购入库单相关(报表功能中旧准成本单价使用)
        /// </summary>
        /// <returns></returns>
        public string SearchInstock()
        {
            _result = $@"
                            select t1.子项物料内码,t1.单价
						    from (select ROW_NUMBER()over(partition by t2.fmaterialid order by t2.fmaterialid,t1.fdate desc) r,t1.fdate,t2.FMATERIALID 子项物料内码,t3.FTAXPRICE 单价
								    from T_STK_INSTOCK t1
									    inner join T_STK_INSTOCKENTRY t2 on t1.fid = t2.fid
									    inner join T_STK_INSTOCKENTRY_F t3 on t2.fentryid = t3.fentryid
									    left join t_bd_materialbase t4 on t2.FMATERIALID = t4.FMATERIALID
								    where t4.FERPCLSID<>2) t1
						    where t1.r = 1
                        ";

            return _result;
        }

        /// <summary>
        /// 采购价目表(报表功能中旧准成本单价使用)
        /// </summary>
        /// <returns></returns>
        public string SearchPricelist()
        {
            _result = $@"
                          select t1.子项物料内码 ,t1.单价 
						  from (select row_number()over(partition by t2.fmaterialid order by t2.fmaterialid,t2.fentryid desc) r,t2.FMATERIALID 子项物料内码,t2.FTAXPRICE 单价
								from T_PUR_PRICELIST t1 
									inner join t_PUR_PriceListentry t2 on t1.FID = t2.FID
									left join t_bd_materialbase t3 on t2.FMATERIALID = t3.FMATERIALID
								where t1.FFORBIDSTATUS='A' and t1.FDOCUMENTSTATUS='C' and t3.FERPCLSID<>2) t1
						  where t1.r = 1
                        ";
            return _result;
        }

        /// <summary>
        /// 销售价目表相关(产品成本毛利润报表使用)
        /// </summary>
        /// <returns></returns>
        public string SearchSalesInstock()
        {
            _result = @"
                            SELECT a.FID,B.FMATERIALID,B.FPRICE,D.FNAME
                            FROM dbo.T_SAL_PRICELIST A
                            INNER JOIN dbo.T_SAL_PRICELISTENTRY B ON A.FID=B.FID
                            INNER JOIN T_BD_CURRENCY C ON A.FCURRENCYID=C.FCURRENCYID
                            INNER JOIN dbo.T_BD_CURRENCY_L D ON C.FCURRENCYID=D.FCURRENCYID AND D.FLOCALEID=2052

                            WHERE A.FDOCUMENTSTATUS='C'  --'已审核' 
                            AND a.FFORBIDSTATUS='A'      --末失效
                            AND (a.FNUMBER NOT LIKE '%Z%' and a.FNUMBER NOT LIKE '%z%')  --编号不能包含Z
                            AND NOT EXISTS(
                                             SELECT NULL 
				                             FROM dbo.T_SAL_APPLYCUSTOMER X
				                             INNER JOIN dbo.T_BD_CUSTOMER X1 ON X.FCUSTID=X1.FCUSTID
				                             INNER JOIN dbo.T_BD_CUSTOMER_L x2 ON x1.FCUSTID=x2.FCUSTID
				                             WHERE A.FID=X.FID
				                             AND x2.FLOCALEID=2052
				                             AND (/*X1.FNUMBER LIKE 'INT-%' OR*/ x2.FNAME LIKE '%晶创%') --将晶创国内及晶创海外的客户排除
			                              ) 
                           /* AND NOT EXISTS (
				                              SELECT NULL FROM (
                                                SELECT COUNT(*) NUM FROM dbo.T_SAL_APPLYCUSTOMER Y
					                            WHERE A.FID=Y.FID)Z
				                              WHERE Z.NUM=1
                                           )*/   --当‘适用客户’记录行数只有一行时排除
                            --AND B.FMATERIALID='124439'--'420210'--'830727'--'126340'--'123816'--'126340'
                            ORDER BY a.FID DESC
                        ";
            return _result;
        }

        /// <summary>
        /// 采购入库单(产品成本毛利润报表专用)
        /// </summary>
        /// <returns></returns>
        public string SearchPurchase()
        {
            _result = @"
                            SELECT T1.FMATERIALID,T1.净价 
                            FROM (
                                    SELECT ROW_NUMBER()OVER(PARTITION BY b.FMATERIALID ORDER BY b.FMATERIALID,a.FDATE DESC) r, b.FMATERIALID,c.FTAXNETPRICE 净价
                                    FROM dbo.T_STK_INSTOCK a
                                    INNER JOIN dbo.T_STK_INSTOCKENTRY b ON a.FID=b.FID
                                    INNER JOIN dbo.T_STK_INSTOCKENTRY_F c ON b.FENTRYID=c.FENTRYID
                                    AND a.FDOCUMENTSTATUS='C')T1 
                            WHERE T1.r=1

                            /*SELECT b.FMATERIALID,c.FTAXNETPRICE 净价
                            FROM dbo.T_STK_INSTOCK a
                            INNER JOIN dbo.T_STK_INSTOCKENTRY b ON a.FID=b.FID
                            INNER JOIN dbo.T_STK_INSTOCKENTRY_F c ON b.FENTRYID=c.FENTRYID
                            AND a.FDOCUMENTSTATUS='C'  --已审核
                            ORDER BY a.FDATE DESC*/
                        ";
            return _result;
        }

        /// <summary>
        /// 人工制造费用(产品成本毛利润报表使用)
        /// </summary>
        /// <returns></returns>
        public string SearchRenCost()
        {
            _result = @"
                            SELECT a.classtype 品类,a.sorttype 分类,a.SalaryTotal 人工制造费用
                            FROM dbo.T_BD_RenCost a
                        ";
            return _result;
        }

        /// <summary>
        /// 暂存表相关查询(条件:针对当前用户)
        /// </summary>
        /// <returns></returns>
        public string SearchTempOrder(string createname)
        {
            _result = $@"
                            SELECT a.FId,a.OAorderno OA流水号,
                                   CASE A.DevGroupid WHEN 1 THEN '地坪漆' WHEN 2 THEN '高温漆' WHEN 3 THEN '水性大巴轨交' WHEN 4 THEN '水性工程机械' WHEN 5 THEN '水性头盔' 
		                                     WHEN 6 THEN '水性小工业' WHEN 7 THEN '水性修补' WHEN 8 THEN '油性保险杠' WHEN 9 THEN '油性标识标牌' WHEN 10 THEN '油性大巴轨交' WHEN 11 THEN '油性汽车配件'
			                                 WHEN 12 THEN '油性小工业' WHEN 13 THEN '油性修补' WHEN 14 THEN '原子灰'
		                                ELSE '' END 研发类别,
                                   b.ProductName 产品名称,CONVERT(varchar(100), A.CreateDt, 23)  创建日期,
	                               a.CreateName 创建人,
	                               CASE a.Typeid WHEN 0 THEN 'BOM成本报价单' WHEN 1 THEN '新产品成本报价单' WHEN 2 THEN '空白报价单' END 单据类型,
	                               b.Bao '包装规格',b.BaoQty '包装成本',b.RenQty '人工制造费用',
	                               ROUND(b.KGQty/(1-30/100),4) '30%',ROUND(b.KGQty/(1-38/100),4) '38%',
	                               b.FourQty '40%',b.FourFiveQty '45%',b.FiveQty '50%'
                            FROM dbo.T_TempOrder a
                            INNER JOIN dbo.T_TempOrderHead b ON a.FId=b.FId
                            WHERE A.CreateName='{createname}'
                            ORDER BY a.CreateDt DESC
                        ";
            return _result;
        }

        /// <summary>
        /// 查询‘暂存’功能明细记录
        /// </summary>
        /// <param name="fid"></param>
        /// <returns></returns>
        public string SearchTempOrderDetail(int fid)
        {
            _result = $@"
                            SELECT a.FId,a.OAorderno,-1 Fstatus,a.CreateDt,-1 Confirmdt,a.CreateName,a.Typeid,a.DevGroupid,

	                                b.Headid,b.ProductName,b.Bao,b.ProductMi,b.MaterialQty,b.BaoQty,
	                                b.RenQty,b.KGQty,b.LQty,b.FiveQty,b.FourFiveQty,b.FourQty,
	                                b.Fremark,b.FBomOrder,b.FPrice,b.CustName,

		                            c.Entryid,c.MaterialID,c.MaterialCode,c.MaterialName,c.PeiQty,c.ratioQty,c.MaterialPrice,c.MaterialAmount,
		                            c.Remark,c.LastChangeUser,c.LastChanageDt
                            FROM dbo.T_TempOrder a
                            INNER JOIN dbo.T_TempOrderHead b ON a.FId=b.FId
                            INNER JOIN dbo.T_TempOrderEntry c ON b.Headid=c.Headid
                            WHERE a.FId='{fid}'";
            return _result;
        }

        //////////////////////////////////////////////////权限使用////////////////////////////////////////////////////////

        /// <summary>
        /// 权限查询-用户权限主窗体使用
        /// </summary>
        /// <param name="typeid">0:用户名称 1:创建人 2:创建日期 3:启用状态</param>
        /// <param name="value"></param>
        /// <returns></returns>
        public string SearchAdminDetail(int typeid, string value)
        {
            if (typeid == 0)
            {
                _result = $@"
                                SELECT a.userid,a.UserName 用户,a.CreateName 创建人,a.CreateDt 创建日期,
	                                   CASE a.ApplyId WHEN 0 THEN '已启用' ELSE '未启用' END '启用状态',
	                                   CASE a.CanBackConfirm WHEN 0 THEN '是' ELSE '否' END '可反审核',
	                                   CASE a.Readid WHEN 0 THEN '是' ELSE '否' END '可查阅明细金额',
	                                   CASE a.Addid WHEN 0 THEN '是' ELSE '否' END '可修改物料明细',
                                       CASE a.UserRelid WHEN 0 THEN '是' ELSE '否' END '是否不关联用户'
                                FROM dbo.T_AD_User a
                                where a.UserName like '%{value}%'
                            ";
            }
            else if (typeid == 1)
            {
                _result = $@"
                                SELECT a.userid,a.UserName 用户,a.CreateName 创建人,a.CreateDt 创建日期,
	                                   CASE a.ApplyId WHEN 0 THEN '已启用' ELSE '未启用' END '启用状态',
	                                   CASE a.CanBackConfirm WHEN 0 THEN '是' ELSE '否' END '可反审核',
	                                   CASE a.Readid WHEN 0 THEN '是' ELSE '否' END '可查阅明细金额',
	                                   CASE a.Addid WHEN 0 THEN '是' ELSE '否' END '可修改物料明细',
                                       CASE a.UserRelid WHEN 0 THEN '是' ELSE '否' END '是否不关联用户'
                                FROM dbo.T_AD_User a
                                where a.CreateName like '%{value}%'
                            ";
            }
            else if (typeid == 2)
            {
                _result = $@"
                                SELECT a.userid,a.UserName 用户,a.CreateName 创建人,a.CreateDt 创建日期,
	                                   CASE a.ApplyId WHEN 0 THEN '已启用' ELSE '未启用' END '启用状态',
	                                   CASE a.CanBackConfirm WHEN 0 THEN '是' ELSE '否' END '可反审核',
	                                   CASE a.Readid WHEN 0 THEN '是' ELSE '否' END '可查阅明细金额',
	                                   CASE a.Addid WHEN 0 THEN '是' ELSE '否' END '可修改物料明细',
                                       CASE a.UserRelid WHEN 0 THEN '是' ELSE '否' END 是否不关联用户'
                                FROM dbo.T_AD_User a
                                where CONVERT(VARCHAR(100),a.CreateDt,23)>=CONVERT(VARCHAR(100),CONVERT(DATETIME,'{value}'),23)
                            ";
            }
            else if (typeid == 3)
            {
                _result = $@"
                                SELECT a.userid,a.UserName 用户,a.CreateName 创建人,a.CreateDt 创建日期,
	                                   CASE a.ApplyId WHEN 0 THEN '已启用' ELSE '未启用' END '启用状态',
	                                   CASE a.CanBackConfirm WHEN 0 THEN '是' ELSE '否' END '可反审核',
	                                   CASE a.Readid WHEN 0 THEN '是' ELSE '否' END '可查阅明细金额',
                                       CASE a.UserRelid WHEN 0 THEN '是' ELSE '否' END '是否不关联用户'
                                FROM dbo.T_AD_User a
                                where a.ApplyId='{value}'
                            ";
            }
            return _result;
        }

        /// <summary>
        /// 查询K3用户信息
        /// </summary>
        /// <returns></returns>
        public string SearchK3User(string value)
        {
            //初始化使用
            if (value == "")
            {
                _result = $@"
                                select a.FNAME K3用户姓名,c.FNAME K3用户组别,a.FPHONE K3用户手机
                                FROM T_SEC_USER a
                                left JOIN dbo.T_SEC_USERGROUP b ON a.FPRIMARYGROUP=b.FID
                                left JOIN dbo.T_SEC_USERGROUP_L c ON b.FID=c.FID
                                WHERE a.FFORBIDSTATUS='A' --AND a.FNAME LIKE'%蓝%'
                                AND a.FNAME NOT IN ('Guest','Administrator','attendance')
                                /*AND NOT EXISTS (
					                                SELECT NULL 
					                                FROM BomOffer.dbo.T_AD_User a1
					                                WHERE a.FNAME=a1.UserName
				                                )*/
                                ORDER BY a.FPWDVALIDDATE
                           ";
            }
            //查询使用
            else
            {
                _result = $@"
                                select a.FNAME K3用户姓名,c.FNAME K3用户组别,a.FPHONE K3用户手机
                                FROM T_SEC_USER a
                                left JOIN dbo.T_SEC_USERGROUP b ON a.FPRIMARYGROUP=b.FID
                                left JOIN dbo.T_SEC_USERGROUP_L c ON b.FID=c.FID
                                WHERE a.FFORBIDSTATUS='A' --AND a.FNAME LIKE'%蓝%'
                                AND a.FNAME NOT IN ('Guest','Administrator','attendance')
                                /*AND NOT EXISTS (
					                                SELECT NULL 
					                                FROM BomOffer.dbo.T_AD_User a1
					                                WHERE a.FNAME=a1.UserName
				                                )*/
                                and a.FName like '%{value}%'
                                ORDER BY a.FPWDVALIDDATE
                            ";
            }
            return _result;
        }

        /// <summary>
        /// 获取最新的Useid值
        /// </summary>
        /// <returns></returns>
        public string GetNewUseridValue()
        {
            _result = @"
                            DECLARE
	                            @id INT;
                            BEGIN
	                            INSERT INTO dbo.T_AD_User_KEY( Column1 )
	                            VALUES  (1)

	                            SELECT @id=Id FROM dbo.T_AD_User_KEY

	                            DELETE FROM dbo.T_AD_User_KEY

	                            SELECT @id
                            END
                       ";
            return _result;
        }

        /// <summary>
        /// 获取GroupID主键值
        /// </summary>
        /// <returns></returns>
        public string GetGroupidKey()
        {
            _result = @"
                            DECLARE
	                            @id INT;
                            BEGIN
	                            INSERT INTO dbo.T_BD_UserGroup_KEY( Column1 )
	                            VALUES  (1)

	                            SELECT @id=Id FROM dbo.T_BD_UserGroup_KEY

	                            DELETE FROM dbo.T_BD_UserGroup_KEY

	                            SELECT @id
                            END
                       ";
            return _result;
        }

        /// <summary>
        /// 获取Dtlid主键值
        /// </summary>
        /// <returns></returns>
        public string GetDtlidKey()
        {
            _result = @"
                            DECLARE
	                            @id INT;
                            BEGIN
	                            INSERT INTO dbo.T_BD_UserGroupDtl_KEY( Column1 )
	                            VALUES  (1)

	                            SELECT @id=Id FROM dbo.T_BD_UserGroupDtl_KEY

	                            DELETE FROM dbo.T_BD_UserGroupDtl_KEY

	                            SELECT @id
                            END
                       ";
            return _result;
        }

        /// <summary>
        /// 获取用户权限表记录
        /// </summary>
        /// <returns></returns>
        public string SearchUseDetail()
        {
            _result = $@"
                            SELECT a.Userid,a.UserName,a.UserPwd,a.ApplyId,a.CanBackConfirm,a.Readid,a.Addid
                            FROM dbo.T_AD_User a
                        ";
            return _result;
        }

        /// <summary>
        /// 根据Userid查询该用户是否占用
        /// </summary>
        /// <returns></returns>
        public string SearchAdminUserInfo(int userid)
        {
            _result = $@"
                            SELECT a.Useid 
                            FROM dbo.T_AD_User a
                            where a.Userid='{userid}'
                        ";
            return _result;
        }

        /// <summary>
        /// 更新用户权限占用
        /// </summary>
        /// <param name="userid"></param>
        /// <returns></returns>
        public string UpdateAccountUseid(int userid)
        {
            _result = $@"
                           UPDATE dbo.T_AD_User SET Useid=0 WHERE Userid='{userid}'
                        ";
            return _result;
        }

        /// <summary>
        /// 清空用户权限占用
        /// </summary>
        /// <param name="userid"></param>
        /// <returns></returns>
        public string RemoveAccountUseid(int userid)
        {
            _result = $@"
                          UPDATE dbo.T_AD_User SET Useid=1 WHERE Userid='{userid}' 
                        ";
            return _result;
        }

        public string UpdateUserNewpwd(int userid, string newpwd)
        {
            _result = $@"
                            UPDATE dbo.T_AD_User SET UserPwd='{newpwd}' WHERE Userid='{userid}'
                        ";
            return _result;
        }

        /// <summary>
        /// 查询基础表‘用户组别’表头信息
        /// </summary>
        /// <returns></returns>
        public string SearchUserGroup()
        {
            _result = @"
                            SELECT * 
                            FROM dbo.T_BD_UserGroup A
                        ";
            return _result;
        }

        /// <summary>
        /// 查询基础表‘用户组别’表体信息(包含“不启用”字段)--用户关联功能使用
        /// </summary>
        /// <returns></returns>
        public string SearchUserGroupDetail()
        {
            _result = @"
                            SELECT a.Groupid,a.Dtlid,a.UserName 员工名称,A.K3UserGroup K3用户组别,A.K3UserPhone K3用户手机,a.CreateName 创建人,
                                   a.CreateDt 创建日期,'' 不启用
                            FROM dbo.T_BD_UserGroupDtl a
                       ";
            return _result;
        }

        /// <summary>
        /// 查询基础表‘用户组别’表体信息(不包含“不启用”字段)--基础资料-用户组别设置使用
        /// </summary>
        /// <returns></returns>
        public string SearchBdGroupDt()
        {
            _result = @"
                           SELECT a.Groupid,a.Dtlid,a.UserName 员工名称,A.K3UserGroup K3用户组别,A.K3UserPhone K3用户手机,a.CreateName 创建人,
                                  a.CreateDt 创建日期 
                           FROM  T_BD_UserGroupDtl a
                        ";
            return _result;
        }

        /// <summary>
        ///  查询‘用户关联’表头信息
        /// </summary>
        /// <returns></returns>
        public string SearchRelUser()
        {
            _result = @"
                            SELECT * FROM dbo.T_AD_RelUser
                       ";
            return _result;
        }

        /// <summary>
        /// 查询‘用户关联’表体信息
        /// </summary>
        /// <returns></returns>
        public string SearchRelUserDtl()
        {
            _result = @"
                          SELECT * FROM dbo.T_AD_RelUserDtl
                       ";
            return _result;
        }

        /// <summary>
        /// 删除T_AD_RelUser(注:在保存用户关联时使用)
        /// </summary>
        /// <returns></returns>
        public string DelRelUser(int userid)
        {
            _result = $@"
                           DELETE FROM dbo.T_AD_RelUser WHERE Userid='{userid}'
                       ";

            return _result;
        }


        /// <summary>
        /// 删除T_AD_RelUserDtl(注:在保存用户关联时使用)
        /// </summary>
        /// <param name="userid"></param>
        /// <returns></returns>
        public string DelRelUserDtl(int userid)
        {
            _result = $@"
                          DELETE FROM dbo.T_AD_RelUserDtl WHERE Userid='{userid}'
                       ";
            return _result;
        }

        /// <summary>
        /// 查询已关联的‘研发类别’信息
        /// </summary>
        /// <returns></returns>
        public string SearchRelUserDevGroup()
        {
            _result = @"
                          SELECT * FROM dbo.T_AD_RelUserDevGroup
                       ";
            return _result;
        }

        /// <summary>
        /// 删除T_AD_RelUserDevGroup(注:在保存用户关联时使用)
        /// </summary>
        /// <param name="userid"></param>
        /// <returns></returns>
        public string DelRelUserDevGroup(int userid)
        {
            _result = $@"
                          DELETE T_AD_RelUserDevGroup WHERE Userid='{userid}'
                       ";
            return _result;
        }

        /// <summary>
        /// 初始化基本表-‘研发类别’DT
        /// </summary>
        /// <returns></returns>
        public string SearchDevGroup()
        {
            _result = @"
                           SELECT DevGroupid Id,DevGroupName Name FROM dbo.T_BD_DevGroup
                       ";
            #region Hide
            /* _result = @"
                 SELECT x.* FROM 
                 (
                     SELECT 0 Id,'' Name

                     UNION ALL

                     SELECT b.FNUMBER Id,C.FDATAVALUE Name 
                     FROM dbo.T_BAS_ASSISTANTDATA a
                     INNER JOIN dbo.T_BAS_ASSISTANTDATAENTRY B ON A.FID=B.FID
                     INNER JOIN dbo.T_BAS_ASSISTANTDATAENTRY_L C ON B.FENTRYID=C.FENTRYID AND C.FLOCALEID=2052
                     WHERE a.FID='5dd1e57223d58f'
                 )x
            ";*/
            #endregion
            return _result;
        }

        /// <summary>
        /// 单据删除
        /// </summary>
        /// <returns></returns>
        public string Delorder(string fidlist)
        {
            _result = $@"
                            DELETE FROM dbo.T_OfferOrderEntry
                            WHERE EXISTS (
				                            SELECT * FROM dbo.T_OfferOrderHead a
				                            INNER JOIN dbo.T_OfferOrder b ON a.FId=b.FId
				                            WHERE a.Headid=dbo.T_OfferOrderEntry.Headid
				                            AND b.FId IN({fidlist})
			                            )

                            DELETE FROM dbo.T_OfferOrderHead 
                            WHERE EXISTS (
				                            SELECT * FROM dbo.T_OfferOrder a
				                            WHERE dbo.T_OfferOrderHead.FId=a.FId
				                            AND a.FId IN({fidlist})
			                            )

                            DELETE FROM dbo.T_OfferOrder WHERE fid IN({fidlist})
                       ";
            return _result;
        }

        /// <summary>
        /// 暂存单据删除
        /// </summary>
        /// <returns></returns>
        public string Deltemporder(string fidlist)
        {
            _result = $@"
                            DELETE FROM dbo.T_TempOrderEntry
                            WHERE EXISTS (
			                                    SELECT * FROM dbo.T_TempOrderHead A
				                                INNER JOIN dbo.T_TempOrder B ON A.FId=B.FId
				                                WHERE A.Headid=dbo.T_TempOrderEntry.Headid
				                                AND B.FId IN({fidlist})
			                                )

                            DELETE FROM dbo.T_TempOrderHead
                            WHERE EXISTS (
                                            SELECT * FROM dbo.T_TempOrder A
				                            WHERE dbo.T_TempOrderHead.FId=A.FId
				                            AND A.FId IN({fidlist})
			                             )

                            DELETE FROM dbo.T_TempOrder WHERE FId IN({fidlist})
                       ";
            return _result;
        }
    }
}
