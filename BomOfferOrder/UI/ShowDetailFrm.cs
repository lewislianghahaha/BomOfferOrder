using System;
using System.Data;
using System.Windows.Forms;

namespace BomOfferOrder.UI
{
    public partial class ShowDetailFrm : Form
    {
        

        public ShowDetailFrm()
        {
            InitializeComponent();
        }

        public void AddDbToFrm(int id)
        {
            textBox1.Text = Convert.ToString(id);
            gvdtl.DataSource = getdt(id);
        }

        private DataTable getdt(int id)
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
                        dr[0] = $"{id}";
                        dr[1] = "物料名称";
                        break;
                    case 1:
                        dr[0] = $"{id}";
                        dr[1] = "物料编码";
                        break;
                }
                dt.Rows.Add(dr);
            }
            return dt;
        }

        public int ReturnValue()
        {

            var a= textBox1.Text;
            return 0;
        }
    }
}
