// -----------------------------------------------------------------------
//  <copyright file="DummyLoggingBuilder.cs" company="Forem">
//  Copyright (c) Forem. All rights reserved.
//  </copyright>
// -----------------------------------------------------------------------

using System.Diagnostics.CodeAnalysis;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Prism.Picshare.Tests.Insights.Fakes;

[ExcludeFromCodeCoverage]
public class DummyLoggingBuilder : ILoggingBuilder
{
    public IServiceCollection Services { get; } = new ServiceCollection();
}