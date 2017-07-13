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
    public partial class CreateProduct : Form
    {
        private bool isEditingProduct = false;
        private Product TheProduct;
        private BindingList<Variant> productOptionList;
        private BindingList<Surcount> productSurcountList;
        private BindingList<Product> productIncludedItemList;
        private PosDoshiiController DoshiiController;

        public CreateProduct(PosDoshiiController doshiiController, Product productToEdit = null)
        {
            InitializeComponent();
            DoshiiController = doshiiController;
            if (productToEdit == null)
            {
                TheProduct = new Product();
            }
            else
            {
                isEditingProduct = true;
                TheProduct = productToEdit;
                textBox1.Enabled = false;
            }
            
            SetData();
        }

        public void SetData()
        {
            textBox1.Text = TheProduct.PosId;
            textBox2.Text = TheProduct.Name;
            textBox3.Text = TheProduct.Description;
            textBox4.Text = TheProduct.UnitPrice.ToString();
            StringBuilder productTagsBuilder = new StringBuilder();
            if (TheProduct.Tags.Any())
            {
                var last = TheProduct.Tags.Last();
                foreach (var tag in TheProduct.Tags)
                {
                    if (tag.Equals(last))
                    {
                        productTagsBuilder.AppendFormat("{0}", tag);
                    }
                    else
                    {
                        productTagsBuilder.AppendFormat("{0},", tag);
                    }

                }
            }
            else
            {
                productTagsBuilder.Append("");
            }
            
            textBox5.Text = productTagsBuilder.ToString();

            StringBuilder menuDirBuilder = new StringBuilder();
            if (TheProduct.MenuDir.Any())
            {
                var last = TheProduct.MenuDir.Last();
                foreach (var mdir in TheProduct.MenuDir)
                {
                    if (mdir.Equals(last))
                    {
                        menuDirBuilder.AppendFormat("{0}", mdir);
                    }
                    else
                    {
                        menuDirBuilder.AppendFormat("{0},", mdir);
                    }

                }
            }
            else
            {
                menuDirBuilder.Append("");
            }
            
            textBox6.Text = menuDirBuilder.ToString();

            comboBox1.DataSource = LocalLists.ProductTypes;

            if (!string.IsNullOrEmpty(TheProduct.Type))
            {
                comboBox1.SelectedIndex = comboBox1.FindStringExact(TheProduct.Type);
            }
            
            
            comboBox2.DataSource = LiveData.ProductList;
            comboBox2.ValueMember = "PosId";
            comboBox2.DisplayMember = "Name";

            comboBox3.DataSource = LiveData.SurcountList;
            comboBox3.DisplayMember = "Name";
            comboBox3.ValueMember = "Id";

            comboBox4.DataSource = LiveData.ProductList;
            comboBox4.ValueMember = "PosId";
            comboBox4.DisplayMember = "Name";

            if (TheProduct.ProductOptions.Any())
            {
                productOptionList = new BindingList<Variant>(TheProduct.ProductOptions[0].Variants);
            }
            else
            {
                productOptionList = new BindingList<Variant>();
            }
            dataGridView1.DataSource = productOptionList;
            dataGridView1.DataMember = "Name";

            if (TheProduct.ProductSurcounts.Any())
            {
                productSurcountList = new BindingList<Surcount>(TheProduct.ProductSurcounts.ToList());
            }
            else
            {
                productSurcountList = new BindingList<Surcount>();
            }
            dataGridView2.DataSource = productSurcountList;
            dataGridView2.DataMember = "Name";

            if (TheProduct.IncludedItems.Any())
            {
                productIncludedItemList = new BindingList<Product>(TheProduct.IncludedItems);
            }
            else
            {
                productIncludedItemList = new BindingList<Product>();
            }
            dataGridView2.DataSource = productIncludedItemList;
            dataGridView2.DataMember = "Name";
        }
        
        
        private void button1_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            var productToAdd = LiveData.ProductList.FirstOrDefault(x => x.PosId == comboBox4.SelectedValue);
            if (productToAdd != null)
            {
                var varientToAdd = new Variant();
                varientToAdd.PosId = productToAdd.PosId;
                varientToAdd.Name = productToAdd.Name;
                varientToAdd.Price = productToAdd.UnitPrice;
                productOptionList.Add(varientToAdd);
            }
        }

        private void button7_Click(object sender, EventArgs e)
        {
            var productToAdd = LiveData.ProductList.FirstOrDefault(x => x.PosId == comboBox4.SelectedValue);
            if (productToAdd != null)
            {
                productIncludedItemList.Add(productToAdd);
            }
            
        }

        private void button5_Click(object sender, EventArgs e)
        {
            var surcountToAdd = LiveData.SurcountList.FirstOrDefault(x => x.Id == comboBox3.SelectedValue);
            if (surcountToAdd != null)
            {
                productSurcountList.Add(surcountToAdd);
            }
            
        }

        private void button4_Click(object sender, EventArgs e)
        {
            Variant currentObject = (Variant)dataGridView1.CurrentRow.DataBoundItem;
            productOptionList.Remove(currentObject);
        }

        private void button6_Click(object sender, EventArgs e)
        {
            Surcount currentObject = (Surcount)dataGridView2.CurrentRow.DataBoundItem;
            productSurcountList.Remove(currentObject);
        }

        private void button8_Click(object sender, EventArgs e)
        {
            Product currentObject = (Product)dataGridView3.CurrentRow.DataBoundItem;
            productIncludedItemList.Remove(currentObject);
        }

        private void convertProductFromFormData()
        {
            if (!isEditingProduct)
            {
                TheProduct.PosId = textBox1.Text;
            }


            TheProduct.Name = textBox2.Text;
            TheProduct.Description = textBox3.Text;
            decimal thisUnitPrice = 0;
            if (decimal.TryParse(textBox4.Text, out thisUnitPrice))
            {
                TheProduct.UnitPrice = thisUnitPrice;
            }
            else
            {
                TheProduct.UnitPrice = 0;
            }

            var stringList = textBox5.Text.Split(',').ToList();
            foreach (var str in stringList)
            {
                if (!string.IsNullOrEmpty(str))
                {
                    TheProduct.Tags.Add(str);
                }
            }

            stringList = textBox6.Text.Split(',').ToList();
            foreach (var str in stringList)
            {
                if (!string.IsNullOrEmpty(str))
                {
                    TheProduct.MenuDir.Add(str);
                }
            }
            
            if (((string) comboBox1.SelectedItem).Equals("none"))
            {
                TheProduct.Type = "";
            }
            else
            {
                TheProduct.Type = (string)comboBox1.SelectedItem;
            }
            

            if (TheProduct.ProductOptions.Any())
            {
                TheProduct.ProductOptions[0].Variants = productOptionList.ToList();
            }
            else
            {
                if (productOptionList.Any())
                {
                    var productOptions = new ProductOptions();
                    productOptions.PosId = "options list 1";
                    productOptions.Variants = productOptionList.ToList();
                    productOptions.Max = 1;
                    productOptions.Min = 1;
                    productOptions.Name = "options list 1";
                    TheProduct.ProductOptions.Add(productOptions);
                }
            }

            if (productIncludedItemList.Any())
            {
                TheProduct.IncludedItems = productIncludedItemList.ToList();
            }

            if (productSurcountList.Any())
            {
                TheProduct.ProductSurcounts = productSurcountList.ToList();
            }
        }

        private bool validateInput()
        {
            if (!isEditingProduct)
            {
                if (LiveData.ProductList.Exists(x => x.PosId == textBox1.Text))
                {
                    MessageBox.Show(this, "you must create a new product with a unique posId.", "Duplicate PosId");
                    return false;
                }
            }

            if (!InputValidation.TestIsDecimal(textBox4.Text))
            {
                MessageBox.Show(this, "you must add a decimal value to the unit price.", "not a decimal");
                return false;
            }

            if (string.IsNullOrEmpty(textBox2.Text))
            {
                MessageBox.Show(this, "you must add name to the product.", "no name");
                return false;
            }
            return true;
        }


        private void button2_Click(object sender, EventArgs e)
        {
            if (!validateInput())
            {
                return;
            }
            convertProductFromFormData();
            var result = DoshiiController.Doshii.UpdateProduct(TheProduct);
            if (result.Success)
            {
                if (isEditingProduct)
                {
                    DoshiiController.UpdateProduct(TheProduct);
                }
                else
                {
                    LiveData.ProductList.Add(TheProduct);
                }
                this.Close();
            }
            else
            {
                MessageBox.Show(this, "there was an error creating / updating the product", "error");
            }
        }
    }
}
