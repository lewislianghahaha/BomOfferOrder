using System.Drawing.Printing;

namespace BomOfferOrder.DB
{
    public class SqlList
    {
        //根据SQLID返回对应的SQL语句  
        private string _result;

        /// <summary>
        /// 查询窗体使用
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

                                WHERE c.FDATAVALUE IN('产成品','原漆半成品')
                                AND a.FDOCUMENTSTATUS='C'
                                AND a.FFORBIDSTATUS='A' --物料禁用状态:否
                                AND d.FLOCALEID=2052
                                AND g.FLOCALEID=2052
                                AND D.FNAME LIKE '%{searchvalue}%'
                                AND EXISTS (
												SELECT NULL FROM T_ENG_BOM A1
												WHERE A1.FMATERIALID=A.FMATERIALID
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

                                WHERE c.FDATAVALUE IN('产成品','原漆半成品')
                                AND a.FDOCUMENTSTATUS='C'
                                AND a.FFORBIDSTATUS='A' --物料禁用状态:否
                                AND d.FLOCALEID=2052
                                AND g.FLOCALEID=2052
                                AND a.FNUMBER LIKE '%{searchvalue}%'
                                AND EXISTS (
												SELECT NULL FROM T_ENG_BOM A1
												WHERE A1.FMATERIALID=A.FMATERIALID
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
                            SELECT A.FMATERIALID 表头物料ID,A.FNUMBER 'BOM编号',--a.FMODIFYDATE '修改日期',CONVERT(varchar(100), a.FMODIFYDATE, 23) '修改日期',
	                               b.FMATERIALID 表体物料ID,c.FNUMBER 物料编码,d.FNAME 物料名称,
                                   CASE E.FERPCLSID WHEN 1 THEN '外购' WHEN 2 THEN '自制' ELSE '其它' END 物料属性,
                                   cast(b.FNUMERATOR/b.FDENOMINATOR*(1+b.FSCRAPRATE/100) as nvarchar(250)) 用量,
                                   b.FNUMERATOR 分子,b.FDENOMINATOR 分母,b.FSCRAPRATE 变动损耗率,c.F_YTC_DECIMAL8 最新采购价格,
                                   f.F_YTC_DECIMAL8 表头采购单价

                            FROM T_ENG_BOM A
                            INNER JOIN dbo.T_ENG_BOMCHILD b ON a.FID=b.FID

                            INNER JOIN dbo.T_BD_MATERIAL C ON B.FMATERIALID=C.FMATERIALID
                            INNER JOIN dbo.T_BD_MATERIAL_L D ON C.FMATERIALID=D.FMATERIALID
                            INNER JOIN dbo.T_BD_MATERIALBASE E ON D.FMATERIALID=E.FMATERIALID

                            INNER JOIN dbo.T_BD_MATERIAL f ON a.FMATERIALID=f.FMATERIALID

                            WHERE A.FFORBIDSTATUS='A' --BOM禁用状态:否
                            AND A.FDOCUMENTSTATUS='C' --BOM审核状态:已审核
                            AND C.FDOCUMENTSTATUS='C' --物料审核状态:已审核
                            AND C.FFORBIDSTATUS='A'   --物料禁用状态:否
                            AND D.FLOCALEID='2052'
                            AND CONVERT(varchar(100), a.FMODIFYDATE, 20)= (
												                            SELECT CONVERT(varchar(100), MAX(A1.FMODIFYDATE), 20)
												                            FROM T_ENG_BOM A1
												                            WHERE A1.FMATERIALID=A.FMATERIALID
											                               )  --获取最大的‘修改日期’记录
                            --AND A.FMATERIALID='136357'
                            --AND A.FNUMBER='QQ-G5-0001_V1.7'
                            ORDER BY a.FMATERIALID,e.FERPCLSID--,a.FMODIFYDATE DESC
                        ";

            return _result;
        }

        /// <summary>
        /// 获取原材料物料记录(Bom明细调用新物料时使用)
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
                            SELECT a.FMATERIALID,a.FNUMBER 物料编码,d.FNAME 物料名称,c.FDATAVALUE 物料分组,
                                   d.FSPECIFICATION '规格型号',g.FNAME '基本单位',a.F_YTC_DECIMAL8 最新采购价格

                            FROM dbo.T_BD_MATERIAL a
                            INNER JOIN dbo.T_BAS_ASSISTANTDATAENTRY b ON a.F_YTC_ASSISTANT5=b.FENTRYID
                            INNER JOIN dbo.T_BAS_ASSISTANTDATAENTRY_L c ON b.FENTRYID=c.FENTRYID
                            INNER JOIN dbo.T_BD_MATERIAL_L d ON a.FMATERIALID=d.FMATERIALID

                            INNER JOIN dbo.T_BD_MATERIALBASE e ON a.FMATERIALID=e.FMATERIALID
                            INNER JOIN dbo.T_BD_UNIT f ON e.FBASEUNITID=f.FUNITID
                            INNER JOIN dbo.T_BD_UNIT_L g ON f.FUNITID=g.FUNITID

                            WHERE c.FDATAVALUE ='原材料'
                            AND a.FDOCUMENTSTATUS='C'
                            AND a.FFORBIDSTATUS='A' --物料禁用状态:否
                            AND d.FLOCALEID=2052
                            AND g.FLOCALEID=2052
                        ";
            }
            else
            {
                //按照‘物料名称’进行查询
                if (searchid == 1)
                {
                    _result = $@"
                            SELECT a.FMATERIALID,a.FNUMBER 物料编码,d.FNAME 物料名称,c.FDATAVALUE 物料分组,
                                   d.FSPECIFICATION '规格型号',g.FNAME '基本单位',a.F_YTC_DECIMAL8 最新采购价格

                            FROM dbo.T_BD_MATERIAL a
                            INNER JOIN dbo.T_BAS_ASSISTANTDATAENTRY b ON a.F_YTC_ASSISTANT5=b.FENTRYID
                            INNER JOIN dbo.T_BAS_ASSISTANTDATAENTRY_L c ON b.FENTRYID=c.FENTRYID
                            INNER JOIN dbo.T_BD_MATERIAL_L d ON a.FMATERIALID=d.FMATERIALID

                            INNER JOIN dbo.T_BD_MATERIALBASE e ON a.FMATERIALID=e.FMATERIALID
                            INNER JOIN dbo.T_BD_UNIT f ON e.FBASEUNITID=f.FUNITID
                            INNER JOIN dbo.T_BD_UNIT_L g ON f.FUNITID=g.FUNITID

                            WHERE c.FDATAVALUE ='原材料'
                            AND D.FNAME LIKE '%{searchvalue}%'
                            AND a.FDOCUMENTSTATUS='C'
                            AND a.FFORBIDSTATUS='A' --物料禁用状态:否
                            AND d.FLOCALEID=2052
                            AND g.FLOCALEID=2052
                        ";
                }
                //按照'物料编码'进行查询
                else
                {
                    _result = $@"
                            SELECT a.FMATERIALID,a.FNUMBER 物料编码,d.FNAME 物料名称,c.FDATAVALUE 物料分组,
                                   d.FSPECIFICATION '规格型号',g.FNAME '基本单位',a.F_YTC_DECIMAL8 最新采购价格

                            FROM dbo.T_BD_MATERIAL a
                            INNER JOIN dbo.T_BAS_ASSISTANTDATAENTRY b ON a.F_YTC_ASSISTANT5=b.FENTRYID
                            INNER JOIN dbo.T_BAS_ASSISTANTDATAENTRY_L c ON b.FENTRYID=c.FENTRYID
                            INNER JOIN dbo.T_BD_MATERIAL_L d ON a.FMATERIALID=d.FMATERIALID

                            INNER JOIN dbo.T_BD_MATERIALBASE e ON a.FMATERIALID=e.FMATERIALID
                            INNER JOIN dbo.T_BD_UNIT f ON e.FBASEUNITID=f.FUNITID
                            INNER JOIN dbo.T_BD_UNIT_L g ON f.FUNITID=g.FUNITID

                            WHERE c.FDATAVALUE ='原材料'
                            AND a.FNUMBER LIKE '%{searchvalue}%'
                            AND a.FDOCUMENTSTATUS='C'
                            AND a.FFORBIDSTATUS='A' --物料禁用状态:否
                            AND d.FLOCALEID=2052
                            AND g.FLOCALEID=2052
                        ";
                }
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
                                                            CreateName=@CreateName,Useid=@Useid,UserName=@UserName
                                WHERE FId=@FId";
                    break;
                case "T_OfferOrderHead":
                    _result = @"UPDATE dbo.T_OfferOrderHead SET ProductName=@ProductName,Bao=@Bao,ProductMi=@ProductMi,MaterialQty=@MaterialQty,BaoQty=@BaoQty,RenQty=@RenQty,KGQty=@KGQty,
                                                                LQty=@LQty,FiveQty=@FiveQty,FourFiveQty=@FourFiveQty,FourQty=@FourQty,Fremark=@Fremark,FBomOrder=@FBomOrder,FPrice=@FPrice
                                WHERE Headid=@Headid
                                ";
                    break;
                case "":
                    _result = @"UPDATE dbo.T_OfferOrderEntry SET MaterialID=@MaterialID,MaterialCode=@MaterialCode,MaterialName=@MaterialName,
                                                                 PeiQty=@PeiQty,MaterialPrice=@MaterialPrice,MaterialAmount=@MaterialAmount
                                WHERE Entryid=@Entryid
                               ";
                    break;
            }
            return _result;
        }

        /// <summary>
        /// 删除指定行记录(主要针对T_OfferOrderEntry表)
        /// </summary>
        /// <returns></returns>
        public string DelEntry(int entryid)
        {
            _result = $@"
                            DELETE FROM dbo.T_OfferOrderEntry WHERE Entryid='{entryid}'
                       ";
            return _result;
        }


        /////////////////////////////////////查询端使用//////////////////////////////////////////////////////

        /// <summary>
        /// Main查询及查询端使用
        /// </summary>
        /// <returns></returns>
        public string SearchBomList(int typeid,string value)
        {
            //OA流水号
            if (typeid == 0)
            {
                _result = $@"
                                SELECT A.FId,A.OAorderno OA流水号,CASE A.Fstatus WHEN 0 THEN '已审核' ELSE '反审核' END 单据状态,CONVERT(varchar(100), A.CreateDt, 23)  创建日期,
                                       CONVERT(VARCHAR(100),A.ConfirmDt,23) 审核日期,A.CreateName 创建人
                                FROM dbo.T_OfferOrder A
                                WHERE a.OAorderno LIKE '%{value}%'
                            ";
            }
            //产品名称
            else if (typeid == 1)
            {
                _result = $@"
                                SELECT A.FId,A.OAorderno OA流水号,CASE A.Fstatus WHEN 0 THEN '已审核' ELSE '反审核' END 单据状态,CONVERT(varchar(100), A.CreateDt, 23)  创建日期,
                                       CONVERT(VARCHAR(100),A.ConfirmDt,23) 审核日期,A.CreateName 创建人
                                FROM dbo.T_OfferOrder A
                                WHERE EXISTS (
				                                SELECT NULL
				                                FROM dbo.T_OfferOrderHead b 
				                                WHERE a.FId=b.FId
				                                AND b.ProductName LIKE '%{value}%'
		                                        )
                            ";
            }
            //创建日期
            else if (typeid == 2)
            {
                _result = $@"
                                SELECT A.FId,A.OAorderno OA流水号,CASE A.Fstatus WHEN 0 THEN '已审核' ELSE '反审核' END 单据状态,CONVERT(varchar(100), A.CreateDt, 23)  创建日期,
                                       CONVERT(VARCHAR(100),A.ConfirmDt,23) 审核日期,A.CreateName 创建人
                                FROM dbo.T_OfferOrder A
                                WHERE CONVERT(VARCHAR(100),a.CreateDt,23)>=CONVERT(VARCHAR(100),CONVERT(DATETIME,'{value}'),23)
                            ";
            }
            //审核日期
            else if (typeid == 3)
            {
                _result = $@"
                                SELECT A.FId,A.OAorderno OA流水号,CASE A.Fstatus WHEN 0 THEN '已审核' ELSE '反审核' END 单据状态,CONVERT(varchar(100), A.CreateDt, 23)  创建日期,
                                       CONVERT(VARCHAR(100),A.ConfirmDt,23) 审核日期,A.CreateName 创建人
                                FROM dbo.T_OfferOrder A
                                WHERE CONVERT(VARCHAR(100),a.ConfirmDt,23)>=CONVERT(VARCHAR(100),CONVERT(DATETIME,'{value}'),23)
                            ";
            }
            //单据状态
            else if(typeid == 4)
            {
                _result = $@"
                                SELECT A.FId,A.OAorderno OA流水号,CASE A.Fstatus WHEN 0 THEN '已审核' ELSE '反审核' END 单据状态,CONVERT(varchar(100), A.CreateDt, 23)  创建日期,
                                       CONVERT(VARCHAR(100),A.ConfirmDt,23) 审核日期,A.CreateName 创建人
                                FROM dbo.T_OfferOrder A
                                WHERE a.Fstatus='{value}'
                            ";
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
                            SELECT a.FId,a.OAorderno,a.Fstatus,a.ConfirmDt,a.CreateDt,a.CreateName,

	                               b.Headid,b.ProductName,b.Bao,b.ProductMi,b.MaterialQty,b.BaoQty,
	                               b.RenQty,b.KGQty,b.LQty,b.FiveQty,b.FourFiveQty,b.FourQty,
	                               b.Fremark,b.FBomOrder,b.FPrice,

	                               c.Entryid,c.MaterialID,c.MaterialCode,c.MaterialName,c.PeiQty,c.MaterialPrice,c.MaterialAmount
	    
                            FROM dbo.T_OfferOrder a
                            INNER JOIN dbo.T_OfferOrderHead b ON a.FId=b.FId
                            INNER JOIN dbo.T_OfferOrderEntry c ON b.Headid=c.Headid

                            WHERE a.FId='{fid}'
                        ";
            return _result;
        }

    }
}
