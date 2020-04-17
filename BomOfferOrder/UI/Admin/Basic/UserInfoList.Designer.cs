namespace BomOfferOrder.UI.Admin.Basic
{
    partial class UserInfoList
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(UserInfoList));
            this.tctotalpage = new System.Windows.Forms.TabControl();
            this.SuspendLayout();
            // 
            // tctotalpage
            // 
            this.tctotalpage.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tctotalpage.DrawMode = System.Windows.Forms.TabDrawMode.OwnerDrawFixed;
            this.tctotalpage.Location = new System.Drawing.Point(0, 0);
            this.tctotalpage.Name = "tctotalpage";
            this.tctotalpage.SelectedIndex = 0;
            this.tctotalpage.Size = new System.Drawing.Size(576, 301);
            this.tctotalpage.TabIndex = 4;
            // 
            // UserInfoList
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(576, 301);
            this.Controls.Add(this.tctotalpage);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "UserInfoList";
            this.Text = "用户信息列表";
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TabControl tctotalpage;
    }
}