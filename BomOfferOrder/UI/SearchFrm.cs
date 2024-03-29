﻿using System;
using System.Data;
using System.Text.RegularExpressions;
using System.Threading;
using System.Windows.Forms;
using BomOfferOrder.Task;
using ICSharpCode.SharpZipLib.Zip;

namespace BomOfferOrder.UI
{
    public partial class SearchFrm : Form
    {
        TaskLogic task=new TaskLogic();
        Load load=new Load();
        DtlFrm dtlFrm=new DtlFrm();

        #region 变量参数

        //保存查询出来的GridView记录
        private DataTable _dtl;
        //保存查询出来的角色权限记录
        //private DataTable _userdt;
        //保存采购价目表DT
        private DataTable _pricelistdt;
        //保存采购入库单DT
        private DataTable _purchaseInstockdt;
        //保存‘研发类别‘基础资料DT
        private DataTable _devgroupdt;

        //记录当前页数(GridView页面跳转使用)
        private int _pageCurrent = 1;
        //记录计算出来的总页数(GridView页面跳转使用)
        private int _totalpagecount;
        //记录初始化标记(GridView页面跳转 初始化时使用)
        private bool _pageChange;
        #endregion

        #region Set
        /// <summary>
        /// 保存采购价目表DT
        /// </summary>
        public DataTable Pricelistdt { set { _pricelistdt = value; } }
        /// <summary>
        /// 保存采购入库单DT
        /// </summary>
        public DataTable PurchaseInstockdt { set { _purchaseInstockdt = value; } }
        /// <summary>
        /// 保存研发类别DT
        /// </summary>
        public DataTable Devgroupdt { set { _devgroupdt = value; } }
        #endregion

        public SearchFrm()
        {
            InitializeComponent();
            OnRegisterEvents();
            OnInitialize();
        }

        private void OnRegisterEvents()
        {
            btnsearch.Click += Btnsearch_Click;
            tmbackconfirm.Click += Tmbackconfirm_Click;
            tmshowdetail.Click += Tmshowdetail_Click;
            comselectvalue.SelectedIndexChanged += Comselectvalue_SelectedIndexChanged;

            bnMoveFirstItem.Click += BnMoveFirstItem_Click;
            bnMovePreviousItem.Click += BnMovePreviousItem_Click;
            bnMoveNextItem.Click += BnMoveNextItem_Click;
            bnMoveLastItem.Click += BnMoveLastItem_Click;
            bnPositionItem.Leave += BnPositionItem_Leave;
            tmshowrows.DropDownClosed += Tmshowrows_DropDownClosed;
            panel2.Visible = false;
        }

        /// <summary>
        /// 初始化相关记录
        /// </summary>
        private void OnInitialize()
        {
            //初始化下拉列表
            OnShowSelectTypeList();
        }


        /// <summary>
        /// 根据所选的选择条件刷新GridView
        /// </summary>
        private void OnSearch()
        {
            //获取下拉列表所选值
            var dvordertylelist = (DataRowView)comselectvalue.Items[comselectvalue.SelectedIndex];
            var typeId = Convert.ToInt32(dvordertylelist["Id"]);

            task.TaskId = "0.4";
            task.SearchId = typeId;

            switch (typeId)
            {
                case 0:   //OA流水号
                case 1:   //产品名称
                    task.SearchValue = txtvalue.Text;
                    break;
                case 2:  //创建日期
                case 3:  //审核日期
                    task.Dpstart = Convert.ToString(dpstart.Value.Date);
                    task.Dpend = Convert.ToString(dpend.Value.Date);
                    break;
                default: //单据状态  //研发类别
                    // var statuslist = (DataRowView)comselectvalue.Items[comstatus.SelectedIndex];
                    var statuslist = (DataRowView)comstatus.Items[comstatus.SelectedIndex];
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
                panel2.Visible = true;
                //初始化下拉框所选择的默认值
                //tmshowrows.SelectedItem = "10"; 
                tmshowrows.SelectedItem = Convert.ToInt32(tmshowrows.SelectedItem) == 0
                    ? (object)"10"
                    : Convert.ToInt32(tmshowrows.SelectedItem);
                //定义初始化标记
                _pageChange = _pageCurrent <= 1;
                //GridView分页
                GridViewPageChange();
            }
            //注:当为空记录时,不显示跳转页;只需将临时表赋值至GridView内
            else
            {
                gvdtl.DataSource = task.ResultTable;
                panel2.Visible = false;
            }
            //控制GridView单元格显示方式
            ControlGridViewisShow();
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

                
                switch (typeId)
                {
                    case 0:   //OA流水号
                    case 1:   //产品名称
                        txtvalue.Visible = true;
                        comstatus.Visible = false;

                        dpstart.Visible = false;
                        dpend.Visible = false;
                        label1.Visible = false;
                        break;
                    case 2:   //创建日期
                    case 3:   //审核日期
                        dpstart.Visible = true;
                        dpstart.Enabled = true;
                        dpend.Visible = true;
                        dpend.Enabled = true;
                        label1.Visible = true;
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
                        label1.Visible = false;
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
                        label1.Visible = false;
                        txtvalue.Visible = false;
                        //初始化研发类别下拉列表
                        OnShowDevGroupList();
                        break;
                        #region 其它项:单据状态 研发类别
                        //default:
                        //    comstatus.Visible = true;
                        //    //将日期 以及 文本框控件隐藏
                        //    dtpdt.Visible = false;
                        //    txtvalue.Visible = false;
                        //    //初始化单据状态下拉列表
                        //    OnShowStatusList();
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
        /// 查询 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Btnsearch_Click(object sender, EventArgs e)
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
                if (gvdtl.SelectedRows.Count == 0) throw new Exception("没有明细记录,不能继续操作");
                //获取OA流水号
                var oaorder = Convert.ToString(gvdtl.Rows[gvdtl.CurrentCell.RowIndex].Cells[1].Value);
                //根据所选择的行获取其fid值
                var fid = Convert.ToInt32(gvdtl.Rows[gvdtl.CurrentCell.RowIndex].Cells[0].Value);
                //根据Fid获取数据库useid 及 username值;并根据useid判断是否占用
                var usedt = ShowUsedtl(fid);
                if (Convert.ToInt32(usedt.Rows[0][0]) == 0)
                    throw new Exception($"所选单据'{oaorder}'不能进入, \n 原因:已被用户'{Convert.ToString(usedt.Rows[0][1])}'占用,需用户'{Convert.ToString(usedt.Rows[0][1])}'退出才能继续操作");

                task.TaskId = "0.5";
                task.Fid = fid;

                new Thread(Start).Start();
                load.StartPosition = FormStartPosition.CenterScreen;
                load.ShowDialog();

                //弹出对应窗体相关设置
                //初始化信息
                dtlFrm.FunState = "R";
                dtlFrm.OnInitialize(task.ResultTable, _pricelistdt,_purchaseInstockdt, _devgroupdt);
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
                if (orderstatus != "已审核") throw new Exception($"单据'{oaorder}'的单据状态不为已审核,不能进行反审核操作");

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

                    if (!task.ResultMark) throw new Exception("反审核操作出现异常,请联系管理员.");
                    else
                    {
                        MessageBox.Show($"反审核成功,请右键选择‘查询明细’进入", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
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
                    bnPositionItem.Text = Convert.ToString(1);                       //初始化填充跳转页为1
                    tmshowrows.Enabled = true;                                      //每页显示行数（下拉框）  

                    //初始化时判断;若“总页数”=1，四个按钮不可用；若>1,“下一页” “末页”按钮可用
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
                MessageBox.Show(ex.Message, "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        /// <summary>
        ///子线程使用(重:用于监视功能调用情况,当完成时进行关闭LoadForm)
        /// </summary>
        private void Start()
        {
            task.StartTask();

            //当完成后将Load子窗体关闭
            this.Invoke((ThreadStart)(() =>
            {
                load.Close();
            }));
        }

        /// <summary>
        /// 控制GridView单元格显示方式
        /// </summary>
        private void ControlGridViewisShow()
        {
            //注:当没有值时,若还设置某一行Row不显示的话,就会出现异常
            if (gvdtl?.RowCount >= 0)
                gvdtl.Columns[0].Visible = false;
        }

        /// <summary>
        /// 初始化查询下拉列表
        /// </summary>
        private void OnShowSelectTypeList()
        {
            var dt = new DataTable();

            //创建表头
            for (var i = 0; i < 5; i++)
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

    }
}
