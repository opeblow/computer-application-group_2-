# SVY 323 Traverse Computation Program

VB.NET WinForms desktop application computing closed traverse bearings, departures/latitudes, Bowditch correction, final coordinates, linear misclosure, accuracy ratio, and polygon area.

## Academic Context

This project was built as a group assignment for SVY 323 (Surveying), implementing a closed traverse computation program covering bearings, departures/latitudes, Bowditch correction, misclosure, accuracy ratio, and area calculation.

## Requirements

- .NET SDK 10.0 or later

## How to Build

```
dotnet build
```

## How to Run

```
dotnet run
```

## Features

- Editable Control Points and Station Data inputs
- Compute / Export / Clear All / Add Row / Save as Image buttons
- Angle-sum validation warning (red text when sum deviates from (n-2)*180)
- .txt report export with station-by-station table and summary statistics
- Full-form PNG/JPEG export that captures all rows even beyond visible screen area

## Project Structure

| File | Description |
|------|-------------|
| `Program.vb` | Application entry point; launches the main form |
| `Form1.vb` | Main form logic: input validation, computation orchestration, results population, image capture, and event handlers |
| `Form1.Designer.vb` | Designer-generated control declarations and layout |
| `TraverseStation.vb` | Data model for a single station: observed angle/distance and all computed fields |
| `TraverseCalculator.vb` | Static computation engine: bearing propagation, departure/latitude, misclosure, Bowditch adjustment, coordinate calculation, and shoelace area |
| `ReportExporter.vb` | Plain-text report file export |

## Sample Dataset

The program loads the following dataset on startup:

| Station | Included Angle (deg) | Distance (m) |
|---------|---------------------|--------------|
| BM1 | 142.3500 | 85.420 |
| ST1 | 128.1200 | 102.650 |
| ST2 | 115.8300 | 76.300 |
| ST3 | 96.4700 | 93.180 |
| ST4 | 122.9000 | 121.050 |
| BM2 | (final control point) | |

Initial back bearing: 60.0 degrees

BM1 Northing/Easting: 5000.00 / 5000.00
BM2 Northing/Easting: 5000.00 / 5000.00

Expected linear misclosure is approximately 0.49 m, confirming correct computation of the closed-link traverse.

## Contributing

Please see [CONTRIBUTING.md](CONTRIBUTING.md) for guidelines on how to fork, build, test, and submit pull requests.

## Author

Mobolaji Opeyemi Bolatito -- opeblow2021@gmail.com

## License

This project is licensed under the MIT License. See [LICENSE](LICENSE) for details.
