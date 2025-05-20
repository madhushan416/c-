using FireSharp.Config;
using FireSharp.Interfaces;
using FireSharp.Response;
using System;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace XZO_Fitness
{
    public partial class Form3 : Form
    {
        IFirebaseConfig config = new FirebaseConfig
        {
            AuthSecret = "J4itJhMeP8WX8eaTZPnRl9Ik0Lu8IYYZKuwbmW8D",
            BasePath = "https://test1-75790-default-rtdb.asia-southeast1.firebasedatabase.app/",
        };

        IFirebaseClient Client;

        public Form3()
        {
            InitializeComponent();
            loadform(new Form4());
            Client = new FireSharp.FirebaseClient(config);

            this.Load += Form3_Load;  // Hook form load event if not done via designer
        }
        public void loadform(object Form)
        {
            if (this.panel1.Controls.Count > 0)
                this.panel1.Controls.RemoveAt(0);
            Form f = Form as Form;
            f.TopLevel = false;
            f.Dock = DockStyle.Fill;
            this.panel1.Controls.Add(f);
            this.panel1.Tag = f;
            f.Show();

        }

        private async void Form3_Load(object sender, EventArgs e)
        {
            await LoadData();
        }

        private async void button4_Click(object sender, EventArgs e)
        {
            await LoadData();
        }

        private async Task LoadData()
        {
            try
            {
                FirebaseResponse response = await Client.GetTaskAsync("information");

                if (response.Body == "null")
                {
                    MessageBox.Show("No data found.");
                    dataGridView1.DataSource = null;
                    return;
                }

                var dataDict = response.ResultAs<Dictionary<string, Data>>();

                if (dataDict == null || dataDict.Count == 0)
                {
                    MessageBox.Show("No data available.");
                    dataGridView1.DataSource = null;
                    return;
                }

                DataTable dt = new DataTable();
                dt.Columns.Add("RegNumber");
                dt.Columns.Add("FName");
                dt.Columns.Add("LName");
                dt.Columns.Add("Email");
                dt.Columns.Add("Phone");
                dt.Columns.Add("Address");
                dt.Columns.Add("EmergencyContact");
                dt.Columns.Add("Gender");
                dt.Columns.Add("DOB");

                foreach (var item in dataDict.Values)
                {
                    dt.Rows.Add(
                        item.RegNumber ?? "",
                        item.FName ?? "",
                        item.LName ?? "",
                        item.Email ?? "",
                        item.Phone ?? "",
                        item.Address ?? "",
                        item.EmergencyContact ?? "",
                        item.Gender ?? "",
                        item.DOB ?? ""
                    );
                }

                dataGridView1.DataSource = dt;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error loading data: " + ex.Message);
            }
        }

        private void button7_Click(object sender, EventArgs e)
        {
            loadform(new Form4());
        }

        private void button6_Click(object sender, EventArgs e)
        {
            loadform(new Form5());
        }
    }
}
