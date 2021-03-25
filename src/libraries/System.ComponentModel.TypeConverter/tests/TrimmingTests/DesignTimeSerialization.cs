// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.ComponentModel.Design;
using System.IO;
using System;

/// <summary>
/// Tests that the System.ComponentModel.TypeConverter.EnableUnsafeBinaryFormatterInDesigntimeLicenseContextSerialization
/// property works as expected when used in a trimmed application.
/// </summary>
class Program
{
    static int Main(string[] args)
    {
        var context = new DesigntimeLicenseContext();
        context.SetSavedLicenseKey(typeof(int), "key");
        AppContext.SetSwitch("System.ComponentModel.TypeConverter.EnableUnsafeBinaryFormatterInDesigntimeLicenseContextSerialization", true);

        //byte[] currentBuffer;
        using (MemoryStream stream = new MemoryStream())
        {
            long position = stream.Position;
            System.ComponentModel.Design.DesigntimeLicenseContextSerializer.Serialize(stream, "key", context);
            stream.Seek(position, SeekOrigin.Begin);

            byte[] currentBuffer = stream.ToArray();
            // Verify contents with saved stream
            byte[] correctBytes = File.ReadAllBytes(@"C:\temp\DesigntimeLicenseContextSerializer_SampleStream");

            if (currentBuffer.Length != correctBytes.Length)
            {
                File.WriteAllText(@"C:\temp\debug.txt", $"Lengths don't match: currentBufferLength = {currentBuffer.Length}, correctLength = {correctBytes.Length}");
                File.WriteAllBytes(@"C:\temp\DesigntimeLicenseContextSerializer_SampleStream.debug", currentBuffer);
                return -1;
            }
            for (int i = 0; i < currentBuffer.Length; i++)
            {
                if (currentBuffer[i] != correctBytes[i])
                {
                    return -1;
                }
            }

        }

        return 100;
    }
}
