Install Windows Studio Professional Edition (Or Visual Studio Community Edition with VC++ support + The windows SDK)

For the Community+SDK you may need to retarget the solution to the SDK. (rightclick it inside VS and select retarget solution)

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

