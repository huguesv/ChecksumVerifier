// Copyright (c) Hugues Valois. All rights reserved.
// Licensed under the MIT license. See LICENSE in the project root for license information.

namespace Woohoo.ChecksumVerifier.Core;

public class ChecksumEntry
{
    public ChecksumEntry(string name, string checksum, string algorithm)
    {
        this.Name = name;
        this.Checksum = checksum;
        this.Algorithm = algorithm;
    }

    public string Name { get; }

    public string Checksum { get; }

    public string Algorithm { get; }
}
