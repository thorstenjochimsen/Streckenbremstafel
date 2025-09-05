Imports System.Collections.ObjectModel
Imports System.ComponentModel
Imports System.IO
Imports System.Xml.Linq
Imports ClosedXML.Excel

Class MainWindow
    Implements INotifyPropertyChanged
    Public Event PropertyChanged As PropertyChangedEventHandler Implements INotifyPropertyChanged.PropertyChanged
    Public Sub NotifyPropertyChanged(ByVal propertyName As String)
        RaiseEvent PropertyChanged(Me, New PropertyChangedEventArgs(propertyName))
    End Sub

    Public Listata As ObservableCollection(Of C_Bremswegeintrag)

    Private _Laufweg As New ObservableCollection(Of C_Bremswegeintrag)
    Public Property Laufweg As ObservableCollection(Of C_Bremswegeintrag)
        Get
            Return _Laufweg
        End Get
        Set(value As ObservableCollection(Of C_Bremswegeintrag))
            If _Laufweg Is value Then Return
            _Laufweg = value
            NotifyPropertyChanged(NameOf(Laufweg))
        End Set
    End Property

    Private _LaufwegTemp As ObservableCollection(Of C_Bremswegeintrag)
    Public Property LaufwegTemp As ObservableCollection(Of C_Bremswegeintrag)
        Get
            Return _LaufwegTemp
        End Get
        Set(value As ObservableCollection(Of C_Bremswegeintrag))
            If _LaufwegTemp Is value Then Return
            _LaufwegTemp = value
            NotifyPropertyChanged(NameOf(LaufwegTemp))
        End Set
    End Property


    Private draggedEllipse As System.Windows.Shapes.Ellipse = Nothing
    Private draggedEntry As C_Bremswegeintrag = Nothing
    Private mouseOffset As Point

    Private isPanning As Boolean = False
    Private panStart As Point

    Private _Edit As Boolean = False

    Property StartEintrag As C_Bremswegeintrag = Nothing
    Property ZielEintrag As C_Bremswegeintrag = Nothing

    Public Property Edit As Boolean
        Get
            Return _Edit
        End Get
        Set(value As Boolean)
            If _Edit = value Then Return
            _Edit = value
            If _Edit Then
                EditElements = Visibility.Visible
            Else
                EditElements = Visibility.Collapsed
            End If
            NotifyPropertyChanged(NameOf(Edit))
        End Set
    End Property

    Private _EditElements As Visibility
    Public Property EditElements As Visibility
        Get
            Return _EditElements
        End Get
        Set(value As Visibility)
            If _EditElements = value Then Return
            _EditElements = value
            NotifyPropertyChanged(NameOf(EditElements))
        End Set
    End Property

    Public Sub New()
        ' Dieser Aufruf ist für den Designer erforderlich.
        InitializeComponent()

        ' Fügen Sie Initialisierungen nach dem InitializeComponent()-Aufruf hinzu.
        Laufweg = New ObservableCollection(Of C_Bremswegeintrag)
        LaufwegTemp = New ObservableCollection(Of C_Bremswegeintrag)

        Me.DataContext = Me
        Edit = False
    End Sub

    Private Sub BTN_Laden_Click(sender As Object, e As RoutedEventArgs)
        Listata = LoadListFromXmlFromAppPath()

        For Each item In Listata
            item.GeschwindigkeitSollChangedCallback = Sub(eintrag)
                                                          AusgabeBerechnen()
                                                      End Sub

            Dim nextID As String = item.NachbarID
            Dim nextEintrag As C_Bremswegeintrag = Listata.FirstOrDefault(Function(x) x.ID = nextID)
            If nextEintrag IsNot Nothing Then
                item.Nachbar = nextEintrag
            Else
                item.Nachbar = Nothing
            End If

            Dim nextID2 As String = item.Nachbar2ID
            Dim nextEintrag2 As C_Bremswegeintrag = Listata.FirstOrDefault(Function(x) x.ID = nextID2)
            If nextEintrag2 IsNot Nothing Then
                item.Nachbar2 = nextEintrag2
            Else
                item.Nachbar2 = Nothing
            End If


        Next
        'CB_Start.ItemsSource = Listata
        'CB_Ziel.ItemsSource = Listata

        ZeichneStrecke()
        DG_Bremswegeintraege.ItemsSource = Listata
    End Sub
    Private Sub Window_Loaded(sender As Object, e As RoutedEventArgs)
        BTN_Laden_Click(Nothing, Nothing)
    End Sub
    Public Shared Function LoadListFromXmlFromAppPath() As ObservableCollection(Of C_Bremswegeintrag)
        Try
            Dim appPath As String = AppDomain.CurrentDomain.BaseDirectory
            Dim filePath As String = Path.Combine(appPath, "streckendaten.xml")
            Dim result As New ObservableCollection(Of C_Bremswegeintrag)
            Dim doc As XDocument = XDocument.Load(filePath)

            For Each entry In doc.Root.Elements("C_Bremswegeintrag")
                Dim item As New C_Bremswegeintrag()
                item.Streckennummer = entry.Element("Streckennummer")?.Value
                item.Betriebsstelle = entry.Element("Betriebsstelle")?.Value

                Dim typStr = entry.Element("Typ")?.Value
                If Not String.IsNullOrEmpty(typStr) Then
                    item.Typ = [Enum].Parse(GetType(EN_Betriebsstellentyp), typStr)
                End If
                item.ID = entry.Element("ID")?.Value
                item.NachbarID = entry.Element("NachbarID")?.Value
                item.Nachbar2ID = entry.Element("Nachbar2ID")?.Value

                item.Signalbezeichnung = entry.Element("Signalbezeichnung")?.Value
                item.km = Double.Parse(entry.Element("km")?.Value)


                If Not String.IsNullOrEmpty(entry.Element("Neigung")?.Value) Then
                    item.Neigung = Double.Parse(entry.Element("Neigung")?.Value.Replace(",", "."))
                Else
                    item.Neigung = 0
                End If

                item.MbrG20 = Integer.Parse(entry.Element("MbrG20")?.Value)
                item.MbrG30 = Integer.Parse(entry.Element("MbrG30")?.Value)
                item.MbrG40 = Integer.Parse(entry.Element("MbrG40")?.Value)
                item.MbrG50 = Integer.Parse(entry.Element("MbrG50")?.Value)
                item.MbrP20 = Integer.Parse(entry.Element("MbrP20")?.Value)
                item.MbrP30 = Integer.Parse(entry.Element("MbrP30")?.Value)
                item.MbrP40 = Integer.Parse(entry.Element("MbrP40")?.Value)
                item.MbrP50 = Integer.Parse(entry.Element("MbrP50")?.Value)

                If Not String.IsNullOrEmpty(entry.Element("PositionX")?.Value) Then
                    item.PositionX = Integer.Parse(entry.Element("PositionX")?.Value)
                Else
                    item.PositionX = 0 ' or another default value
                End If

                If Not String.IsNullOrEmpty(entry.Element("PositionY")?.Value) Then
                    item.PositionY = Integer.Parse(entry.Element("PositionY")?.Value)
                Else
                    item.PositionY = 0 ' or another default value
                End If
                result.Add(item)
            Next
            Return result
        Catch ex As Exception
            MsgBox(ex.Message, MsgBoxStyle.Critical, "Fehler beim Laden der Streckendaten")
            Return Nothing
        End Try

    End Function
    Private Sub CB_Start_DropDownClosed(sender As Object, e As EventArgs)
        Berechne_Laufweg()
    End Sub
    Private Sub CB_Ziel_DropDownClosed(sender As Object, e As EventArgs)
        Berechne_Laufweg()
    End Sub
    Private Sub Berechne_Laufweg()
        'If CB_Start.SelectedValue Is Nothing Then
        '    TBHinweis.Text = "Bitte Start auswählen!"
        '    Return
        'End If
        'If CB_Ziel.SelectedValue Is Nothing Then
        '    TBHinweis.Text = "Bitte Ziel auswählen!"
        '    Return
        'End If

        If startEintrag Is Nothing Then
            TBHinweis.Text = "Bitte Start auswählen!"
            Return
        End If
        If zielEintrag Is Nothing Then
            TBHinweis.Text = "Bitte Ziel auswählen!"
            Return
        End If

        'Dim startEintrag As C_Bremswegeintrag = CType(CB_Start.SelectedItem, C_Bremswegeintrag)
        'Dim zielEintrag As C_Bremswegeintrag = CType(CB_Ziel.SelectedItem, C_Bremswegeintrag)

        Debug.WriteLine($"Laufwegssuche gestartet - Start:{startEintrag.Signalbezeichnung} Ziel:{zielEintrag.Signalbezeichnung}")

        Dim Laufwegsuche As New List(Of List(Of C_Bremswegeintrag))
        Laufwegsuche.Add(New List(Of C_Bremswegeintrag))
        Laufwegsuche(0).Add(startEintrag)
        Dim LaufwegKomplett As New List(Of C_Bremswegeintrag)


StartLaufwegsuche:
        Do
            Dim iindex As Integer = Laufwegsuche(0).Count - 1
            Dim aktuellerEintrag As C_Bremswegeintrag = Laufwegsuche(0)(iindex)
            Debug.WriteLine($"Aktueller Eintrag: {aktuellerEintrag.Signalbezeichnung} ({aktuellerEintrag.ID})")

            If aktuellerEintrag.ID = ZielEintrag.ID Then 'Ziel erreicht
                LaufwegKomplett.AddRange(Laufwegsuche(0))
                Debug.WriteLine("Ziel erreicht bei " & aktuellerEintrag.Signalbezeichnung & " (" & aktuellerEintrag.ID & ")")
                Exit Do
            End If

            Dim nextEintrag As C_Bremswegeintrag = Listata.FirstOrDefault(Function(x) x.ID = aktuellerEintrag.NachbarID)
            Dim nextEintrag2 As C_Bremswegeintrag = Listata.FirstOrDefault(Function(x) x.ID = aktuellerEintrag.Nachbar2ID)

            aktuellerEintrag.Nachbar = nextEintrag
            aktuellerEintrag.Nachbar2 = nextEintrag2

            If nextEintrag2 IsNot Nothing Then 'Es wurde eine Abzweigung gefunden
                Laufwegsuche.Add(New List(Of C_Bremswegeintrag))
                Laufwegsuche(Laufwegsuche.Count - 1).AddRange(Laufwegsuche(0))
                Laufwegsuche(Laufwegsuche.Count - 1).Add(nextEintrag2)
                Debug.WriteLine("Abzweigung gefunden bei " & aktuellerEintrag.Signalbezeichnung & " (" & aktuellerEintrag.ID & ")")
            End If


            If nextEintrag Is Nothing Then
                Debug.WriteLine($"Kein weiterer Eintrag gefunden (NachbarID {aktuellerEintrag.NachbarID} nicht vorhanden)!")
                Laufwegsuche.RemoveAt(0)
                If Laufwegsuche.Count = 0 Then
                    TBHinweis.Text = "Kein Weg zum Ziel gefunden!"
                    Exit Sub
                Else
                    Debug.WriteLine("Wechsel zu anderem Suchpfad")
                    GoTo StartLaufwegsuche
                End If
            ElseIf nextEintrag IsNot Nothing Then
                Laufwegsuche(0).Add(nextEintrag)
            End If

        Loop

        LaufwegTemp = New ObservableCollection(Of C_Bremswegeintrag)

        For Each item In LaufwegKomplett
            LaufwegTemp.Add(item)
        Next


        'laufweg.Reverse()
        ' Ausgabe des Laufwegs
        TBHinweis.Text = ""

        Dim VorhandenBrH As Integer = 0
        ZeichneStrecke()

        Try
            VorhandenBrH = CInt(TB_Mbr.Text)
        Catch ex As Exception
            TBHinweis.Text = "Die vorhandenen BrH können nicht umgerechnet werden!"
            Exit Sub
        End Try

        Dim aktuelleVmax As Integer = 0

        For Each eintrag In Laufweg
            Dim kmh20 As Integer = 0
            Dim kmh30 As Integer = 0
            Dim kmh40 As Integer = 0
            Dim kmh50 As Integer = 0
            Select Case CB_Bremsstellung.Text
                Case "G"
                    kmh20 = eintrag.MbrG20
                    kmh30 = eintrag.MbrG30
                    kmh40 = eintrag.MbrG40
                    kmh50 = eintrag.MbrG50
                Case "P"
                    kmh20 = eintrag.MbrP20
                    kmh30 = eintrag.MbrP30
                    kmh40 = eintrag.MbrP40
                    kmh50 = eintrag.MbrP50
                Case Else
                    TBHinweis.Text = "Keine Bremsstellung ausgewählt!"
                    Exit Sub
            End Select

            If eintrag.Nachbar IsNot Nothing Then
                If VorhandenBrH >= kmh50 Then
                    eintrag.Geschwindigkeit = 50
                    eintrag.Geschwindigkeit_Soll = 50
                ElseIf VorhandenBrH >= kmh40 Then
                    eintrag.Geschwindigkeit = 40
                    eintrag.Geschwindigkeit_Soll = 40
                ElseIf VorhandenBrH >= kmh30 Then
                    eintrag.Geschwindigkeit = 30
                    eintrag.Geschwindigkeit_Soll = 30
                ElseIf VorhandenBrH >= kmh20 Then
                    eintrag.Geschwindigkeit = 20
                    eintrag.Geschwindigkeit_Soll = 20
                Else
                    eintrag.Geschwindigkeit = 20
                    eintrag.Geschwindigkeit_Soll = 20
                End If
            Else
                eintrag.Geschwindigkeit = 20
                eintrag.Geschwindigkeit_Soll = 20
            End If
            AusgabeBerechnen()
        Next
    End Sub

    Public Sub AusgabeBerechnen(Optional beschleunigungsRate As Double = 0.5, Optional bremsRate As Double = 0.7)
        If Laufweg Is Nothing OrElse Laufweg.Count = 0 Then
            TBHinweis.Text = "Kein Laufweg vorhanden!"
            Return
        End If

        Dim aktuelleVmax As Integer = 0

        For Each eintrag In Laufweg
            If aktuelleVmax <> eintrag.Geschwindigkeit_Soll Then
                eintrag.Ausgeben = True
                aktuelleVmax = eintrag.Geschwindigkeit_Soll
            Else
                eintrag.Ausgeben = False
            End If
        Next

        Dim abfahrtsZeit As DateTime
        Try
            abfahrtsZeit = DateTime.Parse(DP_Datum.DisplayDate.ToString("dd.MM.yyyy") & " " & TB_Abfahrt.Text)
        Catch ex As Exception
            TBHinweis.Text = "Ungültige Abfahrtszeit!"
            Return
        End Try

        Dim letzteKm As Double = Laufweg(0).km
        Dim letzteV As Double = 0
        Dim aktuelleZeit As DateTime = abfahrtsZeit

        For i As Integer = 0 To Laufweg.Count - 1
            If i = 0 Then
                Laufweg(i).Abfahrt = aktuelleZeit
                letzteV = Laufweg(i).Geschwindigkeit_Soll
                StartEintrag = Laufweg(0)
                Continue For
            ElseIf i = Laufweg.Count - 1 Then
                ZielEintrag = Laufweg(i)
            End If

            Dim abschnitt As C_Bremswegeintrag = Laufweg(i)
            Dim strecke As Double = abschnitt.km - letzteKm
            If strecke < 0 Then strecke = strecke * -1
            letzteKm = abschnitt.km

            Dim vStart As Double = letzteV
            Dim vEnde As Double = abschnitt.Geschwindigkeit_Soll
            If vEnde <= 0 Then vEnde = 1

            ' Zeit für konstante Fahrt
            Dim zeitAbschnittStunden As Double = strecke / vEnde
            Dim zeitAbschnittMinuten As Double = zeitAbschnittStunden * 60

            ' Zeit für Beschleunigung/Bremsung
            Dim deltaV As Double = vEnde - vStart
            Dim beschleunigungsZeit As Double = 0
            If deltaV > 0 And beschleunigungsRate > 0 Then
                beschleunigungsZeit = (deltaV * 1000 / 3600) / beschleunigungsRate / 60 ' in Minuten
            ElseIf deltaV < 0 And bremsRate > 0 Then
                beschleunigungsZeit = (Math.Abs(deltaV) * 1000 / 3600) / bremsRate / 60 ' in Minuten
            End If

            zeitAbschnittMinuten += beschleunigungsZeit

            ' Zeitzuschlag von 10 %
            zeitAbschnittMinuten *= 1.1

            aktuelleZeit = aktuelleZeit.AddMinutes(zeitAbschnittMinuten)
            If i = Laufweg.Count - 1 Then
                abschnitt.Ankunft = aktuelleZeit
            Else
                abschnitt.Abfahrt = aktuelleZeit
            End If

            letzteV = vEnde
        Next

        ZeichneGeschwindigkeitsprofil()


    End Sub
    Private Sub BTN_Brechnen_Click(sender As Object, e As RoutedEventArgs)
        AusgabeBerechnen()
    End Sub

    Private Sub BTN_Export_XLS_Click(sender As Object, e As RoutedEventArgs)
        Dim dlg As New Microsoft.Win32.SaveFileDialog()
        dlg.Filter = "Excel-Datei (*.xlsx)|*.xlsx"
        dlg.FileName = $"{DP_Datum.DisplayDate.ToString("yyyy_dd_MM")}_{TB_Zugnummer.Text}.xlsx"
        If dlg.ShowDialog() <> True Then Return
        Try
            Using wb As New XLWorkbook()
                Dim ws = wb.Worksheets.Add("Fahrplan")
                ws.Cell(1, 1).Value = "Datum:"
                ws.Cell(1, 2).Value = DP_Datum.DisplayDate.ToString("dd.MM.yyyy")
                ws.Cell(2, 1).Value = "Zugnummer:"
                ws.Cell(2, 2).Value = TB_Zugnummer.Text
                ws.Cell(3, 1).Value = "Länge:"
                ws.Cell(3, 2).Value = TB_Laenge.Text & " m"
                ws.Cell(4, 1).Value = "Gewicht:"
                ws.Cell(4, 2).Value = TB_Gewicht.Text & " t"
                ws.Cell(5, 1).Value = "Bremsstellung:"
                ws.Cell(5, 2).Value = CB_Bremsstellung.Text
                ws.Cell(6, 1).Value = "BrH:"
                ws.Cell(6, 2).Value = TB_Mbr.Text
                ws.Cell(7, 1).Value = "Fahrzeug(e):"
                ws.Cell(7, 2).Value = TB_Fahrzeuge.Text

                Dim row = 9

                ws.Cell(row, 1).Value = "Geschwindigkeit"
                ws.Cell(row, 2).Value = "km"
                ws.Cell(row, 3).Value = "Betriebsstelle"
                ws.Cell(row, 4).Value = "Ankunft"
                ws.Cell(row, 5).Value = "Abfahrt"
                ws.Range(row, 1, row, 5).Style.Font.Bold = True
                ws.Range(row, 1, row, 5).Style.Border.OutsideBorder = XLBorderStyleValues.Thin
                ws.Range(row, 1, row, 5).Style.Border.InsideBorder = XLBorderStyleValues.Thin
                ws.Range(row, 1, row, 5).Style.Border.OutsideBorderColor = XLColor.Black
                ws.Range(row, 1, row, 5).Style.Border.InsideBorderColor = XLColor.Black

                row += 1

                For Each eintrag In Listata
                    If eintrag.Ausgeben = False Then Continue For
                    ws.Cell(row, 1).Value = eintrag.Geschwindigkeit_Soll & " km/h"
                    ws.Cell(row, 2).Value = eintrag.km.ToString("N1")
                    ws.Cell(row, 3).Value = eintrag.Betriebsstelle & " - " & eintrag.Signalbezeichnung

                    If eintrag.Ankunft <> Date.MinValue Then
                        ws.Cell(row, 4).Value = eintrag.Ankunft.ToString("HH:mm")
                    End If
                    If eintrag.Abfahrt <> Date.MinValue Then
                        ws.Cell(row, 5).Value = eintrag.Abfahrt.ToString("HH:mm")
                    End If

                    ' Rahmen um die Zellen ziehen
                    ws.Range(row, 1, row, 5).Style.Border.OutsideBorder = XLBorderStyleValues.Thin
                    ws.Range(row, 1, row, 5).Style.Border.InsideBorder = XLBorderStyleValues.Thin
                    ws.Range(row, 1, row, 5).Style.Border.OutsideBorderColor = XLColor.Black
                    ws.Range(row, 1, row, 5).Style.Border.InsideBorderColor = XLColor.Black
                    row += 1
                Next

                ws.Columns().AdjustToContents()
                wb.SaveAs(dlg.FileName)
            End Using
        Catch ex As Exception
            MsgBox(ex.Message, MsgBoxStyle.Critical, "Fehler beim Export")
            TBHinweis.Text = "Fehler beim Export"
            Exit Sub
        End Try


        TBHinweis.Text = "Export erfolgreich: " & dlg.FileName
        ' Excel-Datei öffnen
        Try
            Process.Start(New ProcessStartInfo(dlg.FileName) With {.UseShellExecute = True})
        Catch ex As Exception
            TBHinweis.Text &= vbCrLf & "Datei konnte nicht geöffnet werden: " & ex.Message
        End Try

    End Sub
    Public Sub ZeichneGeschwindigkeitsprofil()
        Try
            Canvas_Fahrtverlauf.Children.Clear()

            If Laufweg Is Nothing OrElse Laufweg.Count < 2 Then Return

            ' Wertebereich bestimmen
            Dim minKm = Laufweg.Min(Function(e) e.km)
            Dim maxKm = Laufweg.Max(Function(e) e.km)
            Dim minV = 0
            Dim maxV = Laufweg.Max(Function(e) e.Geschwindigkeit_Soll)

            Dim margin As Double = 50
            Dim width = Canvas_Fahrtverlauf.ActualWidth
            Dim height = Canvas_Fahrtverlauf.ActualHeight

            If width = 0 Or height = 0 Then
                width = Canvas_Fahrtverlauf.Width
                height = Canvas_Fahrtverlauf.Height
            End If
            If width = 0 Or height = 0 Then Return

            ' Zeichenfläche abzüglich Rand
            Dim plotWidth = width - 2 * margin
            Dim plotHeight = height - 2 * margin

            Dim kmRange = maxKm - minKm
            Dim vRange = maxV - minV
            If kmRange = 0 Or vRange = 0 Then Return

            ' Hilfslinien für 20, 30, 40, 50 km/h
            Dim geschwindigkeiten() As Integer = {20, 30, 40, 50}
            For Each v In geschwindigkeiten
                If v > maxV Then Continue For
                Dim y = margin + plotHeight - ((v - minV) / vRange) * plotHeight
                Dim line As New System.Windows.Shapes.Line()
                line.X1 = margin
                line.Y1 = y
                line.X2 = margin + plotWidth
                line.Y2 = y
                line.Stroke = System.Windows.Media.Brushes.LightGray
                line.StrokeThickness = 1
                line.StrokeDashArray = New System.Windows.Media.DoubleCollection() From {2, 2}
                Canvas_Fahrtverlauf.Children.Add(line)

                Dim txtV As New System.Windows.Controls.TextBlock()
                txtV.Text = v.ToString() & " km/h"
                txtV.FontSize = 11
                txtV.Foreground = System.Windows.Media.Brushes.Gray
                Canvas.SetLeft(txtV, 0)
                Canvas.SetTop(txtV, y - 10)
                Canvas_Fahrtverlauf.Children.Add(txtV)
            Next

            ' Treppenprofil zeichnen
            Dim lastX As Double = margin + ((Laufweg(0).km - minKm) / kmRange) * plotWidth
            Dim lastY As Double = margin + plotHeight - ((Laufweg(0).Geschwindigkeit_Soll - minV) / vRange) * plotHeight

            For i As Integer = 1 To Laufweg.Count - 1
                Dim eintrag = Laufweg(i)
                Dim x = margin + ((eintrag.km - minKm) / kmRange) * plotWidth
                Dim y = margin + plotHeight - ((eintrag.Geschwindigkeit_Soll - minV) / vRange) * plotHeight

                ' Horizontale Linie bis zum nächsten km
                Dim lineH As New System.Windows.Shapes.Line()
                lineH.X1 = lastX
                lineH.Y1 = lastY
                lineH.X2 = x
                lineH.Y2 = lastY
                lineH.Stroke = System.Windows.Media.Brushes.Blue
                lineH.StrokeThickness = 2
                Canvas_Fahrtverlauf.Children.Add(lineH)

                ' Senkrechte Linie zum neuen Geschwindigkeitswert (falls Geschwindigkeit sich ändert)
                If lastY <> y Then
                    Dim lineV As New System.Windows.Shapes.Line()
                    lineV.X1 = x
                    lineV.Y1 = lastY
                    lineV.X2 = x
                    lineV.Y2 = y
                    lineV.Stroke = System.Windows.Media.Brushes.Blue
                    lineV.StrokeThickness = 2
                    Canvas_Fahrtverlauf.Children.Add(lineV)
                End If
                lastX = x
                lastY = y
            Next


            ' Signalbezeichnungen und gestrichelte Linien unterhalb des Diagramms zeichnen
            For Each eintrag In Laufweg
                Dim x = margin + ((eintrag.km - minKm) / kmRange) * plotWidth
                Dim yProfil = margin + plotHeight - ((eintrag.Geschwindigkeit_Soll - minV) / vRange) * plotHeight
                Dim yText = margin + plotHeight + 5 ' 5 Pixel unterhalb des Diagramms

                ' 90 Grad gedrehte Signalbezeichnung
                Dim txt As New System.Windows.Controls.TextBlock()
                txt.Text = eintrag.Signalbezeichnung
                txt.FontSize = 12
                txt.Foreground = System.Windows.Media.Brushes.Black
                txt.TextAlignment = TextAlignment.Center
                txt.RenderTransform = New System.Windows.Media.RotateTransform(90)
                txt.RenderTransformOrigin = New System.Windows.Point(0, 0)
                Canvas.SetLeft(txt, x + 8)
                Canvas.SetTop(txt, yText)
                Canvas_Fahrtverlauf.Children.Add(txt)

                ' Gestrichelte Linie von Text zur Profil-Linie
                Dim dashedLine As New System.Windows.Shapes.Line()
                dashedLine.X1 = x + txt.ActualHeight / 2 ' Korrektur für Drehung, ggf. anpassen
                dashedLine.Y1 = yText - 10
                dashedLine.X2 = x
                dashedLine.Y2 = yProfil
                dashedLine.Stroke = System.Windows.Media.Brushes.DarkGray
                dashedLine.StrokeThickness = 1
                dashedLine.StrokeDashArray = New System.Windows.Media.DoubleCollection() From {3, 3}
                Canvas_Fahrtverlauf.Children.Add(dashedLine)
            Next


        Catch ex As Exception

        End Try
    End Sub

    Public Sub ZeichneStrecke()
        Canvas_Strecke.Children.Clear()

        ' Zuerst alle Linien zeichnen (Hintergrund)
        For i As Integer = 0 To Listata.Count - 2
            Dim p = Listata(i)

            If p.Nachbar IsNot Nothing Then
                If (p.PositionX <> 0 Or p.PositionY <> 0) And (p.Nachbar.PositionX <> 0 Or p.Nachbar.PositionY <> 0) Then
                    Dim line As New System.Windows.Shapes.Line()
                    line.X1 = p.PositionX + 10 ' Mittelpunkt der Ellipse
                    line.Y1 = p.PositionY + 10
                    line.X2 = p.Nachbar.PositionX + 10
                    line.Y2 = p.Nachbar.PositionY + 10
                    line.Stroke = System.Windows.Media.Brushes.Black
                    line.StrokeThickness = 2
                    Canvas_Strecke.Children.Add(line)
                End If
            End If
            If p.Nachbar2 IsNot Nothing Then
                If (p.PositionX <> 0 Or p.PositionY <> 0) And (p.Nachbar2.PositionX <> 0 Or p.Nachbar2.PositionY <> 0) Then
                    Dim line As New System.Windows.Shapes.Line()
                    line.X1 = p.PositionX + 10 ' Mittelpunkt der Ellipse
                    line.Y1 = p.PositionY + 10
                    line.X2 = p.Nachbar2.PositionX + 10
                    line.Y2 = p.Nachbar2.PositionY + 10
                    line.Stroke = System.Windows.Media.Brushes.DimGray
                    line.StrokeThickness = 2
                    Canvas_Strecke.Children.Add(line)
                End If
            End If

        Next

        ' Danach alle Ellipsen und TextBlöcke zeichnen (Vordergrund)
        For i As Integer = 0 To Listata.Count - 1
            Dim p = Listata(i)
            If p.PositionX = 0 And p.PositionY = 0 Then
                'Continue For
            End If
            Dim ellipse As New System.Windows.Shapes.Ellipse()
            ellipse.Width = 20
            ellipse.Height = 20
            ellipse.Fill = System.Windows.Media.Brushes.White
            ellipse.Stroke = System.Windows.Media.Brushes.LightGray

            If Laufweg.Contains(p) Then
                If Laufweg.IndexOf(p) = 0 Then
                    ellipse.Fill = System.Windows.Media.Brushes.LightGreen
                ElseIf Laufweg.IndexOf(p) = Laufweg.Count - 1 Then
                    ellipse.Fill = System.Windows.Media.Brushes.Orange
                Else
                    ellipse.Fill = System.Windows.Media.Brushes.LightGray
                End If
            End If
            If LaufwegTemp.Contains(p) Then
                If LaufwegTemp.IndexOf(p) = 0 Then
                    ellipse.Stroke = System.Windows.Media.Brushes.LightGreen
                ElseIf LaufwegTemp.IndexOf(p) = LaufwegTemp.Count - 1 Then
                    ellipse.Stroke = System.Windows.Media.Brushes.Orange
                Else
                    ellipse.Stroke = System.Windows.Media.Brushes.Black
                End If
            End If
            ellipse.StrokeThickness = 3

            ellipse.ToolTip = $"ID: {p.ID} N1:{p.NachbarID} N2:{p.Nachbar2ID}"

            Canvas.SetLeft(ellipse, p.PositionX)
            Canvas.SetTop(ellipse, p.PositionY)
            Canvas_Strecke.Children.Add(ellipse)

            ' Drag-Events hinzufügen
            AddHandler ellipse.MouseLeftButtonDown, Sub(senderEllipse, e)
                                                        If Edit = True Then
                                                            draggedEllipse = CType(senderEllipse, System.Windows.Shapes.Ellipse)
                                                            draggedEntry = p
                                                            mouseOffset = e.GetPosition(Canvas_Strecke)
                                                            mouseOffset.X -= p.PositionX
                                                            mouseOffset.Y -= p.PositionY
                                                            draggedEllipse.CaptureMouse()
                                                        Else
                                                            If StartEintrag Is Nothing Then
                                                                ellipse.Fill = System.Windows.Media.Brushes.LightGreen
                                                                StartEintrag = p
                                                                TBHinweis.Text = "Start gesetzt: " & p.Signalbezeichnung
                                                                ' CB_Start.SelectedValue = StartEintrag
                                                                LaufwegTemp.Clear()
                                                                Berechne_Laufweg()
                                                            ElseIf StartEintrag Is p Then
                                                                ellipse.Fill = System.Windows.Media.Brushes.White
                                                                StartEintrag = Nothing
                                                                ZielEintrag = Nothing
                                                                ' CB_Start.SelectedValue = Nothing
                                                                ' CB_Ziel.SelectedValue = Nothing
                                                                LaufwegTemp.Clear()
                                                                ZeichneStrecke()
                                                            ElseIf StartEintrag IsNot Nothing And ZielEintrag Is Nothing Then
                                                                ellipse.Fill = System.Windows.Media.Brushes.Orange
                                                                ZielEintrag = p
                                                                TBHinweis.Text = "Ziel gesetzt: " & p.Signalbezeichnung
                                                                ' CB_Start.SelectedValue = StartEintrag
                                                                ' CB_Ziel.SelectedValue = ZielEintrag
                                                                Berechne_Laufweg()
                                                            ElseIf StartEintrag IsNot Nothing And ZielEintrag IsNot Nothing Then
                                                                ellipse.Fill = System.Windows.Media.Brushes.White
                                                                StartEintrag = Nothing
                                                                ' CB_Start.SelectedValue = Nothing
                                                                ZielEintrag = Nothing
                                                                '  CB_Ziel.SelectedValue = Nothing
                                                                LaufwegTemp.Clear()
                                                                ZeichneStrecke()
                                                            End If
                                                        End If
                                                    End Sub
            AddHandler ellipse.MouseMove, Sub(senderEllipse, e)
                                              If draggedEllipse IsNot Nothing AndAlso draggedEntry IsNot Nothing AndAlso e.LeftButton = MouseButtonState.Pressed Then
                                                  Dim pos = e.GetPosition(Canvas_Strecke)
                                                  Dim newX = pos.X - mouseOffset.X
                                                  Dim newY = pos.Y - mouseOffset.Y
                                                  draggedEntry.PositionX = CInt(newX)
                                                  draggedEntry.PositionY = CInt(newY)
                                                  Canvas.SetLeft(draggedEllipse, draggedEntry.PositionX)
                                                  Canvas.SetTop(draggedEllipse, draggedEntry.PositionY)
                                                  ZeichneStrecke() ' Canvas neu zeichnen, damit Linien und Text passen
                                              End If
                                          End Sub

            AddHandler ellipse.MouseLeftButtonUp, Sub(senderEllipse, e)
                                                      If draggedEllipse IsNot Nothing Then
                                                          draggedEntry.PositionX = CInt(Math.Ceiling(draggedEntry.PositionX / 10) * 10)
                                                          draggedEntry.PositionY = CInt(Math.Ceiling(draggedEntry.PositionY / 10) * 10)
                                                          Canvas.SetLeft(draggedEllipse, draggedEntry.PositionX)
                                                          Canvas.SetTop(draggedEllipse, draggedEntry.PositionY)
                                                          draggedEllipse.ReleaseMouseCapture()
                                                          draggedEllipse = Nothing
                                                          draggedEntry = Nothing
                                                          ZeichneStrecke()
                                                      End If
                                                  End Sub

            Dim TBBezeichung As New TextBlock
            TBBezeichung.Text = p.Signalbezeichnung
            TBBezeichung.FontSize = 8
            TBBezeichung.TextAlignment = TextAlignment.Center
            Canvas.SetLeft(TBBezeichung, p.PositionX + 5)
            Canvas.SetTop(TBBezeichung, p.PositionY + 18)
            Canvas_Strecke.Children.Add(TBBezeichung)
        Next
    End Sub

    Private Sub TB_XLS_Input_TextChanged(sender As Object, e As TextChangedEventArgs)
        Try
            Dim input As String = TB_XLS_Input.Text
            Dim XIndex As Integer = TB_XLS_Startindex.Text

            Dim lines = input.Split({vbCrLf, vbLf}, StringSplitOptions.RemoveEmptyEntries)
            Dim xmlRoot As New XElement("Streckendaten")

            For i As Integer = 0 To lines.Length - 1
                Dim line = lines(i)
                Dim cols = line.Split(vbTab)

                If cols.Length = 3 Then 'Streckenbeginn
                    Dim eintrag1 As New XElement("C_Bremswegeintrag",
                   New XElement("Streckennummer", TB_XLS_Streckennummer.Text),
                   New XElement("Betriebsstelle", cols(0)),
                   New XElement("Typ", cols(1).Trim()),
                   New XElement("ID", TB_XLS_Streckennummer.Text & "-" & XIndex.ToString),
                   New XElement("NachbarID", TB_XLS_Streckennummer.Text & "-" & XIndex + 1.ToString),
                   New XElement("Signalbezeichnung", "Beginn"),
                   New XElement("km", cols(2)),
                   New XElement("Neigung", "0"),
                   New XElement("MbrG20", "0"),
                   New XElement("MbrG30", "0"),
                   New XElement("MbrG40", "0"),
                   New XElement("MbrG50", "0"),
                   New XElement("MbrP20", "0"),
                   New XElement("MbrP30", "0"),
                   New XElement("MbrP40", "0"),
                   New XElement("MbrP50", "0"))
                    XIndex += 1
                    xmlRoot.Add(eintrag1)
                    Continue For
                End If

                If cols.Length < 13 Then Continue For

                Dim eintrag As New XElement("C_Bremswegeintrag",
                    New XElement("Streckennummer", TB_XLS_Streckennummer.Text),
                    New XElement("Betriebsstelle", cols(0)),
                    New XElement("Typ", cols(1).Trim()),
                    New XElement("ID", TB_XLS_Streckennummer.Text & "-" & XIndex.ToString),
                    New XElement("NachbarID", TB_XLS_Streckennummer.Text & "-" & XIndex + 1.ToString),
                    New XElement("Signalbezeichnung", cols(2)),
                    New XElement("km", cols(3).Trim()),
                    New XElement("Neigung", cols(5).Trim()),
                    New XElement("MbrG20", cols(6).Trim()),
                    New XElement("MbrG30", cols(7).Trim()),
                    New XElement("MbrG40", cols(8).Trim()),
                    New XElement("MbrG50", cols(9).Trim()),
                    New XElement("MbrP20", cols(10).Trim()),
                    New XElement("MbrP30", cols(11).Trim()),
                    New XElement("MbrP40", cols(12).Trim()),
                    New XElement("MbrP50", cols(13).Trim()))

                xmlRoot.Add(eintrag)
                XIndex += 1
            Next

            Dim ausgabe As String = xmlRoot.ToString()

            ausgabe = ausgabe.Replace("<Streckendaten>", "")
            ausgabe = ausgabe.Replace("<Streckendaten/>", "")
            ausgabe = ausgabe.Replace("<Streckendaten />", "")
            ausgabe = ausgabe.Replace("</Streckendaten>", "")

            TB_XML_Output.Text = ausgabe
        Catch ex As Exception
            TB_XML_Output.Text = "Fehler beim parsen." & vbLf & ex.ToString
        End Try

    End Sub

    Private Sub BTN_XML_Speichern_Click(sender As Object, e As RoutedEventArgs)
        Dim filePath As String = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "streckendaten.xml")
        Dim xmlRoot As New XElement("Streckendaten")
        For Each item In Listata
            xmlRoot.Add(New XElement("C_Bremswegeintrag",
                New XElement("Streckennummer", item.Streckennummer),
                New XElement("Betriebsstelle", item.Betriebsstelle),
                New XElement("Typ", item.Typ.ToString()),
                New XElement("ID", item.ID),
                New XElement("NachbarID", item.NachbarID),
                New XElement("Nachbar2ID", item.Nachbar2ID),
                New XElement("Signalbezeichnung", item.Signalbezeichnung),
                New XElement("km", item.km.ToString.Replace(".", ",")),
                New XElement("Neigung", item.Neigung.ToString.Replace(".", ",")),
                New XElement("MbrG20", item.MbrG20),
                New XElement("MbrG30", item.MbrG30),
                New XElement("MbrG40", item.MbrG40),
                New XElement("MbrG50", item.MbrG50),
                New XElement("MbrP20", item.MbrP20),
                New XElement("MbrP30", item.MbrP30),
                New XElement("MbrP40", item.MbrP40),
                New XElement("MbrP50", item.MbrP50),
                New XElement("PositionX", item.PositionX),
                New XElement("PositionY", item.PositionY)
            ))
        Next
        xmlRoot.Save(filePath)
        TBHinweis.Text = Now.ToString & " XML gespeichert!"
        ZeichneStrecke()
    End Sub

    Private Sub BTN_Edit_Click(sender As Object, e As RoutedEventArgs)
        If CB_EditModus.IsChecked Then
            CB_EditModus.IsChecked = False
            Edit = False
        Else
            CB_EditModus.IsChecked = True
            Edit = True
        End If
    End Sub

    Private Sub BTN_Beenden_Click(sender As Object, e As RoutedEventArgs)
        End
    End Sub

    Private Sub Canvas_Strecke_MouseWheel(sender As Object, e As MouseWheelEventArgs)
        Dim scaleStep As Double = 0.1
        Dim oldScale As Double = StreckeScale.ScaleX
        Dim newScale As Double = If(e.Delta > 0, oldScale + scaleStep, Math.Max(0.1, oldScale - scaleStep))
        StreckeScale.ScaleX = newScale
        StreckeScale.ScaleY = newScale

        ' Zoom auf Mausposition
        Dim mousePos = e.GetPosition(Canvas_Strecke)
        StreckeTranslate.X = (StreckeTranslate.X - mousePos.X) * (newScale / oldScale) + mousePos.X
        StreckeTranslate.Y = (StreckeTranslate.Y - mousePos.Y) * (newScale / oldScale) + mousePos.Y

    End Sub

    Private Sub Canvas_Strecke_MouseDown(sender As Object, e As MouseButtonEventArgs)
        If e.MiddleButton = MouseButtonState.Pressed Then
            isPanning = True
            panStart = e.GetPosition(Me)
            Canvas_Strecke.CaptureMouse()
        End If
    End Sub

    Private Sub Canvas_Strecke_MouseMove(sender As Object, e As MouseEventArgs)
        If isPanning AndAlso e.MiddleButton = MouseButtonState.Pressed Then
            Dim pos = e.GetPosition(Me)
            Dim dx = pos.X - panStart.X
            Dim dy = pos.Y - panStart.Y
            StreckeTranslate.X += dx
            StreckeTranslate.Y += dy
            panStart = pos
        End If
    End Sub

    Private Sub Canvas_Strecke_MouseUp(sender As Object, e As MouseButtonEventArgs)
        If isPanning Then
            isPanning = False
            Canvas_Strecke.ReleaseMouseCapture()
        End If
    End Sub

    Private Sub BTN_Hinzufuegen_Click(sender As Object, e As RoutedEventArgs)
        For Each eintrag In LaufwegTemp
            Laufweg.Add(eintrag)
        Next
        LaufwegTemp.Clear()
        StartEintrag = Nothing
        ZielEintrag = Nothing
        ZeichneStrecke()
    End Sub

    Private Sub BTN_Löschen_Click(sender As Object, e As RoutedEventArgs)
        LaufwegTemp.Clear()
        Laufweg.Clear()
        StartEintrag = Nothing
        ZielEintrag = Nothing

        ZeichneStrecke()
    End Sub
End Class
