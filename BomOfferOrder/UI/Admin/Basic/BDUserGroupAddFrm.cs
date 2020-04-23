using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace BomOfferOrder.UI.Admin.Basic
{
    public partial class BdUserGroupAddFrm : Form
    {
        #region 参数
        private string _value;
        #endregion

        #region Get
        public string ResultValue => _value;
        #endregion

        public BdUserGroupAddFrm()
        {
            InitializeComponent();
            OnRegisterEvents();
        }

        private void OnRegisterEvents()
        {
            tmSet.Click += TmSet_Click;
            tmClose.Click += TmClose_Click;
        }

        /// <summary>
        /// 设置
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void TmSet_Click(object sender, EventArgs e)
        {
            try
            {
                //判断若文本框没有填写记录,出现异常
                if(string.IsNullOrEmpty(txtvalue.Text)) throw new Exception("请填写用户组别名称后再继续.");
                //将相关值赋给_value变量内进行返回
                _value = txtvalue.Text;
                //结束后将对应的文本框清空
                txtvalue.Text = "";
                this.Close();
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
        private void TmClose_Click(object sender, EventArgs e)
        {
            try
            {
                //关闭前将文本框清空
                txtvalue.Text = "";
                this.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, $"错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}
