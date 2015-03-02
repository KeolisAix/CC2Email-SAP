Imports System.Net.Mail
Imports System.Text

Public Class Form1
    Dim Donnees = "D:\COMPTAGE\DONNEES_COMPTAGE\" 'Variable de des données
    Dim Evenement = "D:\COMPTAGE\EVENEMENT\DOSSIER_ALARME_PATRICE\VIDAGE_BUS_2.csv" 'Variable  du Vidage

    Private Sub Form1_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        Dim i As Integer
        For i = 0 To My.Computer.FileSystem.GetFiles(Donnees).Count - 1
            TextBox3.AppendText((My.Computer.FileSystem.GetFiles(Donnees).Item(i)) & Environment.NewLine)
        Next i
        Dim DateDuJour As DateTime = Date.Now
        DateDuJour = DateDuJour.AddDays(-1)
        Dim format As String = "yyMMdd"
        Dim IlExisteDepot1 As String
        Dim FichierDuJour As String = "4_" + DateDuJour.ToString(format) + ".csv"
        If My.Computer.FileSystem.FileExists(Donnees + FichierDuJour.ToString) Then
            IlExisteDepot1 = "Oui"
        Else
            IlExisteDepot1 = "Non"
        End If
        If My.Computer.FileSystem.FileExists(Evenement) Then My.Computer.FileSystem.DeleteFile(Evenement)
        Try
            My.Computer.FileSystem.CopyFile("D:\COMPTAGE\EVENEMENT\VIDAGE_BUS.csv", Evenement)
        Catch supp As Exception
            MsgBox("Impossible à copier : " + supp.ToString)
        End Try
        Dim pb As String
        Dim pb2 As String
        Dim TotalNoDonnees As Integer = 0
        Dim LignePlus10 As String = ""
        Dim LignePasInfo As String = ""
        Try
            pb2 = "Non"
            pb = "Non"
            For Each line As String In System.IO.File.ReadAllLines(Evenement)
                Dim word = line.Split(";")
                DataGridView1.Rows.Add(word(0), word(1), word(2), word(3), word(4))

                If Val(word(2)) >= "5" And word(2) <> "Nb jours sans vidage" Then
                    pb = "Oui"
                    LignePlus10 = LignePlus10 + "Le BUS : <b>" + word(0) + "</b> n'a pas vidé depuis le <b>" + word(1) + "</b> soit <b>" + word(2) + "</b> jours.<br>"
                End If
                If Val(word(2)) = Nothing And word(2) <> "Nb jours sans vidage" And word(2) <> "0" Then
                    pb2 = "Oui"
                    LignePasInfo = LignePasInfo + "Le BUS : <b>" + word(0) + "</b> n'a pas de données ! <br>"
                    TotalNoDonnees = TotalNoDonnees + 1
                End If
            Next
        Catch ex As Exception
            pb = "<h2>Probleme avec le fichier Vidage</h2>"
        End Try
        If pb <> "Oui" Then pb = "Non"
        Dim sb As New StringBuilder
        sb.AppendLine("<center><img src='http://goodlogo.com/images/logos/sap_logo_2789.gif'><br><br>Date du rapport : <b>" + Date.Today + "</b> <br>Existance du Fichier du jour pour le DEPOT SAP : <b>" + IlExisteDepot1.ToString + "</b> <br>Plus de 5 jours sans vidage : <b>" + pb.ToString + "</b> <br>Absence de données (Total : " + TotalNoDonnees.ToString + ") : <b>" + pb2.ToString + "</b><br></center>")
        Try
            Dim Smtp_Server As New SmtpClient
            Dim e_mail As New MailMessage()
            Smtp_Server.UseDefaultCredentials = False
            Smtp_Server.Credentials = New Net.NetworkCredential("cellulescompteusessap@gmail.com", "KeolisSAP")
            Smtp_Server.Port = 587
            Smtp_Server.EnableSsl = True
            Smtp_Server.Host = "smtp.gmail.com"

            e_mail = New MailMessage()
            e_mail.From = New MailAddress("CellulesCompteuses@keolis.com")
            e_mail.To.Add("patrice.maldi@keolis.com")
            e_mail.CC.Add("sylvain.mennillo@keolis.com")
            e_mail.CC.Add("sylvain.pitot@keolis.com")
            e_mail.CC.Add("marine.cendrier@keolis.com")
            e_mail.CC.Add("julien.agnes@keolis.com")
            e_mail.Subject = "Recap Cellules Compteuses SAP du " + Date.Now + ""
            e_mail.IsBodyHtml = True

            If IlExisteDepot1 = "Oui" Then e_mail.Attachments.Add(New Attachment(Donnees + FichierDuJour.ToString))
            If pb = "Oui" Or pb2 = "Oui" Then
                e_mail.Attachments.Add(New Attachment(Evenement))
                sb.AppendLine("<br><br><center>" + LignePlus10 + "</center><br>")
                sb.AppendLine("<br><br><center>" + LignePasInfo + "</center>")

            End If
            e_mail.Body = sb.ToString()
            Smtp_Server.Send(e_mail)
            Me.Dispose()

        Catch error_t As Exception
            MsgBox(error_t.ToString)
        End Try
    End Sub
End Class
