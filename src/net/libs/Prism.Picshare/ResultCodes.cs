// -----------------------------------------------------------------------
//  <copyright file = "ResultCodes.cs" company = "Prism">
//  Copyright (c) Prism.All rights reserved.
//  </copyright>
// -----------------------------------------------------------------------

namespace Prism.Picshare;

public enum ResultCodes
{
    Unknown = 0,
    Ok = 200,
    InvalidCredentials = 40101,
    EmailNotValidated = 40102,
    MailActionNotFound = 40401,
    UserNotFound = 40402,
    PictureNotFound = 40403,
    MailActionAlreadyConsumed = 40901,
    ExistingOrganisation = 40902,
    ExistingUsername = 40903
}