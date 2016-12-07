using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Diagnostics;
using System.Threading;
using System.IO;
using signalViewer;
using signalViewer.MathUtils;
using ManagedCuda.CudaFFT;
using ManagedCuda;
using ManagedCuda.BasicTypes;
using ManagedCuda.VectorTypes;


namespace plugins_cuda_filters
{
    public partial class SVP_FFTfilterCuda : UserControl
    {
        //CudaContext ctx = new CudaContext();
        bool cudaOK=true;
        bool compcap = true;
        List<signalViewer.graphPanel> linkedChannels = new List<signalViewer.graphPanel>();

        //-----help file arguments ----
        public string helpNamedDest = "SignalPlant.indd:FFT filter:298"; //---named destination in the help file
        public string helpFile = "SignalPlant.pdf"; //--name of help file

        double[] DFTresult;
        double[] smoothedFFT;

        int dlen = 5000;

        float donePerc = 0;

        int div = 2; //---DFT se bude vyšetřovat a zobrazovat jen do fs/div vzorků

        float freqBinWidth = 0; //frekvenční šířka jednoho binu

        Stopwatch processWatch = new Stopwatch();
        List<Task> workTasks = new List<Task>();

        float[] inputPre; 
        float[] outputPre;

        //Bitmap bcim;

        float limit1 = 40;
        float limit2 = 60;

        int limit1Sample = 0;
        int limit2Sample = 0;

        int radLim = 5;

        int cc = 0;

        bool movingP1 = false;
        bool movingP2 = false;

        bool movingMid = false;

        string fftState = "";

        bool doEnvelope = false;
        bool powerEnvelope = false;

        mainView mv;

        WindowingFunction winFunction = new WindowingFunction(100, 0, 0);

        private void report(int perc, string mess)
        {
            mainView.footerMessage = "FFT:" + mess + " / " + perc + "% done";

            if (bgw.IsBusy)
                bgw.ReportProgress(perc, mess);
        }

        public void presetLoaded()
        {
            movingP1 = false;
            movingP2 = false;
            movingMid = false;

            /*
            for (int i = 0; i < signalViewer.WindowingFunction.windowNames.Length; i++)
            {
                if (signalViewer.WindowingFunction.windowNames[i].Equals(button3.Text))
                {
                    winFunction.winType = i;
                    break;
                }
            }
            */

            winFunction.winType = Array.IndexOf(WindowingFunction.windowNames, button3.Text);


            refrControls();
            //doPreview();
        }

        public void presetBeforeLoaded()
        {
            movingMid = true;
            movingP1 = true;
            movingP2 = true;
        }

        public string getDeveloperInfo()
        {
            return "Filip Plešinger, UPT, AVČR, 2013; Petr Nejedlý, UPT, AVČR, 2016";
        }

        public string getDescription()
        {
            return "FFT Filtr - Fast Fourrier Transofrm filter CUDA GPU";
        }

        public string getName()
        {
            return "CUDA FFT filter";
        }

        public string getCategory()
        {
            return "CUDA";
        }


        public void doExternalRefresh()
        {
            if (chbAkt.Checked)
            refrControls();
        }

        public SVP_FFTfilterCuda()
        {


            InitializeComponent();

            




        }


        private string Get_CUDA_DriversDirectory()
        {
            string softwareKeyName = string.Empty;
            string homeDirectory = string.Empty;

            if (IntPtr.Size == 8)
            {
                softwareKeyName = @"HKEY_LOCAL_MACHINE\SOFTWARE\NVIDIA Corporation\GPU Computing Toolkit\CUDA";
            }
            else
            {
                softwareKeyName = @"HKEY_LOCAL_MACHINE\SOFTWARE\NVIDIA Corporation\GPU Computing Toolkit\CUDA";
            }

            object regValue = Microsoft.Win32.Registry.GetValue(softwareKeyName, "FirstVersionInstalled", "No drivers found");
            if (regValue != null)
            {
                homeDirectory = regValue.ToString();
            }
            else
            {
                homeDirectory = "No CUDA drivers found.";
            }
            return homeDirectory;
        }


        double[][] DFTR = new double[2][];

        private int dataPos = 0;
        Brush fillBrush = new SolidBrush(Color.FromArgb(64, 255, 255, 255));
        Pen inputPen = new Pen(Color.FromArgb(64, 64, 64, 64));
        Pen outputPen = new Pen(Color.FromArgb(255, 0, 0, 0));
        Pen gridPen = new Pen(Color.FromArgb(64,255,255,255));
        Pen DFTPenDel = new Pen(Color.FromArgb(60, 255, 255, 255));
        Pen DFTPen = new Pen(Color.FromArgb(255, 255, 255, 255));
        Pen limitPen = new Pen(Color.FromArgb(255, 155, 255, 255),1);
       
        Brush traceBrush = new SolidBrush(Color.Yellow);
        float rullerFreq = 0;

        Brush removedBrush = new SolidBrush(Color.FromArgb(64, 64, 64, 64));
        Pen removedPen = new Pen(Color.FromArgb(128, 255, 255, 255));

        Pen removedPenB = new Pen(Color.FromArgb(64, 64, 64, 64));

        bool bandStop = true;

        int lastMpos = 0;

        ManualResetEvent COMRE = new ManualResetEvent(false);


        public void doPreview()
        {

            chbPower.Enabled = chEnvelope.Checked;

           if (linkedChannels.Count==0) return;
           dataPos = (int)signalViewer.graphPanel.traceDataIndex;
           if (dataPos<0) dataPos = 0;
            if (dataPos+dlen>=linkedChannels[0].dataCache[linkedChannels[0].currentDataChace].data.Count)
                return;
            if (bgw.IsBusy) return;

            if (DFTR == null || DFTresult == null) return;

            processWatch.Restart();
            bgw.RunWorkerAsync();
        }


        private void onlyChangeLimits(float L1, float L2, bool calledFromNUP1, bool calledFromNUP2)
        {
            if (L2 > signalViewer.mainView.sampleFrequency / 2)
                L2 = signalViewer.mainView.sampleFrequency / 2;

            if (L1 >= L2)
                L1 = L2 - 1;

            if (L1 < 0) L1 = 0;
            if (L2 < 0) L2 = 0;

            limit1 = L1;
            limit2 = L2;

            recomputeWindow();

            if (!calledFromNUP1) nuP1.Value = (decimal)L1;
            if (!calledFromNUP2) nuP2.Value = (decimal)L2;

            pbs.Refresh();

            if (!movingP1 && !movingP2 && !movingMid)
            doPreview();

        }

        public void refrControls()
        {
            if (cudaOK == false || compcap ==false)
            {
                this.Enabled = false;
                return;
            }
            button1.Text = "";
            if (linkedChannels.Count == 0) button1.Text = "Drag a channel here or click";
            if (linkedChannels.Count == 1) button1.Text = "Channel : " + linkedChannels[0].channelName;
            if (linkedChannels.Count > 1) button1.Text = linkedChannels.Count.ToString() + " channels were chosen";

            button3.Text = WindowingFunction.windowNames[winFunction.winType];

            prepareFilter();

            if (chbBS.Checked == true) chEnvelope.Enabled = false;
            else
            {
                chEnvelope.Enabled = true;
            }

            doPreview();

        }

        private void button1_DragDrop(object sender, DragEventArgs e)
        {
            string s = e.Data.GetData(DataFormats.Text).ToString();

            try
            {
                this.linkedChannels.Add(mv.getGPbyName(s));
                refrControls();
                
            }
            catch (Exception exp)
            {
                MessageBox.Show("Error:"+exp.Message);
            }
        }

        private void button1_DragEnter(object sender, DragEventArgs e)
        {
            string s = e.Data.GetData(DataFormats.Text).ToString();

            if (mv.getGPbyName(s) != null)
            {
                e.Effect = DragDropEffects.Copy;
            }
            else
                e.Effect = DragDropEffects.None;
        }

        private void prepareFilter()
        {
            try
            {


                int ld = graphPanel.leftI;
                int rd = graphPanel.rightI;

                if (rd > ld + 5000)
                    rd = ld + 5000; //safe limit for quick preview

                int wd = rd - ld;


                dlen = mainView.convertToHigherPow2(wd);

                dataPos = ld;

                inputPre = new float[dlen];

                if (linkedChannels.Count == 0) return;

                if (dlen < 0) return;
                if (dataPos + dlen >= linkedChannels[0].dataCache[linkedChannels[0].currentDataChace].data.Count) return;

                linkedChannels[0].dataCache[linkedChannels[0].currentDataChace].data.CopyTo(dataPos, inputPre, 0, dlen);


                //int neededLength = FFT.convertToHigherPow2(inputPre.Length);

                

                float[] sourceD = new float[dlen];

                for (int i = 0; i < inputPre.Length; i++)
                    sourceD[i] = inputPre[i];

                DFTR = FFT.fft(sourceD);

                DFTresult = new double[sourceD.Length];


                for (int i = 0; i < (DFTR[0].Length); i++)
                {
                    DFTresult[i] = Math.Sqrt(Math.Pow(DFTR[0][i], 2) + Math.Pow(DFTR[1][i], 2));
                }

                double max = DFTresult.Max();

                for (int i = 0; i < DFTresult.Length; i++)
                {
                    DFTresult[i] = 20.0 * Math.Log10(DFTresult[i] / max);
                }


                int rad = 10;

                if (chbSFFT.Checked)
                {

                    double[] block = new double[rad * 2 + 1]; // block for averaging;
                    smoothedFFT = new double[DFTresult.Length];
                    for (int i = 0; i < DFTresult.Length; i++)
                    {
                        int start = i - rad;
                        int end = i + rad;

                        if (start < 0) start = 0;
                        if (end >= DFTresult.Length) end = DFTresult.Length - 1;

                        block = new double[end - start]; // block for averaging;

                        Array.Copy(DFTresult, start, block, 0, block.Length);

                        smoothedFFT[i] = block.Average();
                        ;
                    }
                }
                else 
                {
                    smoothedFFT = DFTresult;
                }

                freqBinWidth = ((float)signalViewer.mainView.sampleFrequency) / ((float)(DFTresult.Length));

                recomputeWindow();

                pbs.Refresh();
            }
            catch (Exception ex)
            {
                mainView.log(ex, "FFT-PrepareFilter", this);
                MessageBox.Show("Problem with FFT preparation", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

        }

        private void recomputeWindow()
        {
            //--------------window creation----------------------

            limit1Sample = (int)(limit1 / freqBinWidth);
            limit2Sample = (int)(limit2 / freqBinWidth);

            int winWidth = limit2Sample - limit1Sample;

            if (winWidth > 1)
            {
                winFunction = new WindowingFunction(winWidth, winFunction.winType, winFunction.winParam);

                if (bandStop)
                    for (int i = 0; i < winFunction.window.Count; i++)
                        winFunction.window[i] = 1 - winFunction.window[i];
            }
            



        }


        private void button1_Click(object sender, EventArgs e)
        {
            signalViewer.selectChannelForm sc = new signalViewer.selectChannelForm();
            sc.regenerateList(linkedChannels);

            if (sc.ShowDialog() == DialogResult.OK)
            {
                this.linkedChannels.Clear();
                for (int i = 0; i < sc.lv.SelectedItems.Count; i++)
                {
                    this.linkedChannels.Add(mv.getGPbyName(sc.lv.SelectedItems[i].Text));
                }
                refrControls();
            }
        }




        private void pbx_Paint(object sender, PaintEventArgs e)
        {

            
            if (cudaOK == false)
            {
                Font fa = new Font("Calibri", 14, FontStyle.Bold);
               
                    //e.Graphics.DrawString("GPU compute capability not supported. Compute capability must be greater than 3.0", fa, Brushes.Black, pbx.Width / 2, pbx.Height / 2, mainView.sfc);                
                    e.Graphics.DrawString("No CUDA drivers. Please install CUDA drivers from NVIDIA website.", fa, Brushes.Black, pbx.Width / 2, pbx.Height / 2, mainView.sfc); 
                return;
            }

            if (compcap == false)
            {
                Font fa = new Font("Calibri", 14, FontStyle.Bold);

                e.Graphics.DrawString("GPU compute capability not supported. Compute capability must be greater than 3.0", fa, Brushes.Black, pbx.Width / 2, pbx.Height / 4, mainView.sfc);                
                
                return;
            }


            try
            {

                e.Graphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
                e.Graphics.TextRenderingHint = System.Drawing.Text.TextRenderingHint.AntiAlias;

                int bh = 20;



                if (outputPre == null || outputPre.Length == 0) return;


               
                if (!bgw.IsBusy)
                {

                   

                    float minVal = (float)inputPre.Min();
                    float maxVal = (float)inputPre.Max();

                    /*
                    float[] psub = new float[outputPre.Length - 2 * order];
                    Array.Copy(outputPre, order, psub, 0, psub.Length);

                    float resMinVal = psub.Min();
                    float resMaxVal = psub.Max();
                    */

                    float resMinVal = outputPre.Min();
                    float resMaxVal = outputPre.Max();


                    PointF prevT = mainView.computeScaleandBaseFromMinMax(resMinVal, resMaxVal, pbx.Height);
                    PointF realT = mainView.computeScaleandBaseFromMinMax(minVal, maxVal, pbx.Height);

                    if (rbFitOrig.Checked) prevT = realT;
                    if (rbFitRes.Checked) realT = prevT;
                    if (rbFitAll.Checked)
                    {
                        float totMin = minVal < resMinVal ? minVal : resMinVal;
                        float totMax = maxVal > resMaxVal ? maxVal : resMaxVal;
                        prevT = mainView.computeScaleandBaseFromMinMax(totMin, totMax, pbx.Height);
                        realT = prevT;
                    }



                    float y0 = (float)inputPre[0] * prevT.X + prevT.Y;
                    float x0 = 0;


                    float xScale = (float)pbx.Width / (float)(inputPre.Length);

                    e.Graphics.Clear(Color.White);


                    for (int i = 1; i < inputPre.Length; i++)
                    {
                        float x1 = (float)i * xScale;
                        float y1 = inputPre[i] * realT.X + realT.Y;

                        e.Graphics.DrawLine(Pens.LightGray, x1, y1, x0, y0);

                        x0 = x1;
                        y0 = y1;
                    }



                    if (outputPre == null)
                    {
                        e.Graphics.DrawString("No preview data", SystemFonts.DefaultFont, Brushes.Black, pbx.Width / 2, pbx.Height / 2, mainView.sfc);
                        return;
                    }



                    y0 = (float)outputPre[0] * prevT.X + prevT.Y;
                    x0 = 0;


                    for (int i = 1; i < outputPre.Length; i++)
                    {
                        float x1 = (float)i * xScale;
                        float y1 = outputPre[i] * prevT.X + prevT.Y;

                        e.Graphics.DrawLine(Pens.Black, x1, y1, x0, y0);

                        x0 = x1;
                        y0 = y1;
                    }


                 
                }
                 

                else
                {
                    int x = pbx.Width / 2;
                    int y = pbx.Height / 2;

                    string str = "Working..." + donePerc + "%";

                    e.Graphics.DrawString(str, SystemFonts.DefaultFont, Brushes.Black, x, y);

                }

               // e.Graphics.DrawString(cc.ToString() + ", L1=" + limit1 + ", L2=" + limit2, SystemFonts.DefaultFont, Brushes.Black, 0, 0);


            }
            catch (Exception ex)
            {
                e.Graphics.DrawString("Error:" + ex.Message+" | "+ex.Source+" | "+ex.StackTrace, SystemFonts.DefaultFont, Brushes.Black, 0, 0);
            }

        }

        private void pbx_Resize(object sender, EventArgs e)
        {
            pbx.Refresh();
        }



        private void nud_ValueChanged(object sender, EventArgs e)
        {
        }

        private void bgw_DoWork(object sender, DoWorkEventArgs e)
        {
            outputPre = null;
            processWatch.Restart();

            outputPre = FFT.FFT_filter(inputPre, limit1, limit2, bandStop, doEnvelope, (int)winFunction.winType, 0);

            if (doEnvelope && powerEnvelope)
                for (int i = 0; i < outputPre.Length; i++)
                    outputPre[i] = outputPre[i] * outputPre[i];
           
        }

        private void bgw_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            processWatch.Stop();
            pbx.Refresh();
        }

        private void bgw_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            //donePerc = e.ProgressPercentage;
            //pbx.Refresh();
        }


        private void DFTdisplay_Load(object sender, EventArgs e)
        {
            mv = (mainView)Application.OpenForms[0]; 
            
            if (signalViewer.mainView.sampleFrequency < limit1)
            {
                limit1 = signalViewer.mainView.sampleFrequency / 10;
                limit2 = signalViewer.mainView.sampleFrequency / 8;

            }

            if (float.IsNaN(mainView.sampleFrequency) || mainView.sampleFrequency == 0) return;

            nuP1.Maximum = (decimal)(signalViewer.mainView.sampleFrequency / 2);
            nuP2.Maximum = nuP1.Maximum;

            onlyChangeLimits(limit1, limit2,false,false);

            label9.Text = Get_CUDA_DriversDirectory();
            if (label9.Text == "No CUDA drivers found.")
            {
                label9.BackColor = Color.Red;
                MessageBox.Show("No CUDA drivers found.");
                cudaOK = false;
            }

            int deviceCount = CudaContext.GetDeviceCount();
            List<CudaDeviceProperties> result = new List<CudaDeviceProperties>(deviceCount);

            for (int i = 0; i < deviceCount; i++)
            {
                result.Add(CudaContext.GetDeviceInfo(i));
            }
            label1.Text = result[0].DeviceName;
            label5.Text = (result[0].TotalGlobalMemory / (1024 * 1024)).ToString() + " MB";
            label6.Text = (result[0].ClockRate / 1000).ToString() + " MHz";
            if (result[0].ComputeCapability.Major >= 3)
            {
                label7.Text = (result[0].ComputeCapability).ToString();
            }
            else
            {
                label7.Text = (result[0].ComputeCapability).ToString() +". GPU Compute capability not supported.";
                label7.BackColor = Color.Red;
                MessageBox.Show("GPU Compute capability not supported. Compute capability 3.0 or greater is supported.");
                
                compcap = false;
            }

            if (cudaOK == false) { this.Enabled = false; }
        }



        private void pbx_MouseDown(object sender, MouseEventArgs e)
        {

        }

        private void pbx_MouseMove(object sender, MouseEventArgs e)
        {
         
        }

        private void panel1_Paint(object sender, PaintEventArgs e)
        {

        }

        private void cbf_SelectedIndexChanged(object sender, EventArgs e)
        {
        }

        private void nud2_ValueChanged(object sender, EventArgs e)
        {
        }

        private void pbs_Paint(object sender, PaintEventArgs e)
        {
            try
            {
                if (smoothedFFT == null || smoothedFFT.Length == 0)
                {
                    e.Graphics.DrawString("FFT spectrum not found", SystemFonts.DefaultFont, Brushes.White, 5, 5);
                }
                else
                {

                    float xScale = (float)(pbs.Width) / (float)(smoothedFFT.Length / 2);

                    signalViewer.GraphScale gs = new signalViewer.GraphScale(smoothedFFT.Min(), smoothedFFT.Max(), pbs.Height);

                    for (int i = 1; i < smoothedFFT.Length / 2; i++)
                    {
                        float x = (float)i * xScale;
                        float x0 = (float)(i - 1) * xScale;

                        float y = (float)(smoothedFFT[i] * gs.yScale + gs.yBase);
                        float y0 = (float)(smoothedFFT[i - 1] * gs.yScale + gs.yBase);

                        float freq = (float)i * freqBinWidth;

                        bool toDel = freq >= limit1 && freq <= limit2;

                        if (!bandStop) toDel = !toDel;

                        if (toDel)
                            e.Graphics.DrawLine(DFTPenDel, x0, y0, x, y);
                        else
                        {
                            e.Graphics.FillRectangle(fillBrush, x, y0, x - x0, pbs.Height - y0);
                            e.Graphics.DrawLine(DFTPen, x0, y0, x, y);
                        }
                    }

                    //----draw rectangles to remove---- A + C
                    if (!bandStop)
                    {
                        e.Graphics.FillRectangle(removedBrush, 0, 0, limit1/freqBinWidth * xScale, this.Height);
                        float x2 = limit2/freqBinWidth*xScale;
                        e.Graphics.FillRectangle(removedBrush, x2, 0, this.Width - x2, this.Height);
                    }


                    float fsPul = signalViewer.mainView.sampleFrequency / 2;

                    string str = fsPul.ToString() + " Hz";


                    float xScaleLim = (float)(pbs.Width) / fsPul;


                    float xL1 = limit1 * xScaleLim;
                    float xL2 = limit2 * xScaleLim;

                    string bo = "Removed";

                    SizeF sf = e.Graphics.MeasureString(bo, SystemFonts.DefaultFont);

                    //----draw windowing------

                    int limit1Sample = (int)(limit1 / freqBinWidth);
                    int limit2Sample = (int)(limit2 / freqBinWidth);

                    float winWidth = limit2Sample - limit1Sample;

                    float winSizeRatio = (limit2 - limit1) / winWidth;

                    float xO = 0;
                    float yO = 0;



                    for (int wx = 0; wx < winFunction.window.Count; wx++)
                    {
                        float xp = ((float)wx * winSizeRatio + limit1) * xScaleLim;
                        float yp = (1 - winFunction.window[wx]) * (pbs.Height - 3);

                        if (xO > 0)
                            e.Graphics.DrawLine(Pens.White, xp, yp, xO, yO);

                        if ((int)xp != (int)xO)
                            e.Graphics.DrawLine(removedPenB, xp, yp, xp, 0);

                        xO = xp;
                        yO = yp;
                    }


                    if (bandStop)
                    {
                        //e.Graphics.FillRectangle(removedBrush, xL1, 0, xL2 - xL1, pbs.Height);

                        if (xL2 - xL1 < sf.Width)
                        {
                            e.Graphics.DrawLine(removedPen, xL1, 0, xL2, pbs.Height);
                            e.Graphics.DrawLine(removedPen, xL1, pbs.Height, xL2, 0);
                        }
                        else
                        {
                            e.Graphics.DrawString(bo, SystemFonts.DefaultFont, Brushes.White, (xL2 + xL1) / 2, pbs.Height / 2, signalViewer.mainView.sfc);
                        }

                        e.Graphics.DrawLine(DFTPen, xL1, pbs.Height - 3, xL2, pbs.Height - 3);

                    }

                    else
                    {
                        //  e.Graphics.FillRectangle(removedBrush, 0,0, xL1, pbs.Height);
                        //   e.Graphics.FillRectangle(removedBrush, xL2, 0, pbs.Width-xL2, pbs.Height);

                        if (xL1 < sf.Width)
                        {
                            e.Graphics.DrawLine(removedPen, 0, 0, xL1, pbs.Height);
                            e.Graphics.DrawLine(removedPen, 0, pbs.Height, xL1, 0);
                        }
                        else
                            e.Graphics.DrawString(bo, SystemFonts.DefaultFont, Brushes.White, (xL1) / 2, pbs.Height / 2, signalViewer.mainView.sfc);


                        if (pbs.Width - xL2 < sf.Width)
                        {
                            e.Graphics.DrawLine(removedPen, xL2, 0, pbs.Width, pbs.Height);
                            e.Graphics.DrawLine(removedPen, xL2, pbs.Height, pbs.Width, 0);
                        }
                        else
                            e.Graphics.DrawString(bo, SystemFonts.DefaultFont, Brushes.White, (pbs.Width + xL2) / 2, pbs.Height / 2, signalViewer.mainView.sfc);

                        e.Graphics.DrawLine(DFTPen, 0, pbs.Height - 3, xL1, pbs.Height - 3);
                        e.Graphics.DrawLine(DFTPen, xL2, pbs.Height - 3, pbs.Width, pbs.Height - 3);
                    }


                    e.Graphics.DrawLine(limitPen, xL1, 0, xL1, pbs.Height);
                    e.Graphics.DrawLine(limitPen, xL2, 0, xL2, pbs.Height);

                    e.Graphics.DrawString("0 Hz", SystemFonts.DefaultFont, Brushes.White, 0, pbs.Height - 14);
                    e.Graphics.DrawString(str, SystemFonts.DefaultFont, Brushes.White, pbs.Width - 1, pbs.Height - 14, signalViewer.mainView.sfr);
                }
            }
            catch (Exception ex)
            {
                e.Graphics.DrawString("ERR:" + ex.Message + " | " + ex.Source + " | " + ex.StackTrace, SystemFonts.DefaultFont, Brushes.Black, 0, 0);
            }
        }

        private void pbs_Resize(object sender, EventArgs e)
        {
            pbs.Refresh();
        }

        private void nuP1_ValueChanged(object sender, EventArgs e)
        {
            limit1 = (float) nuP1.Value;
            limit2 = (float) nuP2.Value;
            
            if (!movingP1 && !movingP2)
            onlyChangeLimits(limit1, limit2, true, false);
        }

        private void nuP2_ValueChanged(object sender, EventArgs e)
        {
            limit1 = (float)nuP1.Value;
            limit2 = (float)nuP2.Value;
            if (!movingP1 && !movingP2)
            onlyChangeLimits(limit1, limit2, false, true);

        }

        private void chbBS_CheckedChanged(object sender, EventArgs e)
        {
            bandStop = chbBS.Checked;
            
            if (!movingP1)
            onlyChangeLimits(limit1, limit2, false, false);
        }

        private void pbs_MouseMove(object sender, MouseEventArgs e)
        {
            float fsPul = signalViewer.mainView.sampleFrequency / 2;
            float xScaleLim = (float)(pbs.Width) / fsPul;

            float xL1 = limit1 * xScaleLim;
            float xL2 = limit2 * xScaleLim;

            bool aboveL1 = e.X > xL1 - radLim && e.X < xL1 + radLim;
            bool aboveL2 = e.X > xL2 - radLim && e.X < xL2 + radLim;

            bool aboveMid = (e.X >= xL1 + radLim) && (e.X <= xL2 - radLim);

            if (aboveL1 || aboveL2 || movingP1 || movingP2)
                pbs.Cursor = Cursors.VSplit;
            else if (aboveMid)
                pbs.Cursor = Cursors.Hand;
            else
                pbs.Cursor = Cursors.Default;

            if (movingP1)
                limit1 = (float)e.X / xScaleLim;

            if (movingP2)
                limit2 = (float)e.X / xScaleLim;


            if (movingMid)
            {
                float dl = (float)(e.X - lastMpos) / xScaleLim;
                limit1 += dl;
                limit2 += dl;

                lastMpos = e.X;
            }

            if (movingP1 || movingP2 || movingMid)
            {
                onlyChangeLimits(limit1, limit2, false, false);
            }

        }

        private void pbs_MouseDown(object sender, MouseEventArgs e)
        {
            float fsPul = signalViewer.mainView.sampleFrequency / 2;
            float xScaleLim = (float)(pbs.Width) / fsPul;

            float xL1 = limit1 * xScaleLim;
            float xL2 = limit2 * xScaleLim;

            bool aboveL1 = e.X > xL1 - radLim && e.X < xL1 + radLim;
            bool aboveL2 = e.X > xL2 - radLim && e.X < xL2 + radLim;
            bool aboveMid = (e.X >= xL1 + radLim) && (e.X <= xL2 - radLim);

                if (aboveL1 && e.Button == System.Windows.Forms.MouseButtons.Left)
                {
                    movingP1 = true;
                    movingP2 = false;
                    lastMpos = e.X;
                }

                if (aboveL2 && e.Button == System.Windows.Forms.MouseButtons.Left && !movingP1)
                {
                    movingP2 = true;
                    movingP1 = false;
                    lastMpos = e.X;
                }

                if (aboveMid && e.Button == System.Windows.Forms.MouseButtons.Left && !movingP1 && !movingP2)
                {
                    movingMid = true;
                    lastMpos = e.X;
                }
        }

        private void pbs_MouseUp(object sender, MouseEventArgs e)
        {
            movingP1 = false;
            movingP2 = false;
            movingMid = false;

            pbs.Cursor = Cursors.Default;

            onlyChangeLimits(limit1, limit2, false, false);

        }

        private void numericUpDown1_ValueChanged(object sender, EventArgs e)
        {
            if (!movingP1)
            {
                refrControls();
                dlen = (int)numericUpDown1.Value;
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            UseWaitCursor = true;
            bgProc.RunWorkerAsync();
        }

        private void bgProc_DoWork(object sender, DoWorkEventArgs e)
        {
            graphPanel gp;

            Stopwatch watch1 = new Stopwatch();
            watch1.Start();
            int iter = 1;
            foreach (signalViewer.graphPanel ach in linkedChannels)
            {
                try
                {

                    // signalViewer.graphPanel ach = linkedChannels[0];

                    
                    string name = "CUDA FFT filter: ";

                    if (bandStop == true)
                    {
                        name += " BS ";
                    }
                    else
                    {
                        name += " BP ";
                    }
                    if (doEnvelope == true)
                    {
                        name += "ENVELOPE";
                    }
                    name += "<" + limit1.ToString("0.0") + " Hz, " + limit2.ToString("0,0") + " Hz> ";
                    name += ach.channelName;

                    float[] outputCUDA = CUDA_FFT_filter2(ach.dataCache[0].data.ToArray(), limit1, limit2, bandStop, doEnvelope, winFunction.winType, 0);
                    if (outputCUDA != null)
                    {
                        gp = new graphPanel(name, outputCUDA.ToList(), mv);
                        mv.addNewChannel(gp, 0, true);
                        mv.rebuiltAndRedrawAll();

                    }
                    else
                    {
                        break;
                        return;
                    }
                    this.Invoke((MethodInvoker)delegate
                    {
                        pg1.Value = (int)(100*iter/ linkedChannels.Count);
                        labProg.Text = "Processing: " + iter.ToString() + "/" + linkedChannels.Count.ToString();

                    });
                }
                catch (Exception exc)
                {
                    
                    
                }
                iter++;
                //COMRE.Set();
            }
            watch1.Stop();
            globalVariables.setVar("@CUDA_FFT_FILTER_TIME: ", watch1.ElapsedMilliseconds.ToString());

        }
        public float[] errorFunc(float[] a, float[] b)
        {
            float[] c = new float[a.Count()];
            for (int i = 0; i < a.Count(); i++)
            {
                c[i] = a[i] - b[i];
            }
            return c;
        }

        public float[] trend(float[] a)
        {
            float[] temp = new float[a.Count()];
            for (int i = 0; i < a.Count(); i++)
            {
                temp[i] = ((a.Last()-a.First())*i/(a.Count()-1)+a.First());
            }
            return temp;
        }
        public float[] detrend(float[] x, float[] a)
        {
            
            for (int i = 0; i < a.Count(); i++)
            {
                x[i] =x[i]-a[i];
            }
            return x;
        }
        public float[] retrend(float[] x,float[] a)
        {
            
            for (int i = 0; i < a.Count(); i++)
            {
                x[i] = x[i] + a[i];
            }
            return x;
        }

        public float[] CUDA_FFT_filter(float[] inputPre, float limit1, float limit2, bool bandStop, bool doEnvelope, int windowType, int windowParam)
        {
            CudaContext ctx = new CudaContext();
            //float[] trend_func = trend(inputPre);
            //inputPre = detrend(inputPre, trend_func);

            int neededLength = FFT.convertToHigherPow2(inputPre.Length);

            //kernel path
            string path = Path.GetDirectoryName(mv.plugins[0].filename);

            //alloc data to cuda format 
            double2[] temp_x = new double2[neededLength];

            //data copy
            for (int i = 0; i < inputPre.Count(); i++)
            {
                temp_x[i].x = inputPre[i];
            }


            //fft cuda plan set
            CudaFFTPlan1D plan1D = new CudaFFTPlan1D(neededLength, cufftType.Z2Z, 1);
            CudaKernel kernel = ctx.LoadKernel(path + "\\kernel.ptx", "SpectLinesZeroCUDA");
            kernel.GridDimensions = (int)Math.Ceiling((double)(inputPre.Count()) / 1024);
            kernel.BlockDimensions = 1024;

            //alloc GPU data
            CudaDeviceVariable<double2> d_x = null;
            try
            {
                d_x = temp_x;
            }
            catch (Exception e)
            {
                mainView.log(e, "CUDA alloc data error", this);
                return null;
            }

            int n1 = freq2sample(limit1, mainView.sampleFrequency, neededLength);
            int n2 = freq2sample(limit2, mainView.sampleFrequency, neededLength);

            plan1D.Exec(d_x.DevicePointer, TransformDirection.Forward);
            kernel.Run( d_x.DevicePointer, neededLength, n1,n2);

            plan1D.Exec(d_x.DevicePointer, TransformDirection.Inverse);
            temp_x = d_x;

            //return retrend(temp_x.Select(data => (float)data.x).ToArray(), trend_func);
            float[] output = new float[inputPre.Count()];

            Array.Copy(temp_x.Select(data => (float)data.x).ToArray(),output, inputPre.Count());
            return output;
        }


        public float[] CUDA_FFT_filter2(float[] inputPre, float limit1, float limit2, bool bandStop, bool doEnvelope, int windowType, int windowParam)
        {

            int dlen = inputPre.Length;

            string path = Path.GetDirectoryName(mv.plugins[0].filename);

            int neededLength = FFT.convertToHigherPow2(inputPre.Length);

            double[] sourceD = new double[neededLength];

            CudaContext ctx = new CudaContext();
            CudaKernel kernel = ctx.LoadKernel(path + "\\kernel.ptx", "SpectrumNullSPmethodCUDA");
            kernel.GridDimensions = (int)Math.Ceiling((double)(neededLength) / 1024);
            kernel.BlockDimensions = 1024;

            CudaFFTPlan1D plan = new CudaFFTPlan1D(neededLength, cufftType.Z2Z, 1);
            CudaDeviceVariable<double2> d_x = null;

            double2[] complex_temp = new double2[neededLength];

            for (int i = 0; i < inputPre.Length; i++)
            {
                sourceD[i] = inputPre[i];
                complex_temp[i].x = inputPre[i];
            }

            try
            {
                d_x = complex_temp;
                plan.Exec(d_x.DevicePointer, TransformDirection.Forward);
                complex_temp = d_x;
            }
            catch (Exception e)
            {
                MessageBox.Show("CUDA Error:" + e.Message);
                return null;
            }


            float freqBinWidth = ((float)signalViewer.mainView.sampleFrequency) / ((float)(neededLength));



            int limit1Sample = (int)(limit1 / freqBinWidth);
            int limit2Sample = (int)(limit2 / freqBinWidth);

            int tempBandstop = Convert.ToInt32(bandStop);
            int tempEnvelope = Convert.ToInt32(doEnvelope);

            try
            {
                
                kernel.Run(d_x.DevicePointer, neededLength, (double)freqBinWidth, (double)limit1,(double)limit2,tempBandstop,tempEnvelope);
                complex_temp = d_x;
                plan.Exec(d_x.DevicePointer, TransformDirection.Inverse);
                complex_temp = d_x;
            }
            catch (Exception e)
            {
                MessageBox.Show("CUDA Error:"+e.Message);
                return null;
            }

            
            float[] output = new float[inputPre.Count()];
            for (int i = 0; i < inputPre.Count(); i++)
            {
                if (doEnvelope==true)
                {
                    output[i] = (float)Math.Sqrt(complex_temp[i].x* complex_temp[i].x+ complex_temp[i].y* complex_temp[i].y) / neededLength;
                }
                else
                {
                    output[i] = (float)complex_temp[i].x / neededLength;
                }           
            }
            d_x.Dispose();
            plan.Dispose();
            ctx.Dispose();
            return output;
            
        }

        public void SpectrumNull(double [][]x,float freqBinWidth,List<float> window,float limit1, float limit2, int limit1Sample)
        {

            for (int i = 0; i < x[0].Length / 2; i++)
            {
                float freq = (float)i * freqBinWidth;

                bool toDel = freq >= limit1 && freq <= limit2;
                if (!bandStop) toDel = !toDel;
                
                int i2 = x[0].Length - i - 1;


                bool insideLimits = freq >= limit1 && freq <= limit2;

                if (insideLimits)
                {
                    float konst = 0;

                    int winIndex = i - limit1Sample - 1;
                    if (winIndex < window.Count && winIndex >= 0)
                    {
                        konst = window[winIndex];

                        x[0][i] *= konst;
                        x[1][i] *= konst;

                        x[0][i2] *= konst;
                        x[1][i2] *= konst;
                    }
                }


                if (toDel && !insideLimits)
                {
                    x[0][i] = 0;
                    x[1][i] = 0;

                    x[0][i2] = 0;
                    x[1][i2] = 0;
                }

            }
        }

        public void SpectLinesZeroCUDA(double2[] b, int n, int s1, int s2)
        {

            for (int id = 0; id < b.Count(); id++)
            {
                b[id].x = b[id].x / n;
                b[id].y = b[id].y / n;
                if (id < s2 && id > s1)
                {
                    b[id].x = 0;
                    b[id].y = 0;
                    b[n - 1 - id].x = 0;
                    b[n - 1 - id].y = 0;
                }

            }
        }



        public float[] getAbsValue(double2[] x)
        {
             float[] temp =new float[x.Count()];
            for (int i = 0; i < x.Count(); i++)
            {
                temp[i] =(float) Math.Sqrt(x[i].x* x[i].x + x[i].y* x[i].y);
            }
            return temp;
        }


        public int freq2sample(float f, float fs, int N)
        {
            float k = fs / N;
            int n = (int)(f / k);
            return n;
        }

        private void bgProc_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            COMRE.Set();

            mv.rebuiltAndRedrawAll();
            signalViewer.mainView.actualizePluginForms();

            if (this.Parent == null)
            {

            }
            else
            {
                UseWaitCursor = false;
                this.ParentForm.Close();
            }
        }

        private void bgProc_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            //labProg.Text = fftState;
            //pg1.Value = e.ProgressPercentage;
        }


        public static string CMDDESCRIPTION_FFTF()
        {
            return "FFT filter. Use : FFTF CHANNEL(V1;V2;V3) FREQ(LEFT; RIGHT[; BANDPASS][; ENVELOPE]) [WINDOW(WINDOW_TYPE [,WINDOW_PARAM])]";
        }

        public string COMMAND_FFTF(string parameters)
        {
            // dekompozice



            try
            {

                mv = (mainView)Application.OpenForms[0]; 

                string[] pars = parameters.Split(' ');

                if (pars.Length < 3) throw (new Exception("Include all parameters"));

                if (parameters.IndexOf("CHANNEL(") < 5) throw (new Exception("Include some channels, i.e.: CHANNELS(V1)"));

                if (parameters.IndexOf("FREQ(") < 5) throw (new Exception("Describe frequencies, i.e.: FREQ(48.9;52.2[;BANDPASS] [;ENVELOPE])."));


                //--procházení parametrů a nastavení podmínek
                for (int i = 1; i < pars.Length; i++)
                {
                    string msg = pars[i];



                    if (msg.Length >= 9 && msg.Substring(0, 8).Equals("CHANNEL("))
                    {

                        string filtr = msg.Substring(8, msg.Length - 9);

                        for (int q = 0; q < mv.gpList.Count; q++)
                        {
                            if (signalViewer.mainView.respectsFilter(mv.gpList[q].channelName, filtr))
                                linkedChannels.Add(mv.gpList[q]);
                        }
                    }

                    if (msg.Length > 9 && msg.Substring(0, 5).Equals("FREQ("))
                    {
                        string[] winpars = msg.Substring(5, msg.Length - 6).Split(';');
                        if (winpars.Length <2 || winpars.Length>4) throw (new Exception("Bad number of frequency parameters. Should be 2,3 or 4."));


                        float fL = Convert.ToSingle(winpars[0]);
                        float fR = Convert.ToSingle(winpars[1]);


                        limit1 = fL;
                        limit2 = fR;

                        bandStop = true;

                        if (winpars.Length >= 3)
                            if (winpars[2].Equals("BANDPASS")) bandStop = false;

                        if (winpars.Length == 4)
                            if (winpars[3].Equals("ENVELOPE")) doEnvelope = true;
                    }

                    if (msg.IndexOf("WINDOW(") >= 0)
                    {

                        string ps = msg.Substring(msg.IndexOf("WINDOW"), msg.Length - msg.IndexOf("WINDOW"));

                        string abrev = mainView.insideAbbrevs(ps);


                        string[] parts = abrev.Split(';');

                        if (parts.Length < 1  || parts.Length>=3) return "Needs 1 or 2 params!";

                        winFunction.winType = (int) mainView.convertToSingleGeneral(parts[0]);

                        if (parts.Length == 2) winFunction.winParam = mainView.convertToSingleGeneral(parts[1]);
                        

                    }


                }

                bgProc.RunWorkerAsync();
                
                COMRE.WaitOne();


                int r = 0;
            }
            catch (Exception e)
            {
                return "Error:" + e.Message;
            }


            return ("Completed Succesfully");

        }

        private void chbAkt_CheckedChanged(object sender, EventArgs e)
        {

        }

        private void chEnvelope_CheckedChanged(object sender, EventArgs e)
        {
            doEnvelope = chEnvelope.Checked;
            if (!movingP1)
            onlyChangeLimits(limit1, limit2, false, false);
        }

        private void button3_Click(object sender, EventArgs e)
        {
            signalViewer.windowFunc wfd = new signalViewer.windowFunc();

            

            wfd.wf.winType = winFunction.winType;
            wfd.wf.winParam = winFunction.winParam;



            wfd.Location = new Point(button3.Left + this.ParentForm.Location.X, button3.Top + this.ParentForm.Location.Y);

            if (wfd.ShowDialog() == DialogResult.OK)
            {
                string str = signalViewer.WindowingFunction.windowNames[wfd.wf.winType];

                if (signalViewer.WindowingFunction.numParams[wfd.wf.winType] > 0)
                {
                    str += " (" + wfd.wf.winParam.ToString() + ")";
                }

                button3.Text = str;

                winFunction.winType = wfd.wf.winType;
                winFunction.winParam = wfd.wf.winParam;

                prepareFilter();

                doPreview();
            }
        }

        private void rbFitOrig_CheckedChanged(object sender, EventArgs e)
        {
            refrControls();
        }

        private void rbFitRes_CheckedChanged(object sender, EventArgs e)
        {
            refrControls();
        }

        private void rbFitAll_CheckedChanged(object sender, EventArgs e)
        {
            refrControls();
        }

        private void rbFitBoth_CheckedChanged(object sender, EventArgs e)
        {
            refrControls();
        }

        private void chbSFFT_CheckedChanged(object sender, EventArgs e)
        {
            refrControls();
        }

        private void chbBS_CheckedChanged_1(object sender, EventArgs e)
        {
            bandStop = chbBS.Checked;

            if (!movingP1)
                onlyChangeLimits(limit1, limit2, false, false);
            refrControls();
        }

        private void pigTimer_Tick(object sender, EventArgs e)
        {
            //if (!bgw.IsBusy && !bgProc.IsBusy)
            //{
            //    labProg.Text = "Iddle...";
            //    pg1.Value = 0;
            //    return;
            //}
            //string txt = "Working on FFT... "+cc+" of "+linkedChannels.Count + " done.";
            //labProg.Text = txt;
            //pg1.Value = (int)FFT.fftProc;
        }

        private void chbPower_CheckedChanged(object sender, EventArgs e)
        {
            powerEnvelope = chbPower.Checked;
            if (!movingP1)
                onlyChangeLimits(limit1, limit2, false, false);
        }

        private void label12_Click(object sender, EventArgs e)
        {

        }

        // test kernel for correct function
        public void test(int N)
        {
            CudaContext ctx = new CudaContext();
            CudaKernel kernel = ctx.LoadKernel("kernel.ptx", "SpectLinesZeroCUDA");
            kernel.GridDimensions = N;
            kernel.BlockDimensions = 1;
            double2[] a = new double2[N];
            double2[] b = new double2[N];
            double2[] c = new double2[N];
            for (int i = 0; i < N; i++)
            {
                a[i].x = i+1;
                a[i].y = 0;
                b[i].x = i+1;
                b[i].y = 0;
            }

            CudaDeviceVariable<double2> d_a = null;
            CudaDeviceVariable<double2> d_b = null;

            try
            {
                d_a = a;
                d_b = b;
            }
            catch (Exception e)
            {
                Console.WriteLine("{0} Exception caught.", e);
                return;
            }
            kernel.Run(d_b.DevicePointer,N,0,0);
            c = d_b;


        }

        //test fft filtering 
        public void test2(int N)
        {
            CudaContext ctx = new CudaContext();
            CudaKernel kernel = ctx.LoadKernel("kernel.ptx", "SpectLinesZeroCUDA");
            CudaFFTPlan1D plan1D = new CudaFFTPlan1D(N, cufftType.Z2Z, 1);
            kernel.GridDimensions = N;
            kernel.BlockDimensions = 1;
            double2[] a = new double2[N];
            double2[] b = new double2[N];
            double2[] c = new double2[N];
            for (int i = 0; i < N; i++)
            {
                a[i].x = i + 1;
                a[i].y = 0;
            }

            CudaDeviceVariable<double2> d_a = null;
            CudaDeviceVariable<double2> d_b = null;

            try
            {
                d_a = a;
            }
            catch (Exception e)
            {
                Console.WriteLine("{0} Exception caught.", e);
                return;
            }
            plan1D.Exec(d_a.DevicePointer, TransformDirection.Forward);
            c = d_a;
            kernel.Run(d_a.DevicePointer, N, 20, 30);
            c = d_a;
            plan1D.Exec(d_a.DevicePointer, TransformDirection.Inverse);
            
            c = d_a;


        }

        private void radioButton1_CheckedChanged(object sender, EventArgs e)
        {
            refrControls();
        }
    }
}
