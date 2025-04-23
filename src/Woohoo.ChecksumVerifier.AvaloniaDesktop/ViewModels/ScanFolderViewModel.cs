// Copyright (c) Hugues Valois. All rights reserved.
// Licensed under the MIT license. See LICENSE in the project root for license information.

namespace Woohoo.ChecksumVerifier.AvaloniaDesktop.ViewModels;

using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

public partial class ScanFolderViewModel : ObservableObject
{
    [ObservableProperty]
    private string folderPath = string.Empty;

    [RelayCommand]
    private void Remove()
    {
        // TODO: bind to a command on parent view model instead
        // https://github.com/AvaloniaUI/Avalonia/discussions/12719
    }
}
