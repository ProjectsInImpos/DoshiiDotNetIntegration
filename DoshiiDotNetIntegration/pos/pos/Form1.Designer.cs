namespace pos
{
    partial class Form1
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.chk_socketConnection = new System.Windows.Forms.CheckBox();
            this.button1 = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.txt_baseUrl = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.txt_timeoutSec = new System.Windows.Forms.TextBox();
            this.txt_locationToken = new System.Windows.Forms.TextBox();
            this.txt_doshiiVendor = new System.Windows.Forms.TextBox();
            this.txt_secretKey = new System.Windows.Forms.TextBox();
            this.txt_socketUrl = new System.Windows.Forms.TextBox();
            this.chk_apps = new System.Windows.Forms.CheckBox();
            this.chk_reservations = new System.Windows.Forms.CheckBox();
            this.chk_membership = new System.Windows.Forms.CheckBox();
            this.button2 = new System.Windows.Forms.Button();
            this.textBox1 = new System.Windows.Forms.TextBox();
            this.label7 = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // chk_socketConnection
            // 
            this.chk_socketConnection.AutoSize = true;
            this.chk_socketConnection.Location = new System.Drawing.Point(16, 223);
            this.chk_socketConnection.Name = "chk_socketConnection";
            this.chk_socketConnection.Size = new System.Drawing.Size(139, 17);
            this.chk_socketConnection.TabIndex = 0;
            this.chk_socketConnection.Text = "Use Socket Connection";
            this.chk_socketConnection.UseVisualStyleBackColor = true;
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(198, 223);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(146, 51);
            this.button1.TabIndex = 1;
            this.button1.Text = "Start Pos";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(13, 16);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(44, 13);
            this.label1.TabIndex = 2;
            this.label1.Text = "BaseUrl";
            // 
            // txt_baseUrl
            // 
            this.txt_baseUrl.Location = new System.Drawing.Point(141, 12);
            this.txt_baseUrl.Name = "txt_baseUrl";
            this.txt_baseUrl.Size = new System.Drawing.Size(330, 20);
            this.txt_baseUrl.TabIndex = 3;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(13, 141);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(61, 13);
            this.label2.TabIndex = 4;
            this.label2.Text = "timeout sec";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(13, 116);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(82, 13);
            this.label3.TabIndex = 5;
            this.label3.Text = "Location Token";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(12, 91);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(70, 13);
            this.label4.TabIndex = 6;
            this.label4.Text = "doshii vendor";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(13, 66);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(59, 13);
            this.label5.TabIndex = 7;
            this.label5.Text = "Secret Key";
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(13, 41);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(52, 13);
            this.label6.TabIndex = 8;
            this.label6.Text = "socketUrl";
            // 
            // txt_timeoutSec
            // 
            this.txt_timeoutSec.Location = new System.Drawing.Point(141, 137);
            this.txt_timeoutSec.Name = "txt_timeoutSec";
            this.txt_timeoutSec.Size = new System.Drawing.Size(330, 20);
            this.txt_timeoutSec.TabIndex = 9;
            // 
            // txt_locationToken
            // 
            this.txt_locationToken.Location = new System.Drawing.Point(141, 112);
            this.txt_locationToken.Name = "txt_locationToken";
            this.txt_locationToken.Size = new System.Drawing.Size(330, 20);
            this.txt_locationToken.TabIndex = 10;
            // 
            // txt_doshiiVendor
            // 
            this.txt_doshiiVendor.Location = new System.Drawing.Point(141, 87);
            this.txt_doshiiVendor.Name = "txt_doshiiVendor";
            this.txt_doshiiVendor.Size = new System.Drawing.Size(330, 20);
            this.txt_doshiiVendor.TabIndex = 11;
            // 
            // txt_secretKey
            // 
            this.txt_secretKey.Location = new System.Drawing.Point(141, 62);
            this.txt_secretKey.Name = "txt_secretKey";
            this.txt_secretKey.Size = new System.Drawing.Size(330, 20);
            this.txt_secretKey.TabIndex = 12;
            // 
            // txt_socketUrl
            // 
            this.txt_socketUrl.Location = new System.Drawing.Point(141, 37);
            this.txt_socketUrl.Name = "txt_socketUrl";
            this.txt_socketUrl.Size = new System.Drawing.Size(330, 20);
            this.txt_socketUrl.TabIndex = 13;
            // 
            // chk_apps
            // 
            this.chk_apps.AutoSize = true;
            this.chk_apps.Location = new System.Drawing.Point(16, 292);
            this.chk_apps.Name = "chk_apps";
            this.chk_apps.Size = new System.Drawing.Size(72, 17);
            this.chk_apps.TabIndex = 14;
            this.chk_apps.Text = "Use Apps";
            this.chk_apps.UseVisualStyleBackColor = true;
            // 
            // chk_reservations
            // 
            this.chk_reservations.AutoSize = true;
            this.chk_reservations.Location = new System.Drawing.Point(15, 269);
            this.chk_reservations.Name = "chk_reservations";
            this.chk_reservations.Size = new System.Drawing.Size(105, 17);
            this.chk_reservations.TabIndex = 15;
            this.chk_reservations.Text = "Use Reservation";
            this.chk_reservations.UseVisualStyleBackColor = true;
            // 
            // chk_membership
            // 
            this.chk_membership.AutoSize = true;
            this.chk_membership.Location = new System.Drawing.Point(16, 246);
            this.chk_membership.Name = "chk_membership";
            this.chk_membership.Size = new System.Drawing.Size(105, 17);
            this.chk_membership.TabIndex = 16;
            this.chk_membership.Text = "Use Membership";
            this.chk_membership.UseVisualStyleBackColor = true;
            // 
            // button2
            // 
            this.button2.Location = new System.Drawing.Point(402, 292);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(75, 23);
            this.button2.TabIndex = 17;
            this.button2.Text = "Exit Pos";
            this.button2.UseVisualStyleBackColor = true;
            this.button2.Click += new System.EventHandler(this.button2_Click);
            // 
            // textBox1
            // 
            this.textBox1.Location = new System.Drawing.Point(141, 163);
            this.textBox1.Name = "textBox1";
            this.textBox1.Size = new System.Drawing.Size(330, 20);
            this.textBox1.TabIndex = 19;
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(13, 167);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(78, 13);
            this.label7.TabIndex = 18;
            this.label7.Text = "Organisation Id";
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(489, 324);
            this.Controls.Add(this.textBox1);
            this.Controls.Add(this.label7);
            this.Controls.Add(this.button2);
            this.Controls.Add(this.chk_membership);
            this.Controls.Add(this.chk_reservations);
            this.Controls.Add(this.chk_apps);
            this.Controls.Add(this.txt_socketUrl);
            this.Controls.Add(this.txt_secretKey);
            this.Controls.Add(this.txt_doshiiVendor);
            this.Controls.Add(this.txt_locationToken);
            this.Controls.Add(this.txt_timeoutSec);
            this.Controls.Add(this.label6);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.txt_baseUrl);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.chk_socketConnection);
            this.Name = "Form1";
            this.Text = "Form1";
            this.Load += new System.EventHandler(this.Form1_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.CheckBox chk_socketConnection;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox txt_baseUrl;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.TextBox txt_timeoutSec;
        private System.Windows.Forms.TextBox txt_locationToken;
        private System.Windows.Forms.TextBox txt_doshiiVendor;
        private System.Windows.Forms.TextBox txt_secretKey;
        private System.Windows.Forms.TextBox txt_socketUrl;
        private System.Windows.Forms.CheckBox chk_apps;
        private System.Windows.Forms.CheckBox chk_reservations;
        private System.Windows.Forms.CheckBox chk_membership;
        private System.Windows.Forms.Button button2;
        private System.Windows.Forms.TextBox textBox1;
        private System.Windows.Forms.Label label7;
    }
}

