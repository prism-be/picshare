// -----------------------------------------------------------------------
//  <copyright file="PictureTaken.cs" company="Prism">
//  Copyright (c) Prism. All rights reserved.
//  </copyright>
// -----------------------------------------------------------------------

using FluentValidation;
using MediatR;

namespace Prism.Picshare.Photobooth.Commands;

public record PictureTaken(string Organisation, Guid Session, Guid Id): IRequest;

public class PictureTakenValidator : AbstractValidator<PictureTaken>
{
    public PictureTakenValidator()
    {
        RuleFor(x => x.Organisation).NotNull().NotEmpty().MaximumLength(Constants.MaxShortStringLength);
        RuleFor(x => x.Id).NotEmpty();
        RuleFor(x => x.Session).NotEmpty();
    }
}