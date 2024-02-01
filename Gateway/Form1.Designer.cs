namespace Gateway
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
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.lbl_save = new System.Windows.Forms.Label();
            this.comBox_Time = new System.Windows.Forms.ComboBox();
            this.btn_GetValue = new System.Windows.Forms.Button();
            this.btn_stop = new System.Windows.Forms.Button();
            this.label2 = new System.Windows.Forms.Label();
            this.lbl_Progress = new System.Windows.Forms.Label();
            this.lbl_min2 = new System.Windows.Forms.Label();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.label1 = new System.Windows.Forms.Label();
            this.ProductTypeCmb = new System.Windows.Forms.ComboBox();
            this.SQLRadioBtn = new System.Windows.Forms.RadioButton();
            this.txtBox_Secret = new System.Windows.Forms.TextBox();
            this.btn_Login = new System.Windows.Forms.Button();
            this.lbl_Username = new System.Windows.Forms.Label();
            this.lbl_Sercet = new System.Windows.Forms.Label();
            this.txtBox_Usename = new System.Windows.Forms.TextBox();
            this.lbl_Note = new System.Windows.Forms.Label();
            this.txt_Log = new System.Windows.Forms.TextBox();
            this.TEPRadioBtn = new System.Windows.Forms.RadioButton();
            this.groupBox2.SuspendLayout();
            this.groupBox1.SuspendLayout();
            this.SuspendLayout();
            // 
            // groupBox2
            // 
            this.groupBox2.BackColor = System.Drawing.Color.LightSteelBlue;
            this.groupBox2.Controls.Add(this.lbl_save);
            this.groupBox2.Controls.Add(this.comBox_Time);
            this.groupBox2.Controls.Add(this.btn_GetValue);
            this.groupBox2.Controls.Add(this.btn_stop);
            this.groupBox2.Controls.Add(this.label2);
            this.groupBox2.Controls.Add(this.lbl_Progress);
            this.groupBox2.Controls.Add(this.lbl_min2);
            this.groupBox2.Location = new System.Drawing.Point(12, 173);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(776, 98);
            this.groupBox2.TabIndex = 37;
            this.groupBox2.TabStop = false;
            // 
            // lbl_save
            // 
            this.lbl_save.AutoSize = true;
            this.lbl_save.Location = new System.Drawing.Point(114, 153);
            this.lbl_save.Name = "lbl_save";
            this.lbl_save.Size = new System.Drawing.Size(0, 20);
            this.lbl_save.TabIndex = 34;
            // 
            // comBox_Time
            // 
            this.comBox_Time.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comBox_Time.FormattingEnabled = true;
            this.comBox_Time.Location = new System.Drawing.Point(114, 32);
            this.comBox_Time.Name = "comBox_Time";
            this.comBox_Time.Size = new System.Drawing.Size(121, 28);
            this.comBox_Time.TabIndex = 33;
            // 
            // btn_GetValue
            // 
            this.btn_GetValue.Location = new System.Drawing.Point(400, 25);
            this.btn_GetValue.Name = "btn_GetValue";
            this.btn_GetValue.Size = new System.Drawing.Size(104, 41);
            this.btn_GetValue.TabIndex = 19;
            this.btn_GetValue.Text = "确认";
            this.btn_GetValue.UseVisualStyleBackColor = true;
            this.btn_GetValue.Click += new System.EventHandler(this.btn_GetValue_Click);
            // 
            // btn_stop
            // 
            this.btn_stop.Location = new System.Drawing.Point(570, 25);
            this.btn_stop.Name = "btn_stop";
            this.btn_stop.Size = new System.Drawing.Size(104, 41);
            this.btn_stop.TabIndex = 31;
            this.btn_stop.Text = "停止";
            this.btn_stop.UseVisualStyleBackColor = true;
            this.btn_stop.Click += new System.EventHandler(this.btn_stop_Click);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(9, 35);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(89, 20);
            this.label2.TabIndex = 24;
            this.label2.Text = "请设置频率";
            // 
            // lbl_Progress
            // 
            this.lbl_Progress.AutoSize = true;
            this.lbl_Progress.Location = new System.Drawing.Point(447, 199);
            this.lbl_Progress.Name = "lbl_Progress";
            this.lbl_Progress.Size = new System.Drawing.Size(0, 20);
            this.lbl_Progress.TabIndex = 30;
            // 
            // lbl_min2
            // 
            this.lbl_min2.AutoSize = true;
            this.lbl_min2.Location = new System.Drawing.Point(257, 35);
            this.lbl_min2.Name = "lbl_min2";
            this.lbl_min2.Size = new System.Drawing.Size(41, 20);
            this.lbl_min2.TabIndex = 28;
            this.lbl_min2.Text = "分钟";
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.TEPRadioBtn);
            this.groupBox1.Controls.Add(this.label1);
            this.groupBox1.Controls.Add(this.ProductTypeCmb);
            this.groupBox1.Controls.Add(this.SQLRadioBtn);
            this.groupBox1.Controls.Add(this.txtBox_Secret);
            this.groupBox1.Controls.Add(this.btn_Login);
            this.groupBox1.Controls.Add(this.lbl_Username);
            this.groupBox1.Controls.Add(this.lbl_Sercet);
            this.groupBox1.Controls.Add(this.txtBox_Usename);
            this.groupBox1.Controls.Add(this.lbl_Note);
            this.groupBox1.Location = new System.Drawing.Point(12, 12);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(776, 155);
            this.groupBox1.TabIndex = 36;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "登录";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(20, 46);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(102, 20);
            this.label1.TabIndex = 21;
            this.label1.Text = "Product Type";
            // 
            // ProductTypeCmb
            // 
            this.ProductTypeCmb.FormattingEnabled = true;
            this.ProductTypeCmb.Location = new System.Drawing.Point(138, 41);
            this.ProductTypeCmb.Name = "ProductTypeCmb";
            this.ProductTypeCmb.Size = new System.Drawing.Size(144, 28);
            this.ProductTypeCmb.TabIndex = 20;
            // 
            // SQLRadioBtn
            // 
            this.SQLRadioBtn.AutoSize = true;
            this.SQLRadioBtn.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.SQLRadioBtn.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(128)))), ((int)(((byte)(0)))));
            this.SQLRadioBtn.Location = new System.Drawing.Point(24, 99);
            this.SQLRadioBtn.Name = "SQLRadioBtn";
            this.SQLRadioBtn.Size = new System.Drawing.Size(117, 26);
            this.SQLRadioBtn.TabIndex = 19;
            this.SQLRadioBtn.TabStop = true;
            this.SQLRadioBtn.Text = "SQL Save";
            this.SQLRadioBtn.UseVisualStyleBackColor = true;
            // 
            // txtBox_Secret
            // 
            this.txtBox_Secret.ForeColor = System.Drawing.Color.Black;
            this.txtBox_Secret.Location = new System.Drawing.Point(444, 99);
            this.txtBox_Secret.Name = "txtBox_Secret";
            this.txtBox_Secret.PasswordChar = '*';
            this.txtBox_Secret.Size = new System.Drawing.Size(118, 26);
            this.txtBox_Secret.TabIndex = 15;
            // 
            // btn_Login
            // 
            this.btn_Login.Location = new System.Drawing.Point(656, 68);
            this.btn_Login.Name = "btn_Login";
            this.btn_Login.Size = new System.Drawing.Size(100, 39);
            this.btn_Login.TabIndex = 11;
            this.btn_Login.Text = "登录";
            this.btn_Login.UseVisualStyleBackColor = true;
            this.btn_Login.Click += new System.EventHandler(this.btn_Login_Click_1);
            // 
            // lbl_Username
            // 
            this.lbl_Username.AutoSize = true;
            this.lbl_Username.Location = new System.Drawing.Point(356, 49);
            this.lbl_Username.Name = "lbl_Username";
            this.lbl_Username.Size = new System.Drawing.Size(57, 20);
            this.lbl_Username.TabIndex = 12;
            this.lbl_Username.Text = "用户名";
            // 
            // lbl_Sercet
            // 
            this.lbl_Sercet.AutoSize = true;
            this.lbl_Sercet.Location = new System.Drawing.Point(359, 102);
            this.lbl_Sercet.Name = "lbl_Sercet";
            this.lbl_Sercet.Size = new System.Drawing.Size(53, 20);
            this.lbl_Sercet.TabIndex = 13;
            this.lbl_Sercet.Text = "密   码";
            // 
            // txtBox_Usename
            // 
            this.txtBox_Usename.Location = new System.Drawing.Point(444, 49);
            this.txtBox_Usename.Name = "txtBox_Usename";
            this.txtBox_Usename.Size = new System.Drawing.Size(118, 26);
            this.txtBox_Usename.TabIndex = 14;
            // 
            // lbl_Note
            // 
            this.lbl_Note.AutoSize = true;
            this.lbl_Note.Location = new System.Drawing.Point(565, 77);
            this.lbl_Note.Name = "lbl_Note";
            this.lbl_Note.Size = new System.Drawing.Size(0, 20);
            this.lbl_Note.TabIndex = 16;
            // 
            // txt_Log
            // 
            this.txt_Log.Location = new System.Drawing.Point(12, 277);
            this.txt_Log.Multiline = true;
            this.txt_Log.Name = "txt_Log";
            this.txt_Log.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.txt_Log.Size = new System.Drawing.Size(776, 203);
            this.txt_Log.TabIndex = 38;
            // 
            // TEPRadioBtn
            // 
            this.TEPRadioBtn.AutoSize = true;
            this.TEPRadioBtn.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.TEPRadioBtn.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(128)))), ((int)(((byte)(0)))));
            this.TEPRadioBtn.Location = new System.Drawing.Point(165, 99);
            this.TEPRadioBtn.Name = "TEPRadioBtn";
            this.TEPRadioBtn.Size = new System.Drawing.Size(117, 26);
            this.TEPRadioBtn.TabIndex = 22;
            this.TEPRadioBtn.TabStop = true;
            this.TEPRadioBtn.Text = "TEP  Test";
            this.TEPRadioBtn.UseVisualStyleBackColor = true;
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(9F, 20F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(797, 484);
            this.Controls.Add(this.txt_Log);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.groupBox1);
            this.Name = "Form1";
            this.Text = "Gateway Monitor";
            this.Load += new System.EventHandler(this.Form1_Load);
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.Label lbl_save;
        private System.Windows.Forms.ComboBox comBox_Time;
        private System.Windows.Forms.Button btn_GetValue;
        private System.Windows.Forms.Button btn_stop;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label lbl_Progress;
        private System.Windows.Forms.Label lbl_min2;
        private System.Windows.Forms.GroupBox groupBox1;
        protected internal System.Windows.Forms.TextBox txtBox_Secret;
        private System.Windows.Forms.Button btn_Login;
        private System.Windows.Forms.Label lbl_Username;
        private System.Windows.Forms.Label lbl_Sercet;
        private System.Windows.Forms.TextBox txtBox_Usename;
        private System.Windows.Forms.Label lbl_Note;
        private System.Windows.Forms.TextBox txt_Log;
        private System.Windows.Forms.RadioButton SQLRadioBtn;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.ComboBox ProductTypeCmb;
        private System.Windows.Forms.RadioButton TEPRadioBtn;
    }
}