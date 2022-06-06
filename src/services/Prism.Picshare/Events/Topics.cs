﻿// -----------------------------------------------------------------------
//  <copyright file="Topics.cs" company="Prism">
//  Copyright (c) Prism. All rights reserved.
//  </copyright>
// -----------------------------------------------------------------------

namespace Prism.Picshare.Events;

public static class Topics
{
    public static class Photobooth
    {
        public const string PictureTaken = "photobooth/picture/taken";
        public const string PictureUploaded = "photobooth/picture/uploaded";
        public const string PictureReady = "photobooth/picture/ready";
    }
}