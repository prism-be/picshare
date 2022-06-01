// -----------------------------------------------------------------------
//  <copyright file="PictureTaken.cs" company="Prism">
//  Copyright (c) Prism. All rights reserved.
//  </copyright>
// -----------------------------------------------------------------------

using System.Net;
using FluentValidation;
using MediatR;
using Microsoft.Azure.Cosmos;
using Prism.Picshare.Data.CosmosDB;
using Prism.Picshare.Domain;

namespace Prism.Picshare.Services.Photobooth.Commands;

public record PictureTaken(Guid OrganisationId, Guid SessionId) : IRequest<Guid>;

public class PictureTakenValidator : AbstractValidator<PictureTaken>
{
    public PictureTakenValidator()
    {
        this.RuleFor(x => x.OrganisationId).NotEmpty();
        this.RuleFor(x => x.SessionId).NotEmpty();
    }
}

public class PictureTakenHandler : IRequestHandler<PictureTaken, Guid>
{
    private readonly CosmosClient _cosmosClient;
    private readonly ILogger<PictureTakenHandler> _logger;

    public PictureTakenHandler(ILogger<PictureTakenHandler> logger, CosmosClient cosmosClient)
    {
        this._logger = logger;
        this._cosmosClient = cosmosClient;
    }

    public async Task<Guid> Handle(PictureTaken request, CancellationToken cancellationToken)
    {
        this._logger.LogInformation("Processing a picture taken request: {request}", request);

        var pictureId = Guid.NewGuid();

        var picture = await this.CreatePicture(request.OrganisationId, cancellationToken);
        await this.AddPictureToAlbum(picture.Id, request.OrganisationId, request.SessionId, cancellationToken);

        return pictureId;
    }

    private async Task AddPictureToAlbum(Guid pictureId, Guid organisationId, Guid albumId, CancellationToken cancellationToken)
    {
        var albumContainer = this._cosmosClient.GetContainer(DatabaseStructure.Albums.Database, DatabaseStructure.Albums.Container);

        var albumResult = await albumContainer.ReadItemAsync<Album>(albumId.ToString(), new PartitionKey(organisationId.ToString()), cancellationToken: cancellationToken);

        if (albumResult.StatusCode == HttpStatusCode.OK)
        {
            var album = albumResult.Resource;

            album.Pictures.Insert(0, pictureId);
            await albumContainer.ReplaceItemAsync(album, album.Id.ToString(), cancellationToken: cancellationToken);

            this._logger.LogInformation("Picture added to the album: {pictureId} -> {albumId}", pictureId, albumId);
        }
        else
        {
            this._logger.LogCritical("Error when adding picture to the album: {pictureId} -> {albumId} => {statusCode}", pictureId, albumId, albumResult.StatusCode);
            throw new BadHttpRequestException("Error when creating the picture", (int)albumResult.StatusCode);
        }
    }

    private async Task<Picture> CreatePicture(Guid organisationId, CancellationToken cancellationToken)
    {
        var pictureContainer = this._cosmosClient.GetContainer(DatabaseStructure.Pictures.Database, DatabaseStructure.Pictures.Container);

        var picture = new Picture
        {
            Id = Guid.NewGuid(), Source = PictureSource.Photobooth, CreationDate = DateTime.UtcNow, OrganisationId = organisationId
        };

        var createPictureResult = await pictureContainer.CreateItemAsync(picture, cancellationToken: cancellationToken);

        if (createPictureResult.StatusCode == HttpStatusCode.OK)
        {
            this._logger.LogInformation("Picture created with success: {pictureId}", picture.Id);
        }
        else
        {
            this._logger.LogCritical("Error when creating the picture: {pictureId} => {statusCode}", picture.Id, createPictureResult.StatusCode);
            throw new BadHttpRequestException("Error when creating the picture", (int)createPictureResult.StatusCode);
        }

        return createPictureResult.Resource;
    }
}