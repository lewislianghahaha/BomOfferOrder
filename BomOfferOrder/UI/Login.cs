using System;
using System.Windows.Forms;

namespace BomOfferOrder.UI
{
    public partial class Login : Form
    {
        public Login()
        {
            InitializeComponent();
            button1.Click += Button1_Click;
        }

        private void Button1_Click(object sender, EventArgs e)
        {
            var main = new Main();
            main.ShowDialog();

            //this.DialogResult = DialogResult.OK;
            //this.Close();
        }
    }
}
