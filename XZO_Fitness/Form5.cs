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
    public partial class Form5 : Form
    {
        IFirebaseConfig config = new FirebaseConfig
        {
            AuthSecret = "J4itJhMeP8WX8eaTZPnRl9Ik0Lu8IYYZKuwbmW8D",
            BasePath = "https://test1-75790-default-rtdb.asia-southeast1.firebasedatabase.app/",
        };

        IFirebaseClient Client;
        private Image defaultImage;  // To store the default picture

        public Form5()
        {
            InitializeComponent();
            Client = new FireSharp.FirebaseClient(config);
        }

        private void Form5_Load(object sender, EventArgs e)
        {
            // Save default image (assuming pictureBox1 exists on form)
            defaultImage = pictureBox1.Image;
        }

        // Insert or Update Data (Button 2)
        private async void button2_Click(object sender, EventArgs e)
        {
            try
            {
                // Get current counter value
                FirebaseResponse resp = await Client.GetTaskAsync("counter/node");

                counter_class get;
                if (resp.Body == "null")
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

                int nextId;

                // Determine if this is an update or new insert
                if (string.IsNullOrWhiteSpace(textBox8.Text))
                {
                    nextId = Convert.ToInt32(get.cnt) + 1;
                }
                else
                {
                    if (!int.TryParse(textBox8.Text.Trim(), out nextId))
                    {
                        MessageBox.Show("Invalid Registration Number.");
                        return;
                    }
                }

                string regNumberStr = nextId.ToString("D3");

                var data = new Data
                {
                    RegNumber = regNumberStr,
                    FName = textBox1.Text.Trim(),
                    LName = textBox2.Text.Trim(),
                    Email = textBox3.Text.Trim(),
                    Phone = textBox4.Text.Trim(),
                    Address = textBox6.Text.Trim(),
                    EmergencyContact = textBox5.Text.Trim(),
                    Gender = gender,
                    DOB = dateTimePicker1.Value.ToString("yyyy-MM-dd"),
                    ImageBase64 = imageBase64
                };

                // Save or update to Firebase
                SetResponse response = await Client.SetTaskAsync("information/" + regNumberStr, data);
                Data result = response.ResultAs<Data>();

                // If new record, update counter node
                if (string.IsNullOrWhiteSpace(textBox8.Text))
                {
                    var obj = new counter_class { cnt = regNumberStr };
                    await Client.SetTaskAsync("counter/node", obj);
                }

                MessageBox.Show("Data saved successfully with RegNumber: " + regNumberStr);

                ClearForm();
                textBox8.Text = regNumberStr;  // Show assigned RegNumber
                textBox8.Focus();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error saving data: " + ex.Message);
            }
        }

        // Retrieve Data (Button 4)
        private async void button4_Click(object sender, EventArgs e)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(textBox8.Text))
                {
                    MessageBox.Show("Please enter a Registration Number to search.");
                    return;
                }

                FirebaseResponse response = await Client.GetTaskAsync("information/" + textBox8.Text.Trim());
                Data obj = response.ResultAs<Data>();

                if (obj == null)
                {
                    MessageBox.Show("No data found for RegNumber: " + textBox8.Text);
                    return;
                }

                // Populate UI controls
                textBox8.Text = obj.RegNumber;
                textBox1.Text = obj.FName;
                textBox2.Text = obj.LName;
                textBox3.Text = obj.Email;
                textBox4.Text = obj.Phone;
                textBox5.Text = obj.EmergencyContact;
                textBox6.Text = obj.Address;

                if (obj.Gender == "Male")
                {
                    radioButton1.Checked = true;
                    radioButton2.Checked = false;
                }
                else if (obj.Gender == "Female")
                {
                    radioButton2.Checked = true;
                    radioButton1.Checked = false;
                }
                else
                {
                    radioButton1.Checked = false;
                    radioButton2.Checked = false;
                }

                if (DateTime.TryParse(obj.DOB, out DateTime dob))
                {
                    dateTimePicker1.Value = dob;
                }

                if (!string.IsNullOrEmpty(obj.ImageBase64))
                {
                    byte[] imageBytes = Convert.FromBase64String(obj.ImageBase64);
                    using (MemoryStream ms = new MemoryStream(imageBytes))
                    {
                        pictureBox1.Image = Image.FromStream(ms);
                    }
                }
                else
                {
                    pictureBox1.Image = defaultImage;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error retrieving data: " + ex.Message);
            }
        }

        // Delete Data (Button 3)
        private async void button3_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(textBox8.Text))
            {
                MessageBox.Show("Please enter a Registration Number to delete.");
                return;
            }

            var confirmResult = MessageBox.Show($"Are you sure you want to delete record with RegNumber {textBox8.Text}?",
                                     "Confirm Delete",
                                     MessageBoxButtons.YesNo);

            if (confirmResult == DialogResult.Yes)
            {
                try
                {
                    await Client.DeleteTaskAsync("information/" + textBox8.Text.Trim());
                    MessageBox.Show("Data deleted successfully.");
                    ClearForm();
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error deleting data: " + ex.Message);
                }
            }
        }

        private void ClearForm()
        {
            textBox1.Clear();
            textBox2.Clear();
            textBox3.Clear();
            textBox4.Clear();
            textBox5.Clear();
            textBox6.Clear();
            // clear additional textboxes if any
            radioButton1.Checked = false;
            radioButton2.Checked = false;
            dateTimePicker1.Value = DateTime.Today;
            pictureBox1.Image = defaultImage;
        }

        private async void button2_Click_1(object sender, EventArgs e)
        {
            try
            {
                FirebaseResponse resp = await Client.GetTaskAsync("counter/node");

                counter_class get;
                if (resp.Body == "null")
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

                int nextId;

                bool isNewRecord = string.IsNullOrWhiteSpace(textBox8.Text);

                if (isNewRecord)
                {
                    nextId = Convert.ToInt32(get.cnt) + 1;
                }
                else
                {
                    if (!int.TryParse(textBox8.Text.Trim(), out nextId))
                    {
                        MessageBox.Show("Invalid Registration Number.");
                        return;
                    }
                }

                string regNumberStr = nextId.ToString("D3");

                var data = new Data
                {
                    RegNumber = regNumberStr,
                    FName = textBox1.Text.Trim(),
                    LName = textBox2.Text.Trim(),
                    Email = textBox3.Text.Trim(),
                    Phone = textBox4.Text.Trim(),
                    Address = textBox6.Text.Trim(),
                    EmergencyContact = textBox5.Text.Trim(),
                    Gender = gender,
                    DOB = dateTimePicker1.Value.ToString("yyyy-MM-dd"),
                    ImageBase64 = imageBase64
                };

                // Save or update to Firebase
                SetResponse response = await Client.SetTaskAsync("information/" + regNumberStr, data);
                Data result = response.ResultAs<Data>();

                // Update counter node only if new record
                if (isNewRecord)
                {
                    var obj = new counter_class { cnt = regNumberStr };
                    await Client.SetTaskAsync("counter/node", obj);
                }

                MessageBox.Show("Data saved successfully with RegNumber: " + regNumberStr);

                // Update RegNumber textbox so subsequent update/delete work on same record
                textBox8.Text = regNumberStr;

                ClearForm();

                textBox8.Focus();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error saving data: " + ex.Message);
            }
        }
    }
}
