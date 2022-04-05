// -----------------------------------------------------------------------
//  <copyright file="Authentication.cs" company="Prism">
//  Copyright (c) Prism. All rights reserved.
//  </copyright>
// -----------------------------------------------------------------------

using FluentValidation;
using MediatR;

// ReSharper disable once CheckNamespace
namespace Prism.Picshare.Events.Authentication;

public record LoginRequest(string Organisation, string Login, string Password) : IRequest<LoginResponse>;

public class LoginRequestValidator : AbstractValidator<LoginRequest>
{
    public LoginRequestValidator()
    {
        RuleFor(x => x.Organisation).NotNull().NotEmpty().MaximumLength(Constants.MaxShortStringLength);
        RuleFor(x => x.Login).NotNull().NotEmpty().MaximumLength(Constants.MaxShortStringLength);
        RuleFor(x => x.Password).NotNull().NotEmpty().MaximumLength(Constants.MaxShortStringLength);
    }
}

public record LoginResponse(ReturnCodes ReturnCode, string? Token);