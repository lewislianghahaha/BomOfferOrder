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
                                --AND a.FNUMBER='010-0025(P-2)-4L-00-43-CK'
                            ";
            }
            return _result;
        }



    }
}
