using System;
using System.Threading;
using System.Windows.Forms;
using BomOfferOrder.Task;

namespace BomOfferOrder.UI
{
    public partial class ChangeAccount : Form
    {
        TaskLogic task=new TaskLogic();
        Load load=new Load();

        public ChangeAccount()
        {
            InitializeComponent();
            OnRegisterEvents();
            OnInitialize();
        }

        private void OnRegisterEvents()
        {
            btnconfirm.Click += Btnconfirm_Click;
            btncancel.Click += Btncancel_Click;
        }

        /// <summary>
        /// 初始化相关信息
        /// </summary>
        private void OnInitialize()
        {
            txtoldpwd.Text = GlobalClasscs.User.StrUsrpwd;
        }

        /// <summary>
        /// 确定
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Btnconfirm_Click(object sender, EventArgs e)
        {
            try
            {
                //判断所填的‘新密码’及‘确定新密码’是否为空(注:不能为空)
                if (txtnewpwd.Text == "" || txtnewpwdconfirm.Text == "") throw new Exception("请填写新密码(确定新密码),再继续");

                //判断新旧密码是否相同(注:不能相同)
                if(txtnewpwd.Text==txtoldpwd.Text)throw new Exception("新密码与旧密码不能相同,请重新填写后继续");

                //判断新密码与确定密码是否一致(注:要一致)
                if(txtnewpwd.Text!=txtnewpwdconfirm.Text)throw new Exception("新密码与确定新密码必须一致,请重新填写后继续");

                //判断所输入的密码个数是否>=6
                if(txtnewpwd.Text.Length<6)throw new Exception("所输入的新密码少于6位,请重新填写后继续");

                //提交
                var clickMessage = $"用户'{GlobalClasscs.User.StrUsrName}'可以更新密码 \n 是否继续?";
                if (MessageBox.Show(clickMessage, $"提示", MessageBoxButtons.YesNo, MessageBoxIcon.Information) == DialogResult.Yes)
                {
                    task.TaskId = "4.2";
                    task.Fid = GlobalClasscs.User.UserId;
                    task.Newpwd = txtnewpwd.Text;

                    new Thread(Start).Start();
                    load.StartPosition = FormStartPosition.CenterScreen;
                    load.ShowDialog();

                    if (!task.ResultMark) throw new Exception("更新发生异常,请联系管理员");
                    else
                    {
                        var message = $"用户'{GlobalClasscs.User.StrUsrName}' 已更新密码成功,请退出该修改工具,再重新进入";
                        MessageBox.Show(message, $"提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        btnconfirm.Enabled = false;
                        txtnewpwd.Text = "";
                        txtnewpwdconfirm.Text = "";
                    }
                }   
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, $"错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
                txtnewpwd.Text = "";
                txtnewpwdconfirm.Text = "";
            }
        }

        /// <summary>
        /// 退出
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Btncancel_Click(object sender, EventArgs e)
        {
            this.Close();
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
