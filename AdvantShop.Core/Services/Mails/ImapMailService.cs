using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using AdvantShop.Configuration;
using AdvantShop.Core.Services.Loging.Emails;
using AdvantShop.Core.SQL;
using AdvantShop.Diagnostics;
using MailKit;
using MailKit.Net.Imap;

namespace AdvantShop.Core.Services.Mails
{
    public class ImapMailSettings
    {
        public string Host { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }
        public int Port { get; set; }
        public bool Ssl { get; set; }
    }

    public class ImapMailService
    {
        private static ImapClient _client;
        private static ImapMailSettings _settings;

        public ImapMailService()
        {
        }

        public bool IsValid()
        {
            var settings = GetSettings();

            var isValid =
                !(string.IsNullOrWhiteSpace(settings.Host) ||
                  string.IsNullOrWhiteSpace(settings.UserName) || string.IsNullOrWhiteSpace(settings.Password));
            
            return isValid;
        }

        private ImapMailSettings GetSettings()
        {
            return new ImapMailSettings()
            {
                Host = SettingsMail.ImapHost,
                UserName = SettingsMail.Login,
                Password = SettingsMail.Password,
                Port = SettingsMail.ImapPort,
                Ssl = SettingsMail.SSL
            };
        }

        private bool IsSettingsNotChanged()
        {
            var s = GetSettings();
            return _settings.Host == s.Host && _settings.UserName == s.UserName && _settings.Password == s.Password &&
                   _settings.Port == s.Port && _settings.Ssl == s.Ssl;
        }

        private ImapClient GetClient()
        {
            if (_client != null && _client.IsAuthenticated && _client.IsConnected && _settings != null && IsSettingsNotChanged())
                return _client;

            if (_client != null)
                _client.Disconnect(true);

            _settings = GetSettings();

            _client = new ImapClient();

            // accept all SSL certificates
            _client.ServerCertificateValidationCallback = (s, c, h, e) => true;

            _client.Connect(_settings.Host, _settings.Port, _settings.Ssl);

            _client.AuthenticationMechanisms.Remove("XOAUTH2");

            _client.Authenticate(_settings.UserName, _settings.Password);

            return _client;
        }

        public List<EmailImap> GetEmails(string email)
        {
            var result = SQLDataAccess
                .Query<EmailImap>("Select * From [Customers].[ImapLetter] Where FromEmail=@email Or ToEmail=@email", new { email })
                .ToList();

            return result;
        }
        
        public EmailImap GetEmail(string uid, string folder)
        {
            if (!IsValid())
                return null;

            EmailImap email = null;

            if (string.IsNullOrEmpty(uid))
                return null;

            try
            {
                var client = GetClient();

                var id = new UniqueId(Convert.ToUInt32(uid));
                var searchInSent = false;

                var inbox = !string.IsNullOrEmpty(folder) ? TryGetFolder(client, folder) : null;
                if (inbox == null)
                {
                    inbox = TryGetFolder(client, SpecialFolder.All);
                    if (inbox == null)
                    {
                        inbox = client.Inbox;
                        searchInSent = true;
                    }
                }

                inbox.Open(FolderAccess.ReadOnly);

                var msg = inbox.GetMessage(id);
                if (msg != null)
                {
                    email = new EmailImap(id, msg, inbox.FullName);
                }
                else if (searchInSent)
                {
                    inbox = TryGetFolder(client, SpecialFolder.Sent);
                    if (inbox != null)
                    {
                        inbox.Open(FolderAccess.ReadOnly);

                        msg = inbox.GetMessage(id);
                        if (msg != null)
                            email = new EmailImap(id, msg, inbox.FullName);
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.Log.Error("GetEmailImap", ex);
            }

            return email;
        }

        /// <summary>
        /// Получаем письма с последнего uid
        /// </summary>
        public List<EmailImap> GetLastEmails()
        {
            var lastId = SettingsMail.ImapLastUpdateLetterId;

            var emails = new List<EmailImap>();

            try
            {
                var client = GetClient();
                
                var inbox = TryGetFolder(client, SpecialFolder.All) ?? client.Inbox;

                inbox.Open(FolderAccess.ReadOnly);
                
                var range = new UniqueIdRange(new UniqueId(lastId + 1), UniqueId.MaxValue);

                var messages = inbox.Fetch(range, MessageSummaryItems.UniqueId | MessageSummaryItems.Envelope | MessageSummaryItems.InternalDate);

                if (messages != null && messages.Count > 0)
                {
                    if (messages.Count > 3000)
                        messages = messages.Skip(messages.Count - 3000).ToList();
                    
                    foreach (var message in messages.Where(x => x.UniqueId.Id > lastId))
                    {
                        var mail = new EmailImap(message, inbox.FullName);

                        emails.Add(mail);

                        AddLetter(mail);
                    }

                    var maxId = messages.Max(x => x.UniqueId).Id;
                    if (maxId > lastId)
                        SettingsMail.ImapLastUpdateLetterId = maxId;
                }
            }
            catch (SocketException) { }
            catch (ImapProtocolException) { }
            catch (IOException) { }
            catch (Exception ex)
            {
                Debug.Log.Warn(ex);
            }

            return emails;
        }


        public bool TestImap()
        {
            var settings = GetSettings();
            return TestImap(settings.Host, settings.Port, settings.Ssl, settings.UserName, settings.Password);
        }

        public bool TestImap(string host, int port, bool ssl, string userName, string password)
        {
            var client = new ImapClient();

            client.ServerCertificateValidationCallback = (s, c, h, e) => true;
            client.Connect(host, port, ssl);
            client.AuthenticationMechanisms.Remove("XOAUTH2");
            client.Authenticate(userName, password);

            var inbox = TryGetFolder(client, SpecialFolder.All) ?? client.Inbox;

            inbox.Open(FolderAccess.ReadOnly);
            var msgs = inbox.Fetch(0, -1, MessageSummaryItems.UniqueId);

            return true;
        }


        private static IMailFolder TryGetFolder(ImapClient client, SpecialFolder type)
        {
            try
            {
                return client.GetFolder(type);
            }
            catch
            {
            }
            return null;
        }

        private static IMailFolder TryGetFolder(ImapClient client, string folder)
        {
            try
            {
                return client.GetFolder(folder);
            }
            catch
            {
            }
            return null;
        }


        public static void AddLetter(EmailImap letter)
        {
            SQLDataAccess.ExecuteNonQuery(
                "INSERT INTO [Customers].[ImapLetter] ([Id],[Folder],[Subject],[Date],[From],[FromEmail],[To],[ToEmail]) " +
                "VALUES (@Id,@Folder,@Subject,@Date,@From,@FromEmail,@To,@ToEmail);",
                CommandType.Text,
                new SqlParameter("@Id", letter.Id ?? ""),
                new SqlParameter("@Folder", letter.Folder ?? ""),
                new SqlParameter("@Subject", letter.Subject ?? ""),
                new SqlParameter("@Date", letter.Date),
                new SqlParameter("@From", letter.From ?? ""),
                new SqlParameter("@FromEmail", letter.FromEmail ?? ""),
                new SqlParameter("@To", letter.To ?? ""),
                new SqlParameter("@ToEmail", letter.ToEmail ?? "")
            );
        }
    }
}
