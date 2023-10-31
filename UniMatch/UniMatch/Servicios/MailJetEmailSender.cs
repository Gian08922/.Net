using Mailjet.Client;
using Mailjet.Client.Resources;
using Microsoft.AspNetCore.Identity.UI.Services;
using Newtonsoft.Json.Linq;

namespace UniMatch.Servicios
{
    public class MailJetEmailSender : IEmailSender
    {
        private readonly IConfiguration _config;
        public OpcionesMailJet _opcionesMailJet;
        public MailJetEmailSender(IConfiguration conf)
        {
            _config = conf;
        }
        public async Task SendEmailAsync(string email, string subject, string htmlMessage)
        {
            _opcionesMailJet = _config.GetSection("MailJet").Get<OpcionesMailJet>();
            MailjetClient client = new MailjetClient(_opcionesMailJet.ApiKey, _opcionesMailJet.SecretKey)
            {
                Version = ApiVersion.V3_1,
            };
            MailjetRequest request = new MailjetRequest
            {
                Resource = Send.Resource,
            }
             .Property(Send.Messages, new JArray {
                        new JObject {
                        {
                            "From",
                            new JObject {
                                {"Email", "davidpiga@hotmail.es"},
                                {"Name", "UniMatch"}
                            }
                        }, {
                                "To",
                        new JArray {
                        new JObject {
                            {
                            "Email",
                            email
                            }, {
                            "Name",
                            "david"
                            }
                        }
                        }
                        }, {
                        "Subject",
                        subject
                        }, {
                        "HTMLPart",
                        htmlMessage
                        }
                     }
             });
            await client.PostAsync(request);

        }
    }
}
