﻿using System;
using System.Data;
using System.Globalization;
using System.Text.RegularExpressions;
using System.Threading;
using System.Windows.Forms;
using BomOfferOrder.DB;
using BomOfferOrder.Task;
using BomOfferOrder.UI.Admin.Basic;

namespace BomOfferOrder.UI.Admin
{
    public partial class PermissionFrm : Form
    {
        TaskLogic task=new TaskLogic();
        Load load=new Load();
        GetAccountFrm getAccount=new GetAccountFrm();
        //AccountDetailFrm accountDetail=new AccountDetailFrm();
        AccountPerFrm accountPer = new AccountPerFrm();
        BdUserGroupFrm bdUserGroup=new BdUserGroupFrm();
        DbList dbList=new DbList();

        #region 变量参数
        //获取K3用户信息
        private DataTable _k3UserDt;
        //保存初始化'研发类别'基础资料DT
        private DataTable _devgroupdt;

        //保存查询出来的GridView记录
        private DataTable _dtl;
        ////保存查询出来的角色权限记录
        //private DataTable _userdt;
        ////保存基础-用户组别DT
        //private DataTable _bdusergroupdt;

        //记录当前页数(GridView页面跳转使用)
        private int _pageCurrent = 1;
        //记录计算出来的总页数(GridView页面跳转使用)
        private int _totalpagecount;
        //记录初始化标记(GridView页面跳转 初始化时使用)
        private bool _pageChange;
        #endregion


        public PermissionFrm()
        {
            InitializeComponent();
            OnInitialize();
            OnRegisterEvents();
        }

        /// <summary>
        /// 初始化相关记录
        /// </summary>
        private void OnInitialize()
        {
            //初始化查询下拉列表
            OnShowSelectTypeList();
            //初始化获取K3用户信息(注:不包括已添加到T_AD_User表的用户)
            OnInitializeK3UserDt();
            //初始化基本表-‘研发类别’DT
            OnInitializeDevGroup();
        }

        private void OnRegisterEvents()
        {
            btnsearch.Click += Btnsearch_Click;
            comselectvalue.SelectedIndexChanged += Comselectvalue_SelectedIndexChanged;
            tmAddUse.Click += TmAddUse_Click;
            tmshowdetail.Click += Tmshowdetail_Click;
            tmbdusergroup.Click += Tmbdusergroup_Click;

            bnMoveFirstItem.Click += BnMoveFirstItem_Click;
            bnMovePreviousItem.Click += BnMovePreviousItem_Click;
            bnMoveNextItem.Click += BnMoveNextItem_Click;
            bnMoveLastItem.Click += BnMoveLastItem_Click;
            bnPositionItem.Leave += BnPositionItem_Leave;
            tmshowrows.DropDownClosed += Tmshowrows_DropDownClosed;
            panel2.Visible = false;
        }

        /// <summary>
        /// 查询相关值
        /// </summary>
        private void OnSearch()
        {
            //获取下拉列表所选值
            var dvordertylelist = (DataRowView)comselectvalue.Items[comselectvalue.SelectedIndex];
            var typeId = Convert.ToInt32(dvordertylelist["Id"]);

            task.TaskId = "0.8";
            task.SearchId = typeId;
            switch (typeId)
            {
                //用户名称 创建人
                case 0:
                case 1:
                    task.SearchValue = txtvalue.Text;
                    break;
                //创建日期
                case 2:
                    task.SearchValue = Convert.ToString(dtpdt.Value.Date, CultureInfo.InvariantCulture);
                    break;
                //启用状态
                default:
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
        /// 查询
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Btnsearch_Click(object sender, EventArgs e)
        {
            try
            {
                //执行查询
                OnSearch();
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

                switch (typeId)
                {
                    //用户名称 创建人
                    case 0:
                    case 1:
                        txtvalue.Text = "";
                        txtvalue.Visible = true;
                        comstatus.Visible = false;
                        dtpdt.Visible = false;
                        break;
                    //创建日期
                    case 2:
                        dtpdt.Value = DateTime.Today;
                        dtpdt.Visible = true;
                        dtpdt.Enabled = true;
                        //将单据状态 及 文本框控件隐藏
                        comstatus.Visible = false;
                        txtvalue.Visible = false;
                        break;
                    //启用状态
                    case 3:
                        comstatus.Visible = true;
                        //将日期 以及 文本框控件隐藏
                        dtpdt.Visible = false;
                        txtvalue.Visible = false;
                        //初始化单据状态下拉列表
                        OnShowStatusList();
                        break;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        /// <summary>
        /// 创建用户功能权限
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void TmAddUse_Click(object sender, EventArgs e)
        {
            try
            {
                //初始化K3用户表
                getAccount.OnInitialize(0,_k3UserDt,_devgroupdt);
                getAccount.StartPosition = FormStartPosition.CenterParent;
                getAccount.ShowDialog();
                //执行查询
                OnSearch();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, $"错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        /// <summary>
        /// 用户组别设置
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Tmbdusergroup_Click(object sender, EventArgs e)
        {
            try
            {
                bdUserGroup.OnInitialize(_k3UserDt);
                bdUserGroup.StartPosition=FormStartPosition.CenterParent;
                bdUserGroup.ShowDialog();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, $"错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        /// <summary>
        ///  查阅明细
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Tmshowdetail_Click(object sender, EventArgs e)
        {
            try
            {
                if (gvdtl.SelectedRows.Count == 0) throw new Exception("没有明细记录,不能继续操作");

                //获取用户组别表头DT
                GlobalClasscs.Ad.UserGroupDt = OnInitializeUserGroupDt();
                //获取用户组别表体DT
                GlobalClasscs.Ad.UserGroupDtlDt = OnInitializeUserGroupDtlDt();
                //获取关联用户表头DT
                GlobalClasscs.Ad.RelUserDt = OnInitializeRelUserDt();
                //获取关联用户表体DT(主要收集用户‘不关联’的记录)
                GlobalClasscs.Ad.RelUserDtlDt = OnInitializeRelUserDtlDt();
                //获取已关联的‘研发类别’DT
                GlobalClasscs.Ad.DevGroupDt = OnInitializeDevGroupDt();

                //初始化信息并读取
                accountPer.OnInitialize("R", null, null, null, OnGetSelectRecordTemp(), _devgroupdt);
                accountPer.StartPosition = FormStartPosition.CenterParent;
                accountPer.ShowDialog();
                //刷新
                OnSearch();
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
                MessageBox.Show(ex.Message, "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
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
                GridViewPageChange();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
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
                MessageBox.Show(ex.Message, $"错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
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
            for (var j = 0; j < 4; j++)
            {
                var dr = dt.NewRow();

                switch (j)
                {
                    case 0:
                        dr[0] = "0";
                        dr[1] = "用户名称";
                        break;
                    case 1:
                        dr[0] = "1";
                        dr[1] = "创建人";
                        break;
                    case 2:
                        dr[0] = "2";
                        dr[1] = "创建日期";
                        break;
                    case 3:
                        dr[0] = "3";
                        dr[1] = "启用状态";
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
                        dr[1] = "已启用";
                        break;
                    case 1:
                        dr[0] = "1";
                        dr[1] = "未启用";
                        break;
                }
                dt.Rows.Add(dr);
            }

            comstatus.DataSource = dt;
            comstatus.DisplayMember = "Name"; //设置显示值
            comstatus.ValueMember = "Id";    //设置默认值内码
        }

        /// <summary>
        /// 初始化获取K3用户信息
        /// </summary>
        private void OnInitializeK3UserDt()
        {
            task.TaskId = "0.9";
            task.StartTask();
            _k3UserDt = task.ResultTable;
        }

        /// <summary>
        /// 初始化基本表-‘研发类别’DT
        /// </summary>
        private void OnInitializeDevGroup()
        {
            task.TaskId = "0.0.0.3";
            task.StartTask();
            _devgroupdt = task.ResultTable;
        }

        /// <summary>
        /// 获取用户组别表头DT
        /// </summary>
        private DataTable OnInitializeUserGroupDt()
        {
            task.TaskId = "0.9.7";
            task.StartTask();
            return task.ResultTable;
        }

        /// <summary>
        /// 获取用户组别表体DT
        /// </summary>
        private DataTable OnInitializeUserGroupDtlDt()
        {
            task.TaskId = "0.9.8";
            task.StartTask();
            return task.ResultTable;
        }

        /// <summary>
        /// 获取关联用户表头DT
        /// </summary>
        private DataTable OnInitializeRelUserDt()
        {
            task.TaskId = "0.0.0.1";
            task.StartTask();
            return task.ResultTable;
        }

        /// <summary>
        /// 获取关联用户表体DT
        /// </summary>
        private DataTable OnInitializeRelUserDtlDt()
        {
            task.TaskId = "0.0.0.2";
            task.StartTask();
            return task.ResultTable;
        }

        /// <summary>
        /// 初始化已关联‘研发类别’
        /// </summary>
        private DataTable OnInitializeDevGroupDt()
        {
            task.TaskId = "0.0.0.4";
            task.StartTask();
            return task.ResultTable;
        }

        /// <summary>
        /// 控制GridView单元格显示方式
        /// </summary>
        private void ControlGridViewisShow()
        {
            //注:当没有值时,若还设置某一行Row不显示的话,就会出现异常
            if (gvdtl?.Rows.Count > 0)
                gvdtl.Columns[0].Visible = false;
        }

        /// <summary>
        /// 获取所选择的GridView行生成DT
        /// </summary>
        private DataTable OnGetSelectRecordTemp()
        {
            //获取用户权限临时表
            var tempdt = dbList.CreateUserPermissionTemp();
            var newrow = tempdt.NewRow();
            newrow[0] = Convert.ToInt32(gvdtl.Rows[gvdtl.CurrentCell.RowIndex].Cells[0].Value);     //UseId
            newrow[1] = Convert.ToString(gvdtl.Rows[gvdtl.CurrentCell.RowIndex].Cells[1].Value);    //用户名称
            newrow[2] = DBNull.Value;                                                               //用户密码
            newrow[3] = Convert.ToString(gvdtl.Rows[gvdtl.CurrentCell.RowIndex].Cells[2].Value);    //创建人
            newrow[4] = Convert.ToDateTime(gvdtl.Rows[gvdtl.CurrentCell.RowIndex].Cells[3].Value);  //创建日期

            newrow[5] = Convert.ToString(gvdtl.Rows[gvdtl.CurrentCell.RowIndex].Cells[4].Value) == "已启用" ? 0 : 1; //是否启用  
            newrow[6] = Convert.ToString(gvdtl.Rows[gvdtl.CurrentCell.RowIndex].Cells[5].Value) == "是" ? 0 : 1;     //能否反审核
            newrow[7] = Convert.ToString(gvdtl.Rows[gvdtl.CurrentCell.RowIndex].Cells[6].Value) == "是" ? 0 : 1;     //能否查阅明细金额
            newrow[8] = Convert.ToString(gvdtl.Rows[gvdtl.CurrentCell.RowIndex].Cells[7].Value) == "是" ? 0 : 1;     //能否对明细物料操作
            newrow[9] = 1;                                                                                           //是否占用
            newrow[10] = Convert.ToString(gvdtl.Rows[gvdtl.CurrentCell.RowIndex].Cells[8].Value) == "是" ? 0 : 1;    //是否不关联用户 
            tempdt.Rows.Add(newrow);
            return tempdt;
        }
    }
}
