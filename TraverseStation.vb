Option Strict On
Option Explicit On

''' <summary>
''' Represents a single station in a traverse survey, storing
''' raw field observations and computed coordinate values.
''' </summary>
Public Class TraverseStation

    ''' <summary>The point identifier or label for this station.</summary>
    Public Property Name As String = ""

    ''' <summary>The interior (or deflection) angle measured at this station, in decimal degrees.</summary>
    Public Property IncludedAngle As Double = 0.0

    ''' <summary>The horizontal distance from this station to the next, in metres.</summary>
    Public Property Distance As Double = 0.0

    ''' <summary>The computed forward bearing of the leg leaving this station, in decimal degrees (0-360).</summary>
    Public Property ForwardBearing As Double = 0.0

    ''' <summary>The back bearing of the leg arriving at this station, in decimal degrees (0-360).</summary>
    Public Property BackBearing As Double = 0.0

    ''' <summary>The departure (easting component) of this leg, in metres.</summary>
    Public Property DN As Double = 0.0

    ''' <summary>The latitude (northing component) of this leg, in metres.</summary>
    Public Property DE As Double = 0.0

    ''' <summary>The departure after applying the Bowditch adjustment, in metres.</summary>
    Public Property CorrectedDN As Double = 0.0

    ''' <summary>The latitude after applying the Bowditch adjustment, in metres.</summary>
    Public Property CorrectedDE As Double = 0.0

    ''' <summary>The final adjusted northing coordinate of this station, in metres.</summary>
    Public Property Northing As Double = 0.0

    ''' <summary>The final adjusted easting coordinate of this station, in metres.</summary>
    Public Property Easting As Double = 0.0

End Class
