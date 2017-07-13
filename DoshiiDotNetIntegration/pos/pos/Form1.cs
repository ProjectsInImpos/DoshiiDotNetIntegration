using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using pos.Helpers;

namespace pos
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
            DataPersistanceHelper.RetreiveDataFromStore();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            DataPersistanceHelper.SaveDataToStore();
            Application.Exit();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            
            LiveData.PosSettings.BaseUrl = txt_baseUrl.Text;
            LiveData.PosSettings.socketUrl = txt_socketUrl.Text;
            LiveData.PosSettings.secretKey = txt_secretKey.Text;
            LiveData.PosSettings.doshiiVendor = txt_doshiiVendor.Text;
            LiveData.PosSettings.locationToken = txt_locationToken.Text;
            LiveData.PosSettings.OrganisationId = textBox1.Text;
            int socketTimeOutSec = 600;
            if (int.TryParse(txt_timeoutSec.Text, out socketTimeOutSec))
            {
                LiveData.PosSettings.socketTimeOutSec = socketTimeOutSec;
            }
            else
            {
                LiveData.PosSettings.socketTimeOutSec = 600;
            }
            LiveData.PosSettings.UseApps = chk_apps.Checked;
            LiveData.PosSettings.UseMembership = chk_membership.Checked;
            LiveData.PosSettings.UseReservations = chk_reservations.Checked;
            LiveData.PosSettings.UseSocketConnection = chk_socketConnection.Checked;

            PosFunctions posForm = new PosFunctions();
            posForm.ShowDialog();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            txt_baseUrl.Text = LiveData.PosSettings.BaseUrl;
            txt_doshiiVendor.Text = LiveData.PosSettings.doshiiVendor;
            txt_locationToken.Text = LiveData.PosSettings.locationToken;
            txt_secretKey.Text = LiveData.PosSettings.secretKey;
            txt_socketUrl.Text = LiveData.PosSettings.socketUrl;
            txt_timeoutSec.Text = LiveData.PosSettings.socketTimeOutSec.ToString();
            chk_apps.Checked = LiveData.PosSettings.UseApps;
            chk_membership.Checked = LiveData.PosSettings.UseMembership;
            chk_reservations.Checked = LiveData.PosSettings.UseReservations;
            chk_socketConnection.Checked = LiveData.PosSettings.UseSocketConnection;
            textBox1.Text = LiveData.PosSettings.OrganisationId;
        }
    }
}
