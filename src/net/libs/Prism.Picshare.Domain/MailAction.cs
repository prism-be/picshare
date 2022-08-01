// -----------------------------------------------------------------------
//  <copyright file = "MailAction.cs" company = "Prism">
//  Copyright (c) Prism.All rights reserved.
//  </copyright>
// -----------------------------------------------------------------------

using System.Text.Json.Serialization;

namespace Prism.Picshare.Domain;

public class MailAction<T>
{
    public MailAction(Guid id, MailActionType actionType, T data)
    {
        Data = data;
        Id = id;
        ActionType = actionType;
        CreationDate = DateTime.UtcNow;
    }

    [JsonPropertyName("consumed")]
    public bool Consumed { get; set; }

    [JsonPropertyName("creationDate")]
    public DateTime CreationDate { get; set; }

    [JsonPropertyName("confirmationDate")]
    public DateTime? ConfirmationDate { get; set; }

    [JsonPropertyName("id")]
    public Guid Id { get; set; }

    [JsonPropertyName("actionType")]
    public MailActionType ActionType { get; set; }

    [JsonPropertyName("data")]
    public T Data { get; set; }
}

public enum MailActionType
{
    ConfirmUserRegistration = 0
}