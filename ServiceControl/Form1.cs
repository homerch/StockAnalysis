using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;

namespace ServiceControl
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void btnInit_Click(object sender, EventArgs e)
        {
            MainServiceReference.MainServiceSoapClient client = new MainServiceReference.MainServiceSoapClient();
            lbResult.Text = "...";
            lbResult.Text = client.RunInit();
        }

        private void btnNode_Click(object sender, EventArgs e)
        {
            NodeServiceReference.NodeServiceSoapClient client = new NodeServiceReference.NodeServiceSoapClient();
            lbResult.Text = "...";
            lbResult.Text = client.Run();
        }

        private void btnWorkInit_Click(object sender, EventArgs e)
        {
            MainServiceReference.MainServiceSoapClient client = new MainServiceReference.MainServiceSoapClient();
            lbResult.Text = "...";
            lbResult.Text = client.RunWorkInit();
        }

        private void btnExport_Click(object sender, EventArgs e)
        {
            KeepServiceReference.KeepServiceSoapClient client = new KeepServiceReference.KeepServiceSoapClient();
            lbResult.Text = "...";
            lbResult.Text = client.ExportCSV(dtpReceiveDate.Value.ToString("yyyy/MM/dd"));
        }

        private void btnSettlement_Click(object sender, EventArgs e)
        {
            //KeepServiceReference.KeepServiceSoapClient client = new KeepServiceReference.KeepServiceSoapClient();
            //lbResult.Text = "...";
            //lbResult.Text = client.Settlement(dtpReceiveDate.Value.ToString("yyyy/MM/dd"));
        }

        private void btnSplit_Click(object sender, EventArgs e)
        {
            int fileNo = 0;
            string line = "";
            if (openFileDialog1.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                using (StreamReader reader = new StreamReader(openFileDialog1.FileName))
                {
                    while ((line = reader.ReadLine()) != null)
                    {
                        using (StreamWriter writer = new StreamWriter(string.Format("{0}_{1}", openFileDialog1.FileName, fileNo)))
                        {
                            for (int i = 0; i < 30000; i++)
                            {
                                if ((line = reader.ReadLine()) != null)
                                {
                                    writer.WriteLine(line);
                                }
                                else {
                                    break;
                                }
                            }
                        }
                        fileNo++;
                    }
                }
            }
        }

        private void btnLite_Click(object sender, EventArgs e)
        {
            using (stockdbaEntitiesLite db = new stockdbaEntitiesLite())
            {
                db.DailyDetail.Add(new DailyDetail()
                {
                    receiveDate = DateTime.Now,
                    stockId = "9999",
                    no = 1,
                    brokerId = "xxxx"
                });

                db.SaveChanges();

                db.Configuration.ProxyCreationEnabled = false;
                var obj = (from a in db.DailyDetail.Where(o=>o.brokerId == "xxxx") select a).Take(100);
                dataGridView1.DataSource = obj.ToList();
            }
        }
    }
}
