// -----------------------------------------------------------------------
//  <copyright file = "ResultCodes.cs" company = "Prism">
//  Copyright (c) Prism.All rights reserved.
//  </copyright>
// -----------------------------------------------------------------------

namespace Prism.Picshare.Services.Mailing.Model;

public enum ResultCodes
{
    Unknown = 0,
    Ok = 200,
    MailActionNotFound = 40401,
    MailActionAlreadyConsumed = 40901
}