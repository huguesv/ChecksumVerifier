// Copyright (c) Hugues Valois. All rights reserved.
// Licensed under the MIT license. See LICENSE in the project root for license information.

namespace Woohoo.ChecksumVerifier.Core;

using System.IO;

public static class Md5FileSerializer
{
    public static ChecksumFile LoadFrom(string filePath)
    {
        var checksumFile = new ChecksumFile();

        var lines = File.ReadAllLines(filePath);
        foreach (var originalLine in lines)
        {
            var line = originalLine.Trim();
            if (line.Length > 0 && !line.StartsWith("#") && !line.StartsWith(";"))
            {
                var firstSpaceIndex = line.IndexOf(' ');
                if (firstSpaceIndex >= 0)
                {
                    var sum = line[..firstSpaceIndex].Trim();
                    var fileName = line[(firstSpaceIndex + 1)..].Trim();
                    if (fileName.StartsWith("*"))
                    {
                        fileName = fileName[1..];
                    }

                    checksumFile.Entries.Add(new ChecksumEntry(fileName, sum, "MD5"));
                }
            }
        }

        return checksumFile;
    }
}
