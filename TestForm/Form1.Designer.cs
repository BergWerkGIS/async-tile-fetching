namespace Mapbox.Platform {
	partial class Form1 {
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.IContainer components = null;

		/// <summary>
		/// Clean up any resources being used.
		/// </summary>
		/// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
		protected override void Dispose(bool disposing) {
			if (disposing && (components != null)) {
				components.Dispose();
			}
			base.Dispose(disposing);
		}

		#region Windows Form Designer generated code

		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent() {
			this.outerLoopStart = new System.Windows.Forms.NumericUpDown();
			this.outerLoopStop = new System.Windows.Forms.NumericUpDown();
			this.label1 = new System.Windows.Forms.Label();
			this.label2 = new System.Windows.Forms.Label();
			this.tileXstart = new System.Windows.Forms.NumericUpDown();
			this.tileXstop = new System.Windows.Forms.NumericUpDown();
			this.btnGo = new System.Windows.Forms.Button();
			this.lvInfo = new System.Windows.Forms.ListView();
			this.columnHeader1 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
			this.lblReqCnt = new System.Windows.Forms.Label();
			this.lblRespCnt = new System.Windows.Forms.Label();
			this.lblTodoCnt = new System.Windows.Forms.Label();
			this.btnGo2 = new System.Windows.Forms.Button();
			this.btnGo3 = new System.Windows.Forms.Button();
			this.IDC_chkDecodeVTs = new System.Windows.Forms.CheckBox();
			((System.ComponentModel.ISupportInitialize)(this.outerLoopStart)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.outerLoopStop)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.tileXstart)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.tileXstop)).BeginInit();
			this.SuspendLayout();
			// 
			// outerLoopStart
			// 
			this.outerLoopStart.Location = new System.Drawing.Point(157, 12);
			this.outerLoopStart.Name = "outerLoopStart";
			this.outerLoopStart.Size = new System.Drawing.Size(120, 20);
			this.outerLoopStart.TabIndex = 0;
			this.outerLoopStart.ValueChanged += new System.EventHandler(this.outerLoopStart_ValueChanged);
			// 
			// outerLoopStop
			// 
			this.outerLoopStop.Location = new System.Drawing.Point(283, 12);
			this.outerLoopStop.Name = "outerLoopStop";
			this.outerLoopStop.Size = new System.Drawing.Size(120, 20);
			this.outerLoopStop.TabIndex = 1;
			this.outerLoopStop.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
			this.outerLoopStop.ValueChanged += new System.EventHandler(this.outerLoopStop_ValueChanged);
			// 
			// label1
			// 
			this.label1.AutoSize = true;
			this.label1.Location = new System.Drawing.Point(40, 19);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(54, 13);
			this.label1.TabIndex = 2;
			this.label1.Text = "outer loop";
			// 
			// label2
			// 
			this.label2.AutoSize = true;
			this.label2.Location = new System.Drawing.Point(40, 37);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(28, 13);
			this.label2.TabIndex = 3;
			this.label2.Text = "tile x";
			// 
			// tileXstart
			// 
			this.tileXstart.Location = new System.Drawing.Point(157, 35);
			this.tileXstart.Maximum = new decimal(new int[] {
            74984,
            0,
            0,
            0});
			this.tileXstart.Name = "tileXstart";
			this.tileXstart.Size = new System.Drawing.Size(120, 20);
			this.tileXstart.TabIndex = 4;
			this.tileXstart.Value = new decimal(new int[] {
            74983,
            0,
            0,
            0});
			this.tileXstart.ValueChanged += new System.EventHandler(this.tileXstart_ValueChanged);
			// 
			// tileXstop
			// 
			this.tileXstop.Location = new System.Drawing.Point(283, 35);
			this.tileXstop.Maximum = new decimal(new int[] {
            74984,
            0,
            0,
            0});
			this.tileXstop.Name = "tileXstop";
			this.tileXstop.Size = new System.Drawing.Size(120, 20);
			this.tileXstop.TabIndex = 5;
			this.tileXstop.Value = new decimal(new int[] {
            74984,
            0,
            0,
            0});
			this.tileXstop.ValueChanged += new System.EventHandler(this.tileXstop_ValueChanged);
			// 
			// btnGo
			// 
			this.btnGo.Location = new System.Drawing.Point(43, 76);
			this.btnGo.Name = "btnGo";
			this.btnGo.Size = new System.Drawing.Size(360, 23);
			this.btnGo.TabIndex = 6;
			this.btnGo.Text = "-= GO =-";
			this.btnGo.UseVisualStyleBackColor = true;
			this.btnGo.Click += new System.EventHandler(this.btnGo_Click);
			// 
			// lvInfo
			// 
			this.lvInfo.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.lvInfo.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnHeader1});
			this.lvInfo.GridLines = true;
			this.lvInfo.Location = new System.Drawing.Point(12, 105);
			this.lvInfo.Name = "lvInfo";
			this.lvInfo.Size = new System.Drawing.Size(728, 441);
			this.lvInfo.TabIndex = 7;
			this.lvInfo.UseCompatibleStateImageBehavior = false;
			this.lvInfo.View = System.Windows.Forms.View.Details;
			// 
			// columnHeader1
			// 
			this.columnHeader1.Text = "info";
			this.columnHeader1.Width = 600;
			// 
			// lblReqCnt
			// 
			this.lblReqCnt.Location = new System.Drawing.Point(437, 19);
			this.lblReqCnt.Name = "lblReqCnt";
			this.lblReqCnt.Size = new System.Drawing.Size(100, 18);
			this.lblReqCnt.TabIndex = 8;
			this.lblReqCnt.Text = "request count";
			// 
			// lblRespCnt
			// 
			this.lblRespCnt.Location = new System.Drawing.Point(437, 37);
			this.lblRespCnt.Name = "lblRespCnt";
			this.lblRespCnt.Size = new System.Drawing.Size(100, 18);
			this.lblRespCnt.TabIndex = 9;
			this.lblRespCnt.Text = "response count";
			// 
			// lblTodoCnt
			// 
			this.lblTodoCnt.Location = new System.Drawing.Point(437, 64);
			this.lblTodoCnt.Name = "lblTodoCnt";
			this.lblTodoCnt.Size = new System.Drawing.Size(100, 23);
			this.lblTodoCnt.TabIndex = 10;
			this.lblTodoCnt.Text = "total tile count";
			// 
			// btnGo2
			// 
			this.btnGo2.Location = new System.Drawing.Point(534, 76);
			this.btnGo2.Name = "btnGo2";
			this.btnGo2.Size = new System.Drawing.Size(75, 23);
			this.btnGo2.TabIndex = 11;
			this.btnGo2.Text = "go 2";
			this.btnGo2.UseVisualStyleBackColor = true;
			this.btnGo2.Click += new System.EventHandler(this.btnGo2_Click);
			// 
			// btnGo3
			// 
			this.btnGo3.Location = new System.Drawing.Point(615, 76);
			this.btnGo3.Name = "btnGo3";
			this.btnGo3.Size = new System.Drawing.Size(75, 23);
			this.btnGo3.TabIndex = 12;
			this.btnGo3.Text = "go 3";
			this.btnGo3.UseVisualStyleBackColor = true;
			this.btnGo3.Click += new System.EventHandler(this.btnGo3_Click);
			// 
			// IDC_chkDecodeVTs
			// 
			this.IDC_chkDecodeVTs.AutoSize = true;
			this.IDC_chkDecodeVTs.Location = new System.Drawing.Point(43, 53);
			this.IDC_chkDecodeVTs.Name = "IDC_chkDecodeVTs";
			this.IDC_chkDecodeVTs.Size = new System.Drawing.Size(103, 17);
			this.IDC_chkDecodeVTs.TabIndex = 13;
			this.IDC_chkDecodeVTs.Text = "decode VT data";
			this.IDC_chkDecodeVTs.UseVisualStyleBackColor = true;
			// 
			// Form1
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(752, 558);
			this.Controls.Add(this.IDC_chkDecodeVTs);
			this.Controls.Add(this.btnGo3);
			this.Controls.Add(this.btnGo2);
			this.Controls.Add(this.lblTodoCnt);
			this.Controls.Add(this.lblRespCnt);
			this.Controls.Add(this.lblReqCnt);
			this.Controls.Add(this.lvInfo);
			this.Controls.Add(this.btnGo);
			this.Controls.Add(this.tileXstop);
			this.Controls.Add(this.tileXstart);
			this.Controls.Add(this.label2);
			this.Controls.Add(this.label1);
			this.Controls.Add(this.outerLoopStop);
			this.Controls.Add(this.outerLoopStart);
			this.Name = "Form1";
			this.Text = "Form1";
			this.Load += new System.EventHandler(this.Form1_Load);
			((System.ComponentModel.ISupportInitialize)(this.outerLoopStart)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.outerLoopStop)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.tileXstart)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.tileXstop)).EndInit();
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.NumericUpDown outerLoopStart;
		private System.Windows.Forms.NumericUpDown outerLoopStop;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.NumericUpDown tileXstart;
		private System.Windows.Forms.NumericUpDown tileXstop;
		private System.Windows.Forms.Button btnGo;
		private System.Windows.Forms.ListView lvInfo;
		private System.Windows.Forms.ColumnHeader columnHeader1;
		private System.Windows.Forms.Label lblReqCnt;
		private System.Windows.Forms.Label lblRespCnt;
		private System.Windows.Forms.Label lblTodoCnt;
		private System.Windows.Forms.Button btnGo2;
		private System.Windows.Forms.Button btnGo3;
		private System.Windows.Forms.CheckBox IDC_chkDecodeVTs;
	}
}

