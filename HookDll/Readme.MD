
# Compiling the GDIA DLL file
## Prerequisites:
## Visual Studio
Either of:
* Install Windows Studio Professional Edition 
* Visual Studio Community Edition with VC++ support & The windows SDK

For the Community+SDK you may need to retarget the solution to the SDK. (rightclick it inside VS and select retarget solution)

## Boost 1.61.0
C++ Boost v1.61.0 is required, along with the 32bit and 64bit library files for your version of Visual Studio.


1. Compile the detours project in release mode
2. Compile the Custom (dll) project


## Compiling detours 64 bit
VSPATH...\VC\bin\amd64\vcvars64.bat
nmake clean
nmake -D DETOURS_TARGET_PROCESSOR=X64

## Compiling detours 32 bit
VSPATH...\Common7\Tools\vsvars32.bat
nmake clean
nmake

## Debugging the DLL
Debugging the DLL is a pain, but %appdata%\..\local\temp\iagd_hook.log contains the logs from the DLL itself.