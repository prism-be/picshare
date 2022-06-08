// -----------------------------------------------------------------------
//  <copyright file="NotifyPictureTaken.cs" company="Prism">
//  Copyright (c) Prism. All rights reserved.
//  </copyright>
// -----------------------------------------------------------------------

using MediatR;
using Microsoft.AspNetCore.SignalR;
using Prism.Picshare.Domain;
using Prism.Picshare.Services.Photobooth.Hubs;

namespace Prism.Picshare.Services.Photobooth.Commands;

public record NotifyPictureUploaded(PhotoboothPicture PhotoboothPicture) : IRequest<PhotoboothPicture>;

public class NotifyPictureUploadedHandler : IRequestHandler<NotifyPictureUploaded, PhotoboothPicture>
{
    private readonly IHubContext<PhotoboothHub> _hub;
    private readonly ILogger<NotifyPictureUploadedHandler> _logger;

    public NotifyPictureUploadedHandler(IHubContext<PhotoboothHub> hub, ILogger<NotifyPictureUploadedHandler> logger)
    {
        _hub = hub;
        _logger = logger;
    }

    public async Task<PhotoboothPicture> Handle(NotifyPictureUploaded request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Notify the user for picture uploaded : {picture}", request.PhotoboothPicture);

        await _hub.Clients.All.SendAsync("PictureUploaded", request.PhotoboothPicture, cancellationToken);

        return request.PhotoboothPicture;
    }
}