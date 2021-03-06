﻿using System;
using System.Data;
using System.Drawing;
using System.Globalization;
using System.Text.RegularExpressions;
using System.Threading;
using System.Windows.Forms;
using BomOfferOrder.DB;
using BomOfferOrder.Task;
using BomOfferOrder.UI.ReportFrm;
using Stimulsoft.Report;

namespace BomOfferOrder.UI
{
    public partial class Main : Form
    {
        TaskLogic task=new TaskLogic();
        Load load=new Load();
        ChangeAccount changeAccount=new ChangeAccount();
        MaterialReportFrm materialReportFrm=new MaterialReportFrm();
        DtlFrm dtlFrm=new DtlFrm();
        DbList dbList=new DbList();

        #region 变量参数
        //保存BOM明细DT(生成时使用;注:当打开录入界面时初始化执行)
        private DataTable _bomdt;
        //保存采购入库单相关DT(报表-旧标准成本单价使用)
        private DataTable _instockdt;
        //保存采购价目表DT(报表功能及BOM明细中各物料的‘物料单价’使用)
        private DataTable _pricelistdt;
        //保存销售价目表DT(毛利润报表使用)
        private DataTable _salesPricedt;
        //保存采购入库单相关DT(毛利润报表使用)
        private DataTable _purchaseInstockDt;
        //保存人工制造费用DT(毛利润报表使用)
        private DataTable _renCostDt;
        //保存‘研发类别’DT
        private DataTable _devgroupdt;

        //定义关闭符号的宽
        const int CloseSize = 11;

        //保存查询出来的GridView记录
        private DataTable _dtl;

        //记录当前页数(GridView页面跳转使用)
        private int _pageCurrent = 1;
        //记录计算出来的总页数(GridView页面跳转使用)
        private int _totalpagecount;
        //记录初始化标记(GridView页面跳转 初始化时使用)
        private bool _pageChange;

        #region 暂存功能模块
        //保存查询出来的GridViewTemp记录(注:暂存功能使用)
        private DataTable _tempdtl;
        //记录当前页数(GridView页面跳转使用)
        private int _pageCurrentTemp = 1;
        //记录计算出来的总页数(GridView页面跳转使用)
        private int _totalpagecountTemp;
        //记录初始化标记(GridView页面跳转 初始化时使用)
        private bool _pageChangeTemp;
        #endregion

        #endregion

        public Main()
        {
            InitializeComponent();
            OnRegisterEvents();
            OnInitialize();
        }

        /// <summary>
        /// 初始化相关记录
        /// </summary>
        private void OnInitialize()
        {
            pbimg.BackgroundImage = Image.FromFile(Application.StartupPath + @"\PIC\2.png");
            var tip1=new ToolTip();
            tip1.SetToolTip(btnSearch,"成本Bom报价单-查询");
            tip1.SetToolTip(btnCreate,"成本Bom报价单-创建");
            tip1.SetToolTip(btnCreateNew, "新产品成本报价单-创建");
            tip1.SetToolTip(btngenemptynew,"空白报价单-创建");
            tip1.SetToolTip(btnreport,"报表生成");

            //初始化登入用户信息
            lbaccountmessage.Text = GlobalClasscs.User.StrUsrName;
            lbaccountmessage.ForeColor = Color.Brown;
            //初始化登入用户时间
            lbaccountdt.Text = DateTime.Now.ToString();
            lbaccountdt.ForeColor = Color.Brown;

            //初始化查询下拉列表
            OnShowSelectTypeList();
            //初始化BOM明细DT(‘生成BOM明细’及‘报表查询’功能时使用)
            OnInitializeBomdt();
            //初始化入库单相关DT(报表旧标准成本单价功能时使用)
            OnInitializeInstockdt();
            //初始化价目表相关DT(报表查询功能时使用)
            OnInitializePriceListdt();
            //初始化销售价目表DT
            OnInitializeSalesPricedt();
            //初始化采购入库单DT-(毛利润报表使用)
            OnInitializePurchaseInstockDt();
            //初始化人工制造费用DT
            OnInitializeRenCostDt();
            //初始化研发类别DT
            OnInitializeDevGroup();
            //更新用户占用值 useid
            UpUseridValue(0);
            //权限控制
            PrivilegeControl();
        }

        private void OnRegisterEvents()
        {
            btnSearch.Click += BtnSearch_Click;
            btnCreate.Click += BtnCreate_Click;
            btnusersearch.Click += Btnusersearch_Click;
            tmbackconfirm.Click += Tmbackconfirm_Click;
            tmshowdetail.Click += Tmshowdetail_Click;
            comselectvalue.SelectedIndexChanged += Comselectvalue_SelectedIndexChanged;
            tmchangepwd.Click += Tmchangepwd_Click;
            this.FormClosing += Main_FormClosing;
            btnCreateNew.Click += BtnCreateNew_Click;
            btngenemptynew.Click += Btngenemptynew_Click;
            btnreport.Click += Btnreport_Click;
            tmorderdel.Click += Tmorderdel_Click; ;

            bnMoveFirstItem.Click += BnMoveFirstItem_Click;
            bnMovePreviousItem.Click += BnMovePreviousItem_Click;
            bnMoveNextItem.Click += BnMoveNextItem_Click;
            bnMoveLastItem.Click += BnMoveLastItem_Click;
            bnPositionItem.Leave += BnPositionItem_Leave;
            tmshowrows.DropDownClosed += Tmshowrows_DropDownClosed;
            panel8.Visible = false;

            dpstart.Enabled = false;
            tctotalpage.DrawItem += Tctotalpage_DrawItem;
            tctotalpage.MouseDown += Tctotalpage_MouseDown;
            tctotalpage.Padding = new Point(CloseSize, CloseSize - 8);   //初始化时添加Tab Control控件各Page选项卡的额外宽与高(重)

            /////////暂存功能页///////////
            bnMoveFirstItemtemp.Click += BnMoveFirstItemtemp_Click;
            bnMovePreviousItemtemp.Click += BnMovePreviousItemtemp_Click;
            bnMoveNextItemtemp.Click += BnMoveNextItemtemp_Click;
            bnMoveLastItemtemp.Click += BnMoveLastItemtemp_Click;
            bnPositionItemtemp.Leave += BnPositionItemtemp_Leave;
            tmshowrowstemp.DropDownClosed += Tmshowrowstemp_DropDownClosed;
            panel10.Visible = false;
            btnrefresh.Click += Btnrefresh_Click;
            tmshowtempdetail.Click += Tmshowtempdetail_Click;
            tmtempdel.Click += Tmtempdel_Click;
        }

        /// <summary>
        /// 根据所选的选择条件刷新GridView
        /// </summary>
        private void OnSearch()
        {
            //获取下拉列表所选值
            var dvordertylelist = (DataRowView)comselectvalue.Items[comselectvalue.SelectedIndex];
            var typeId = Convert.ToInt32(dvordertylelist["Id"]);

            task.TaskId = "0.7";
            task.SearchId = typeId;

            switch (typeId)
            {
                case 0:   //OA流水号
                case 1:   //产品名称
                    task.SearchValue = txtvalue.Text;
                    break;
                case 2:   //创建日期
                case 3:   //审核日期
                    //task.SearchValue = Convert.ToString(dpstart.Value.Date, CultureInfo.InvariantCulture);
                    task.Dpstart = Convert.ToString(dpstart.Value.Date);
                    task.Dpend = Convert.ToString(dpend.Value.Date);
                    break;
                default:  //单据状态  研发类别
                    var statuslist = (DataRowView)comselectvalue.Items[comstatus.SelectedIndex];
                    var id = Convert.ToInt32(statuslist["Id"]);
                    task.SearchValue = Convert.ToString(id);
                    break;
            }

            new Thread(Start).Start();
            load.StartPosition = FormStartPosition.CenterScreen;
            load.ShowDialog();

            if (task.ResultTable.Rows.Count > 0)
            {
                _dtl = task.ResultTable;
                panel8.Visible = true;
                //初始化下拉框所选择的默认值
                //tmshowrows.SelectedItem = "10";
                tmshowrows.SelectedItem = Convert.ToInt32(tmshowrows.SelectedItem) == 0
                    ? (object)"10"
                    : Convert.ToInt32(tmshowrows.SelectedItem);
                //定义初始化标记(设置为不初始化)
                _pageChange = _pageCurrent <= 1;
                //GridView分页
                GridViewPageChange();
            }
            //注:当为空记录时,不显示跳转页;只需将临时表赋值至GridView内
            else
            {
                gvdtl.DataSource = task.ResultTable;
                panel8.Visible = false;
            }
            //控制GridView单元格显示方式
            ControlGridViewisShow();
        }

        /// <summary>
        /// 修改用户密码
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Tmchangepwd_Click(object sender, EventArgs e)
        {
            try
            {
                changeAccount.StartPosition = FormStartPosition.CenterParent;
                changeAccount.ShowDialog();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, $"错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        /// <summary>
        /// 当条件查询下拉列表ID发生变化时使用
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Comselectvalue_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                //获取下拉列表所选值
                var dvordertylelist = (DataRowView)comselectvalue.Items[comselectvalue.SelectedIndex];
                var typeId = Convert.ToInt32(dvordertylelist["Id"]);

                //OA流水号  产品名称
                switch (typeId)
                {
                    //OA流水号
                    case 0:
                    //产品名称
                    case 1:
                        txtvalue.Text = "";
                        txtvalue.Visible = true;
                        comstatus.Visible = false;

                        dpstart.Visible = false;
                        dpend.Visible = false;
                        label2.Visible = false;
                        break;
                    //创建日期
                    case 2:
                    //审核日期
                    case 3:
                       // dpstart.Value = DateTime.Today;
                        dpstart.Visible = true;
                        dpstart.Enabled = true;
                        dpend.Visible = true;
                        dpend.Enabled = true;
                        label2.Visible = true;
                        //将单据状态 及 文本框控件隐藏
                        comstatus.Visible = false;
                        txtvalue.Visible = false;
                        break;
                    //单据状态
                    case 4:
                        comstatus.Visible = true;
                        //将日期 以及 文本框控件隐藏
                        dpstart.Visible = false;
                        dpend.Visible = false;
                        label2.Visible = false;

                        txtvalue.Visible = false;
                        //初始化单据状态下拉列表
                        OnShowStatusList();
                        break;
                    //研发类别
                    case 5:
                        comstatus.Visible = true;
                        //将日期 以及 文本框控件隐藏
                        dpstart.Visible = false;
                        dpend.Visible = false;
                        label2.Visible = false;

                        txtvalue.Visible = false;
                        //初始化研发类别下拉列表
                        OnShowDevGroupList();
                        break;
                        #region 其它项:单据状态 研发类别
                        ////其它项:单据状态 研发类别
                        //default:
                        //    comstatus.Visible = true;
                        //    //将日期 以及 文本框控件隐藏
                        //    dtpdt.Visible = false;
                        //    txtvalue.Visible = false;
                        //    //初始化单据状态下拉列表
                        //    OnShowStatusList();
                        //    //初始化研发类别下拉列表
                        //    OnShowDevGroupList();
                        //    break;
                        #endregion
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, $"错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        /// <summary>
        /// 用户记录查询
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Btnusersearch_Click(object sender, EventArgs e)
        {
            try
            {
                //根据所选的选择条件刷新GridView
                OnSearch();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, $"错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        /// <summary>
        /// 显示单据明细
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Tmshowdetail_Click(object sender, EventArgs e)
        {
            try
            {
                if(gvdtl.SelectedRows.Count==0)throw new Exception("没有明细记录,不能继续操作");
                //获取OA流水号
                var oaorder = Convert.ToString(gvdtl.Rows[gvdtl.CurrentCell.RowIndex].Cells[1].Value);
                //根据所选择的行获取其fid值
                var fid = Convert.ToInt32(gvdtl.Rows[gvdtl.CurrentCell.RowIndex].Cells[0].Value);
                //根据Fid获取数据库useid 及 username值;并根据useid判断是否占用
                var usedt = ShowUsedtl(fid);
                if (Convert.ToInt32(usedt.Rows[0][0])==0)
                    throw new Exception($"所选单据'{oaorder}'不能进入, \n 原因:已被用户'{Convert.ToString(usedt.Rows[0][1])}'占用,需用户'{Convert.ToString(usedt.Rows[0][1])}'退出才能继续操作");

                task.TaskId = "0.5";
                task.Fid = fid;

                new Thread(Start).Start();
                load.StartPosition = FormStartPosition.CenterScreen;
                load.ShowDialog();

                //弹出对应窗体相关设置
                //初始化信息
                dtlFrm.FunState = "R";
                dtlFrm.OnInitialize(task.ResultTable, _pricelistdt,_instockdt,_devgroupdt);     
                dtlFrm.StartPosition = FormStartPosition.CenterParent;
                dtlFrm.ShowDialog();

                //根据所选的选择条件刷新GridView
                OnSearch();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, $"错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        /// <summary>
        /// 反审核
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Tmbackconfirm_Click(object sender, EventArgs e)
        {
            try
            {
                if (gvdtl.SelectedRows.Count == 0) throw new Exception("没有明细记录,不能继续操作");

                //获取OA流水号
                var oaorder = Convert.ToString(gvdtl.Rows[gvdtl.CurrentCell.RowIndex].Cells[1].Value);
                //根据所选择的行获取其fid值
                var fid = Convert.ToInt32(gvdtl.Rows[gvdtl.CurrentCell.RowIndex].Cells[0].Value);
                //根据所选择的行获取其单据状态
                var orderstatus = Convert.ToString(gvdtl.Rows[gvdtl.CurrentCell.RowIndex].Cells[4].Value);

                //判断若所选的行中的‘单据状态’不为已审核,即跳出异常
                if(orderstatus!="已审核") throw new Exception($"单据'{oaorder}'的单据状态不为已审核,不能进行反审核操作");

                //判断是否能进行反审核
                if (!GlobalClasscs.User.Canbackconfirm) throw new Exception($"用户'{GlobalClasscs.User.StrUsrName}'没有反审核权限,不能进行反审核");

                
                //根据Fid获取数据库useid 及 username值;并根据useid判断是否占用
                var usedt = ShowUsedtl(fid);
                if (Convert.ToInt32(usedt.Rows[0][0]) == 0)
                    throw new Exception($"所选单据'{oaorder}'不能进入, \n 原因:已被用户'{Convert.ToString(usedt.Rows[0][1])}'占用,需用户'{Convert.ToString(usedt.Rows[0][1])}'退出才能继续操作");

                var clickMessage = $"您所选择的单据为:\n '{oaorder}' \n 是否进行反审核?";
                if (MessageBox.Show(clickMessage, $"提示", MessageBoxButtons.YesNo, MessageBoxIcon.Information) == DialogResult.Yes)
                {
                    task.TaskId = "3";
                    task.Fid = fid;

                    new Thread(Start).Start();
                    load.StartPosition = FormStartPosition.CenterScreen;
                    load.ShowDialog();

                    if(!task.ResultMark)throw new Exception("反审核操作出现异常,请联系管理员.");
                    else
                    {
                        MessageBox.Show($"反审核成功,请右键选择‘查询明细’进入", $"提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        //根据所选的选择条件刷新GridView
                        OnSearch();
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, $"错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        /// <summary>
        /// 成本BOM报价单-创建
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BtnCreate_Click(object sender, EventArgs e)
        {
            try
            {
                CreateTabPages("成本BOM报价单-创建");
                //根据所选的选择条件刷新GridView
                OnSearch();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, $"错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        /// <summary>
        /// 成本BOM报价单-查询
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BtnSearch_Click(object sender, EventArgs e)
        {
            try
            {
                //显示除登入用户外关联的用户单据记录
                CreateTabPages("成本BOM报价单-查询");
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, $"错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        /// <summary>
        /// 新产品成本报价单-创建
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BtnCreateNew_Click(object sender, EventArgs e)
        {
            try
            {
                CreateTabPages("新产品成本报价单-创建");
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, $"错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        /// <summary>
        /// 生成空白报价单
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Btngenemptynew_Click(object sender, EventArgs e)
        {
            try
            {
                //将功能变量设置为E-空白报价单标记
                GlobalClasscs.Fun.EmptyFunctionName = "E";
                //弹出对应窗体相关设置
                dtlFrm.FunState = "C";
                dtlFrm.OnInitialize(null, _pricelistdt,_instockdt, _devgroupdt);     //初始化信息
                dtlFrm.StartPosition = FormStartPosition.CenterParent;
                dtlFrm.ShowDialog();

                //根据所选的选择条件刷新GridView
                OnSearch();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, $"错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        /// <summary>
        /// 报表查询
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Btnreport_Click(object sender, EventArgs e)
        {
            try
            {
                //定义'输出报表地址'
                string printadd;
                //定义'输出类型ID'
                var printtaskid=string.Empty;
                //定义‘物料相关’DT
                DataTable materialdt;

                materialReportFrm.StartPosition = FormStartPosition.CenterParent;
                materialReportFrm.ShowDialog();

                //以下为返回相关记录回本窗体相关处理
                //判断若返回的DT为空的话,就不需要任何效果
                if (materialReportFrm.ResultTable == null || materialReportFrm.ResultTable.Rows.Count == 0) return;
                //将返回的结果赋值至GridView内(注:判断若返回的DT不为空或行数大于0才执行更新效果)
                if (materialReportFrm.ResultTable != null || materialReportFrm.ResultTable.Rows.Count > 0)
                { //searchlist = GetSearchList(materialReportFrm.ResultTable); 

                    //若materialReportFrm.reporttypeid为0或2,需要检测两点:
                    //1)即需要检测其导入的物料是否已在BOMDT内存在若没有,跳过此行,不作异常处理
                    //2)检测其内是否有相同的物料,若有,报异常
                    materialdt = materialReportFrm.ResultTable.Clone();
                    //当reporttypeid=0时,表示执行生成‘批量成本报表’
                    if (materialReportFrm.Reporttypeid == 0)
                    {
                        //循环从EXCEL导入的物料DT
                        foreach (DataRow rows in materialReportFrm.ResultTable.Rows)
                        {
                            //检测若EXCEL导入的物料不在_bomdt存在,即跳过不作异常处理 change date:20200211
                            var dtlrow = _bomdt.Select("表头物料ID='" + rows[0] + "'");
                            if(dtlrow.Length == 0) continue;//throw new Exception($"检测到物料'{rows[1]}'不在BOM记录中存在,请检查后继续");

                            //检测导入DT内的物料是否重复
                            if(materialdt.Select("FMATERIALID='"+rows[0]+"'").Length>0)
                                throw new Exception($"检测到导入的EXCEL中物料'{rows[1]}'有重复记录,请检查后重新导入");

                            //若没有重复即进行插入至materialdt内
                            var newrow = materialdt.NewRow();
                            newrow[0] = rows[0];         //FMATERIALID
                            newrow[1] = rows[1];         //物料编码
                            newrow[2] = rows[2];         //物料名称
                            newrow[3] = rows[3];         //规格
                            newrow[4] = rows[4];         //换算率(密度)
                            newrow[5] = rows[5];         //重量(净重)
                            materialdt.Rows.Add(newrow);
                        }
                    }
                    //表示执行生成'毛利润报表'
                    else if (materialReportFrm.Reporttypeid == 2)
                    {
                        //循环从EXCEL导入的物料DT
                        foreach (DataRow rows in materialReportFrm.ResultTable.Rows)
                        {
                            //检测若EXCEL导入的物料不在_bomdt存在,即跳过不作异常处理
                            var dtlrow = _bomdt.Select("表头物料ID='" + rows[0] + "'");
                            if (dtlrow.Length == 0) continue;

                            //检测导入DT内的物料是否重复
                            if (materialdt.Select("FMATERIALID='" + rows[0] + "'").Length > 0)
                                throw new Exception($"检测到导入的EXCEL中物料'{rows[1]}'有重复记录,请检查后重新导入");

                            //若没有重复即进行插入至materialdt内
                            var newrow = materialdt.NewRow();
                            newrow[0] = rows[0];         //FMATERIALID
                            newrow[1] = rows[1];         //物料编码
                            newrow[2] = rows[2];         //物料名称
                            newrow[3] = rows[3];         //规格
                            newrow[4] = rows[4];         //换算率(密度)
                            newrow[5] = rows[5];         //重量(净重)
                            newrow[6] = rows[6];         //罐箱
                            newrow[7] = rows[7];         //分类
                            newrow[8] = rows[8];         //品类
                            newrow[9] = rows[9];         //销售计价单位
                            newrow[10] = rows[10];       //U订货计价规格
                            materialdt.Rows.Add(newrow);
                        }
                    }
                    //表示不用导入方式生成报表,但也是以‘批量成本’模板方式导出
                    else
                    {
                        materialdt = materialReportFrm.ResultTable;
                    }


                    //分类运算
                    //毛利润报表使用
                    printtaskid = materialReportFrm.Reporttypeid == 2 ? "5.5" : "5.1";

                    //将相关值插入至对应的中转值内
                    task.TaskId = printtaskid;
                    task.Data = materialdt;
                    task.Bomdt = _bomdt;
                    task.Instockdt = _instockdt;                  //初始化采购入库单DT-旧标准成本单价使用
                    task.Pricelistdt = _pricelistdt;              //初始化采购价目表DT
                    task.Salespricedt = _salesPricedt;            //初始化销售价目表DT-(产品成本毛利润报表专用)
                    task.Purchaseinstockdt = _purchaseInstockDt;  //初始化采购入库单DT-(产品成本毛利润报表专用)
                    task.Rencostdt = _renCostDt;                  //初始化人工制造费用DT-(产品成本毛利润报表专用)

                    new Thread(Start).Start();
                    load.StartPosition = FormStartPosition.CenterScreen;
                    load.ShowDialog();

                    if (task.ResultTable.Rows.Count == 0) throw new Exception("导出异常,请联系管理员");
                    //调用STI模板并执行导出代码
                    //加载STI模板
                    //定义模板地址
                    printadd = materialReportFrm.Reporttypeid == 2 ? "/Report/MaterialProfitReport.mrt" : "/Report/MaterialCostReport.mrt";
                    var filepath = Application.StartupPath + printadd;
                    var stireport = new StiReport();
                    stireport.Load(filepath);
                    //加载DATASET 或 DATATABLE
                    stireport.RegData("Order", task.ResultTable);
                    stireport.Compile();
                    stireport.Show();   //调用预览功能
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, $"错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        /// <summary>
        /// 对TABPAGE页画‘关闭’按钮(注:当TabControl需要绘制它的每一个选项卡时发生,直至跳出整个应用程序)
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Tctotalpage_DrawItem(object sender, DrawItemEventArgs e)
        {
            //执行顺序:先画一个矩形框,再填充矩形框,最后画关闭符号
            var myTab = tctotalpage.GetTabRect(e.Index);

            //设置当读取至Index=0 即首页时,就不需要画‘X’关闭图标,其它就需要
            if (e.Index == 0)
            {
                //先添加TabPage属性
                var sf = new StringFormat
                {
                    LineAlignment = StringAlignment.Center, //设置选项卡名称 垂直居中
                    Alignment = StringAlignment.Center      //设置选项卡名称 水平居中 
                };
                e.Graphics.DrawString(tctotalpage.TabPages[e.Index].Text, Font, SystemBrushes.ControlText, myTab, sf);
            }
            else
            {
                //先添加TabPage属性
                e.Graphics.DrawString(tctotalpage.TabPages[e.Index].Text, Font, SystemBrushes.ControlText, myTab.X + 3, myTab.Y + 3);

                //再画一个矩形框
                using (Pen p = new Pen(Color.White))
                {
                    myTab.Offset((myTab.Width - (CloseSize + 3)), 2);
                    myTab.Width = CloseSize;
                    myTab.Height = CloseSize;
                    e.Graphics.DrawRectangle(p, myTab);
                }
                //填充
                Color recColor = e.State == DrawItemState.Selected ? Color.CornflowerBlue : Color.Beige;
                using (Brush b = new SolidBrush(recColor))
                {
                    e.Graphics.FillRectangle(b, myTab);
                }

                //画关闭符号
                using (Pen objpen = new Pen(Color.Black))
                {
                    //'\'线
                    Point p1 = new Point(myTab.X + 3, myTab.Y + 3);
                    Point p2 = new Point(myTab.X + myTab.Width - 3, myTab.Y + myTab.Height - 3);
                    e.Graphics.DrawLine(objpen, p1, p2);
                    //'/'线
                    Point p3 = new Point(myTab.X + 3, myTab.Y + myTab.Height - 3);
                    Point p4 = new Point(myTab.X + myTab.Width - 3, myTab.Y + 3);
                    e.Graphics.DrawLine(objpen, p3, p4);
                }
            }
            e.Graphics.Dispose();
        }

        /// <summary>
        /// 控制当点击‘关闭’按钮时将TABPAGE 页面关闭
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Tctotalpage_MouseDown(object sender, MouseEventArgs e)
        {
            //当使用Mouse左键点击 除‘首页’外的选项卡时执行
            if (e.Button == MouseButtons.Left && tctotalpage.SelectedIndex != 0)
            {
                int x = e.X, y = e.Y;
                //计算关闭区域
                Rectangle mytab = this.tctotalpage.GetTabRect(this.tctotalpage.SelectedIndex);

                mytab.Offset(mytab.Width - (CloseSize + 3), 2);
                mytab.Width = CloseSize;
                mytab.Height = CloseSize;

                //如果Mouse在区域内就关闭选项卡
                bool isClose = x > mytab.X && x < mytab.Right && y > mytab.Y && y < mytab.Bottom;

                if (isClose)
                {
                    var id = tctotalpage.SelectedIndex;
                    this.tctotalpage.TabPages.Remove(this.tctotalpage.SelectedTab);
                    //当完成‘关闭’后,将当前页设置为'关闭'页的‘前一页’(注:若不这样设置,当关闭后会返回当前页为'首页')
                    tctotalpage.SelectedTab = tctotalpage.TabPages[id - 1];
                }

                //获取当前TabPage页的Text(重)后面的权限以及功能分支会用到   
                switch (tctotalpage.SelectedTab.Text)
                {
                    case "成本BOM报价单-创建":
                        GlobalClasscs.Fun.FunctionName = "B";
                        break;
                    case "新产品成本报价单-创建":
                        GlobalClasscs.Fun.FunctionName = "N";
                        break;
                }
            }
        }

        /// <summary>
        /// 根据指定的Tab Text条件打开对应的Tab Page(注:若相同的话,不会再次新建,而会将当前页设为该页)
        /// </summary>
        /// <param name="tabtext"></param>
        private void CreateTabPages(string tabtext)
        {
            var newpage = new TabPage { Text = $"{tabtext}" };

            if (CheckModiFrm(tabtext))
            {
                switch (tabtext)
                {
                    case "成本BOM报价单-创建":
                    case "新产品成本报价单-创建":
                        var createFrm = new CreateFrm
                        {
                            TopLevel = false,
                            BackColor = Color.White,
                            Dock = DockStyle.Fill,
                            FormBorderStyle = FormBorderStyle.None
                        };
                        GlobalClasscs.Fun.FunctionName = tabtext == "成本BOM报价单-创建" ? "B" : "N";
                        createFrm.Bomdt = _bomdt;
                        createFrm.Pricelistdt = _pricelistdt;
                        createFrm.PurchaseInstockdt = _instockdt;
                        createFrm.Devgroupdt = _devgroupdt;
                        createFrm.Show();                                      
                        newpage.Controls.Add(createFrm);    
                        break;

                    case "成本BOM报价单-查询":
                        var searchFrm = new SearchFrm
                        {
                            TopLevel = false,
                            BackColor = Color.White,
                            Dock = DockStyle.Fill,
                            FormBorderStyle = FormBorderStyle.None
                        };
                        searchFrm.Pricelistdt = _pricelistdt;
                        searchFrm.PurchaseInstockdt = _instockdt;
                        searchFrm.Devgroupdt = _devgroupdt;
                        searchFrm.Show();                    
                        newpage.Controls.Add(searchFrm); 
                        break;
                }
                tctotalpage.TabPages.Add(newpage);                      //将新创建的Tab Page添加至TabControl控件内
                tctotalpage.SelectedIndex = tctotalpage.TabCount - 1;  //设置显示新增的页为当前页
            }
            //若已在TabControl内存在,即将当前页设置为该页
            else
            {
                tctotalpage.SelectedTab = SearchTabPage(tabtext);
            }
        }

        /// <summary>
        /// 作用:遍历要打开的选项卡是否已存在TabControl内,若存在返回false;反之返回:true
        /// </summary>
        /// <param name="tabtext">要添加的TabPage名称</param>
        /// <returns></returns>
        private bool CheckModiFrm(string tabtext)
        {
            var result = true;
            foreach (Control cons in tctotalpage.Controls)
            {
                var tab = (TabPage) cons;
                if (tab.Text == tabtext)
                {
                    result = false;
                    break;
                }
            }
            return result;
        }

        /// <summary>
        /// 将当前页设置为已存在的页
        /// </summary>
        /// <returns></returns>
        private TabPage SearchTabPage(string tabtext)
        {
            var tab=new TabPage();
            foreach (Control cons in tctotalpage.Controls)
            {
                var tabpage = (TabPage) cons;
                if (tabpage.Text == tabtext)
                {
                    tab = tabpage;
                    break;
                }
            }
            return tab;
        }

        /// <summary>
        /// 关闭窗体
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Main_FormClosing(object sender, FormClosingEventArgs e)
        {
            var clickMessage = $"是否退出?";
            if (e.CloseReason != CloseReason.ApplicationExitCall)
            {
                var result = MessageBox.Show(clickMessage, $"提示", MessageBoxButtons.YesNo, MessageBoxIcon.Information);
                //当点击"OK"按钮时执行以下操作
                if (result == DialogResult.Yes)
                {
                    //当退出时,清空用户权限useid相关占用信息
                    UpUseridValue(1);
                    //允许窗体关闭
                    e.Cancel = false;
                }
                else
                {
                    //将Cancel属性设置为 true 可以"阻止"窗体关闭
                    e.Cancel = true;
                }
            }
        }


        /// <summary>
        /// 首页按钮(GridView页面跳转时使用)
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BnMoveFirstItem_Click(object sender, EventArgs e)
        {
            try
            {
                //1)将当前页变量PageCurrent=1; 2)并将“首页” 及 “上一页”按钮设置为不可用 将“下一页” “末页”按设置为可用
                _pageCurrent = 1;
                bnMoveFirstItem.Enabled = false;
                bnMovePreviousItem.Enabled = false;

                bnMoveNextItem.Enabled = true;
                bnMoveLastItem.Enabled = true;
                GridViewPageChange();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        /// <summary>
        /// 上一页(GridView页面跳转时使用)
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BnMovePreviousItem_Click(object sender, EventArgs e)
        {
            try
            {
                //1)将PageCurrent自减 2)将“下一页” “末页”按钮设置为可用
                _pageCurrent--;
                bnMoveNextItem.Enabled = true;
                bnMoveLastItem.Enabled = true;
                //判断若PageCurrent=1的话,就将“首页” “上一页”按钮设置为不可用
                if (_pageCurrent == 1)
                {
                    bnMoveFirstItem.Enabled = false;
                    bnMovePreviousItem.Enabled = false;
                }
                GridViewPageChange();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        /// <summary>
        /// 下一页按钮(GridView页面跳转时使用)
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BnMoveNextItem_Click(object sender, EventArgs e)
        {
            try
            {
                //1)将PageCurrent自增 2)将“首页” “上一页”按钮设置为可用
                _pageCurrent++;
                bnMoveFirstItem.Enabled = true;
                bnMovePreviousItem.Enabled = true;
                //判断若PageCurrent与“总页数”一致的话,就将“下一页” “末页”按钮设置为不可用
                if (_pageCurrent == _totalpagecount)
                {
                    bnMoveNextItem.Enabled = false;
                    bnMoveLastItem.Enabled = false;
                }
                GridViewPageChange();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        /// <summary>
        /// 末页按钮(GridView页面跳转使用)
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BnMoveLastItem_Click(object sender, EventArgs e)
        {
            try
            {
                //1)将“总页数”赋值给PageCurrent 2)将“下一页” “末页”按钮设置为不可用 并将 “上一页” “首页”按钮设置为可用
                _pageCurrent = _totalpagecount;
                bnMoveNextItem.Enabled = false;
                bnMoveLastItem.Enabled = false;

                bnMovePreviousItem.Enabled = true;
                bnMoveFirstItem.Enabled = true;

                GridViewPageChange();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        /// <summary>
        /// 跳转页文本框
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BnPositionItem_Leave(object sender, EventArgs e)
        {
            try
            {
                //判断所输入的跳转数必须为整数
                if (!Regex.IsMatch(bnPositionItem.Text, @"^-?[1-9]\d*$|^0$")) throw new Exception("请输入整数再继续");
                //判断所输入的跳转数不能大于总页数
                if (Convert.ToInt32(bnPositionItem.Text) > _totalpagecount) throw new Exception("所输入的页数不能超出总页数,请修改后继续");
                //判断若所填跳转数为0时跳出异常
                if (Convert.ToInt32(bnPositionItem.Text) == 0) throw new Exception("请输入大于0的整数再继续");

                //将所填的跳转页赋值至“当前页”变量内
                _pageCurrent = Convert.ToInt32(bnPositionItem.Text);
                //根据所输入的页数动态控制四个方向键是否可用
                //若为第1页，就将“首页” “上一页”按钮设置为不可用 将“下一页” “末页”设置为可用
                if (_pageCurrent == 1)
                {
                    bnMoveFirstItem.Enabled = false;
                    bnMovePreviousItem.Enabled = false;

                    bnMoveNextItem.Enabled = true;
                    bnMoveLastItem.Enabled = true;
                }
                //若为末页,就将"下一页" “末页”按钮设置为不可用 将“上一页” “首页”设置为可用
                else if (_pageCurrent == _totalpagecount)
                {
                    bnMoveNextItem.Enabled = false;
                    bnMoveLastItem.Enabled = false;
                    bnMovePreviousItem.Enabled = true;
                    bnMoveFirstItem.Enabled = true;
                }
                //否则四个按钮都可用
                else
                {
                    bnMoveFirstItem.Enabled = true;
                    bnMovePreviousItem.Enabled = true;
                    bnMoveNextItem.Enabled = true;
                    bnMoveLastItem.Enabled = true;
                }
                GridViewPageChange();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, $"错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
                bnPositionItem.Text = Convert.ToString(_pageCurrent);
            }
        }

        /// <summary>
        /// 每页显示行数 下拉框关闭时执行
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Tmshowrows_DropDownClosed(object sender, EventArgs e)
        {
            try
            {
                //每次选择新的“每页显示行数”，都要 1)将_pageChange标记设为true(即执行初始化方法) 2)将“当前页”初始化为1
                _pageChange = true;
                _pageCurrent = 1;
                //将“上一页” “首页”设置为不可用
                bnMovePreviousItem.Enabled = false;
                bnMoveFirstItem.Enabled = false;
                GridViewPageChange();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, $"错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        /// <summary>
        /// GridView分页功能
        /// </summary>
        private void GridViewPageChange()
        {
            try
            {
                //获取查询的总行数
                var dtltotalrows = _dtl.Rows.Count;
                //获取“每页显示行数”所选择的行数
                var pageCount = Convert.ToInt32(tmshowrows.SelectedItem);
                //计算出总页数
                _totalpagecount = dtltotalrows % pageCount == 0 ? dtltotalrows / pageCount : dtltotalrows / pageCount + 1;
                //赋值"总页数"项
                bnCountItem.Text = $"/ {_totalpagecount} 页";

                //初始化BindingNavigator控件内的各子控件 及 对应初始化信息
                if (_pageChange)
                {
                    bnPositionItem.Text = Convert.ToString(1);                      //初始化填充跳转页为1
                    tmshowrows.Enabled = true;                                      //每页显示行数（下拉框）  

                    //初始化时判断;若“总页数”=1，四个按钮不可用；若>1,将“下一页” “末页”按钮及跳转页文本框可用
                    if (_totalpagecount == 1)
                    {
                        bnMoveFirstItem.Enabled = false;                            //'首页'按钮
                        bnMovePreviousItem.Enabled = false;                         //'上一页'按钮
                        bnMoveNextItem.Enabled = false;                             //'下一页'按钮
                        bnMoveLastItem.Enabled = false;                             //'末页'按钮
                        bnPositionItem.Enabled = false;                             //跳转页文本框
                    }
                    else
                    {
                        bnMoveNextItem.Enabled = true;
                        bnMoveLastItem.Enabled = true;
                        bnPositionItem.Enabled = true;                             //跳转页文本框
                    }
                    _pageChange = false;
                }

                //显示_dtl的查询总行数
                tstotalrow.Text = $"共 {_dtl.Rows.Count} 行";

                //根据“当前页” 及 “固定行数” 计算出新的行数记录并进行赋值
                //计算进行循环的起始行
                var startrow = (_pageCurrent - 1) * pageCount;
                //计算进行循环的结束行
                var endrow = _pageCurrent == _totalpagecount ? dtltotalrows : _pageCurrent * pageCount;
                //复制 查询的DT的列信息（不包括行）至临时表内
                var tempdt = _dtl.Clone();
                //循环将所需的_dtl的行记录复制至临时表内
                for (var i = startrow; i < endrow; i++)
                {
                    tempdt.ImportRow(_dtl.Rows[i]);
                }

                //最后将刷新的DT重新赋值给GridView
                gvdtl.DataSource = tempdt;
                //将“当前页”赋值给"跳转页"文本框内
                bnPositionItem.Text = Convert.ToString(_pageCurrent);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, $"错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        /// <summary>
        /// 控制GridView单元格显示方式
        /// </summary>
        private void ControlGridViewisShow()
        {
            //注:当没有值时,若还设置某一行Row不显示的话,就会出现异常
            if (gvdtl?.Rows.Count >= 0)
                gvdtl.Columns[0].Visible = false;
        }

        /// <summary>
        /// 初始化查询下拉列表
        /// </summary>
        private void OnShowSelectTypeList()
        {
            var dt = new DataTable();

            //创建表头
            for (var i = 0; i < 2; i++)
            {
                var dc = new DataColumn();
                switch (i)
                {
                    case 0:
                        dc.ColumnName = "Id";
                        break;
                    case 1:
                        dc.ColumnName = "Name";
                        break;
                }
                dt.Columns.Add(dc);
            }

            //创建行内容
            for (var j = 0; j < 6; j++)
            {
                var dr = dt.NewRow();

                switch (j)
                {
                    case 0:
                        dr[0] = "0";
                        dr[1] = "OA流水号";
                        break;
                    case 1:
                        dr[0] = "1";
                        dr[1] = "产品名称";
                        break;
                    case 2:
                        dr[0] = "2";
                        dr[1] = "创建日期";
                        break;
                    case 3:
                        dr[0] = "3";
                        dr[1] = "审核日期";
                        break;
                    case 4:
                        dr[0] = "4";
                        dr[1] = "单据状态";
                        break;
                    case 5:
                        dr[0] = "5";
                        dr[1] = "研发类别";
                        break;
                }
                dt.Rows.Add(dr);
            }

            comselectvalue.DataSource = dt;
            comselectvalue.DisplayMember = "Name"; //设置显示值
            comselectvalue.ValueMember = "Id";    //设置默认值内码
        }

        /// <summary>
        /// 初始化单据状态下拉列表
        /// </summary>
        private void OnShowStatusList()
        {
            var dt = new DataTable();

            //若comvaluedt内有值即先清空,后再插入
            var comvaluedt = (DataTable)comstatus.DataSource;
     
            if (comvaluedt?.Rows.Count > 0)
            {
                 comvaluedt.Rows.Clear();
                 comvaluedt.Columns.Clear();
            }

            #region 创建表头 表体

            //创建表头
            for (var i = 0; i < 2; i++)
            {
                var dc = new DataColumn();
                switch (i)
                {
                    case 0:
                        dc.ColumnName = "Id";
                        break;
                    case 1:
                        dc.ColumnName = "Name";
                        break;
                }
                dt.Columns.Add(dc);
            }

            //创建行内容
            for (var j = 0; j < 2; j++)
            {
                var dr = dt.NewRow();

                switch (j)
                {
                    case 0:
                        dr[0] = "0";
                        dr[1] = "已审核";
                        break;
                    case 1:
                        dr[0] = "1";
                        dr[1] = "反审核";
                        break;
                }
                dt.Rows.Add(dr);
            }
            #endregion

            comstatus.DataSource = dt.Copy();
            comstatus.DisplayMember = "Name"; //设置显示值
            comstatus.ValueMember = "Id";    //设置默认值内码
        }

        /// <summary>
        /// 初始化‘研发类别’下拉列表
        /// </summary>
        private void OnShowDevGroupList()
        {
            //若comvaluedt内有值即先清空,后再插入
            var comvaluedt = (DataTable)comstatus.DataSource;

            if (comvaluedt?.Rows.Count > 0)
            {
                comvaluedt.Rows.Clear();
                comvaluedt.Columns.Clear();
            }

            comstatus.DataSource = _devgroupdt.Copy();
            comstatus.DisplayMember = "Name";  //设置显示值
            comstatus.ValueMember = "Id";      //设置默认值内码
        }

        /// <summary>
        ///子线程使用(重:用于监视功能调用情况,当完成时进行关闭LoadForm)
        /// </summary>
        private void Start()
        {
            task.StartTask();

            //当完成后将Form2子窗体关闭
            this.Invoke((ThreadStart)(() =>
            {
                load.Close();
            }));
        }

        /// <summary>
        /// 根据FId查询占用记录
        /// </summary>
        /// <param name="fid"></param>
        /// <returns></returns>
        private DataTable ShowUsedtl(int fid)
        {
            task.TaskId = "0.6";
            task.Fid = fid;
            task.StartTask();
            return task.ResultTable;
        }

        /// <summary>
        /// 更新或清空用户占用记录(0:更新用户占用 1:清空)
        /// </summary>
        /// <param name="typeid">0:更新用户占用 1:清空</param>
        private void UpUseridValue(int typeid)
        {
            task.TaskId = "4.1";
            task.Fid = GlobalClasscs.User.UserId;
            task.Type = typeid;
            task.StartTask();
        }

        /// <summary>
        /// 初始化BOM明细DT
        /// </summary>
        private void OnInitializeBomdt()
        {
            task.TaskId = "0.1";
            task.StartTask();
            _bomdt = task.Resultbomdt;
            GlobalClasscs.Bd.Bomdt = _bomdt;
        }

        /// <summary>
        /// 初始化采购入库单DT
        /// </summary>
        private void OnInitializeInstockdt()
        {
            task.TaskId = "5.3";
            task.StartTask();
            _instockdt = task.ResultTable;
        }

        /// <summary>
        /// 初始化采购价目表DT
        /// </summary>
        private void OnInitializePriceListdt()
        {
            task.TaskId = "5.4";
            task.StartTask();
            _pricelistdt = task.ResultTable;
        }

        /// <summary>
        /// 初始化销售价目表DT
        /// </summary>
        private void OnInitializeSalesPricedt()
        {
            task.TaskId = "5.6";
            task.StartTask();
            _salesPricedt = task.ResultTable;
        }

        /// <summary>
        /// 初始化采购入库单DT-(毛利润报表专用)
        /// </summary>
        private void OnInitializePurchaseInstockDt()
        {
            task.TaskId = "5.7";
            task.StartTask();
            _purchaseInstockDt = task.ResultTable;
        }

        /// <summary>
        /// 初始化人工制造费用DT
        /// </summary>
        private void OnInitializeRenCostDt()
        {
            task.TaskId = "5.8";
            task.StartTask();
            _renCostDt = task.ResultTable;
        }

        /// <summary>
        /// 初始化研发类别DT
        /// </summary>
        private void OnInitializeDevGroup()
        {
            task.TaskId = "0.0.0.3";
            task.StartTask();
            _devgroupdt = task.ResultTable;
        }

        /// <summary>
        /// 权限控制
        /// </summary>
        private void PrivilegeControl()
        {
            //若用户没有可读权限,即‘成本BOM报价单’功能有不显示
            if (!GlobalClasscs.User.Readid)
            {
                btnCreate.Visible = false;
            }
        }

        /// <summary>
        /// 获取报表所选取的FMATERIALID列表,如:11234,12345
        /// </summary>
        /// <param name="sourcedt"></param>
        /// <param name="rowid"></param>
        /// <returns></returns>
        private string GetSearchList(DataTable sourcedt,int rowid)
        {
            //FMATERIALID列表结果
            var result = string.Empty;
            //中转比较变量
            var temp = string.Empty;

            foreach (DataRow rows in sourcedt.Rows)
            {
                //第一行(初始化)时,将第一行的相关值赋给对应的变量内
                if (temp == "")
                {
                    temp = Convert.ToString(rows[rowid]);
                    result= "'" + Convert.ToString(rows[rowid]) + "'";
                }
                //从第二行开始判断是否一致
                else if (temp != Convert.ToString(rows[rowid]))
                {
                    temp = Convert.ToString(rows[rowid]);
                    result+= ',' + "'" + Convert.ToString(rows[rowid]) + "'";
                }
            }
            return result;
        }

        /// <summary>
        /// 单据删除
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Tmorderdel_Click(object sender, EventArgs e)
        {
            try
            {
                if (gvdtl.SelectedRows.Count == 0) throw new Exception("没有选中行,请选择后再继续");
                //获取删除临时表
                var tempdt = dbList.DelOrderDt();
                //循将将所选择的行插入至tempdt内
                foreach (DataGridViewRow rows in gvdtl.SelectedRows)
                {
                    if (Convert.ToString(rows.Cells[4].Value) == "已审核") throw new Exception("不能执行删除,原因:所选的单据为已审核状态.");
                    var newrow = tempdt.NewRow();
                    newrow[0] = rows.Cells[0].Value; //fid
                    newrow[1] = rows.Cells[1].Value; //orderno
                    tempdt.Rows.Add(newrow);
                }
                //分别获取不重复的FID及ORDERNO
                var fidlist = GetSearchList(tempdt, 0);
                var ordernolist = GetSearchList(tempdt, 1);
                //显示及决定是否删除
                var clickMessage =  $"您所选择需要删除的单据为:\n {ordernolist} \n 是否继续? \n 注:删除后不能恢复, \n 请谨慎处理.";

                if (MessageBox.Show(clickMessage, $"提示", MessageBoxButtons.YesNo, MessageBoxIcon.Information) == DialogResult.Yes)
                {
                    task.TaskId = "6.2";
                    task.Fidlist = fidlist;
                    task.StartTask();
                    if(!task.ResultMark) throw new Exception("删除不成功,请联系管理员.");
                    else
                    {
                        MessageBox.Show($"删除成功", $"提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        //刷新Gridview
                        OnSearch();
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, $"错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        #region 暂存功能模块

        /// <summary>
        /// 暂存功能-查询使用
        /// </summary>
        private void OnSearchTempDt()
        {
            task.TaskId = "0.9.5";

            new Thread(Start).Start();
            load.StartPosition = FormStartPosition.CenterScreen;
            load.ShowDialog();

            if (task.ResultTable.Rows.Count > 0)
            {
                _tempdtl = task.ResultTable;
                panel10.Visible = true;
                //初始化下拉框所选择的默认值
                //tmshowrows.SelectedItem = "10";
                tmshowrowstemp.SelectedItem = Convert.ToInt32(tmshowrowstemp.SelectedItem) == 0
                    ? (object)"10"
                    : Convert.ToInt32(tmshowrowstemp.SelectedItem);
                //定义初始化标记(设置为不初始化)
                _pageChangeTemp = _pageCurrentTemp <= 1;
                //GridView分页
                GridViewPageChangeTemp();
            }
            //注:当为空记录时,不显示跳转页;只需将临时表赋值至GridView内
            else
            {
                gvtempdtl.DataSource = task.ResultTable;
                panel10.Visible = false;
            }
            //控制GridView单元格显示方式
            ControlGridViewisShowTemp();
        }

        /// <summary>
        /// 查阅明细-(注:暂存单据只能操作一次,当完成后就会将其删除)
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Tmshowtempdetail_Click(object sender, EventArgs e)
        {
            try
            {
                if(gvtempdtl.SelectedRows.Count==0)throw new Exception("没有明细记录,不能继续操作");
                //根据所选择的行获取其fid值
                var fid = Convert.ToInt32(gvtempdtl.Rows[gvtempdtl.CurrentCell.RowIndex].Cells[0].Value);

                task.TaskId = "0.9.6";
                task.Fid = fid;
                GlobalClasscs.Fun.RfFunctionName = "RF";   //定义为'暂存'单据

                new Thread(Start).Start();
                load.StartPosition = FormStartPosition.CenterScreen;
                load.ShowDialog();

                //弹出对应窗体相关设置
                //初始化信息
                dtlFrm.FunState = "R";
                dtlFrm.OnInitialize(task.ResultTable, _pricelistdt, _instockdt, _devgroupdt);
                dtlFrm.StartPosition = FormStartPosition.CenterParent;
                dtlFrm.ShowDialog();

                //刷新GridView
                OnSearchTempDt();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, $"错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        /// <summary>
        /// 刷新
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Btnrefresh_Click(object sender, EventArgs e)
        {
            try
            {
                OnSearchTempDt();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, $"错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        /// <summary>
        /// 首页按钮(GridView页面跳转时使用)
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BnMoveFirstItemtemp_Click(object sender, EventArgs e)
        {
            try
            {
                //1)将当前页变量PageCurrent=1; 2)并将“首页” 及 “上一页”按钮设置为不可用 将“下一页” “末页”按设置为可用
                _pageCurrentTemp = 1;
                bnMoveFirstItemtemp.Enabled = false;
                bnMovePreviousItemtemp.Enabled = false;

                bnMoveNextItemtemp.Enabled = true;
                bnMoveLastItemtemp.Enabled = true;
                GridViewPageChangeTemp();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, $"错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        /// <summary>
        /// 上一页(GridView页面跳转时使用)
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BnMovePreviousItemtemp_Click(object sender, EventArgs e)
        {
            try
            {
                //1)将PageCurrent自减 2)将“下一页” “末页”按钮设置为可用
                _pageCurrentTemp--;
                bnMoveNextItemtemp.Enabled = true;
                bnMoveLastItemtemp.Enabled = true;
                //判断若PageCurrent=1的话,就将“首页” “上一页”按钮设置为不可用
                if (_pageCurrentTemp == 1)
                {
                    bnMoveFirstItemtemp.Enabled = false;
                    bnMovePreviousItemtemp.Enabled = false;
                }
                GridViewPageChangeTemp();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, $"错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        /// <summary>
        /// 下一页按钮(GridView页面跳转时使用)
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BnMoveNextItemtemp_Click(object sender, EventArgs e)
        {
            try
            {
                //1)将PageCurrent自增 2)将“首页” “上一页”按钮设置为可用
                _pageCurrentTemp++;
                bnMoveFirstItemtemp.Enabled = true;
                bnMovePreviousItemtemp.Enabled = true;
                //判断若PageCurrent与“总页数”一致的话,就将“下一页” “末页”按钮设置为不可用
                if (_pageCurrentTemp == _totalpagecountTemp)
                {
                    bnMoveNextItemtemp.Enabled = false;
                    bnMoveLastItemtemp.Enabled = false;
                }
                GridViewPageChangeTemp();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, $"错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        /// <summary>
        /// 末页按钮(GridView页面跳转使用)
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BnMoveLastItemtemp_Click(object sender, EventArgs e)
        {
            try
            {
                //1)将“总页数”赋值给PageCurrent 2)将“下一页” “末页”按钮设置为不可用 并将 “上一页” “首页”按钮设置为可用
                _pageCurrentTemp = _totalpagecountTemp;
                bnMoveNextItemtemp.Enabled = false;
                bnMoveLastItemtemp.Enabled = false;

                bnMovePreviousItemtemp.Enabled = true;
                bnMoveFirstItemtemp.Enabled = true;

                GridViewPageChangeTemp();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, $"错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        /// <summary>
        /// 跳转页文本框
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BnPositionItemtemp_Leave(object sender, EventArgs e)
        {
            try
            {
                //判断所输入的跳转数必须为整数
                if (!Regex.IsMatch(bnPositionItemtemp.Text, @"^-?[1-9]\d*$|^0$")) throw new Exception("请输入整数再继续");
                //判断所输入的跳转数不能大于总页数
                if (Convert.ToInt32(bnPositionItemtemp.Text) > _totalpagecountTemp) throw new Exception("所输入的页数不能超出总页数,请修改后继续");
                //判断若所填跳转数为0时跳出异常
                if (Convert.ToInt32(bnPositionItemtemp.Text) == 0) throw new Exception("请输入大于0的整数再继续");

                //将所填的跳转页赋值至“当前页”变量内
                _pageCurrentTemp = Convert.ToInt32(bnPositionItemtemp.Text);
                //根据所输入的页数动态控制四个方向键是否可用
                //若为第1页，就将“首页” “上一页”按钮设置为不可用 将“下一页” “末页”设置为可用
                if (_pageCurrentTemp == 1)
                {
                    bnMoveFirstItemtemp.Enabled = false;
                    bnMovePreviousItemtemp.Enabled = false;

                    bnMoveNextItemtemp.Enabled = true;
                    bnMoveLastItemtemp.Enabled = true;
                }
                //若为末页,就将"下一页" “末页”按钮设置为不可用 将“上一页” “首页”设置为可用
                else if (_pageCurrentTemp == _totalpagecountTemp)
                {
                    bnMoveNextItemtemp.Enabled = false;
                    bnMoveLastItemtemp.Enabled = false;

                    bnMovePreviousItemtemp.Enabled = true;
                    bnMoveFirstItemtemp.Enabled = true;
                }
                //否则四个按钮都可用
                else
                {
                    bnMoveFirstItemtemp.Enabled = true;
                    bnMovePreviousItemtemp.Enabled = true;
                    bnMoveNextItemtemp.Enabled = true;
                    bnMoveLastItemtemp.Enabled = true;
                }
                GridViewPageChangeTemp();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, $"错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
                bnPositionItemtemp.Text = Convert.ToString(_pageCurrentTemp);
            }
        }

        /// <summary>
        /// 每页显示行数 下拉框关闭时执行
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Tmshowrowstemp_DropDownClosed(object sender, EventArgs e)
        {
            try
            {
                //每次选择新的“每页显示行数”，都要 1)将_pageChange标记设为true(即执行初始化方法) 2)将“当前页”初始化为1
                _pageChangeTemp = true;
                _pageCurrentTemp = 1;
                //将“上一页” “首页”设置为不可用
                bnMovePreviousItem.Enabled = false;
                bnMoveFirstItem.Enabled = false;
                GridViewPageChangeTemp();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, $"错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        /// <summary>
        /// GridView分页功能(暂存页使用)
        /// </summary>
        private void GridViewPageChangeTemp()
        {
            try
            {
                //获取查询的总行数
                var dtltotalrows = _tempdtl.Rows.Count;
                //获取“每页显示行数”所选择的行数
                var pageCount = Convert.ToInt32(tmshowrowstemp.SelectedItem);
                //计算出总页数
                _totalpagecountTemp = dtltotalrows % pageCount == 0 ? dtltotalrows / pageCount : dtltotalrows / pageCount + 1;
                //赋值"总页数"项
                bnCountItemtemp.Text = $"/ {_totalpagecountTemp} 页";

                //初始化BindingNavigator控件内的各子控件 及 对应初始化信息
                if (_pageChangeTemp)
                {
                    bnPositionItemtemp.Text = Convert.ToString(1);                      //初始化填充跳转页为1
                    tmshowrowstemp.Enabled = true;                                      //每页显示行数（下拉框）  

                    //初始化时判断;若“总页数”=1，四个按钮不可用；若>1,“下一页” “末页”按钮可用
                    if (_totalpagecountTemp == 1)
                    {
                        bnMoveFirstItemtemp.Enabled = false;                            //'首页'按钮
                        bnMovePreviousItemtemp.Enabled = false;                         //'上一页'按钮
                        bnMoveNextItemtemp.Enabled = false;                             //'下一页'按钮
                        bnMoveLastItemtemp.Enabled = false;                             //'末页'按钮
                        bnPositionItemtemp.Enabled = false;                             //跳转页文本框
                    }
                    else
                    {
                        bnMoveNextItemtemp.Enabled = true;
                        bnMoveLastItemtemp.Enabled = true;
                        bnPositionItemtemp.Enabled = true;                             //跳转页文本框
                    }
                    _pageChangeTemp = false;
                }

                //显示_dtl的查询总行数
                tstotalrowtemp.Text = $"共 {_tempdtl.Rows.Count} 行";

                //根据“当前页” 及 “固定行数” 计算出新的行数记录并进行赋值
                //计算进行循环的起始行
                var startrow = (_pageCurrentTemp - 1) * pageCount;
                //计算进行循环的结束行
                var endrow = _pageCurrentTemp == _totalpagecountTemp ? dtltotalrows : _pageCurrentTemp * pageCount;
                //复制 查询的DT的列信息（不包括行）至临时表内
                var tempdt = _tempdtl.Clone();
                //循环将所需的_dtl的行记录复制至临时表内
                for (var i = startrow; i < endrow; i++)
                {
                    tempdt.ImportRow(_tempdtl.Rows[i]);
                }

                //最后将刷新的DT重新赋值给GridView
                gvtempdtl.DataSource = tempdt;
                //将“当前页”赋值给"跳转页"文本框内
                bnPositionItemtemp.Text = Convert.ToString(_pageCurrentTemp);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, $"错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        /// <summary>
        /// 控制GridView单元格显示方式
        /// </summary>
        private void ControlGridViewisShowTemp()
        {
            //注:当没有值时,若还设置某一行Row不显示的话,就会出现异常
            if (gvtempdtl?.Rows.Count >= 0)
                gvtempdtl.Columns[0].Visible = false;
        }

        /// <summary>
        /// 暂存单据删除
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Tmtempdel_Click(object sender, EventArgs e)
        {
            try
            {
                if (gvtempdtl.SelectedRows.Count == 0) throw new Exception("没有选中行,请选择后再继续");
                //获取删除临时表
                var tempdt = dbList.DelOrderDt();
                //循将将所选择的行插入至tempdt内
                foreach (DataGridViewRow rows in gvtempdtl.SelectedRows)
                {
                    var newrow = tempdt.NewRow();
                    newrow[0] = rows.Cells[0].Value; //fid
                    tempdt.Rows.Add(newrow);
                }
                //分别获取不重复的FID及ORDERNO
                var fidlist = GetSearchList(tempdt, 0);
                //显示及决定是否删除
                var clickMessage = $"是否继续进行删除操作? \n 注:删除后不能恢复, \n 请谨慎处理.";

                if (MessageBox.Show(clickMessage, $"提示", MessageBoxButtons.YesNo, MessageBoxIcon.Information) ==
                    DialogResult.Yes)
                {
                    task.TaskId = "6.3";
                    task.Fidlist = fidlist;
                    task.StartTask();
                    if (!task.ResultMark) throw new Exception("删除不成功,请联系管理员.");
                    else
                    {
                        MessageBox.Show($"删除成功", $"提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        //最后执行刷新
                        OnSearchTempDt();
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, $"错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        #endregion

    }
}
