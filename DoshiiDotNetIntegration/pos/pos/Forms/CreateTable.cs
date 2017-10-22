using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Text;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using DoshiiDotNetIntegration.Models;
using DoshiiDotNetIntegration.Models.ActionResults;
using pos.DoshiiImplementation;
using pos.Helpers;

namespace pos.Forms
{
    public partial class CreateTable : Form
    {
        private Table TheTable;
        
        private bool isEditingProduct = false;
        private PosDoshiiController DoshiiController;
        private string oldTableName = "";
        
        public CreateTable(PosDoshiiController doshiiController, Table tableToEdit = null)
        {
            InitializeComponent();
            DoshiiController = doshiiController;
            if (tableToEdit == null)
            {
                TheTable = new Table();
            }
            else
            {
                isEditingProduct = true;
                TheTable = tableToEdit;
                oldTableName = tableToEdit.Name;
            }

            SetData();
        }

        private void SetData()
        {
            textBox1.Text = TheTable.Name;
            textBox2.Text = TheTable.Covers.ToString();
            checkBox1.Checked = TheTable.IsActive;
            
            if (TheTable.Criteria != null)
            {
                checkBox2.Checked = TheTable.Criteria.IsCommunal;
                checkBox3.Checked = TheTable.Criteria.CanMerge;
                checkBox4.Checked = TheTable.Criteria.IsSmoking;
                checkBox5.Checked = TheTable.Criteria.IsOutdoor;
            }
        }

        private bool ValidateInputData()
        {
            if (!isEditingProduct || !oldTableName.Equals(textBox1.Text))
            {
                if (LiveData.TablesList.Exists(x => x.Name == textBox1.Text))
                {
                    MessageBox.Show(this, "the table name is already used", "duplicate table name");
                    return false;
                }

            }
            if (!InputValidation.TestIsInteger(textBox2.Text))
            {
                MessageBox.Show(this, "the covers must be an integer", "invalid covers");
                    return false;
            }
            return true;
        }

        private void CreateTableForData()
        {
            TheTable.Name = textBox1.Text;
            int newCovers = 0;
            int.TryParse(textBox2.Text, out newCovers);
            TheTable.Covers = newCovers;
            TheTable.IsActive = checkBox1.Checked;
            TheTable.Criteria = new TableCriteria();
            TheTable.Criteria.IsCommunal = checkBox2.Checked;
            TheTable.Criteria.CanMerge = checkBox3.Checked;
            TheTable.Criteria.IsSmoking = checkBox4.Checked;
            TheTable.Criteria.IsOutdoor = checkBox5.Checked;
        }

        private void CreateUpdateTable()
        {

            ObjectActionResult<Table> result;
            if (isEditingProduct)
            {
                result = DoshiiController.Doshii.UpdateTable(TheTable, oldTableName);
                if (result.Success)
                {
                    DoshiiController.UpdateTable(result.ReturnObject);
                    this.Close();
                }
            }
            else
            {
                result = DoshiiController.Doshii.CreateTable(TheTable);
                if (result.Success)
                {
                    LiveData.TablesList.Add(result.ReturnObject);
                    this.Close();
                }
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
            CreateTableForData();
            CreateUpdateTable();
        } 
    }
}
