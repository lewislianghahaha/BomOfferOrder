using System.Data;

namespace BomOfferOrder.Task
{
    public class TaskLogic
    {
        SearchDb searchDb=new SearchDb();
        GenerateDb generateDb=new GenerateDb();

        #region 变量定义
            private string _taskid;             //记录中转ID
            private int _searchid;              //记录查询列表ID值(查询时使用)
            private string _searchvalue;        //查询值(查询时使用)
            private DataTable _dt;              //保存需要进行生成明细记录的DT
            private string _valuelist;          //保存Fmaterialid列表(如:'426464','738199')
            private DataTable _bomdt;           //保存BOM明细DT(生成时使用)

            private DataTable _resultTable;     //返回DT类型
            private DataTable _resultbomdt;     //返回BOM DT
            private bool _resultMark;           //返回是否成功标记
        #endregion

        #region Set
            /// <summary>
            /// 中转ID
            /// </summary>
            public string TaskId { set { _taskid = value; } }

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

            /// <summary>
            /// 保存Fmaterialid列表(如:'426464','738199')
            /// </summary>
            public string Valuelist { set { _valuelist = value; } }

            /// <summary>
            /// 保存BOM明细DT(生成时使用)
            /// </summary>
            public DataTable Bomdt { set { _bomdt = value; } }

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

            /// <summary>
            ///返回DataTable至主窗体
            /// </summary>
            public DataTable Resultbomdt => _resultbomdt;
        #endregion

        public void StartTask()
        {
            switch (_taskid)
            {
                //查询:查询窗体使用
                case "0":
                    Searchdt(_searchid,_searchvalue);
                    break;
                //查询:初始化获取BOM明细记录时使用
                case "0.1":
                    Searchbomdt();
                    break;
                //运算
                case "1":
                    Generatedt(_valuelist,_dt,_bomdt);
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
        /// 初始化获取BOM明细记录
        /// </summary>
        private void Searchbomdt()
        {
            _resultbomdt=searchDb.SearchMaterialDtl();
        }

        /// <summary>
        /// 运算
        /// </summary>
        /// <param name="valuelist">FMATERIALID列表ID</param>
        /// <param name="sourcedt"></param>
        /// <param name="bomdt">BOM明细记录DT</param>
        private void Generatedt(string valuelist,DataTable sourcedt,DataTable bomdt)
        {
            _resultTable = generateDb.Generatedt(valuelist,sourcedt,bomdt);
            _resultMark = _resultTable.Rows.Count > 0;
        }
    }
}
