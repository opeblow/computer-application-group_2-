Option Strict On
Option Explicit On

''' <summary>
''' Exports a formatted plain-text traverse report to a file.
''' </summary>
Public Class ReportExporter

    ''' <summary>
    ''' Builds a formatted plain-text report string containing a
    ''' station-by-station table of results and summary statistics,
    ''' then writes it to the specified file path.
    ''' </summary>
    Public Shared Sub ExportReport(stations As List(Of TraverseStation),
                                   dnError As Double,
                                   deError As Double,
                                   linearMisclosure As Double,
                                   accuracyRatio As Double,
                                   area As Double,
                                   filePath As String)
        Dim sb As New System.Text.StringBuilder()

        sb.AppendLine("TRAVERSE COMPUTATION REPORT")
        sb.AppendLine(New String("="c, 80))
        sb.AppendLine()

        sb.AppendLine("STATION RESULTS")
        sb.AppendLine(New String("-"c, 80))
        Dim header = $"{"Station",-10} {"Fwd Bearing",-14} {"Back Bearing",-14} {"DN",-12} {"DE",-12} {"Corr DN",-12} {"Corr DE",-12} {"Northing",-12} {"Easting",-12}"
        sb.AppendLine(header)
        sb.AppendLine(New String("-"c, 80))

        For Each st In stations
            Dim line = $"{st.Name,-10} {st.ForwardBearing,10:F4}   {st.BackBearing,10:F4}   {st.DN,10:F4}  {st.DE,10:F4}  {st.CorrectedDN,10:F4}  {st.CorrectedDE,10:F4}  {st.Northing,10:F4}  {st.Easting,10:F4}"
            sb.AppendLine(line)
        Next

        sb.AppendLine()
        sb.AppendLine("SUMMARY")
        sb.AppendLine(New String("-"c, 80))
        sb.AppendLine($"Total DN Misclosure:    {dnError,12:F6} m")
        sb.AppendLine($"Total DE Misclosure:    {deError,12:F6} m")
        sb.AppendLine($"Linear Misclosure:     {linearMisclosure,12:F6} m")
        Dim ratioInv As Double = 0.0
        If accuracyRatio > 0.0 Then
            ratioInv = 1.0 / accuracyRatio
        End If
        sb.AppendLine($"Accuracy Ratio:        1 : {ratioInv,12:F0}")
        sb.AppendLine($"Area:                  {area,12:F4} sq. m")
        sb.AppendLine(New String("="c, 80))

        System.IO.File.WriteAllText(filePath, sb.ToString(), System.Text.Encoding.UTF8)
    End Sub

End Class
