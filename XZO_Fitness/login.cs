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
    public partial class login : Form
    {
        public login()
        {
            InitializeComponent();
            textBox2.UseSystemPasswordChar = true;

        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (textBox1.Text == "admin" && textBox2.Text == "1234")
            {
                this.DialogResult = DialogResult.OK;
                this.Close(); // Closes the login form and allows Main() to continue
            }
            else
            {
                MessageBox.Show("Invalid login credentials.");
            }
        }
    }
}
