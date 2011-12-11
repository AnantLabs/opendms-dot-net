﻿using System;
using Http = OpenDMS.Networking.Protocols.Http;

namespace OpenDMS.Storage.Providers.CouchDB.Commands
{
    public class GetDocument : Base
    {
        public GetDocument(Uri uri)
            : base(uri, new Http.Methods.Get())
        {
        }

        public override ReplyBase MakeReply(Http.Response response)
        {
            try
            {
                return new GetDocumentReply(response);
            }
            catch (Exception e)
            {
                Logger.Storage.Error("An exception occurred while creating the GetDocumentReply.", e);
                throw;
            }
        }
    }
}
