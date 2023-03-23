﻿using Blackbird.Applications.Sdk.Common.Authentication;
using Blackbird.Applications.Sdk.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Apps.GoogleTranslate.Connections
{
    public class ConnectionProvider : IConnectionProvider
    {
        public AuthenticationCredentialsProvider Create(IDictionary<string, string> connectionValues)
        {
            var credential = connectionValues.First(x => x.Key == "projectId");
            return new AuthenticationCredentialsProvider(AuthenticationCredentialsRequestLocation.None, credential.Key, credential.Value);
        }

        public string ConnectionName => "Blackbird";


        public IEnumerable<string> ConnectionProperties => new[] { "serviceAccountConfString", "projectId" };
    }
}
