// -----------------------------------------------------------------------
//  <copyright file = "VerifyAuthenticationRequest.cs" company = "Prism">
//  Copyright (c) Prism.All rights reserved.
//  </copyright>
// -----------------------------------------------------------------------

using FluentValidation;
using MediatR;

namespace Prism.Picshare.Services.Authentication.Commands;

public record VerifyAuthenticationRequest(string Bearer) : IRequest<ResultCodes>;

public class VerifyAuthenticationRequestValidator : AbstractValidator<VerifyAuthenticationRequest>
{
    public VerifyAuthenticationRequestValidator()
    {
        
    }
}

public class VerifyAuthenticationRequestHandler : IRequestHandler<VerifyAuthenticationRequest, ResultCodes>
{

    public Task<ResultCodes> Handle(VerifyAuthenticationRequest request, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }
}