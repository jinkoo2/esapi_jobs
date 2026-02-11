@set MSBUILD=C:\Windows\Microsoft.NET\Framework\v4.0.30319\MSBuild.exe

@if not exist %MSBUILD% goto :error

%MSBUILD% Projects\DataMining\DataMining.csproj /p:Platform=AnyCPU /verbosity:quiet
%MSBUILD% Projects\DvhLookups\DvhLookups.csproj /p:Platform=AnyCPU /verbosity:quiet
%MSBUILD% Projects\PlanarDoseIn3D\PlanarDoseIn3D.csproj /p:Platform=AnyCPU /verbosity:quiet

@goto :exit

:error
@echo Cannot find %MSBUILD%
:exit
@Pause Done! 
