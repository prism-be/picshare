﻿// -----------------------------------------------------------------------
//  <copyright file="DummyModel.cs" company="Prism">
//  Copyright (c) Prism. All rights reserved.
//  </copyright>
// -----------------------------------------------------------------------

using System;

namespace Prism.Picshare.Data.LiteDB.Tests;

public record DummyModel(Guid Id, string Lastname, string Firstname, string Email, int Age);