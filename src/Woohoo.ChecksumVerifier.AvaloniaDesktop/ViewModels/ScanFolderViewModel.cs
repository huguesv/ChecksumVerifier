// Copyright (c) Hugues Valois. All rights reserved.
// Licensed under the MIT license. See LICENSE in the project root for license information.

namespace Woohoo.ChecksumVerifier.AvaloniaDesktop.ViewModels;

using CommunityToolkit.Mvvm.ComponentModel;

public partial class ScanFolderViewModel : ObservableObject
{
    [ObservableProperty]
    public partial string FolderPath { get; set; } = string.Empty;
}
