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
    public partial class CreateLocation : Form
    {
        private Location TheLoc = new Location();
        private PosDoshiiController DoshiiController;

        public CreateLocation(PosDoshiiController doshiiController)
        {
            InitializeComponent();
            DoshiiController = doshiiController;
            textBox1.Text = LiveData.PosSettings.OrganisationId;
        }

        private void button2_Click(object sender, EventArgs e)
        {
            CreateObjectsFromInput();
            var result = DoshiiController.Doshii.CreateLocation(TheLoc);

            if (result.Success)
            {
                MessageBox.Show(this, "location successfully created", "success");
            }
            else
            {
                MessageBox.Show(this, string.Format("failed to create location with error code {0}", result.FailReason));
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void CreateObjectsFromInput()
        {
            TheLoc.Name = textBox21.Text;
            TheLoc.AddressLine1 = textBox20.Text;
            TheLoc.AddressLine2 = textBox19.Text;
            TheLoc.City = textBox18.Text;
            TheLoc.State = textBox17.Text;
            TheLoc.PostalCode = textBox16.Text;
            TheLoc.Country = textBox15.Text;
            TheLoc.PhoneNumber = textBox14.Text;
            TheLoc.OrganisationId = textBox1.Text;
        }
    }
}
