// -----------------------------------------------------------------------
//  <copyright file = "Topics.cs" company = "Prism">
//  Copyright (c) Prism.All rights reserved.
//  </copyright>
// -----------------------------------------------------------------------

namespace Prism.Picshare.Events;

public static class Topics
{
    public const string Subscription = "picshare";

    public static class User
    {
        public const string Register = "user/register";
    }

    public static class Email
    {
        public const string Validated = "email/validated";
    }

    public static class Pictures
    {
        public const string Created = "picture/created";
        public const string ExifRead = "picture/exif/read";
        public const string SummaryUpdated = "picture/summary/updated";
        public const string ThumbnailsGenerated = "picture/thumbnails/generated";
        public const string Updated = "picture/updated";
        public const string Uploaded = "picture/uploaded";
        public const string Seen = "picture/seen";
    }
}