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

public record NotifyPictureTaken(PhotoboothPicture PhotoboothPicture) : IRequest<PhotoboothPicture>;

public class NotifyPictureTakenHandler : IRequestHandler<NotifyPictureTaken, PhotoboothPicture>
{
    private readonly IHubContext<PhotoboothHub> _hub;
    private readonly ILogger<NotifyPictureTakenHandler> _logger;

    public NotifyPictureTakenHandler(IHubContext<PhotoboothHub> hub, ILogger<NotifyPictureTakenHandler> logger)
    {
        _hub = hub;
        _logger = logger;
    }

    public async Task<PhotoboothPicture> Handle(NotifyPictureTaken request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Notify the user for a new picture : {picture}", request.PhotoboothPicture);

        await _hub.Clients.All.SendAsync("PictureTaken", request.PhotoboothPicture, cancellationToken);

        return request.PhotoboothPicture;
    }
}