using System.Data;

namespace BomOfferOrder.Task
{
    public class TaskLogic
    {
        SearchDb searchDb=new SearchDb();
        GenerateDb generateDb=new GenerateDb();

        #region 变量定义
        private int _taskid;             //记录中转ID

        private DataTable _resultTable;  //返回DT类型
        private bool _resultMark;        //返回是否成功标记
        #endregion

        #region Set
        /// <summary>
        /// 中转ID
        /// </summary>
        public int TaskId { set { _taskid = value; } }

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

                    break;
                //运算
                case 1:

                    break;
            }
        }



    }
}
