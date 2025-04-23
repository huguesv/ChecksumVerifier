// Copyright (c) Hugues Valois. All rights reserved.
// Licensed under the MIT license. See LICENSE in the project root for license information.

namespace Woohoo.ChecksumVerifier.AvaloniaDesktop.Services;

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Avalonia.Platform.Storage;

internal class StorageService : IStorageService
{
    public async Task<string[]> GetFilePathsAsync(string title, bool allowMultiple)
    {
        var window = App.GetTopLevel(App.Current ?? throw new InvalidOperationException());
        if (window is null)
        {
            throw new InvalidOperationException();
        }

        var files = await window.StorageProvider.OpenFilePickerAsync(new FilePickerOpenOptions
        {
            Title = title,
            AllowMultiple = allowMultiple,
        });

        var filePaths = new List<string>();
        foreach (var file in files)
        {
            string? path = file.TryGetLocalPath();
            if (!string.IsNullOrEmpty(path))
            {
                filePaths.Add(path);
            }
        }

        return filePaths.ToArray();
    }

    public async Task<string[]> GetFolderPathsAsync(string title, bool allowMultiple)
    {
        var window = App.GetTopLevel(App.Current ?? throw new InvalidOperationException());
        if (window is null)
        {
            throw new InvalidOperationException();
        }

        var folders = await window.StorageProvider.OpenFolderPickerAsync(new FolderPickerOpenOptions
        {
            Title = title,
            AllowMultiple = allowMultiple,
        });

        var folderPaths = new List<string>();
        foreach (var folder in folders)
        {
            string? path = folder.TryGetLocalPath();

            if (!string.IsNullOrEmpty(path))
            {
                folderPaths.Add(path);
            }
        }

        return folderPaths.ToArray();
    }
}
