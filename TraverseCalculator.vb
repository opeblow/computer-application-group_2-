Option Strict On
Option Explicit On

''' <summary>
''' Provides static methods for each step of a closed-link traverse
''' calculation: bearing propagation, coordinate differences,
''' misclosure, Bowditch adjustment, final coordinates, and area.
''' </summary>
Public Class TraverseCalculator

    ''' <summary>
    ''' Computes the forward bearing for each station leg and sets the
    ''' corresponding back bearing.  Forward bearing of leg i is the
    ''' back bearing of leg i-1 plus the included angle, normalised to
    ''' [0, 360).  The back bearing is forward bearing + 180, also
    ''' normalised.
    ''' </summary>
    ''' <param name="stations">List of traverse stations in order.</param>
    ''' <param name="initialBackBearing">Known back bearing into the first station, in decimal degrees.</param>
    Public Shared Sub ComputeForwardBearings(stations As List(Of TraverseStation), initialBackBearing As Double)
        Dim prevBackBearing As Double = initialBackBearing

        For Each st In stations
            Dim fwd = prevBackBearing + st.IncludedAngle
            fwd = NormaliseAngle(fwd)
            st.ForwardBearing = fwd

            Dim back = fwd + 180.0
            back = NormaliseAngle(back)
            st.BackBearing = back

            prevBackBearing = back
        Next
    End Sub

    ''' <summary>
    ''' Computes the departure (DE = Distance * Sin(bearing)) and
    ''' latitude (DN = Distance * Cos(bearing)) for each leg.  The
    ''' bearing is first converted from degrees to radians.
    ''' </summary>
    Public Shared Sub ComputeDepartureAndLatitude(stations As List(Of TraverseStation))
        For Each st In stations
            Dim rad = st.ForwardBearing * (Math.PI / 180.0)
            st.DE = st.Distance * Math.Sin(rad)
            st.DN = st.Distance * Math.Cos(rad)
        Next
    End Sub

    ''' <summary>
    ''' Computes the total misclosure in DN and DE by comparing the
    ''' summed departures/latitudes with the known coordinate
    ''' difference between the initial and final control points.
    ''' </summary>
    ''' <returns>A tuple (dnError, deError) in metres.</returns>
    Public Shared Function ComputeMisclosure(stations As List(Of TraverseStation),
                                             initialNorthing As Double,
                                             initialEasting As Double,
                                             finalNorthing As Double,
                                             finalEasting As Double) As (dnError As Double, deError As Double)
        Dim sumDN As Double = 0.0
        Dim sumDE As Double = 0.0
        For Each st In stations
            sumDN += st.DN
            sumDE += st.DE
        Next

        Dim dnError = sumDN - (finalNorthing - initialNorthing)
        Dim deError = sumDE - (finalEasting - initialEasting)
        Return (dnError, deError)
    End Function

    ''' <summary>
    ''' Applies the Bowditch (compass rule) adjustment to each leg.
    ''' The correction is proportional to the leg distance divided by
    ''' the total traverse distance.
    ''' </summary>
    Public Shared Sub ApplyBowditchCorrection(stations As List(Of TraverseStation),
                                              dnError As Double,
                                              deError As Double)
        Dim totalDist As Double = 0.0
        For Each st In stations
            totalDist += st.Distance
        Next

        If totalDist = 0.0 Then Return

        For Each st In stations
            Dim factor = st.Distance / totalDist
            st.CorrectedDN = st.DN - dnError * factor
            st.CorrectedDE = st.DE - deError * factor
        Next
    End Sub

    ''' <summary>
    ''' Computes the final adjusted northing and easting for each
    ''' station by cumulatively adding the corrected DN and DE
    ''' starting from the initial control point coordinates.
    ''' </summary>
    Public Shared Sub ComputeFinalCoordinates(stations As List(Of TraverseStation),
                                              initialNorthing As Double,
                                              initialEasting As Double)
        Dim runningN As Double = initialNorthing
        Dim runningE As Double = initialEasting

        For Each st In stations
            runningN += st.CorrectedDN
            runningE += st.CorrectedDE
            st.Northing = runningN
            st.Easting = runningE
        Next
    End Sub

    ''' <summary>
    ''' Computes the linear misclosure (Sqrt(dnError^2 + deError^2))
    ''' and the accuracy ratio (linear misclosure / total distance).
    ''' </summary>
    ''' <returns>A tuple (linearMisclosure, accuracyRatio).</returns>
    Public Shared Function ComputeLinearAccuracy(dnError As Double,
                                                 deError As Double,
                                                 totalDistance As Double) As (linearMisclosure As Double, accuracyRatio As Double)
        Dim linearMisclosure = Math.Sqrt(dnError * dnError + deError * deError)
        Dim accuracyRatio = 0.0
        If totalDistance > 0.0 Then
            accuracyRatio = linearMisclosure / totalDistance
        End If
        Return (linearMisclosure, accuracyRatio)
    End Function

    ''' <summary>
    ''' Computes the enclosed area of the traverse using the shoelace
    ''' formula.  Easting is treated as X and Northing as Y.  Returns
    ''' the absolute value of the area in square metres.
    ''' </summary>
    Public Shared Function ComputeAreaByCoordinates(stations As List(Of TraverseStation)) As Double
        Dim n = stations.Count
        If n < 3 Then Return 0.0

        Dim area As Double = 0.0
        For i As Integer = 0 To n - 1
            Dim j = (i + 1) Mod n
            area += stations(i).Easting * stations(j).Northing
            area -= stations(j).Easting * stations(i).Northing
        Next

        Return Math.Abs(area) / 2.0
    End Function

    Private Shared Function NormaliseAngle(angle As Double) As Double
        While angle < 0.0
            angle += 360.0
        End While
        While angle >= 360.0
            angle -= 360.0
        End While
        Return angle
    End Function

End Class
