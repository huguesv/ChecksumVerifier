// Copyright (c) Hugues Valois. All rights reserved.
// Licensed under the MIT license. See LICENSE in the project root for license information.

namespace Woohoo.ChecksumVerifier.AvaloniaDesktop.Services;

using System.Threading.Tasks;

public interface IStorageService
{
    Task<string[]> GetFilePathsAsync(string title, bool allowMultiple);

    Task<string[]> GetFolderPathsAsync(string title, bool allowMultiple);
}
