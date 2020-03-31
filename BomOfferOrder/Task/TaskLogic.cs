using System.Data;

namespace BomOfferOrder.Task
{
    public class TaskLogic
    {
        SearchDt searchDt=new SearchDt();
        GenerateDt generateDt=new GenerateDt();
        ImportDt importDt=new ImportDt();
        ImportTempDt importTempDt=new ImportTempDt();

        #region 变量定义
        private string _taskid;             //记录中转ID
        private int _searchid;              //记录查询列表ID值(查询时使用)
        private string _searchvalue;        //查询值(查询时使用)
        private DataTable _dt;              //保存需要进行生成明细记录的DT
        private DataTable _bomdt;           //保存BOM明细DT(生成时使用)
        private string _oaorder;            //获取OA流水号
        private DataTable _importdt;        //获取准备提交的DT(提交时使用)
        private string _funState;           //记录单据状态(C:创建 R:读取) 
        private DataTable _deldt;           //记录从GridView内需要删除的DT记录(单据状态为R时使用)
        private int _fid;                   //记录从BOM报价单查询出来获取的FID值(查询明细时使用)
        private int _type;                  //记录占用情况类型(更新单据占用情况时使用)
        private string _newpwd;             //记录用户新密码
        private string _fileAddress;        //导入地址
        private DataTable _instockdt;       //保存采购入库单相关DT(报表-旧标准成本单价功能使用)
        private DataTable _pricelistdt;     //保存采购价目表DT(报表功能使用)
        private string _reporttype;         //导入EXCEL时的类型(0:报表功能使用  1:BOM物料明细使用 2:)
        private DataTable _salespricedt;     //保存销售价目表相关DT
        private DataTable _purchaseinstockdt;//保存采购入库单相关DT-(毛利润报表生成使用)
        private DataTable _rencostdt;        //保存人工制造费用相关DT

        private DataTable _resultTable;          //返回DT类型
        private DataTable _resultbomdt;          //返回BOM DT
        private bool _resultMark;                //返回是否成功标记
        private DataTable _importExceldtTable;   //返回导入EXCEL的DT
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
        public DataTable Importdt { set { _importdt = value; } }

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

        /// <summary>
        /// 记录用户新密码
        /// </summary>
        public string Newpwd { set { _newpwd = value; } }

        /// <summary>
        /// 导入EXCEL地址
        /// </summary>
        public string FileAddress { set { _fileAddress = value; }}

        /// <summary>
        /// 保存入库单相关DT(报表功能使用)
        /// </summary>
        public DataTable Instockdt { set { _instockdt = value; } }

        /// <summary>
        /// 保存价目表DT(报表功能使用)
        /// </summary>
        public DataTable Pricelistdt { set { _pricelistdt = value; } }

        /// <summary>
        /// 导入EXCEL时的类型(0:报表功能使用  1:BOM物料明细使用)
        /// </summary>
        public string Reporttype { set { _reporttype = value; } }

        /// <summary>
        /// 保存销售价目表相关DT
        /// </summary>
        public DataTable Salespricedt { set { _salespricedt = value; } }

        /// <summary>
        /// 保存采购入库单相关DT-(毛利润报表生成使用)
        /// </summary>
        public DataTable Purchaseinstockdt { set { _purchaseinstockdt = value; } }

        /// <summary>
        /// 保存人工制造费用相关DT
        /// </summary>
        public DataTable Rencostdt { set { _rencostdt = value; } }

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

        /// <summary>
        /// 返回导入EXCEL的DT
        /// </summary>
        public DataTable ImportExceldtTable => _importExceldtTable;
        #endregion

        public void StartTask()
        {
            switch (_taskid)
            {
                #region 查询
                //查询:查询窗体使用
                case "0":
                    Searchdt(_searchid, _searchvalue);
                    break;
                //查询:初始化获取BOM明细记录时使用
                case "0.1":
                    Searchbomdt();
                    break;
                //查询:调用物料明细窗体时使用
                case "0.2":
                    SearchMaterialdt(_searchid, _searchvalue);
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
                    SearchMainBomOrder(_searchid, _searchvalue, GlobalClasscs.User.StrUsrName);
                    break;
                //权限查询-用户权限主窗体使用
                case "0.8":
                    SearchAdminDetail(_searchid, _searchvalue);
                    break;
                //权限查询-获取K3用户记录(初始化及查询页面使用)
                case "0.9":
                    SearchK3User(_searchvalue);
                    break;
                //获取用户权限表记录
                case "0.9.1":
                    SearchUseDetail();
                    break;
                //查询:检测该用户是否占用
                case "0.9.2":
                    SearchUseUseid(_fid);
                    break;
                //查询:新产品报价单历史记录
                case "0.9.3":
                    SearchOfferBomHistoryDtl(_searchid, _searchvalue);
                    break;
                //查询:客户记录
                case "0.9.4":
                    SearchK3CustomerList(_searchid, _searchvalue);
                    break;
                //查询:主窗体-暂存使用
                case "0.9.5":
                    SearchTempOrder(GlobalClasscs.User.StrUsrName);
                    break;
                //查询:‘暂存’单据明细记录使用
                case "0.9.6":
                    SearchTempOrderDetail(_fid);
                    break;
                #endregion

                #region 运算
                //运算
                case "1":
                    Generatedt(/*_valuelist,*/ _dt, _bomdt);
                    break;
                #endregion
                
                #region 提交
                //提交
                case "2":
                    ImportDt(_funState,_importdt,_deldt);
                    break;
                //用户权限提交
                case "2.1":
                    ImportUserPermissionDt(_importdt);
                    break;
                //暂存信息提交
                case "2.2":
                    ImportTempDt(_importdt);
                    break;
                //暂存信息删除
                case "2.3":
                    DelTempDt(_fid);
                    break;
                #endregion

                #region 更新
                //更新单据状态
                case "3":
                    UpdateOrderStatus(_fid);
                    break;
                //更新单据占用状态
                case "4":
                    UpdateUseDetail(_fid, _type,_oaorder);
                    break;
                //更新用户权限占用状态
                case "4.1":
                    UpAccountUseridValue(_fid, _type);
                    break;
                //更新用户密码
                case "4.2":
                    UpdateUserNewpwd(_fid, _newpwd);
                    break;
                #endregion

                #region 报表
                //查询-物料明细
                case "5":
                    SearchMaterial(_searchid,_searchvalue);
                    break;
                //运算-批量报表生成使用
                case "5.1":
                    GenerateReportDt(_dt, _bomdt,_instockdt,_pricelistdt);
                    break;
                //查询:入库单相关(报表使用)
                case "5.3":
                    SearchInstockDt();
                    break;
                //查询:采购价目表相关(报表使用)
                case "5.4":
                    SearchPricelistDt();
                    break;

                //运算-毛利润报表生成使用
                case "5.5":
                    GenerateProfitDt(_dt, _bomdt, _instockdt, _pricelistdt,_salespricedt,_purchaseinstockdt,_rencostdt);
                    break;
                //查询-销售价目表DT
                case "5.6":
                    SearchSalesPriceDt();
                    break;
                //查询-采购入库单DT(毛利润报表使用)
                case "5.7":
                    SearchPurchaseInstockDt();
                    break;
                //查询-人工制造费用DT
                case "5.8":
                    SearchRenCostDt();
                    break;

                #endregion

                #region 导入(包括:报表EXCEL导入 以及 BOM物料明细导入)
                //EXCEL模板导入-报表使用
                case "5.2":
                    ImportExcelToDt(_reporttype,_fileAddress);
                    break;
                //EXCEL模板导入-BOM明细记录使用
                case "6":
                    ImportExcelToBomDt(_reporttype,_fileAddress);
                    break;
                //EXCEL模板导入-毛利润报表使用
                case "6.1":
                    ImportExcelToProfitDt(_reporttype,_fileAddress);
                    break;
                #endregion
            }
        }

        #region 查询相关方法
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
            _resultTable = searchDt.SearchMaterialDtl(searchid, searchvalue);
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
        /// 查询端窗体使用
        /// </summary>
        private void SearchBomOrder(int typeid, string value)
        {
            _resultTable = searchDt.SearchBomOrder(typeid, value);
        }

        /// <summary>
        /// Main查询使用
        /// </summary>
        /// <param name="typeid"></param>
        /// <param name="value"></param>
        /// <param name="createname">创建人</param>
        private void SearchMainBomOrder(int typeid, string value, string createname)
        {
            _resultTable = searchDt.SearchMainBomOrder(typeid, value, createname);
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

        /// <summary>
        /// 权限查询-用户权限主窗体使用
        /// </summary>
        /// <param name="typeid"></param>
        /// <param name="value"></param>
        private void SearchAdminDetail(int typeid, string value)
        {
            _resultTable = searchDt.SearchAdminDetail(typeid, value);
        }

        /// <summary>
        /// 查询K3用户信息(初始化及查询页面使用)
        /// </summary>
        /// <param name="value"></param>
        private void SearchK3User(string value)
        {
            _resultTable = searchDt.SearchK3User(value);
        }

        /// <summary>
        /// 获取用户权限表明细
        /// </summary>
        private void SearchUseDetail()
        {
            _resultTable = searchDt.SearchUseDetail();
        }

        /// <summary>
        /// 根据USERID查询用户是否占用
        /// </summary>
        private void SearchUseUseid(int userid)
        {
            _resultTable = searchDt.SearchAdminUserInfo(userid);
        }

        /// <summary>
        /// 查询‘新产品报价单历史记录’
        /// </summary>
        /// <param name="searchid"></param>
        /// <param name="searchvalue"></param>
        private void SearchOfferBomHistoryDtl(int searchid, string searchvalue)
        {
            _resultTable = searchDt.SearchOfferBomHistoryDtl(searchid,searchvalue);
        }

        /// <summary>
        /// 查询K3客户信息
        /// </summary>
        /// <param name="searchid"></param>
        /// <param name="searchvalue"></param>
        private void SearchK3CustomerList(int searchid, string searchvalue)
        {
            _resultTable = searchDt.SearchK3CustomerList(searchid, searchvalue);
        }

        /// <summary>
        /// '暂存'查询使用
        /// </summary>
        private void SearchTempOrder(string createname)
        {
            _resultTable = searchDt.SearchTempOrder(createname);
        }

        /// <summary>
        /// '暂存'单据明细使用
        /// </summary>
        /// <param name="fid"></param>
        private void SearchTempOrderDetail(int fid)
        {
            _resultTable = searchDt.SearchTempOrderDetail(fid);
        }


        #endregion

        #region 运算相关方法
        /// <summary>
        /// 运算
        /// </summary>
        /// <param name="sourcedt"></param>
        /// <param name="bomdt">BOM明细记录DT</param>
        private void Generatedt(DataTable sourcedt, DataTable bomdt)
        {
            //若_resultTable有值,即先将其清空,再进行赋值
            if (_resultTable?.Rows.Count > 0)
            {
                _resultTable.Rows.Clear();
                _resultTable.Columns.Clear();
            }
            _resultTable = generateDt.Generatedt(sourcedt, bomdt);
            _resultMark = _resultTable.Rows.Count > 0;
        }

        #endregion

        #region 提交相关方法
        /// <summary>
        /// 提交
        /// </summary>
        /// <param name="funState">单据状态</param>
        /// <param name="souredt">Tab Pages收集过来的DT</param>
        /// <param name="deldt">需要删除的记录DT(单据状态为R时使用)</param>
        private void ImportDt(string funState, DataTable souredt, DataTable deldt)
        {
            _resultMark = importDt.ImportDtToDb(funState, souredt, deldt);
        }

        /// <summary>
        /// 用户权限提交
        /// </summary>
        private void ImportUserPermissionDt(DataTable sourcedt)
        {
            _resultMark = importDt.ImportUserPermissionDt(sourcedt);
        }

        /// <summary>
        /// 暂存功能提交使用
        /// </summary>
        /// <param name="sourcedt"></param>
        private void ImportTempDt(DataTable sourcedt)
        {
            _resultMark = importTempDt.ImportTempDtToDb(sourcedt);
        }

        private void DelTempDt(int searchvalue)
        {
            _resultMark = importTempDt.DeleteRecord(searchvalue);
        }
        #endregion

        #region 更新相关方法
        /// <summary>
        /// 更新单据状态
        /// </summary>
        /// <param name="fid"></param>
        private void UpdateOrderStatus(int fid)
        {
            _resultMark = generateDt.UpdateOrderStatus(fid);
        }

        /// <summary>
        /// 根据FID更新此单据占用情况
        /// </summary>
        /// <param name="fid"></param>
        /// <param name="type">type:0(更新当前用户信息) 1(清空占用记录)</param>
        /// <param name="oano">OA流水号(当单据状态为C时使用)</param>
        private void UpdateUseDetail(int fid, int type,string oano)
        {
            generateDt.UpDateUpUseDetail(fid, type,oano);
        }

        /// <summary>
        /// 根据userid更新此用户的占用记录
        /// </summary>
        /// <param name="userid"></param>
        /// <param name="type">0:更新用户占用 1:清空</param>
        private void UpAccountUseridValue(int userid, int type)
        {
            generateDt.UpAccountUseridValue(userid, type);
        }

        /// <summary>
        /// 更新用户新密码
        /// </summary>
        /// <param name="userid"></param>
        /// <param name="newpwd"></param>
        private void UpdateUserNewpwd(int userid,string newpwd)
        {
            _resultMark = generateDt.UpdateUserNewpwd(userid,newpwd);
        }

        #endregion

        #region 报表相关

        /// <summary>
        ///报表-物料明细查询
        /// </summary>
        private void SearchMaterial(int searchid,string searchvalue)
        {
            _resultTable = searchDt.SearchMaterial(searchid,searchvalue);
        }

        /// <summary>
        /// 运算-批量成本报表生成使用
        /// </summary>
        /// <param name="sourcedt">EXCEL数据源</param>
        /// <param name="bomdt">BOM数据源DT</param>
        /// <param name="instockdt">保存采购入库单相关DT(报表功能-旧标准成本单价使用)</param>
        /// <param name="priceListdt">保存价目表DT(报表功能使用)</param>
        private void GenerateReportDt(DataTable sourcedt, DataTable bomdt,DataTable instockdt,DataTable priceListdt)
        {
            _resultTable = generateDt.GenerateReportDt(sourcedt, bomdt,instockdt,priceListdt);
            _resultMark = _resultTable.Rows.Count > 0;
        }

        /// <summary>
        /// 查询采购入库单相关
        /// </summary>
        private void SearchInstockDt()
        {
            _resultTable = searchDt.SearchInstockDt();
        }

        /// <summary>
        /// 查询采购价目表相关-旧标准成本单价使用
        /// </summary>
        private void SearchPricelistDt()
        {
            _resultTable = searchDt.SearchPricelistDt();
        }

        /// <summary>
        /// 运算-毛利润所表生成使用
        /// </summary>
        /// <param name="sourcedt">EXCEL数据源</param>
        /// <param name="bomdt">BOM数据源DT</param>
        /// <param name="instockdt">保存采购入库单相关DT(报表功能-旧标准成本单价使用)</param>
        /// <param name="priceListdt">保存价目表DT(报表功能使用)</param>
        /// <param name="salespricedt">保存销售价目表相关</param>
        /// <param name="purchaseinstockdt">保存采购入库单相关-(毛利润报表生成使用)</param>
        /// <param name="rencostdt">保存人工制造费用相关</param>
        private void GenerateProfitDt(DataTable sourcedt, DataTable bomdt, DataTable instockdt, DataTable priceListdt,
                                      DataTable salespricedt,DataTable purchaseinstockdt,DataTable rencostdt)
        {
            _resultTable = generateDt.GenerateProfitReportDt(sourcedt,bomdt,instockdt,priceListdt,salespricedt,purchaseinstockdt,rencostdt);
            _resultMark = _resultTable.Rows.Count > 0;
        }

        /// <summary>
        /// 查询销售价目表相关
        /// </summary>
        private void SearchSalesPriceDt()
        {
            _resultTable = searchDt.SearchSalesPriceDt();
        }

        /// <summary>
        /// 查询采购入库单相关-(毛利润报表生成使用)
        /// </summary>
        private void SearchPurchaseInstockDt()
        {
            _resultTable = searchDt.SearchPurchaseInstockDt();
        }

        /// <summary>
        /// 查询人工制造费用相关
        /// </summary>
        private void SearchRenCostDt()
        {
            _resultTable = searchDt.SearchRenCostDt();
        }


        #endregion

        #region 导入(包括:报表EXCEL导入 以及 BOM物料明细导入)

        /// <summary>
        /// 导入EXCEL-批量成本报表使用
        /// </summary>
        /// <param name="reporttype">导入EXCEL时的类型(0:批量成本报表功能使用  1:BOM物料明细使用 2:毛利润报表使用)</param>
        /// <param name="fileAddress"></param>
        private void ImportExcelToDt(string reporttype,string fileAddress)
        {
            //若_resultTable有值,即先将其清空,再进行赋值
            if (_importExceldtTable?.Rows.Count > 0)
            {
                _importExceldtTable.Rows.Clear();
                _importExceldtTable.Columns.Clear();
            }
            _importExceldtTable = importDt.ImportExcelToDt(reporttype,fileAddress);
        }

        /// <summary>
        /// EXCEL模板导入-BOM明细记录使用
        /// </summary>
        /// <param name="reporttype"></param>
        /// <param name="fileAddress"></param>
        private void ImportExcelToBomDt(string reporttype, string fileAddress)
        {
            //若_resultTable有值,即先将其清空,再进行赋值
            if (_importExceldtTable?.Rows.Count > 0)
            {
                _importExceldtTable.Rows.Clear();
                _importExceldtTable.Columns.Clear();
            }
            _importExceldtTable = importDt.ImportExcelToDt(reporttype, fileAddress);
        }

        /// <summary>
        /// 导入EXCEL-毛利润报表使用
        /// </summary>
        /// <param name="reporttype">导入EXCEL时的类型(0:批量成本报表功能使用  1:BOM物料明细使用 2:毛利润报表使用)</param>
        /// <param name="fileAddress"></param>
        private void ImportExcelToProfitDt(string reporttype, string fileAddress)
        {
            //若_resultTable有值,即先将其清空,再进行赋值
            if (_importExceldtTable?.Rows.Count > 0)
            {
                _importExceldtTable.Rows.Clear();
                _importExceldtTable.Columns.Clear();
            }
            _importExceldtTable = importDt.ImportExcelToDt(reporttype, fileAddress);
        }

        #endregion
    }
}
