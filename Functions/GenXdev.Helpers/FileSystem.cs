// ################################################################################
// Part of PowerShell module : GenXdev.Helpers
// Original cmdlet filename  : FileSystem.cs
// Original author           : René Vaessen / GenXdev
// Version                   : 1.280.2025
// ################################################################################
// MIT License
//
// Copyright 2021-2025 GenXdev
//
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
//
// The above copyright notice and this permission notice shall be included in all
// copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
// SOFTWARE.
// ################################################################################



using System.Runtime.InteropServices;
using System.Text.RegularExpressions;

namespace GenXdev.Helpers
{
    public static class FileSystem
    {
        /// <summary>
        /// Flags for controlling file move operations at the Windows API level.
        /// </summary>
        [Flags]
        internal enum MoveFileFlags
        {
            None = 0,
            ReplaceExisting = 1,
            CopyAllowed = 2,
            DelayUntilReboot = 4,
            WriteThrough = 8,
            CreateHardlink = 16,
            FailIfNotTrackable = 32,
        }

        /// <summary>
        /// Native Windows API methods for advanced file operations.
        /// </summary>
        internal static class NativeMethods
        {
            /// <summary>
            /// Moves a file using extended options like delayed reboot deletion.
            /// </summary>
            [DllImport("kernel32.dll", SetLastError = true, CharSet = CharSet.Unicode)]
            public static extern bool MoveFileEx(
                string lpExistingFileName,
                string lpNewFileName,
                MoveFileFlags dwFlags);
        }

        /// <summary>
        /// Tests if a filename matches a wildcard pattern using regex conversion.
        /// </summary>
        /// <param name="filename">The filename to test.</param>
        /// <param name="filemask">The wildcard pattern (e.g., "*.txt").</param>
        /// <returns>True if the filename matches the pattern.</returns>
        public static bool FileNameFitsMask(string filename, string filemask)
        {
            // extract just the filename part for pattern matching
            filename = Path.GetFileName(filename);
            try
            {
                // convert wildcard pattern to regex (escape dots, convert * and ?)
                Regex mask = new Regex(filemask.Replace(".", "[.]").Replace("*", ".*").Replace("?", "."));

                return mask.IsMatch(filename);
            }
            catch
            {
                // invalid pattern syntax returns no match
                return false;
            }
        }

        /// <summary>
        /// Moves a file forcibly and updates the source path reference.
        /// </summary>
        /// <param name="sourceFilePath">Reference to the source file path (updated on success).</param>
        /// <param name="targetFilePath">The destination file path.</param>
        /// <param name="deleteDirIfEmpty">Whether to clean up empty source directories.</param>
        /// <returns>True if the move operation succeeded.</returns>
        public static bool ForciblyMoveFile(ref string sourceFilePath, string targetFilePath, bool deleteDirIfEmpty)
        {
            // attempt the move and update the reference if successful
            if (ForciblyMoveFile(sourceFilePath, targetFilePath, deleteDirIfEmpty))
            {
                sourceFilePath = targetFilePath;
                return true;
            }

            return false;
        }

        /// <summary>
        /// Moves a file forcibly using multiple fallback strategies for locked files.
        /// </summary>
        /// <param name="sourceFilePath">The source file path.</param>
        /// <param name="targetFilePath">The destination file path.</param>
        /// <param name="deleteDirIfEmpty">Whether to clean up empty source directories.</param>
        /// <returns>True if the move operation succeeded.</returns>
        public static bool ForciblyMoveFile(string sourceFilePath, string targetFilePath, bool deleteDirIfEmpty)
        {
            // prepare target location (create directories, remove conflicts)
            ForciblyPrepareTargetFilePath(targetFilePath);

            try
            {
                try
                {
                    // attempt simple move first
                    File.Move(sourceFilePath, targetFilePath);
                    return true;
                }
                catch
                {
                    // file may be locked, try taking ownership (commented out)
                    // TakeOwnership(sourceFilePath);
                }

                // prepare target again and retry move
                ForciblyPrepareTargetFilePath(targetFilePath);
                File.Move(sourceFilePath, targetFilePath);

                return true;
            }
            catch
            {
                // all move attempts failed
                return false;
            }
            finally
            {
                // clean up empty source directory if requested
                if (deleteDirIfEmpty)
                {
                    DeleteDirectoryIfEmpty(sourceFilePath);
                }
            }
        }

        /// <summary>
        /// Prepares a target file path by creating directories and optionally removing existing files.
        /// </summary>
        /// <param name="targetFilePath">The target file path to prepare.</param>
        /// <param name="deleteIfExists">Whether to delete existing files at the target path.</param>
        public static void ForciblyPrepareTargetFilePath(string targetFilePath, bool deleteIfExists = true)
        {
            // ensure target directory exists
            ForciblyPrepareTargetDirectory(Path.GetDirectoryName(targetFilePath));

            // remove existing file if requested
            if (deleteIfExists && File.Exists(targetFilePath))
            {
                try
                {
                    ForciblyDeleteFile(targetFilePath, false);
                }
                catch { }
            }
        }

        /// <summary>
        /// Creates a target directory if it doesn't exist, handling permission issues.
        /// </summary>
        /// <param name="targetDirectory">The directory path to create.</param>
        public static void ForciblyPrepareTargetDirectory(string targetDirectory)
        {
            // skip if directory path is empty
            if (String.IsNullOrWhiteSpace(targetDirectory))
                return;

            if (!Directory.Exists(targetDirectory))
            {
                try
                {
                    // attempt to create the directory
                    Directory.CreateDirectory(targetDirectory);
                }
                catch { }
            }
            else
            {
                // directory exists, may need ownership (commented out)
                // TakeOwnership(targetDirectory);
            }
        }

        /// <summary>
        /// Generates a unique temporary file path in the specified directory.
        /// </summary>
        /// <param name="directory">The directory for the temp file (uses system temp if null).</param>
        /// <returns>Path to a newly prepared temporary file.</returns>
        public static string GetTempFileName(string directory = null)
        {
            // use system temp directory if none specified
            if (String.IsNullOrWhiteSpace(directory))
            {
                directory = Path.GetTempPath();
            }

            // create unique filename with random component
            var result = Path.Combine(directory, "." + Path.GetRandomFileName() + ".tmp");

            // ensure the file can be created
            ForciblyPrepareTargetFilePath(result);

            return result;
        }

        /// <summary>
        /// Creates a temporary file stream with specified options.
        /// </summary>
        /// <param name="fileOptions">File options for the stream.</param>
        /// <param name="directory">Directory for the temp file.</param>
        /// <returns>FileStream for the temporary file.</returns>
        public static FileStream GetTempFileStream(FileOptions fileOptions = FileOptions.None, string directory = null)
        {
            // get unique temp file path
            var filePath = GetTempFileName(directory);
            var fileInfo = new FileInfo(filePath);

            // create file stream with specified options
            var stream =
                new FileStream(
                    filePath,
                    FileMode.Create,
                    FileAccess.Write,
                    FileShare.None,
                    4096,
                    fileOptions
                );

            // optimize temporary files for memory caching
            if ((fileOptions & (FileOptions.DeleteOnClose | FileOptions.RandomAccess)) == (FileOptions.DeleteOnClose | FileOptions.RandomAccess))
            {
                // Set the Attribute property of this file to Temporary.
                // Although this is not completely necessary, the .NET Framework is able
                // to optimize the use of Temporary files by keeping them cached in memory.
                fileInfo.Attributes = FileAttributes.Temporary;
            }

            return stream;
        }

        /// <summary>
        /// Gets disk free space information using Windows API.
        /// </summary>
        /// <param name="lpDirectoryName">Directory on the disk to check.</param>
        /// <param name="lpFreeBytesAvailable">Available bytes for current user.</param>
        /// <param name="lpTotalNumberOfBytes">Total bytes on disk.</param>
        /// <param name="lpTotalNumberOfFreeBytes">Total free bytes on disk.</param>
        /// <returns>True if the operation succeeded.</returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Globalization", "CA2101:SpecifyMarshalingForPInvokeStringArguments", MessageId = "0"), System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1060:MovePInvokesToNativeMethodsClass"), DllImport("kernel32.dll", SetLastError = true, CharSet = CharSet.Auto)]
        [return: MarshalAs(UnmanagedType.Bool)]
        static extern bool GetDiskFreeSpaceEx(string lpDirectoryName,
       out ulong lpFreeBytesAvailable,
       out ulong lpTotalNumberOfBytes,
       out ulong lpTotalNumberOfFreeBytes);

        /// <summary>
        /// Retrieves the free bytes available on the disk containing the specified folder.
        /// </summary>
        /// <param name="folderName">Path to a folder on the target disk.</param>
        /// <param name="freespace">Output parameter for available free bytes.</param>
        /// <returns>True if the free space was successfully retrieved.</returns>
        public static bool DriveFreeBytes(string folderName, out ulong freespace)
        {
            // initialize output
            freespace = 0;

            // validate input
            if (string.IsNullOrEmpty(folderName))
            {
                throw new ArgumentNullException("folderName");
            }

            // ensure path ends with backslash for API
            if (!folderName.EndsWith("\\"))
            {
                folderName += '\\';
            }

            // call Windows API to get disk space info
            ulong free = 0, dummy1 = 0, dummy2 = 0;

            if (GetDiskFreeSpaceEx(folderName, out free, out dummy1, out dummy2))
            {
                freespace = free;
                return true;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// Recursively deletes all empty subdirectories within a root directory.
        /// </summary>
        /// <param name="rootDirectory">The root directory to clean up.</param>
        public static void ForciblyDeleteAllEmptySubDirectories(string rootDirectory)
        {
            try
            {
                // process each subdirectory if root exists
                if (Directory.Exists(rootDirectory))
                    foreach (var directory in Directory.GetDirectories(rootDirectory, "*", SearchOption.TopDirectoryOnly))
                    {
                        // recursively clean subdirectories first
                        ForciblyDeleteAllEmptySubDirectories(directory);

                        try
                        {
                            // attempt to delete if empty
                            Directory.Delete(directory, false);
                        }
                        catch
                        {
                            // directory not empty, skip
                        }
                    }
            }
            catch
            {
                // ignore errors during cleanup
            }
        }

        /// <summary>
        /// Deletes a file using multiple fallback strategies for locked or protected files.
        /// </summary>
        /// <param name="filepath">The file path to delete.</param>
        /// <param name="DeleteDirIfEmpty">Whether to clean up empty parent directories.</param>
        /// <returns>True if the file was successfully deleted.</returns>
        public static bool ForciblyDeleteFile(string filepath, bool DeleteDirIfEmpty)
        {
            // file already gone
            if (!File.Exists(filepath))
                return true;

            try
            {
                try
                {
                    // attempt simple delete first
                    File.Delete(filepath);

                    return true;
                }
                catch
                {
                    // file may be locked, try taking ownership (commented out)
                    // // TakeOwnership(filepath);
                }

                try
                {
                    // retry simple delete after ownership attempt
                    File.Delete(filepath);

                    return true;
                }
                catch
                {
                    // ownership didn't help
                }

                try
                {
                    // try recycle bin deletion as fallback
                    Microsoft.VisualBasic.FileIO.FileSystem.DeleteFile(
                        filepath,
                        0,
                        Microsoft.VisualBasic.FileIO.RecycleOption.SendToRecycleBin,
                        Microsoft.VisualBasic.FileIO.UICancelOption.ThrowException
                        );

                    // check if deletion succeeded
                    if (!File.Exists(filepath))
                        return true;
                }
                catch
                {
                    // recycle bin not available
                }

                try
                {
                    // last resort: move to temp location and schedule for reboot deletion
                    if (filepath.ToLowerInvariant().EndsWith(".deleted_-"))
                    {
                        // clean up previously failed deletion attempts
                        var parts = filepath.Split('.').ToList<string>();

                        parts.RemoveAt(parts.Count - 1);
                        parts.RemoveAt(parts.Count - 1);

                        filepath = string.Join(".", parts.ToArray<string>());
                    }

                    // create unique destination name for failed deletion
                    var destination = filepath + "." +
                        Guid.NewGuid().ToString().Replace("-", "").ToLowerInvariant() +
                        ".deleted_-";

                    // move file and schedule reboot deletion
                    if (ForciblyMoveFile(filepath, destination, false))
                    {
                        NativeMethods.MoveFileEx(destination, null, MoveFileFlags.DelayUntilReboot);
                    }

                    // check if file is gone
                    if (!File.Exists(filepath))
                        return true;
                }
                catch
                {
                    // all deletion methods failed
                }

                // return success status based on file existence
                return !File.Exists(filepath);
            }
            catch
            {
                // unexpected error, check if file still exists
                return !File.Exists(filepath);
            }
            finally
            {
                // clean up empty parent directory if requested
                if (DeleteDirIfEmpty)
                {
                    var dir = Path.GetDirectoryName(filepath);

                    ForciblyDeleteDirIfEmpty(dir);
                }
            }
        }

        /// <summary>
        /// Deletes a directory if it becomes empty after cleaning up subdirectories.
        /// </summary>
        /// <param name="dir">The directory path to clean up.</param>
        public static void ForciblyDeleteDirIfEmpty(string dir)
        {
            // first clean up any empty subdirectories
            ForciblyDeleteAllEmptySubDirectories(dir);

            // then try to delete the directory itself
            DeleteDirectoryIfEmpty(dir);
        }

        /// <summary>
        /// Deletes a directory only if it exists and is empty.
        /// </summary>
        /// <param name="path">The directory path to delete.</param>
        public static void DeleteDirectoryIfEmpty(string path)
        {
            try
            {
                // if path is a file, get its directory
                if (File.Exists(path))
                {
                    path = Path.GetDirectoryName(path);
                }

                // delete directory if it exists (will fail if not empty)
                if (Directory.Exists(path))
                {
                    Directory.Delete(path);
                }
            }
            catch { }
        }

        /// <summary>
        /// Checks if a file is currently in use by attempting to open it exclusively.
        /// </summary>
        /// <param name="fullPath">The full path to the file to check.</param>
        /// <returns>True if the file is in use (locked by another process).</returns>
        public static bool FileIsInUse(string fullPath)
        {
            // file doesn't exist, so not in use
            if (!File.Exists(fullPath))
                return false;

            // directories are not "in use" in this context
            if (Directory.Exists(fullPath))
                return false;

            try
            {
                // attempt exclusive access - if successful, file is not in use
                (new FileStream(fullPath, FileMode.Open, FileAccess.ReadWrite, FileShare.None)).Close();

                return false;
            }
            catch
            {
                // failed to get exclusive access, file is in use
                return true;
            }
        }

        /// <summary>
        /// Tests if a filename matches any of the provided wildcard patterns.
        /// </summary>
        /// <param name="filename">The filename to test.</param>
        /// <param name="filemasks">Array of wildcard patterns to test against.</param>
        /// <returns>True if the filename matches any pattern.</returns>
        public static bool FileNameFitsMasks(string filename, string[] filemasks)
        {
            // check each mask until one matches
            foreach (var mask in filemasks)
                if (FileNameFitsMask(filename, mask))
                    return true;

            return false;
        }

        /// <summary>
        /// Deletes a directory and all its contents using multiple fallback strategies.
        /// </summary>
        /// <param name="path">The directory path to delete.</param>
        /// <returns>True if the directory was successfully deleted.</returns>
        public static bool ForciblyDeleteDirectory(string path)
        {
            // directory already gone
            if (!Directory.Exists(path))
                return true;

            try
            {
                try
                {
                    // attempt recursive delete first
                    Directory.Delete(path, true);
                }
                catch
                {
                    // directory may be protected, try taking ownership (commented out)
                    // TakeOwnership(path);
                }

                // retry recursive delete after ownership attempt
                Directory.Delete(path, true);

                return true;
            }
            catch
            {
                // all deletion attempts failed
                return false;
            }
        }
    }
}
