// -----------------------------------------------------------------------
//  <copyright file="Photobooth.cs" company="Prism">
//  Copyright (c) Prism. All rights reserved.
//  </copyright>
// -----------------------------------------------------------------------

using FluentValidation;
using MediatR;

// ReSharper disable once CheckNamespace
namespace Prism.Picshare.Events.Photobooth;

public record PictureTaken(string Organisation, Guid Session, Guid Id, DateTime DateTaken): IRequest;

public class PictureTakenValidator : AbstractValidator<PictureTaken>
{
    public PictureTakenValidator()
    {
        RuleFor(x => x.Organisation).NotNull().NotEmpty().MaximumLength(Constants.MaxShortStringLength);
        RuleFor(x => x.Id).NotEmpty();
        RuleFor(x => x.Session).NotEmpty();
        RuleFor(x => x.DateTaken).NotEmpty();
    }
}