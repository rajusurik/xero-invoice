﻿using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Xero.Api.Common;
using Xero.Api.Core.Model;
using Xero.Api.Core.Model.Types;
using Xero.Api.Core.Response;
using Xero.Api.Infrastructure.Http;

namespace Xero.Api.Core.Endpoints
{
    public class AttachmentsEndpoint
    {
        private XeroHttpClient Client { get; set; }

        public AttachmentsEndpoint(XeroHttpClient client)
        {
            Client = client;
        }

        public Task<IEnumerable<Attachment>> ListAsync(AttachmentEndpointType type, Guid parent)
        {
            return Client.GetAsync<Attachment, AttachmentsResponse>(string.Format("/api.xro/2.0/{0}/{1}/Attachments", type, parent.ToString("D")));
        }

        public async Task<Attachment> GetAsync(AttachmentEndpointType type, Guid parent, string fileName)
        {
            var response = await Client.GetAsync(string.Format("/api.xro/2.0/{0}/{1}/Attachments/{2}", type, parent.ToString("D"), fileName)).ConfigureAwait(false);

            if (response.StatusCode == HttpStatusCode.OK)
            {
                var stream = await response.Content.ReadAsStreamAsync().ConfigureAwait(false);

                return new Attachment(stream, fileName, response.Content.Headers.ContentType.ToString(), (int)stream.Length);
            }

            await Client.HandleErrorsAsync(response).ConfigureAwait(false);
            return null;
        }

        public async Task<Attachment> AddOrReplaceAsync(Attachment attachment, AttachmentEndpointType type, Guid parent, bool includeOnline = false)
        {
            var mimeType = MimeTypes.GetMimeType(attachment.FileName);

            var url = string.Format("/api.xro/2.0/{0}/{1}/Attachments/{2}", type, parent.ToString("D"), attachment.FileName);

            var parameters = new NameValueCollection();

            if (SupportsOnline(type) && includeOnline)
            {
                parameters.AddIfNotNull("IncludeOnline", true);
            }

            var result = await Client.PostAsync<Attachment, AttachmentsResponse>(url, attachment.Content, mimeType, parameters).ConfigureAwait(false);

            return result.FirstOrDefault();
        }

        private static bool SupportsOnline(AttachmentEndpointType type)
        {
            return type == AttachmentEndpointType.Invoices || type == AttachmentEndpointType.CreditNotes;
        }
    }
}
