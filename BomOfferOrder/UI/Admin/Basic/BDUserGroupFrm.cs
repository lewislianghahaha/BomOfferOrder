using System;
using System.Data;
using System.Text.RegularExpressions;
using System.Threading;
using System.Windows.Forms;
using BomOfferOrder.Task;

namespace BomOfferOrder.UI.Admin.Basic
{
    public partial class BdUserGroupFrm : Form
    {
        TaskLogic task = new TaskLogic();
        Load load = new Load();
        UserInfoList userInfo=new UserInfoList();
        BdUserGroupAddFrm bdUserGroupAdd=new BdUserGroupAddFrm();

        #region 变量参数

        //保存(树菜单使用)-包括新增及从数据库读取的记录
        private DataTable _dt;
        //保存查询出来的GridView记录(注:保存所有明细记录行信息)
        private DataTable _dtl;

        //存放删除的记录(表头信息)
        private DataTable _deldt;
        //存放删除的记录(表体信息)
        private DataTable _deldtldt;

        //获取K3用户信息
        private DataTable _k3Userdt;

        //记录当前页数(GridView页面跳转使用)
        private int _pageCurrent = 1;
        //记录计算出来的总页数(GridView页面跳转使用)
        private int _totalpagecount;
        //记录初始化标记(GridView页面跳转 初始化时使用)
        private bool _pageChange;
        #endregion

        public BdUserGroupFrm()
        {
            InitializeComponent();
            OnRegisterEvents();
        }

        private void OnRegisterEvents()
        {
            tmSave.Click += TmSave_Click;
            tmclose.Click += Tmclose_Click;
            btncreate.Click += Btncreate_Click;
            btnchange.Click += Btnchange_Click;
            btndel.Click += Btndel_Click;
            tvview.AfterSelect += Tvview_AfterSelect;
            btnShowUserList.Click += BtnShowUserList_Click;
            tmdel.Click += Tmdel_Click;

            bnMoveFirstItem.Click += BnMoveFirstItem_Click;
            bnMovePreviousItem.Click += BnMovePreviousItem_Click;
            bnMoveNextItem.Click += BnMoveNextItem_Click;
            bnMoveLastItem.Click += BnMoveLastItem_Click;
            bnPositionItem.Leave += BnPositionItem_Leave;
            tmshowrows.DropDownClosed += Tmshowrows_DropDownClosed;
            panel2.Visible = false;
        }

        /// <summary>
        /// 初始化
        /// </summary>
        public void OnInitialize(DataTable k3Userdt)
        {
            _k3Userdt = k3Userdt;
            //清空树菜单信息
            tvview.Nodes.Clear();
            //获取相关数据源(包括表头信息)
            OnSearch();
            //树菜单读取
            ShowTreeList(_dt);
            //展开根节点
            tvview.ExpandAll();
            //连接GridView页面跳转功能
            LinkGridViewPageChange(OnSearchDtl());
            //控制GridView单元格显示方式
            ControlGridViewisShow();
        }

        /// <summary>
        /// 查询及获取表头信息
        /// </summary>
        private void OnSearch()
        {
            //获取表头信息
            task.TaskId = "0.9.7";
            task.StartTask();
            _dt = task.ResultTable;
            //将表头信息列插入至_deldt内
            _deldt = _dt.Clone();
        }

        /// <summary>
        /// 查询及获取表体信息
        /// </summary>
        /// <returns></returns>
        private DataTable OnSearchDtl()
        {
            //获取表体信息
            task.TaskId = "0.9.8";
            task.StartTask();
            var resuldt = task.ResultTable;
            //将表体信息列插入至_deldtldt内
            _deldtldt = resuldt.Clone();
            return resuldt;
        }

        /// <summary>
        /// 树菜单读取
        /// </summary>
        /// <param name="dt"></param>
        private void ShowTreeList(DataTable dt)
        {
            if (dt.Rows.Count == 1)
            {
                var tree=new TreeNode();
                tree.Tag = 0;
                tree.Text = "ALL";
                tvview.Nodes.Add(tree);
            }
            else
            {
                var tree=new TreeNode();
                tree.Tag = 0;
                tree.Text = "ALL";
                tvview.Nodes.Add(tree);
                //展开根节点
                tvview.ExpandAll();
                //读取记录并增加至子节点内(从二级节点开始)
                AddChildNode(tvview,dt);
            }
        }

        /// <summary>
        /// 子节点菜单相关内容读取
        /// </summary>
        /// <param name="tvView"></param>
        /// <param name="dt"></param>
        private void AddChildNode(TreeView tvView,DataTable dt)
        {
            var tnode = tvView.Nodes[0];
            var rows = dt.Select("Parentid='"+Convert.ToInt32(tnode.Tag)+"'");
            //循环获取子节点信息及进行添加
            foreach (var r in rows)
            {
                var tn=new TreeNode();
                tn.Tag = Convert.ToInt32(r[0]);     //自身主键ID
                tn.Text = Convert.ToString(r[1]);   //节点内容
                //将二级节点添加至根节点下
                tnode.Nodes.Add(tn);
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
                _dtl = dt;
                gvdtl.DataSource = dt;
                panel2.Visible = false;
            }
        }

        /// <summary>
        /// 点击树菜单某个节点时出现-用于读取记录
        /// 作用:当点击某个节点时,GridView显示相关的记录
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Tvview_AfterSelect(object sender, TreeViewEventArgs e)
        {
            //复制表结构
            var dt = _dtl.Clone();

            try
            {
                //若点击"ALL"节点,即在GridView内显示全部信息;而其它节点需使用当前所选的节点.Tag为条件
                if (Convert.ToInt32(tvview.SelectedNode.Tag) == 0)
                {
                    dt = _dtl;
                }
                else
                {
                    //以Convert.ToInt32(tvview.SelectedNode.Tag)为条件进行对_dtl查询,并将结果插入至dt内
                    var dtlrows = _dtl.Select("Groupid='"+ Convert.ToInt32(tvview.SelectedNode.Tag) + "'");
                    if (dtlrows.Length >0)
                    {
                        for (var i = 0; i < dtlrows.Length; i++)
                        {
                            var newrow = dt.NewRow();
                            for (var j = 0; j < dt.Columns.Count; j++)
                            {
                                newrow[j] = dtlrows[i][j];
                            }
                            dt.Rows.Add(newrow);
                        }
                    }
                }
                //刷新信息
                LinkGridViewPageChange(dt);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, $"错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        /// <summary>
        /// 获取员工信息
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BtnShowUserList_Click(object sender, EventArgs e)
        {
            try
            {
                //判断若没有点击任何TreeView节点,会作出异常提示
                if (tvview.SelectedNode == null) throw new Exception("没有选择任何节点,请选择再继续");
                //判断除点击“ALL”节点外才可以添加
                if (tvview.SelectedNode.Text == $"ALL") throw new Exception("请选择除ALL节点外的节点进行添加");

                userInfo.OnInitialize(_k3Userdt);
                userInfo.StartPosition = FormStartPosition.CenterParent;
                userInfo.ShowDialog();

                //以下为返回相关记录返回本窗体相关处理
                //判断若返回的DT为空的话,就不需要任何效果
                if (userInfo.ResultTable == null || userInfo.ResultTable.Rows.Count == 0) return;
                //将返回的结果赋值至GridView内(注:判断若返回的DT不为空或行数大于0才执行更新效果)
                if (userInfo.ResultTable != null || userInfo.ResultTable.Rows.Count > 0)
                {
                    var a = userInfo.ResultTable;
                    //循环将userInfo.ResultTable内的记录插入至_dtl内
                    foreach (DataRow rows in userInfo.ResultTable.Rows)
                    {
                        var newrow = _dtl.NewRow();
                        newrow[0] = Convert.ToInt32(tvview.SelectedNode.Tag); //GroupID
                        newrow[1] = GenerateDtlId();                          //Dtlid
                        newrow[2] = Convert.ToString(rows[0]);                //员工名称
                        newrow[3] = Convert.ToString(rows[1]);                //K3用户组别
                        newrow[4] = Convert.ToString(rows[2]);                //K3用户手机
                        newrow[5] = "Admin";                                  //创建人
                        newrow[6] = DateTime.Now;                             //创建日期
                        _dtl.Rows.Add(newrow);
                    }
                    //刷新信息
                    LinkGridViewPageChange(_dtl);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, $"错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        /// <summary>
        ///  创建新增的DtlId
        /// </summary>
        /// <returns></returns>
        private int GenerateDtlId()
        {
            var result = 0;
            if (_dtl.Rows.Count == 0)
            {
                result = -1;
            }
            else
            {
                result = Convert.ToInt32("-" + (_dtl.Rows.Count + 1));
            }
            return result;
        }

        /// <summary>
        /// 创建用户组别
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Btncreate_Click(object sender, EventArgs e)
        {
            try
            {
                //判断若没有选择节点 或 选择的节点不为ALL的话,就报异常
                if (tvview.SelectedNode == null) throw new Exception("没有选择任何节点,请选择再继续");
                if (tvview.SelectedNode.Text != $"ALL") throw new Exception("请在ALL节点下进行新增用户组别");

                //调用用户分组对话框
                bdUserGroupAdd.StartPosition=FormStartPosition.CenterParent;
                bdUserGroupAdd.ShowDialog();
                
                var newrow = _dt.NewRow();
                newrow[0] = GenerateGroupId();          //GroupId=>(注:以-开头+累加的ID值)
                newrow[1] = tvview.SelectedNode.Tag;    //Parentid=>默认为0
                newrow[2] = bdUserGroupAdd.ResultValue; //GroupName
                newrow[3] ="Admin";                     //CreateName
                newrow[4] = DateTime.Now;               //CreateDt
                _dt.Rows.Add(newrow);

                //创建完成后将_dt放至ShowTreeList()方法内
                ShowTreeList(_dt);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, $"错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        /// <summary>
        /// 创建新增的GroupId(注:使用_dt.rows[0]进行累加,从-1开始)
        /// </summary>
        /// <returns></returns>
        private int GenerateGroupId()
        {
            var result = 0;
            if (_dt.Rows.Count == 0)
            {
                result = -1;
            }
            else
            {
                result = Convert.ToInt32("-" + (_dt.Rows.Count + 1));
            }
            return result;
        }

        /// <summary>
        /// 修改组别
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Btnchange_Click(object sender, EventArgs e)
        {
            try
            {
                //判断若没有选择节点就报异常
                if (tvview.SelectedNode == null) throw new Exception("没有选择任何节点,请选择再继续");
                //若点击的当前节点为ALL的时候,就报异常
                if (tvview.SelectedNode.Text == $"ALL") throw new Exception("不能修改ALL节点");
                //获取当前节点信息(Tag)
                var currenttage = Convert.ToInt32(tvview.SelectedNode.Tag);

                //调用bdUserGroupAdd窗体
                bdUserGroupAdd.StartPosition = FormStartPosition.CenterParent;
                bdUserGroupAdd.ShowDialog();

                //获取bdUserGroupAdd窗体返回的值并进行对当前节点进行更新
                UpdateDt(currenttage,bdUserGroupAdd.ResultValue);
                //更新完成后将新的DT信息放到ShowTreeList()进行刷新
                ShowTreeList(_dt);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, $"错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        /// <summary>
        /// 更新节点dt信息
        /// </summary>
        /// <returns></returns>
        private void UpdateDt(int tag,string value)
        { 
            for (var i = 0; i < _dt.Rows.Count; i++)
            {
                if(Convert.ToInt32(_dt.Rows[i][0]) != tag) continue;
                _dt.Rows[i].BeginEdit();
                _dt.Rows[i][2] = value;
                _dt.Rows[i].EndEdit();
            }
        }

        /// <summary>
        /// 判断使用_tempdt或_dt的前提条件取决于当前选定的节点是否含有“-”
        /// 删除组别=>作用:1)对_dt _dtldt进行相关节点记录删除;
        /// 2)若对应的TAG包含“-”,就将其信息保存至_deldt及_deldtldt(这个用来保存表体信息)内(最后的删除使用)
        /// 注:此功能执行时,是将用户组别以及对应的明细记录进行都进行删除!!!
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Btndel_Click(object sender, EventArgs e)
        {
            try
            {
                //判断若没有选择节点就报异常
                if (tvview.SelectedNode == null) throw new Exception("没有选择任何节点,请选择再继续");
                //判断若点选的当前节点为ALL的话,就报异常
                if (tvview.SelectedNode.Text == $"ALL") throw new Exception("ALL节点不能删除");

                var clickMessage = $"是否确定删除节点:{tvview.SelectedNode.Text}的相关信息? \n 注:此删除操作是会将关联的明细记录进行删除";

                if (MessageBox.Show(clickMessage, $"提示", MessageBoxButtons.YesNo, MessageBoxIcon.Information) == DialogResult.Yes)
                {
                    //针对_dt及 _dtldt进行删除操作(注:若当前所选节点的TAG包含"-",也要将删除信息分别存放至_deldt及_deldtldt内)
                    DelDt(Convert.ToInt32(tvview.SelectedNode.Tag));
                    //完成后分别对树菜单 及 GridView进行刷新
                    ShowTreeList(_dt);
                    LinkGridViewPageChange(_dtl);
                } 
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, $"错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        /// <summary>
        /// 根据节点删除对应的表头及表体记录
        /// 注:若判断TAG不包含“-”,即将删除记录保存至_deldt 及 _deldtldt内
        /// </summary>
        /// <returns></returns>
        private void DelDt(int tag)
        {
            //若不包含“-”,即将记录进行插入至_deldt 及 _deldtldt内
            if (!Convert.ToString(tag).Contains("-"))
            {
                //对_deldt进行插入操作
                InsertDeldt(tag);
                //对_deldtldt进行插入操作
                InsertDelDtldt(tag,0,0);
            }

            //最后针对_dt及_dtldt进行删除
            for (var i = _dt.Rows.Count; i > 0; i--)
            {
                if (Convert.ToInt32(_dt.Rows[i - 1][0]) == tag)
                {
                    _dt.Rows.RemoveAt(i-1);
                }
            }

            for (var i = _dtl.Rows.Count; i > 0; i--)
            {
                if (Convert.ToInt32(_dtl.Rows[i - 1][0]) == tag)
                {
                    _dtl.Rows.RemoveAt(i - 1);
                }
            }
        }

        /// <summary>
        /// 保存需删除的表头记录(注:需不包含“-”)
        /// </summary>
        /// <param name="tag"></param>
        private void InsertDeldt(int tag)
        {
            if (!Convert.ToString(tag).Contains("-"))
            {
                var newrow = _deldt.NewRow();
                for (var i = 0; i < _dt.Rows.Count; i++)
                {
                    if (Convert.ToInt32(_dt.Rows[i][0]) != tag) continue;
                    for (var j = 0; j < _dt.Columns.Count; j++)
                    {
                        newrow[j] = _dt.Rows[i][j];
                    }
                }
                _deldt.Rows.Add(newrow);
            }
        }

        /// <summary>
        /// 保存需删除的表体记录(注:需不包含“-”)
        /// </summary>
        /// <param name="tag"></param>
        /// <param name="id">0:‘组别删除’使用 1:“明细删除”使用</param>
        /// <param name="dtlid">“明细删除”使用=>记录所选中的GridView.Dtlid信息</param>
        private void InsertDelDtldt(int tag,int id,int dtlid)
        {
            if (!Convert.ToString(tag).Contains("-"))
            {

                if (id == 0)
                {
                    for (var i = 0; i < _dtl.Rows.Count; i++)
                    {
                        if (Convert.ToInt32(_dtl.Rows[i][0]) != tag) continue;
                        var newrow = _deldtldt.NewRow();
                        for (var j = 0; j < _dtl.Columns.Count; j++)
                        {
                            newrow[j] = _dtl.Rows[i][j];
                        }
                        _deldtldt.Rows.Add(newrow);
                    }
                }
                else
                {
                    for (var i = 0; i < _dtl.Rows.Count; i++)
                    {
                        if (Convert.ToInt32(_dtl.Rows[i][0]) != tag && Convert.ToInt32(_dtl.Rows[i][1]) != dtlid) continue;
                        var newrow = _deldtldt.NewRow();
                        for (var j = 0; j < _dtl.Columns.Count; j++)
                        {
                            newrow[j] = _dtl.Rows[i][j];
                        }
                        _deldtldt.Rows.Add(newrow);
                    }
                }
            }
        }

        /// <summary>
        /// 删除指定明细行=>作用:1)根据当前所选节点判断是否包含“-”,是;只针对_dtldt进行删除;反之,除删除外,将删除记录保存至_deldtldt内
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Tmdel_Click(object sender, EventArgs e)
        {
            try
            {
                //判断若没有选择节点就报异常
                if (tvview.SelectedNode == null) throw new Exception("没有选择任何节点,请选择再继续");
                //判断若点选的当前节点为ALL的话,就报异常
                if (tvview.SelectedNode.Text == $"ALL") throw new Exception("请点击除ALL节点外的节点进行删除明细行记录");
                if (gvdtl.SelectedRows.Count==0) throw new Exception("没有选择行,不能继续");
                if (_dtl.Rows.Count==0) throw new Exception("没有明细记录,不能进行删除");

                var clickMessage = $"您所选择需删除的行数为:{gvdtl.SelectedRows.Count}行 \n 是否继续?";

                if (MessageBox.Show(clickMessage, $"提示", MessageBoxButtons.YesNo, MessageBoxIcon.Information) == DialogResult.Yes)
                {
                    //若选择的当前节点的TAG不包含“-”就将所选记录保存至_deldtldt内
                    foreach (DataGridViewRow rows in gvdtl.SelectedRows)
                    {
                        InsertDelDtldt(Convert.ToInt32(tvview.SelectedNode.Tag),1,Convert.ToInt32(rows.Cells[1].Value));
                    }
                    //最后再执行删除操作
                    for (var i = _dtl.Rows.Count; i > 0; i++)
                    {
                        for (var j = 0; j < gvdtl.SelectedRows.Count; j++)
                        {
                            //需要_dtl.Groupid及Dtlid与所选的行相同才执行删除
                            if (Convert.ToInt32(_dtl.Rows[i - 1][0]) != Convert.ToInt32(tvview.SelectedNode.Tag) && 
                                Convert.ToInt32(_dtl.Rows[i - 1][1]) != Convert.ToInt32(gvdtl.SelectedRows[j].Cells[1].Value)) continue;
                                _dtl.Rows.RemoveAt(i-1);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, $"错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        /// <summary>
        /// 保存
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void TmSave_Click(object sender, EventArgs e)
        {
            try
            {
                task.TaskId = "2.4";
                task.Groupdt = _dt;              //表头信息 _dt
                task.Groupdtldt = _dtl;          //表体信息 _dtl
                task.Groupdeldt = _deldt;        //删除表头信息 _deldt
                task.Groupdeldtldt = _deldtldt;  //删除表体信息 _deldtldt

                new Thread(Start).Start();
                load.StartPosition = FormStartPosition.CenterScreen;
                load.ShowDialog();

                if (!task.ResultMark) throw new Exception("提交异常,请联系管理员");
                else
                {
                    MessageBox.Show($"用户组别保存成功,若需要用户权限创建请关闭此窗体", $"提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
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
                this.Close();
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
            if (gvdtl.Rows.Count > 0)
                gvdtl.Columns[0].Visible = false;
                gvdtl.Columns[1].Visible = false;
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


    }
}
