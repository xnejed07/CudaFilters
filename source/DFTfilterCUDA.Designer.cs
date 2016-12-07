namespace plugins_cuda_filters
{
    partial class SVP_FFTfilterCuda
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

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.button1 = new System.Windows.Forms.Button();
            this.panel1 = new System.Windows.Forms.Panel();
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            this.label9 = new System.Windows.Forms.Label();
            this.label8 = new System.Windows.Forms.Label();
            this.label18 = new System.Windows.Forms.Label();
            this.label17 = new System.Windows.Forms.Label();
            this.label16 = new System.Windows.Forms.Label();
            this.label12 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.label7 = new System.Windows.Forms.Label();
            this.chbPower = new System.Windows.Forms.CheckBox();
            this.radioButton1 = new System.Windows.Forms.RadioButton();
            this.chbBS = new System.Windows.Forms.RadioButton();
            this.chbSFFT = new System.Windows.Forms.CheckBox();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.rbFitBoth = new System.Windows.Forms.RadioButton();
            this.rbFitRes = new System.Windows.Forms.RadioButton();
            this.radioButton2 = new System.Windows.Forms.RadioButton();
            this.rbFitAll = new System.Windows.Forms.RadioButton();
            this.rbFitOrig = new System.Windows.Forms.RadioButton();
            this.label2 = new System.Windows.Forms.Label();
            this.button3 = new System.Windows.Forms.Button();
            this.chEnvelope = new System.Windows.Forms.CheckBox();
            this.numericUpDown1 = new System.Windows.Forms.NumericUpDown();
            this.label4 = new System.Windows.Forms.Label();
            this.nuP2 = new System.Windows.Forms.NumericUpDown();
            this.pbs = new System.Windows.Forms.PictureBox();
            this.label3 = new System.Windows.Forms.Label();
            this.nuP1 = new System.Windows.Forms.NumericUpDown();
            this.chbAkt = new System.Windows.Forms.CheckBox();
            this.bgw = new System.ComponentModel.BackgroundWorker();
            this.panel2 = new System.Windows.Forms.Panel();
            this.labProg = new System.Windows.Forms.Label();
            this.pg1 = new System.Windows.Forms.ProgressBar();
            this.button2 = new System.Windows.Forms.Button();
            this.pbx = new System.Windows.Forms.PictureBox();
            this.bgProc = new System.ComponentModel.BackgroundWorker();
            this.pigTimer = new System.Windows.Forms.Timer(this.components);
            this.panel3 = new System.Windows.Forms.Panel();
            this.panel1.SuspendLayout();
            this.groupBox3.SuspendLayout();
            this.groupBox2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDown1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.nuP2)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pbs)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.nuP1)).BeginInit();
            this.panel2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pbx)).BeginInit();
            this.panel3.SuspendLayout();
            this.SuspendLayout();
            // 
            // button1
            // 
            this.button1.AllowDrop = true;
            this.button1.Location = new System.Drawing.Point(3, 3);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(313, 37);
            this.button1.TabIndex = 12;
            this.button1.Text = "(Choose a channel)";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            this.button1.DragDrop += new System.Windows.Forms.DragEventHandler(this.button1_DragDrop);
            this.button1.DragEnter += new System.Windows.Forms.DragEventHandler(this.button1_DragEnter);
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.groupBox3);
            this.panel1.Controls.Add(this.chbPower);
            this.panel1.Controls.Add(this.radioButton1);
            this.panel1.Controls.Add(this.chbBS);
            this.panel1.Controls.Add(this.chbSFFT);
            this.panel1.Controls.Add(this.groupBox2);
            this.panel1.Controls.Add(this.label2);
            this.panel1.Controls.Add(this.button3);
            this.panel1.Controls.Add(this.chEnvelope);
            this.panel1.Controls.Add(this.numericUpDown1);
            this.panel1.Controls.Add(this.label4);
            this.panel1.Controls.Add(this.nuP2);
            this.panel1.Controls.Add(this.pbs);
            this.panel1.Controls.Add(this.label3);
            this.panel1.Controls.Add(this.nuP1);
            this.panel1.Controls.Add(this.chbAkt);
            this.panel1.Controls.Add(this.button1);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Top;
            this.panel1.Location = new System.Drawing.Point(0, 0);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(955, 240);
            this.panel1.TabIndex = 16;
            this.panel1.Paint += new System.Windows.Forms.PaintEventHandler(this.panel1_Paint);
            // 
            // groupBox3
            // 
            this.groupBox3.Controls.Add(this.label9);
            this.groupBox3.Controls.Add(this.label8);
            this.groupBox3.Controls.Add(this.label18);
            this.groupBox3.Controls.Add(this.label17);
            this.groupBox3.Controls.Add(this.label16);
            this.groupBox3.Controls.Add(this.label12);
            this.groupBox3.Controls.Add(this.label1);
            this.groupBox3.Controls.Add(this.label5);
            this.groupBox3.Controls.Add(this.label6);
            this.groupBox3.Controls.Add(this.label7);
            this.groupBox3.Location = new System.Drawing.Point(725, 3);
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.Size = new System.Drawing.Size(213, 101);
            this.groupBox3.TabIndex = 39;
            this.groupBox3.TabStop = false;
            this.groupBox3.Text = "GPU info";
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Location = new System.Drawing.Point(112, 71);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(35, 13);
            this.label9.TabIndex = 26;
            this.label9.Text = "label9";
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(7, 71);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(71, 13);
            this.label8.TabIndex = 25;
            this.label8.Text = "CUDA drivers";
            // 
            // label18
            // 
            this.label18.AutoSize = true;
            this.label18.Location = new System.Drawing.Point(6, 57);
            this.label18.Name = "label18";
            this.label18.Size = new System.Drawing.Size(96, 13);
            this.label18.TabIndex = 24;
            this.label18.Text = "Compute capability";
            // 
            // label17
            // 
            this.label17.AutoSize = true;
            this.label17.Location = new System.Drawing.Point(6, 42);
            this.label17.Name = "label17";
            this.label17.Size = new System.Drawing.Size(57, 13);
            this.label17.TabIndex = 23;
            this.label17.Text = "Frequency";
            // 
            // label16
            // 
            this.label16.AutoSize = true;
            this.label16.Location = new System.Drawing.Point(7, 29);
            this.label16.Name = "label16";
            this.label16.Size = new System.Drawing.Size(44, 13);
            this.label16.TabIndex = 22;
            this.label16.Text = "Memory";
            // 
            // label12
            // 
            this.label12.AutoSize = true;
            this.label12.Location = new System.Drawing.Point(6, 16);
            this.label12.Name = "label12";
            this.label12.Size = new System.Drawing.Size(41, 13);
            this.label12.TabIndex = 21;
            this.label12.Text = "Device";
            this.label12.Click += new System.EventHandler(this.label12_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(112, 14);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(35, 13);
            this.label1.TabIndex = 17;
            this.label1.Text = "label1";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(112, 29);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(35, 13);
            this.label5.TabIndex = 18;
            this.label5.Text = "label5";
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(112, 42);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(35, 13);
            this.label6.TabIndex = 19;
            this.label6.Text = "label6";
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(112, 57);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(35, 13);
            this.label7.TabIndex = 20;
            this.label7.Text = "label7";
            // 
            // chbPower
            // 
            this.chbPower.AutoSize = true;
            this.chbPower.Location = new System.Drawing.Point(583, 56);
            this.chbPower.Name = "chbPower";
            this.chbPower.Size = new System.Drawing.Size(38, 17);
            this.chbPower.TabIndex = 38;
            this.chbPower.Tag = "POW:Checked";
            this.chbPower.Text = "^2";
            this.chbPower.UseVisualStyleBackColor = true;
            this.chbPower.Visible = false;
            this.chbPower.CheckedChanged += new System.EventHandler(this.chbPower_CheckedChanged);
            // 
            // radioButton1
            // 
            this.radioButton1.AutoSize = true;
            this.radioButton1.Location = new System.Drawing.Point(420, 55);
            this.radioButton1.Name = "radioButton1";
            this.radioButton1.Size = new System.Drawing.Size(73, 17);
            this.radioButton1.TabIndex = 5;
            this.radioButton1.TabStop = true;
            this.radioButton1.Text = "BandPass";
            this.radioButton1.UseVisualStyleBackColor = true;
            this.radioButton1.CheckedChanged += new System.EventHandler(this.radioButton1_CheckedChanged);
            // 
            // chbBS
            // 
            this.chbBS.AutoSize = true;
            this.chbBS.Checked = true;
            this.chbBS.Location = new System.Drawing.Point(343, 55);
            this.chbBS.Name = "chbBS";
            this.chbBS.Size = new System.Drawing.Size(70, 17);
            this.chbBS.TabIndex = 4;
            this.chbBS.TabStop = true;
            this.chbBS.Tag = "BandStop:rbChecked";
            this.chbBS.Text = "Bandstop";
            this.chbBS.UseVisualStyleBackColor = true;
            this.chbBS.CheckedChanged += new System.EventHandler(this.chbBS_CheckedChanged_1);
            // 
            // chbSFFT
            // 
            this.chbSFFT.AutoSize = true;
            this.chbSFFT.Checked = true;
            this.chbSFFT.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chbSFFT.Location = new System.Drawing.Point(430, 16);
            this.chbSFFT.Name = "chbSFFT";
            this.chbSFFT.Size = new System.Drawing.Size(84, 17);
            this.chbSFFT.TabIndex = 11;
            this.chbSFFT.Text = "Smooth FFT";
            this.chbSFFT.UseVisualStyleBackColor = true;
            this.chbSFFT.CheckedChanged += new System.EventHandler(this.chbSFFT_CheckedChanged);
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.rbFitBoth);
            this.groupBox2.Controls.Add(this.rbFitRes);
            this.groupBox2.Controls.Add(this.radioButton2);
            this.groupBox2.Controls.Add(this.rbFitAll);
            this.groupBox2.Controls.Add(this.rbFitOrig);
            this.groupBox2.Location = new System.Drawing.Point(5, 50);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(311, 51);
            this.groupBox2.TabIndex = 37;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Fit display to";
            // 
            // rbFitBoth
            // 
            this.rbFitBoth.AutoSize = true;
            this.rbFitBoth.Location = new System.Drawing.Point(221, 22);
            this.rbFitBoth.Name = "rbFitBoth";
            this.rbFitBoth.Size = new System.Drawing.Size(85, 17);
            this.rbFitBoth.TabIndex = 3;
            this.rbFitBoth.Tag = "vizBoth:rbChecked";
            this.rbFitBoth.Text = "Independent";
            this.rbFitBoth.UseVisualStyleBackColor = true;
            this.rbFitBoth.CheckedChanged += new System.EventHandler(this.rbFitBoth_CheckedChanged);
            // 
            // rbFitRes
            // 
            this.rbFitRes.AutoSize = true;
            this.rbFitRes.Location = new System.Drawing.Point(76, 22);
            this.rbFitRes.Name = "rbFitRes";
            this.rbFitRes.Size = new System.Drawing.Size(55, 17);
            this.rbFitRes.TabIndex = 1;
            this.rbFitRes.Tag = "vizRes:rbChecked";
            this.rbFitRes.Text = "Result";
            this.rbFitRes.UseVisualStyleBackColor = true;
            this.rbFitRes.CheckedChanged += new System.EventHandler(this.rbFitRes_CheckedChanged);
            // 
            // radioButton2
            // 
            this.radioButton2.AutoSize = true;
            this.radioButton2.Enabled = false;
            this.radioButton2.Location = new System.Drawing.Point(324, 32);
            this.radioButton2.Name = "radioButton2";
            this.radioButton2.Size = new System.Drawing.Size(54, 17);
            this.radioButton2.TabIndex = 2;
            this.radioButton2.Tag = "typeNF:rbChecked";
            this.radioButton2.Text = "Notch";
            this.radioButton2.UseVisualStyleBackColor = true;
            this.radioButton2.Visible = false;
            // 
            // rbFitAll
            // 
            this.rbFitAll.AutoSize = true;
            this.rbFitAll.Location = new System.Drawing.Point(133, 22);
            this.rbFitAll.Name = "rbFitAll";
            this.rbFitAll.Size = new System.Drawing.Size(83, 17);
            this.rbFitAll.TabIndex = 2;
            this.rbFitAll.Tag = "vizTotE:rbChecked";
            this.rbFitAll.Text = "Tot. extrems";
            this.rbFitAll.UseVisualStyleBackColor = true;
            this.rbFitAll.CheckedChanged += new System.EventHandler(this.rbFitAll_CheckedChanged);
            // 
            // rbFitOrig
            // 
            this.rbFitOrig.AutoSize = true;
            this.rbFitOrig.Checked = true;
            this.rbFitOrig.Location = new System.Drawing.Point(11, 22);
            this.rbFitOrig.Name = "rbFitOrig";
            this.rbFitOrig.Size = new System.Drawing.Size(60, 17);
            this.rbFitOrig.TabIndex = 0;
            this.rbFitOrig.TabStop = true;
            this.rbFitOrig.Tag = "vizOrig:rbChecked";
            this.rbFitOrig.Text = "Original";
            this.rbFitOrig.UseVisualStyleBackColor = true;
            this.rbFitOrig.CheckedChanged += new System.EventHandler(this.rbFitOrig_CheckedChanged);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(645, 56);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(74, 13);
            this.label2.TabIndex = 36;
            this.label2.Text = "Used window:";
            this.label2.Visible = false;
            // 
            // button3
            // 
            this.button3.Location = new System.Drawing.Point(581, 77);
            this.button3.Name = "button3";
            this.button3.Size = new System.Drawing.Size(138, 27);
            this.button3.TabIndex = 9;
            this.button3.Tag = "Wintype:Text";
            this.button3.Text = "Rectangular";
            this.button3.UseVisualStyleBackColor = true;
            this.button3.Visible = false;
            this.button3.Click += new System.EventHandler(this.button3_Click);
            // 
            // chEnvelope
            // 
            this.chEnvelope.AutoSize = true;
            this.chEnvelope.Location = new System.Drawing.Point(501, 56);
            this.chEnvelope.Name = "chEnvelope";
            this.chEnvelope.Size = new System.Drawing.Size(71, 17);
            this.chEnvelope.TabIndex = 6;
            this.chEnvelope.Tag = "Envelope:Checked";
            this.chEnvelope.Text = "Envelope";
            this.chEnvelope.UseVisualStyleBackColor = true;
            this.chEnvelope.CheckedChanged += new System.EventHandler(this.chEnvelope_CheckedChanged);
            // 
            // numericUpDown1
            // 
            this.numericUpDown1.Enabled = false;
            this.numericUpDown1.Increment = new decimal(new int[] {
            1024,
            0,
            0,
            0});
            this.numericUpDown1.Location = new System.Drawing.Point(667, 12);
            this.numericUpDown1.Maximum = new decimal(new int[] {
            500000,
            0,
            0,
            0});
            this.numericUpDown1.Minimum = new decimal(new int[] {
            128,
            0,
            0,
            0});
            this.numericUpDown1.Name = "numericUpDown1";
            this.numericUpDown1.Size = new System.Drawing.Size(49, 20);
            this.numericUpDown1.TabIndex = 32;
            this.numericUpDown1.TabStop = false;
            this.numericUpDown1.Tag = "PreviewSamples:NUD_Value";
            this.numericUpDown1.Value = new decimal(new int[] {
            4096,
            0,
            0,
            0});
            this.numericUpDown1.Visible = false;
            this.numericUpDown1.ValueChanged += new System.EventHandler(this.numericUpDown1_ValueChanged);
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(484, 83);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(10, 13);
            this.label4.TabIndex = 31;
            this.label4.Text = "-";
            // 
            // nuP2
            // 
            this.nuP2.DecimalPlaces = 1;
            this.nuP2.Increment = new decimal(new int[] {
            1,
            0,
            0,
            65536});
            this.nuP2.Location = new System.Drawing.Point(500, 81);
            this.nuP2.Maximum = new decimal(new int[] {
            500000,
            0,
            0,
            0});
            this.nuP2.Name = "nuP2";
            this.nuP2.Size = new System.Drawing.Size(72, 20);
            this.nuP2.TabIndex = 8;
            this.nuP2.Tag = "EndF:NUD_Value";
            this.nuP2.Value = new decimal(new int[] {
            60,
            0,
            0,
            0});
            this.nuP2.ValueChanged += new System.EventHandler(this.nuP2_ValueChanged);
            // 
            // pbs
            // 
            this.pbs.BackColor = System.Drawing.Color.SteelBlue;
            this.pbs.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.pbs.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.pbs.Location = new System.Drawing.Point(0, 107);
            this.pbs.Name = "pbs";
            this.pbs.Size = new System.Drawing.Size(955, 133);
            this.pbs.TabIndex = 28;
            this.pbs.TabStop = false;
            this.pbs.Paint += new System.Windows.Forms.PaintEventHandler(this.pbs_Paint);
            this.pbs.MouseDown += new System.Windows.Forms.MouseEventHandler(this.pbs_MouseDown);
            this.pbs.MouseMove += new System.Windows.Forms.MouseEventHandler(this.pbs_MouseMove);
            this.pbs.MouseUp += new System.Windows.Forms.MouseEventHandler(this.pbs_MouseUp);
            this.pbs.Resize += new System.EventHandler(this.pbs_Resize);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(340, 83);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(60, 13);
            this.label3.TabIndex = 27;
            this.label3.Text = "Frequency:";
            // 
            // nuP1
            // 
            this.nuP1.DecimalPlaces = 1;
            this.nuP1.Increment = new decimal(new int[] {
            1,
            0,
            0,
            65536});
            this.nuP1.Location = new System.Drawing.Point(406, 81);
            this.nuP1.Maximum = new decimal(new int[] {
            500000,
            0,
            0,
            0});
            this.nuP1.Name = "nuP1";
            this.nuP1.Size = new System.Drawing.Size(72, 20);
            this.nuP1.TabIndex = 7;
            this.nuP1.Tag = "StartF:NUD_Value";
            this.nuP1.Value = new decimal(new int[] {
            40,
            0,
            0,
            0});
            this.nuP1.ValueChanged += new System.EventHandler(this.nuP1_ValueChanged);
            // 
            // chbAkt
            // 
            this.chbAkt.AutoSize = true;
            this.chbAkt.Checked = true;
            this.chbAkt.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chbAkt.Location = new System.Drawing.Point(346, 15);
            this.chbAkt.Name = "chbAkt";
            this.chbAkt.Size = new System.Drawing.Size(69, 17);
            this.chbAkt.TabIndex = 10;
            this.chbAkt.Text = "Actualize";
            this.chbAkt.UseVisualStyleBackColor = true;
            this.chbAkt.CheckedChanged += new System.EventHandler(this.chbAkt_CheckedChanged);
            // 
            // bgw
            // 
            this.bgw.WorkerReportsProgress = true;
            this.bgw.DoWork += new System.ComponentModel.DoWorkEventHandler(this.bgw_DoWork);
            this.bgw.ProgressChanged += new System.ComponentModel.ProgressChangedEventHandler(this.bgw_ProgressChanged);
            this.bgw.RunWorkerCompleted += new System.ComponentModel.RunWorkerCompletedEventHandler(this.bgw_RunWorkerCompleted);
            // 
            // panel2
            // 
            this.panel2.Controls.Add(this.panel3);
            this.panel2.Controls.Add(this.button2);
            this.panel2.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.panel2.Location = new System.Drawing.Point(0, 490);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(955, 42);
            this.panel2.TabIndex = 17;
            // 
            // labProg
            // 
            this.labProg.Dock = System.Windows.Forms.DockStyle.Top;
            this.labProg.Location = new System.Drawing.Point(0, 0);
            this.labProg.Name = "labProg";
            this.labProg.Size = new System.Drawing.Size(759, 13);
            this.labProg.TabIndex = 18;
            this.labProg.Text = "---";
            this.labProg.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // pg1
            // 
            this.pg1.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.pg1.Location = new System.Drawing.Point(0, 19);
            this.pg1.Name = "pg1";
            this.pg1.Size = new System.Drawing.Size(759, 23);
            this.pg1.TabIndex = 17;
            // 
            // button2
            // 
            this.button2.AllowDrop = true;
            this.button2.Dock = System.Windows.Forms.DockStyle.Left;
            this.button2.Location = new System.Drawing.Point(0, 0);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(196, 42);
            this.button2.TabIndex = 13;
            this.button2.Text = "Process";
            this.button2.UseVisualStyleBackColor = true;
            this.button2.Click += new System.EventHandler(this.button2_Click);
            // 
            // pbx
            // 
            this.pbx.BackColor = System.Drawing.Color.White;
            this.pbx.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pbx.Location = new System.Drawing.Point(0, 240);
            this.pbx.Name = "pbx";
            this.pbx.Size = new System.Drawing.Size(955, 250);
            this.pbx.TabIndex = 18;
            this.pbx.TabStop = false;
            this.pbx.Paint += new System.Windows.Forms.PaintEventHandler(this.pbx_Paint);
            this.pbx.Resize += new System.EventHandler(this.pbx_Resize);
            // 
            // bgProc
            // 
            this.bgProc.WorkerReportsProgress = true;
            this.bgProc.DoWork += new System.ComponentModel.DoWorkEventHandler(this.bgProc_DoWork);
            this.bgProc.ProgressChanged += new System.ComponentModel.ProgressChangedEventHandler(this.bgProc_ProgressChanged);
            this.bgProc.RunWorkerCompleted += new System.ComponentModel.RunWorkerCompletedEventHandler(this.bgProc_RunWorkerCompleted);
            // 
            // pigTimer
            // 
            this.pigTimer.Enabled = true;
            this.pigTimer.Interval = 200;
            this.pigTimer.Tick += new System.EventHandler(this.pigTimer_Tick);
            // 
            // panel3
            // 
            this.panel3.Controls.Add(this.labProg);
            this.panel3.Controls.Add(this.pg1);
            this.panel3.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel3.Location = new System.Drawing.Point(196, 0);
            this.panel3.Name = "panel3";
            this.panel3.Size = new System.Drawing.Size(759, 42);
            this.panel3.TabIndex = 19;
            // 
            // SVP_FFTfilterCuda
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.pbx);
            this.Controls.Add(this.panel2);
            this.Controls.Add(this.panel1);
            this.Name = "SVP_FFTfilterCuda";
            this.Size = new System.Drawing.Size(955, 532);
            this.Load += new System.EventHandler(this.DFTdisplay_Load);
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.groupBox3.ResumeLayout(false);
            this.groupBox3.PerformLayout();
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDown1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.nuP2)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pbs)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.nuP1)).EndInit();
            this.panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.pbx)).EndInit();
            this.panel3.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.CheckBox chbAkt;
        private System.ComponentModel.BackgroundWorker bgw;
        private System.Windows.Forms.NumericUpDown nuP1;
        private System.Windows.Forms.PictureBox pbs;
        private System.Windows.Forms.Panel panel2;
        private System.Windows.Forms.PictureBox pbx;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.NumericUpDown nuP2;
        private System.Windows.Forms.NumericUpDown numericUpDown1;
        private System.Windows.Forms.ProgressBar pg1;
        private System.Windows.Forms.Button button2;
        private System.ComponentModel.BackgroundWorker bgProc;
        private System.Windows.Forms.Label labProg;
        private System.Windows.Forms.CheckBox chEnvelope;
        private System.Windows.Forms.Button button3;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.RadioButton rbFitBoth;
        private System.Windows.Forms.RadioButton rbFitRes;
        private System.Windows.Forms.RadioButton radioButton2;
        private System.Windows.Forms.RadioButton rbFitAll;
        private System.Windows.Forms.RadioButton rbFitOrig;
        private System.Windows.Forms.CheckBox chbSFFT;
        private System.Windows.Forms.RadioButton chbBS;
        private System.Windows.Forms.RadioButton radioButton1;
        private System.Windows.Forms.Timer pigTimer;
        private System.Windows.Forms.CheckBox chbPower;
        private System.Windows.Forms.GroupBox groupBox3;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.Label label18;
        private System.Windows.Forms.Label label17;
        private System.Windows.Forms.Label label16;
        private System.Windows.Forms.Label label12;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.Panel panel3;
    }
}
