// Copyright (c) Hugues Valois. All rights reserved.
// Licensed under the MIT license. See LICENSE in the project root for license information.

namespace Woohoo.ChecksumVerifier.AvaloniaDesktop.ViewModels.DesignTime;

using Woohoo.ChecksumVerifier.AvaloniaDesktop.Services;

public class DesignMainWindowViewModel : MainWindowViewModel
{
    public DesignMainWindowViewModel()
        : base(new NullStorageService())
    {
        this.ScanFolders.Add(new ScanFolderViewModel() { FolderPath = @"C:\Scan\Folder" });
        this.ScanFolders.Add(new ScanFolderViewModel() { FolderPath = @"D:\Scan\Folder" });
    }
}
