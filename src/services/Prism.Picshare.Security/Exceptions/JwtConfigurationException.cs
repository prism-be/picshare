// -----------------------------------------------------------------------
//  <copyright file="DatabaseConfigurationException.cs" company="Prism">
//  Copyright (c) Prism. All rights reserved.
//  </copyright>
// -----------------------------------------------------------------------

using System.Diagnostics.CodeAnalysis;
using System.Runtime.Serialization;

namespace Prism.Picshare.Security.Exceptions;

[Serializable]
public class JwtConfigurationException : Exception
{
    public JwtConfigurationException(string message) : base(message)
    {
    }

    [ExcludeFromCodeCoverage]
    protected JwtConfigurationException(SerializationInfo info, StreamingContext context) : base(info, context)
    {
    }

    [ExcludeFromCodeCoverage]
    public override void GetObjectData(SerializationInfo info, StreamingContext context)
    {
        base.GetObjectData(info, context);
    }
}