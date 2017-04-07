namespace Gadgeteer
{
    using Microsoft.SPOT;
    using Microsoft.SPOT.IO;
    using System;
    using System.IO;
    using System.Runtime.InteropServices;

    public class StorageDevice
    {
        public readonly string RootDirectory;
        public readonly VolumeInfo Volume;

        public StorageDevice(VolumeInfo volumeInfo)
        {
            this.Volume = volumeInfo;
            this.RootDirectory = this.Volume.RootDirectory;
        }

        public void CreateDirectory(string directoryPath)
        {
            Directory.CreateDirectory(Path.Combine(this.RootDirectory, directoryPath));
        }

        public void Delete(string filePath)
        {
            File.Delete(Path.Combine(this.RootDirectory, filePath));
        }

        public void DeleteDirectory(string dirPath, [Optional, DefaultParameterValue(false)] bool recursive)
        {
            Directory.Delete(Path.Combine(this.RootDirectory, dirPath), recursive);
        }

        public string[] ListDirectories(string path)
        {
            return this.RemoveRootDirectoryFromPaths(Directory.GetDirectories(Path.Combine(this.RootDirectory, path)));
        }

        public string[] ListFiles(string path)
        {
            return this.RemoveRootDirectoryFromPaths(Directory.GetFiles(Path.Combine(this.RootDirectory, path)));
        }

        public string[] ListRootDirectoryFiles()
        {
            return this.RemoveRootDirectoryFromPaths(Directory.GetFiles(this.RootDirectory));
        }

        public string[] ListRootDirectorySubdirectories()
        {
            return this.RemoveRootDirectoryFromPaths(Directory.GetDirectories(this.RootDirectory));
        }

        public Bitmap LoadBitmap(string filePath, Bitmap.BitmapImageType imageType)
        {
            return new Bitmap(File.ReadAllBytes(Path.Combine(this.RootDirectory, filePath)), imageType);
        }

        public FileStream Open(string filePath, FileMode mode, FileAccess access)
        {
            return File.Open(Path.Combine(this.RootDirectory, filePath), mode, access);
        }

        public FileStream OpenRead(string filePath)
        {
            return File.OpenRead(Path.Combine(this.RootDirectory, filePath));
        }

        public FileStream OpenWrite(string filePath)
        {
            return File.OpenWrite(Path.Combine(this.RootDirectory, filePath));
        }

        public byte[] ReadFile(string filePath)
        {
            return File.ReadAllBytes(Path.Combine(this.RootDirectory, filePath));
        }

        private string[] RemoveRootDirectoryFromPaths(string[] filePaths)
        {
            if (filePaths == null)
            {
                return null;
            }
            for (int i = 0; i < filePaths.Length; i++)
            {
                int index = filePaths[i].IndexOf(this.RootDirectory + Path.DirectorySeparatorChar.ToString());
                if (index >= 0)
                {
                    filePaths[i] = filePaths[i].Substring((index + this.RootDirectory.Length) + 1);
                }
            }
            return filePaths;
        }

        public void WriteFile(string filePath, byte[] fileData)
        {
            File.WriteAllBytes(Path.Combine(this.RootDirectory, filePath), fileData);
        }
    }
}

