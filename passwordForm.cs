using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace changeModelClient_Das
{
    public partial class passwordForm : Form
    {
        public passwordForm()
        {
            InitializeComponent();
            txt_Password.UseSystemPasswordChar = true;
        }

        private void txt_Password_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Escape) txt_Password.Text = string.Empty;
            if (e.KeyCode != Keys.Enter) return;
            if (txt_Password.Text.Trim() == "KZ")
            {
                mainForm.lockApp = true;
                this.Close();
            }
            else
            {
                MessageBox.Show("Password không chính xác!");
            }
        }
    }
}
