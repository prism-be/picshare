// -----------------------------------------------------------------------
//  <copyright file = "ReadMetaData.cs" company = "Prism">
//  Copyright (c) Prism.All rights reserved.
//  </copyright>
// -----------------------------------------------------------------------

using System.Text.Json;
using Dapr.Client;
using FluentValidation;
using ImageMagick;
using MediatR;
using Prism.Picshare.Dapr;
using Prism.Picshare.Domain;
using Prism.Picshare.Events;
using Prism.Picshare.Extensions;

namespace Prism.Picshare.Services.Processor.Commands;

public record ReadMetaData(Guid OrganisationId, Guid PictureId) : IRequest<ResultCodes>;

public class ReadMetaDataValidator : AbstractValidator<ReadMetaData>
{
    public ReadMetaDataValidator()
    {
        RuleFor(x => x.OrganisationId).NotEmpty();
        RuleFor(x => x.PictureId).NotEmpty();
    }
}

public class ReadMetaDataHandler : IRequestHandler<ReadMetaData, ResultCodes>
{
    private readonly DaprClient _daprClient;

    public ReadMetaDataHandler(DaprClient daprClient)
    {
        _daprClient = daprClient;
    }

    public async Task<ResultCodes> Handle(ReadMetaData request, CancellationToken cancellationToken)
    {
        var blobName = BlobNamesExtensions.GetSourcePath(request.OrganisationId, request.PictureId);
        var response = await _daprClient.ReadPictureAsync(blobName, cancellationToken);
        var pictureData = response.Data.ToArray();

        using var image = new MagickImage(pictureData);
        var profile = image.GetExifProfile();

        var picture = new Picture
        {
            Id = request.PictureId,
            OrganisationId = request.OrganisationId
        };

        if (profile != null)
        {
            foreach (var exifValue in profile.Values)
            {
                picture.Exifs.Add(new ExifData
                {
                    Tag = exifValue.Tag.ToString(),
                    Type = exifValue.DataType.ToString(),
                    Value = exifValue.GetValue()
                });
            }
        }

        await _daprClient.PublishEventAsync(Publishers.PubSub, Topics.Pictures.ExifRead, picture, cancellationToken);

        return ResultCodes.Ok;
    }
}