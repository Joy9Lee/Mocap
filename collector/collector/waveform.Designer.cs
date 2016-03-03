namespace waveformClib
{
    partial class waveform
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

        #region 组件设计器生成的代码

        /// <summary>
        /// 设计器支持所需的方法 - 不要
        /// 使用代码编辑器修改此方法的内容。
        /// </summary>
        private void InitializeComponent()
        {
            this.pictureBoxBottom = new System.Windows.Forms.PictureBox();
            this.pictureBoxLeft = new System.Windows.Forms.PictureBox();
            this.pictureBoxRight = new System.Windows.Forms.PictureBox();
            this.pictureBoxTop = new System.Windows.Forms.PictureBox();
            this.pictureBoxGraph = new System.Windows.Forms.PictureBox();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxBottom)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxLeft)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxRight)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxTop)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxGraph)).BeginInit();
            this.SuspendLayout();
            // 
            // pictureBoxBottom
            // 
            this.pictureBoxBottom.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)
            | System.Windows.Forms.AnchorStyles.Right)));
            this.pictureBoxBottom.BackColor = System.Drawing.Color.White;
            this.pictureBoxBottom.ErrorImage = null;
            this.pictureBoxBottom.InitialImage = null;
            this.pictureBoxBottom.Location = new System.Drawing.Point(0, 121);
            this.pictureBoxBottom.Margin = new System.Windows.Forms.Padding(0);
            this.pictureBoxBottom.MinimumSize = new System.Drawing.Size(0, 10);
            this.pictureBoxBottom.Name = "pictureBoxBottom";
            this.pictureBoxBottom.Size = new System.Drawing.Size(631, 20);
            this.pictureBoxBottom.TabIndex = 0;
            this.pictureBoxBottom.TabStop = false;
            this.pictureBoxBottom.Paint += new System.Windows.Forms.PaintEventHandler(this.pictureBoxBottom_Paint);
            // 
            // pictureBoxLeft
            // 
            this.pictureBoxLeft.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
            | System.Windows.Forms.AnchorStyles.Left)));
            this.pictureBoxLeft.BackColor = System.Drawing.Color.White;
            this.pictureBoxLeft.ErrorImage = null;
            this.pictureBoxLeft.InitialImage = null;
            this.pictureBoxLeft.Location = new System.Drawing.Point(0, 0);
            this.pictureBoxLeft.Margin = new System.Windows.Forms.Padding(0);
            this.pictureBoxLeft.MinimumSize = new System.Drawing.Size(10, 0);
            this.pictureBoxLeft.Name = "pictureBoxLeft";
            this.pictureBoxLeft.Size = new System.Drawing.Size(50, 121);
            this.pictureBoxLeft.TabIndex = 1;
            this.pictureBoxLeft.TabStop = false;
            this.pictureBoxLeft.Paint += new System.Windows.Forms.PaintEventHandler(this.pictureBoxLeft_Paint);
            // 
            // pictureBoxRight
            // 
            this.pictureBoxRight.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
            | System.Windows.Forms.AnchorStyles.Right)));
            this.pictureBoxRight.BackColor = System.Drawing.Color.White;
            this.pictureBoxRight.ErrorImage = null;
            this.pictureBoxRight.InitialImage = null;
            this.pictureBoxRight.Location = new System.Drawing.Point(631, 0);
            this.pictureBoxRight.Margin = new System.Windows.Forms.Padding(0);
            this.pictureBoxRight.MinimumSize = new System.Drawing.Size(10, 0);
            this.pictureBoxRight.Name = "pictureBoxRight";
            this.pictureBoxRight.Size = new System.Drawing.Size(20, 141);
            this.pictureBoxRight.TabIndex = 2;
            this.pictureBoxRight.TabStop = false;
            this.pictureBoxRight.Paint += new System.Windows.Forms.PaintEventHandler(this.pictureBoxRight_Paint);
            // 
            // pictureBoxTop
            // 
            this.pictureBoxTop.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
            | System.Windows.Forms.AnchorStyles.Right)));
            this.pictureBoxTop.BackColor = System.Drawing.Color.White;
            this.pictureBoxTop.ErrorImage = null;
            this.pictureBoxTop.InitialImage = null;
            this.pictureBoxTop.Location = new System.Drawing.Point(50, 0);
            this.pictureBoxTop.Margin = new System.Windows.Forms.Padding(0);
            this.pictureBoxTop.MinimumSize = new System.Drawing.Size(0, 10);
            this.pictureBoxTop.Name = "pictureBoxTop";
            this.pictureBoxTop.Size = new System.Drawing.Size(581, 20);
            this.pictureBoxTop.TabIndex = 3;
            this.pictureBoxTop.TabStop = false;
            this.pictureBoxTop.Paint += new System.Windows.Forms.PaintEventHandler(this.pictureBoxTop_Paint);
            // 
            // pictureBoxGraph
            // 

            this.pictureBoxGraph.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
            | System.Windows.Forms.AnchorStyles.Left)
            | System.Windows.Forms.AnchorStyles.Right)));
            this.pictureBoxGraph.BackColor = System.Drawing.Color.Black;
            this.pictureBoxGraph.ErrorImage = null;
            this.pictureBoxGraph.InitialImage = null;
            this.pictureBoxGraph.Location = new System.Drawing.Point(50, 20);
            this.pictureBoxGraph.Margin = new System.Windows.Forms.Padding(0);
            this.pictureBoxGraph.Name = "pictureBoxGraph";
            this.pictureBoxGraph.Size = new System.Drawing.Size(581, 101);
            this.pictureBoxGraph.TabIndex = 4;
            this.pictureBoxGraph.TabStop = false;
            this.pictureBoxGraph.Paint += new System.Windows.Forms.PaintEventHandler(this.pictureBoxGraph_Paint);
            // 
            // waveform
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.White;
            this.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.Controls.Add(this.pictureBoxGraph);
            this.Controls.Add(this.pictureBoxTop);
            this.Controls.Add(this.pictureBoxRight);
            this.Controls.Add(this.pictureBoxLeft);
            this.Controls.Add(this.pictureBoxBottom);
            this.Margin = new System.Windows.Forms.Padding(0);
            this.MinimumSize = new System.Drawing.Size(390, 50);
            this.Name = "waveform";
            this.Size = new System.Drawing.Size(650, 141);
            this.Resize += new System.EventHandler(this.ZGraph_Resize);
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxBottom)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxLeft)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxRight)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxTop)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxGraph)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion


        private System.Windows.Forms.PictureBox pictureBoxBottom;
        private System.Windows.Forms.PictureBox pictureBoxLeft;
        private System.Windows.Forms.PictureBox pictureBoxRight;
        private System.Windows.Forms.PictureBox pictureBoxTop;
        private System.Windows.Forms.PictureBox pictureBoxGraph;


    }
}
