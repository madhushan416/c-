using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Windows.Forms;
using FireSharp.Config;
using FireSharp.Interfaces;
using FireSharp.Response;

namespace XZO_Fitness
{
    public partial class Form4 : Form
    {
        IFirebaseConfig config = new FirebaseConfig
        {
            AuthSecret = "J4itJhMeP8WX8eaTZPnRl9Ik0Lu8IYYZKuwbmW8D",
            BasePath = "https://test1-75790-default-rtdb.asia-southeast1.firebasedatabase.app/",
        };

        IFirebaseClient Client;
        private Image defaultImage;  // To store the default picture

        public Form4()
        {
            InitializeComponent();
        }

        public class counter_class
        {
            public string cnt { get; set; }
        }

        private async void Form4_Load(object sender, EventArgs e)
        {
            Client = new FireSharp.FirebaseClient(config);

            // Save the default image from pictureBox1 on form load
            defaultImage = pictureBox1.Image;
        }

        private async void button2_Click(object sender, EventArgs e)
        {
            FirebaseResponse resp = await Client.GetTaskAsync("counter/node");

            counter_class get;
            if (resp.Body == "null") // counter does not exist yet
            {
                get = new counter_class { cnt = "0" };
                await Client.SetTaskAsync("counter/node", get);
            }
            else
            {
                get = resp.ResultAs<counter_class>();
            }

            string gender = "";
            if (radioButton1.Checked)
                gender = "Male";
            else if (radioButton2.Checked)
                gender = "Female";
            else
            {
                MessageBox.Show("Please select a gender.");
                return;
            }

            if (pictureBox1.Image == null)
            {
                MessageBox.Show("Please select an image.");
                return;
            }

            MemoryStream ms = new MemoryStream();
            pictureBox1.Image.Save(ms, ImageFormat.Jpeg);
            byte[] imageBytes = ms.ToArray();
            string imageBase64 = Convert.ToBase64String(imageBytes);

            int nextId = Convert.ToInt32(get.cnt) + 1;

            var data = new Data
            {
                RegNumber = nextId.ToString("D3"),
                FName = textBox1.Text,
                LName = textBox2.Text,
                Email = textBox3.Text,
                Phone = textBox4.Text,
                Address = textBox6.Text,
                EmergencyContact = textBox5.Text,
                Gender = gender,
                DOB = dateTimePicker1.Value.ToString("yyyy-MM-dd"),
                ImageBase64 = imageBase64
            };

            // Save user data under root/information/{RegNumber}
            SetResponse response = await Client.SetTaskAsync("information/" + data.RegNumber, data);
            Data result = response.ResultAs<Data>();

            MessageBox.Show("Registration Successfully " + result.RegNumber);

            // Update counter node under root/counter/node
            var obj = new counter_class { cnt = data.RegNumber };
            await Client.SetTaskAsync("counter/node", obj);

            ClearForm();
            textBox1.Focus();
        }
        private void ClearForm()
        {
            textBox1.Clear();
            textBox2.Clear();
            textBox3.Clear();
            textBox4.Clear();
            textBox5.Clear();
            textBox6.Clear();
            textBox8.Clear();
            radioButton1.Checked = false;
            radioButton2.Checked = false;
            dateTimePicker1.Value = DateTime.Today;
            pictureBox1.Image = defaultImage; // Reset to original image
        }

    }
}
