using System;
using System.Net;
using System.Net.Mail;

namespace nnunet_client
{
    /// <summary>
    /// Sends notification emails (failure or success). All parameters from config.json (smtp_* shared).
    /// </summary>
    public static class FailureNotifyMail
    {
        public static void SendIfConfigured(string jobId, string errorMessage, string datasetId, string imagesFor)
        {
            var cfg = global.appConfig;
            if (cfg == null || string.IsNullOrWhiteSpace(cfg.failure_notify_email_to))
                return;
            if (string.IsNullOrWhiteSpace(cfg.smtp_host))
                return;

            try
            {
                using (var client = new SmtpClient(cfg.smtp_host, cfg.smtp_port > 0 ? cfg.smtp_port : 587))
                {
                    client.EnableSsl = cfg.smtp_use_ssl;
                    if (!string.IsNullOrWhiteSpace(cfg.smtp_user))
                    {
                        client.Credentials = new NetworkCredential(cfg.smtp_user, cfg.smtp_password ?? "");
                    }

                    string from = string.IsNullOrWhiteSpace(cfg.smtp_from_address)
                        ? "noreply@localhost"
                        : cfg.smtp_from_address;
                    string displayName = cfg.smtp_from_display_name ?? "nnUNet Submit Dataset";

                    var mail = new MailMessage
                    {
                        From = new MailAddress(from, displayName),
                        Subject = $"[nnUNet Submit] Job failed: {jobId}",
                        Body = $"Job ID: {jobId}\r\n"
                             + $"Dataset: {datasetId}\r\n"
                             + $"ImagesFor: {imagesFor}\r\n"
                             + $"Time: {DateTime.Now:yyyy-MM-dd HH:mm:ss}\r\n\r\n"
                             + $"Error:\r\n{errorMessage ?? "(none)"}",
                        IsBodyHtml = false
                    };
                    mail.To.Add(cfg.failure_notify_email_to.Trim());

                    client.Send(mail);
                    helper.log($"Failure notification email sent to {cfg.failure_notify_email_to}");
                }
            }
            catch (Exception ex)
            {
                helper.log($"Failed to send failure notification email: {ex.Message}");
            }
        }

        public static void SendSuccessIfConfigured(string jobId, string datasetId, string imagesFor)
        {
            var cfg = global.appConfig;
            if (cfg == null || string.IsNullOrWhiteSpace(cfg.success_notify_email_to))
                return;
            if (string.IsNullOrWhiteSpace(cfg.smtp_host))
                return;

            try
            {
                using (var client = new SmtpClient(cfg.smtp_host, cfg.smtp_port > 0 ? cfg.smtp_port : 587))
                {
                    client.EnableSsl = cfg.smtp_use_ssl;
                    if (!string.IsNullOrWhiteSpace(cfg.smtp_user))
                    {
                        client.Credentials = new NetworkCredential(cfg.smtp_user, cfg.smtp_password ?? "");
                    }

                    string from = string.IsNullOrWhiteSpace(cfg.smtp_from_address)
                        ? "noreply@localhost"
                        : cfg.smtp_from_address;
                    string displayName = cfg.smtp_from_display_name ?? "nnUNet Submit Dataset";

                    var mail = new MailMessage
                    {
                        From = new MailAddress(from, displayName),
                        Subject = $"[nnUNet Submit] Job completed: {jobId}",
                        Body = $"Job ID: {jobId}\r\n"
                             + $"Dataset: {datasetId}\r\n"
                             + $"ImagesFor: {imagesFor}\r\n"
                             + $"Time: {DateTime.Now:yyyy-MM-dd HH:mm:ss}\r\n\r\n"
                             + "Submission completed successfully.",
                        IsBodyHtml = false
                    };
                    mail.To.Add(cfg.success_notify_email_to.Trim());

                    client.Send(mail);
                    helper.log($"Success notification email sent to {cfg.success_notify_email_to}");
                }
            }
            catch (Exception ex)
            {
                helper.log($"Failed to send success notification email: {ex.Message}");
            }
        }
    }
}
