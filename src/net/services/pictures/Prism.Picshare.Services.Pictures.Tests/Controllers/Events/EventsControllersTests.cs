// -----------------------------------------------------------------------
//  <copyright file = "EventsControllersTests.cs" company = "Prism">
//  Copyright (c) Prism.All rights reserved.
//  </copyright>
// -----------------------------------------------------------------------

using System.Threading.Tasks;
using Prism.Picshare.Domain;
using Prism.Picshare.Services.Pictures.Commands.Pictures;
using Prism.Picshare.Services.Pictures.Controllers.Events;
using Prism.Picshare.UnitTests;
using Xunit;

namespace Prism.Picshare.Services.Pictures.Tests.Controllers.Events;

public class EventsControllersTests
{

    [Fact]
    public async Task SummaryUpdated_Ok()
    {
        await EventsControllerTest.VerifyController<SummaryUpdatedController, PictureSummary, UpdateFlowSummary, Flow>();
    }

    [Fact]
    public async Task ThumbnailsGenerated_Ok()
    {
        await EventsControllerTest.VerifyController<ThumbnailsGeneratedController, EntityReference, SetPictureReady, PictureSummary>();
    }
}