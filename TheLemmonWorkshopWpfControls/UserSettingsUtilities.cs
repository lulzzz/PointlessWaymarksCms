﻿using System;
using System.IO;
using System.Text.Json;
using System.Threading.Tasks;

namespace TheLemmonWorkshopWpfControls
{
    public static class UserSettingsUtilities
    {
        public static async Task<UserSettings> ReadSettings()
        {
            var currentFile = SettingsFile();
            if (!currentFile.Exists)
            {
                await WriteSettings(new UserSettings());
                currentFile.Refresh();
            }

            return await JsonSerializer.DeserializeAsync<UserSettings>(File.OpenRead(currentFile.FullName));
        }

        public static FileInfo SettingsFile()
        {
            return new FileInfo(Path.Combine(StorageDirectory().FullName, "HikeLemmonWorkshopSettings.json"));
        }

        public static async Task<(bool, string)> ValidateLocalSiteRootDirectory()
        {
            var settings = await ReadSettings();

            if (string.IsNullOrWhiteSpace(settings.LocalSiteRootDirectory))
            {
                return (false, "No Local File Root User Setting Found");
            }

            try
            {
                var directory = new DirectoryInfo(settings.LocalSiteRootDirectory);
                if (!directory.Exists) directory.Create();
                directory.Refresh();
            }
            catch (Exception e)
            {
                return (false, "Trouble with Local File Root Directory.");
            }

            return (true, string.Empty);
        }

        public static async Task<(bool, string)> ValidateLocalMasterMediaArchive()
        {
            var settings = await ReadSettings();

            if (string.IsNullOrWhiteSpace(settings.LocalMasterMediaArchive))
            {
                return (false, "No Local File Root User Setting Found");
            }

            try
            {
                var directory = new DirectoryInfo(settings.LocalMasterMediaArchive);
                if (!directory.Exists) directory.Create();
                directory.Refresh();
            }
            catch (Exception e)
            {
                return (false, "Trouble with Local Mast Image Archive Directory.");
            }

            return (true, string.Empty);
        }

        public static DirectoryInfo StorageDirectory()
        {
            var storageDirectory =
                new DirectoryInfo(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments),
                    "HikeLemmon Workshop"));

            if (!storageDirectory.Exists) storageDirectory.Create();

            storageDirectory.Refresh();

            return storageDirectory;
        }

        public static void VerifyAndCreate()
        {
            ReadSettings();
        }

        public static async Task WriteSettings(UserSettings toWrite)
        {
            var currentFile = SettingsFile();
            await File.WriteAllTextAsync(currentFile.FullName, JsonSerializer.Serialize(toWrite));
        }
    }
}