using System;
using System.Data;
using System.Text.RegularExpressions;
using System.Threading;
using System.Windows.Forms;
using BomOfferOrder.DB;
using BomOfferOrder.Task;

namespace BomOfferOrder.UI.Admin
{
    public partial class AccountPerFrm : Form
    {
        DbList dbList=new DbList();
        TaskLogic task=new TaskLogic();
        Load load=new Load();

        #region 参数
        //记录用户权限记录是否保存
        private bool _saveid;
        //存放单据状态
        private string _funState;

        //存放节点跳转时的查询信息
        private DataTable _showdtldt;

        //保存‘用户’的USERID（读取时使用）
        private int _userid;
        //保存‘用户’信息DT（作用:1)读取时使用外部传过来的DT 2)收集创建的用户信息,保存时使用）
        private DataTable _userdt;
        //记录‘用户关联’表头信息 作用:1)读取时使用外部传过来的DT 2)收集创建的用户信息,保存时使用）
        private DataTable _reluserdt;
        //记录‘用户关联’表体信息 作用:1)读取时使用外部传过来的DT 2)收集创建的用户信息,保存时使用）
        private DataTable _reluserdtldt;
        //接收‘用户组别’表头DT
        private DataTable _usergroupdt;
        //接收‘用户组别’表体DT
        private DataTable _usergroupdtldt;


        //保存查询出来的GridView记录(用户组别表体信息)
        private DataTable _dtl;

        //记录当前页数(GridView页面跳转使用)
        private int _pageCurrent = 1;
        //记录计算出来的总页数(GridView页面跳转使用)
        private int _totalpagecount;
        //记录初始化标记(GridView页面跳转 初始化时使用)
        private bool _pageChange;
        #endregion


        public AccountPerFrm()
        {
            InitializeComponent();
            OnRegisterEvents();
        }

        private void OnRegisterEvents()
        {
            tmSave.Click += TmSave_Click;
            tmclose.Click += Tmclose_Click;
            cbnoneed.Click += Cbnoneed_Click;
            tvview.AfterCheck += Tvview_AfterCheck;
            tvview.AfterSelect += Tvview_AfterSelect;
            tmSet.Click += TmSet_Click;
            tmreback.Click += Tmreback_Click;

            bnMoveFirstItem.Click += BnMoveFirstItem_Click;
            bnMovePreviousItem.Click += BnMovePreviousItem_Click;
            bnMoveNextItem.Click += BnMoveNextItem_Click;
            bnMoveLastItem.Click += BnMoveLastItem_Click;
            bnPositionItem.Leave += BnPositionItem_Leave;
            tmshowrows.DropDownClosed += Tmshowrows_DropDownClosed;
            panel4.Visible = false;
        }

        /// <summary>
        /// 初始化
        /// </summary>
        /// <param name="funState">C:创建 R:读取</param>
        /// <param name="k3Name">K3用户名称</param>
        /// <param name="k3Group">K3用户组别</param>
        /// <param name="k3Phone">K3用户手机</param>
        /// <param name="userdt">用户信息DT(读取时使用)</param>
        public void OnInitialize(string funState, string k3Name, string k3Group, string k3Phone, DataTable userdt)
        {
            //读取单据状态
            _funState = funState;

            //清空树菜单信息
            tvview.Nodes.Clear();

            //接收‘用户组别’表头DT
            _usergroupdt = GlobalClasscs.Ad.UserGroupDt;

            //接收‘用户组别’表体DT
            _usergroupdtldt = GlobalClasscs.Ad.UserGroupDtlDt;

            //创建
            if (funState == "C")
            {
                //创建状态时将‘关联用户’相关表CLONE
                _reluserdt = GlobalClasscs.Ad.RelUserDt.Clone();
                _reluserdtldt = GlobalClasscs.Ad.RelUserDtlDt.Clone();
                //获取‘用户’临时表
                _userdt = dbList.CreateUserPermissionTemp();
            }
            //读取
            else if (funState=="R")
            {
                //将K3用户组别 以及 K3用户手机设置为不可见
                label3.Visible = false;
                txtGroup.Visible = false;
                label4.Visible = false;
                txtphone.Visible = false;

                //获取‘用户’DT
                _userdt = userdt.Copy();
                //获取‘关联用户’表头DT
                _reluserdt = GlobalClasscs.Ad.RelUserDt.Copy();
                //获取‘关联用户’表体DT
                _reluserdtldt = GlobalClasscs.Ad.RelUserDtlDt.Copy();
            }
            //将表体信息结构插入至_dtl内(初始化表结构)
            _dtl = _usergroupdtldt.Clone();
            //读取表头信息至对应的项内
            OnShow(funState,k3Name,k3Group,k3Phone,userdt);
            //树菜单读取
            ShowTreeList(funState,_usergroupdt,_reluserdt);
            //展开根节点
            tvview.ExpandAll();
            //连接GridView页面跳转功能
            LinkGridViewPageChange(MarkGridViewRecord(funState,_usergroupdtldt,_reluserdtldt));
            //控制GridView单元格显示方式
            ControlGridViewisShow();
            //初始化保存标记为FALSE
            _saveid = false;
        }

        /// <summary>
        /// 读取表头信息
        /// (R状态时,注:若检测到userdt.UserRelid=0,即将树菜单及GridView设置为不可用)
        /// </summary>
        private void OnShow(string funState, string k3Name, string k3Group, string k3Phone, DataTable sourcedt)
        {
            if (funState == "C")
            {
                txtusername.Text = k3Name;
                txtGroup.Text = k3Group;
                txtphone.Text = k3Phone;
            }
            else
            {
                _userid = Convert.ToInt32(sourcedt.Rows[0][0]);                    //UserId
                txtusername.Text = Convert.ToString(sourcedt.Rows[0][1]);          //用户名称
                cbapplyid.Checked = Convert.ToInt32(sourcedt.Rows[0][5]) == 0;     //是否启用
                cbbackconfirm.Checked = Convert.ToInt32(sourcedt.Rows[0][6]) == 0; //能否反审核
                cbreadid.Checked = Convert.ToInt32(sourcedt.Rows[0][7]) == 0;      //能否查阅明细金额
                cbaddid.Checked = Convert.ToInt32(sourcedt.Rows[0][8]) == 0;       //能否对明细物料操作
                cbnoneed.Checked = Convert.ToInt32(sourcedt.Rows[0][10]) == 0;     //是否关联用户

                //若是否关联用户为0(是),即将TreeView及GridView设置为不启用
                if (cbnoneed.Checked)
                {
                    splitContainer1.Enabled = false;
                }
            }
        }

        /// <summary>
        /// 控制GridView单元格显示方式
        /// </summary>
        private void ControlGridViewisShow()
        {
            //注:当没有值时,若还设置某一行Row不显示的话,就会出现异常
            if (gvdtl.Rows.Count >= 0)
            {
                gvdtl.Columns[0].Visible = false;
                gvdtl.Columns[1].Visible = false;
            }
        }

        /// <summary>
        /// 整合数据至GridView内
        /// </summary>
        /// <param name="funState"></param>
        /// <param name="usergroupdtldt"></param>
        /// <param name="reluserdtldt"></param>
        /// <returns></returns>
        private DataTable MarkGridViewRecord(string funState,DataTable usergroupdtldt,DataTable reluserdtldt)
        {
            var resultdt=new DataTable();
            //若为C时,直接将usergroupdtldt返回至resultdt
            if (funState == "C")
            {
                resultdt = usergroupdtldt.Copy();
            }
            //若为R时,将usergroupdtldt与reluserdtldt合并显示
            //若usergroupdtldt的Groupid 与 Dtlid在reluserdtldt对应的项内存在,即将T_BD_UserGroupDtl最后一项设置为‘是’
            else
            {
                for (var i = 0; i < usergroupdtldt.Rows.Count; i++)
                {
                    var rowslength = reluserdtldt.Select("Userid='"+ _userid + "' and Groupid='"+usergroupdtldt.Rows[i][0]+"' and Dtlid='"+usergroupdtldt.Rows[i][1]+"'").Length;
                    if (rowslength > 0)
                    {
                        usergroupdtldt.Rows[i].BeginEdit();
                        usergroupdtldt.Rows[i][7] = "是";
                        usergroupdtldt.Rows[i].EndEdit();
                    }
                }
                resultdt = usergroupdtldt.Copy();
            }
            return resultdt;
        }

        /// <summary>
        /// 树菜单读取
        /// </summary>
        /// <param name="usergroupdt">用户组别DT</param>
        /// <param name="funState">C:创建 R:读取</param>
        /// <param name="reluserdt">用户关联DT</param>
        private void ShowTreeList(string funState,DataTable usergroupdt,DataTable reluserdt)
        {
            //清空树菜单信息
            tvview.Nodes.Clear();

            if (usergroupdt.Rows.Count == 0)
            {
                var tree = new TreeNode();
                tree.Tag = 0;
                tree.Text = $"ALL";
                tvview.Nodes.Add(tree);
            }
            else
            {
                //若节点总数等于0时就创建ALL节点
                if (tvview.Nodes.Count == 0)
                {
                    var tree = new TreeNode();
                    tree.Tag = 0;
                    tree.Text = $"ALL";
                    tvview.Nodes.Add(tree);
                }
                //读取记录并增加至子节点内(从二级节点开始)
                AddChildNode(funState,tvview, usergroupdt,reluserdt);
                //展开根节点
                tvview.ExpandAll();
            }
        }

        /// <summary>
        /// 子节点菜单相关内容读取
        /// </summary>
        /// <param name="funState"></param>
        /// <param name="tvView"></param>
        /// <param name="usergroupdt">用户组别DT</param>
        /// <param name="reluserdt">用户关联DT</param>
        private void AddChildNode(string funState, TreeView tvView, DataTable usergroupdt,DataTable reluserdt)
        {
            var tnode = tvView.Nodes[0];
            var rows = usergroupdt.Select("Parentid='" + Convert.ToInt32(tnode.Tag) + "'");

            //若为C状态时,即直接读取usergroupdt
            if (funState == "C")
            {
                //循环获取子节点信息及进行添加
                foreach (var r in rows)
                {
                    var tn = new TreeNode();
                    tn.Tag = Convert.ToInt32(r[0]);     //自身主键ID
                    tn.Text = Convert.ToString(r[2]);   //节点内容
                    tnode.Nodes.Add(tn);                //将二级节点添加至根节点下
                }
            }
            //若为R状态时,使用_userid及_Groupid放到reluserdt内进行查询;若有就为“选中”状态
            else
            {
                foreach (var r in rows)
                {
                    var tn = new TreeNode();
                    tn.Tag = Convert.ToInt32(r[0]);     //自身主键ID
                    tn.Text = Convert.ToString(r[2]);   //节点内容
                    var rowslength = reluserdt.Select("Userid='" + _userid + "' and Groupid='" + Convert.ToInt32(r[0]) + "'");
                    if (rowslength.Length > 0)
                        tn.Checked = true;
                    tnode.Nodes.Add(tn);
                }
            }
        }

        /// <summary>
        /// 连接GridView页面跳转
        /// 注:_dtl是保存所有明细记录的集合DT
        /// </summary>
        /// <param name="dt"></param>
        private void LinkGridViewPageChange(DataTable dt)
        {
            if (dt.Rows.Count > 0)
            {
                //初始化时才将dt赋值给_dtl
                if (_dtl.Rows.Count == 0)
                {
                    _dtl = dt.Copy();
                    //复制表结构
                    _showdtldt = dt.Clone();
                }
                panel4.Visible = true;
                //初始化下拉框所选择的默认值
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
                panel4.Visible = false;
            }
        }

        /// <summary>
        /// 点击树菜单复选框后执行
        /// 作用:当点击某个节点时,GridView显示相关的记录
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Tvview_AfterCheck(object sender, TreeViewEventArgs e)
        {
            try
            {
                //若点选ALL节点,即其它子节点也会勾选
                if (Convert.ToInt32(e.Node.Tag) == 0)
                {
                    SetChild(e.Node);
                }
                //跳转显示内容
                JustPageShow(Convert.ToInt32(e.Node.Tag));
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, $"错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
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
            try
            {
                //跳转显示内容
                JustPageShow(Convert.ToInt32(e.Node.Tag));
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, $"错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        /// <summary>
        /// 根据父节点的‘复选框’是否选中,确定对应的子节点是否选中
        /// 仅适用于点击父节点‘复选框’时
        /// </summary>
        private void SetChild(TreeNode node)
        {
            //获取当前父节点的‘复选框’是否选中
            var check = node.Checked;

            foreach (TreeNode childNode in node.Nodes)
            {
                //根据父节点的‘复选框’是否选中,确定对应的子节点是否选中
                childNode.Checked = check;
            }
        }

        /// <summary>
        /// 根据tag为条件,跳转显示对应的内容至GridView内
        /// </summary>
        /// <param name="tag"></param>
        private void JustPageShow(int tag)
        {
            try
            {
                //复制表结构
                _showdtldt = _dtl.Clone();

                //若点击"ALL"节点,即在GridView内显示全部信息;而其它节点需使用当前所选的节点.Tag为条件
                if (tag == 0)
                {
                    _showdtldt = _dtl.Copy();
                }
                else
                {
                    //以tag为条件进行对_dtl查询,并将结果插入至_showdtldt内
                    var dtlrows = _dtl.Select("Groupid='" + tag + "'");

                    if (dtlrows.Length > 0)
                    {
                        for (var i = 0; i < dtlrows.Length; i++)
                        {
                            var newrow = _showdtldt.NewRow();
                            for (var j = 0; j < _showdtldt.Columns.Count; j++)
                            {
                                newrow[j] = dtlrows[i][j];
                            }
                            _showdtldt.Rows.Add(newrow);
                        }
                    }
                }
                //刷新信息
                LinkGridViewPageChange(_showdtldt);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, $"错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        /// <summary>
        /// 设置不需要用户关联
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Cbnoneed_Click(object sender, EventArgs e)
        {
            try
            {
                //若勾选,即令树菜单及GridView设置为不可用;反之,设置为可用
                splitContainer1.Enabled = !cbnoneed.Checked;
                //将树菜单去掉勾中=>先将父节点的复选框设置check=false,再调用SetChild()方法,最后执行JustPageShow()跳转
                tvview.Nodes[0].Checked = false;
                SetChild(tvview.Nodes[0]);
                JustPageShow(0);
                //将_dtl内7项都清空
                DelGridViewRecord();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, $"错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        /// <summary>
        /// 清空_dtl 7项
        /// </summary>
        private void DelGridViewRecord()
        {
            //获取所选中行中 Groupid Dtlid对应的值
            for (var i = 0; i < _dtl.Rows.Count; i++)
            {
                    _dtl.Rows[i].BeginEdit();
                    _dtl.Rows[i][7] = "";
                    _dtl.Rows[i].EndEdit();
            }
        }

        /// <summary>
        /// 设置明细项不启用
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void TmSet_Click(object sender, EventArgs e)
        {
            try
            {
                if (gvdtl.SelectedRows.Count == 0) throw new Exception("没有选中行,请选择后再继续");
                //当前用户不能设置自身“不启用”
                foreach (DataGridViewRow row in gvdtl.SelectedRows)
                {
                    if(Convert.ToString(row.Cells[2].Value) == txtusername.Text)throw new Exception($"用户'{txtusername.Text}'不能设置自身不启用,请重新选择.");
                    break;
                }

                //循环将所选中的行=>将7列设置为"是"(更新_dtl)
                foreach (DataGridViewRow row in gvdtl.SelectedRows)
                {

                    //获取所选中行中 Groupid Dtlid对应的值
                    for (var i = 0; i < _dtl.Rows.Count; i++)
                    {
                        if (Convert.ToInt32(_dtl.Rows[i][0]) == Convert.ToInt32(row.Cells[0].Value) &&
                            Convert.ToInt32(_dtl.Rows[i][1]) == Convert.ToInt32(row.Cells[1].Value))
                        {
                            _dtl.Rows[i].BeginEdit();
                            _dtl.Rows[i][7] = "是";
                            _dtl.Rows[i].EndEdit();
                        }
                    }
                }
                //根据当前选择节点进行刷新
                JustPageShow(Convert.ToInt32(tvview.SelectedNode.Tag));
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, $"错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        /// <summary>
        /// 取消不启用
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Tmreback_Click(object sender, EventArgs e)
        {
            try
            {
                if (gvdtl.SelectedRows.Count == 0) throw new Exception("没有选中行,请选择后再继续");
                //检测若所选择的行中没有设置“不启用”,会报异常不能继续
                foreach (DataGridViewRow row in gvdtl.SelectedRows)
                {
                    if(!Convert.ToString(row.Cells[7].Value).Contains("是")) throw new Exception($"检测员工名称'{row.Cells[2].Value}'没有设置不启用,故不能执行取消操作");
                    break;
                }
                //循环将_dtl.rows[7]取消"是"
                foreach (DataGridViewRow row in gvdtl.SelectedRows)
                {
                    //获取所选中行中 Groupid Dtlid对应的值
                    for (var i = 0; i < _dtl.Rows.Count; i++)
                    {
                        if (Convert.ToInt32(_dtl.Rows[i][0]) == Convert.ToInt32(row.Cells[0].Value) &&
                            Convert.ToInt32(_dtl.Rows[i][1]) == Convert.ToInt32(row.Cells[1].Value))
                        {
                              _dtl.Rows[i].BeginEdit();
                              _dtl.Rows[i][7] = "";
                              _dtl.Rows[i].EndEdit();
                        }

                    }
                }
                //根据当前选择节点进行刷新
                JustPageShow(Convert.ToInt32(tvview.SelectedNode.Tag));
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, $"错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        /// <summary>
        /// 保存
        /// 当保存时,将所勾选的组别信息 以及 不启用的明细记录进行记录;最后提交
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void TmSave_Click(object sender, EventArgs e)
        {
            try
            {
                //收集‘用户’信息
                var userdt = CreateUserDt();

                //收集‘用户关联’表头信息
                var reluserdt = CreateRelUserDt();

                //收集‘用户关联’表体信息
                var reluserdtldt = CreateRelUserDtlDt();

                //执行提交
                task.TaskId = "2.1";
                task.Userdt = userdt;
                task.Reluserdt = reluserdt;
                task.Reluserdtldt = reluserdtldt;

                new Thread(Start).Start();
                load.StartPosition = FormStartPosition.CenterScreen;
                load.ShowDialog();

                if (!task.ResultMark) throw new Exception("提交异常,请联系管理员");
                else
                {
                    MessageBox.Show($"用户'{txtusername.Text}'权限创建成功,可关闭此权限窗体", $"提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    tmSave.Enabled = false;
                    _saveid = true;
                    //保存后将所有控件都设置为不能操作
                    groupBox2.Enabled = false;
                    panel3.Enabled = false;
                    splitContainer1.Enabled = false;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, $"错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        /// <summary>
        /// 收集‘用户’信息
        /// </summary>
        /// <returns></returns>
        private DataTable CreateUserDt()
        {
            var tempdt = _userdt.Clone();
            var newrow = tempdt.NewRow();
            newrow[0] = _funState == "C" ? 0 : _userid;                             //Useid
            newrow[1] = txtusername.Text;                                           //用户名称
            newrow[2] = _funState == "C" ? "888888" : "";                           //用户密码
            newrow[3] = _funState == "C" ? GlobalClasscs.User.StrUsrName : "";      //创建人
            newrow[4] = _funState == "C" ? DateTime.Now : Convert.ToDateTime(null); //创建日期
            newrow[5] = cbapplyid.Checked ? 0 : 1;                                  //是否启用
            newrow[6] = cbbackconfirm.Checked ? 0 : 1;                              //能否反审核
            newrow[7] = cbreadid.Checked ? 0 : 1;                                   //能否查阅明细金额
            newrow[8] = cbaddid.Checked ? 0 : 1;                                    //能否对明细物料操作
            newrow[9] = 1;                                                          //是否占用
            newrow[10] = cbnoneed.Checked ? 0 : 1;                                  //是否不关联用户
            tempdt.Rows.Add(newrow);
            return tempdt;
        }

        /// <summary>
        /// 收集‘用户关联’表头信息
        /// 循环树菜单,并记录已选择的节点
        /// </summary>
        /// <returns></returns>
        private DataTable CreateRelUserDt()
        {
            var tempdt = _reluserdt.Clone();
            //循环树菜单中‘父节点’下的各节点
            foreach (TreeNode childNode in tvview.Nodes[0].Nodes)
            {
                if (!childNode.Checked) continue;
                var newrow = tempdt.NewRow();
                newrow[0] = _funState == "C" ? 0 : _userid; //Userid
                newrow[1] = childNode.Tag;                  //GroupID
                newrow[2] = DateTime.Now;                   //CreateDt
                tempdt.Rows.Add(newrow);
            }
            return tempdt;
        }

        /// <summary>
        /// 收集‘用户关联’表体信息
        /// 循环_dtl,收集将设置为“是”的记录
        /// </summary>
        /// <returns></returns>
        private DataTable CreateRelUserDtlDt()
        {
            var tempdt = _reluserdtldt.Clone();
            //循环_dtl
            foreach (DataRow rows in _dtl.Rows)
            {
                if (!Convert.ToString(rows[7]).Contains("是")) continue;
                var newrow = tempdt.NewRow();
                newrow[0] = _funState == "C" ? 0 : _userid; //Userid
                newrow[1] = rows[0];                        //Groupid
                newrow[2] = rows[1];                        //Dtlid
                newrow[3] = DateTime.Now;                   //CreateDt
                tempdt.Rows.Add(newrow);
            }
            return tempdt;
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
                var clickMessage = !_saveid ? $"是否退出? \n 注:没保存的记录退出后将会消失" : "是否退出?";

                if (MessageBox.Show(clickMessage, $"提示", MessageBoxButtons.YesNo, MessageBoxIcon.Information) == DialogResult.Yes)
                {
                    this.Close();
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
                //判断若_showdtldt有值时就将记录赋给resultdt内,反之使用_dtl
                var resultdt = _showdtldt.Rows.Count > 0 ? _showdtldt : _dtl;

                //获取查询的总行数
                var dtltotalrows = resultdt.Rows.Count;    //_dtl.Rows.Count;
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
                var tempdt = resultdt.Clone();//_dtl.Clone();
                //循环将所需的_dtl的行记录复制至临时表内
                for (var i = startrow; i < endrow; i++)
                {
                    tempdt.ImportRow(resultdt.Rows[i]);
                    //tempdt.ImportRow(_dtl.Rows[i]);
                }

                //最后将刷新的DT重新赋值给GridView
                gvdtl.DataSource = tempdt;
                //将“当前页”赋值给"跳转页"文本框内
                bnPositionItem.Text = Convert.ToString(_pageCurrent);
                //最后将_showdtldt清空
                if (_showdtldt?.Rows.Count > 0)
                {
                    _showdtldt.Rows.Clear();
                    _showdtldt.Columns.Clear();
                }
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
