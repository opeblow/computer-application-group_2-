Option Strict On
Option Explicit On

Public Class Form1

    Private stations As List(Of TraverseStation)
    Private dnError As Double
    Private deError As Double
    Private linearMisclosure As Double
    Private accuracyRatio As Double
    Private area As Double

    Public Sub New()
        InitializeComponent()
    End Sub

    Private Sub Form1_Shown(sender As Object, e As EventArgs) Handles MyBase.Shown
        btnCompute.PerformClick()
        Dim diagPath = System.IO.Path.Combine(System.IO.Path.GetTempPath(), "svy_diag.txt")
        Dim lastSt = stations(stations.Count - 1)
        System.IO.File.WriteAllText(diagPath,
            "dnError field: " & dnError.ToString("F6") & vbCrLf &
            "deError field: " & deError.ToString("F6") & vbCrLf &
            "linearMisclosure: " & linearMisclosure.ToString("F6") & vbCrLf &
            "accuracyRatio: " & accuracyRatio.ToString("F10") & vbCrLf &
            "lblDnMisclosure.Text: [" & lblDnMisclosure.Text & "]" & vbCrLf &
            "lblDeMisclosure.Text: [" & lblDeMisclosure.Text & "]" & vbCrLf &
            "lblLinearMisclosure.Text: [" & lblLinearMisclosure.Text & "]" & vbCrLf &
            "lblAccuracy.Text: [" & lblAccuracy.Text & "]" & vbCrLf &
            "Last station name: " & lastSt.Name & vbCrLf &
            "Last station Northing: " & lastSt.Northing.ToString("F4") & vbCrLf &
            "Last station Easting: " & lastSt.Easting.ToString("F4"))
    End Sub

    Private Sub btnCompute_Click(sender As Object, e As EventArgs) Handles btnCompute.Click
        ' Validate control point inputs
        Dim initName As String = txtInitName.Text.Trim()
        Dim initNorthing As Double
        Dim initEasting As Double
        Dim initBearing As Double
        Dim finalName As String = txtFinalName.Text.Trim()
        Dim finalNorthing As Double
        Dim finalEasting As Double

        If String.IsNullOrEmpty(initName) Then
            MessageBox.Show("Initial station name cannot be empty.", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning)
            Return
        End If
        If String.IsNullOrEmpty(finalName) Then
            MessageBox.Show("Final station name cannot be empty.", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning)
            Return
        End If
        If Not Double.TryParse(txtInitNorthing.Text, initNorthing) Then
            MessageBox.Show("Invalid initial northing.", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning)
            Return
        End If
        If Not Double.TryParse(txtInitEasting.Text, initEasting) Then
            MessageBox.Show("Invalid initial easting.", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning)
            Return
        End If
        If Not Double.TryParse(txtInitBearing.Text, initBearing) OrElse initBearing < 0.0 OrElse initBearing >= 360.0 Then
            MessageBox.Show("Initial back bearing must be a number between 0 and 360.", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning)
            Return
        End If
        If Not Double.TryParse(txtFinalNorthing.Text, finalNorthing) Then
            MessageBox.Show("Invalid final northing.", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning)
            Return
        End If
        If Not Double.TryParse(txtFinalEasting.Text, finalEasting) Then
            MessageBox.Show("Invalid final easting.", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning)
            Return
        End If

        ' Read station data from dgvInput
        stations = New List(Of TraverseStation)()
        Dim totalDistance As Double = 0.0
        Dim angleSum As Double = 0.0

        For Each row As DataGridViewRow In dgvInput.Rows
            If row.IsNewRow Then Continue For

            Dim nameVal As Object = row.Cells(0).Value
            Dim angleVal As Object = row.Cells(1).Value
            Dim distVal As Object = row.Cells(2).Value

            If nameVal Is Nothing OrElse String.IsNullOrEmpty(nameVal.ToString().Trim()) Then
                MessageBox.Show("Station name cannot be empty.", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning)
                Return
            End If

            Dim angle As Double
            If Not Double.TryParse(angleVal?.ToString(), angle) OrElse angle < 0.0 OrElse angle > 360.0 Then
                MessageBox.Show("Included angle must be a number between 0 and 360 for station " & nameVal.ToString() & ".", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning)
                Return
            End If

            Dim dist As Double
            If Not Double.TryParse(distVal?.ToString(), dist) OrElse dist <= 0.0 Then
                MessageBox.Show("Distance must be a positive number for station " & nameVal.ToString() & ".", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning)
                Return
            End If

            Dim st As New TraverseStation()
            st.Name = nameVal.ToString().Trim()
            st.IncludedAngle = angle
            st.Distance = dist
            stations.Add(st)
            totalDistance += dist
            angleSum += angle
        Next

        If stations.Count < 3 Then
            MessageBox.Show("At least 3 stations are required to form a closed polygon.", "Invalid Input", MessageBoxButtons.OK, MessageBoxIcon.Warning)
            Return
        End If

        Dim expectedSum As Double = (stations.Count - 2) * 180.0
        lblAngleSum.Text = "Sum of Included Angles: " & angleSum.ToString("F4") & " degrees"
        If Math.Abs(angleSum - expectedSum) > 0.01 Then
            lblAngleSum.ForeColor = Color.Red
        Else
            lblAngleSum.ForeColor = Color.Black
        End If

        ' Perform calculations
        TraverseCalculator.ComputeForwardBearings(stations, initBearing)
        TraverseCalculator.ComputeDepartureAndLatitude(stations)

        Dim misclosure = TraverseCalculator.ComputeMisclosure(stations, initNorthing, initEasting, finalNorthing, finalEasting)
        dnError = misclosure.dnError
        deError = misclosure.deError

        TraverseCalculator.ApplyBowditchCorrection(stations, dnError, deError)
        TraverseCalculator.ComputeFinalCoordinates(stations, initNorthing, initEasting)
        area = TraverseCalculator.ComputeAreaByCoordinates(stations)

        Dim accuracy = TraverseCalculator.ComputeLinearAccuracy(dnError, deError, totalDistance)
        linearMisclosure = accuracy.linearMisclosure
        accuracyRatio = accuracy.accuracyRatio

        ' Populate results grid
        PopulateResults()

        ' DIAGNOSTIC: Write all station data to file
        Dim diagPath As String = System.IO.Path.Combine(System.IO.Path.GetTempPath(), "svy_traverse_diagnostic.txt")
        Dim lines As New List(Of String)
        lines.Add("Station | IncludedAngle | Distance | ForwardBearing | BackBearing | DN | DE | CorrectedDN | CorrectedDE | Northing | Easting")
        For Each st In stations
            lines.Add(st.Name & " | " &
                st.IncludedAngle.ToString("F6") & " | " &
                st.Distance.ToString("F6") & " | " &
                st.ForwardBearing.ToString("F6") & " | " &
                st.BackBearing.ToString("F6") & " | " &
                st.DN.ToString("F6") & " | " &
                st.DE.ToString("F6") & " | " &
                st.CorrectedDN.ToString("F6") & " | " &
                st.CorrectedDE.ToString("F6") & " | " &
                st.Northing.ToString("F6") & " | " &
                st.Easting.ToString("F6"))
        Next
        lines.Add("")
        lines.Add("=== dgvInput order (top to bottom) ===")
        For Each row As DataGridViewRow In dgvInput.Rows
            If row.IsNewRow Then Continue For
            Dim nameVal As Object = row.Cells(0).Value
            If nameVal IsNot Nothing Then
                lines.Add(nameVal.ToString())
            End If
        Next
        lines.Add("")
        lines.Add("=== dgvResults order (top to bottom) ===")
        For Each row As DataGridViewRow In dgvResults.Rows
            Dim nameVal As Object = row.Cells(0).Value
            If nameVal IsNot Nothing Then
                lines.Add(nameVal.ToString())
            End If
        Next
        System.IO.File.WriteAllLines(diagPath, lines)

        ' Update summary labels
        lblDnMisclosure.Text = "Total DN Misclosure: " & dnError.ToString("F6") & " m"
        lblDeMisclosure.Text = "Total DE Misclosure: " & deError.ToString("F6") & " m"
        lblLinearMisclosure.Text = "Linear Misclosure: " & linearMisclosure.ToString("F6") & " m"
        Dim ratioInv As Double = 0.0
        If accuracyRatio > 0.0 Then
            ratioInv = 1.0 / accuracyRatio
        End If
        lblAccuracy.Text = "Accuracy Ratio: 1 : " & ratioInv.ToString("F0")
        lblArea.Text = "Area: " & area.ToString("F4") & " sq. m"
    End Sub

    Private Sub PopulateResults()
        dgvResults.Rows.Clear()
        For Each st In stations
            dgvResults.Rows.Add(
                st.Name,
                st.ForwardBearing.ToString("F4"),
                st.BackBearing.ToString("F4"),
                st.DN.ToString("F4"),
                st.DE.ToString("F4"),
                st.CorrectedDN.ToString("F4"),
                st.CorrectedDE.ToString("F4"),
                st.Northing.ToString("F4"),
                st.Easting.ToString("F4"))
        Next
    End Sub

    Private Sub btnAddRow_Click(sender As Object, e As EventArgs) Handles btnAddRow.Click
        dgvInput.Rows.Add()
    End Sub

    Private Sub btnClearAll_Click(sender As Object, e As EventArgs) Handles btnClearAll.Click
        txtInitName.Text = ""
        txtInitNorthing.Text = ""
        txtInitEasting.Text = ""
        txtInitBearing.Text = ""
        txtFinalName.Text = ""
        txtFinalNorthing.Text = ""
        txtFinalEasting.Text = ""
        dgvInput.Rows.Clear()
        dgvInput.Rows.Add()
        dgvResults.Rows.Clear()
        lblDnMisclosure.Text = "Total DN Misclosure:"
        lblDeMisclosure.Text = "Total DE Misclosure:"
        lblLinearMisclosure.Text = "Linear Misclosure:"
        lblAccuracy.Text = "Accuracy Ratio:"
        lblArea.Text = "Area:"
        lblAngleSum.Text = "Sum of Included Angles:"
    End Sub

    Private Sub btnExport_Click(sender As Object, e As EventArgs) Handles btnExport.Click
        If stations Is Nothing OrElse stations.Count = 0 Then
            MessageBox.Show("Run the computation first before exporting.", "Export Error", MessageBoxButtons.OK, MessageBoxIcon.Warning)
            Return
        End If

        Using sfd As New SaveFileDialog()
            sfd.Filter = "Text Files (*.txt)|*.txt|All Files (*.*)|*.*"
            sfd.FileName = "TraverseReport.txt"
            sfd.Title = "Export Traverse Report"
            If sfd.ShowDialog() = DialogResult.OK Then
                ReportExporter.ExportReport(stations, dnError, deError, linearMisclosure, accuracyRatio, area, sfd.FileName)
                MessageBox.Show("Report exported to " & sfd.FileName, "Export Complete", MessageBoxButtons.OK, MessageBoxIcon.Information)
            End If
        End Using
    End Sub

    Private _lastCaptureSnapshot As LabelPositionData

    Private Sub CaptureToBitmap(filePath As String)
        If dgvResults.Rows.Count = 0 Then
            Return
        End If

        Dim origInputHeight As Integer = dgvInput.Height
        Dim origResultsHeight As Integer = dgvResults.Height
        Dim origStationDataHeight As Integer = grpStationData.Height
        Dim origResultsGroupHeight As Integer = grpResults.Height
        Dim origResultsTop As Integer = grpResults.Top
        Dim origFormHeight As Integer = Me.Height
        Dim origClientSize As Size = Me.ClientSize
        Dim btnTopOffsets As New Dictionary(Of Button, Integer)()
        For Each ctrl As Control In Me.Controls
            Dim btn As Button = TryCast(ctrl, Button)
            If btn IsNot Nothing Then
                btnTopOffsets(btn) = btn.Top
            End If
        Next

        Dim origLabelTops As New Dictionary(Of Label, Integer)()
        For Each ctrl As Control In grpResults.Controls
            Dim lbl As Label = TryCast(ctrl, Label)
            If lbl IsNot Nothing Then
                origLabelTops(lbl) = lbl.Top
            End If
        Next

        Try
            Dim oldStationBottom = grpStationData.Bottom

            Dim newInputHeight = dgvInput.ColumnHeadersHeight + (dgvInput.Rows.Count * dgvInput.RowTemplate.Height) + 4
            dgvInput.Height = newInputHeight
            grpStationData.Height = dgvInput.Top + newInputHeight + 10

            Dim shiftDown = grpStationData.Bottom - oldStationBottom

            For Each kvp In btnTopOffsets
                kvp.Key.Top = kvp.Value + shiftDown
            Next

            grpResults.Top = origResultsTop + shiftDown

            Dim newResultsHeight = dgvResults.ColumnHeadersHeight + (dgvResults.Rows.Count * dgvResults.RowTemplate.Height) + 4
            dgvResults.Height = newResultsHeight

            Dim labelsTop As Integer = dgvResults.Top + dgvResults.Height + 10
            Dim labelGap As Integer = 5
            Dim labelHeight As Integer = 22

            lblDnMisclosure.Top = labelsTop
            lblDeMisclosure.Top = labelsTop
            lblLinearMisclosure.Top = labelsTop

            lblAccuracy.Top = labelsTop + labelHeight + labelGap
            lblArea.Top = labelsTop + labelHeight + labelGap

            lblAngleSum.Top = labelsTop + (labelHeight + labelGap) * 2

            Dim bottomLabelTop = lblAngleSum.Top
            Dim bottomLabelBottom = bottomLabelTop + lblAngleSum.Height
            grpResults.Height = bottomLabelBottom + 10

            _lastCaptureSnapshot = New LabelPositionData With {
                .dgvResultsTop = dgvResults.Top,
                .dgvResultsHeight = dgvResults.Height,
                .lblDnMisclosureTop = lblDnMisclosure.Top,
                .lblDeMisclosureTop = lblDeMisclosure.Top,
                .lblLinearMisclosureTop = lblLinearMisclosure.Top,
                .lblAccuracyTop = lblAccuracy.Top,
                .lblAreaTop = lblArea.Top,
                .lblAngleSumTop = lblAngleSum.Top,
                .bottomLabelBottom = lblAngleSum.Top + lblAngleSum.Height,
                .grpResultsHeight = grpResults.Height
            }

            Dim contentWidth As Integer = Me.ClientSize.Width

            Dim allControls As New List(Of Control) From {
                grpControlPoints, grpStationData, btnCompute, grpResults,
                lblDnMisclosure, lblDeMisclosure, lblLinearMisclosure,
                lblAccuracy, lblArea, lblAngleSum
            }

            Dim maxBottom As Integer = 0
            For Each ctrl In allControls
                Dim parentOffset As Integer = 0
                If ctrl.Parent IsNot Nothing AndAlso ctrl.Parent IsNot Me Then
                    parentOffset = ctrl.Parent.Top
                End If
                Dim ctrlBottom As Integer = parentOffset + ctrl.Top + ctrl.Height
                If ctrlBottom > maxBottom Then maxBottom = ctrlBottom
            Next

            Dim finalHeight As Integer = maxBottom + 30
            Dim angleSumBottom As Integer = lblAngleSum.Top
            If lblAngleSum.Parent IsNot Nothing AndAlso lblAngleSum.Parent IsNot Me Then
                angleSumBottom = lblAngleSum.Parent.Top + lblAngleSum.Top + lblAngleSum.Height
            Else
                angleSumBottom = lblAngleSum.Top + lblAngleSum.Height
            End If
            System.IO.File.WriteAllText(
                System.IO.Path.Combine(System.IO.Path.GetTempPath(), "svy_capture_dims.txt"),
                "maxBottom=" & maxBottom.ToString() & vbCrLf &
                "finalHeight=" & finalHeight.ToString() & vbCrLf &
                "angleSumBottom=" & angleSumBottom.ToString() & vbCrLf &
                "isVisible=" & (angleSumBottom < finalHeight).ToString() & vbCrLf &
                "lblAngleSum.Top=" & lblAngleSum.Top.ToString() & vbCrLf &
                "lblAngleSum.Height=" & lblAngleSum.Height.ToString() & vbCrLf &
                "lblAngleSum.Parent.Top=" & (If(lblAngleSum.Parent IsNot Nothing, lblAngleSum.Parent.Top.ToString(), "N/A")) & vbCrLf &
                "grpResults.Top=" & grpResults.Top.ToString() & vbCrLf &
                "grpResults.Height=" & grpResults.Height.ToString() & vbCrLf &
                "grpResults.Bottom=" & (grpResults.Top + grpResults.Height).ToString() & vbCrLf &
                "Me.ClientSize.Height=" & Me.ClientSize.Height.ToString())
            Me.ClientSize = New Size(Me.ClientSize.Width, finalHeight)

            Using bitmap As New Bitmap(contentWidth, finalHeight)
                Using g As Graphics = Graphics.FromImage(bitmap)
                    g.Clear(Me.BackColor)
                End Using
                For Each ctrl As Control In Me.Controls
                    If ctrl.Top + ctrl.Height <= 0 OrElse ctrl.Top >= finalHeight Then
                        Continue For
                    End If
                    Dim ctrlBitmap As New Bitmap(ctrl.Width, ctrl.Height)
                    Try
                        ctrl.DrawToBitmap(ctrlBitmap, New Rectangle(0, 0, ctrl.Width, ctrl.Height))
                        Using cg As Graphics = Graphics.FromImage(bitmap)
                            cg.DrawImage(ctrlBitmap, ctrl.Left, ctrl.Top)
                        End Using
                    Finally
                        ctrlBitmap.Dispose()
                    End Try
                Next

                ' Re-render all children of every GroupBox individually to avoid
                ' DrawToBitmap clipping when children extend beyond form's visible area
                For Each gbCtrl As Control In Me.Controls
                    Dim gb As GroupBox = TryCast(gbCtrl, GroupBox)
                    If gb Is Nothing Then Continue For
                    For Each child As Control In gb.Controls
                        Dim childAbsTop = gb.Top + child.Top
                        If childAbsTop >= finalHeight Then Continue For
                        Dim childBmp As New Bitmap(child.Width, child.Height)
                        Try
                            child.DrawToBitmap(childBmp, New Rectangle(0, 0, child.Width, child.Height))
                            Using cg As Graphics = Graphics.FromImage(bitmap)
                                cg.DrawImage(childBmp, gb.Left + child.Left, childAbsTop)
                            End Using
                        Finally
                            childBmp.Dispose()
                        End Try
                    Next
                Next

                Dim ext As String = System.IO.Path.GetExtension(filePath).ToLower()
                If ext = ".jpg" OrElse ext = ".jpeg" Then
                    bitmap.Save(filePath, System.Drawing.Imaging.ImageFormat.Jpeg)
                Else
                    bitmap.Save(filePath, System.Drawing.Imaging.ImageFormat.Png)
                End If
            End Using
        Finally
            For Each kvp In origLabelTops
                kvp.Key.Top = kvp.Value
            Next
            dgvInput.Height = origInputHeight
            dgvResults.Height = origResultsHeight
            grpStationData.Height = origStationDataHeight
            grpResults.Height = origResultsGroupHeight
            grpResults.Top = origResultsTop
            For Each kvp In btnTopOffsets
                kvp.Key.Top = kvp.Value
            Next
            Me.Height = origFormHeight
            Me.ClientSize = origClientSize
        End Try
    End Sub

    Private Sub btnSaveImage_Click(sender As Object, e As EventArgs) Handles btnSaveImage.Click
        If dgvResults.Rows.Count = 0 Then
            MessageBox.Show("Please click Compute first.", "No Data", MessageBoxButtons.OK, MessageBoxIcon.Warning)
            Return
        End If

        Using sfd As New SaveFileDialog()
            sfd.Filter = "PNG Image|*.png|JPEG Image|*.jpg"
            sfd.FileName = "TraverseCapture.png"
            sfd.Title = "Save as Image"
            If sfd.ShowDialog() = DialogResult.OK Then
                Try
                    CaptureToBitmap(sfd.FileName)
                    MessageBox.Show("Image saved successfully to " & sfd.FileName, "Save Complete", MessageBoxButtons.OK, MessageBoxIcon.Information)
                Catch ex As Exception
                    MessageBox.Show("Error saving image: " & ex.Message, "Save Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
                End Try
            End If
        End Using
    End Sub

    Friend Function GetResultsRowCount() As Integer
        Return dgvResults.Rows.Count
    End Function

    Friend Sub AddInputRow(name As String, angle As String, dist As String)
        dgvInput.Rows.Add(name, angle, dist)
    End Sub

    Friend Sub PerformCompute()
        btnCompute.PerformClick()
    End Sub

    Friend Sub SaveImageToFile(filePath As String)
        CaptureToBitmap(filePath)
    End Sub

    Friend Function GetLabelPositions() As LabelPositionData
        Return _lastCaptureSnapshot
    End Function

End Class

Public Class LabelPositionData
    Public Property dgvResultsTop As Integer
    Public Property dgvResultsHeight As Integer
    Public Property lblDnMisclosureTop As Integer
    Public Property lblDeMisclosureTop As Integer
    Public Property lblLinearMisclosureTop As Integer
    Public Property lblAccuracyTop As Integer
    Public Property lblAreaTop As Integer
    Public Property lblAngleSumTop As Integer
    Public Property bottomLabelBottom As Integer
    Public Property grpResultsHeight As Integer
End Class
