// -----------------------------------------------------------------------
//  <copyright file = "GeneratePictureSummary.cs" company = "Prism">
//  Copyright (c) Prism.All rights reserved.
//  </copyright>
// -----------------------------------------------------------------------

using Dapr.Client;
using MediatR;
using Prism.Picshare.Domain;

namespace Prism.Picshare.Services.Pictures.Commands.Pictures;

public record GeneratePictureSummary(Guid OrganisationId, Guid PictureId, List<ExifData> PictureExifs) : IRequest<Picture>;

public class GeneratePictureSummaryHandler : IRequestHandler<GeneratePictureSummary, Picture>
{
    private readonly DaprClient _daprClient;

    public GeneratePictureSummaryHandler(DaprClient daprClient)
    {
        _daprClient = daprClient;
    }

    public async Task<Picture> Handle(GeneratePictureSummary request, CancellationToken cancellationToken)
    {
        var picture = await _daprClient.GetStatePictureAsync(request.OrganisationId, request.PictureId, cancellationToken);

        picture.Exifs = request.PictureExifs;

        await _daprClient.SaveStateAsync(picture, cancellationToken);

        return picture;
    }
}