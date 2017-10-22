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
    public partial class CreateEmployee : Form
    {
        private bool isEditingProduct = false;
        private PosDoshiiController DoshiiController;
        private Employee TheEmployee;

        public CreateEmployee(PosDoshiiController doshiiController, Employee memberToEdit = null)
        {
            InitializeComponent();
            DoshiiController = doshiiController;
            if (memberToEdit == null)
            {
                TheEmployee = new Employee();
            }
            else
            {
                isEditingProduct = true;
                TheEmployee = memberToEdit;
                textBox1.Enabled = false;
            }

            SetData();
        }

        private void SetData()
        {
            textBox1.Text = TheEmployee.Id;
            textBox3.Text = TheEmployee.Name;
            textBox5.Text = TheEmployee.Email;
            textBox6.Text = TheEmployee.Phone;
            textBox7.Text = TheEmployee.PosRef;
            textBox13.Text = TheEmployee.LocationId;

            if (TheEmployee.Address != null)
            {
                textBox12.Text = TheEmployee.Address.Line1;
                textBox11.Text = TheEmployee.Address.Line2;
                textBox10.Text = TheEmployee.Address.State;
                textBox9.Text = TheEmployee.Address.City;
                textBox8.Text = TheEmployee.Address.PostalCode;
            }
        }


        private bool ValidateInputData()
        {
            if (!isEditingProduct)
            {
                if (LiveData.MemberList.Exists(x => x.Id == textBox1.Text))
                {
                    MessageBox.Show(this, "the employee id is already used", "duplicate employee id");
                    return false;
                }

                if (LiveData.MemberList.Exists(x => x.Email == textBox5.Text))
                {
                    MessageBox.Show(this, "the employee email must be unique", "duplicate email");
                    return false;
                }

            }

            return true;
        }

        private void CreateMemberForData()
        {
            TheEmployee.Id = textBox1.Text;
            TheEmployee.FirstName = textBox3.Text;
            TheEmployee.LastName = textBox4.Text;
            TheEmployee.Name = textBox3.Text + textBox4.Text;
            TheEmployee.Email = textBox5.Text;
            TheEmployee.Phone = textBox6.Text;
            TheEmployee.PosRef = textBox7.Text;
            TheEmployee.LocationId = textBox13.Text;

            if (!string.IsNullOrEmpty(textBox12.Text) || !string.IsNullOrEmpty(textBox11.Text) ||
                !string.IsNullOrEmpty(textBox10.Text) || !string.IsNullOrEmpty(textBox9.Text) ||
                !string.IsNullOrEmpty(textBox8.Text))
            {
                TheEmployee.Address = new Address();
                TheEmployee.Address.Line1 = textBox12.Text;
                TheEmployee.Address.Line2 = textBox11.Text;
                TheEmployee.Address.State = textBox10.Text;
                TheEmployee.Address.City = textBox9.Text;
                TheEmployee.Address.PostalCode = textBox8.Text;
                TheEmployee.Address.Country = "AU";
            }
        }

        private void CreateUpdateMember()
        {
            var result = DoshiiController.Doshii.UpdateEmployee(TheEmployee);
            if (result.Success)
            {
                if (isEditingProduct)
                {
                    DoshiiController.UpdateEmployee(result.ReturnObject);
                }
                else
                {
                    LiveData.EmployeeList.Add(result.ReturnObject);
                }
                this.Close();
            }
            else
            {
                MessageBox.Show(this, "there was an issue creating / updateing the employee.");
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
