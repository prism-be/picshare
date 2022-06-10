// -----------------------------------------------------------------------
//  <copyright file="DaprConfiguration.cs" company="Prism">
//  Copyright (c) Prism. All rights reserved.
//  </copyright>
// -----------------------------------------------------------------------

namespace Prism.Picshare;

public static class DaprConfiguration
{
    public const string PubSub = "pubsub";
    public const string DataStore = "datastore";

    public static class Photobooth
    {
        // ReSharper disable once MemberHidesStaticFromOuterClass
        public const string PubSub = "pubsub-photobooth";
    }
}