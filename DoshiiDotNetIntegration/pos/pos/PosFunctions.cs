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
using pos.DoshiiImplementation;
using pos.Forms;
using pos.Helpers;

namespace pos
{
    public partial class PosFunctions : Form
    {

        public PosDoshiiController DoshiiController;

        private BindingList<App> AppsList;
        private BindingList<Booking> BookingList;
        private BindingList<Consumer> ConsumerList;
        private BindingList<Product> ItemList;
        private BindingList<MemberOrg> MemberList;
        private BindingList<Order> OrdersList;
        private BindingList<Table> TablesList;
        private BindingList<Checkin> CheckinList;
        private BindingList<Surcount> SurcountList;
        private BindingList<Location> LocationList;
        private BindingList<Employee> EmployeeList;
        private BindingList<Transaction> TransactionList;

        public PosFunctions()
        {
            InitializeComponent();
            RefreshData();
        }

        private void RefreshData()
        {
            dataGridView1.DataSource = null;
            dataGridView2.DataSource = null;
            dataGridView3.DataSource = null;
            dataGridView4.DataSource = null;
            dataGridView5.DataSource = null;
            dataGridView6.DataSource = null;
            dataGridView7.DataSource = null;
            dataGridView8.DataSource = null;
            dataGridView9.DataSource = null;
            dataGridView10.DataSource = null;
            dataGridView11.DataSource = null;
            AppsList = new BindingList<App>(LiveData.AppsList);
            BookingList = new BindingList<Booking>(LiveData.BookingList);
            ConsumerList = new BindingList<Consumer>(LiveData.ConsumerList);
            ItemList = new BindingList<Product>(LiveData.ProductList);
            MemberList = new BindingList<MemberOrg>(LiveData.MemberList);
            OrdersList = new BindingList<Order>(LiveData.OrdersList);
            TablesList = new BindingList<Table>(LiveData.TablesList);
            CheckinList = new BindingList<Checkin>(LiveData.ChecinList);
            SurcountList = new BindingList<Surcount>(LiveData.SurcountList);
            LocationList = new BindingList<Location>(LiveData.LocationList);
            EmployeeList = new BindingList<Employee>(LiveData.EmployeeList);
            TransactionList = new BindingList<Transaction>(LiveData.TransactionList);
            dataGridView1.DataSource = new BindingSource(AppsList, null);
            dataGridView2.DataSource = new BindingSource(OrdersList, null);
            dataGridView3.DataSource = new BindingSource(ItemList, null);
            dataGridView4.DataSource = new BindingSource(MemberList, null);
            dataGridView5.DataSource = new BindingSource(BookingList, null);
            dataGridView6.DataSource = new BindingSource(TablesList, null);
            dataGridView7.DataSource = new BindingSource(CheckinList, null);
            dataGridView8.DataSource = new BindingSource(SurcountList, null);
            dataGridView9.DataSource = new BindingSource(LocationList, null);
            dataGridView10.DataSource = new BindingSource(EmployeeList, null);
            dataGridView11.DataSource = new BindingSource(TransactionList, null);
        }

        public void WriteToLog(string message, Color? textColour, Color? backColour, FontStyle style)
        {
            if (InvokeRequired)
                BeginInvoke(new LogDelegate(UpdateLog), new object[] { message, textColour, backColour, style });
            else
                UpdateLog(message, textColour, backColour, style);
        }

        private delegate void LogDelegate(string message, Color? textColour, Color? backColour, FontStyle style);
        private static readonly object logLock = new object();
        private void UpdateLog(string message, Color? textColour, Color? backColour, FontStyle style)
        {
            lock (logLock)
            {
                int start = rtbLog.Text.Length;
                int length = message.Length;

                rtbLog.AppendText(message);
                rtbLog.AppendText(Environment.NewLine);

                rtbLog.SelectionStart = start;
                rtbLog.SelectionLength = length;

                if (textColour.HasValue)
                    rtbLog.SelectionColor = textColour.Value;
                if (backColour.HasValue)
                    rtbLog.SelectionBackColor = backColour.Value;
                rtbLog.SelectionFont = new Font(rtbLog.Font, style);

                rtbLog.SelectionLength = 0;
                rtbLog.SelectionStart = rtbLog.Text.Length;

                rtbLog.Refresh();
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            this.WriteToLog("Saving pos data to file, before close", Color.Aqua, Color.White, FontStyle.Bold);
            DataPersistanceHelper.SaveDataToStore();
            this.Close();
        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            LiveData.PosSettings.ConfirmAllOrders = checkBox1.Checked;
        }

        private void checkBox2_CheckedChanged(object sender, EventArgs e)
        {
            LiveData.PosSettings.ConfirmAllTransactions = checkBox2.Checked;
        }

        private void checkBox3_CheckedChanged(object sender, EventArgs e)
        {
            LiveData.PosSettings.ConfirmAllRefunds = checkBox3.Checked;
        }

        private void PosFunctions_Load(object sender, EventArgs e)
        {
            WriteToLog("Loading Doshii", null, null, FontStyle.Bold);
            DoshiiController = new PosDoshiiController();
            DoshiiController.Initialize(new ConfigurationManager(new DisplayHelper(this), DoshiiController));
        }

        private void Refresh_Click(object sender, EventArgs e)
        {
            RefreshData();
        }

        private void syncMenu_Click(object sender, EventArgs e)
        {
            DoshiiController.SyncMenu();
            RefreshData();
        }

        private void button26_Click(object sender, EventArgs e)
        {
            DoshiiController.SyncTables();
            RefreshData();
        }

        private void button11_Click(object sender, EventArgs e)
        {
            CreateProduct spForm = new CreateProduct(DoshiiController);
            spForm.ShowDialog();
            RefreshData();
        }

        private void button29_Click(object sender, EventArgs e)
        {
            Product currentObject = (Product)dataGridView3.CurrentRow.DataBoundItem;
            CreateProduct spForm = new CreateProduct(DoshiiController, currentObject);
            spForm.ShowDialog();
            RefreshData();
        }

        private void button20_Click(object sender, EventArgs e)
        {
            Product currentObject = (Product)dataGridView3.CurrentRow.DataBoundItem;
            if (DoshiiController.Doshii.DeleteProduct(currentObject.PosId).Success)
            {
                LiveData.ProductList.RemoveAll(x => x.PosId == currentObject.PosId);
                RefreshData();
            }
        }

        private void button12_Click(object sender, EventArgs e)
        {
            CreateMember spForm = new CreateMember(DoshiiController);
            spForm.ShowDialog();
            RefreshData();
        }

        private void button30_Click(object sender, EventArgs e)
        {
            MemberOrg currentObject = (MemberOrg)dataGridView4.CurrentRow.DataBoundItem;
            CreateMember spForm = new CreateMember(DoshiiController, currentObject);
            spForm.ShowDialog();
            RefreshData();
        }

        private void button21_Click(object sender, EventArgs e)
        {
            MemberOrg currentObject = (MemberOrg)dataGridView4.CurrentRow.DataBoundItem;
            if (DoshiiController.Doshii.DeleteMember(currentObject.Id).Success)
            {
                LiveData.MemberList.RemoveAll(x => x.Id == currentObject.Id);
                RefreshData();
            }
        }

        private void button15_Click(object sender, EventArgs e)
        {
            CreateTable spForm = new CreateTable(DoshiiController);
            spForm.ShowDialog();
            RefreshData();
        }

        private void button14_Click(object sender, EventArgs e)
        {
            Table currentObject = (Table)dataGridView6.CurrentRow.DataBoundItem;
            CreateTable spForm = new CreateTable(DoshiiController, currentObject);
            spForm.ShowDialog();
            RefreshData();
        }

        private void button23_Click(object sender, EventArgs e)
        {
            Table currentObject = (Table)dataGridView6.CurrentRow.DataBoundItem;
            if (DoshiiController.Doshii.DeleteTable(currentObject.Name).Success)
            {
                LiveData.TablesList.RemoveAll(x => x.Name == currentObject.Name);
                RefreshData();
            }
        }

        private void button17_Click(object sender, EventArgs e)
        {
            CreateSurcount spForm = new CreateSurcount(DoshiiController);
            spForm.ShowDialog();
            RefreshData();
        }

        private void button34_Click(object sender, EventArgs e)
        {
            Surcount currentObject = (Surcount)dataGridView8.CurrentRow.DataBoundItem;
            CreateSurcount spForm = new CreateSurcount(DoshiiController, currentObject);
            spForm.ShowDialog();
            RefreshData();
        }

        private void button25_Click(object sender, EventArgs e)
        {
            Surcount currentObject = (Surcount)dataGridView8.CurrentRow.DataBoundItem;
            if (DoshiiController.Doshii.DeleteSurcount(currentObject.Id).Success)
            {
                LiveData.SurcountList.RemoveAll(x => x.Id == currentObject.Id);
                RefreshData();
            }
        }

        private void button35_Click(object sender, EventArgs e)
        {
            MessageBox.Show(this, "functionaity not currently supported by Doshii", "comming soon");
            return;
            Booking currentObject = (Booking)dataGridView5.CurrentRow.DataBoundItem;
            if (DoshiiController.Doshii.DeleteBooking(currentObject.Id).Success)
            {
                LiveData.SurcountList.RemoveAll(x => x.Id == currentObject.Id);
                RefreshData();
            }
        }

        private void button9_Click(object sender, EventArgs e)
        {
            CreateOrder spForm = new CreateOrder(DoshiiController);
            spForm.ShowDialog();
            RefreshData();
        }

        private void button28_Click(object sender, EventArgs e)
        {
            Order currentObject = (Order)dataGridView2.CurrentRow.DataBoundItem;
            CreateOrder spForm = new CreateOrder(DoshiiController, currentObject);
            spForm.ShowDialog();
            RefreshData();
        }

        private void button27_Click(object sender, EventArgs e)
        {
            var order = (Order)dataGridView2.CurrentRow.DataBoundItem;
            //create transaciton,
            //send transaction to doshii
            //complete order on Doshii. 
            //remove order from list. 
            var tran = new Transaction();
            tran.OrderId = order.Id;
            tran.PaymentAmount = OrderTotalHelper.CalculateOrderTotal(order);
            tran.Status = "complete";
            tran.Reference = "miniPos";

            var result = DoshiiController.Doshii.RecordPosTransactionOnDoshii(tran);
            if (result.Success)
            {
                order.Status = "complete";
                var orderResult = DoshiiController.Doshii.UpdateOrder(order);
                if (orderResult.Success)
                {
                    LiveData.OrdersList.RemoveAll(x => x.Id == order.Id);
                    RefreshData();
                }
                else
                {
                    MessageBox.Show(this, "there was an error updating either the transacion of the order on doshii",
                        "order payment error");
                }
            }
        }

        private void button36_Click(object sender, EventArgs e)
        {
            MemberOrg currentObject = (MemberOrg)dataGridView4.CurrentRow.DataBoundItem;
            MemberRewards mrf = new MemberRewards(DoshiiController, currentObject.Id, false);
            mrf.ShowDialog();
        }

        private void button32_Click(object sender, EventArgs e)
        {
            var appsResult = DoshiiController.Doshii.GetApps();
            if (appsResult.Success)
            {
                LiveData.AppsList = appsResult.ReturnObject;
            }
            RefreshData();
        }

        private void button24_Click(object sender, EventArgs e)
        {
            DoshiiController.Doshii.SyncDoshiiMembersWithPosMembers();
        }

        private void button37_Click(object sender, EventArgs e)
        {
            DoshiiController.GetOrdersFromDoshii();
            RefreshData();
        }

        private void button31_Click(object sender, EventArgs e)
        {
            var reservationsList = DoshiiController.Doshii.GetBookings(DateTime.Now.AddDays(-100), DateTime.Now.AddDays(100));
            if (reservationsList.Success)
            {
                LiveData.BookingList = reservationsList.ReturnObject;
            }
            RefreshData();
        }

        private void button38_Click(object sender, EventArgs e)
        {
            Booking currentObject = (Booking)dataGridView5.CurrentRow.DataBoundItem;
            CreateBooking spForm = new CreateBooking(DoshiiController, currentObject);
            spForm.ShowDialog();
            RefreshData();
        }

        private void button13_Click(object sender, EventArgs e)
        {
            CreateBooking spForm = new CreateBooking(DoshiiController);
            spForm.ShowDialog();
            RefreshData();
        }

        private void button22_Click(object sender, EventArgs e)
        {
            MessageBox.Show(this, "functionaity not currently supported by Doshii", "comming soon");
        }

        private void button39_Click(object sender, EventArgs e)
        {
            CreateOrginisation spForm = new CreateOrginisation(DoshiiController);
            spForm.ShowDialog();
            RefreshData();
        }

        private void button40_Click(object sender, EventArgs e)
        {
            RefreshData();
        }

        private void button42_Click(object sender, EventArgs e)
        {
            var locationsResult = DoshiiController.Doshii.GetLocations();
            if (locationsResult.Success)
            {
                LiveData.LocationList = locationsResult.ReturnObject;
            }
            RefreshData();
        }

        private void button41_Click(object sender, EventArgs e)
        {
            var locationsResult = DoshiiController.Doshii.GetLocation();
            if (locationsResult.Success)
            {
                ShowLocation frm = new ShowLocation(locationsResult.ReturnObject);
                frm.ShowDialog();
            }
            else
            {
                MessageBox.Show(this,
                    string.Format("There was an error retreiving the location details from doshii - {0}{1}",
                        locationsResult.responseStatusCode, locationsResult.FailReason));
            }
            RefreshData();
        }

        private void button43_Click(object sender, EventArgs e)
        {
            CreateLocation spForm = new CreateLocation(DoshiiController);
            spForm.ShowDialog();
            RefreshData();
        }

        private void button49_Click(object sender, EventArgs e)
        {
            RefreshData();
        }

        private void button45_Click(object sender, EventArgs e)
        {
            DoshiiController.SyncEmployees();
            RefreshData();
        }

        private void button47_Click(object sender, EventArgs e)
        {
            Employee currentObject = (Employee)dataGridView10.CurrentRow.DataBoundItem;
            if (DoshiiController.Doshii.DeleteEmployee(currentObject.Id).Success)
            {
                LiveData.EmployeeList.RemoveAll(x => x.Id == currentObject.Id);
                RefreshData();
            }
        }

        private void button48_Click(object sender, EventArgs e)
        {
            CreateEmployee spForm = new CreateEmployee(DoshiiController);
            spForm.ShowDialog();
            RefreshData();
        }

        private void button46_Click(object sender, EventArgs e)
        {
            Employee currentObject = (Employee)dataGridView10.CurrentRow.DataBoundItem;
            CreateEmployee spForm = new CreateEmployee(DoshiiController, currentObject);
            spForm.ShowDialog();
            RefreshData();
        }

        private void button51_Click(object sender, EventArgs e)
        {
            RefreshData();
        }

        private void button50_Click(object sender, EventArgs e)
        {
            Transaction currentTransaction = (Transaction)dataGridView11.CurrentRow.DataBoundItem;
            if (currentTransaction != null && !string.IsNullOrEmpty(currentTransaction.OrderId))
            {
                var currentOrderResult = DoshiiController.Doshii.GetOrder(currentTransaction.OrderId);
                if (currentOrderResult != null && currentOrderResult.ReturnObject != null)
                {
                    var result = DoshiiController.Doshii.RequestRefundFromPartner(currentOrderResult.ReturnObject,
                        currentTransaction.PaymentAmount * -1, currentTransaction.LinkedTrxIds);
                    if (result != null && result.Success)
                    {
                        MessageBox.Show(this, "request refund failed");
                    }
                    else
                    {
                        MessageBox.Show(this, "request refund success");
                    }
                } 
            }
            RefreshData();
        }
    }
}
