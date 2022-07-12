// -----------------------------------------------------------------------
//  <copyright file = "GeneratePictureSummary.cs" company = "Prism">
//  Copyright (c) Prism.All rights reserved.
//  </copyright>
// -----------------------------------------------------------------------

using System.Globalization;
using Dapr.Client;
using MediatR;
using Prism.Picshare.Dapr;
using Prism.Picshare.Domain;
using Prism.Picshare.Events;

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
        picture.Summary.Id = picture.Id;
        picture.Summary.OrganisationId = picture.OrganisationId;
        picture.Summary.Name = picture.Name;
        picture.Summary.Date = RetrieveDate(picture.Exifs);

        await _daprClient.SaveStateAsync(picture, cancellationToken);
        await _daprClient.PublishEventAsync(Publishers.PubSub, Topics.Pictures.SummaryUpdated, picture.Summary, cancellationToken);

        return picture;
    }

    private static DateTime RetrieveDate(IReadOnlyCollection<ExifData> exifs)
    {
        return RetrieveDate(exifs, "DateTimeOriginal")
               ?? RetrieveDate(exifs, "DateTimeDigitized")
               ?? RetrieveDate(exifs, "DateTime")
               ?? DateTime.UtcNow;
    }

    private static DateTime? RetrieveDate(IEnumerable<ExifData> exifs, string tag)
    {
        var exif = exifs.SingleOrDefault(x => x.Tag == tag);

        if (exif == null)
        {
            return null;
        }

        string value = exif.Value.ToString();

        var splitted = value.Split(' ');

        if (splitted.Length != 2)
        {
            return null;
        }

        value = splitted[0].Replace(':', '/') + " " + splitted[1];

        if (DateTime.TryParse(value, CultureInfo.InvariantCulture, DateTimeStyles.AssumeUniversal, out var date))
        {
            return date.ToUniversalTime();
        }

        return null;
    }
}