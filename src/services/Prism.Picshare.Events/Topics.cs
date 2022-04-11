// -----------------------------------------------------------------------
//  <copyright file="Topics.cs" company="Prism">
//  Copyright (c) Prism. All rights reserved.
//  </copyright>
// -----------------------------------------------------------------------

namespace Prism.Picshare.Events;

public static class Topics
{
    public static class Authentication
    {
        public const string UserAuthenticated = "authentication.user.authenticated";
    }

    public static class Photobooth
    {
        public const string PictureTaken = "photobooth.picture.taken";
    }
}