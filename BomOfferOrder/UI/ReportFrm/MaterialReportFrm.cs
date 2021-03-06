﻿using System;
using System.Data;
using System.Text.RegularExpressions;
using System.Threading;
using System.Windows.Forms;
using BomOfferOrder.DB;
using BomOfferOrder.Task;

namespace BomOfferOrder.UI.ReportFrm
{
    public partial class MaterialReportFrm : Form
    {
        TaskLogic task=new TaskLogic();
        Load load=new Load();
        DbList dbList=new DbList();

        #region 变量定义
        //返回DT类型
        private DataTable _resultTable;
        //记录报表生成方式;0:按导入EXCEL生成 1:按获取生成
        private int _reporttypeid;
        //记录初始化得出的物料DT
        private DataTable _materialdt;

        //保存查询出来的GridView记录（GridView页面跳转时使用）
        private DataTable _dtl;
        //记录当前页数(GridView页面跳转使用)
        private int _pageCurrent = 1;
        //记录计算出来的总页数(GridView页面跳转使用)
        private int _totalpagecount;
        //记录初始化标记(GridView页面跳转 初始化时使用)
        private bool _pageChange;
        #endregion

        #region Get
        /// <summary>
        /// 返回DT
        /// </summary>
        public DataTable ResultTable => _resultTable;
        /// <summary>
        /// 记录报表生成方式;0:按导入EXCEL生成 1:按获取生成
        /// </summary>
        public int Reporttypeid => _reporttypeid;
        #endregion


        public MaterialReportFrm()
        {
            InitializeComponent();
            OnRegisterEvents();
            OnInitialize();
        }

        private void OnRegisterEvents()
        {
            tmGet.Click += TmGet_Click;
            tmExcelImportGet.Click += TmExcelImportGet_Click;
            tmclose.Click += Tmclose_Click;
            btnsearch.Click += Btnsearch_Click;
            comtype.SelectedIndexChanged += Comtype_SelectedIndexChanged;
            tmExcelImport.Click += TmExcelImport_Click;

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
            //初始化生成下拉列表
            OnShowTypeList();
            //初始化获取物料DT
            OnInitializeMaterialdt();
        }

        /// <summary>
        /// 获取及打印
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void TmGet_Click(object sender, EventArgs e)
        {
            try
            {
                if (gvdtl.SelectedRows.Count == 0) throw new Exception("没有选中行,请选择后再继续");

                //获取GridView临时表
                _resultTable = dbList.MarkMaterialReportTemp();
                //循环GridView选中的记录
                foreach (DataGridViewRow row in gvdtl.SelectedRows)
                {
                    var newrow = _resultTable.NewRow();
                    newrow[0] = row.Cells[0].Value;  //FMATERIALID
                    newrow[1] = row.Cells[1].Value;  //物料编码
                    newrow[2] = row.Cells[2].Value;  //物料名称
                    newrow[3] = row.Cells[3].Value;  //规格
                    newrow[4] = row.Cells[4].Value;  //换算率 (密度(KG/L))
                    newrow[5] = row.Cells[5].Value;  //重量 (净重)
                    _resultTable.Rows.Add(newrow);
                }
                //记录生成报表方式(0:按EXCEL导入方式 1:按选取方式)
                _reporttypeid = 1;
                //清空文本框以前GridView值
                var dt = (DataTable) gvdtl.DataSource;
                dt.Rows.Clear();
                dt.Columns.Clear();
                gvdtl.DataSource = dt;
                //完成后关闭该窗体
                this.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, $"错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        /// <summary>
        /// 导入Excel打印(批量成本报表使用)
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void TmExcelImportGet_Click(object sender, EventArgs e)
        {
            try
            {
                var openFileDialog = new OpenFileDialog { Filter = $"Xlsx文件|*.xlsx" };
                if (openFileDialog.ShowDialog() != DialogResult.OK) return;

                task.TaskId = "5.2";
                task.FileAddress = openFileDialog.FileName;
                task.Reporttype = "0";  //导入EXCEL时的类型(0:报表功能使用  1:BOM物料明细使用)
                task.StartTask();

                //通过_materialdt循环获取其对应的FMATERIALID
                _resultTable = dbList.MarkMaterialReportTemp();
                foreach (DataRow row in task.ImportExceldtTable.Rows)
                {
                    //通过_materialdt获取其对应的FMATERIALID
                    var dtlrow = _materialdt.Select("物料编码='"+row[0]+"'");
                    //若返回的数据为0即跳过
                    if(dtlrow.Length ==0) continue;
                    var newrow = _resultTable.NewRow();
                    newrow[0] = dtlrow[0][0];   //FMATERIALID
                    newrow[1] = row[0];         //物料编码
                    newrow[2] = row[1];         //物料名称
                    newrow[3] = row[2];         //规格
                    newrow[4] = row[4];         //换算率(密度)
                    newrow[5] = row[5];         //重量(净重)
                    _resultTable.Rows.Add(newrow);
                }

                if(_resultTable.Rows.Count==0) throw new Exception("不能成功导入EXCEL内容,请检查模板是否正确.");
                else
                {
                    //记录生成报表方式(0:按EXCEL导入方式 1:按选取方式)
                    _reporttypeid = 0;
                    //若GridView有值的话,就清空相关行记录
                    if (gvdtl?.Rows.Count > 0)
                    {
                        var dt = (DataTable)gvdtl.DataSource;
                        dt.Rows.Clear();
                        dt.Columns.Clear();
                        gvdtl.DataSource = dt;
                    }
                    //完成后关闭窗体
                    this.Close();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, $"错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        /// <summary>
        /// 导入Excel打印(产品成本毛利润表使用)
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void TmExcelImport_Click(object sender, EventArgs e)
        {
            try
            {
                var openFileDialog = new OpenFileDialog { Filter = $"Xlsx文件|*.xlsx" };
                if (openFileDialog.ShowDialog() != DialogResult.OK) return;

                task.TaskId = "6.1";
                task.FileAddress = openFileDialog.FileName;
                task.Reporttype = "2";  //导入EXCEL时的类型(0:批量成本报表功能使用  1:BOM物料明细使用  2:毛利润报表使用)
                task.StartTask();

                //通过_materialdt循环获取其对应的FMATERIALID
                _resultTable = dbList.MarkMaterialReportTemp();

                foreach (DataRow row in task.ImportExceldtTable.Rows)
                {
                    //通过_materialdt获取其对应的FMATERIALID
                    var dtlrow = _materialdt.Select("物料编码='" + row[0] + "'");
                    //若返回的数据为0即跳过
                    if (dtlrow.Length == 0) continue;
                    var newrow = _resultTable.NewRow();
                    newrow[0] = dtlrow[0][0];   //FMATERIALID
                    newrow[1] = row[0];         //物料编码
                    newrow[2] = row[1];         //物料名称
                    newrow[3] = dtlrow[0][3];   //规格
                    newrow[4] = dtlrow[0][4];   //换算率(密度)
                    newrow[5] = dtlrow[0][5];   //重量(净重)
                    newrow[6] = dtlrow[0][6];   //罐箱
                    newrow[7] = dtlrow[0][7];   //分类
                    newrow[8] = dtlrow[0][8];   //品类
                    newrow[9] = dtlrow[0][9];   //销售计价单位
                    newrow[10] = dtlrow[0][10]; //U订货计价规格
                    _resultTable.Rows.Add(newrow);
                }

                if (_resultTable.Rows.Count == 0) throw new Exception("不能成功导入EXCEL内容,请检查模板是否正确.");
                else
                {
                    //记录生成报表方式(0:按EXCEL-批量成本报表使用导入方式 1:按选取方式 2:按EXCEL-毛利润报表使用导入方式)
                    _reporttypeid = 2;
                    //若GridView有值的话,就清空相关行记录
                    if (gvdtl?.Rows.Count > 0)
                    {
                        var dt = (DataTable)gvdtl.DataSource;
                        dt.Rows.Clear();
                        dt.Columns.Clear();
                        gvdtl.DataSource = dt;
                    }
                    //完成后关闭窗体
                    this.Close();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, $"错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
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
                gvdtl.DataSource = _resultTable;
                this.Close();
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
                //获取下拉列表所选值
                var dvordertylelist = (DataRowView)comtype.Items[comtype.SelectedIndex];
                var ordertypeId = Convert.ToInt32(dvordertylelist["Id"]);

                task.TaskId = "5";
                task.SearchId = ordertypeId;
                task.SearchValue = txtvalue.Text;

                new Thread(Start).Start();
                load.StartPosition = FormStartPosition.CenterScreen;
                load.ShowDialog();

                //连接GridView页面跳转功能
                LinkGridViewPageChange(task.ResultTable);
                //控制GridView单元格显示方式
                ControlGridViewisShow();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, $"错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        /// <summary>
        /// 下拉列表改变时发生
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Comtype_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                txtvalue.Text = "";
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
                MessageBox.Show(ex.Message, $"错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
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
        /// 查询下拉列表生成
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

            comtype.DataSource = dt;
            comtype.DisplayMember = "Name"; //设置显示值
            comtype.ValueMember = "Id";    //设置默认值内码
        }

        /// <summary>
        /// 控制GridView单元格显示方式
        /// </summary>
        private void ControlGridViewisShow()
        {
            //注:当没有值时,若还设置某一行Row不显示的话,就会出现异常
            if (gvdtl.Rows.Count > 0)
                gvdtl.Columns[0].Visible = false;
        }

        /// <summary>
        /// 初始化物料DT
        /// </summary>
        private void OnInitializeMaterialdt()
        {
            var dvordertylelist = (DataRowView)comtype.Items[comtype.SelectedIndex];
            var ordertypeId = Convert.ToInt32(dvordertylelist["Id"]);

            task.TaskId = "5";
            task.SearchId = ordertypeId;
            task.SearchValue = txtvalue.Text;
            task.StartTask();
            _materialdt = task.ResultTable;
        }
    }
}
