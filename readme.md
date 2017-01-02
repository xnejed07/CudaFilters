# CudaFilters

CudaFilters is library for the SignalPlant software. The purpose of this library is GPU hardware acceleration of FIR and FFT filtering.
Plugins are released as dynamic link library (.dll) that must be copied to the SignalPlant plugins folder.
Moreover, source codes for FIR and FFT plugins are also released.

**Software and hardware requirements**
- Nvidia CUDA drivers 8.0
- Nvidia GPU with compute capability higher than 3.0
- [SignalPlant](https://signalplant.codeplex.com/) version 1.2.2.6 or higher 
- [ManagedCuda](https://github.com/kunzmi/managedCuda) version 8


**Instalation**
  - Check if your GPU is supported. 
  - Install Nvidia CUDA drivers 8.0 from [CUDA Drivers](https://developer.nvidia.com/cuda-downloads).  
  - Download zip file from [release](https://github.com/xnejed07/CudaFilters/releases) folder in this repository and copy all files from ..\bin\ to SignalPlant plugins folder


**Usage**
  - In SignalPlant run plugins from menu *Plugins/CUDA/...*

**License**
- MIT

**Preview**

![preview](https://github.com/xnejed07/CudaFilters/blob/master/preview.jpg)

# Help
Before you start using this library, please install CUDA drivers from link given above. Once you open FIR or FFT filtering plugin check compatibility of your Nvidia GPU card, compute capability shoud be higher or equal to 3.0. In the right corder of plugin window, you will be given informations about your GPU, i.e: device name, memory, frequency, compute capability and drivers info. 

**FFT Filter**
FFT filter allows computation of bandstop and bandpass filtering in frequency domain. Moreover, amplitude envelope might be estimated by Hilbert transform. Usage of this plugins is the same as standard SignalPlant version. Firstly, attach a channel to the plugin by clicking on the loading button. Secondly,  select filter type and frequency range. Interactive live preview shows results of filtering, when you are satisfied with settings use "Process" button to run filtering on all attached channels. 
![navod1.jpg](https://github.com/xnejed07/CudaFilters/blob/master/navod1.jpg)


**FIR Filter**
FIR filtering allows computation of lowpass,highpass,bandstop and bandpass filtering. Moreover, several types of windowing function might be used.
Firstly, attach a channel to the plugin by clicking on the loading button. Secondly,  select filter type and frequency range. After that, use "Process" button to run filtering.
![navod2.jpg](https://github.com/xnejed07/CudaFilters/blob/master/navod2.jpg)