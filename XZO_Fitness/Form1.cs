using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace XZO_Fitness
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
            loadform(new Form2());
        }

        public void loadform(object Form)
        {
            if (this.panel3.Controls.Count > 0)
                this.panel3.Controls.RemoveAt(0);
            Form f = Form as Form;
            f.TopLevel = false;
            f.Dock = DockStyle.Fill;
            this.panel3.Controls.Add(f);
            this.panel3.Tag = f;
            f.Show();

        }


        private void button2_Click(object sender, EventArgs e)
        {
            loadform(new Form3());
        }

        private void button1_Click(object sender, EventArgs e)
        {
            loadform(new Form2());
        }

        private void button3_Click(object sender, EventArgs e)
        {
            
        }
    }
}
