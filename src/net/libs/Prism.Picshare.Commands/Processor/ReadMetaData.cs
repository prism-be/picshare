﻿// -----------------------------------------------------------------------
//  <copyright file = "ReadMetaData.cs" company = "Prism">
//  Copyright (c) Prism.All rights reserved.
//  </copyright>
// -----------------------------------------------------------------------

using FluentValidation;
using ImageMagick;
using MediatR;
using Prism.Picshare.Domain;
using Prism.Picshare.Events;
using Prism.Picshare.Extensions;
using Prism.Picshare.Services;

namespace Prism.Picshare.Commands.Processor;

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
    private readonly BlobClient _blobClient;
    private readonly PublisherClient _publisherClient;

    public ReadMetaDataHandler(BlobClient blobClient, PublisherClient publisherClient)
    {
        _blobClient = blobClient;
        _publisherClient = publisherClient;
    }

    public async Task<ResultCodes> Handle(ReadMetaData request, CancellationToken cancellationToken)
    {
        var blobName = BlobNamesExtensions.GetSourcePath(request.OrganisationId, request.PictureId);
        var pictureData = await _blobClient.ReadAsync(blobName, cancellationToken);

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
                    Value = exifValue.GetValue().ToString()
                });
            }
        }

        await _publisherClient.PublishEventAsync(Topics.Pictures.ExifRead, picture, cancellationToken);

        return ResultCodes.Ok;
    }
}