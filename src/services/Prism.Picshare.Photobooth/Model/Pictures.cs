// -----------------------------------------------------------------------
//  <copyright file="Pictures.cs" company="Prism">
//  Copyright (c) Prism. All rights reserved.
//  </copyright>
// -----------------------------------------------------------------------

namespace Prism.Picshare.Photobooth.Model;

public record Pictures(Guid Id, Guid Session, DateTime DateTaken);