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
                                AND a.FFORBIDSTATUS='A'
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
                                AND a.FFORBIDSTATUS='A'
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
        /// 获取BOM明细记录信息(生成时使用)
        /// </summary>
        /// <param name="valuelist">保存Fmaterialid列表(如:'426464','738199')</param>
        /// <returns></returns>
        public string Get_Materialdtl(string valuelist)
        {
            _result = $@"
                                SELECT x.FMATERIALID,x.BOM编号,x.修改日期,x.物料编码id,x.物料编码,x.物料名称,x.markid
                                FROM (
			                                --不计算原漆的SQL
			                                SELECT a.FMATERIALID,A.FNUMBER 'BOM编号' ,CONVERT(varchar(100), a.FMODIFYDATE, 23) '修改日期',
				                                   c.FMATERIALID 物料编码id,c.FNUMBER 物料编码,d.FNAME 物料名称,0 markid
	 
			                                FROM T_ENG_BOM A
			                                INNER JOIN dbo.T_ENG_BOMCHILD B ON A.FID=B.FID
			                                INNER JOIN dbo.T_BD_MATERIAL c ON b.FMATERIALID=c.FMATERIALID
			                                INNER JOIN dbo.T_BD_MATERIAL_L d ON c.FMATERIALID=d.FMATERIALID

			                                WHERE A.FMATERIALID IN({valuelist})
			                                AND a.FFORBIDSTATUS='A'   --禁止状态:否
			                                AND a.FDOCUMENTSTATUS='C' --审核状态:已审核
			                                AND c.FDOCUMENTSTATUS='C'
			                                AND d.FLOCALEID='2052'
			                                AND d.FNAME NOT LIKE '%原漆%'

			                                UNION ALL
    
			                                --计算原漆的SQL
			                                SELECT x.FMATERIALID,x.BOM编号,CONVERT(varchar(100), a.FMODIFYDATE, 23) '修改日期',
				                                    c.FMATERIALID 物料编码id,C.FNUMBER 物料编码,d.FNAME 物料名称,1 markid
			                                FROM (
					                                SELECT a.FMATERIALID,A.FNUMBER 'BOM编号' ,b.FMATERIALID 明细物料ID
	 
					                                FROM T_ENG_BOM A
					                                INNER JOIN dbo.T_ENG_BOMCHILD B ON A.FID=B.FID
					                                INNER JOIN dbo.T_BD_MATERIAL c ON b.FMATERIALID=c.FMATERIALID
					                                INNER JOIN dbo.T_BD_MATERIAL_L d ON c.FMATERIALID=d.FMATERIALID

					                                WHERE A.FMATERIALID IN({valuelist})
					                                AND a.FFORBIDSTATUS='A'   --禁止状态:否
					                                AND a.FDOCUMENTSTATUS='C' --审核状态:已审核
					                                AND c.FDOCUMENTSTATUS='C'
					                                AND d.FLOCALEID='2052'
					                                AND d.FNAME LIKE '%原漆%')x
			                                INNER JOIN dbo.T_ENG_BOM a ON x.明细物料ID=a.FMATERIALID
			                                INNER JOIN dbo.T_ENG_BOMCHILD b ON a.FID=b.FID
			                                INNER JOIN dbo.T_BD_MATERIAL C ON B.FMATERIALID=C.FMATERIALID
			                                INNER JOIN dbo.T_BD_MATERIAL_L D ON C.FMATERIALID=D.FMATERIALID

			                                WHERE a.FFORBIDSTATUS='A' --禁止状态:否
			                                AND A.FDOCUMENTSTATUS='C'
			                                AND c.FDOCUMENTSTATUS='C'
			                                AND D.FLOCALEID='2052'
	                                    )x
                                ORDER BY x.FMATERIALID,x.markid,x.修改日期 DESC
                        ";

            return _result;
        }

    }
}
