// Copyright (c) Hugues Valois. All rights reserved.
// Licensed under the MIT license. See LICENSE in the project root for license information.

namespace Woohoo.ChecksumVerifier.AvaloniaDesktop.ViewModels;

using CommunityToolkit.Mvvm.ComponentModel;

public partial class HashResultViewModel : ObservableObject
{
    [ObservableProperty]
    private string name = string.Empty;

    [ObservableProperty]
    private string algorithm = string.Empty;

    [ObservableProperty]
    private string expected = string.Empty;

    [ObservableProperty]
    private string actual = string.Empty;

    [ObservableProperty]
    private string status = string.Empty;
}
