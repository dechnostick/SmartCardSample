Imports System.Text
Imports GemCard

Public Class Form1

    Private reader As String
    Private text As String
    Protected Friend WithEvents smartCard As CardBase

    Private Sub Form1_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load
        smartCard = New CardNative()
        reader = smartCard.ListReaders()(0)
        smartCard.StartCardEvents(reader)
        Msg("Reader: " & reader)
        Timer1.Start()
    End Sub

    Private Sub smartCard_OnCardInserted() Handles smartCard.OnCardInserted
        text = ""
        Msg("カード読み取り中")
        Try
            smartCard.Connect(reader, SHARE.Shared, PROTOCOL.T0orT1)
        Catch ex As Exception
            Msg("接続失敗")
        End Try

        Try
            Dim atrValue As Byte() = smartCard.GetAttribute(SCARD_ATTR_VALUE.ATR_STRING)
            Msg(ByteArrayToString(atrValue) & "(winscard:ATR)")
            Msg(ToIDm(atrValue) & "(winscard:IDm)")
        Catch ex As Exception
            Msg("読み取り失敗")
        End Try

        Try
            smartCard.Disconnect(DISCONNECT.Unpower)
        Catch ex As Exception
            Msg("切断失敗")
        End Try
    End Sub

    Private Sub smartCard_OnCardRemoved() Handles smartCard.OnCardRemoved
        Msg("カード抜き取り完了")
    End Sub

    Private Shared Function ByteArrayToString(ByVal data As Byte()) As String
        Dim sb As New StringBuilder
        For i As Integer = 0 To data.Length - 1
            sb.AppendFormat("{0:X02}", data(i))
        Next
        Return sb.ToString()
    End Function

    Private Shared Function ToIDm(ByVal data As Byte()) As String
        Dim sb As New StringBuilder
        ' 3B 8C 80 01 04 43 FD
        If data.Length = 17 Then
            For nI As Integer = 7 To 14
                sb.AppendFormat("{0:X02}", data(nI))
            Next
        End If
        Return sb.ToString()
    End Function

    Private Sub Msg(str As String)
        text &= str & vbNewLine
    End Sub

    Private Sub Form1_FormClosing(ByVal sender As System.Object, ByVal e As System.Windows.Forms.FormClosingEventArgs) Handles MyBase.FormClosing
        smartCard.Disconnect(DISCONNECT.Unpower)
        smartCard.StopCardEvents()
        e.Cancel = False
    End Sub

    Private Sub Timer1_Tick(sender As Object, e As EventArgs) Handles Timer1.Tick
        TextBox1.Text = text
    End Sub
End Class
