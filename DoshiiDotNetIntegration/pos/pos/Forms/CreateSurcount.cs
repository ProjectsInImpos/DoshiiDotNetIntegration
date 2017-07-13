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
    public partial class CreateSurcount : Form
    {
        
        private bool isEditingProduct = false;
        private PosDoshiiController DoshiiController;
        private Surcount TheSurcount;

        public CreateSurcount(PosDoshiiController doshiiController, Surcount surcountToEdit = null)
        {
            InitializeComponent();
            DoshiiController = doshiiController;
            comboBox1.DataSource = LocalLists.SurcountTypes;
            if (surcountToEdit == null)
            {
                TheSurcount = new Surcount();
            }
            else
            {
                isEditingProduct = true;
                TheSurcount = surcountToEdit;
                textBox1.Enabled = false;
            }

            SetData();
        }

        private void SetData()
        {
            textBox1.Text = TheSurcount.Id;
            textBox2.Text = TheSurcount.Name;
            
            textBox4.Text = TheSurcount.Amount.ToString();
        }

        private bool ValidateInputData()
        {
            if (!isEditingProduct)
            {
                if (LiveData.SurcountList.Exists(x => x.Id == textBox1.Text))
                {
                    MessageBox.Show(this, "the surcount id is already used", "duplicate surcount id");
                    return false;
                }

            }

            if (!InputValidation.TestIsDecimal(textBox4.Text))
            {
                MessageBox.Show(this, "the surcount Amount must be an decimal", "invalid amount");
                return false;
            }

            return true;
        }

        private void CreateSurcountForData()
        {
            TheSurcount.Id = textBox1.Text;
            TheSurcount.Type = (string)comboBox1.SelectedValue;
            TheSurcount.Name = textBox2.Text;
            int surcountAmount = 0;
            int.TryParse(textBox4.Text, out surcountAmount);
            TheSurcount.Amount = surcountAmount;
            if (TheSurcount.Type.ToLowerInvariant() == "absolute")
            {
                TheSurcount.Value = (int)TheSurcount.Amount;
            }
            else
            {
                TheSurcount.Value = 0;
            }
        }

        private void CreateUpdateSurcount()
        {
            var result = DoshiiController.Doshii.UpdateSurcount(TheSurcount);
            if (result.Success)
            {
                if (isEditingProduct)
                {
                    DoshiiController.UpdateSurcount(TheSurcount);
                }
                else
                {
                    LiveData.SurcountList.Add(TheSurcount);
                }
                this.Close();
            }
            else
            {
                MessageBox.Show(this, "there was an issue creating / updateing the surcount.");
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
            CreateSurcountForData();
            CreateUpdateSurcount();
        }

        private void CreateSurcount_Load(object sender, EventArgs e)
        {
            if (TheSurcount.Type == "absolute" || TheSurcount.Type == "percentage")
            {
                comboBox1.SelectedIndex = comboBox1.FindStringExact(TheSurcount.Type);
            }
            
        }
    }
}
