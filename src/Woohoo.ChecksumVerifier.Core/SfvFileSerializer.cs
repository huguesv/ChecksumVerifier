// Copyright (c) Hugues Valois. All rights reserved.
// Licensed under the MIT license. See LICENSE in the project root for license information.

namespace Woohoo.ChecksumVerifier.Core;

using System.IO;

public static class SfvFileSerializer
{
    public static ChecksumFile LoadFrom(string filePath)
    {
        var checksumFile = new ChecksumFile();

        var lines = File.ReadAllLines(filePath);
        foreach (var line in lines)
        {
            // Comments start with ';'
            if (line.StartsWith(";"))
            {
                continue;
            }

            var lastSpaceIndex = line.LastIndexOf(' ');
            if (lastSpaceIndex >= 0)
            {
                var fileName = line[..lastSpaceIndex].Trim();
                var sum = line[(lastSpaceIndex + 1)..].Trim();

                var algorithm = sum.Length switch
                {
                    8 => "CRC32",
                    32 => "MD5",
                    40 => "SHA1",
                    64 => "SHA256",
                    96 => "SHA384",
                    128 => "SHA512",
                    _ => "CRC32",
                };

                checksumFile.Entries.Add(new ChecksumEntry(fileName, sum, algorithm));
            }
        }

        return checksumFile;
    }
}
