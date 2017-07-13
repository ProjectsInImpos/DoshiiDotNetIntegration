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
using pos.Helpers;
using pos.Models;

namespace pos.Forms
{
    public partial class CreateBooking : Form
    {
        private bool isEditingProduct = false;
        private PosDoshiiController DoshiiController;
        private Booking TheBooking;
        private BindingList<Table> BookingTableList;

        public CreateBooking(PosDoshiiController doshiiController, Booking bookingToEdit = null)
        {
            InitializeComponent();
            DoshiiController = doshiiController;
            if (bookingToEdit == null)
            {
                TheBooking = new Booking();
            }
            else
            {
                isEditingProduct = true;
                TheBooking = bookingToEdit;
                textBox1.Enabled = false;
            }

            SetData();
        }

        private void SetData()
        {
            BookingTableList = new BindingList<Table>();
            if (TheBooking.TableNames != null && TheBooking.TableNames.Any())
            {
                foreach (var tableName in TheBooking.TableNames)
                {
                    BookingTableList.Add(LiveData.TablesList.FirstOrDefault(x => x.Name == tableName));
                }
            }
            dataGridView2.DataSource = BookingTableList;

            textBox1.Text = TheBooking.Id;

            if (TheBooking.Date == null)
            {
                dateTimePicker1.Value = DateTime.Now;
            }
            else
            {
                dateTimePicker1.Value = TheBooking.Date.Value;
            }
            

            textBox2.Text = TheBooking.Covers.ToString();

            comboBox1.DataSource = LiveData.ConsumerList;
            comboBox1.ValueMember = "Name";
            comboBox1.DisplayMember = "Name";

            comboBox2.DataSource = LiveData.ChecinList;
            comboBox2.ValueMember = "Id";
            comboBox2.DisplayMember = "Id";

            comboBox4.DataSource = LiveData.AppsList;
            comboBox4.ValueMember = "Id";
            comboBox4.DisplayMember = "Id";

            comboBox5.DataSource = LocalLists.BookingStatus;


            if (!string.IsNullOrEmpty(TheBooking.Status))
            {
                comboBox5.SelectedText = TheBooking.Status;
            }

            comboBox4.Enabled = false;
            if (!isEditingProduct)
            {
                comboBox4.Visible = false;
                label7.Visible = false;
                comboBox1.Visible = false;
            }
            else
            {
                comboBox4.SelectedText = TheBooking.App.Name;
                textBox3.Visible = false;
                textBox4.Visible = false;
                textBox5.Visible = false;
                label8.Visible = false;
                label9.Visible = false;
                label10.Visible = false;
                comboBox1.SelectedItem = LiveData.ConsumerList.FirstOrDefault(x => x.Name == TheBooking.Consumer.Name);
            }

            comboBox3.DataSource = LiveData.TablesList;
            comboBox3.ValueMember = "Name";
            comboBox3.DisplayMember = "Name";
            
        }

        private void button2_Click(object sender, EventArgs e)
        {
            if (!ValidateInputData())
            {
                return;
            }
            CreateBookingFormData();
            CreateUpdateBooking();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void button5_Click(object sender, EventArgs e)
        {
            var tableToAdd = LiveData.TablesList.FirstOrDefault(x => x.Name == ((Table)comboBox3.SelectedItem).Name);
            if (tableToAdd != null)
            {
                BookingTableList.Add(tableToAdd);
            }
            dataGridView2.DataSource = null;
            dataGridView2.DataSource = BookingTableList;
        }

        private void button6_Click(object sender, EventArgs e)
        {
            Table currentObject = (Table)dataGridView2.CurrentRow.DataBoundItem;
            BookingTableList.Remove(currentObject);
        }

        private bool ValidateInputData()
        {
            if (!isEditingProduct)
            {
                if (LiveData.BookingList.Exists(x => x.Id == textBox1.Text))
                {
                    MessageBox.Show(this, "the booking id is already used", "duplicate booking id");
                    return false;
                }
            }

            if (!InputValidation.TestIsInteger(textBox2.Text))
            {
                MessageBox.Show(this, "The booking covers must be an integer", "invalid booking data");
                return false;
            }

            return true;
        }

        private void CreateBookingFormData()
        {
            TheBooking.Id = textBox1.Text;

            TheBooking.Status = (string)comboBox5.SelectedItem;

            int thecovers = 0;
            if (int.TryParse(textBox2.Text, out thecovers))
            {
                TheBooking.Covers = thecovers;
            }
            else
            {
                TheBooking.Covers = 0;
            }
            TheBooking.Date = dateTimePicker1.Value;
            if (isEditingProduct)
            {
                TheBooking.Consumer = LiveData.ConsumerList.FirstOrDefault(x => x.Name == ((Consumer)comboBox1.SelectedItem).Name);
            }
            else
            {
                TheBooking.Consumer = new Consumer();
                TheBooking.Consumer.Name = textBox3.Text;
                TheBooking.Consumer.Email = textBox4.Text;
                TheBooking.Consumer.Phone = textBox5.Text;
            }
            
            TheBooking.CheckinId = comboBox2.SelectedText;
            TheBooking.TableNames.Clear();
            foreach (var table in BookingTableList)
            {
                TheBooking.TableNames.Add(table.Name);
            }
            TheBooking.App = null;
            //TheBooking.App = LiveData.AppsList.FirstOrDefault(x => x.Id == comboBox4.SelectedText);
        }

        private void CreateUpdateBooking()
        {
            var result = DoshiiController.Doshii.UpdateBooking(TheBooking);
            if (result.Success)
            {
                if (isEditingProduct)
                {
                    DoshiiController.UpdateBooking(result.ReturnObject);
                }
                else
                {
                    LiveData.BookingList.Add(result.ReturnObject);
                    LiveData.ConsumerList.Add(result.ReturnObject.Consumer);
                }
                this.Close();
            }
            else
            {
                MessageBox.Show(this, "there was an issue creating / updateing the booking.");
            }

        }

        
    }
}
