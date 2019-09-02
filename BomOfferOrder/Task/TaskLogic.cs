﻿using System.Data;

namespace BomOfferOrder.Task
{
    public class TaskLogic
    {
        SearchDb searchDb=new SearchDb();
        GenerateDb generateDb=new GenerateDb();

        #region 变量定义
            private int _taskid;             //记录中转ID
            private int _searchid;           //记录查询列表ID值(查询时使用)
            private string _searchvalue;     //查询值(查询时使用)
            private DataTable _dt;           //保存需要进行生成明细记录的DT

            private DataTable _resultTable;  //返回DT类型
            private bool _resultMark;        //返回是否成功标记
        #endregion

        #region Set
            /// <summary>
            /// 中转ID
            /// </summary>
            public int TaskId { set { _taskid = value; } }

            /// <summary>
            ///查询值(查询时使用)
            /// </summary>
            public string SearchValue { set { _searchvalue = value; } }

            /// <summary>
            /// 记录查询列表ID值(查询时使用)
            /// </summary>
            public int SearchId { set { _searchid = value; } }

            /// <summary>
            /// 保存需要进行生成明细记录的DT
            /// </summary>
            public DataTable Data { set { _dt = value; } }
        #endregion

        #region Get
            /// <summary>
            ///返回DataTable至主窗体
            /// </summary>
            public DataTable ResultTable => _resultTable;

            /// <summary>
            /// 返回结果标记
            /// </summary>
            public bool ResultMark => _resultMark;
        #endregion

        public void StartTask()
        {
            switch (_taskid)
            {
                //查询
                case 0:
                    Searchdt(_searchid,_searchvalue);
                    break;
                //运算
                case 1:
                    Generatedt(_dt);
                    break;
            }
        }

        /// <summary>
        /// 查询窗体使用
        /// </summary>
        /// <param name="searchid"></param>
        /// <param name="searchvalue"></param>
        private void Searchdt(int searchid, string searchvalue)
        {
            _resultTable = searchDb.SearchValue(searchid, searchvalue);
        }

        /// <summary>
        /// 运算
        /// </summary>
        /// <param name="sourcedt"></param>
        private void Generatedt(DataTable sourcedt)
        {
            _resultTable = generateDb.Generatedt(sourcedt);
            _resultMark = _resultTable.Rows.Count > 0;
        }
    }
}
