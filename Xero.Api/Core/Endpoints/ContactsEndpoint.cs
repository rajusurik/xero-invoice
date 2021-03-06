﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xero.Api.Common;
using Xero.Api.Core.Endpoints.Base;
using Xero.Api.Core.Model;
using Xero.Api.Core.Request;
using Xero.Api.Core.Response;
using Xero.Api.Infrastructure.Http;

namespace Xero.Api.Core.Endpoints
{
    public interface IContactsEndpoint
        : IXeroUpdateEndpoint<ContactsEndpoint, Contact, ContactsRequest, ContactsResponse>,
        IPageableEndpoint<IContactsEndpoint>
    {
        IContactsEndpoint IncludeArchived(bool include);
        Task<ContactCisSetting> GetCisSettingsAsync(Guid id);
        IContactsEndpoint Ids(IEnumerable<Guid> ids);
    }

    public class ContactsEndpoint
        : XeroUpdateEndpoint<ContactsEndpoint, Contact, ContactsRequest, ContactsResponse>, IContactsEndpoint
    {
        internal ContactsEndpoint(XeroHttpClient client)
            : base(client, "/api.xro/2.0/Contacts")
        {
            AddParameter("page", 1, false);
        }

        public IContactsEndpoint Page(int page)
        {
            return AddParameter("page", page);
        }

        public IContactsEndpoint IncludeArchived(bool include)
        {
            return include ? AddParameter("includeArchived", true) : this;
        }

        public async Task<ContactCisSetting> GetCisSettingsAsync(Guid id)
        {
            var contactCisSettings = await Client.GetAsync<ContactCisSetting, ContactCisSettingsResponse>($"/api.xro/2.0/contacts/{id}/cissettings").ConfigureAwait(false);

            return contactCisSettings.FirstOrDefault();
        }

        public IContactsEndpoint Ids(IEnumerable<Guid> ids)
        {
            return AddParameter("ids", string.Join(",", ids));
        }

        public override void ClearQueryString()
        {
            base.ClearQueryString();
            AddParameter("page", 1, false);
        }
    }
}