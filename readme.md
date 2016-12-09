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
  - Download all attached files from [release](https://github.com/xnejed07/CudaFilters/releases) folder in this repository and copy files from ..\bin\ to SignalPlant plugins folder


**Usage**
  - In SignalPlant run plugins from menu *Plugins/CUDA/...*

**License**
- MIT

**Preview**

![preview](https://github.com/xnejed07/CudaFilters/blob/master/preview.jpg)
