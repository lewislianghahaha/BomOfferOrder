﻿using System;
using System.Data;
using System.Drawing;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using BomOfferOrder.Task;

namespace BomOfferOrder.UI
{
    public partial class Main : Form
    {
        TaskLogic task=new TaskLogic();
        Load load=new Load();

        #region 变量参数
        //定义关闭符号的宽
        const int CloseSize = 11;

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
        #endregion

        public Main()
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
            pbimg.BackgroundImage = Image.FromFile(Application.StartupPath + @"\PIC\2.png");
            ToolTip tip1=new ToolTip();
            tip1.SetToolTip(btnSearch,"成本Bom报价单-查询");
            tip1.SetToolTip(btnCreate,"成本Bom报价单-创建");
            //初始化登入用户信息
            lbaccountmessage.Text = GlobalClasscs.User.StrUsrName;
            //初始化下拉列表
            OnShowTypeList();
        }

        private void OnRegisterEvents()
        {
            btnSearch.Click += BtnSearch_Click;
            btnCreate.Click += BtnCreate_Click;
            btnusersearch.Click += Btnusersearch_Click;

            bnMoveFirstItem.Click += BnMoveFirstItem_Click;
            bnMovePreviousItem.Click += BnMovePreviousItem_Click;
            bnMoveNextItem.Click += BnMoveNextItem_Click;
            bnMoveLastItem.Click += BnMoveLastItem_Click;
            bnPositionItem.TextChanged += BnPositionItem_TextChanged;
            tmshowrows.DropDownClosed += Tmshowrows_DropDownClosed;
            panel8.Visible = false;

            dtpconfirm.Enabled = false;
            tctotalpage.DrawItem += Tctotalpage_DrawItem;
            tctotalpage.MouseDown += Tctotalpage_MouseDown;
            tctotalpage.Padding = new Point(CloseSize, CloseSize - 8);   //初始化时添加Tab Control控件各Page选项卡的额外宽与高(重)
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
                task.TaskId = "0.4";

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
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
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
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
                CreateTabPages("成本BOM报价单-查询");
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
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
                        var createFrm = new CreateFrm
                        {
                            TopLevel = false,
                            BackColor = Color.White,
                            Dock = DockStyle.Fill,
                            FormBorderStyle = FormBorderStyle.None
                        };

                        createFrm.Show();                                         //只能使用Show()
                        newpage.Controls.Add(createFrm);                         //将窗体控件加入至新创建的Tab Page内
                        break;

                    case "成本BOM报价单-查询":
                        var searchFrm = new SearchFrm
                        {
                            TopLevel = false,
                            BackColor = Color.White,
                            Dock = DockStyle.Fill,
                            FormBorderStyle = FormBorderStyle.None
                        };

                        searchFrm.Show();                               //只能使用Show()
                        newpage.Controls.Add(searchFrm);                //将窗体控件加入至新创建的Tab Page内
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
        /// 控制GridView单元格显示方式
        /// </summary>
        private void ControlGridViewisShow(int id)
        {
            
        }

        /// <summary>
        /// 初始化下拉列表
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
                        dr[1] = "反审核";
                        break;
                    case 1:
                        dr[0] = "0";
                        dr[1] = "已审核";
                        break;
                }
                dt.Rows.Add(dr);
            }

            comstatus.DataSource = dt;
            comstatus.DisplayMember = "Name"; //设置显示值
            comstatus.ValueMember = "Id";    //设置默认值内码
        }



    }
}
