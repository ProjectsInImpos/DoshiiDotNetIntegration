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
using DoshiiDotNetIntegration.Models.ActionResults;
using pos.DoshiiImplementation;
using pos.Helpers;
using pos.Models;

namespace pos.Forms
{
    public partial class CreateOrder : Form
    {
        private bool isEditingProduct = false;
        private PosDoshiiController DoshiiController;
        private Order TheOrder;
        private BindingList<Product> OrderItemList;
        private BindingList<Surcount> OrderSurcountList;

        public CreateOrder(PosDoshiiController doshiiController, Order orderToEdit = null)
        {
            InitializeComponent();
            DoshiiController = doshiiController;
            if (orderToEdit == null)
            {
                TheOrder = new Order();
                TheOrder.Id = LiveData.PosSettings.OrderNumber.ToString();
            }
            else
            {
                isEditingProduct = true;
                TheOrder = orderToEdit;
                textBox1.Enabled = false;
            }

            SetData();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        public void SetData()
        {
            textBox1.Text = TheOrder.Id;
            textBox2.Text = TheOrder.Phase;

            if (TheOrder.Items.Any())
            {
                OrderItemList = new BindingList<Product>(TheOrder.Items);
            }
            else
            {
                OrderItemList = new BindingList<Product>();
            }
            dataGridView3.DataSource = OrderItemList;

            if (TheOrder.Surcounts.Any())
            {
                OrderSurcountList = new BindingList<Surcount>(TheOrder.Surcounts);
            }
            else
            {
                OrderSurcountList = new BindingList<Surcount>();
            }
            dataGridView2.DataSource = OrderSurcountList;

            comboBox1.DataSource = LocalLists.OrderStatus;

            comboBox6.DataSource = LocalLists.OrderTypes;

            comboBox7.DataSource = LiveData.TablesList;
            comboBox7.DisplayMember = "Name";

            if (!LiveData.ChecinList.Exists(x => x.Id == "none"))
            {
                var ch = new Checkin();
                ch.Id = "none";
                LiveData.ChecinList.Add(ch);
            }
            comboBox2.DataSource = LiveData.ChecinList;
            comboBox2.ValueMember = "Id";
            comboBox2.DisplayMember = "Id";

            if (!LiveData.MemberList.Exists(x => x.Id == "none"))
            {
                var ch = new MemberOrg();
                ch.Id = "none";
                LiveData.MemberList.Add(ch);
            }
            comboBox5.DataSource = LiveData.MemberList;
            comboBox5.ValueMember = "Id";
            comboBox5.DisplayMember = "Id";

            comboBox3.DataSource = LiveData.SurcountList;
            comboBox3.ValueMember = "Id";
            comboBox3.DisplayMember = "Name";

            comboBox4.DataSource = LiveData.ProductList;
            comboBox4.ValueMember = "PosId";
            comboBox4.DisplayMember = "Name";
        }

        private void CreateOrder_Load(object sender, EventArgs e)
        {
            comboBox1.SelectedIndex = comboBox1.FindStringExact(TheOrder.Status);

            if (string.IsNullOrEmpty(TheOrder.CheckinId))
            {
                comboBox2.SelectedIndex = comboBox2.FindStringExact(TheOrder.CheckinId);
            }
            else
            {
                comboBox2.SelectedIndex = comboBox2.FindStringExact("none");
            }

            if (string.IsNullOrEmpty(TheOrder.MemberId))
            {
                comboBox5.SelectedIndex = comboBox5.FindStringExact(TheOrder.MemberId);
            }
            else
            {
                comboBox5.SelectedIndex = comboBox5.FindStringExact("none");
            }
            
        }

        private void button6_Click(object sender, EventArgs e)
        {
            Surcount currentObject = (Surcount)dataGridView2.CurrentRow.DataBoundItem;
            OrderSurcountList.Remove(currentObject);
        }

        private void button8_Click(object sender, EventArgs e)
        {
            Product currentObject = (Product)dataGridView3.CurrentRow.DataBoundItem;
            OrderItemList.Remove(currentObject);
        }

        private void button5_Click(object sender, EventArgs e)
        {
            var surcountToAdd = LiveData.SurcountList.FirstOrDefault(x => x.Id == comboBox3.SelectedValue);
            if (surcountToAdd != null)
            {
                OrderSurcountList.Add(surcountToAdd);
            }
            dataGridView2.DataSource = null;
            dataGridView2.DataSource = OrderSurcountList;
        }

        private void button7_Click(object sender, EventArgs e)
        {
            var productToAdd = LiveData.ProductList.FirstOrDefault(x => x.PosId == comboBox4.SelectedValue);
            if (productToAdd != null)
            {
                if (productToAdd.Quantity == 0)
                {
                    productToAdd.TotalAfterSurcounts = productToAdd.UnitPrice;
                    productToAdd.TotalBeforeSurcounts = productToAdd.UnitPrice;
                    productToAdd.Quantity = 1;
                }
                OrderItemList.Add(productToAdd);
            }
            dataGridView3.DataSource = null;
            dataGridView3.DataSource = OrderItemList;

        }

        private bool ValidateInput()
        {
            if (!isEditingProduct)
            {
                if (LiveData.OrdersList.Exists(x => x.Id == textBox1.Text))
                {
                    MessageBox.Show(this, "you must create a new order with a unique Id.", "Duplicate Id");
                    return false;
                }
            }
            return true;
        }

        private void ConvertOrderFromFormData()
        {
            TheOrder.Id = textBox1.Text;
            TheOrder.Phase = textBox2.Text;
            TheOrder.Status = (string)comboBox1.SelectedItem;

            if ((comboBox2.SelectedValue == null)||((string)comboBox2.SelectedValue).Equals("none"))
            {
                TheOrder.CheckinId = "";
            }
            else
            {
                TheOrder.CheckinId = (string)comboBox2.SelectedItem;
            }

            if ((comboBox5.SelectedValue == null) || ((string)comboBox5.SelectedValue).Equals("none"))
            {
                TheOrder.MemberId = "";
            }
            else
            {
                TheOrder.MemberId = (string)comboBox5.SelectedItem;
            }
            TheOrder.Type = (string)comboBox6.SelectedItem;
            foreach (var item in OrderItemList)
            {
                var newItem = LiveData.ProductList.FirstOrDefault(x => x.PosId == item.PosId);
                if (newItem.Quantity == 0)
                {
                    newItem.Quantity = 1;
                    
                }
                newItem.TotalBeforeSurcounts = newItem.Quantity * newItem.UnitPrice;
                newItem.TotalAfterSurcounts = newItem.TotalBeforeSurcounts;
                TheOrder.Items.Add(item);
            }
            TheOrder.Items = OrderItemList.ToList();
            foreach (var item in TheOrder.Items)
            {
                if (item.Type == null || string.IsNullOrEmpty(item.Type))
                {
                    item.Type = "single";
                }
            }
            TheOrder.Surcounts = OrderSurcountList.ToList();
            OrderTotalHelper.CalculateOrderTotal(TheOrder);
        }

        private ObjectActionResult<Order> UpdateOrderOnDoshii()
        {
            var result = DoshiiController.Doshii.UpdateOrder(TheOrder);
            if (result.Success)
            {
                if (checkBox1.Checked)
                {
                    if (!string.IsNullOrEmpty(result.ReturnObject.CheckinId))
                    {
                        var table = comboBox7.SelectedItem as Table;
                        if (table != null)
                        {
                            DoshiiController.Doshii.ModifyTableAllocation(result.ReturnObject.CheckinId,
                                new List<string>() { table.Name }, 2);
                        }
                    }
                    else
                    {
                        var checkinResult = DoshiiController.Doshii.GetNewCheckin();
                        result.ReturnObject.CheckinId = checkinResult.ReturnObject.Id;
                        var table = comboBox7.SelectedItem as Table;
                        if (table != null)
                        {
                            DoshiiController.Doshii.ModifyTableAllocation(result.ReturnObject.CheckinId,
                                new List<string>() { table.Name }, 2);
                        }
                        
                        result = DoshiiController.Doshii.UpdateOrder(TheOrder);
                        
                    }
                }
                if (isEditingProduct)
                {
                    DoshiiController.UpdateOrder(result.ReturnObject);
                }
                else
                {
                    LiveData.OrdersList.Add(result.ReturnObject);
                }
                return result;
            }
            else
            {
                MessageBox.Show(this, "there was an error creating / updating the product", "error");
                return result;
            }
        }
        
        private void button2_Click(object sender, EventArgs e)
        {
            if (sendOrderToDoshii())
            {
                this.Close();
            }
        }

        private bool sendOrderToDoshii()
        {
            if (!ValidateInput())
            {
                return false;
            }
            ConvertOrderFromFormData();
            if (!UpdateOrderOnDoshii().Success)
            {
                return false;
            }
            return true;
        }


        private void button3_Click(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(comboBox5.SelectedText))
            {
                MemberRewards mrf = new MemberRewards(DoshiiController, comboBox5.SelectedText, true);
                mrf.ShowDialog();
                var theRewrad = mrf.TheSelectedReward;
                var theMember = LiveData.MemberList.FirstOrDefault(x => x.Id == comboBox5.SelectedText);
                if (sendOrderToDoshii())
                {
                    var result = DoshiiController.Doshii.RedeemRewardForMember(theMember, theRewrad, TheOrder);
                    if (result.Success)
                    {
                        this.Close();
                    }
                    else
                    {
                        MessageBox.Show(this,
                            string.Format("there was a problem redeeming the reward with Doshii - {0}",
                                result.FailReason));
                    }
                }
                else
                {
                    MessageBox.Show(this, "there was an issue redeeming the reward");
                }
            }
        }


            
		    
    }
}
