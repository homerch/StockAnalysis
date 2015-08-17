namespace DataControl
{
    partial class Form1
    {
        /// <summary>
        /// 設計工具所需的變數。
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// 清除任何使用中的資源。
        /// </summary>
        /// <param name="disposing">如果應該處置 Managed 資源則為 true，否則為 false。</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form 設計工具產生的程式碼

        /// <summary>
        /// 此為設計工具支援所需的方法 - 請勿使用程式碼編輯器
        /// 修改這個方法的內容。
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.MainTimer = new System.Windows.Forms.Timer(this.components);
            this.DetailTimer = new System.Windows.Forms.Timer(this.components);
            this.SummaryTimer = new System.Windows.Forms.Timer(this.components);
            this.MainWork = new System.ComponentModel.BackgroundWorker();
            this.RateWorker = new System.ComponentModel.BackgroundWorker();
            this.TestWorker = new System.ComponentModel.BackgroundWorker();
            this.tabControl1 = new System.Windows.Forms.TabControl();
            this.tabPage1 = new System.Windows.Forms.TabPage();
            this.splitContainer1 = new System.Windows.Forms.SplitContainer();
            this.btnSQuery = new System.Windows.Forms.Button();
            this.btnStock = new System.Windows.Forms.Button();
            this.btnBasic = new System.Windows.Forms.Button();
            this.lbStock = new System.Windows.Forms.ListBox();
            this.tabPage3 = new System.Windows.Forms.TabPage();
            this.splitContainer3 = new System.Windows.Forms.SplitContainer();
            this.btnOQuery = new System.Windows.Forms.Button();
            this.listBox3 = new System.Windows.Forms.ListBox();
            this.tabPage4 = new System.Windows.Forms.TabPage();
            this.splitContainer4 = new System.Windows.Forms.SplitContainer();
            this.btnSWQuery = new System.Windows.Forms.Button();
            this.lbSW = new System.Windows.Forms.ListBox();
            this.tabPage5 = new System.Windows.Forms.TabPage();
            this.splitContainer5 = new System.Windows.Forms.SplitContainer();
            this.btnOWQuery = new System.Windows.Forms.Button();
            this.lbOW = new System.Windows.Forms.ListBox();
            this.tabPage2 = new System.Windows.Forms.TabPage();
            this.splitContainer2 = new System.Windows.Forms.SplitContainer();
            this.button4 = new System.Windows.Forms.Button();
            this.dtpRate = new System.Windows.Forms.DateTimePicker();
            this.btnWeeklyRate = new System.Windows.Forms.Button();
            this.listBox1 = new System.Windows.Forms.ListBox();
            this.InitWorker = new System.ComponentModel.BackgroundWorker();
            this.QueryWorker = new System.ComponentModel.BackgroundWorker();
            this.SWWorker = new System.ComponentModel.BackgroundWorker();
            this.OWWorker = new System.ComponentModel.BackgroundWorker();
            this.tabControl1.SuspendLayout();
            this.tabPage1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).BeginInit();
            this.splitContainer1.Panel1.SuspendLayout();
            this.splitContainer1.Panel2.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            this.tabPage3.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer3)).BeginInit();
            this.splitContainer3.Panel1.SuspendLayout();
            this.splitContainer3.Panel2.SuspendLayout();
            this.splitContainer3.SuspendLayout();
            this.tabPage4.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer4)).BeginInit();
            this.splitContainer4.Panel1.SuspendLayout();
            this.splitContainer4.Panel2.SuspendLayout();
            this.splitContainer4.SuspendLayout();
            this.tabPage5.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer5)).BeginInit();
            this.splitContainer5.Panel1.SuspendLayout();
            this.splitContainer5.Panel2.SuspendLayout();
            this.splitContainer5.SuspendLayout();
            this.tabPage2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer2)).BeginInit();
            this.splitContainer2.Panel1.SuspendLayout();
            this.splitContainer2.Panel2.SuspendLayout();
            this.splitContainer2.SuspendLayout();
            this.SuspendLayout();
            // 
            // MainTimer
            // 
            this.MainTimer.Tick += new System.EventHandler(this.MainTimer_Tick);
            // 
            // DetailTimer
            // 
            this.DetailTimer.Tick += new System.EventHandler(this.DetailTimer_Tick);
            // 
            // SummaryTimer
            // 
            this.SummaryTimer.Tick += new System.EventHandler(this.SummaryTimer_Tick);
            // 
            // MainWork
            // 
            this.MainWork.WorkerReportsProgress = true;
            this.MainWork.DoWork += new System.ComponentModel.DoWorkEventHandler(this.MainWork_DoWork);
            // 
            // RateWorker
            // 
            this.RateWorker.WorkerReportsProgress = true;
            this.RateWorker.DoWork += new System.ComponentModel.DoWorkEventHandler(this.RateWorker_DoWork);
            this.RateWorker.ProgressChanged += new System.ComponentModel.ProgressChangedEventHandler(this.RateWorker_ProgressChanged);
            // 
            // TestWorker
            // 
            this.TestWorker.WorkerReportsProgress = true;
            this.TestWorker.DoWork += new System.ComponentModel.DoWorkEventHandler(this.TestWorker_DoWork);
            this.TestWorker.ProgressChanged += new System.ComponentModel.ProgressChangedEventHandler(this.TestWorker_ProgressChanged);
            this.TestWorker.RunWorkerCompleted += new System.ComponentModel.RunWorkerCompletedEventHandler(this.TestWorker_RunWorkerCompleted);
            // 
            // tabControl1
            // 
            this.tabControl1.Controls.Add(this.tabPage1);
            this.tabControl1.Controls.Add(this.tabPage3);
            this.tabControl1.Controls.Add(this.tabPage4);
            this.tabControl1.Controls.Add(this.tabPage5);
            this.tabControl1.Controls.Add(this.tabPage2);
            this.tabControl1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tabControl1.Location = new System.Drawing.Point(0, 0);
            this.tabControl1.Name = "tabControl1";
            this.tabControl1.SelectedIndex = 0;
            this.tabControl1.Size = new System.Drawing.Size(432, 303);
            this.tabControl1.TabIndex = 1;
            // 
            // tabPage1
            // 
            this.tabPage1.Controls.Add(this.splitContainer1);
            this.tabPage1.Location = new System.Drawing.Point(4, 22);
            this.tabPage1.Name = "tabPage1";
            this.tabPage1.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage1.Size = new System.Drawing.Size(424, 277);
            this.tabPage1.TabIndex = 0;
            this.tabPage1.Text = "上市";
            this.tabPage1.UseVisualStyleBackColor = true;
            // 
            // splitContainer1
            // 
            this.splitContainer1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer1.Location = new System.Drawing.Point(3, 3);
            this.splitContainer1.Name = "splitContainer1";
            this.splitContainer1.Orientation = System.Windows.Forms.Orientation.Horizontal;
            // 
            // splitContainer1.Panel1
            // 
            this.splitContainer1.Panel1.Controls.Add(this.btnSQuery);
            this.splitContainer1.Panel1.Controls.Add(this.btnStock);
            this.splitContainer1.Panel1.Controls.Add(this.btnBasic);
            // 
            // splitContainer1.Panel2
            // 
            this.splitContainer1.Panel2.Controls.Add(this.lbStock);
            this.splitContainer1.Size = new System.Drawing.Size(418, 271);
            this.splitContainer1.SplitterDistance = 129;
            this.splitContainer1.TabIndex = 3;
            // 
            // btnSQuery
            // 
            this.btnSQuery.Location = new System.Drawing.Point(178, 22);
            this.btnSQuery.Name = "btnSQuery";
            this.btnSQuery.Size = new System.Drawing.Size(75, 23);
            this.btnSQuery.TabIndex = 3;
            this.btnSQuery.Text = "Query";
            this.btnSQuery.UseVisualStyleBackColor = true;
            this.btnSQuery.Click += new System.EventHandler(this.btnSQuery_Click);
            // 
            // btnStock
            // 
            this.btnStock.Location = new System.Drawing.Point(38, 69);
            this.btnStock.Name = "btnStock";
            this.btnStock.Size = new System.Drawing.Size(75, 23);
            this.btnStock.TabIndex = 2;
            this.btnStock.Text = "StockData";
            this.btnStock.UseVisualStyleBackColor = true;
            this.btnStock.Click += new System.EventHandler(this.btnStock_Click);
            // 
            // btnBasic
            // 
            this.btnBasic.Location = new System.Drawing.Point(38, 22);
            this.btnBasic.Name = "btnBasic";
            this.btnBasic.Size = new System.Drawing.Size(75, 23);
            this.btnBasic.TabIndex = 1;
            this.btnBasic.Text = "BasicData";
            this.btnBasic.UseVisualStyleBackColor = true;
            this.btnBasic.Click += new System.EventHandler(this.btnBasic_Click);
            // 
            // lbStock
            // 
            this.lbStock.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lbStock.FormattingEnabled = true;
            this.lbStock.ItemHeight = 12;
            this.lbStock.Location = new System.Drawing.Point(0, 0);
            this.lbStock.Name = "lbStock";
            this.lbStock.Size = new System.Drawing.Size(418, 138);
            this.lbStock.TabIndex = 0;
            // 
            // tabPage3
            // 
            this.tabPage3.Controls.Add(this.splitContainer3);
            this.tabPage3.Location = new System.Drawing.Point(4, 22);
            this.tabPage3.Name = "tabPage3";
            this.tabPage3.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage3.Size = new System.Drawing.Size(424, 277);
            this.tabPage3.TabIndex = 2;
            this.tabPage3.Text = "上櫃";
            this.tabPage3.UseVisualStyleBackColor = true;
            // 
            // splitContainer3
            // 
            this.splitContainer3.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer3.Location = new System.Drawing.Point(3, 3);
            this.splitContainer3.Name = "splitContainer3";
            this.splitContainer3.Orientation = System.Windows.Forms.Orientation.Horizontal;
            // 
            // splitContainer3.Panel1
            // 
            this.splitContainer3.Panel1.Controls.Add(this.btnOQuery);
            // 
            // splitContainer3.Panel2
            // 
            this.splitContainer3.Panel2.Controls.Add(this.listBox3);
            this.splitContainer3.Size = new System.Drawing.Size(418, 271);
            this.splitContainer3.SplitterDistance = 129;
            this.splitContainer3.TabIndex = 4;
            // 
            // btnOQuery
            // 
            this.btnOQuery.Location = new System.Drawing.Point(172, 53);
            this.btnOQuery.Name = "btnOQuery";
            this.btnOQuery.Size = new System.Drawing.Size(75, 23);
            this.btnOQuery.TabIndex = 1;
            this.btnOQuery.Text = "Query";
            this.btnOQuery.UseVisualStyleBackColor = true;
            this.btnOQuery.Click += new System.EventHandler(this.btnOQuery_Click);
            // 
            // listBox3
            // 
            this.listBox3.Dock = System.Windows.Forms.DockStyle.Fill;
            this.listBox3.FormattingEnabled = true;
            this.listBox3.ItemHeight = 12;
            this.listBox3.Location = new System.Drawing.Point(0, 0);
            this.listBox3.Name = "listBox3";
            this.listBox3.Size = new System.Drawing.Size(418, 138);
            this.listBox3.TabIndex = 0;
            // 
            // tabPage4
            // 
            this.tabPage4.Controls.Add(this.splitContainer4);
            this.tabPage4.Location = new System.Drawing.Point(4, 22);
            this.tabPage4.Name = "tabPage4";
            this.tabPage4.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage4.Size = new System.Drawing.Size(424, 277);
            this.tabPage4.TabIndex = 3;
            this.tabPage4.Text = "上市權證";
            this.tabPage4.UseVisualStyleBackColor = true;
            // 
            // splitContainer4
            // 
            this.splitContainer4.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer4.Location = new System.Drawing.Point(3, 3);
            this.splitContainer4.Name = "splitContainer4";
            this.splitContainer4.Orientation = System.Windows.Forms.Orientation.Horizontal;
            // 
            // splitContainer4.Panel1
            // 
            this.splitContainer4.Panel1.Controls.Add(this.btnSWQuery);
            // 
            // splitContainer4.Panel2
            // 
            this.splitContainer4.Panel2.Controls.Add(this.lbSW);
            this.splitContainer4.Size = new System.Drawing.Size(418, 271);
            this.splitContainer4.SplitterDistance = 129;
            this.splitContainer4.TabIndex = 4;
            // 
            // btnSWQuery
            // 
            this.btnSWQuery.Location = new System.Drawing.Point(172, 53);
            this.btnSWQuery.Name = "btnSWQuery";
            this.btnSWQuery.Size = new System.Drawing.Size(75, 23);
            this.btnSWQuery.TabIndex = 2;
            this.btnSWQuery.Text = "Query";
            this.btnSWQuery.UseVisualStyleBackColor = true;
            this.btnSWQuery.Click += new System.EventHandler(this.btnSWQuery_Click);
            // 
            // lbSW
            // 
            this.lbSW.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lbSW.FormattingEnabled = true;
            this.lbSW.ItemHeight = 12;
            this.lbSW.Location = new System.Drawing.Point(0, 0);
            this.lbSW.Name = "lbSW";
            this.lbSW.Size = new System.Drawing.Size(418, 138);
            this.lbSW.TabIndex = 0;
            // 
            // tabPage5
            // 
            this.tabPage5.Controls.Add(this.splitContainer5);
            this.tabPage5.Location = new System.Drawing.Point(4, 22);
            this.tabPage5.Name = "tabPage5";
            this.tabPage5.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage5.Size = new System.Drawing.Size(424, 277);
            this.tabPage5.TabIndex = 4;
            this.tabPage5.Text = "上櫃權證";
            this.tabPage5.UseVisualStyleBackColor = true;
            // 
            // splitContainer5
            // 
            this.splitContainer5.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer5.Location = new System.Drawing.Point(3, 3);
            this.splitContainer5.Name = "splitContainer5";
            this.splitContainer5.Orientation = System.Windows.Forms.Orientation.Horizontal;
            // 
            // splitContainer5.Panel1
            // 
            this.splitContainer5.Panel1.Controls.Add(this.btnOWQuery);
            // 
            // splitContainer5.Panel2
            // 
            this.splitContainer5.Panel2.Controls.Add(this.lbOW);
            this.splitContainer5.Size = new System.Drawing.Size(418, 271);
            this.splitContainer5.SplitterDistance = 129;
            this.splitContainer5.TabIndex = 4;
            // 
            // btnOWQuery
            // 
            this.btnOWQuery.Location = new System.Drawing.Point(172, 53);
            this.btnOWQuery.Name = "btnOWQuery";
            this.btnOWQuery.Size = new System.Drawing.Size(75, 23);
            this.btnOWQuery.TabIndex = 2;
            this.btnOWQuery.Text = "Query";
            this.btnOWQuery.UseVisualStyleBackColor = true;
            this.btnOWQuery.Click += new System.EventHandler(this.btnOWQuery_Click);
            // 
            // lbOW
            // 
            this.lbOW.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lbOW.FormattingEnabled = true;
            this.lbOW.ItemHeight = 12;
            this.lbOW.Location = new System.Drawing.Point(0, 0);
            this.lbOW.Name = "lbOW";
            this.lbOW.Size = new System.Drawing.Size(418, 138);
            this.lbOW.TabIndex = 0;
            // 
            // tabPage2
            // 
            this.tabPage2.Controls.Add(this.splitContainer2);
            this.tabPage2.Location = new System.Drawing.Point(4, 22);
            this.tabPage2.Name = "tabPage2";
            this.tabPage2.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage2.Size = new System.Drawing.Size(424, 277);
            this.tabPage2.TabIndex = 1;
            this.tabPage2.Text = "Summary";
            this.tabPage2.UseVisualStyleBackColor = true;
            // 
            // splitContainer2
            // 
            this.splitContainer2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer2.Location = new System.Drawing.Point(3, 3);
            this.splitContainer2.Name = "splitContainer2";
            this.splitContainer2.Orientation = System.Windows.Forms.Orientation.Horizontal;
            // 
            // splitContainer2.Panel1
            // 
            this.splitContainer2.Panel1.Controls.Add(this.button4);
            this.splitContainer2.Panel1.Controls.Add(this.dtpRate);
            this.splitContainer2.Panel1.Controls.Add(this.btnWeeklyRate);
            // 
            // splitContainer2.Panel2
            // 
            this.splitContainer2.Panel2.Controls.Add(this.listBox1);
            this.splitContainer2.Size = new System.Drawing.Size(418, 271);
            this.splitContainer2.SplitterDistance = 104;
            this.splitContainer2.TabIndex = 2;
            // 
            // button4
            // 
            this.button4.Location = new System.Drawing.Point(187, 29);
            this.button4.Name = "button4";
            this.button4.Size = new System.Drawing.Size(75, 23);
            this.button4.TabIndex = 2;
            this.button4.Text = "結算";
            this.button4.UseVisualStyleBackColor = true;
            // 
            // dtpRate
            // 
            this.dtpRate.Format = System.Windows.Forms.DateTimePickerFormat.Short;
            this.dtpRate.Location = new System.Drawing.Point(25, 27);
            this.dtpRate.Name = "dtpRate";
            this.dtpRate.Size = new System.Drawing.Size(117, 22);
            this.dtpRate.TabIndex = 0;
            this.dtpRate.Value = new System.DateTime(2014, 1, 27, 0, 0, 0, 0);
            // 
            // btnWeeklyRate
            // 
            this.btnWeeklyRate.Location = new System.Drawing.Point(288, 29);
            this.btnWeeklyRate.Name = "btnWeeklyRate";
            this.btnWeeklyRate.Size = new System.Drawing.Size(75, 23);
            this.btnWeeklyRate.TabIndex = 1;
            this.btnWeeklyRate.Text = "集中率";
            this.btnWeeklyRate.UseVisualStyleBackColor = true;
            this.btnWeeklyRate.Click += new System.EventHandler(this.btnWeeklyRate_Click);
            // 
            // listBox1
            // 
            this.listBox1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.listBox1.FormattingEnabled = true;
            this.listBox1.ItemHeight = 12;
            this.listBox1.Location = new System.Drawing.Point(0, 0);
            this.listBox1.Name = "listBox1";
            this.listBox1.Size = new System.Drawing.Size(418, 163);
            this.listBox1.TabIndex = 0;
            // 
            // InitWorker
            // 
            this.InitWorker.WorkerReportsProgress = true;
            this.InitWorker.DoWork += new System.ComponentModel.DoWorkEventHandler(this.InitWorker_DoWork);
            this.InitWorker.ProgressChanged += new System.ComponentModel.ProgressChangedEventHandler(this.InitWorker_ProgressChanged);
            this.InitWorker.RunWorkerCompleted += new System.ComponentModel.RunWorkerCompletedEventHandler(this.InitWorker_RunWorkerCompleted);
            // 
            // QueryWorker
            // 
            this.QueryWorker.WorkerReportsProgress = true;
            this.QueryWorker.DoWork += new System.ComponentModel.DoWorkEventHandler(this.QueryWorker_DoWork);
            this.QueryWorker.ProgressChanged += new System.ComponentModel.ProgressChangedEventHandler(this.QueryWorker_ProgressChanged);
            this.QueryWorker.RunWorkerCompleted += new System.ComponentModel.RunWorkerCompletedEventHandler(this.QueryWorker_RunWorkerCompleted);
            // 
            // SWWorker
            // 
            this.SWWorker.WorkerReportsProgress = true;
            this.SWWorker.DoWork += new System.ComponentModel.DoWorkEventHandler(this.SWWorker_DoWork);
            this.SWWorker.ProgressChanged += new System.ComponentModel.ProgressChangedEventHandler(this.SWWorker_ProgressChanged);
            this.SWWorker.RunWorkerCompleted += new System.ComponentModel.RunWorkerCompletedEventHandler(this.SWWorker_RunWorkerCompleted);
            // 
            // OWWorker
            // 
            this.OWWorker.WorkerReportsProgress = true;
            this.OWWorker.DoWork += new System.ComponentModel.DoWorkEventHandler(this.OWWorker_DoWork);
            this.OWWorker.ProgressChanged += new System.ComponentModel.ProgressChangedEventHandler(this.OWWorker_ProgressChanged);
            this.OWWorker.RunWorkerCompleted += new System.ComponentModel.RunWorkerCompletedEventHandler(this.OWWorker_RunWorkerCompleted);
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(432, 303);
            this.Controls.Add(this.tabControl1);
            this.Name = "Form1";
            this.Text = "v1.0.2014042300";
            this.Load += new System.EventHandler(this.Form1_Load);
            this.tabControl1.ResumeLayout(false);
            this.tabPage1.ResumeLayout(false);
            this.splitContainer1.Panel1.ResumeLayout(false);
            this.splitContainer1.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).EndInit();
            this.splitContainer1.ResumeLayout(false);
            this.tabPage3.ResumeLayout(false);
            this.splitContainer3.Panel1.ResumeLayout(false);
            this.splitContainer3.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer3)).EndInit();
            this.splitContainer3.ResumeLayout(false);
            this.tabPage4.ResumeLayout(false);
            this.splitContainer4.Panel1.ResumeLayout(false);
            this.splitContainer4.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer4)).EndInit();
            this.splitContainer4.ResumeLayout(false);
            this.tabPage5.ResumeLayout(false);
            this.splitContainer5.Panel1.ResumeLayout(false);
            this.splitContainer5.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer5)).EndInit();
            this.splitContainer5.ResumeLayout(false);
            this.tabPage2.ResumeLayout(false);
            this.splitContainer2.Panel1.ResumeLayout(false);
            this.splitContainer2.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer2)).EndInit();
            this.splitContainer2.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Timer MainTimer;
        private System.Windows.Forms.Timer DetailTimer;
        private System.Windows.Forms.Timer SummaryTimer;
        private System.ComponentModel.BackgroundWorker MainWork;
        private System.ComponentModel.BackgroundWorker RateWorker;
        private System.ComponentModel.BackgroundWorker TestWorker;
        private System.Windows.Forms.TabControl tabControl1;
        private System.Windows.Forms.TabPage tabPage1;
        private System.Windows.Forms.TabPage tabPage2;
        private System.Windows.Forms.Button btnWeeklyRate;
        private System.Windows.Forms.DateTimePicker dtpRate;
        private System.Windows.Forms.SplitContainer splitContainer2;
        private System.Windows.Forms.ListBox listBox1;
        private System.Windows.Forms.TabPage tabPage3;
        private System.Windows.Forms.SplitContainer splitContainer1;
        private System.Windows.Forms.Button btnBasic;
        private System.Windows.Forms.ListBox lbStock;
        private System.Windows.Forms.Button btnStock;
        private System.Windows.Forms.SplitContainer splitContainer3;
        private System.Windows.Forms.Button btnOQuery;
        private System.Windows.Forms.ListBox listBox3;
        private System.Windows.Forms.Button button4;
        private System.ComponentModel.BackgroundWorker InitWorker;
        private System.Windows.Forms.Button btnSQuery;
        private System.ComponentModel.BackgroundWorker QueryWorker;
        private System.Windows.Forms.TabPage tabPage4;
        private System.Windows.Forms.SplitContainer splitContainer4;
        private System.Windows.Forms.ListBox lbSW;
        private System.Windows.Forms.TabPage tabPage5;
        private System.Windows.Forms.SplitContainer splitContainer5;
        private System.Windows.Forms.ListBox lbOW;
        private System.Windows.Forms.Button btnSWQuery;
        private System.Windows.Forms.Button btnOWQuery;
        private System.ComponentModel.BackgroundWorker SWWorker;
        private System.ComponentModel.BackgroundWorker OWWorker;
    }
}

