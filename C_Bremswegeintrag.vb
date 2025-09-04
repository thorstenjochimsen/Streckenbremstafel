Imports System.Collections.ObjectModel
Imports System.ComponentModel

Public Class C_Bremswegeintrag
    Implements INotifyPropertyChanged

    Public Event PropertyChanged As PropertyChangedEventHandler Implements INotifyPropertyChanged.PropertyChanged

    Public Sub NotifyPropertyChanged(ByVal propertyName As String)
        RaiseEvent PropertyChanged(Me, New PropertyChangedEventArgs(propertyName))
    End Sub

    Public ReadOnly Property AnzeigeText As String
        Get
            Return $"{Streckennummer} - {Betriebsstelle} - {Signalbezeichnung} ({Typ}) - km {km:F3} {ID} > {NachbarID}"
        End Get
    End Property

    Private _PositionX As Double
    <Category("Darstellung")>
    <DisplayName("Position X")>
    <Description("X-Koordinate für die Darstellung im Canvas")>
    Public Property PositionX As Double
        Get
            Return _PositionX
        End Get
        Set(value As Double)
            If _PositionX <> value Then
                _PositionX = value
                NotifyPropertyChanged(NameOf(PositionX))
            End If
        End Set
    End Property

    Private _PositionY As Double
    <Category("Darstellung")>
    <DisplayName("Position Y")>
    <Description("Y-Koordinate für die Darstellung im Canvas")>
    Public Property PositionY As Double
        Get
            Return _PositionY
        End Get
        Set(value As Double)
            If _PositionY <> value Then
                _PositionY = value
                NotifyPropertyChanged(NameOf(PositionY))
            End If
        End Set
    End Property

    Private _ID As String
    <Category("Allgemein")>
    <DisplayName("ID")>
    <Description("ID")>
    Public Property ID As String
        Get
            Return _ID
        End Get
        Set(value As String)
            If _ID <> value Then
                _ID = value
                NotifyPropertyChanged(NameOf(ID))
            End If
        End Set
    End Property

    Private _NachbarID As String
    <Category("Allgemein")>
    <DisplayName("NachbarID")>
    <Description("NachbarID")>
    Public Property NachbarID As String
        Get
            Return _NachbarID
        End Get
        Set(value As String)
            If _NachbarID <> value Then
                _NachbarID = value
                NotifyPropertyChanged(NameOf(NachbarID))
            End If
        End Set
    End Property

    Private _Nachbar As C_Bremswegeintrag
    <Category("Allgemein")>
    <DisplayName("Nachbar")>
    <Description("Nachbar")>
    Public Property Nachbar As C_Bremswegeintrag
        Get
            Return _Nachbar
        End Get
        Set(value As C_Bremswegeintrag)
            If _Nachbar IsNot value Then
                _Nachbar = value
                NotifyPropertyChanged(NameOf(Nachbar))
            End If
        End Set
    End Property

    Private _Nachbar2ID As String
    <Category("Allgemein")>
    <DisplayName("Nachbar2ID")>
    <Description("Nachbar2ID")>
    Public Property Nachbar2ID As String
        Get
            Return _Nachbar2ID
        End Get
        Set(value As String)
            If _Nachbar2ID <> value Then
                _Nachbar2ID = value
                NotifyPropertyChanged(NameOf(Nachbar2ID))
            End If
        End Set
    End Property
    Private _Nachbar2 As C_Bremswegeintrag
    <Category("Allgemein")>
    <DisplayName("Nachbar2")>
    <Description("Nachbar2")>
    Public Property Nachbar2 As C_Bremswegeintrag
        Get
            Return _Nachbar2
        End Get
        Set(value As C_Bremswegeintrag)
            If _Nachbar2 IsNot value Then
                _Nachbar2 = value
                NotifyPropertyChanged(NameOf(Nachbar2))
            End If
        End Set
    End Property



    Private _Streckennummer As String
    <Category("Allgemein")>
    <DisplayName("Streckennummer")>
    <Description("Streckennummer")>
    Public Property Streckennummer As String
        Get
            Return _Streckennummer
        End Get
        Set(value As String)
            If _Streckennummer <> value Then
                _Streckennummer = value
                NotifyPropertyChanged(NameOf(Streckennummer))
            End If
        End Set
    End Property


    Private _Betriebsstelle As String
    <Category("Allgemein")>
    <DisplayName("Betriebsstelle")>
    <Description("Betriebsstelle")>
    Public Property Betriebsstelle As String
        Get
            Return _Betriebsstelle
        End Get
        Set(value As String)
            If _Betriebsstelle <> value Then
                _Betriebsstelle = value
                NotifyPropertyChanged(NameOf(Betriebsstelle))
            End If
        End Set
    End Property

    Private _Typ As EN_Betriebsstellentyp
    <Category("Allgemein")>
    <DisplayName("Typ")>
    <Description("Typ")>
    Public Property Typ As EN_Betriebsstellentyp
        Get
            Return _Typ
        End Get
        Set(value As EN_Betriebsstellentyp)
            If _Typ <> value Then
                _Typ = value
                NotifyPropertyChanged(NameOf(Typ))
            End If
        End Set
    End Property


    Private _Signalbezeichnung As String
    <Category("Allgemein")>
    <DisplayName("Signalbezeichnung")>
    <Description("Signalbezeichnung")>
    Public Property Signalbezeichnung As String
        Get
            Return _Signalbezeichnung
        End Get
        Set(value As String)
            If _Signalbezeichnung <> value Then
                _Signalbezeichnung = value
                NotifyPropertyChanged(NameOf(Signalbezeichnung))
            End If
        End Set
    End Property

    Private _km As Double
    <Category("Allgemein")>
    <DisplayName("km")>
    <Description("km")>
    Public Property km As Double
        Get
            Return _km
        End Get
        Set(value As Double)
            If _km <> value Then
                _km = value
                NotifyPropertyChanged(NameOf(km))
            End If
        End Set
    End Property

    Private _Bremsweg As Double
    <Category("Allgemein")>
    <DisplayName("Bremsweg")>
    <Description("Bremsweg")>
    Public ReadOnly Property Bremsweg As Double
        Get
            Return _Bremsweg
        End Get
    End Property

    Private _Neigung As Double
    <Category("Allgemein")>
    <DisplayName("Neigung")>
    <Description("Neigung")>
    Public Property Neigung As Double
        Get
            Return _Neigung
        End Get
        Set(value As Double)
            If _Neigung <> value Then
                _Neigung = value
                NotifyPropertyChanged(NameOf(Neigung))
            End If
        End Set
    End Property

    Private _MbrG20 As Integer
    <Category("Allgemein")>
    <DisplayName("MbrG20")>
    <Description("MbrG20")>
    Public Property MbrG20 As Integer
        Get
            Return _MbrG20
        End Get
        Set(value As Integer)
            If _MbrG20 <> value Then
                _MbrG20 = value
                NotifyPropertyChanged(NameOf(MbrG20))
            End If
        End Set
    End Property

    Private _MbrG30 As Integer
    <Category("Allgemein")>
    <DisplayName("MbrG30")>
    <Description("MbrG30")>
    Public Property MbrG30 As Integer
        Get
            Return _MbrG30
        End Get
        Set(value As Integer)
            If _MbrG30 <> value Then
                _MbrG30 = value
                NotifyPropertyChanged(NameOf(MbrG30))
            End If
        End Set
    End Property


    Private _MbrG40 As Integer
    <Category("Allgemein")>
    <DisplayName("MbrG40")>
    <Description("MbrG40")>
    Public Property MbrG40 As Integer
        Get
            Return _MbrG40
        End Get
        Set(value As Integer)
            If _MbrG40 <> value Then
                _MbrG40 = value
                NotifyPropertyChanged(NameOf(MbrG40))
            End If
        End Set
    End Property

    Private _MbrG50 As Integer
    <Category("Allgemein")>
    <DisplayName("MbrG50")>
    <Description("MbrG50")>
    Public Property MbrG50 As Integer
        Get
            Return _MbrG50
        End Get
        Set(value As Integer)
            If _MbrG50 <> value Then
                _MbrG50 = value
                NotifyPropertyChanged(NameOf(MbrG50))
            End If
        End Set
    End Property

    Private _MbrP20 As Integer
    <Category("Allgemein")>
    <DisplayName("MbrP20")>
    <Description("MbrP20")>
    Public Property MbrP20 As Integer
        Get
            Return _MbrP20
        End Get
        Set(value As Integer)
            If _MbrP20 <> value Then
                _MbrP20 = value
                NotifyPropertyChanged(NameOf(MbrP20))
            End If
        End Set
    End Property

    Private _MbrP30 As Integer
    <Category("Allgemein")>
    <DisplayName("MbrP30")>
    <Description("MbrP30")>
    Public Property MbrP30 As Integer
        Get
            Return _MbrP30
        End Get
        Set(value As Integer)
            If _MbrP30 <> value Then
                _MbrP30 = value
                NotifyPropertyChanged(NameOf(MbrP30))
            End If
        End Set
    End Property

    Private _MbrP40 As Integer
    <Category("Allgemein")>
    <DisplayName("MbrP40")>
    <Description("MbrP40")>
    Public Property MbrP40 As Integer
        Get
            Return _MbrP40
        End Get
        Set(value As Integer)
            If _MbrP40 <> value Then
                _MbrP40 = value
                NotifyPropertyChanged(NameOf(MbrP40))
            End If
        End Set
    End Property
    Private _MbrP50 As Integer
    <Category("Allgemein")>
    <DisplayName("MbrP50")>
    <Description("MbrP50")>
    Public Property MbrP50 As Integer
        Get
            Return _MbrP50
        End Get
        Set(value As Integer)
            If _MbrP50 <> value Then
                _MbrP50 = value
                NotifyPropertyChanged(NameOf(MbrP50))
            End If
        End Set
    End Property


    Private _Geschwindigkeit As Integer
    <Category("Allgemein")>
    <DisplayName("Geschwindigkeit")>
    <Description("Geschwindigkeit")>
    Public Property Geschwindigkeit As Integer
        Get
            Return _Geschwindigkeit
        End Get
        Set(value As Integer)
            If _Geschwindigkeit <> value Then
                _Geschwindigkeit = value
                NotifyPropertyChanged(NameOf(Geschwindigkeit))
                NotifyPropertyChanged(NameOf(S_Geschwindigkeit))
                VerfuegbareGeschwindigkeiten = New List(Of Integer) From {20, 30, 40, 50}.Where(Function(x) x <= _Geschwindigkeit).ToList()
            End If
        End Set
    End Property

    Public ReadOnly Property S_Geschwindigkeit As String
        Get
            Return Geschwindigkeit & " km/h"
        End Get
    End Property

    Private _Geschwindigkeit_Soll As Integer
    <Category("Allgemein")>
    <DisplayName("Geschwindigkeit_Soll")>
    <Description("Geschwindigkeit_Soll")>
    Public Property Geschwindigkeit_Soll As Integer
        Get
            Return _Geschwindigkeit_Soll
        End Get
        Set(value As Integer)
            If _Geschwindigkeit_Soll <> value Then
                _Geschwindigkeit_Soll = value
                NotifyPropertyChanged(NameOf(Geschwindigkeit_Soll))
                NotifyPropertyChanged(NameOf(S_Geschwindigkeit_Soll))
                If GeschwindigkeitSollChangedCallback IsNot Nothing Then
                    GeschwindigkeitSollChangedCallback.Invoke(Me)
                End If
            End If
        End Set
    End Property
    Public ReadOnly Property S_Geschwindigkeit_Soll As String
        Get
            Return Geschwindigkeit_Soll & " km/h"
        End Get
    End Property
    Public Property GeschwindigkeitSollChangedCallback As Action(Of C_Bremswegeintrag)

    Public Property VerfuegbareGeschwindigkeiten As List(Of Integer) = New List(Of Integer) From {20, 30, 40, 50}

    Private _Ausgeben As Boolean
    <Category("Allgemein")>
    <DisplayName("Ausgeben")>
    <Description("Ausgeben")>
    Public Property Ausgeben As Boolean
        Get
            Return _Ausgeben
        End Get
        Set(value As Boolean)
            If _Ausgeben <> value Then
                _Ausgeben = value
                NotifyPropertyChanged(NameOf(Ausgeben))
            End If
        End Set
    End Property

    Private _Bemerkung As String
    <Category("Allgemein")>
    <DisplayName("Bemerkung")>
    <Description("Bemerkung")>
    Public Property Bemerkung As String
        Get
            Return _Bemerkung
        End Get
        Set(value As String)
            If _Bemerkung <> value Then
                _Bemerkung = value
                NotifyPropertyChanged(NameOf(Bemerkung))
            End If
        End Set
    End Property

    Private _Ankunft As DateTime
    <Category("Allgemein")>
    <DisplayName("Ankunft")>
    <Description("Ankunft")>
    Public Property Ankunft As DateTime
        Get
            Return _Ankunft
        End Get
        Set(value As DateTime)
            If _Ankunft <> value Then
                _Ankunft = value
                NotifyPropertyChanged(NameOf(Ankunft))
                NotifyPropertyChanged(NameOf(S_Ankunft))
            End If
        End Set
    End Property
    Public ReadOnly Property S_Ankunft As String
        Get
            If Ankunft = Date.MinValue Then Return ""
            Return Ankunft.ToString("HH:mm:ss")
        End Get
    End Property

    Private _Abfahrt As DateTime
    <Category("Allgemein")>
    <DisplayName("Abfahrt")>
    <Description("Abfahrt")>
    Public Property Abfahrt As DateTime
        Get
            Return _Abfahrt
        End Get
        Set(value As DateTime)
            If _Abfahrt <> value Then
                _Abfahrt = value
                NotifyPropertyChanged(NameOf(Abfahrt))
                NotifyPropertyChanged(NameOf(S_Abfahrt))
            End If
        End Set
    End Property

    Public ReadOnly Property S_Abfahrt As String
        Get
            If Abfahrt = Date.MinValue Then Return ""
            Return Abfahrt.ToString("HH:mm:ss")
        End Get
    End Property

End Class


Public Enum EN_Betriebsstellentyp
    Streckenbeginn = 0
    Sbk
    Esig
    Asig
    Zsig
    Bksig
    Sperrsig
    Streckenende
End Enum



