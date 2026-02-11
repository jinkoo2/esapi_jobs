# Example RBE Report - ESAPI Plugin

An Eclipse Scripting API (ESAPI) plugin for Varian Eclipse Treatment Planning System that generates Radiobiological Effect (RBE) reports from plan setups or plan sums. This plugin calculates Equivalent Dose in 2 Gy fractions (EQD2) and generates comprehensive PDF reports.

## Overview

This ESAPI plugin provides a graphical user interface for analyzing and reporting radiobiological effects of radiation therapy plans. It supports both individual plan setups and plan sums, allowing users to:

- Calculate EQD2 doses using the Linear-Quadratic (LQ) model
- Adjust target α/β ratios interactively
- View plan summaries and detailed information
- Export comprehensive PDF reports

## Features

- **Multi-Plan Support**: Works with individual plan setups or plan sums
- **Interactive α/β Adjustment**: Edit target α/β ratios in real-time and see updated EQD2 calculations
- **Comprehensive Reporting**: Generates detailed PDF reports including:
  - Patient information
  - Plan summary tables
  - Detailed plan information
  - Total planned EQD2 dose
- **Plan Type Detection**: Automatically identifies plan types (External Beam, Brachytherapy) and techniques (VMAT, IMRT, Static, Arc, HDR)
- **Real-time Calculations**: Updates EQD2 doses automatically when α/β values are modified

## Requirements

### Software Requirements

- **Varian Eclipse Treatment Planning System** (ESAPI v11/v13 compatible)
- **.NET Framework 4.6.1** or later
- **Visual Studio** (for building) or **MSBuild** (standalone)
- **Windows** (x64 platform)

### Dependencies

The following libraries are included with the project:

- **MigraDoc.DocumentObjectModel-WPF.dll** - PDF document generation
- **MigraDoc.Rendering-WPF.dll** - PDF rendering
- **PdfSharp-WPF.dll** - PDF creation library
- **PdfSharp.Charting-WPF.dll** - Charting support

### ESAPI References

- `VMS.TPS.Common.Model.API.dll` - ESAPI core library
- `VMS.TPS.Common.Model.Types.dll` - ESAPI type definitions

## Building the Project

### Using the Build Script (Recommended)

1. Open a command prompt or PowerShell in the project directory
2. Run the build script:
   ```batch
   build.bat
   ```
   
   Or specify configuration and platform:
   ```batch
   build.bat Release x64
   ```

The compiled DLL will be placed in `..\..\plugins\RBEReport.esapi.dll`

### Using Visual Studio

1. Open `Example_RBEReport.csproj` in Visual Studio
2. Set the configuration to **Debug** or **Release**
3. Set the platform to **x64**
4. Build the solution (Build → Build Solution)

### Using MSBuild Directly

```batch
msbuild Example_RBEReport.csproj /p:Configuration=Debug /p:Platform=x64
```

## Installation

1. Build the project using one of the methods above
2. Ensure the output DLL (`RBEReport.esapi.dll`) is in the ESAPI plugins directory:
   ```
   <ESAPI_Installation>\plugins\RBEReport.esapi.dll
   ```
   
   The default output path is `..\..\plugins\` relative to the project directory.

3. Copy the required dependency DLLs to the plugins directory (if not already present):
   - `MigraDoc.DocumentObjectModel-WPF.dll`
   - `MigraDoc.Rendering-WPF.dll`
   - `PdfSharp-WPF.dll`
   - `PdfSharp.Charting-WPF.dll`

4. Restart Eclipse if it's already running

## Usage

### Running the Plugin

1. Open a patient in Eclipse
2. Open a plan setup or plan sum
3. Navigate to **Scripts** menu in Eclipse
4. Select **RBEReport** (or the name you assigned to the script)

### Using the Interface

1. **View Plan Information**: The interface displays all plans in scope with their calculated EQD2 doses
2. **Adjust α/β Ratios**: 
   - Click on the "Target a/b" cell in the table
   - Enter a new α/β value
   - The EQD2 dose will automatically recalculate
3. **View Details**: Select a plan from the list to see detailed information
4. **Export PDF**: Use the export button to generate a PDF report

### Supported Plan Types

- **External Beam Plans**: 
  - Static fields
  - IMRT (Intensity Modulated Radiation Therapy)
  - VMAT (Volumetric Modulated Arc Therapy)
  - Arc therapy
- **Brachytherapy Plans**:
  - HDR (High Dose Rate) only
  - PDR (Pulsed Dose Rate) is not supported

### Limitations

- Only one plan sum can be open at a time
- PDR brachytherapy plans are not supported
- The script requires an active plan or plan sum to run

## Technical Details

### EQD2 Calculation

The plugin uses the Linear-Quadratic (LQ) model to calculate Equivalent Dose in 2 Gy fractions:

```
BED = n × d × (1 + d / (α/β))
EQD2 = BED / (1 + 2 / (α/β))
```

Where:
- `n` = number of fractions
- `d` = dose per fraction (in Gy)
- `α/β` = target tissue α/β ratio (default: 10 Gy)
- `BED` = Biologically Effective Dose
- `EQD2` = Equivalent Dose in 2 Gy fractions

### Project Structure

```
Example_RBEReport/
├── RBEReport.cs              # Main script entry point
├── RBEViewModel.cs            # ViewModel with business logic and PDF export
├── UserControl1.xaml          # WPF user interface definition
├── UserControl1.xaml.cs       # Code-behind for UI
├── Example_RBEReport.csproj   # Project file
├── build.bat                  # Build script
├── run.bat                    # Build and information script
├── MigraDoc_License.txt       # MigraDoc license
├── PDFsharp_License.txt       # PDFsharp license
└── README.md                  # This file
```

## License

Copyright (c) 2014 Varian Medical Systems, Inc.

Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated documentation files (the "Software"), to deal in the Software without restriction, including without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons to whom the Software is furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.

## Important Notes

⚠️ **This is an example script, not for clinical use!**

Please verify the accuracy and correctness of the script before use in a clinical environment. The script includes a disclaimer in the generated PDF reports.

## References

- See the article "Radiobiological Effect report and Aria Document post" for additional details
- ESAPI Documentation: Varian Eclipse Scripting API Reference Guide
- Linear-Quadratic Model: Standard radiobiological model for dose fractionation effects

## Support

For issues or questions related to:
- **ESAPI**: Contact Varian support
- **This Plugin**: Review the source code and modify as needed for your requirements

## Version History

- **Initial Release**: Example script for ESAPI v11/v13
- Supports both individual plans and plan sums
- PDF export functionality with MigraDoc/PdfSharp

