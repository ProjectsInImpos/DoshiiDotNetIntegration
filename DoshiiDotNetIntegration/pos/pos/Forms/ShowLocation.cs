using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using DoshiiDotNetIntegration.Models;
using pos.DoshiiImplementation;

namespace pos.Forms
{
    public partial class ShowLocation : Form
    {
        private Location TheLoc;

        public ShowLocation(Location location)
        {
            InitializeComponent();
            TheLoc = location;
            SetData();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void SetData()
        {
            textBox21.Text = TheLoc.Name;
            textBox20.Text = TheLoc.AddressLine1;
            textBox19.Text = TheLoc.AddressLine2;
            textBox18.Text = TheLoc.City;
            textBox17.Text = TheLoc.State;
            textBox16.Text = TheLoc.PostalCode;
            textBox15.Text = TheLoc.Country;
            textBox14.Text = TheLoc.PhoneNumber;

            textBox4.Text = TheLoc.Latitude;
            textBox3.Text = TheLoc.Longitude;
            textBox2.Text = TheLoc.Id;
            textBox1.Text = TheLoc.OrganisationId;

            textBox6.Text = TheLoc.Token;
            textBox5.Text = TheLoc.ApiVersion;

        }
    }
}
