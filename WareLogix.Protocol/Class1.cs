using System.Text;

namespace WareLogix.Protocol;

public class Class1
{
    // Implement mime (Multipurpose Internet Mail Extensions) resolver
    // See C:\Program Files\IIS Express\AppServer\applicationhost.config
    // Look for staticcontent/mimetypes


    async Task ReadChunkedEncodedStreamAsync(Stream inputStream, MemoryStream ms)
    {
        const int BUFFER_SIZE = 1024;
        byte[] buffer = new byte[BUFFER_SIZE];
        string line;

        do
        {
            line = await ReadCrLfLineAsync(inputStream);

            var lineEntries = line.Split(';', StringSplitOptions.RemoveEmptyEntries);

            if (lineEntries.Length > 0)
            {
                int chunkSize = Convert.ToInt32(lineEntries[0], 16);
                int bytesRead = 0;
                int totalBytesRead = 0;
                int bytesToRead = BUFFER_SIZE;

                while (totalBytesRead < chunkSize)
                {
                    if (chunkSize - totalBytesRead < BUFFER_SIZE)
                        bytesToRead = chunkSize - totalBytesRead;

                    bytesRead = await inputStream.ReadAsync(buffer, 0, bytesToRead);
                    ms.Write(buffer, 0, bytesRead);
                    totalBytesRead += bytesRead;
                }

                await inputStream.ReadExactlyAsync(buffer, 0, 1); // read chunk-terminating 'CR'
                await inputStream.ReadExactlyAsync(buffer, 0, 1); // read chunk-terminating 'LF'
            }

        } while (!string.IsNullOrEmpty(line));

        ms.Position = 0;
    }

    async Task<string> ReadCrLfLineAsync(Stream stream)
    {
        var line = new StringBuilder();

        Span<byte> stackSpan = stackalloc byte[1];
        Memory<byte> buffer = stackSpan.ToArray();

        int bytesRead;

        while ((bytesRead = await stream.ReadAsync(buffer)) > 0)
        {
            if (buffer.Span[0] == '\r')
            {
                if (((bytesRead = await stream.ReadAsync(buffer)) > 0) && (buffer.Span[0] == '\n'))
                    break;
                else
                    line.Append((char)buffer.Span[0]);
            }
            else
            {
                line.Append((char)buffer.Span[0]);
            }
        }

        return line.ToString();
    }
}
