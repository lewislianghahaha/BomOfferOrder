﻿using System;
using System.Data;
using System.Text.RegularExpressions;
using System.Threading;
using System.Windows.Forms;
using BomOfferOrder.DB;
using BomOfferOrder.Task;

namespace BomOfferOrder.UI
{
    public partial class ShowMaterialDetailFrm : Form
    {
        TaskLogic task=new TaskLogic();
        Load load=new Load();
        DbList dbList = new DbList();

        #region 变量参数
        //获取remark值(A:新增;U:替换;HA:获取新产品BOM明细记录-新增 HU:获取新产品BOM明细记录-替换)
        private string _remark;
        //返回DT类型
        private DataTable _resultTable;

        //保存查询出来的GridView记录（GridView页面跳转时使用）
        private DataTable _dtl;
        //记录当前页数(GridView页面跳转使用)
        private int _pageCurrent = 1;
        //记录计算出来的总页数(GridView页面跳转使用)
        private int _totalpagecount;
        //记录初始化标记(GridView页面跳转 初始化时使用)
        private bool _pageChange;
        #endregion

        #region Set
            /// <summary>
            /// 获取ID值,(替换时使用)
            /// </summary>
            public string Remark { set { _remark = value; } }
        #endregion

        #region Get

        /// <summary>
        /// 返回DT
        /// </summary>
        public DataTable ResultTable => _resultTable;

        #endregion

        public ShowMaterialDetailFrm()
        {
            InitializeComponent();
            OnRegisterEvents();
            //OnInitialize();
        }

        private void OnRegisterEvents()
        {
            tmGet.Click += TmGet_Click;
            tmclose.Click += Tmclose_Click;
            btnsearch.Click += Btnsearch_Click;
            comtype.SelectedIndexChanged += Comtype_SelectedIndexChanged;

            bnMoveFirstItem.Click += BnMoveFirstItem_Click;
            bnMovePreviousItem.Click += BnMovePreviousItem_Click;
            bnMoveNextItem.Click += BnMoveNextItem_Click;
            bnMoveLastItem.Click += BnMoveLastItem_Click;
            bnPositionItem.Leave += BnPositionItem_Leave;
            tmshowrows.DropDownClosed += Tmshowrows_DropDownClosed;
            panel2.Visible = false;
        }

        /// <summary>
        /// 初始化记录
        /// </summary>
        private void OnInitialize()
        {
            //初始化下拉列表
            OnShowTypeList();
            //若查询文本框不为空,即在初始化时将其清空
            if (txtvalue.Text != "")
                txtvalue.Text = "";
        }

        /// <summary>
        /// 初始化GridView信息
        /// </summary>
        public void OnInitializeGridView(DataTable sourcedt)
        {
            //初始化下拉列表及查询文本框
            OnInitialize();
            //根据_remark初始化标题名称
            OnShowTitle();
            //将数据源放到GridView内显示
            LinkGridViewPageChange(sourcedt);
            //设置GridView是否显示某些列
            ControlGridViewisShow();
        }

        /// <summary>
        /// 下拉列表改变时发生
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Comtype_SelectedIndexChanged(object sender, EventArgs e)
        {
            txtvalue.Text = "";
        }

        /// <summary>
        /// 获取
        /// A:新增;U:替换;HA:获取新产品BOM明细记录-新增 HU:获取新产品BOM明细记录-替换
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void TmGet_Click(object sender, EventArgs e)
        {
            try
            {
                //区分:当_remark="A"时,表示‘新增’记录,可多行选择; 反之,为‘替换’使用,只能选择一行
                if (gvdtl.SelectedRows.Count == 0) throw new Exception("没有选中行,请选择后再继续");
                //获取GridView临时表
                _resultTable = dbList.MakeGridViewTemp();
                //当为‘新增’操作时-> _remark="A"时使用
                if (_remark =="A")
                {
                    //循环所选择的行数
                    foreach (DataGridViewRow row in gvdtl.SelectedRows)
                    {
                        var newrow = _resultTable.NewRow();
                        newrow[1] = row.Cells[0].Value;     //物料编码ID
                        newrow[2] = row.Cells[1].Value;     //物料编码
                        newrow[3] = row.Cells[2].Value;     //物料名称
                        newrow[5] = row.Cells[6].Value;     //物料单价
                        _resultTable.Rows.Add(newrow);
                    }
                }
                //当为‘新增’操作时-> _remark="HA"时使用
                else if (_remark=="HA")
                {
                    //循环所选择的行数
                    foreach (DataGridViewRow row in gvdtl.SelectedRows)
                    {
                        var newrow = _resultTable.NewRow();
                        newrow[1] = row.Cells[0].Value;     //物料编码ID
                        newrow[2] = row.Cells[1].Value;     //物料编码
                        newrow[3] = row.Cells[2].Value;     //物料名称
                        newrow[5] = row.Cells[3].Value;     //物料单价
                        _resultTable.Rows.Add(newrow);
                    }
                }

                //当为‘替换’操作时-> _remark="U"时使用
                else if(_remark=="U")
                {
                    if(gvdtl.SelectedRows.Count>1) throw new Exception("只能选择一行记录进行替换,请重新选择");
                    var newrow = _resultTable.NewRow();
                    newrow[1] = gvdtl.SelectedRows[0].Cells[0].Value; //物料编码ID
                    newrow[2] = gvdtl.SelectedRows[0].Cells[1].Value; //物料编码
                    newrow[3] = gvdtl.SelectedRows[0].Cells[2].Value; //物料名称
                    newrow[5] = gvdtl.SelectedRows[0].Cells[6].Value; //物料单价
                    _resultTable.Rows.Add(newrow);
                }
                //当为‘替换’操作时-> _remark="HU"时使用
                else if (_remark=="HU")
                {
                    if (gvdtl.SelectedRows.Count > 1) throw new Exception("只能选择一行记录进行替换,请重新选择");
                    var newrow = _resultTable.NewRow();
                    newrow[1] = gvdtl.SelectedRows[0].Cells[0].Value; //物料编码ID
                    newrow[2] = gvdtl.SelectedRows[0].Cells[1].Value; //物料编码
                    newrow[3] = gvdtl.SelectedRows[0].Cells[2].Value; //物料名称
                    newrow[5] = gvdtl.SelectedRows[0].Cells[3].Value; //物料单价
                    _resultTable.Rows.Add(newrow);
                }
                //完成后关闭该窗体
                this.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
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
                //获取下拉列表所选值
                var dvordertylelist = (DataRowView)comtype.Items[comtype.SelectedIndex];
                var ordertypeId = Convert.ToInt32(dvordertylelist["Id"]);

                //根据不同_remark得出不同的TaskId值
                switch (_remark)
                {
                    case "A":
                    case "U":
                        task.TaskId = "0.2";
                        break;
                    case "HA":
                    case "HU":
                        task.TaskId = "0.9.3";
                        break;
                }
                task.SearchId = ordertypeId;
                task.SearchValue = txtvalue.Text;

                new Thread(Start).Start();
                load.StartPosition = FormStartPosition.CenterScreen;
                load.ShowDialog();

                //连接GridView页面跳转功能
                LinkGridViewPageChange(task.ResultTable);
                //设置GridView是否显示某些列
                ControlGridViewisShow();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        /// <summary>
        /// 关闭
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Tmclose_Click(object sender, EventArgs e)
        {
            try
            {
                //若返回的DT有值时，先将内容清空,再执行“关闭”(注:必须有值)
                if (_resultTable?.Rows.Count > 0)
                {
                    _resultTable.Rows.Clear();
                    _resultTable.Columns.Clear();
                }
                this.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
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
                        bnMoveNextItem.Enabled = false;
                        bnMoveLastItem.Enabled = false;
                        bnMoveNextItem.Enabled = false;
                        bnMoveLastItem.Enabled = false;
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
        /// 产品系列下拉列表
        /// </summary>
        private void OnShowTypeList()
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

            //若标记为A或U的话,就执行以下语句
            if (_remark == "A" || _remark == "U")
            {
                //创建行内容
                for (var j = 0; j < 2; j++)
                {
                    var dr = dt.NewRow();

                    switch (j)
                    {
                        case 0:
                            dr[0] = "1";
                            dr[1] = "物料名称";
                            break;
                        case 1:
                            dr[0] = "2";
                            dr[1] = "物料编码";
                            break;
                    }
                    dt.Rows.Add(dr);
                }
            }
            //若标记为HA或HU的话,就执行以下语句
            else if (_remark == "HA" || _remark == "HU")
            {
                //创建行内容
                for (var j = 0; j < 3; j++)
                {
                    var dr = dt.NewRow();

                    switch (j)
                    {
                        case 0:
                            dr[0] = "1";
                            dr[1] = "物料名称";
                            break;
                        case 1:
                            dr[0] = "2";
                            dr[1] = "物料编码";
                            break;
                        case 2:
                            dr[0] = "3";
                            dr[1] = "OA流水号";
                            break;
                    }
                    dt.Rows.Add(dr);
                }
            }

            comtype.DataSource = dt;
            comtype.DisplayMember = "Name"; //设置显示值
            comtype.ValueMember = "Id";    //设置默认值内码
        }

        /// <summary>
        /// 按要求显示标题名称
        /// </summary>
        private void OnShowTitle()
        {
            if (_remark == "A" || _remark == "U")
            {
                this.Text = "物料明细表";
            }
            else
            {
                this.Text = "新产品报价单历史记录";
            }
        }

        /// <summary>
        /// 连接GridView页面跳转
        /// </summary>
        /// <param name="dt"></param>
        private void LinkGridViewPageChange(DataTable dt)
        {
            if (dt.Rows.Count > 0)
            {
                _dtl = dt;
                panel2.Visible = true;
                //初始化下拉框所选择的默认值
                tmshowrows.SelectedItem = "10";
                //定义初始化标记
                _pageChange = _pageCurrent <= 1;
                //GridView分页
                GridViewPageChange();
            }
            //注:当为空记录时,不显示跳转页;只需将临时表赋值至GridView内
            else
            {
                gvdtl.DataSource = dt;
                panel2.Visible = false;
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
        /// 控制GridView单元格显示方式
        /// </summary>
        private void ControlGridViewisShow()
        {
            //注:当没有值时,若还设置某一行Row不显示的话,就会出现异常
            gvdtl.Columns[0].Visible = false;
        }

    }
}
