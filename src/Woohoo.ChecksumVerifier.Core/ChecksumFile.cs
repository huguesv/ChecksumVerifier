// Copyright (c) Hugues Valois. All rights reserved.
// Licensed under the MIT license. See LICENSE in the project root for license information.

namespace Woohoo.ChecksumVerifier.Core;

using System.Collections.ObjectModel;

public class ChecksumFile
{
    public ChecksumFile()
    {
        this.Entries = [];
    }

    public Collection<ChecksumEntry> Entries { get; }
}
