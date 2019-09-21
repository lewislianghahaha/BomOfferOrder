using System;
using System.Data;
using System.Text.RegularExpressions;
using System.Threading;
using System.Windows.Forms;
using BomOfferOrder.DB;
using BomOfferOrder.Task;

namespace BomOfferOrder.UI
{
    public partial class CreateFrm : Form
    {
        TaskLogic task=new TaskLogic();
        Load load=new Load();
        DbList dbList=new DbList();
        DtlFrm dtlFrm=new DtlFrm();

        //保存BOM明细DT(生成时使用;注:当打开录入界面时初始化执行)
        private DataTable _bomdt = new DataTable();

        //保存GridView内需要进行添加的临时表
        private DataTable _adddt = new DataTable();

        //保存查询出来的GridView记录
        private DataTable _dtl;
        //保存查询出来的角色权限记录
        private DataTable _userdt;

        //记录当前页数(GridView页面跳转使用)
        private int _pageCurrent = 1;
        //记录计算出来的总页数(GridView页面跳转使用)
        private int _totalpagecount;
        //记录初始化标记(GridView页面跳转 初始化时使用)
        private bool _pageChange;

        public CreateFrm()
        {
            InitializeComponent();
            OnRegisterEvents();
            OnInitialize();
        }

        private void OnRegisterEvents()
        {
            tmclose.Click += Tmclose_Click;
            btnsearch.Click += Btnsearch_Click;
            btngenerate.Click += Btngenerate_Click;
            tmadd.Click += Tmadd_Click;
            tmdel.Click += Tmdel_Click;

            bnMoveFirstItem.Click += BnMoveFirstItem_Click;
            bnMovePreviousItem.Click += BnMovePreviousItem_Click;
            bnMoveNextItem.Click += BnMoveNextItem_Click;
            bnMoveLastItem.Click += BnMoveLastItem_Click;
            bnPositionItem.TextChanged += BnPositionItem_TextChanged;
            tmshowrows.DropDownClosed += Tmshowrows_DropDownClosed;
            panel2.Visible = false;

            btngenerate.Enabled = false;
            comtype.SelectedIndexChanged += Comtype_SelectedIndexChanged;
        }

        /// <summary>
        /// 初始化相关记录(下拉列表)
        /// </summary>
        private void OnInitialize()
        {
            //下拉列表使用
            OnShowTypeList();
            //初始化BOM明细DT
            OnInitializeBomdt();
        }

        /// <summary>
        /// 下拉列表改变时执行
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

                task.TaskId = "0";
                task.SearchId = ordertypeId;
                task.SearchValue = txtvalue.Text;

                new Thread(Start).Start();
                load.StartPosition = FormStartPosition.CenterScreen;
                load.ShowDialog();

                if (task.ResultTable.Rows.Count > 0)
                {
                    _dtl = task.ResultTable;
                    panel2.Visible = true;
                    //初始化下拉框所选择的默认值
                    tmshowrows.SelectedItem = "10";
                    //定义初始化标记
                    _pageChange = true;
                    //GridView分页
                    GridViewPageChange();
                }
                //注:当为空记录时,不显示跳转页;只需将临时表赋值至GridView内
                else
                {
                    gvsearchdtl.DataSource = task.ResultTable;
                    panel2.Visible = false;
                }
                //控制GridView单元格显示方式
                ControlGridViewisShow(0);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        /// <summary>
        /// 添加至明细记录(注:可多行选择)
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Tmadd_Click(object sender, EventArgs e)
        {
            try
            {
                if(gvsearchdtl.RowCount==0)throw new Exception("没有查询结果,不能添加");

                //获取临时表
                var temp = dbList.Get_Searchdt();

                foreach (DataGridViewRow row in gvsearchdtl.SelectedRows)
                {
                    var newrow = temp.NewRow();
                    newrow[0] = row.Cells[0].Value;  //FMATERIALID
                    newrow[1] = row.Cells[1].Value;  //物料编码
                    newrow[2] = row.Cells[2].Value;  //物料名称
                    newrow[3] = row.Cells[4].Value;  //规格型号
                    newrow[4] = row.Cells[6].Value;  //密度(KG/L)
                    temp.Rows.Add(newrow);
                }
                //若_adddt+temp.rowscount得出的总行数>10行时,即提示异常
                if(_adddt.Rows.Count+temp.Rows.Count>10)throw new Exception("添加行数已超过10行,不能继续");
                //判断若需要添加的记录,已在_adddt存在,即提示异常
                if(!CheckRecord(temp))throw new Exception("已添加,不能再次进行添加");
                //将要添加的记录添加至‘添加明细记录’GridView内
                gvdtl.DataSource=AddsoucetoDt(temp);
                //控制GridView单元格显示方式
                ControlGridViewisShow(1);
                //若成行添加,就将‘生成明细记录按钮’按钮设为‘启用’
                btngenerate.Enabled = true;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        /// <summary>
        /// 删除记录(明细窗体使用)
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Tmdel_Click(object sender, EventArgs e)
        {
            try
            {
                if(gvdtl.RowCount==0)throw new Exception("没有明细记录,不能作删除操作");
                for (var i = gvdtl.SelectedRows.Count; i > 0; i--)
                {
                    gvdtl.Rows.RemoveAt(gvdtl.SelectedRows[i - 1].Index);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        /// <summary>
        /// 生成明细记录
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Btngenerate_Click(object sender, EventArgs e)
        {
            try
            {
                if(gvdtl.RowCount==0)throw new Exception("没有明细记录,不能执行运算");
                var clickMessage = $"您所选择进行生成的物料有:'{gvdtl.RowCount}'行物料记录 \n 是否继续生成?";

                if (MessageBox.Show(clickMessage, "提示", MessageBoxButtons.YesNo, MessageBoxIcon.Information) == DialogResult.Yes)
                {
                    task.TaskId = "1";
                    task.Data = (DataTable)gvdtl.DataSource;
                    task.Valuelist = Get_ValueList((DataTable)gvdtl.DataSource); //将Fmaterialid整合至List形式
                    task.Bomdt = _bomdt;                                         //获取初始化的BOM明细DT

                    new Thread(Start).Start();
                    load.StartPosition = FormStartPosition.CenterScreen;
                    load.ShowDialog();

                    if (!task.ResultMark) throw new Exception("生成出现异常,请联系管理员");
                    else
                    {
                        //弹出对应窗体相关设置
                        dtlFrm.FunState = "C";
                        dtlFrm.OnInitialize(task.ResultTable);     //初始化信息
                        dtlFrm.StartPosition = FormStartPosition.CenterParent;
                        dtlFrm.ShowDialog();
                    }
                    //若返回父窗体后将各控件清空
                    gvdtl.DataSource = null;
                    gvsearchdtl.DataSource = null;
                    _adddt.Rows.Clear();
                    txtvalue.Text = "";
                }
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
            this.Close();
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
        private void BnPositionItem_TextChanged(object sender, EventArgs e)
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
                gvsearchdtl.DataSource = tempdt;
                //将“当前页”赋值给"跳转页"文本框内
                bnPositionItem.Text = Convert.ToString(_pageCurrent);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        /// <summary>
        /// 控制GridView单元格显示方式
        /// </summary>
        private void ControlGridViewisShow(int id)
        {
            //注:当没有值时,若还设置某一行Row不显示的话,就会出现异常
            if (id == 0)
            {
                gvsearchdtl.Columns[0].Visible = false;
            }
            else
            {
                if (gvdtl.RowCount > 0)
                    gvdtl.Columns[0].Visible = false;
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
        /// 将查询明细的记录添加至‘明细记录’GridView内
        /// </summary>
        /// <returns></returns>
        private DataTable AddsoucetoDt(DataTable sourcedt)
        {
            try
            {
                //判断若reslut为空,即直接插入,反之作更新操作
                if (_adddt.Rows.Count == 0)
                {
                    _adddt = sourcedt.Copy();
                }
                else
                {
                    foreach (DataRow row in sourcedt.Rows)
                    {
                        var newrow = _adddt.NewRow();
                        for (var i = 0; i < _adddt.Columns.Count; i++)
                        {
                            newrow[i] = row[i];
                        }
                        _adddt.Rows.Add(newrow);
                    }
                }
            }
            catch (Exception)
            {
                _adddt.Columns.Clear();
                _adddt.Rows.Clear();
            }
            return _adddt;
        }

        /// <summary>
        /// 判断若需要添加的记录,已在_adddt存在,即提示异常
        /// </summary>
        /// <param name="sourcedt"></param>
        /// <returns></returns>
        private bool CheckRecord(DataTable sourcedt)
        {
            var result = true;
            try
            {
                //判断若‘添加’的记录是否在_adddt,若存在返回FALSE
                for (var i = 0; i < sourcedt.Rows.Count; i++)
                {
                    for (var j = 0; j < _adddt.Rows.Count; j++)
                    {
                        if (Convert.ToInt32(sourcedt.Rows[i][0]) == Convert.ToInt32(_adddt.Rows[j][0]))
                        {
                            result = false;
                            break;
                        }
                    }
                }
            }
            catch (Exception)
            {
                result = false;
            }
            return result;
        }

        /// <summary>
        /// 获取DT内的FmaterialID列表
        /// </summary>
        /// <param name="sourcedt"></param>
        /// <returns></returns>
        private string Get_ValueList(DataTable sourcedt)
        {
            var result = string.Empty;

            foreach (DataRow row in sourcedt.Rows)
            {
                if (result == "")
                {
                    result = "'"+row[0]+"'";
                }
                else
                {
                    result += "," + "'" + row[0] + "'";
                }
            }
            return result;
        }

        /// <summary>
        /// 初始化BOM明细DT
        /// </summary>
        /// <returns></returns>
        private void OnInitializeBomdt()
        {
            task.TaskId = "0.1";
            task.StartTask();
            _bomdt=task.Resultbomdt;
        }
    }
}
