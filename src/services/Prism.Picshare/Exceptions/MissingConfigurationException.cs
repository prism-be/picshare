// -----------------------------------------------------------------------
//  <copyright file="MissingConfigurationException.cs" company="Prism">
//  Copyright (c) Prism. All rights reserved.
//  </copyright>
// -----------------------------------------------------------------------

using System.Diagnostics.CodeAnalysis;
using System.Runtime.Serialization;

namespace Prism.Picshare.Exceptions;

public class MissingConfigurationException : Exception
{
    public MissingConfigurationException(string message) : base(message)
    {
    }

    [ExcludeFromCodeCoverage]
    protected MissingConfigurationException(SerializationInfo info, StreamingContext context) : base(info, context)
    {
    }

    [ExcludeFromCodeCoverage]
    public override void GetObjectData(SerializationInfo info, StreamingContext context)
    {
        base.GetObjectData(info, context);
    }
}