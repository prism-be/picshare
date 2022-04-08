// -----------------------------------------------------------------------
//  <copyright file="DatabaseConfigurationException.cs" company="Prism">
//  Copyright (c) Prism. All rights reserved.
//  </copyright>
// -----------------------------------------------------------------------

using System.Diagnostics.CodeAnalysis;
using System.Runtime.Serialization;

namespace Prism.Picshare.Events.Exceptions;

[Serializable]
public class EventsConfigurationException : Exception
{
    public EventsConfigurationException(string message) : base(message)
    {
    }

    [ExcludeFromCodeCoverage]
    protected EventsConfigurationException(SerializationInfo info, StreamingContext context) : base(info, context)
    {
    }

    [ExcludeFromCodeCoverage]
    public override void GetObjectData(SerializationInfo info, StreamingContext context)
    {
        base.GetObjectData(info, context);
    }
}