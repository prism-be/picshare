// -----------------------------------------------------------------------
//  <copyright file = "GenerateThumbnail.cs" company = "Prism">
//  Copyright (c) Prism.All rights reserved.
//  </copyright>
// -----------------------------------------------------------------------

using MediatR;

namespace Prism.Picshare.Services.Processor.Commands;

public record GenerateThumbnail(Guid OrganisationId, Guid PictureId, int Width, int Height): IRequest<ResultCodes>;