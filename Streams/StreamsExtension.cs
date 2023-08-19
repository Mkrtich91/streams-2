using System;
using System.Globalization;
using System.IO;
using System.IO.Compression;
using System.Net;
using System.Security.Cryptography;
using System.Text;

namespace Streams
{
    /// <summary>
    /// Class for training streams and operations with it.
    /// </summary>
    public static class StreamsExtension
    {
        /// <summary>
        /// Implements the logic of byte copying the contents of the source text file using class FileStream as a backing store stream.
        /// </summary>
        /// <param name="sourcePath">Path to source file.</param>
        /// <param name="destinationPath">Path to destination file.</param>
        /// <returns>The number of recorded bytes.</returns>
        /// <exception cref="ArgumentException">Throw if path to source file or path to destination file is null or empty.</exception>
        /// <exception cref="FileNotFoundException">Throw if source file doesn't exist.</exception>
        public static int ByteCopyWithFileStream(string? sourcePath, string? destinationPath)
        {
            InputValidation(sourcePath, destinationPath);

            int bytesCopied = 0;

#pragma warning disable CS8604
            using (FileStream sourceStream = new FileStream(sourcePath, FileMode.Open))
            using (FileStream destinationStream = new FileStream(destinationPath, FileMode.Create))
            {
                byte[] buffer = new byte[4096];

                int bytesRead;
                while ((bytesRead = sourceStream.Read(buffer, 0, buffer.Length)) > 0)
                {
                    destinationStream.Write(buffer, 0, bytesRead);
                    bytesCopied += bytesRead;
                }
            }
#pragma warning restore CS8604

            return bytesCopied;
        }

        /// <summary>
        /// Implements the logic of byte copying the contents of the source text file using MemoryStream.
        /// </summary>
        /// <param name="sourcePath">Path to source file.</param>
        /// <param name="destinationPath">Path to destination file.</param>
        /// <returns>The number of recorded bytes.</returns>
        /// <exception cref="ArgumentException">Throw if path to source file or path to destination file is null or empty.</exception>
        /// <exception cref="FileNotFoundException">Throw if source file doesn't exist.</exception>
        public static int ByteCopyWithMemoryStream(string? sourcePath, string? destinationPath)
        {
            InputValidation(sourcePath, destinationPath);

            byte[] buffer = new byte[1024];
            int bytesRead;

#pragma warning disable CS8604
            using (FileStream sourceStream = new FileStream(sourcePath, FileMode.Open))
            {
                using MemoryStream memoryStream = new MemoryStream();
                while ((bytesRead = sourceStream.Read(buffer, 0, buffer.Length)) > 0)
                {
                    memoryStream.Write(buffer, 0, bytesRead);
                }

                byte[] bytes = memoryStream.ToArray();

                using FileStream destinationStream = new FileStream(destinationPath, FileMode.Create);
                destinationStream.Write(bytes, 0, bytes.Length);
            }
#pragma warning restore CS8604

            return buffer.Length;
        }

        /// <summary>
        /// Implements the logic of block copying the contents of the source text file using FileStream buffer.
        /// </summary>
        /// <param name="sourcePath">Path to source file.</param>
        /// <param name="destinationPath">Path to destination file.</param>
        /// <returns>The number of recorded bytes.</returns>
        /// <exception cref="ArgumentException">Throw if path to source file or path to destination file is null or empty.</exception>
        /// <exception cref="FileNotFoundException">Throw if source file doesn't exist.</exception>
        public static int BlockCopyWithFileStream(string? sourcePath, string? destinationPath)
        {
            InputValidation(sourcePath, destinationPath);

            const int bufferSize = 4096;

#pragma warning disable CS8604
            using (FileStream sourceStream = new FileStream(sourcePath, FileMode.Open))
            {
                using FileStream destinationStream = new FileStream(destinationPath, FileMode.Create);
                byte[] buffer = new byte[bufferSize];
                int bytesRead;

                while ((bytesRead = sourceStream.Read(buffer, 0, bufferSize)) > 0)
                {
                    destinationStream.Write(buffer, 0, bytesRead);
                }
            }
#pragma warning restore CS8604

            FileInfo destinationFileInfo = new FileInfo(destinationPath);
            return (int)destinationFileInfo.Length;
        }

        /// <summary>
        /// Implements the logic of block copying the contents of the source text file using MemoryStream.
        /// </summary>
        /// <param name="sourcePath">Path to source file.</param>
        /// <param name="destinationPath">Path to destination file.</param>
        /// <returns>The number of recorded bytes.</returns>
        /// <exception cref="ArgumentException">Throw if path to source file or path to destination file is null or empty.</exception>
        /// <exception cref="FileNotFoundException">Throw if source file doesn't exist.</exception>
        public static int BlockCopyWithMemoryStream(string? sourcePath, string? destinationPath)
        {
            InputValidation(sourcePath, destinationPath);

            const int bufferSize = 4096;

#pragma warning disable CS8604
            using (FileStream sourceStream = new FileStream(sourcePath, FileMode.Open))
            {
                using MemoryStream memoryStream = new MemoryStream();
                byte[] buffer = new byte[bufferSize];
                int bytesRead;

                while ((bytesRead = sourceStream.Read(buffer, 0, bufferSize)) > 0)
                {
                    memoryStream.Write(buffer, 0, bytesRead);
                }

                using FileStream destinationStream = new FileStream(destinationPath, FileMode.Create);
                memoryStream.Seek(0, SeekOrigin.Begin);
                memoryStream.CopyTo(destinationStream);
            }
#pragma warning restore CS8604

            FileInfo destinationFileInfo = new FileInfo(destinationPath);
            return (int)destinationFileInfo.Length;
        }

        /// <summary>
        /// Implements the logic of block copying the contents of the source text file using FileStream and class-decorator BufferedStream.
        /// </summary>
        /// <param name="sourcePath">Path to source file.</param>
        /// <param name="destinationPath">Path to destination file.</param>
        /// <returns>The number of recorded bytes.</returns>
        /// <exception cref="ArgumentException">Throw if path to source file or path to destination file is null or empty.</exception>
        /// <exception cref="FileNotFoundException">Throw if source file doesn't exist.</exception>
        public static int BlockCopyWithBufferedStreamForFileStream(string sourcePath, string destinationPath)
        {
            InputValidation(sourcePath, destinationPath);

            const int bufferSize = 4096; // Choose an appropriate buffer size

            using (FileStream sourceStream = new FileStream(sourcePath, FileMode.Open))
            {
                using FileStream destinationStream = new FileStream(destinationPath, FileMode.Create);
                using BufferedStream bufferedSourceStream = new BufferedStream(sourceStream, bufferSize);
                byte[] buffer = new byte[bufferSize];
                int bytesRead;

                while ((bytesRead = bufferedSourceStream.Read(buffer, 0, bufferSize)) > 0)
                {
                    destinationStream.Write(buffer, 0, bytesRead);
                }
            }

            FileInfo destinationFileInfo = new FileInfo(destinationPath);
            return (int)destinationFileInfo.Length;
        }

        /// <summary>
        /// Implements the logic of block copying the contents of the source text file using MemoryStream and class-decorator BufferedStream.
        /// </summary>
        /// <param name="sourcePath">Path to source file.</param>
        /// <param name="destinationPath">Path to destination file.</param>
        /// <returns>The number of recorded bytes.</returns>
        /// <exception cref="ArgumentException">Throw if path to source file or path to destination file is null or empty.</exception>
        /// <exception cref="FileNotFoundException">Throw if source file doesn't exist.</exception>
        public static int BlockCopyWithBufferedStreamForMemoryStream(string sourcePath, string destinationPath)
        {
            InputValidation(sourcePath, destinationPath);

            InputValidation(sourcePath, destinationPath);

            const int bufferSize = 4096;

            using FileStream sourceStream = new FileStream(sourcePath, FileMode.Open);
            using MemoryStream memoryStream = new MemoryStream();
            using BufferedStream bufferedMemoryStream = new BufferedStream(memoryStream, bufferSize);
            byte[] buffer = new byte[bufferSize];
            int bytesRead;

            while ((bytesRead = sourceStream.Read(buffer, 0, bufferSize)) > 0)
            {
                bufferedMemoryStream.Write(buffer, 0, bytesRead);
            }

            bufferedMemoryStream.Seek(0, SeekOrigin.Begin);

            using (FileStream destinationStream = new FileStream(destinationPath, FileMode.Create))
            {
                bufferedMemoryStream.CopyTo(destinationStream);
            }

            return (int)memoryStream.Length;
        }

        /// <summary>
        /// Implements the logic of line-by-line copying of the contents of the source text file
        /// using FileStream and classes-adapters  StreamReader/StreamWriter.
        /// </summary>
        /// <param name="sourcePath">Path to source file.</param>
        /// <param name="destinationPath">Path to destination file.</param>
        /// <returns>The number of recorded lines.</returns>
        /// <exception cref="ArgumentException">Throw if path to source file or path to destination file are null or empty.</exception>
        /// <exception cref="FileNotFoundException">Throw if source file doesn't exist.</exception>
        public static int LineCopy(string? sourcePath, string? destinationPath)
        {
            InputValidation(sourcePath, destinationPath);
            Encoding encoding = Encoding.UTF8;
            using FileStream sourceStream = new FileStream(sourcePath!, FileMode.Open, FileAccess.Read);
            using StreamReader reader = new StreamReader(sourceStream);
            using FileStream destinationStream = new FileStream(destinationPath!, FileMode.Create, FileAccess.Write);
            using StreamWriter writer = new StreamWriter(destinationStream, encoding);
            int recordedLines = 0;

            string? line;
            while ((line = reader.ReadLine()) != null)
            {
                writer.Write(line);

                if (reader.Peek() >= 0)
                {
                    writer.WriteLine();
                }

                recordedLines++;
            }

            return recordedLines;
        }

        /// <summary>
        /// Reads file content encoded with non Unicode encoding.
        /// </summary>
        /// <param name="sourcePath">Path to source file.</param>
        /// <param name="encoding">Encoding name.</param>
        /// <returns>Uncoding file content.</returns>
        /// <exception cref="ArgumentException">Throw if path to source file or encoding string is null or empty.</exception>
        /// <exception cref="FileNotFoundException">Throw if source file doesn't exist.</exception>
        public static string ReadEncodedText(string? sourcePath, string? encoding)
        {
            InputValidation(sourcePath);

            InputValidation(sourcePath, encoding);

#pragma warning disable CS8604
            Encoding selectedEncoding = Encoding.GetEncoding(encoding);
#pragma warning restore CS8604
#pragma warning disable CS8604
            using FileStream sourceStream = new FileStream(sourcePath, FileMode.Open);
            using StreamReader reader = new StreamReader(sourceStream, selectedEncoding);
            return reader.ReadToEnd();
#pragma warning restore CS8604
        }

        /// <summary>
        /// Returns decompressed stream from file.
        /// </summary>
        /// <param name="sourcePath">Path to source file.</param>
        /// <param name="method">Method used for compression (none, deflate, gzip).</param>
        /// <returns>Output stream.</returns>
        /// <exception cref="ArgumentException">Throw if path to source file is null or empty.</exception>
        /// <exception cref="FileNotFoundException">Throw if source file doesn't exist.</exception>
        public static Stream DecompressStream(string? sourcePath, DecompressionMethods? method)
        {
            InputValidation(sourcePath);

#pragma warning disable CS8604
            Stream inputStream = new FileStream(sourcePath, FileMode.Open);
#pragma warning restore CS8604

#pragma warning disable IDE0066
            switch (method)
            {
                case DecompressionMethods.Deflate:
                    return new DeflateStream(inputStream, CompressionMode.Decompress);
                case DecompressionMethods.GZip:
                    return new GZipStream(inputStream, CompressionMode.Decompress);
                case DecompressionMethods.None:
                    return inputStream;
                case DecompressionMethods.Brotli:
                    return new BrotliStream(inputStream, CompressionMode.Decompress);
                default:
                    throw new ArgumentException("Invalid compression method specified.");
            }
#pragma warning restore IDE0066
        }

        /// <summary>
        /// Calculates hash of stream using specified algorithm.
        /// </summary>
        /// <param name="stream">Source stream.</param>
        /// <param name="hashAlgorithmName">
        ///     Hash algorithm ("MD5","SHA1","SHA256" and other supported by .NET).
        /// </param>
        /// <returns>Hash.</returns>
        public static string CalculateHash(this Stream? stream, string? hashAlgorithmName)
        {
            if (stream == null)
            {
                throw new ArgumentNullException(nameof(stream), "Source stream cannot be null.");
            }

            if (string.IsNullOrEmpty(hashAlgorithmName))
            {
                throw new ArgumentException("Hash algorithm name cannot be null or empty.", nameof(hashAlgorithmName));
            }

            using HashAlgorithm hashAlgorithm = HashAlgorithm.Create(hashAlgorithmName) ?? throw new ArgumentException("Unsupported hash algorithm.", nameof(hashAlgorithmName));
            byte[] hashBytes = hashAlgorithm.ComputeHash(stream);
            StringBuilder hashBuilder = new StringBuilder();

            foreach (byte b in hashBytes)
            {
                hashBuilder.Append(b.ToString("X2", CultureInfo.InvariantCulture));
            }

            return hashBuilder.ToString();
        }

        private static void InputValidation(string? sourcePath, string? destinationPath)
        {
            if (string.IsNullOrWhiteSpace(sourcePath))
            {
                throw new ArgumentException($"{nameof(sourcePath)} cannot be null or empty or whitespace.", nameof(sourcePath));
            }

            if (!File.Exists(sourcePath))
            {
                throw new FileNotFoundException($"File '{sourcePath}' not found. Parameter name: {nameof(sourcePath)}.");
            }

            if (string.IsNullOrWhiteSpace(destinationPath))
            {
                throw new ArgumentException($"{nameof(destinationPath)} cannot be null or empty or whitespace", nameof(destinationPath));
            }
        }

        private static void InputValidation(string? sourcePath)
        {
            if (string.IsNullOrWhiteSpace(sourcePath))
            {
                throw new ArgumentException($"{nameof(sourcePath)} cannot be null or empty or whitespace.", nameof(sourcePath));
            }

            if (!File.Exists(sourcePath))
            {
                throw new FileNotFoundException($"File '{sourcePath}' not found. Parameter name: {nameof(sourcePath)}.");
            }
        }
    }
}
