using System;
using System.Net;
using System.Net.Mail;

public static class EmailUtility
{
    public static void EnvoyerEmail(string destinataire, string sujet, string message, bool isHtml)
    {
        try
        {
            var smtpClient = new SmtpClient("smtp.gmail.com")
            {
                Port = 587,
                Credentials = new NetworkCredential("houdafaty86@gmail.com", "ludp rfsk xwuk sglq"),
                EnableSsl = true,
            };

            var mailMessage = new MailMessage
            {
                From = new MailAddress("houdafaty86@gmail.com"),
                Subject = sujet,
                Body = message,
                IsBodyHtml = isHtml,  // Ici, mettez isHtml à true pour envoyer du contenu HTML
            };
            mailMessage.To.Add(destinataire);

            smtpClient.Send(mailMessage);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Erreur d'envoi d'email : {ex.Message}");
        }
    }

}
