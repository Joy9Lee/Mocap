namespace collector
{
    partial class Main_Form
    {
        /// <summary>
        /// 必需的设计器变量。
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// 清理所有正在使用的资源。
        /// </summary>
        /// <param name="disposing">如果应释放托管资源，为 true；否则为 false。</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows 窗体设计器生成的代码

        /// <summary>
        /// 设计器支持所需的方法 - 不要
        /// 使用代码编辑器修改此方法的内容。
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Main_Form));
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.yAxisLable = new System.Windows.Forms.Label();
            this.xAxisLable = new System.Windows.Forms.Label();
            this.xAxisText = new System.Windows.Forms.TextBox();
            this.AxisSet = new System.Windows.Forms.Button();
            this.yAxisText = new System.Windows.Forms.TextBox();
            this.Boardcast = new System.Windows.Forms.Button();
            this.Connect = new System.Windows.Forms.Button();
            this.Start = new System.Windows.Forms.Button();
            this.NodeList = new System.Windows.Forms.ListView();
            this.Node = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.IpAddress = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.State = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.ChannelCheck = new System.Windows.Forms.CheckedListBox();
            this.Profiles = new System.Windows.Forms.Button();
            this.MotionBox = new System.Windows.Forms.GroupBox();
            this.GyoBox = new System.Windows.Forms.GroupBox();
            this.z_gyo_label = new System.Windows.Forms.Label();
            this.y_gyo_label = new System.Windows.Forms.Label();
            this.x_gyo_label = new System.Windows.Forms.Label();
            this.z_gyo_text = new System.Windows.Forms.TextBox();
            this.y_gyo_text = new System.Windows.Forms.TextBox();
            this.x_gyo_text = new System.Windows.Forms.TextBox();
            this.MegBox = new System.Windows.Forms.GroupBox();
            this.z_meg_label = new System.Windows.Forms.Label();
            this.y_meg_label = new System.Windows.Forms.Label();
            this.x_meg_label = new System.Windows.Forms.Label();
            this.z_meg_text = new System.Windows.Forms.TextBox();
            this.y_meg_text = new System.Windows.Forms.TextBox();
            this.x_meg_text = new System.Windows.Forms.TextBox();
            this.AccBox = new System.Windows.Forms.GroupBox();
            this.z_acc_label = new System.Windows.Forms.Label();
            this.y_acc_label = new System.Windows.Forms.Label();
            this.x_acc_label = new System.Windows.Forms.Label();
            this.z_acc_text = new System.Windows.Forms.TextBox();
            this.y_acc_text = new System.Windows.Forms.TextBox();
            this.x_acc_text = new System.Windows.Forms.TextBox();
            this.backgroundWorker1 = new System.ComponentModel.BackgroundWorker();
            this.axUnityWebPlayer1 = new AxUnityWebPlayerAXLib.AxUnityWebPlayer();
            this.waveform1 = new waveformClib.waveform();
            this.groupBox1.SuspendLayout();
            this.MotionBox.SuspendLayout();
            this.GyoBox.SuspendLayout();
            this.MegBox.SuspendLayout();
            this.AccBox.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.axUnityWebPlayer1)).BeginInit();
            this.SuspendLayout();
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.yAxisLable);
            this.groupBox1.Controls.Add(this.xAxisLable);
            this.groupBox1.Controls.Add(this.xAxisText);
            this.groupBox1.Controls.Add(this.AxisSet);
            this.groupBox1.Controls.Add(this.yAxisText);
            this.groupBox1.Location = new System.Drawing.Point(714, 27);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(168, 109);
            this.groupBox1.TabIndex = 9;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "groupBox1";
            // 
            // yAxisLable
            // 
            this.yAxisLable.AutoSize = true;
            this.yAxisLable.Location = new System.Drawing.Point(8, 53);
            this.yAxisLable.Name = "yAxisLable";
            this.yAxisLable.Size = new System.Drawing.Size(35, 12);
            this.yAxisLable.TabIndex = 9;
            this.yAxisLable.Text = "yAxis";
            // 
            // xAxisLable
            // 
            this.xAxisLable.AutoSize = true;
            this.xAxisLable.Location = new System.Drawing.Point(8, 27);
            this.xAxisLable.Name = "xAxisLable";
            this.xAxisLable.Size = new System.Drawing.Size(35, 12);
            this.xAxisLable.TabIndex = 8;
            this.xAxisLable.Text = "xAxis";
            // 
            // xAxisText
            // 
            this.xAxisText.Location = new System.Drawing.Point(54, 21);
            this.xAxisText.Name = "xAxisText";
            this.xAxisText.Size = new System.Drawing.Size(100, 21);
            this.xAxisText.TabIndex = 5;
            // 
            // AxisSet
            // 
            this.AxisSet.Location = new System.Drawing.Point(54, 75);
            this.AxisSet.Name = "AxisSet";
            this.AxisSet.Size = new System.Drawing.Size(75, 23);
            this.AxisSet.TabIndex = 7;
            this.AxisSet.Text = "Set";
            this.AxisSet.UseVisualStyleBackColor = true;
            this.AxisSet.Click += new System.EventHandler(this.AxisSet_Click);
            // 
            // yAxisText
            // 
            this.yAxisText.Location = new System.Drawing.Point(54, 48);
            this.yAxisText.Name = "yAxisText";
            this.yAxisText.Size = new System.Drawing.Size(100, 21);
            this.yAxisText.TabIndex = 6;
            // 
            // Boardcast
            // 
            this.Boardcast.Location = new System.Drawing.Point(584, 357);
            this.Boardcast.Name = "Boardcast";
            this.Boardcast.Size = new System.Drawing.Size(75, 23);
            this.Boardcast.TabIndex = 10;
            this.Boardcast.Text = "Boardcast";
            this.Boardcast.UseVisualStyleBackColor = true;
            this.Boardcast.Click += new System.EventHandler(this.Boardcast_Click);
            // 
            // Connect
            // 
            this.Connect.Location = new System.Drawing.Point(584, 398);
            this.Connect.Name = "Connect";
            this.Connect.Size = new System.Drawing.Size(75, 23);
            this.Connect.TabIndex = 11;
            this.Connect.Text = "Connect";
            this.Connect.UseVisualStyleBackColor = true;
            this.Connect.Click += new System.EventHandler(this.Connect_Click);
            // 
            // Start
            // 
            this.Start.Location = new System.Drawing.Point(584, 441);
            this.Start.Name = "Start";
            this.Start.Size = new System.Drawing.Size(75, 23);
            this.Start.TabIndex = 12;
            this.Start.Text = "Start";
            this.Start.UseVisualStyleBackColor = true;
            this.Start.Click += new System.EventHandler(this.Start_Click);
            // 
            // NodeList
            // 
            this.NodeList.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.Node,
            this.IpAddress,
            this.State});
            this.NodeList.FullRowSelect = true;
            this.NodeList.GridLines = true;
            this.NodeList.Location = new System.Drawing.Point(676, 483);
            this.NodeList.Name = "NodeList";
            this.NodeList.Size = new System.Drawing.Size(233, 172);
            this.NodeList.TabIndex = 13;
            this.NodeList.UseCompatibleStateImageBehavior = false;
            this.NodeList.View = System.Windows.Forms.View.Details;
            // 
            // Node
            // 
            this.Node.Tag = "";
            this.Node.Text = "Node";
            this.Node.Width = 59;
            // 
            // IpAddress
            // 
            this.IpAddress.Text = "IpAddress";
            this.IpAddress.Width = 90;
            // 
            // State
            // 
            this.State.Text = "State";
            this.State.Width = 62;
            // 
            // ChannelCheck
            // 
            this.ChannelCheck.FormattingEnabled = true;
            this.ChannelCheck.Items.AddRange(new object[] {
            "sEMG1",
            "sEMG2",
            "sEMG3",
            "sEMG4"});
            this.ChannelCheck.Location = new System.Drawing.Point(595, 587);
            this.ChannelCheck.Name = "ChannelCheck";
            this.ChannelCheck.Size = new System.Drawing.Size(64, 68);
            this.ChannelCheck.TabIndex = 14;
            // 
            // Profiles
            // 
            this.Profiles.Location = new System.Drawing.Point(584, 483);
            this.Profiles.Name = "Profiles";
            this.Profiles.Size = new System.Drawing.Size(75, 23);
            this.Profiles.TabIndex = 15;
            this.Profiles.Text = "Profiles";
            this.Profiles.UseVisualStyleBackColor = true;
            this.Profiles.Click += new System.EventHandler(this.Profiles_Click);
            // 
            // MotionBox
            // 
            this.MotionBox.Controls.Add(this.GyoBox);
            this.MotionBox.Controls.Add(this.MegBox);
            this.MotionBox.Controls.Add(this.AccBox);
            this.MotionBox.Location = new System.Drawing.Point(709, 187);
            this.MotionBox.Name = "MotionBox";
            this.MotionBox.Size = new System.Drawing.Size(200, 245);
            this.MotionBox.TabIndex = 16;
            this.MotionBox.TabStop = false;
            this.MotionBox.Text = "MotionBox";
            // 
            // GyoBox
            // 
            this.GyoBox.Controls.Add(this.z_gyo_label);
            this.GyoBox.Controls.Add(this.y_gyo_label);
            this.GyoBox.Controls.Add(this.x_gyo_label);
            this.GyoBox.Controls.Add(this.z_gyo_text);
            this.GyoBox.Controls.Add(this.y_gyo_text);
            this.GyoBox.Controls.Add(this.x_gyo_text);
            this.GyoBox.Location = new System.Drawing.Point(10, 166);
            this.GyoBox.Name = "GyoBox";
            this.GyoBox.Size = new System.Drawing.Size(169, 65);
            this.GyoBox.TabIndex = 0;
            this.GyoBox.TabStop = false;
            this.GyoBox.Text = "GyoBox";
            // 
            // z_gyo_label
            // 
            this.z_gyo_label.AutoSize = true;
            this.z_gyo_label.Location = new System.Drawing.Point(121, 20);
            this.z_gyo_label.Name = "z_gyo_label";
            this.z_gyo_label.Size = new System.Drawing.Size(11, 12);
            this.z_gyo_label.TabIndex = 2;
            this.z_gyo_label.Text = "z";
            // 
            // y_gyo_label
            // 
            this.y_gyo_label.AutoSize = true;
            this.y_gyo_label.Location = new System.Drawing.Point(69, 20);
            this.y_gyo_label.Name = "y_gyo_label";
            this.y_gyo_label.Size = new System.Drawing.Size(11, 12);
            this.y_gyo_label.TabIndex = 2;
            this.y_gyo_label.Text = "y";
            // 
            // x_gyo_label
            // 
            this.x_gyo_label.AutoSize = true;
            this.x_gyo_label.Location = new System.Drawing.Point(15, 20);
            this.x_gyo_label.Name = "x_gyo_label";
            this.x_gyo_label.Size = new System.Drawing.Size(11, 12);
            this.x_gyo_label.TabIndex = 2;
            this.x_gyo_label.Text = "x";
            // 
            // z_gyo_text
            // 
            this.z_gyo_text.Location = new System.Drawing.Point(123, 38);
            this.z_gyo_text.Name = "z_gyo_text";
            this.z_gyo_text.ReadOnly = true;
            this.z_gyo_text.Size = new System.Drawing.Size(40, 21);
            this.z_gyo_text.TabIndex = 1;
            // 
            // y_gyo_text
            // 
            this.y_gyo_text.Location = new System.Drawing.Point(70, 38);
            this.y_gyo_text.Name = "y_gyo_text";
            this.y_gyo_text.ReadOnly = true;
            this.y_gyo_text.Size = new System.Drawing.Size(40, 21);
            this.y_gyo_text.TabIndex = 1;
            // 
            // x_gyo_text
            // 
            this.x_gyo_text.Location = new System.Drawing.Point(15, 38);
            this.x_gyo_text.Name = "x_gyo_text";
            this.x_gyo_text.ReadOnly = true;
            this.x_gyo_text.Size = new System.Drawing.Size(40, 21);
            this.x_gyo_text.TabIndex = 1;
            // 
            // MegBox
            // 
            this.MegBox.Controls.Add(this.z_meg_label);
            this.MegBox.Controls.Add(this.y_meg_label);
            this.MegBox.Controls.Add(this.x_meg_label);
            this.MegBox.Controls.Add(this.z_meg_text);
            this.MegBox.Controls.Add(this.y_meg_text);
            this.MegBox.Controls.Add(this.x_meg_text);
            this.MegBox.Location = new System.Drawing.Point(10, 91);
            this.MegBox.Name = "MegBox";
            this.MegBox.Size = new System.Drawing.Size(169, 65);
            this.MegBox.TabIndex = 0;
            this.MegBox.TabStop = false;
            this.MegBox.Text = "MegBox";
            // 
            // z_meg_label
            // 
            this.z_meg_label.AutoSize = true;
            this.z_meg_label.Location = new System.Drawing.Point(121, 20);
            this.z_meg_label.Name = "z_meg_label";
            this.z_meg_label.Size = new System.Drawing.Size(11, 12);
            this.z_meg_label.TabIndex = 2;
            this.z_meg_label.Text = "z";
            // 
            // y_meg_label
            // 
            this.y_meg_label.AutoSize = true;
            this.y_meg_label.Location = new System.Drawing.Point(69, 20);
            this.y_meg_label.Name = "y_meg_label";
            this.y_meg_label.Size = new System.Drawing.Size(11, 12);
            this.y_meg_label.TabIndex = 2;
            this.y_meg_label.Text = "y";
            // 
            // x_meg_label
            // 
            this.x_meg_label.AutoSize = true;
            this.x_meg_label.Location = new System.Drawing.Point(15, 20);
            this.x_meg_label.Name = "x_meg_label";
            this.x_meg_label.Size = new System.Drawing.Size(11, 12);
            this.x_meg_label.TabIndex = 2;
            this.x_meg_label.Text = "x";
            // 
            // z_meg_text
            // 
            this.z_meg_text.Location = new System.Drawing.Point(123, 38);
            this.z_meg_text.Name = "z_meg_text";
            this.z_meg_text.ReadOnly = true;
            this.z_meg_text.Size = new System.Drawing.Size(40, 21);
            this.z_meg_text.TabIndex = 1;
            // 
            // y_meg_text
            // 
            this.y_meg_text.Location = new System.Drawing.Point(70, 38);
            this.y_meg_text.Name = "y_meg_text";
            this.y_meg_text.ReadOnly = true;
            this.y_meg_text.Size = new System.Drawing.Size(40, 21);
            this.y_meg_text.TabIndex = 1;
            // 
            // x_meg_text
            // 
            this.x_meg_text.Location = new System.Drawing.Point(15, 38);
            this.x_meg_text.Name = "x_meg_text";
            this.x_meg_text.ReadOnly = true;
            this.x_meg_text.Size = new System.Drawing.Size(40, 21);
            this.x_meg_text.TabIndex = 1;
            // 
            // AccBox
            // 
            this.AccBox.Controls.Add(this.z_acc_label);
            this.AccBox.Controls.Add(this.y_acc_label);
            this.AccBox.Controls.Add(this.x_acc_label);
            this.AccBox.Controls.Add(this.z_acc_text);
            this.AccBox.Controls.Add(this.y_acc_text);
            this.AccBox.Controls.Add(this.x_acc_text);
            this.AccBox.Location = new System.Drawing.Point(10, 20);
            this.AccBox.Name = "AccBox";
            this.AccBox.Size = new System.Drawing.Size(169, 65);
            this.AccBox.TabIndex = 0;
            this.AccBox.TabStop = false;
            this.AccBox.Text = "AccBox";
            // 
            // z_acc_label
            // 
            this.z_acc_label.AutoSize = true;
            this.z_acc_label.Location = new System.Drawing.Point(121, 20);
            this.z_acc_label.Name = "z_acc_label";
            this.z_acc_label.Size = new System.Drawing.Size(11, 12);
            this.z_acc_label.TabIndex = 2;
            this.z_acc_label.Text = "z";
            // 
            // y_acc_label
            // 
            this.y_acc_label.AutoSize = true;
            this.y_acc_label.Location = new System.Drawing.Point(69, 20);
            this.y_acc_label.Name = "y_acc_label";
            this.y_acc_label.Size = new System.Drawing.Size(11, 12);
            this.y_acc_label.TabIndex = 2;
            this.y_acc_label.Text = "y";
            // 
            // x_acc_label
            // 
            this.x_acc_label.AutoSize = true;
            this.x_acc_label.Location = new System.Drawing.Point(15, 20);
            this.x_acc_label.Name = "x_acc_label";
            this.x_acc_label.Size = new System.Drawing.Size(11, 12);
            this.x_acc_label.TabIndex = 2;
            this.x_acc_label.Text = "x";
            // 
            // z_acc_text
            // 
            this.z_acc_text.Location = new System.Drawing.Point(123, 38);
            this.z_acc_text.Name = "z_acc_text";
            this.z_acc_text.ReadOnly = true;
            this.z_acc_text.Size = new System.Drawing.Size(40, 21);
            this.z_acc_text.TabIndex = 1;
            // 
            // y_acc_text
            // 
            this.y_acc_text.Location = new System.Drawing.Point(70, 38);
            this.y_acc_text.Name = "y_acc_text";
            this.y_acc_text.ReadOnly = true;
            this.y_acc_text.Size = new System.Drawing.Size(40, 21);
            this.y_acc_text.TabIndex = 1;
            // 
            // x_acc_text
            // 
            this.x_acc_text.Location = new System.Drawing.Point(15, 38);
            this.x_acc_text.Name = "x_acc_text";
            this.x_acc_text.ReadOnly = true;
            this.x_acc_text.Size = new System.Drawing.Size(40, 21);
            this.x_acc_text.TabIndex = 1;
            // 
            // axUnityWebPlayer1
            // 
            this.axUnityWebPlayer1.Enabled = true;
            this.axUnityWebPlayer1.Location = new System.Drawing.Point(36, 337);
            this.axUnityWebPlayer1.Name = "axUnityWebPlayer1";
            this.axUnityWebPlayer1.OcxState = ((System.Windows.Forms.AxHost.State)(resources.GetObject("axUnityWebPlayer1.OcxState")));
            this.axUnityWebPlayer1.Size = new System.Drawing.Size(511, 318);
            this.axUnityWebPlayer1.TabIndex = 17;
            // 
            // waveform1
            // 
            this.waveform1.BackColor = System.Drawing.Color.White;
            this.waveform1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.waveform1.Location = new System.Drawing.Point(36, 18);
            this.waveform1.m_backColorH = System.Drawing.Color.FromArgb(((int)(((byte)(200)))), ((int)(((byte)(200)))), ((int)(((byte)(200)))));
            this.waveform1.m_backColorL = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(255)))), ((int)(((byte)(255)))));
            this.waveform1.m_coordinateLineColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))));
            this.waveform1.m_coordinateStringColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))));
            this.waveform1.m_coordinateStringTitleColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))));
            this.waveform1.m_fXBeginSYS = 0F;
            this.waveform1.m_fXEndSYS = 4F;
            this.waveform1.m_fYBeginSYS = -5F;
            this.waveform1.m_fYEndSYS = 5F;
            this.waveform1.m_GraphBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))));
            this.waveform1.m_iLineShowColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(255)))), ((int)(((byte)(255)))));
            this.waveform1.m_iLineShowColorAlpha = 100;
            this.waveform1.m_SySnameX = "时间";
            this.waveform1.m_SySnameY = "电压";
            this.waveform1.m_SyStitle = "sEMG 通道n";
            this.waveform1.m_titleBorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(250)))), ((int)(((byte)(250)))), ((int)(((byte)(250)))));
            this.waveform1.m_titleColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))));
            this.waveform1.m_titlePosition = 0.4F;
            this.waveform1.m_titleSize = 14;
            this.waveform1.Margin = new System.Windows.Forms.Padding(0);
            this.waveform1.MinimumSize = new System.Drawing.Size(390, 50);
            this.waveform1.Name = "waveform1";
            this.waveform1.Size = new System.Drawing.Size(634, 304);
            this.waveform1.TabIndex = 0;
            // 
            // Main_Form
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(938, 667);
            this.Controls.Add(this.axUnityWebPlayer1);
            this.Controls.Add(this.MotionBox);
            this.Controls.Add(this.Profiles);
            this.Controls.Add(this.ChannelCheck);
            this.Controls.Add(this.NodeList);
            this.Controls.Add(this.Start);
            this.Controls.Add(this.Connect);
            this.Controls.Add(this.Boardcast);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.waveform1);
            this.Name = "Main_Form";
            this.Text = "Form1";
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.MotionBox.ResumeLayout(false);
            this.GyoBox.ResumeLayout(false);
            this.GyoBox.PerformLayout();
            this.MegBox.ResumeLayout(false);
            this.MegBox.PerformLayout();
            this.AccBox.ResumeLayout(false);
            this.AccBox.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.axUnityWebPlayer1)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private waveformClib.waveform waveform1;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Label yAxisLable;
        private System.Windows.Forms.Label xAxisLable;
        private System.Windows.Forms.TextBox xAxisText;
        private System.Windows.Forms.Button AxisSet;
        private System.Windows.Forms.TextBox yAxisText;
        private System.Windows.Forms.Button Boardcast;
        private System.Windows.Forms.Button Connect;
        private System.Windows.Forms.Button Start;
        private System.Windows.Forms.ListView NodeList;
        private System.Windows.Forms.ColumnHeader Node;
        private System.Windows.Forms.ColumnHeader IpAddress;
        private System.Windows.Forms.ColumnHeader State;
        private System.Windows.Forms.CheckedListBox ChannelCheck;
        private System.Windows.Forms.Button Profiles;
        private System.Windows.Forms.GroupBox MotionBox;
        private System.Windows.Forms.GroupBox GyoBox;
        private System.Windows.Forms.Label z_gyo_label;
        private System.Windows.Forms.Label y_gyo_label;
        private System.Windows.Forms.Label x_gyo_label;
        private System.Windows.Forms.TextBox z_gyo_text;
        private System.Windows.Forms.TextBox y_gyo_text;
        private System.Windows.Forms.TextBox x_gyo_text;
        private System.Windows.Forms.GroupBox MegBox;
        private System.Windows.Forms.Label z_meg_label;
        private System.Windows.Forms.Label y_meg_label;
        private System.Windows.Forms.Label x_meg_label;
        private System.Windows.Forms.TextBox z_meg_text;
        private System.Windows.Forms.TextBox y_meg_text;
        private System.Windows.Forms.TextBox x_meg_text;
        private System.Windows.Forms.GroupBox AccBox;
        private System.Windows.Forms.Label z_acc_label;
        private System.Windows.Forms.Label y_acc_label;
        private System.Windows.Forms.Label x_acc_label;
        private System.Windows.Forms.TextBox z_acc_text;
        private System.Windows.Forms.TextBox y_acc_text;
        private System.Windows.Forms.TextBox x_acc_text;
        private System.ComponentModel.BackgroundWorker backgroundWorker1;
        private AxUnityWebPlayerAXLib.AxUnityWebPlayer axUnityWebPlayer1;
    }
}

