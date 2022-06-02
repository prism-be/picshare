// -----------------------------------------------------------------------
//  <copyright file="InvalidModelException.cs" company="Prism">
//  Copyright (c) Prism. All rights reserved.
//  </copyright>
// -----------------------------------------------------------------------

using System.Diagnostics.CodeAnalysis;
using System.Runtime.Serialization;

namespace Prism.Picshare.Exceptions;

[Serializable]
public class InvalidModelException : Exception
{
    public InvalidModelException(Dictionary<string, string[]> validations) : this("The model is invalid", validations)
    {
    }

    public InvalidModelException(string message, Dictionary<string, string[]> validations) : base(message)
    {
        this.Validations = validations;
    }

    [ExcludeFromCodeCoverage]
    protected InvalidModelException(SerializationInfo info, StreamingContext context) : base(info, context)
    {
        this.Validations = info.GetValue(nameof(this.Validations), typeof(Dictionary<string, string[]>)) as Dictionary<string, string[]> ?? new Dictionary<string, string[]>();
    }

    public Dictionary<string, string[]> Validations { get; set; }

    [ExcludeFromCodeCoverage]
    public override void GetObjectData(SerializationInfo info, StreamingContext context)
    {
        base.GetObjectData(info, context);
        this.Validations = info.GetValue(nameof(this.Validations), typeof(Dictionary<string, string[]>)) as Dictionary<string, string[]> ?? new Dictionary<string, string[]>();
    }
}