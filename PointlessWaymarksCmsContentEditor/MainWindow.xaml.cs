﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using FluentMigrator.Runner;
using HtmlTableHelper;
using JetBrains.Annotations;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using MvvmHelpers.Commands;
using Ookii.Dialogs.Wpf;
using PointlessWaymarksCmsData;
using PointlessWaymarksCmsData.FileHtml;
using PointlessWaymarksCmsData.ImageHtml;
using PointlessWaymarksCmsData.IndexHtml;
using PointlessWaymarksCmsData.JsonFiles;
using PointlessWaymarksCmsData.LinkListHtml;
using PointlessWaymarksCmsData.NoteHtml;
using PointlessWaymarksCmsData.PhotoGalleryHtml;
using PointlessWaymarksCmsData.PhotoHtml;
using PointlessWaymarksCmsData.Pictures;
using PointlessWaymarksCmsData.PostHtml;
using PointlessWaymarksCmsData.SearchListHtml;
using PointlessWaymarksCmsWpfControls.FileContentEditor;
using PointlessWaymarksCmsWpfControls.FileList;
using PointlessWaymarksCmsWpfControls.HtmlViewer;
using PointlessWaymarksCmsWpfControls.ImageContentEditor;
using PointlessWaymarksCmsWpfControls.ImageList;
using PointlessWaymarksCmsWpfControls.LinkStreamEditor;
using PointlessWaymarksCmsWpfControls.LinkStreamList;
using PointlessWaymarksCmsWpfControls.NoteList;
using PointlessWaymarksCmsWpfControls.PhotoContentEditor;
using PointlessWaymarksCmsWpfControls.PhotoList;
using PointlessWaymarksCmsWpfControls.PostContentEditor;
using PointlessWaymarksCmsWpfControls.PostList;
using PointlessWaymarksCmsWpfControls.Status;
using PointlessWaymarksCmsWpfControls.TagExclusionEditor;
using PointlessWaymarksCmsWpfControls.UserSettingsEditor;
using PointlessWaymarksCmsWpfControls.Utility;

namespace PointlessWaymarksCmsContentEditor
{
    /// <summary>
    ///     Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : INotifyPropertyChanged
    {
        private Command _generateAllHtmlCommand;
        private Command _generateHtmlForAllFileContentCommand;
        private Command _generateHtmlForAllImageContentCommand;
        private Command _generateHtmlForAllPhotoContentCommand;
        private Command _generateHtmlForAllPostContentCommand;
        private Command _generateIndexCommand;
        private string _infoTitle;
        private LinkStreamListWithActionsContext _linkStreamContext;
        private Command _newFileContentCommand;
        private Command _newImageContentCommand;
        private Command _newLinkContentCommand;
        private Command _newPhotoContentCommand;
        private Command _newPostContentCommand;
        private Command _openIndexUrlCommand;
        private string _recentSettingsFilesNames;
        private UserSettingsEditorContext _settingsEditorContext;
        private SettingsFileChooserControlContext _settingsFileChooser;
        private bool _showSettingsFileChooser;
        private StatusControlContext _statusContext;
        private FileListWithActionsContext _tabFileListContext;
        private ImageListWithActionsContext _tabImageListContext;
        private NoteListWithActionsContext _tabNoteListContext;
        private PhotoListWithActionsContext _tabPhotoListContext;
        private PostListWithActionsContext _tabPostListContext;
        private TagExclusionEditorContext _tagExclusionContext;

        public MainWindow()
        {
            InitializeComponent();

            App.Tracker.Track(this);

            WindowInitialPositionHelpers.EnsureWindowIsVisible(this);

            InfoTitle =
                $"Pointless Waymarks CMS - Built On {GetBuildDate(Assembly.GetEntryAssembly())} - Commit {ThisAssembly.Git.Commit} {(ThisAssembly.Git.IsDirty ? "(Has Local Changes)" : string.Empty)}";

            ShowSettingsFileChooser = true;

            DataContext = this;

            StatusContext = new StatusControlContext();

            GenerateIndexCommand = new Command(() => StatusContext.RunNonBlockingTask(GenerateIndex));
            OpenIndexUrlCommand = new Command(() => StatusContext.RunNonBlockingTask(OpenIndexUrl));

            GenerateAllHtmlCommand = new Command(() => StatusContext.RunBlockingTask(GenerateAllHtml));
            GenerateAllHtmlAndCleanAndResizePicturesCommand = new Command(() =>
                StatusContext.RunBlockingTask(GenerateAllHtmlAndCleanAndResizePictures));
            CleanAndResizePicturesCommand = new Command(() => StatusContext.RunBlockingTask(CleanAndResizePictures));

            NewPhotoContentCommand = new Command(() => StatusContext.RunNonBlockingTask(NewPhotoContent));
            GenerateHtmlForAllPhotoContentCommand =
                new Command(() => StatusContext.RunBlockingTask(GenerateAllPhotoHtml));

            NewPostContentCommand = new Command(() => StatusContext.RunNonBlockingTask(NewPostContent));
            GenerateHtmlForAllPostContentCommand =
                new Command(() => StatusContext.RunBlockingTask(GenerateAllPostHtml));

            NewImageContentCommand = new Command(() => StatusContext.RunNonBlockingTask(NewImageContent));
            GenerateHtmlForAllImageContentCommand =
                new Command(() => StatusContext.RunBlockingTask(GenerateAllImageHtml));

            NewFileContentCommand = new Command(() => StatusContext.RunNonBlockingTask(NewFileContent));
            GenerateHtmlForAllFileContentCommand =
                new Command(() => StatusContext.RunBlockingTask(GenerateAllFileHtml));

            NewLinkContentCommand = new Command(() => StatusContext.RunNonBlockingTask(NewLinkContent));

            GenerateAllTagHtmlCommand = new Command(() => StatusContext.RunBlockingTask(GenerateAllTagHtml));

            ImportJsonFromDirectoryCommand = new Command(() => StatusContext.RunBlockingTask(ImportJsonFromDirectory));

            ToggleDiagnosticLoggingCommand = new Command(() =>
                UserSettingsSingleton.LogDiagnosticEvents = !UserSettingsSingleton.LogDiagnosticEvents);

            ExceptionEventsReportCommand = new Command(() => StatusContext.RunNonBlockingTask(ExceptionEventsReport));
            DiagnosticEventsReportCommand = new Command(() => StatusContext.RunNonBlockingTask(DiagnosticEventsReport));
            AllEventsReportCommand = new Command(() => StatusContext.RunNonBlockingTask(AllEventsReport));

            SettingsFileChooser = new SettingsFileChooserControlContext(StatusContext, RecentSettingsFilesNames);

            SettingsFileChooser.SettingsFileUpdated += SettingsFileChooserOnSettingsFileUpdatedEvent;

            StatusContext.RunFireAndForgetTaskWithUiToastErrorReturn(CleanUpTemporaryFiles);
        }


        public Command AllEventsReportCommand { get; set; }

        public Command CleanAndResizePicturesCommand { get; set; }

        public Command DiagnosticEventsReportCommand { get; set; }

        public Command ExceptionEventsReportCommand { get; set; }

        public Command GenerateAllHtmlAndCleanAndResizePicturesCommand { get; set; }

        public Command GenerateAllHtmlCommand
        {
            get => _generateAllHtmlCommand;
            set
            {
                if (Equals(value, _generateAllHtmlCommand)) return;
                _generateAllHtmlCommand = value;
                OnPropertyChanged();
            }
        }

        public Command GenerateAllTagHtmlCommand { get; set; }

        public Command GenerateHtmlForAllFileContentCommand
        {
            get => _generateHtmlForAllFileContentCommand;
            set
            {
                if (Equals(value, _generateHtmlForAllFileContentCommand)) return;
                _generateHtmlForAllFileContentCommand = value;
                OnPropertyChanged();
            }
        }


        public Command GenerateHtmlForAllImageContentCommand
        {
            get => _generateHtmlForAllImageContentCommand;
            set
            {
                if (Equals(value, _generateHtmlForAllImageContentCommand)) return;
                _generateHtmlForAllImageContentCommand = value;
                OnPropertyChanged();
            }
        }

        public Command GenerateHtmlForAllPhotoContentCommand
        {
            get => _generateHtmlForAllPhotoContentCommand;
            set
            {
                if (Equals(value, _generateHtmlForAllPhotoContentCommand)) return;
                _generateHtmlForAllPhotoContentCommand = value;
                OnPropertyChanged();
            }
        }

        public Command GenerateHtmlForAllPostContentCommand
        {
            get => _generateHtmlForAllPostContentCommand;
            set
            {
                if (Equals(value, _generateHtmlForAllPostContentCommand)) return;
                _generateHtmlForAllPostContentCommand = value;
                OnPropertyChanged();
            }
        }

        public Command GenerateIndexCommand
        {
            get => _generateIndexCommand;
            set
            {
                if (Equals(value, _generateIndexCommand)) return;
                _generateIndexCommand = value;
                OnPropertyChanged();
            }
        }

        public Command ImportJsonFromDirectoryCommand { get; set; }

        public string InfoTitle
        {
            get => _infoTitle;
            set
            {
                if (value == _infoTitle) return;
                _infoTitle = value;
                OnPropertyChanged();
            }
        }

        public LinkStreamListWithActionsContext LinkStreamContext
        {
            get => _linkStreamContext;
            set
            {
                if (Equals(value, _linkStreamContext)) return;
                _linkStreamContext = value;
                OnPropertyChanged();
            }
        }

        public Command NewFileContentCommand
        {
            get => _newFileContentCommand;
            set
            {
                if (Equals(value, _newFileContentCommand)) return;
                _newFileContentCommand = value;
                OnPropertyChanged();
            }
        }

        public Command NewImageContentCommand
        {
            get => _newImageContentCommand;
            set
            {
                if (Equals(value, _newImageContentCommand)) return;
                _newImageContentCommand = value;
                OnPropertyChanged();
            }
        }

        public Command NewLinkContentCommand
        {
            get => _newLinkContentCommand;
            set
            {
                if (Equals(value, _newLinkContentCommand)) return;
                _newLinkContentCommand = value;
                OnPropertyChanged();
            }
        }

        public Command NewPhotoContentCommand
        {
            get => _newPhotoContentCommand;
            set
            {
                if (Equals(value, _newPhotoContentCommand)) return;
                _newPhotoContentCommand = value;
                OnPropertyChanged();
            }
        }

        public Command NewPostContentCommand
        {
            get => _newPostContentCommand;
            set
            {
                if (Equals(value, _newPostContentCommand)) return;
                _newPostContentCommand = value;
                OnPropertyChanged();
            }
        }

        public Command OpenIndexUrlCommand
        {
            get => _openIndexUrlCommand;
            set
            {
                if (Equals(value, _openIndexUrlCommand)) return;
                _openIndexUrlCommand = value;
                OnPropertyChanged();
            }
        }

        public string RecentSettingsFilesNames
        {
            get => _recentSettingsFilesNames;
            set
            {
                if (Equals(value, _recentSettingsFilesNames)) return;
                _recentSettingsFilesNames = value;
                OnPropertyChanged();
            }
        }

        public UserSettingsEditorContext SettingsEditorContext
        {
            get => _settingsEditorContext;
            set
            {
                if (Equals(value, _settingsEditorContext)) return;
                _settingsEditorContext = value;
                OnPropertyChanged();
            }
        }

        public SettingsFileChooserControlContext SettingsFileChooser
        {
            get => _settingsFileChooser;
            set
            {
                if (Equals(value, _settingsFileChooser)) return;
                _settingsFileChooser = value;
                OnPropertyChanged();
            }
        }

        public bool ShowSettingsFileChooser
        {
            get => _showSettingsFileChooser;
            set
            {
                if (value == _showSettingsFileChooser) return;
                _showSettingsFileChooser = value;
                OnPropertyChanged();
            }
        }

        public StatusControlContext StatusContext
        {
            get => _statusContext;
            set
            {
                if (Equals(value, _statusContext)) return;
                _statusContext = value;
                OnPropertyChanged();
            }
        }

        public FileListWithActionsContext TabFileListContext
        {
            get => _tabFileListContext;
            set
            {
                if (Equals(value, _tabFileListContext)) return;
                _tabFileListContext = value;
                OnPropertyChanged();
            }
        }

        public ImageListWithActionsContext TabImageListContext
        {
            get => _tabImageListContext;
            set
            {
                if (Equals(value, _tabImageListContext)) return;
                _tabImageListContext = value;
                OnPropertyChanged();
            }
        }

        public NoteListWithActionsContext TabNoteListContext
        {
            get => _tabNoteListContext;
            set
            {
                if (Equals(value, _tabNoteListContext)) return;
                _tabNoteListContext = value;
                OnPropertyChanged();
            }
        }

        public PhotoListWithActionsContext TabPhotoListContext
        {
            get => _tabPhotoListContext;
            set
            {
                if (Equals(value, _tabPhotoListContext)) return;
                _tabPhotoListContext = value;
                OnPropertyChanged();
            }
        }

        public PostListWithActionsContext TabPostListContext
        {
            get => _tabPostListContext;
            set
            {
                if (Equals(value, _tabPostListContext)) return;
                _tabPostListContext = value;
                OnPropertyChanged();
            }
        }

        public TagExclusionEditorContext TagExclusionContext
        {
            get => _tagExclusionContext;
            set
            {
                if (Equals(value, _tagExclusionContext)) return;
                _tagExclusionContext = value;
                OnPropertyChanged();
            }
        }

        public Command ToggleDiagnosticLoggingCommand { get; set; }

        public event PropertyChangedEventHandler PropertyChanged;

        private async Task AllEventsReport()
        {
            var log = await Db.Log();

            var htmlTable = log.EventLogs.Take(5000).OrderByDescending(x => x.RecordedOn).ToList().ToHtmlTable();

            await ThreadSwitcher.ResumeForegroundAsync();

            var reportWindow = new HtmlViewerWindow(htmlTable);
            reportWindow.Show();
        }

        private async Task CleanAndResizeAllImageFiles()
        {
            await ThreadSwitcher.ResumeBackgroundAsync();

            var db = await Db.Context();

            var allItems = await db.ImageContents.ToListAsync();

            var loopCount = 1;
            var totalCount = allItems.Count;

            StatusContext.Progress($"Found {totalCount} Images to Clean and Resize");

            foreach (var loopItem in allItems)
            {
                StatusContext.Progress($"Clean and Resize for {loopItem.Title} - {loopCount} of {totalCount}");

                var fileCheck = PictureResizing.CopyCleanResizeImage(loopItem, StatusContext.ProgressTracker());

                if (!fileCheck.Item1)
                    await StatusContext.ShowMessage("File Error",
                        $"There was an error processing image {loopItem.Title} " +
                        $"- {fileCheck.Item2} - after you hit Ok processing will continue but" +
                        " the process may error...", new List<string> {"Ok"});

                loopCount++;
            }
        }


        private async Task CleanAndResizeAllPhotoFiles()
        {
            await ThreadSwitcher.ResumeBackgroundAsync();

            var db = await Db.Context();

            var allItems = await db.PhotoContents.ToListAsync();

            var loopCount = 1;
            var totalCount = allItems.Count;

            StatusContext.Progress($"Found {totalCount} Photos to Clean and Resize");

            foreach (var loopItem in allItems)
            {
                StatusContext.Progress($"Clean and Resize for {loopItem.Title} - {loopCount} of {totalCount}");

                var fileCheck = PictureResizing.CopyCleanResizePhoto(loopItem, StatusContext.ProgressTracker());

                if (!fileCheck.Item1)
                    await StatusContext.ShowMessage("File Error",
                        $"There was an error processing photo {loopItem.Title} " +
                        $"- {fileCheck.Item2} - after you hit Ok processing will continue but" +
                        " the process may error...", new List<string> {"Ok"});

                loopCount++;
            }
        }

        private async Task CleanAndResizePictures()
        {
            await CleanAndResizeAllPhotoFiles();
            await CleanAndResizeAllImageFiles();
        }

        private async Task CleanUpTemporaryFiles()
        {
            await ThreadSwitcher.ResumeBackgroundAsync();

            var temporaryDirectory = UserSettingsUtilities.TempStorageDirectory();

            if (!temporaryDirectory.Exists)
            {
                temporaryDirectory.Create();
                return;
            }

            var allFiles = temporaryDirectory.GetFiles().ToList();

            var frozenUtcNow = DateTime.UtcNow;

            foreach (var loopFiles in allFiles)
                try
                {
                    var creationDayDiff = frozenUtcNow.Subtract(loopFiles.CreationTimeUtc).Days;
                    var lastAccessDayDiff = frozenUtcNow.Subtract(loopFiles.LastAccessTimeUtc).Days;
                    var lastWriteDayDiff = frozenUtcNow.Subtract(loopFiles.LastWriteTimeUtc).Days;

                    if (creationDayDiff > 2 && lastAccessDayDiff > 2 && lastWriteDayDiff > 2)
                        loopFiles.Delete();
                }
                catch (Exception e)
                {
                    await EventLogContext.TryWriteExceptionToLog(e, StatusContext.StatusControlContextId.ToString(),
                        $"Could not delete temporary file - {e}");
                }
        }

        private async Task DiagnosticEventsReport()
        {
            var log = await Db.Log();

            var htmlTable = log.EventLogs.Where(x => x.Category == "Diagnostic" || x.Category == "Startup").Take(5000)
                .OrderByDescending(x => x.RecordedOn).ToList().ToHtmlTable();

            await ThreadSwitcher.ResumeForegroundAsync();

            var reportWindow = new HtmlViewerWindow(htmlTable);
            reportWindow.Show();
        }

        private async Task ExceptionEventsReport()
        {
            var log = await Db.Log();

            var htmlTable = log.EventLogs.Where(x => x.Category == "Exception" || x.Category == "Startup").Take(1000)
                .OrderByDescending(x => x.RecordedOn).ToList().ToHtmlTable();

            await ThreadSwitcher.ResumeForegroundAsync();

            var reportWindow = new HtmlViewerWindow(htmlTable);
            reportWindow.Show();
        }

        private async Task GenerateAllDailyPhotoGalleriesHtml()
        {
            await ThreadSwitcher.ResumeBackgroundAsync();

            var allPages = await DailyPhotoPageGenerators.DailyPhotoGalleries(StatusContext.ProgressTracker());

            allPages.ForEach(x => x.WriteLocalHtml());
        }

        private async Task GenerateAllFileHtml()
        {
            await ThreadSwitcher.ResumeBackgroundAsync();

            var db = await Db.Context();

            var allItems = await db.FileContents.ToListAsync();

            var loopCount = 1;
            var totalCount = allItems.Count;

            StatusContext.Progress($"Found {totalCount} Files to Generate");

            foreach (var loopItem in allItems)
            {
                StatusContext.Progress($"Writing HTML for {loopItem.Title} - {loopCount} of {totalCount}");

                var originalFileCheck = PictureResizing.CheckFileOriginalFileIsInMediaAndContentDirectories(loopItem);

                if (!originalFileCheck.Item1)
                    await StatusContext.ShowMessage("File Error",
                        $"There was an error processing file item {loopItem.Title} " +
                        $"- {originalFileCheck.Item2} - after you hit Ok processing will continue but" +
                        " the process may error...", new List<string> {"Ok"});

                var htmlModel = new SingleFilePage(loopItem);
                htmlModel.WriteLocalHtml();
                await Export.WriteLocalDbJson(loopItem, StatusContext.ProgressTracker());

                loopCount++;
            }
        }

        private async Task GenerateAllHtml()
        {
            await GenerateAllTagHtml();
            await GenerateAllImageHtml();
            await GenerateAllPhotoHtml();
            await GenerateAllFileHtml();
            await GenerateAllNoteHtml();
            await GenerateAllPostHtml();
            await GenerateAllListHtml();
            await GenerateAllDailyPhotoGalleriesHtml();
            await GenerateCameraRollHtml();
            await GenerateIndex();

            StatusContext.ToastSuccess("All HTML Generation Finished");
        }

        private async Task GenerateAllHtmlAndCleanAndResizePictures()
        {
            await CleanAndResizePictures();

            await GenerateAllHtml();
        }

        private async Task GenerateAllImageHtml()
        {
            await ThreadSwitcher.ResumeBackgroundAsync();

            var db = await Db.Context();

            var allItems = await db.ImageContents.ToListAsync();

            var loopCount = 1;
            var totalCount = allItems.Count;

            StatusContext.Progress($"Found {totalCount} Images to Generate");

            foreach (var loopItem in allItems)
            {
                StatusContext.Progress($"Writing HTML for {loopItem.Title} - {loopCount} of {totalCount}");

                var htmlModel = new SingleImagePage(loopItem);
                htmlModel.WriteLocalHtml();
                await Export.WriteLocalDbJson(loopItem);

                loopCount++;
            }
        }

        private async Task GenerateAllListHtml()
        {
            await ThreadSwitcher.ResumeBackgroundAsync();

            SearchListPageGenerators.WriteAllContentCommonSearchListHtml();
            SearchListPageGenerators.WriteFileContentListHtml();
            SearchListPageGenerators.WriteImageContentListHtml();
            SearchListPageGenerators.WritePhotoContentListHtml();
            SearchListPageGenerators.WritePostContentListHtml();
            SearchListPageGenerators.WriteNoteContentListHtml();

            var linkListPage = new LinkListPage();
            linkListPage.WriteLocalHtmlRssAndJson();
            Export.WriteLinkListJson();
        }

        private async Task GenerateAllNoteHtml()
        {
            await ThreadSwitcher.ResumeBackgroundAsync();

            var db = await Db.Context();

            var allItems = await db.NoteContents.ToListAsync();

            var loopCount = 1;
            var totalCount = allItems.Count;

            StatusContext.Progress($"Found {totalCount} Posts to Generate");

            foreach (var loopItem in allItems)
            {
                StatusContext.Progress(
                    $"Writing HTML for Note Dated {loopItem.CreatedOn:d} - {loopCount} of {totalCount}");

                var htmlModel = new SingleNotePage(loopItem);
                htmlModel.WriteLocalHtml();
                await Export.WriteLocalDbJson(loopItem);

                loopCount++;
            }
        }

        private async Task GenerateAllPhotoHtml()
        {
            await ThreadSwitcher.ResumeBackgroundAsync();

            var db = await Db.Context();

            var allItems = await db.PhotoContents.ToListAsync();

            var loopCount = 1;
            var totalCount = allItems.Count;

            StatusContext.Progress($"Found {totalCount} Photos to Generate");

            foreach (var loopItem in allItems)
            {
                StatusContext.Progress($"Writing HTML for {loopItem.Title} - {loopCount} of {totalCount}");

                var htmlModel = new SinglePhotoPage(loopItem);
                htmlModel.WriteLocalHtml();
                await Export.WriteLocalDbJson(loopItem);

                loopCount++;
            }
        }

        private async Task GenerateAllPostHtml()
        {
            await ThreadSwitcher.ResumeBackgroundAsync();

            var db = await Db.Context();

            var allItems = await db.PostContents.ToListAsync();

            var loopCount = 1;
            var totalCount = allItems.Count;

            StatusContext.Progress($"Found {totalCount} Posts to Generate");

            foreach (var loopItem in allItems)
            {
                StatusContext.Progress($"Writing HTML for {loopItem.Title} - {loopCount} of {totalCount}");

                var htmlModel = new SinglePostPage(loopItem);
                htmlModel.WriteLocalHtml();
                await Export.WriteLocalDbJson(loopItem);

                loopCount++;
            }
        }

        private async Task GenerateAllTagHtml()
        {
            await ThreadSwitcher.ResumeBackgroundAsync();

            SearchListPageGenerators.WriteTagListAndTagPages(StatusContext.ProgressTracker());
        }

        private async Task GenerateCameraRollHtml()
        {
            await ThreadSwitcher.ResumeBackgroundAsync();

            var cameraRollPage = await CameraRollGalleryPageGenerator.CameraRoll(StatusContext.ProgressTracker());

            cameraRollPage.WriteLocalHtml();
        }

        private async Task GenerateIndex()
        {
            await ThreadSwitcher.ResumeBackgroundAsync();

            var index = new IndexPage();
            index.WriteLocalHtml();

            StatusContext.ToastSuccess($"Generated {index.PageUrl}");
        }

        private static DateTime? GetBuildDate(Assembly assembly)
        {
            var attribute = assembly.GetCustomAttribute<BuildDateAttribute>();
            return attribute?.DateTime;
        }

        private async Task ImportJsonFromDirectory()
        {
            await ThreadSwitcher.ResumeForegroundAsync();

            StatusContext.Progress("Starting JSON load.");

            var dialog = new VistaFolderBrowserDialog();

            if (!(dialog.ShowDialog() ?? false)) return;

            var newDirectory = new DirectoryInfo(dialog.SelectedPath);

            if (!newDirectory.Exists)
            {
                StatusContext.ToastError("Directory doesn't exist?");
                return;
            }

            await ThreadSwitcher.ResumeBackgroundAsync();

            Import.FullImportFromRootDirectory(newDirectory, StatusContext.ProgressTracker());

            StatusContext.Progress("JSON Import Finished");
        }

        private async Task LoadData()
        {
            await ThreadSwitcher.ResumeBackgroundAsync();

            ShowSettingsFileChooser = false;

            var settings = await UserSettingsUtilities.ReadSettings(StatusContext.ProgressTracker());
            settings.VerifyOrCreateAllFolders();

            var possibleDbFile = new FileInfo(settings.DatabaseFile);

            if (possibleDbFile.Exists)
            {
                var sc = new ServiceCollection()
                    // Add common FluentMigrator services
                    .AddFluentMigratorCore().ConfigureRunner(rb => rb
                        // Add SQLite support to FluentMigrator
                        .AddSQLite()
                        // Set the connection string
                        .WithGlobalConnectionString($"Data Source={settings.DatabaseFile}")
                        // Define the assembly containing the migrations
                        .ScanIn(typeof(PointlessWaymarksContext).Assembly).For.Migrations())
                    // Enable logging to console in the FluentMigrator way
                    .AddLogging(lb => lb.AddFluentMigratorConsole())
                    // Build the service provider
                    .BuildServiceProvider(false);

                // Instantiate the runner
                var runner = sc.GetRequiredService<IMigrationRunner>();

                // Execute the migrations
                runner.MigrateUp();
            }

            StatusContext.Progress("Checking for database files...");
            var log = Db.Log().Result;
            await log.Database.EnsureCreatedAsync();
            await EventLogContext.TryWriteStartupMessageToLog(
                $"{InfoTitle} - Settings File {UserSettingsUtilities.SettingsFileName}",
                StatusContext.StatusControlContextId.ToString());

            var db = Db.Context().Result;
            await db.Database.EnsureCreatedAsync();

            StatusContext.Progress("Setting up UI Controls");

            TabImageListContext = new ImageListWithActionsContext(null);
            TabFileListContext = new FileListWithActionsContext(null);
            TabPhotoListContext = new PhotoListWithActionsContext(null);
            TabPostListContext = new PostListWithActionsContext(null);
            TabNoteListContext = new NoteListWithActionsContext(null);
            LinkStreamContext = new LinkStreamListWithActionsContext(null);
            TagExclusionContext = new TagExclusionEditorContext(null);
            SettingsEditorContext =
                new UserSettingsEditorContext(StatusContext, UserSettingsSingleton.CurrentSettings());
        }

        private async Task NewFileContent()
        {
            await ThreadSwitcher.ResumeForegroundAsync();

            var newContentWindow = new FileContentEditorWindow {Left = Left + 4, Top = Top + 4};
            newContentWindow.Show();
        }

        private async Task NewImageContent()
        {
            await ThreadSwitcher.ResumeForegroundAsync();

            var newContentWindow = new ImageContentEditorWindow {Left = Left + 4, Top = Top + 4};
            newContentWindow.Show();
        }

        private async Task NewLinkContent()
        {
            await ThreadSwitcher.ResumeForegroundAsync();

            var newContentWindow = new LinkStreamEditorWindow(null) {Left = Left + 4, Top = Top + 4};
            newContentWindow.Show();
        }

        private async Task NewPhotoContent()
        {
            await ThreadSwitcher.ResumeForegroundAsync();

            var newContentWindow = new PhotoContentEditorWindow {Left = Left + 4, Top = Top + 4};
            newContentWindow.Show();
        }

        private async Task NewPostContent()
        {
            await ThreadSwitcher.ResumeForegroundAsync();

            var newContentWindow = new PostContentEditorWindow(null) {Left = Left + 4, Top = Top + 4};
            newContentWindow.Show();
        }

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        private async Task OpenIndexUrl()
        {
            await ThreadSwitcher.ResumeBackgroundAsync();

            var settings = UserSettingsSingleton.CurrentSettings();

            var url = $@"http://{settings.SiteUrl}";

            var ps = new ProcessStartInfo(url) {UseShellExecute = true, Verb = "open"};
            Process.Start(ps);
        }

        private async Task SettingsFileChooserOnSettingsFileUpdated((bool isNew, string userInput) settingReturn)
        {
            await ThreadSwitcher.ResumeBackgroundAsync();

            if (string.IsNullOrWhiteSpace(settingReturn.userInput))
            {
                StatusContext.ToastError("Error with Settings File? No name?");
                return;
            }

            if (settingReturn.isNew)
                await UserSettingsUtilities.SetupNewSite(settingReturn.userInput, StatusContext.ProgressTracker());
            else
                UserSettingsUtilities.SettingsFileName = settingReturn.userInput;

            StatusContext.Progress($"Using {UserSettingsUtilities.SettingsFileName}");

            var fileList = RecentSettingsFilesNames?.Split("|").ToList() ?? new List<string>();

            if (!fileList.Contains(UserSettingsUtilities.SettingsFileName))
                fileList.Add(UserSettingsUtilities.SettingsFileName);

            if (fileList.Count > 10)
                fileList = fileList.Take(10).ToList();

            RecentSettingsFilesNames = string.Join("|", fileList);

            StatusContext.RunFireAndForgetBlockingTaskWithUiMessageReturn(LoadData);
        }

        private void SettingsFileChooserOnSettingsFileUpdatedEvent(object? sender, (bool isNew, string userString) e)
        {
            StatusContext.RunFireAndForgetBlockingTaskWithUiMessageReturn(async () =>
                await SettingsFileChooserOnSettingsFileUpdated(e));
        }
    }
}