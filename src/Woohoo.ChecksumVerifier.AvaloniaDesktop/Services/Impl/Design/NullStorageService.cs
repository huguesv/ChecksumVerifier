// Copyright (c) Hugues Valois. All rights reserved.
// Licensed under the MIT license. See LICENSE in the project root for license information.

namespace Woohoo.ChecksumVerifier.AvaloniaDesktop.Services;

using System;
using System.Threading.Tasks;

internal class NullStorageService : IStorageService
{
    public Task<string[]> GetFilePathsAsync(string title, bool allowMultiple)
    {
        return Task.FromResult(Array.Empty<string>());
    }

    public Task<string[]> GetFolderPathsAsync(string title, bool allowMultiple)
    {
        return Task.FromResult(Array.Empty<string>());
    }
}
