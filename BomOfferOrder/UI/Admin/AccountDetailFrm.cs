using System;
using System.Data;
using System.Threading;
using System.Windows.Forms;
using BomOfferOrder.DB;
using BomOfferOrder.Task;

namespace BomOfferOrder.UI.Admin
{
    public partial class AccountDetailFrm : Form
    {
        TaskLogic task=new TaskLogic();
        Load load=new Load();
        DbList dbList=new DbList();

        #region 变量参数
        //记录用户权限记录是否保存
        private bool _saveid;
        #endregion

        public AccountDetailFrm()
        {
            InitializeComponent();
            OnRegisterEvents();
        }

        private void OnRegisterEvents()
        {
            tmSave.Click += TmSave_Click;
            tmclose.Click += Tmclose_Click;
        }

        /// <summary>
        /// 初始化相关记录
        /// </summary>
        public void OnInitialize(string k3Name,string k3Group,string k3Phone)
        {
            txtusername.Text = k3Name;
            txtGroup.Text = k3Group;
            txtphone.Text = k3Phone;
            _saveid = false;
        }

        /// <summary>
        /// 保存
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void TmSave_Click(object sender, EventArgs e)
        {
            task.TaskId = "2.1";
            task.Importdt = CreateTempIntoDt();

            new Thread(Start).Start();
            load.StartPosition = FormStartPosition.CenterScreen;
            load.ShowDialog();

            if (!task.ResultMark) throw new Exception("提交异常,请联系管理员");
            else
            {
                MessageBox.Show($"用户'{txtusername.Text}'权限创建成功,可关闭此权限窗体", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                tmSave.Enabled = false;
                _saveid = true;
            }
        }

        /// <summary>
        /// 关闭
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Tmclose_Click(object sender, EventArgs e)
        {
            var clickMessage = !_saveid ? $"是否退出? \n 注:没保存的记录退出后将会消失" : "是否退出?";
            
            if (MessageBox.Show(clickMessage, "提示", MessageBoxButtons.YesNo, MessageBoxIcon.Information) == DialogResult.Yes)
            {
                this.Close();
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
        /// 将用户填写的内容生成DT
        /// </summary>
        /// <returns></returns>
        private DataTable CreateTempIntoDt()
        {
            //获取用户权限临时表
            var tempdt = dbList.CreateUserPermissionTemp();

            var newrow = tempdt.NewRow();
            newrow[0] = DBNull.Value;                     //UseId
            newrow[1] = txtusername.Text ;                //用户名称
            newrow[2] = "888888";                         //用户密码
            newrow[3] = GlobalClasscs.User.StrUsrName;    //创建人
            newrow[4] = DateTime.Now;                     //创建日期
            newrow[5] = cbapplyid.Checked ? 0 : 1;        //是否启用
            newrow[6] = cbbackconfirm.Checked ? 0 : 1;    //能否反审核
            newrow[7] = cbreadid.Checked ? 0 : 1;         //能否查阅明细金额
            newrow[8] = cbaddid.Checked ? 0 : 1;          //能否对明细物料操作
            newrow[9] = 1;                                //是否占用
            tempdt.Rows.Add(newrow);
            return tempdt;
        }

    }
}
