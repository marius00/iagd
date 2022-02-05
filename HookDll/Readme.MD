
# Compiling the GDIA DLL file
## Prerequisites:
## Visual Studio
Either of:
* Install Windows Studio Professional Edition 
* Visual Studio Community Edition with VC++ support & The windows SDK

For the Community+SDK you may need to retarget the solution to the SDK. (rightclick it inside VS and select retarget solution)

## Boost
[C++ Boost](https://sourceforge.net/projects/boost/files/boost-binaries/1.78.0/) for MSVC 14.0-x64 is required, along with the 64bit library files for your version of Visual Studio.

1. Compile the detours project in release mode
2. Compile the Custom (dll) project


## Compiling detours 64 bit
VSPATH...\VC\bin\amd64\vcvars64.bat
nmake clean
nmake -D DETOURS_TARGET_PROCESSOR=X64


## Debugging the DLL
The DLL logs to %appdata%\..\local\evilsoft\iagd\iagd_hook.log