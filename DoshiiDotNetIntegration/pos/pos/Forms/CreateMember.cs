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
    public partial class CreateMember : Form
    {

        private bool isEditingProduct = false;
        private PosDoshiiController DoshiiController;
        private MemberOrg TheMember;
        
        public CreateMember(PosDoshiiController doshiiController, MemberOrg memberToEdit = null)
        {
            InitializeComponent();
            DoshiiController = doshiiController;
            if (memberToEdit == null)
            {
                TheMember = new MemberOrg();
            }
            else
            {
                isEditingProduct = true;
                TheMember = memberToEdit;
                textBox1.Enabled = false;
            }

            SetData();
        }

        private void SetData()
        {
            textBox1.Text = TheMember.Id;
            textBox2.Text = TheMember.Name;
            textBox3.Text = TheMember.FirstName;
            textBox4.Text = TheMember.LastName;
            textBox5.Text = TheMember.Email;
            textBox6.Text = TheMember.Phone;
            textBox7.Text = TheMember.Ref;

            if (TheMember.Address != null)
            {
                textBox12.Text = TheMember.Address.Line1;
                textBox11.Text = TheMember.Address.Line2;
                textBox10.Text = TheMember.Address.State;
                textBox9.Text = TheMember.Address.City;
                textBox8.Text = TheMember.Address.PostalCode;
            }
        }


        private bool ValidateInputData()
        {
            if (!isEditingProduct)
            {
                if (LiveData.MemberList.Exists(x => x.Id == textBox1.Text))
                {
                    MessageBox.Show(this, "the member id is already used", "duplicate member id");
                    return false;
                }

                if (LiveData.MemberList.Exists(x => x.Email == textBox5.Text))
                {
                    MessageBox.Show(this, "the member email must be unique", "duplicate email");
                    return false;
                }

            }

            if (string.IsNullOrEmpty(textBox2.Text))
            {
                MessageBox.Show(this, "the member must have a name", "no member name");
                return false;
            }

            if (string.IsNullOrEmpty(textBox5.Text))
            {
                MessageBox.Show(this, "the member must have an email", "no member email");
                return false;
            }

            return true;
        }
        
        private void CreateMemberForData()
        {
            TheMember.Name = textBox2.Text;
            TheMember.FirstName = textBox3.Text;
            TheMember.LastName = textBox4.Text;
            TheMember.Email = textBox5.Text;
            TheMember.Phone = textBox6.Text;
            TheMember.Ref = textBox7.Text;

            if (!string.IsNullOrEmpty(textBox12.Text) || !string.IsNullOrEmpty(textBox11.Text) ||
                !string.IsNullOrEmpty(textBox10.Text) || !string.IsNullOrEmpty(textBox9.Text) ||
                !string.IsNullOrEmpty(textBox8.Text))
            {
                TheMember.Address = new Address();
                TheMember.Address.Line1 = textBox12.Text;
                TheMember.Address.Line2 = textBox11.Text;
                TheMember.Address.State = textBox10.Text;
                TheMember.Address.City = textBox9.Text;
                TheMember.Address.PostalCode = textBox8.Text;
                TheMember.Address.Country = "AU";
            }
        }

        private void CreateUpdateMember()
        {
            var result = DoshiiController.Doshii.UpdateMember(TheMember);
            if (result.Success)
            {
                if (isEditingProduct)
                {
                    DoshiiController.UpdateMember(result.ReturnObject);
                }
                else
                {
                    LiveData.MemberList.Add(result.ReturnObject);
                }
                this.Close();
            }
            else
            {
                MessageBox.Show(this, "there was an issue creating / updateing the member.");
            }
            
        }

        private void button1_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            if (!ValidateInputData())
            {
                return;
            }
            CreateMemberForData();
            CreateUpdateMember();
        }
    }
}
