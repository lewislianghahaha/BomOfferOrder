using System.Data;

namespace BomOfferOrder.Task
{
    public class TaskLogic
    {
        SearchDt searchDt=new SearchDt();
        GenerateDt generateDt=new GenerateDt();
        ImportDt importDt=new ImportDt();

        #region 变量定义
        private string _taskid;             //记录中转ID
        private int _searchid;              //记录查询列表ID值(查询时使用)
        private string _searchvalue;        //查询值(查询时使用)
        private DataTable _dt;              //保存需要进行生成明细记录的DT
        private string _valuelist;          //保存Fmaterialid列表(如:'426464','738199')
        private DataTable _bomdt;           //保存BOM明细DT(生成时使用)
        private string _oaorder;            //获取OA流水号
        private DataTable _Importdt;        //获取准备提交的DT(提交时使用)
        private string _funState;           //记录单据状态(C:创建 R:读取) 
        private DataTable _deldt;           //记录从GridView内需要删除的DT记录(单据状态为R时使用)
        private int _fid;                   //记录从BOM报价单查询出来获取的FID值(查询明细时使用)
        private int _type;                  //记录占用情况类型(更新单据占用情况时使用)

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

        /// <summary>
        /// 获取OA流水号
        /// </summary>
        public string Oaorder { set { _oaorder = value; } }

        /// <summary>
        /// 获取准备提交的DT(提交时使用)
        /// </summary>
        public DataTable Importdt { set { _Importdt = value; } }

        /// <summary>
        /// 记录单据状态(C:创建 R:读取) 
        /// </summary>
        public string FunState { set { _funState = value; } }

        /// <summary>
        /// 记录从GridView内需要删除的DT记录(单据状态为R时使用)
        /// </summary>
        public DataTable Deldt { set { _deldt = value; } }

        /// <summary>
        /// 记录从BOM报价单查询出来获取的FID值(查询明细时使用)
        /// </summary>
        public int Fid { set { _fid = value; } }

        /// <summary>
        /// 记录占用情况类型(更新单据占用情况时使用)
        /// </summary>
        public int Type { set { _type = value; } }

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
                //查询:调用物料明细窗体时使用
                case "0.2":
                    SearchMaterialdt(_searchid,_searchvalue);
                    break;
                //查询:OA流水号是否存在
                case "0.3":
                    SearchOaOrder(_oaorder);
                    break;
                //单据查询-查询端使用
                case "0.4":
                    SearchBomOrder(_searchid, _searchvalue);
                    break;
                //单据查询-查询明细记录使用
                case "0.5":
                    SearchBomOrderDetail(_fid);
                    break;
                //查询-占用记录
                case "0.6":
                    SearchUseInfo(_fid);
                    break;
                //单据查询-主窗体使用
                case "0.7":
                    SearchMainBomOrder(_searchid,_searchvalue,GlobalClasscs.User.StrUsrName);
                    break;

                //运算
                case "1":
                    Generatedt(_valuelist,_dt,_bomdt);
                    break;
                //提交
                case "2":
                    ImportDt(_funState,_Importdt,_deldt);
                    break;
                //更新单据状态
                case "3":
                    UpdateOrderStatus(_fid);
                    break;
                //更新单据占用状态
                case "4":
                    UpdateUseDetail(_fid, _type);
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
            _resultTable = searchDt.SearchValue(searchid, searchvalue);
        }

        /// <summary>
        /// 初始化获取BOM明细记录
        /// </summary>
        private void Searchbomdt()
        {
            _resultbomdt = searchDt.SearchBomDtl();
        }

        /// <summary>
        /// 初始化物料信息(添加物料明细信息时使用)
        /// </summary>
        private void SearchMaterialdt(int searchid, string searchvalue)
        {
            _resultTable = searchDt.SearchMaterialDtl(searchid,searchvalue);
        }

        /// <summary>
        /// 检测OA流水号是否存在
        /// </summary>
        /// <param name="orderno"></param>
        private void SearchOaOrder(string orderno)
        {
            _resultMark = searchDt.SearchOaOrderInclud(orderno);
        }

        /// <summary>
        /// 查询端使用
        /// </summary>
        private void SearchBomOrder(int typeid, string value)
        {
            _resultTable = searchDt.SearchBomOrder(typeid,value);
        }

        /// <summary>
        /// Main查询使用
        /// </summary>
        /// <param name="typeid"></param>
        /// <param name="value"></param>
        /// <param name="createname">创建人</param>
        private void SearchMainBomOrder(int typeid, string value,string createname)
        {
            _resultTable = searchDt.SearchMainBomOrder(typeid,value,createname);
        }

        /// <summary>
        /// 根据FID查询BOM报价单明细
        /// </summary>
        private void SearchBomOrderDetail(int fid)
        {
            _resultTable = searchDt.SearchBomOrderDetail(fid);
        }

        /// <summary>
        /// 根据FID查询此单据占用情况
        /// </summary>
        /// <param name="fid"></param>
        private void SearchUseInfo(int fid)
        {
            _resultTable = searchDt.SearchUseInfo(fid);
        }





///////////////////////////////////////////////////////////////////////////////////////////////////////////////

        /// <summary>
        /// 运算
        /// </summary>
        /// <param name="valuelist">FMATERIALID列表ID</param>
        /// <param name="sourcedt"></param>
        /// <param name="bomdt">BOM明细记录DT</param>
        private void Generatedt(string valuelist,DataTable sourcedt,DataTable bomdt)
        {
            //若_resultTable有值,即先将其清空,再进行赋值
            if (_resultTable.Rows.Count > 0)
            {
                _resultTable.Rows.Clear();
                _resultTable.Columns.Clear();
            }
            _resultTable = generateDt.Generatedt(valuelist,sourcedt,bomdt);
            _resultMark = _resultTable.Rows.Count > 0;
        }

        /// <summary>
        /// 提交
        /// </summary>
        /// <param name="funState">单据状态</param>
        /// <param name="souredt">Tab Pages收集过来的DT</param>
        /// <param name="deldt">需要删除的记录DT(单据状态为R时使用)</param>
        private void ImportDt(string funState,DataTable souredt,DataTable deldt)
        {
            _resultMark = importDt.ImportDtToDb(funState,souredt,deldt);
        }

        /// <summary>
        /// 更新单据状态
        /// </summary>
        /// <param name="fid"></param>
        private void UpdateOrderStatus(int fid)
        {
            _resultMark = generateDt.UpdateOrderStatus(fid);
        }

        /// <summary>
        /// 根据FID查询此单据占用情况
        /// </summary>
        /// <param name="fid"></param>
        /// <param name="type">type:0(更新当前用户信息) 1(清空占用记录)</param>
        private void UpdateUseDetail(int fid,int type)
        {
            generateDt.UpDateUpUseDetail(fid,type);
        }


    }
}
