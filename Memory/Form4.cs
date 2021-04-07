using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Memory
{
    public partial class Form4 : Form
    {
        public Form4()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            this.Hide();
            Form1 f1 = new Form1();
            f1.ShowDialog();
            this.Close();
        }

        private void Form4_Load(object sender, EventArgs e)
        {
            var users = GetUserList();
            listView1.Items.Clear();
            foreach (var user in users)
            {
                var row = new string[] { user.Name, user.Rate.ToString() };
                var lvi = new ListViewItem(row);
                lvi.Tag = user;
                listView1.Items.Add(lvi);
            }
        }

        private List<User> GetUserList()
        {
            var list = new List<User>();
            list.Add(new User()
            {
                Name = "Tester1",
                Rate = 500
            });
            list.Add(new User()
            {
                Name = "Tester2",
                Rate = 860
            });
            list.Add(new User()
            {
                Name = "Tester3",
                Rate = 123
            });
            return list;
        }

        private void listView1_SelectedIndexChanged(object sender, EventArgs e)
        {

        }
    }
}
