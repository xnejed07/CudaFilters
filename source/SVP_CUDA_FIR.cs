using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using signalViewer;
using System.Threading;
using System.IO;
using ManagedCuda.CudaFFT;
using ManagedCuda;
using ManagedCuda.BasicTypes;
using ManagedCuda.VectorTypes;
using System.Diagnostics;

namespace plugins_cuda_filters
{
    public partial class SVP_CUDA_FIR : UserControl
    {
        // CUDA setup
        //CudaContext ctx = new CudaContext();
        bool cudaOK = true;

        // SP setup
        int FirType=1;
        List<signalViewer.graphPanel> linkedChannels = new List<signalViewer.graphPanel>(); //--array of linked channels
        mainView mv; //-- instance of signal viewer program
        WindowingFunction wf = new WindowingFunction(40, WindowingFunction.windowsType.Hamming, 0); //--windowing function for filter
        bool presetLoading;


        public string getDeveloperInfo()
        {
            return "Petr Nejedlý, 2016"; //----change to your name
        }

        public string getDescription()
        {
            return "Compute FIR filter in CUDA GPU"; //----enter description for your plugin
        }

        public string getCategory()
        {
            return "CUDA"; //---set category in plugins menu. If it does not exists, new category will be created
        }


        public string getName()
        {
            return "CUDA FIR";        //---plugin name, visible in Plugins menu
        }

        public void doExternalRefresh()
        {
            /*
             *  This is calle from signalViewer, when doing RefreshAllPluginForms()
             */

            refrControls();
            doPreviewWork();

        }


        public void presetBeforeLoaded()
        {
            /*
          This funciotn is called after user clicks on preset in menu, but before corresponding controls are filled with new values. 
           */
            presetLoading = true;
        }

        public void presetLoaded()
        {
            /*
             * This funciotn is called after user clicks on preset in menu. Corresponding controls (with filled TAG field) receives values from preset
             * and here you have to set values from controls to corresponding fields
            */

            presetLoading = false;
            refrControls();
        }


        


        private void refrControls()
        {
            /*
             * This function is called for refreshing a plugin form. 
             
             * */
            
            btChannels.Text = "";
            if (linkedChannels.Count == 0) btChannels.Text = "Drag a channel here or click";
            if (linkedChannels.Count == 1) btChannels.Text = "Channel : " + linkedChannels[0].channelName;
            if (linkedChannels.Count > 1) btChannels.Text = linkedChannels.Count.ToString() + " channels are choosed";

            btProcess.Enabled = linkedChannels.Count > 0;

            if (bgw.IsBusy && bgw.CancellationPending) btProcess.Text = "Wait for cancellation";
            if (bgw.IsBusy && !bgw.CancellationPending) btProcess.Text = "Cancel";
            if (!bgw.IsBusy) btProcess.Text = "Process";
            if (numOrder.Value==0 || numFreq1.Value==0 || cudaOK==false) btProcess.Enabled = false;
            button1.Text = signalViewer.WindowingFunction.windowNames[wf.winType];
            if (cudaOK == false) { this.Enabled = false; }




        }

        private void doPreviewWork()
        {
            /*This function creates anything for preview. Not neccesary to implement
             * */

        }

        private void btChannels_DragDrop(object sender, DragEventArgs e)
        {
            /*
             *This method add dragged channel to linkedchannels
             */
            string s = e.Data.GetData(DataFormats.Text).ToString();

            try
            {
                linkedChannels.Add(mv.getGPbyName(s));
                refrControls();
                doPreviewWork();

            }
            catch (Exception exp)
            {
                mainView.log(exp, "Error while drag&drop", this); // this line will log error into "errorlog.txt"
                MessageBox.Show("Error:" + exp.Message);
            }
        }

        private void btChannels_DragEnter(object sender, DragEventArgs e)
        {
            string s = e.Data.GetData(DataFormats.Text).ToString();

            if (mv.getGPbyName(s) != null)
            {
                e.Effect = DragDropEffects.Copy;
            }
            else
                e.Effect = DragDropEffects.None;
        }

        private void btChannels_Click(object sender, EventArgs e)
        {
            signalViewer.selectChannelForm sc = new signalViewer.selectChannelForm();
            sc.regenerateList(linkedChannels);

            if (sc.ShowDialog() == DialogResult.OK)
            {
                linkedChannels.Clear();
                for (int i = 0; i < sc.lv.SelectedItems.Count; i++)
                {
                    linkedChannels.Add(mv.getGPbyName(sc.lv.SelectedItems[i].Text));
                    
                }
                refrControls();
                doPreviewWork();
            }
            
        }


        private void btProcess_Click(object sender, EventArgs e)
        {
            if (!bgw.IsBusy)
            {
                bgw.RunWorkerAsync();
            }
            else
            {
                bgw.CancelAsync();
            }

            refrControls();
        }

        public SVP_CUDA_FIR()
        {
            InitializeComponent();



        }

        private void bgw_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {

        }

        private void pbx_Paint(object sender, PaintEventArgs e)
        {

        }

        private void bgw_DoWork(object sender, DoWorkEventArgs e)
        {

           



            graphPanel gp;

            Stopwatch stopwatch0 = new Stopwatch();
            Stopwatch stopwatch1 = new Stopwatch();
            
            try
            {
                stopwatch0.Start();

                for (int ch = 0; ch < linkedChannels.Count; ch++)
                {

                    this.Invoke((MethodInvoker)delegate
                    {
                        label10.Visible = true;
                        label10.Text = "Processing channel " + (ch + 1).ToString() + "/" + linkedChannels.Count.ToString();
                        progressBar1.Value = (int)(100 * ch / linkedChannels.Count);

                    });
                    
                    List<double> data = Array.ConvertAll<float, double>(linkedChannels[ch].dataCache[0].data.ToArray(), d => (double)d).ToList();
                    List<float> qq = hypotesis_long_save(data, getFirFilter((int)numOrder.Value, (double)numFreq1.Value, (double)numFreq2.Value, FirType, wf).coeffs, 2000000);


                    if (qq != null)
                    {
                        string name = null;
                        switch (FirType)
                        {
                            case 1://Low pass
                                name = "FIR-CUDA Low pass- o:" + numOrder.Value + " - fc:" + numFreq1.Value + "Hz";
                                break;
                            case 2://High pass
                                name = "FIR-CUDA High pass- o:" + numOrder.Value + " - fc:" + numFreq1.Value + "Hz";
                                break;
                            case 3://Band pass
                                name = "FIR-CUDA Band pass- o:" + numOrder.Value + " - f1:" + numFreq1.Value + "Hz - f2:" + numFreq2.Value + "Hz";
                                break;
                            case 4://Band stop
                                name = "FIR-CUDA Band stop- o:" + numOrder.Value + " - f1:" + numFreq1.Value + "Hz - f2:" + numFreq2.Value + "Hz";
                                break;

                        }
                        linkedChannels[ch].dataCache.Add(new signalViewer.dataCacheLevel(name, qq));

                    }
                    

                }
                stopwatch0.Stop();
                globalVariables.setVar("@CUDA_FIR_TIME: " , stopwatch0.ElapsedMilliseconds.ToString());

                this.Invoke((MethodInvoker)delegate
                {
                    
                    label5.Text = "Time elapsed: " +stopwatch0.ElapsedMilliseconds.ToString(); 

                });
            }
            catch (Exception exp)
            {
                mainView.log(exp,"Do work block error", this);
            }
        }

        List<float> ListSubs(List<float>a, List<float>b)
        {
            List<float> c = new List<float>(new float[a.Count]);
            for (int i = 0; i < a.Count; i++)
            {
                c[i] = a[i] - b[i];
            }
            return c;
        }

        private void bgw_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            progressBar1.Value = (int)(100 * 0 / linkedChannels.Count);
            label10.Visible = false;
            refrControls();



        }

        private void btMarks_Click(object sender, EventArgs e)
        {

        }

        private void SVP_plugin_v3_Load(object sender, EventArgs e)
        {
            if (this.ParentForm != null) mv = (mainView)Application.OpenForms[0]; //--this links mv property to signalplant application
            numFreq1.Maximum = System.Convert.ToDecimal(mainView.sampleFrequency / 2);
            numFreq2.Maximum = System.Convert.ToDecimal(mainView.sampleFrequency / 2);
            numOrder.Enabled = true;
            numFreq1.Enabled = true;
            numFreq2.Enabled = false;
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
            label2.Text = (result[0].TotalGlobalMemory / (1024 * 1024)).ToString() + " MB";
            label3.Text = (result[0].ClockRate / 1000).ToString() + " MHz";
            if (result[0].ComputeCapability.Major >= 3)
            {
                label4.Text = (result[0].ComputeCapability).ToString();
            }
            else
            {
                label4.Text = (result[0].ComputeCapability).ToString() + ". GPU Compute capability not supported.";
                label4.BackColor = Color.Red;
                MessageBox.Show("GPU Compute capability not supported. Compute capability 3.0 or greater is supported.");
                cudaOK = false;
            }
            
            label10.Visible = false;
            if (cudaOK == false) { this.Enabled = false; }
            refrControls();

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


        filter_FIR getFirFilter(int order, double cutOffFrequeny1, double cutOffFrequeny2, int type, WindowingFunction wf)
        {
            filter_FIR filter = new filter_FIR();


            switch (type)
            {
                case 1://Low pass
                    filter = filter.generateLowPass(order, cutOffFrequeny1, wf);
                    break;
                case 2://High pass
                    filter = filter.generateHighPass(order, cutOffFrequeny1, wf);
                    break;
                case 3://Band pass
                    filter = filter.generateBandPass(order, cutOffFrequeny1, cutOffFrequeny2, wf);
                    break;
                case 4://Band stop
                    filter = filter.generateBandStop(order, cutOffFrequeny1, cutOffFrequeny2, wf);
                    break;

            }
            return filter;
        }

        public List<float> CUDA_FIR(List<float> x, List<double> h)
        {
            CudaContext ctx = new CudaContext();
            //alloc data to cuda format 
            double2[] temp_x = new double2[x.Count + h.Count - 1];
            double2[] temp_h = new double2[x.Count + h.Count - 1];
            double2[] temp_y = new double2[x.Count + h.Count - 1];

            //data copy
            for (int i = 0; i < x.Count; i++)
            {
                temp_x[i].x = x[i];
            }
            for (int i = 0; i < h.Count; i++)
            {
                temp_h[i].x = h[i];
            }


            CudaDeviceVariable<double2> d_x = null;
            CudaDeviceVariable<double2> d_h = null;


            CudaFFTPlan1D plan1D = new CudaFFTPlan1D(x.Count + h.Count - 1, cufftType.Z2Z, 1);
            CudaKernel kernel = ctx.LoadKernel("kernel.ptx", "ComplexMultCUDA");
            kernel.GridDimensions = (int)Math.Ceiling((double)(x.Count + h.Count - 1) / 1024);
            kernel.BlockDimensions = 1024;

            try
            {
                d_x = temp_x;
                d_h = temp_h;
            }
            catch (Exception e)
            {
                //("{0} Exception caught.", e);
                return null;
            }

            plan1D.Exec(d_x.DevicePointer, TransformDirection.Forward);
            plan1D.Exec(d_h.DevicePointer, TransformDirection.Forward);
            kernel.Run(d_h.DevicePointer, d_x.DevicePointer, x.Count + h.Count - 1);
            plan1D.Exec(d_x.DevicePointer, TransformDirection.Inverse);
            temp_y = d_x;


            return temp_y.Select(data =>(float)data.x).ToList().GetRange(h.Count/2,x.Count);
        }

        public List<float> CUDA_FIR_long(List<float> x, List<double> h)
        {
            CudaContext ctx = new CudaContext();
            string path = Path.GetDirectoryName(mv.plugins[0].filename);
            

            int N = 2000000;
            //alloc data to cuda format 
            double2[][] temp_x = new double2[(int)Math.Ceiling((double)(x.Count + h.Count - 1)/(N + h.Count - 1))][];
            double2[] temp_h = new double2[N + h.Count - 1];
            double2[][] temp_y = new double2[(int)Math.Ceiling((double)(x.Count + h.Count - 1) / (N + h.Count - 1))][];


            //data copy 

            System.Threading.Tasks.Parallel.For(0, (int)Math.Ceiling((double)(x.Count + h.Count - 1) / (N + h.Count - 1)), j => {          
                    temp_x[j] = new double2[N + h.Count - 1];
                    temp_y[j] = new double2[N + h.Count - 1];
                    for (int i = 0; (j * N + i) < x.Count && i < N; i++)
                    {
                        temp_x[j][i].x = x[j * N + i];
                    }
                
            });

            for (int i = 0; i < h.Count; i++)
            {
                temp_h[i].x = h[i];
            }



            CudaDeviceVariable<double2> d_x = null;
            CudaDeviceVariable<double2> d_h = null;


            CudaFFTPlan1D plan1D = new CudaFFTPlan1D(N + h.Count - 1, cufftType.Z2Z, 1);
           
            CudaKernel kernel = ctx.LoadKernel(path+"\\kernel.ptx", "ComplexMultCUDA");
            kernel.GridDimensions = (int)Math.Ceiling((double)(N + h.Count - 1) / 1024);
            kernel.BlockDimensions = 1024;

            try
            {
                d_h = temp_h;
            }
            catch (Exception e)
            {
                //("{0} Exception caught.", e);
                return null;
            }
            plan1D.Exec(d_h.DevicePointer, TransformDirection.Forward);

            for (int g = 0; g < (int)Math.Ceiling((double)(x.Count + h.Count - 1) / (N + h.Count - 1)); g++)
            {
                try
                {
                    d_x = temp_x[g];
                }
                catch (Exception e)
                {
                    mainView.log(e, "cuda alloc data error", this);
                    return null;
                }

                try
                {
                    plan1D.Exec(d_x.DevicePointer, TransformDirection.Forward);
                    kernel.Run(d_h.DevicePointer, d_x.DevicePointer, N + h.Count - 1);
                    plan1D.Exec(d_x.DevicePointer, TransformDirection.Inverse);
                }
                catch (Exception exp)
                {
                    mainView.log(exp, "kernel run cuda error", this);
                }
                temp_y[g] = d_x;

                //this.Invoke((MethodInvoker)delegate
                //{
                //    progressBar1.Value = (int)(50/ (int)Math.Ceiling((double)(x.Count + h.Count - 1) / (N + h.Count - 1)))*g;

                //});
                d_x.Dispose();
            }
            d_h.Dispose();
            
            plan1D.Dispose();

            return OverlapAdd(temp_y, h.Count).GetRange(h.Count / 2, x.Count);
        }

        public List<float> hypotesis(List<double> x, List<double> h,int N)
        {
            //int N = 2000000;

            string path = Path.GetDirectoryName(mv.plugins[0].filename);
            CudaContext ctx = new CudaContext();
            CudaKernel kernel = ctx.LoadKernel(path + "\\kernel.ptx", "ComplexMultCUDA");
            kernel.GridDimensions = (int)Math.Ceiling((double)(N + h.Count - 1) / 1024);
            kernel.BlockDimensions = 1024;

            double[] temp_y = new double[N + h.Count - 1];
            double[] temp_h = new double[N + h.Count - 1];
            double[] temp_x = new double[N + h.Count - 1];

            double2[] temp_x2 = new double2[N + h.Count - 1];


            h.ToArray().CopyTo(temp_h, 0);
            x.ToArray().CopyTo(temp_x, 0);
            
            CudaDeviceVariable<double> d_x = null;
            CudaDeviceVariable<double2> d_X = new CudaDeviceVariable<double2>(N + h.Count - 1);

            CudaDeviceVariable<double> d_h = new CudaDeviceVariable<double>(N + h.Count - 1);
            CudaDeviceVariable<double2> d_H = new CudaDeviceVariable<double2>(N + h.Count - 1);

            CudaDeviceVariable<double> d_y = new CudaDeviceVariable<double>(N + h.Count - 1);


            CudaFFTPlan1D planForward = new CudaFFTPlan1D(N + h.Count - 1, cufftType.D2Z, 1);
            CudaFFTPlan1D planInverse = new CudaFFTPlan1D(N + h.Count - 1, cufftType.Z2D, 1);

            try
            {
                d_h = temp_h;
                planForward.Exec(d_h.DevicePointer, d_H.DevicePointer, TransformDirection.Forward);
            }
            catch (Exception exp)
            {
                mainView.log(exp, "CUDA error: Impulse response FFT", this);
                return null;
            }

            try
            {
                d_x = temp_x;
                planForward.Exec(d_x.DevicePointer, d_X.DevicePointer);
                kernel.Run(d_H.DevicePointer, d_X.DevicePointer, N + h.Count - 1);
                planInverse.Exec(d_X.DevicePointer, d_y.DevicePointer);
            }
            catch (Exception exp)
            {
                mainView.log(exp, "Cuda error: kernel run cuda error", this);
            }
            temp_y = d_y;
             
            return Array.ConvertAll<double, float>(temp_y, d => (float)d).ToList().GetRange(500, x.Count);
        }


        public List<float> hypotesis_long(List<double> x, List<double> h, int N)
        {
            //int N = 2000000;

            string path = Path.GetDirectoryName(mv.plugins[0].filename);
            CudaContext ctx = new CudaContext();
            CudaKernel kernel = ctx.LoadKernel(path + "\\kernel.ptx", "ComplexMultCUDA");
            kernel.GridDimensions = (int)Math.Ceiling((double)(N + h.Count - 1) / 1024);
            kernel.BlockDimensions = 1024;

            int blocks = (int)Math.Ceiling((double)(x.Count + h.Count - 1) / (N + h.Count - 1));

            double[][] temp_y = new double[blocks][];
            double[] temp_h = new double[N + h.Count - 1];
            double[] temp_x = new double[N + h.Count - 1];


            h.ToArray().CopyTo(temp_h, 0);
            

            CudaDeviceVariable<double> d_x = null;
            CudaDeviceVariable<double2> d_X = new CudaDeviceVariable<double2>(N + h.Count - 1);

            CudaDeviceVariable<double> d_h = new CudaDeviceVariable<double>(N + h.Count - 1);
            CudaDeviceVariable<double2> d_H = new CudaDeviceVariable<double2>(N + h.Count - 1);

            //CudaDeviceVariable<double> d_y = new CudaDeviceVariable<double>(N + h.Count - 1);


            CudaFFTPlan1D planForward = new CudaFFTPlan1D(N + h.Count - 1, cufftType.D2Z, 1);
            CudaFFTPlan1D planInverse = new CudaFFTPlan1D(N + h.Count - 1, cufftType.Z2D, 1);

            try
            {
                d_h = temp_h;
                planForward.Exec(d_h.DevicePointer, d_H.DevicePointer, TransformDirection.Forward);
            }
            catch (Exception exp)
            {
                mainView.log(exp, "CUDA error: Impulse response FFT", this);
                return null;
            }

            for (int g = 0; g < blocks; g++)
            {

                int P = N;
                if (x.Count - N * g < N) P = x.Count - N * g;

                x.GetRange(N * g, P).ToArray().CopyTo(temp_x, 0);

                try
                {
                    d_x = temp_x;
                    planForward.Exec(d_x.DevicePointer, d_X.DevicePointer);
                    kernel.Run(d_H.DevicePointer, d_X.DevicePointer, N + h.Count - 1);
                    planInverse.Exec(d_X.DevicePointer, d_x.DevicePointer);
                }
                catch (Exception exp)
                {
                    mainView.log(exp, "Cuda error: kernel run cuda error", this);
                }
                
                temp_y[g] = d_x;
            }

            return OverlapAdd(temp_y, h.Count).GetRange(h.Count / 2, x.Count);
        }

        public List<float> hypotesis_long_save(List<double> xx, List<double> h, int N)
        {

            
            int n = (int)Math.Ceiling((double)(xx.Count()+0.000000000001)/N);

            double[] temp_data = new double[n*(N + h.Count - 1)-(n-1)*(h.Count - 1)];
            xx.CopyTo(temp_data, h.Count - 1);
            List<double> x=temp_data.ToList();
            //int N = 2000000;

            string path = Path.GetDirectoryName(mv.plugins[0].filename);
            CudaContext ctx = new CudaContext();
            CudaKernel kernel = ctx.LoadKernel(path + "\\kernel.ptx", "ComplexMultCUDA");
            kernel.GridDimensions = (int)Math.Ceiling((double)(N + h.Count - 1) / 1024);
            kernel.BlockDimensions = 1024;

            int blocks = (int)Math.Ceiling((double)(x.Count + h.Count - 1) / (N + h.Count - 1));

            double[][] temp_y = new double[n][];
            double[] temp_h = new double[N + h.Count - 1];
            double[] temp_x = new double[N + h.Count - 1];


            h.ToArray().CopyTo(temp_h, 0);


            CudaDeviceVariable<double> d_x = null;
            
            

            CudaDeviceVariable<double> d_h = new CudaDeviceVariable<double>(N + h.Count - 1);
            CudaDeviceVariable<double2> d_H = new CudaDeviceVariable<double2>(N + h.Count - 1);

            //CudaDeviceVariable<double> d_y = new CudaDeviceVariable<double>(N + h.Count - 1);


            CudaFFTPlan1D planForward = new CudaFFTPlan1D(N + h.Count - 1, cufftType.D2Z, 1);
            CudaFFTPlan1D planInverse = new CudaFFTPlan1D(N + h.Count - 1, cufftType.Z2D, 1);

            try
            {
                d_h = temp_h;
                planForward.Exec(d_h.DevicePointer, d_H.DevicePointer, TransformDirection.Forward);
            }
            catch (Exception exp)
            {
                mainView.log(exp, "CUDA error: Impulse response FFT", this);
                return null;
            }


            for (int g = 0; g < n; g++)
            {
                CudaDeviceVariable<double2> d_X = new CudaDeviceVariable<double2>(N + h.Count - 1);
                int P = N + h.Count - 1;
                //if (x.Count - P * g < P) P = x.Count - P * g;
                int L = h.Count - 1;
                if (g == 0) L = 0;

                x.CopyTo(P * g - L * g, temp_x, 0, P);

                try
                {                   
                    d_x = temp_x;
                    planForward.Exec(d_x.DevicePointer, d_X.DevicePointer);
                    kernel.Run(d_H.DevicePointer, d_X.DevicePointer, N + h.Count - 1);
                    planInverse.Exec(d_X.DevicePointer, d_x.DevicePointer);
                }
                catch (Exception exp)
                {
                    mainView.log(exp, "Cuda error: kernel run cuda error", this);
                }
                
                temp_y[g] = d_x;
                d_x.Dispose();
                d_X.Dispose();
            }
            planForward.Dispose();
            planInverse.Dispose();
            d_x.Dispose();
            
            d_h.Dispose();
            d_H.Dispose();
            ctx.Dispose();

            return OverlapSave(temp_y, h.Count,N + h.Count - 1).GetRange(h.Count / 2, xx.Count);
            
        }



        public List<float> OverlapSave(double[][] a, int FIRorder,int N)
        {
            double[] output =new double[a.Count() * a[0].Count()];
            System.Threading.Tasks.Parallel.For(0, a.Count(), i => {
                //for (int i = 0; i < a.Count(); i++)
                //{
                    Array.Copy(a[i], FIRorder-1,output,i*(N - (FIRorder - 1)),N-(FIRorder - 1));
                //}
            });
            return Array.ConvertAll<double,float>(output, d => (float)d).ToList();
        }


        public List<float> OverlapAdd(double2[][] a,int FIRorder)
        {
            List<float> output = new List<float>(new float[a.Count()*a[0].Count()]);//- (FIRorder-1)*(a.Count()-1)
            List<float> temp =new List<float>();

            int q = 0;
            for(int i=0;i< a.Count(); i++)
            {
                temp = a[i].Select(data => (float)data.x).ToList();
                for (int j = 0; j < a[0].Count(); j++)
                {
                    output[q] += temp[j];
                    q += 1;
                    //if (q == output.Count()) break;
                }
                q -= (FIRorder-1);

                this.Invoke((MethodInvoker)delegate
                {
                    progressBar1.Value = (int)(50 / a.Count()) * i+50;

                });
            }
            //this.Invoke((MethodInvoker)delegate
            //{
            //    progressBar1.Value = 0;

            //});
            return output;
        }

        public List<float> OverlapAdd(double[][] a, int FIRorder)
        {
            List<float> output = new List<float>(new float[a.Count() * a[0].Count()]);//- (FIRorder-1)*(a.Count()-1)
            List<double> temp = new List<double>();

            int q = 0;
            for (int i = 0; i < a.Count(); i++)
            {
                temp = a[i].ToList();
                for (int j = 0; j < a[0].Count(); j++)
                {
                    output[q] += (float)temp[j];
                    q += 1;
                    //if (q == output.Count()) break;
                }
                q -= (FIRorder - 1);

                //this.Invoke((MethodInvoker)delegate
                //{
                //    progressBar1.Value = (int)(50 / a.Count()) * i + 50;

                //});
            }
            //this.Invoke((MethodInvoker)delegate
            //{
            //    progressBar1.Value = 0;

            //});
            return output;
        }

        //Test CUDA kernel for complex multiplication
        public void test(int N)
        {
            CudaContext ctx = new CudaContext();
            CudaKernel kernel = ctx.LoadKernel("kernel.ptx", "ComplexMultCUDA");
            kernel.GridDimensions = N;
            kernel.BlockDimensions = 1;
            double2[] a = new double2[N];
            double2[] b = new double2[N];
            double2[] c = new double2[N];
            for (int i = 0; i < N; i++)
            {
                a[i].x = 1;
                a[i].y = 3;
                b[i].x = 2;
                b[i].y = 2;
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
            kernel.Run(d_a.DevicePointer, d_b.DevicePointer, N);
            c = d_b;
            Console.WriteLine("C.last()={0}+i{1}", c.Last().x, c.Last().y);

        }

        public void test2(int N)
        {
            CudaContext ctx = new CudaContext();
            CudaKernel kernel = ctx.LoadKernel("kernel.ptx", "ComplexMultCUDA");
            int dim2 = 2;
            kernel.GridDimensions = N;
            kernel.BlockDimensions = 1;
            double2[][] a = new double2[2][];
            double2[] b = new double2[N];
            double2[][] c = new double2[2][];

            a[0] = new double2[N];
            a[1] = new double2[N];
            c[0] = new double2[N];
            for (int i = 0; i < N; i++)
            {
                a[0][i].x = 1;
                a[0][i].y = 3;
                b[i].x = 2;
                b[i].y = 2;
            }

            CudaDeviceVariable<double2> d_a = null;
            CudaDeviceVariable<double2> d_b = null;

            try
            {
                d_a = a[0];
                d_b = b;
            }
            catch (Exception e)
            {
                Console.WriteLine("{0} Exception caught.", e);
                return;
            }
            kernel.Run(d_a.DevicePointer, d_b.DevicePointer, N);
            c[0] = d_b;


        }

        private void radioButton1_CheckedChanged(object sender, EventArgs e)
        {
            FirType = 1;
            numOrder.Enabled = true;
            numFreq1.Enabled = true;
            numFreq2.Enabled = false;
        }

        private void label8_Click(object sender, EventArgs e)
        {

        }

        private void label15_Click(object sender, EventArgs e)
        {

        }

        private void textBox3_TextChanged(object sender, EventArgs e)
        {

        }

        private void numFreq2_ValueChanged(object sender, EventArgs e)
        {
            refrControls();
        }

        private void radioButton2_CheckedChanged(object sender, EventArgs e)
        {
            FirType = 2;
            numOrder.Enabled = true;
            numFreq1.Enabled = true;
            numFreq2.Enabled = false;
        }

        private void radioButton3_CheckedChanged(object sender, EventArgs e)
        {
            FirType = 3;
            numOrder.Enabled = true;
            numFreq1.Enabled = true;
            numFreq2.Enabled = true;
        }

        private void radioButton4_CheckedChanged(object sender, EventArgs e)
        {
            FirType = 4;
            numOrder.Enabled = true;
            numFreq1.Enabled = true;
            numFreq2.Enabled = true;
        }

        private void label8_Click_1(object sender, EventArgs e)
        {

        }

        private void label13_Click(object sender, EventArgs e)
        {

        }

        private void button3_Click(object sender, EventArgs e)
        {
            
        }

        private void button2_Click(object sender, EventArgs e)
        {
            List<string> lines = new List<string>();

            filter_FIR result = getFirFilter((int)numOrder.Value, (double)numFreq1.Value, (double)numFreq2.Value, FirType, wf);



            if (result == null)
                return;

            lines.Add("SignalPlant FIR Filter design");
            lines.Add("");
            lines.Add("Order: " + (int)numOrder.Value);
            lines.Add("Frequency 1: " + (double)numFreq1.Value + " Hz");
            if (FirType >= 3)
                lines.Add("Frequency 2: " + (double)numFreq2.Value + " Hz");

            lines.Add("Windowing function: " + WindowingFunction.windowNames[wf.winType]);
            if (WindowingFunction.numParams[wf.winType] > 0)
                lines.Add("Windowing function parameter: " + wf.winParam);

            lines.Add("");

            lines.Add("-------Coeffitients-----------------");
            lines.Add("No. \t Value");
            lines.Add("------------------------------------");

            for (int i = 0; i < result.coeffs.Count; i++)
            {
                lines.Add(i + "\t\t" + result.coeffs[i].ToString());
            }


            string wholeTExt = "";

            foreach (string prt in lines)
                wholeTExt = wholeTExt + "\r\n" + prt;

            System.Windows.Forms.Clipboard.SetText(wholeTExt);
        }

        private void button1_Click(object sender, EventArgs e)
        {
            signalViewer.windowFunc wfd = new signalViewer.windowFunc();




            wfd.wf.winType = wf.winType;
            wfd.wf.winParam = wf.winParam;



            wfd.Location = new Point(button1.Left + this.ParentForm.Location.X, button1.Top + this.ParentForm.Location.Y);

            if (wfd.ShowDialog() == DialogResult.OK)
            {
                string str = signalViewer.WindowingFunction.windowNames[wfd.wf.winType];

                /*
                if (signalViewer.WindowingFunction.numParams[wfd.wf.winType] > 0)
                {
                    str += " (" + wfd.wf.winParam.ToString() + ")";
                }

                tbWindow.Text = str;
                */
                wf.winType = wfd.wf.winType;
                wf.winParam = wfd.wf.winParam;
            }

            refrControls();
        }

        private void numOrder_ValueChanged(object sender, EventArgs e)
        {
            refrControls();
        }

        private void numFreq1_ValueChanged(object sender, EventArgs e)
        {
            refrControls();
        }


        class FIR2T
        {
            //direct form II transposed implementation
            public float[] doFIR2Tx(double[] x, double[] b)
            {
                int N = b.Length;
                double[] d = new double[N - 1];
                float[] y = new float[x.Length];
                for (int j = 0; j < x.Length; j++)
                {
                    y[j] = (float)(b[0] * x[j] + d[0]);
                    for (int i = 0; i < N - 1 - 1; i++)
                    {
                        d[i] = b[i + 1] * x[j] + d[i + 1];
                    }
                    d[N - 1 - 1] = b[N - 1] * x[j];
                }
                return y;
            }

            //SignalPlant implementation
            public float[] doFIR(double[] input, double[] coeffs)
            {
                float[] output = new float[input.Length];

                int M = coeffs.Count();
                int n = input.Length;

                for (int j = 0; j < n; j++)
                {
                    double t = 0.0;

                    for (int i = 0; i < M; i++)
                    {
                        int cI = i - M / 2;
                        int sampleIndex = j - cI;

                        if (sampleIndex >= 0 && sampleIndex < input.Length)
                            t += coeffs[i] * input[sampleIndex];
                    }
                    output[j] = (float)t;

                }

                return output;
            }

        }

        private void progressBar1_Click(object sender, EventArgs e)
        {

        }
    }
}
