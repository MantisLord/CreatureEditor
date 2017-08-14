using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace CreatureEditor
{
    public partial class dbLogin : Form
    {
        public dbLogin()
        {
            InitializeComponent();
        }

        private void btnConnect_Click(object sender, EventArgs e)
        {
            try
            {
                panel1.Visible = true;
                Refresh();

                frmMain frm = new frmMain();
                frm.load(txtServer.Text, txtDatabase.Text, txtUsername.Text, txtPassword.Text, txtPort.Text);
                frm.Show();

                panel1.Visible = false;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                panel1.Visible = false;
            }
        }
    }
}
