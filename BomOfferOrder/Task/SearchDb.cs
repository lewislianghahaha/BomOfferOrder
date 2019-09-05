﻿using System;
using System.Data;
using System.Data.SqlClient;
using BomOfferOrder.DB;

namespace BomOfferOrder.Task
{
    public class SearchDb
    {
        SqlList sqlList=new SqlList();

        /// <summary>
        /// 获取连接
        /// </summary>
        /// <returns></returns>
        public SqlConnection GetConn()
        {
            var conn = new Conn();
            var sqlcon = new SqlConnection(conn.GetConnectionString());
            return sqlcon;
        }

        /// <summary>
        /// 查询功能使用
        /// </summary>
        /// <returns></returns>
        public DataTable SearchValue(int searchid, string searchvalue)
        {
            var resultdt = new DataTable();

            try
            {
                var sqlscript = sqlList.Get_Search(searchid, searchvalue);
                var sqlDataAdapter = new SqlDataAdapter(sqlscript, GetConn());
                sqlDataAdapter.Fill(resultdt);
            }
            catch (Exception)
            {
                resultdt.Rows.Clear();
                resultdt.Columns.Clear();
            }
            return resultdt;
        }

        /// <summary>
        /// 查询BOM明细记录信息(生成时使用)
        /// </summary>
        /// <param name="valuelist"></param>
        /// <returns></returns>
        public DataTable SearchMaterialDtl(string valuelist)
        {
            var resultdt = new DataTable();
            try
            {
                var sqlscript = sqlList.Get_Materialdtl(valuelist);
                var sqlDataAdapter = new SqlDataAdapter(sqlscript, GetConn());
                sqlDataAdapter.Fill(resultdt);
            }
            catch (Exception)
            {
                resultdt.Rows.Clear();
                resultdt.Columns.Clear();
            }
            return resultdt;
        }

    }
}
