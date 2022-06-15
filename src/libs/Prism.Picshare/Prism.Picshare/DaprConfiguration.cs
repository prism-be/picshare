// -----------------------------------------------------------------------
//  <copyright file = "DaprConfiguration.cs" company = "Prism">
//  Copyright (c) Prism.All rights reserved.
//  </copyright>
// -----------------------------------------------------------------------

namespace Prism.Picshare;

public static class DaprConfiguration
{
    public const string PubSub = "pubsub";
    public const string DataStore = "datastore";

    public static class Photobooth
    {
        public const string InternalPubSub = "pubsub-photobooth";
    }

    public static class BindingOperation
    {
        public const string Create = "create";
        public const string Get = "get";
    }
}