using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using DoshiiDotNetIntegration;
using DoshiiDotNetIntegration.Models;
using pos.DoshiiImplementation;

namespace pos.Forms
{
    public partial class CreateOrginisation : Form
    {
        private Organisation TheOrg = new Organisation();
        private Location TheLoc = new Location();
        private PosDoshiiController DoshiiController;

        public CreateOrginisation(PosDoshiiController doshiiController)
        {
            InitializeComponent();
            DoshiiController = doshiiController;
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

            TheOrg.Name = textBox1.Text;
            TheOrg.Email = textBox2.Text;
            TheOrg.Phone = textBox3.Text;
            TheOrg.AddressLine1 = textBox4.Text;
            TheOrg.AddressLine2 = textBox5.Text;
            TheOrg.City = textBox6.Text;
            TheOrg.State = textBox7.Text;
            TheOrg.PostalCode = textBox8.Text;
            TheOrg.Country = textBox9.Text;
            TheOrg.Abn = textBox10.Text;
            TheOrg.CompanyNumber = textBox11.Text;
            TheOrg.Location = TheLoc;
        }
        
        private void button2_Click(object sender, EventArgs e)
        {
            CreateObjectsFromInput();
            var result = DoshiiController.Doshii.CreateOrginisation(TheOrg);

            if (result.Success)
            {
                MessageBox.Show(this, "orginisations successfully created", "success");
            }
            else
            {
                MessageBox.Show(this, string.Format("failed to create org with error code {0}", result.FailReason));
            }
        }
    }
}
