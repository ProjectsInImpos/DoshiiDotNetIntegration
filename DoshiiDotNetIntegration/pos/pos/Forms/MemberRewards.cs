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
    public partial class MemberRewards : Form
    {
        private string TheMemberId;
        private BindingList<Reward> TheRewardsList;
        public Reward TheSelectedReward;
        
        private bool isEditingProduct = false;
        private PosDoshiiController DoshiiController;
        private string oldTableName = "";
        public bool IsToApply = false;

        public MemberRewards(PosDoshiiController doshiiController, string theMemberId, bool isToApply)
        {
            InitializeComponent();
            IsToApply = isToApply;
            DoshiiController = doshiiController;
            TheMemberId = theMemberId;
            GetRewards();
            if (!isToApply)
            {
                button2.Enabled = false;
                button2.Visible = false;
            }
        }

        private void GetRewards()
        {
            var response = DoshiiController.Doshii.GetRewardsForMember(TheMemberId);
            if (response.Success)
            {
                TheRewardsList = new BindingList<Reward>(response.ReturnObject);
                dataGridView2.DataSource = null;
                dataGridView2.DataSource = TheRewardsList;
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            Reward currentObject = (Reward)dataGridView2.CurrentRow.DataBoundItem;
            if (currentObject != null)
            {
                TheSelectedReward = currentObject;
                this.Close();
            }
        }
    }
}
