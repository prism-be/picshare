// -----------------------------------------------------------------------
//  <copyright file = "Topics.cs" company = "Prism">
//  Copyright (c) Prism.All rights reserved.
//  </copyright>
// -----------------------------------------------------------------------

namespace Prism.Picshare.Events;

public static class Topics
{
    public const string RoutePrefix = "events/";

    public static class User
    {
        public const string Register = "user/register";
        public const string Created = "user/created";
        public const string AuthenticationSucceeded = "user/authentication/succeeded";
        public const string AuthenticationFailed = "user/authentication/failed";
        public const string PasswordResetRequired = "user/password-reset/required";
        public const string PasswordResetDone = "user/password-reset/done";
    }

    public static class Email
    {
        public const string Validated = "email/validated";
        public const string PasswordResetValidated = "email/password-reset/validated";
    }

    public static class Album
    {
        public const string Updated = "album/updated";
    }

    public static class Pictures
    {
        public const string Created = "picture/created";
        public const string ExifRead = "picture/exif/read";
        public const string SummaryUpdated = "picture/summary/updated";
        public const string Updated = "picture/updated";
        public const string Uploaded = "picture/uploaded";
        public const string Seen = "picture/seen";
    }

    public static class Photobooth
    {
        public static class Picture
        {
            public const string Taken = "photobooth/picture/taken";
            public const string Uploaded = "photobooth/picture/uploaded";
        }
    }
}