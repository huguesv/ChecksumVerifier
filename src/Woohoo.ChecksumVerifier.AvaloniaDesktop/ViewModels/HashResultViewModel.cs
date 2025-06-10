// Copyright (c) Hugues Valois. All rights reserved.
// Licensed under the MIT license. See LICENSE in the project root for license information.

namespace Woohoo.ChecksumVerifier.AvaloniaDesktop.ViewModels;

using CommunityToolkit.Mvvm.ComponentModel;

public partial class HashResultViewModel : ObservableObject
{
    [ObservableProperty]
    public partial string Name { get; set; } = string.Empty;

    [ObservableProperty]
    public partial string Algorithm { get; set; } = string.Empty;

    [ObservableProperty]
    public partial string Expected { get; set; } = string.Empty;

    [ObservableProperty]
    public partial string Actual { get; set; } = string.Empty;

    [ObservableProperty]
    public partial string Status { get; set; } = string.Empty;
}
