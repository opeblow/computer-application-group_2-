Option Strict On
Option Explicit On

Partial Class Form1
    Inherits System.Windows.Forms.Form

    Private components As System.ComponentModel.IContainer

    ' Control Points
    Private grpControlPoints As GroupBox
    Private lblInitName As Label
    Private lblInitNorthing As Label
    Private lblInitEasting As Label
    Private lblInitBearing As Label
    Private lblFinalName As Label
    Private lblFinalNorthing As Label
    Private lblFinalEasting As Label
    Private txtInitName As TextBox
    Private txtInitNorthing As TextBox
    Private txtInitEasting As TextBox
    Private txtInitBearing As TextBox
    Private txtFinalName As TextBox
    Private txtFinalNorthing As TextBox
    Private txtFinalEasting As TextBox

    ' Station Data
    Private grpStationData As GroupBox
    Private dgvInput As DataGridView

    ' Compute
    Private WithEvents btnCompute As Button

    ' Results
    Private grpResults As GroupBox
    Private dgvResults As DataGridView
    Private lblDnMisclosure As Label
    Private lblDeMisclosure As Label
    Private lblLinearMisclosure As Label
    Private lblAccuracy As Label
    Private lblArea As Label
    Private lblAngleSum As Label

    ' Export
    Private WithEvents btnExport As Button

    ' Clear All
    Private WithEvents btnClearAll As Button

    ' Add Row
    Private WithEvents btnAddRow As Button

    ' Save Image
    Private WithEvents btnSaveImage As Button

    <System.Diagnostics.DebuggerStepThrough()>
    Private Sub InitializeComponent()
        Me.Text = "Traverse Computation"
        Me.ClientSize = New Size(1100, 750)
        Me.StartPosition = FormStartPosition.CenterScreen

        ' grpControlPoints
        Me.grpControlPoints = New GroupBox()
        Me.grpControlPoints.Text = "Control Points & Initial Bearing"
        Me.grpControlPoints.Location = New Point(12, 12)
        Me.grpControlPoints.Size = New Size(1060, 120)
        Me.grpControlPoints.Anchor = AnchorStyles.Top Or AnchorStyles.Left Or AnchorStyles.Right

        Me.lblInitName = New Label()
        Me.lblInitName.Text = "Initial Station:"
        Me.lblInitName.Location = New Point(12, 25)
        Me.lblInitName.Size = New Size(90, 22)

        Me.txtInitName = New TextBox()
        Me.txtInitName.Location = New Point(110, 23)
        Me.txtInitName.Size = New Size(60, 22)
        Me.txtInitName.Text = "BM1"

        Me.lblInitNorthing = New Label()
        Me.lblInitNorthing.Text = "Northing:"
        Me.lblInitNorthing.Location = New Point(180, 25)
        Me.lblInitNorthing.Size = New Size(60, 22)

        Me.txtInitNorthing = New TextBox()
        Me.txtInitNorthing.Location = New Point(240, 23)
        Me.txtInitNorthing.Size = New Size(80, 22)
        Me.txtInitNorthing.Text = "5000"

        Me.lblInitEasting = New Label()
        Me.lblInitEasting.Text = "Easting:"
        Me.lblInitEasting.Location = New Point(330, 25)
        Me.lblInitEasting.Size = New Size(60, 22)

        Me.txtInitEasting = New TextBox()
        Me.txtInitEasting.Location = New Point(390, 23)
        Me.txtInitEasting.Size = New Size(80, 22)
        Me.txtInitEasting.Text = "5000"

        Me.lblInitBearing = New Label()
        Me.lblInitBearing.Text = "Init Back Bearing:"
        Me.lblInitBearing.Location = New Point(480, 25)
        Me.lblInitBearing.Size = New Size(110, 22)

        Me.txtInitBearing = New TextBox()
        Me.txtInitBearing.Location = New Point(595, 23)
        Me.txtInitBearing.Size = New Size(80, 22)
        Me.txtInitBearing.Text = "60.0"

        Me.lblFinalName = New Label()
        Me.lblFinalName.Text = "Final Station:"
        Me.lblFinalName.Location = New Point(12, 60)
        Me.lblFinalName.Size = New Size(90, 22)

        Me.txtFinalName = New TextBox()
        Me.txtFinalName.Location = New Point(110, 58)
        Me.txtFinalName.Size = New Size(60, 22)
        Me.txtFinalName.Text = "BM2"

        Me.lblFinalNorthing = New Label()
        Me.lblFinalNorthing.Text = "Northing:"
        Me.lblFinalNorthing.Location = New Point(180, 60)
        Me.lblFinalNorthing.Size = New Size(60, 22)

        Me.txtFinalNorthing = New TextBox()
        Me.txtFinalNorthing.Location = New Point(240, 58)
        Me.txtFinalNorthing.Size = New Size(80, 22)
        Me.txtFinalNorthing.Text = "5000"

        Me.lblFinalEasting = New Label()
        Me.lblFinalEasting.Text = "Easting:"
        Me.lblFinalEasting.Location = New Point(330, 60)
        Me.lblFinalEasting.Size = New Size(60, 22)

        Me.txtFinalEasting = New TextBox()
        Me.txtFinalEasting.Location = New Point(390, 58)
        Me.txtFinalEasting.Size = New Size(80, 22)
        Me.txtFinalEasting.Text = "5000"

        Me.grpControlPoints.Controls.AddRange(New Control() {
            Me.lblInitName, Me.txtInitName,
            Me.lblInitNorthing, Me.txtInitNorthing,
            Me.lblInitEasting, Me.txtInitEasting,
            Me.lblInitBearing, Me.txtInitBearing,
            Me.lblFinalName, Me.txtFinalName,
            Me.lblFinalNorthing, Me.txtFinalNorthing,
            Me.lblFinalEasting, Me.txtFinalEasting})

        ' grpStationData
        Me.grpStationData = New GroupBox()
        Me.grpStationData.Text = "Station Data"
        Me.grpStationData.Location = New Point(12, 140)
        Me.grpStationData.Size = New Size(1060, 200)
        Me.grpStationData.Anchor = AnchorStyles.Top Or AnchorStyles.Left Or AnchorStyles.Right

        Me.dgvInput = New DataGridView()
        Me.dgvInput.Location = New Point(6, 20)
        Me.dgvInput.Size = New Size(1040, 170)
        Me.dgvInput.Anchor = AnchorStyles.Top Or AnchorStyles.Left Or AnchorStyles.Bottom Or AnchorStyles.Right
        Me.dgvInput.ColumnCount = 3
        Me.dgvInput.Columns(0).Name = "Station Name"
        Me.dgvInput.Columns(0).Width = 100
        Me.dgvInput.Columns(1).Name = "Included Angle (deg)"
        Me.dgvInput.Columns(1).Width = 140
        Me.dgvInput.Columns(2).Name = "Distance (m)"
        Me.dgvInput.Columns(2).Width = 100
        Me.dgvInput.AllowUserToAddRows = True
        Me.dgvInput.AllowUserToDeleteRows = True
        Me.dgvInput.RowHeadersVisible = False
        Me.dgvInput.Rows.Add("ST1", "142.3500", "85.420")
        Me.dgvInput.Rows.Add("ST2", "128.1200", "102.650")
        Me.dgvInput.Rows.Add("ST3", "115.8300", "76.300")
        Me.dgvInput.Rows.Add("ST4", "96.4700", "93.180")
        Me.dgvInput.Rows.Add("BM2", "122.9000", "121.050")

        Me.grpStationData.Controls.Add(Me.dgvInput)

        ' btnCompute
        Me.btnCompute = New Button()
        Me.btnCompute.Text = "Compute"
        Me.btnCompute.Location = New Point(12, 348)
        Me.btnCompute.Size = New Size(100, 30)
        Me.btnCompute.Anchor = AnchorStyles.Top Or AnchorStyles.Left
        Me.btnCompute.UseVisualStyleBackColor = True

        ' grpResults
        Me.grpResults = New GroupBox()
        Me.grpResults.Text = "Results"
        Me.grpResults.Location = New Point(12, 385)
        Me.grpResults.Size = New Size(1060, 250)
        Me.grpResults.Anchor = AnchorStyles.Top Or AnchorStyles.Bottom Or AnchorStyles.Left Or AnchorStyles.Right

        Me.dgvResults = New DataGridView()
        Me.dgvResults.Location = New Point(6, 20)
        Me.dgvResults.Size = New Size(1040, 130)
        Me.dgvResults.Anchor = AnchorStyles.Top Or AnchorStyles.Left Or AnchorStyles.Right
        Me.dgvResults.ColumnCount = 9
        Me.dgvResults.Columns(0).Name = "Station"
        Me.dgvResults.Columns(0).Width = 60
        Me.dgvResults.Columns(1).Name = "Fwd Bearing"
        Me.dgvResults.Columns(1).Width = 90
        Me.dgvResults.Columns(2).Name = "Back Bearing"
        Me.dgvResults.Columns(2).Width = 90
        Me.dgvResults.Columns(3).Name = "DN"
        Me.dgvResults.Columns(3).Width = 90
        Me.dgvResults.Columns(4).Name = "DE"
        Me.dgvResults.Columns(4).Width = 90
        Me.dgvResults.Columns(5).Name = "Corr DN"
        Me.dgvResults.Columns(5).Width = 90
        Me.dgvResults.Columns(6).Name = "Corr DE"
        Me.dgvResults.Columns(6).Width = 90
        Me.dgvResults.Columns(7).Name = "Northing"
        Me.dgvResults.Columns(7).Width = 110
        Me.dgvResults.Columns(8).Name = "Easting"
        Me.dgvResults.Columns(8).Width = 110
        Me.dgvResults.ReadOnly = True
        Me.dgvResults.AllowUserToAddRows = False
        Me.dgvResults.AllowUserToDeleteRows = False
        Me.dgvResults.RowHeadersVisible = False

        Dim lblY As Integer = 155
        Me.lblDnMisclosure = New Label()
        Me.lblDnMisclosure.AutoSize = True
        Me.lblDnMisclosure.Text = "Total DN Misclosure:"
        Me.lblDnMisclosure.Location = New Point(10, lblY)

        Me.lblDeMisclosure = New Label()
        Me.lblDeMisclosure.AutoSize = True
        Me.lblDeMisclosure.Text = "Total DE Misclosure:"
        Me.lblDeMisclosure.Location = New Point(280, lblY)

        Me.lblLinearMisclosure = New Label()
        Me.lblLinearMisclosure.AutoSize = True
        Me.lblLinearMisclosure.Text = "Linear Misclosure:"
        Me.lblLinearMisclosure.Location = New Point(550, lblY)

        lblY += 25
        Me.lblAccuracy = New Label()
        Me.lblAccuracy.AutoSize = True
        Me.lblAccuracy.Text = "Accuracy Ratio:"
        Me.lblAccuracy.Location = New Point(10, lblY)

        Me.lblArea = New Label()
        Me.lblArea.AutoSize = True
        Me.lblArea.Text = "Area:"
        Me.lblArea.Location = New Point(420, lblY)

        lblY += 25
        Me.lblAngleSum = New Label()
        Me.lblAngleSum.AutoSize = True
        Me.lblAngleSum.Text = "Sum of Included Angles:"
        Me.lblAngleSum.Location = New Point(10, lblY)

        Me.grpResults.Controls.AddRange(New Control() {
            Me.dgvResults,
            Me.lblDnMisclosure, Me.lblDeMisclosure,
            Me.lblLinearMisclosure, Me.lblAccuracy, Me.lblArea,
            Me.lblAngleSum})

        ' btnExport
        Me.btnExport = New Button()
        Me.btnExport.Text = "Export Report"
        Me.btnExport.Location = New Point(120, 348)
        Me.btnExport.Size = New Size(100, 30)
        Me.btnExport.Anchor = AnchorStyles.Top Or AnchorStyles.Left
        Me.btnExport.UseVisualStyleBackColor = True

        ' btnClearAll
        Me.btnClearAll = New Button()
        Me.btnClearAll.Text = "Clear All"
        Me.btnClearAll.Location = New Point(230, 348)
        Me.btnClearAll.Size = New Size(100, 30)
        Me.btnClearAll.Anchor = AnchorStyles.Top Or AnchorStyles.Left
        Me.btnClearAll.UseVisualStyleBackColor = True

        ' btnAddRow
        Me.btnAddRow = New Button()
        Me.btnAddRow.Text = "Add Row"
        Me.btnAddRow.Location = New Point(340, 348)
        Me.btnAddRow.Size = New Size(100, 30)
        Me.btnAddRow.Anchor = AnchorStyles.Top Or AnchorStyles.Left
        Me.btnAddRow.UseVisualStyleBackColor = True

        ' btnSaveImage
        Me.btnSaveImage = New Button()
        Me.btnSaveImage.Text = "Save as Image"
        Me.btnSaveImage.Location = New Point(450, 348)
        Me.btnSaveImage.Size = New Size(110, 30)
        Me.btnSaveImage.Anchor = AnchorStyles.Top Or AnchorStyles.Left
        Me.btnSaveImage.UseVisualStyleBackColor = True

        Me.Controls.AddRange(New Control() {
            Me.grpControlPoints,
            Me.grpStationData,
            Me.btnCompute,
            Me.btnExport,
            Me.btnClearAll,
            Me.btnAddRow,
            Me.btnSaveImage,
            Me.grpResults})
    End Sub

End Class
