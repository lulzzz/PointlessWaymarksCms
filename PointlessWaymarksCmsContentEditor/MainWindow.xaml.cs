﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Windows.Controls;
using HtmlTableHelper;
using JetBrains.Annotations;
using Microsoft.EntityFrameworkCore;
using MvvmHelpers.Commands;
using Ookii.Dialogs.Wpf;
using PointlessWaymarksCmsData;
using PointlessWaymarksCmsData.Content;
using PointlessWaymarksCmsData.Database;
using PointlessWaymarksCmsData.Html;
using PointlessWaymarksCmsData.Html.CommonHtml;
using PointlessWaymarksCmsData.Json;
using PointlessWaymarksCmsWpfControls.Diagnostics;
using PointlessWaymarksCmsWpfControls.FileList;
using PointlessWaymarksCmsWpfControls.FilesWrittenLogList;
using PointlessWaymarksCmsWpfControls.GeoJsonList;
using PointlessWaymarksCmsWpfControls.HelpDisplay;
using PointlessWaymarksCmsWpfControls.ImageList;
using PointlessWaymarksCmsWpfControls.LineList;
using PointlessWaymarksCmsWpfControls.LinkList;
using PointlessWaymarksCmsWpfControls.MapComponentList;
using PointlessWaymarksCmsWpfControls.MenuLinkEditor;
using PointlessWaymarksCmsWpfControls.NoteList;
using PointlessWaymarksCmsWpfControls.PhotoList;
using PointlessWaymarksCmsWpfControls.PointList;
using PointlessWaymarksCmsWpfControls.PostList;
using PointlessWaymarksCmsWpfControls.Status;
using PointlessWaymarksCmsWpfControls.TagExclusionEditor;
using PointlessWaymarksCmsWpfControls.TagList;
using PointlessWaymarksCmsWpfControls.UserSettingsEditor;
using PointlessWaymarksCmsWpfControls.Utility.ThreadSwitcher;
using PointlessWaymarksCmsWpfControls.WpfHtml;

namespace PointlessWaymarksCmsContentEditor
{
    /// <summary>
    ///     Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : INotifyPropertyChanged
    {
        private FilesWrittenLogListContext _filesWrittenContext;
        private string _infoTitle;
        private string _recentSettingsFilesNames;
        private TabItem _selectedTab;
        private UserSettingsEditorContext _settingsEditorContext;
        private SettingsFileChooserControlContext _settingsFileChooser;
        private bool _showSettingsFileChooser;
        private HelpDisplayContext _softwareComponentsHelpContext;
        private StatusControlContext _statusContext;
        private FileListWithActionsContext _tabFileListContext;
        private GeoJsonListWithActionsContext _tabGeoJsonListContext;
        private ImageListWithActionsContext _tabImageListContext;
        private LineListWithActionsContext _tabLineListContext;
        private LinkListWithActionsContext _tabLinkContext;
        private MapComponentListWithActionsContext _tabMapListContext;
        private MenuLinkEditorContext _tabMenuLinkContext;
        private NoteListWithActionsContext _tabNoteListContext;
        private PhotoListWithActionsContext _tabPhotoListContext;
        private PointListWithActionsContext _tabPointListContext;
        private PostListWithActionsContext _tabPostListContext;
        private TagExclusionEditorContext _tabTagExclusionContext;
        private TagListContext _tabTagListContext;

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

            //Common
            GenerateChangedHtmlCommand = StatusContext.RunBlockingTaskCommand(GenerateChangedHtml);

            RemoveUnusedFilesFromMediaArchiveCommand =
                StatusContext.RunBlockingTaskCommand(RemoveUnusedFilesFromMediaArchive);

            RemoveUnusedFoldersAndFilesFromContentCommand =
                StatusContext.RunBlockingTaskCommand(RemoveUnusedFoldersAndFilesFromContent);

            GenerateIndexCommand = StatusContext.RunBlockingActionCommand(() =>
                HtmlGenerationGroups.GenerateIndex(null, StatusContext.ProgressTracker()));

            CheckAllContentForInvalidBracketCodeContentIdsCommand =
                StatusContext.RunBlockingTaskCommand(CheckAllContentForInvalidBracketCodeContentIds);

            //All/Forced Regeneration
            GenerateAllHtmlCommand = StatusContext.RunBlockingTaskCommand(GenerateAllHtml);

            ConfirmOrGenerateAllPhotosImagesFilesCommand =
                StatusContext.RunBlockingTaskCommand(ConfirmOrGenerateAllPhotosImagesFiles);

            DeleteAndResizePicturesCommand = StatusContext.RunBlockingTaskCommand(CleanAndResizePictures);

            //Diagnostics
            ToggleDiagnosticLoggingCommand = new Command(() =>
                UserSettingsSingleton.LogDiagnosticEvents = !UserSettingsSingleton.LogDiagnosticEvents);

            ExceptionEventsHtmlReportCommand =
                StatusContext.RunNonBlockingTaskCommand(Reports.ExceptionEventsHtmlReport);
            DiagnosticEventsHtmlReportCommand =
                StatusContext.RunNonBlockingTaskCommand(Reports.DiagnosticEventsHtmlReport);
            ExceptionEventsExcelReportCommand =
                StatusContext.RunNonBlockingTaskCommand(Reports.ExceptionEventsExcelReport);
            DiagnosticEventsExcelReportCommand =
                StatusContext.RunNonBlockingTaskCommand(Reports.DiagnosticEventsExcelReport);
            AllEventsHtmlReportCommand = StatusContext.RunNonBlockingTaskCommand(Reports.AllEventsHtmlReport);
            AllEventsExcelReportCommand = StatusContext.RunNonBlockingTaskCommand(Reports.AllEventsExcelReport);

            //Main Parts
            GenerateSiteResourcesCommand = StatusContext.RunBlockingTaskCommand(async () =>
                await FileManagement.WriteSiteResourcesToGeneratedSite(StatusContext.ProgressTracker()));

            WriteStyleCssFileCommand = StatusContext.RunBlockingTaskCommand(async () =>
                await FileManagement.WriteStylesCssToGeneratedSite(StatusContext.ProgressTracker()));

            GenerateHtmlForAllFileContentCommand = StatusContext.RunBlockingTaskCommand(async () =>
                await HtmlGenerationGroups.GenerateAllFileHtml(null, StatusContext.ProgressTracker()));

            GenerateHtmlForAllImageContentCommand = StatusContext.RunBlockingTaskCommand(async () =>
                await HtmlGenerationGroups.GenerateAllImageHtml(null, StatusContext.ProgressTracker()));

            GenerateHtmlForAllNoteContentCommand = StatusContext.RunBlockingTaskCommand(async () =>
                await HtmlGenerationGroups.GenerateAllNoteHtml(null, StatusContext.ProgressTracker()));

            GenerateHtmlForAllPhotoContentCommand = StatusContext.RunBlockingTaskCommand(async () =>
                await HtmlGenerationGroups.GenerateAllPhotoHtml(null, StatusContext.ProgressTracker()));

            GenerateHtmlForAllPostContentCommand = StatusContext.RunBlockingTaskCommand(async () =>
                await HtmlGenerationGroups.GenerateAllPostHtml(null, StatusContext.ProgressTracker()));

            GenerateHtmlForAllPointContentCommand = StatusContext.RunBlockingTaskCommand(async () =>
                await HtmlGenerationGroups.GenerateAllPointHtml(null, StatusContext.ProgressTracker()));

            GenerateHtmlForAllLineContentCommand = StatusContext.RunBlockingTaskCommand(async () =>
                await HtmlGenerationGroups.GenerateAllLineHtml(null, StatusContext.ProgressTracker()));

            GenerateHtmlForAllGeoJsonContentCommand = StatusContext.RunBlockingTaskCommand(async () =>
                await HtmlGenerationGroups.GenerateAllGeoJsonHtml(null, StatusContext.ProgressTracker()));

            //Derived
            GenerateAllListHtmlCommand = StatusContext.RunBlockingActionCommand(() =>
                HtmlGenerationGroups.GenerateAllListHtml(null, StatusContext.ProgressTracker()));

            GenerateAllTagHtmlCommand = StatusContext.RunBlockingActionCommand(() =>
                HtmlGenerationGroups.GenerateAllTagHtml(null, StatusContext.ProgressTracker()));

            GenerateCameraRollCommand = StatusContext.RunBlockingTaskCommand(async () =>
                await HtmlGenerationGroups.GenerateCameraRollHtml(null, StatusContext.ProgressTracker()));

            GenerateDailyGalleryHtmlCommand = StatusContext.RunBlockingTaskCommand(async () =>
                await HtmlGenerationGroups.GenerateAllDailyPhotoGalleriesHtml(null, StatusContext.ProgressTracker()));

            //Rebuild
            ImportJsonFromDirectoryCommand = StatusContext.RunBlockingTaskCommand(ImportJsonFromDirectory);

            SettingsFileChooser = new SettingsFileChooserControlContext(StatusContext, RecentSettingsFilesNames);

            SettingsFileChooser.SettingsFileUpdated += SettingsFileChooserOnSettingsFileUpdatedEvent;

            StatusContext.RunFireAndForgetTaskWithUiToastErrorReturn(CleanupTemporaryFiles);
        }

        public Command AllEventsExcelReportCommand { get; set; }

        public Command AllEventsHtmlReportCommand { get; set; }

        public Command CheckAllContentForInvalidBracketCodeContentIdsCommand { get; set; }

        public Command ConfirmOrGenerateAllPhotosImagesFilesCommand { get; set; }

        public Command DeleteAndResizePicturesCommand { get; set; }

        public Command DiagnosticEventsExcelReportCommand { get; set; }

        public Command DiagnosticEventsHtmlReportCommand { get; set; }

        public Command ExceptionEventsExcelReportCommand { get; set; }

        public Command ExceptionEventsHtmlReportCommand { get; set; }

        public FilesWrittenLogListContext FilesWrittenContext
        {
            get => _filesWrittenContext;
            set
            {
                if (Equals(value, _filesWrittenContext)) return;
                _filesWrittenContext = value;
                OnPropertyChanged();
            }
        }

        public Command GenerateAllHtmlCommand { get; set; }

        public Command GenerateAllListHtmlCommand { get; set; }

        public Command GenerateAllTagHtmlCommand { get; set; }

        public Command GenerateCameraRollCommand { get; set; }

        public Command GenerateChangedHtmlCommand { get; set; }

        public Command GenerateDailyGalleryHtmlCommand { get; set; }

        public Command GenerateHtmlForAllFileContentCommand { get; set; }

        public Command GenerateHtmlForAllGeoJsonContentCommand { get; set; }

        public Command GenerateHtmlForAllImageContentCommand { get; set; }

        public Command GenerateHtmlForAllLineContentCommand { get; set; }

        public Command GenerateHtmlForAllNoteContentCommand { get; set; }

        public Command GenerateHtmlForAllPhotoContentCommand { get; set; }

        public Command GenerateHtmlForAllPointContentCommand { get; set; }

        public Command GenerateHtmlForAllPostContentCommand { get; set; }

        public Command GenerateIndexCommand { get; set; }

        public Command GenerateSiteResourcesCommand { get; set; }

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

        public Command RemoveUnusedFilesFromMediaArchiveCommand { get; set; }


        public Command RemoveUnusedFoldersAndFilesFromContentCommand { get; set; }

        public TabItem SelectedTab
        {
            get => _selectedTab;
            set
            {
                if (Equals(value, _selectedTab)) return;
                _selectedTab = value;
                OnPropertyChanged();

                LoadSelectedTabAsNeeded();
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

        public HelpDisplayContext SoftwareComponentsHelpContext
        {
            get => _softwareComponentsHelpContext;
            set
            {
                if (Equals(value, _softwareComponentsHelpContext)) return;
                _softwareComponentsHelpContext = value;
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

        public GeoJsonListWithActionsContext TabGeoJsonListContext
        {
            get => _tabGeoJsonListContext;
            set
            {
                if (Equals(value, _tabGeoJsonListContext)) return;
                _tabGeoJsonListContext = value;
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

        public LineListWithActionsContext TabLineListContext
        {
            get => _tabLineListContext;
            set
            {
                if (Equals(value, _tabLineListContext)) return;
                _tabLineListContext = value;
                OnPropertyChanged();
            }
        }

        public LinkListWithActionsContext TabLinkContext
        {
            get => _tabLinkContext;
            set
            {
                if (Equals(value, _tabLinkContext)) return;
                _tabLinkContext = value;
                OnPropertyChanged();
            }
        }

        public MapComponentListWithActionsContext TabMapListContext
        {
            get => _tabMapListContext;
            set
            {
                if (Equals(value, _tabMapListContext)) return;
                _tabMapListContext = value;
                OnPropertyChanged();
            }
        }

        public MenuLinkEditorContext TabMenuLinkContext
        {
            get => _tabMenuLinkContext;
            set
            {
                if (Equals(value, _tabMenuLinkContext)) return;
                _tabMenuLinkContext = value;
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

        public PointListWithActionsContext TabPointListContext
        {
            get => _tabPointListContext;
            set
            {
                if (Equals(value, _tabPointListContext)) return;
                _tabPointListContext = value;
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

        public TagExclusionEditorContext TabTagExclusionContext
        {
            get => _tabTagExclusionContext;
            set
            {
                if (Equals(value, _tabTagExclusionContext)) return;
                _tabTagExclusionContext = value;
                OnPropertyChanged();
            }
        }

        public TagListContext TabTagListContext
        {
            get => _tabTagListContext;
            set
            {
                if (Equals(value, _tabTagListContext)) return;
                _tabTagListContext = value;
                OnPropertyChanged();
            }
        }

        public Command ToggleDiagnosticLoggingCommand { get; set; }

        public Command WriteStyleCssFileCommand { get; set; }

        public Command WvTwoExperimentCommand { get; set; }

        public event PropertyChangedEventHandler PropertyChanged;

        private async Task CheckAllContentForInvalidBracketCodeContentIds()
        {
            await ThreadSwitcher.ResumeBackgroundAsync();

            var generationResults =
                await CommonContentValidation.CheckAllContentForBadContentReferences(StatusContext.ProgressTracker());

            if (generationResults.All(x => !x.HasError))
            {
                StatusContext.ToastSuccess("No problems with Bracket Code Content Ids Found!");
                return;
            }

            await Reports.InvalidBracketCodeContentIdsHtmlReport(generationResults);
        }

        private async Task CleanAndResizeAllImageFiles()
        {
            await ThreadSwitcher.ResumeBackgroundAsync();

            var results = await FileManagement.CleanAndResizeAllImageFiles(StatusContext.ProgressTracker());

            if (results.Any())
            {
                var frozenNow = DateTime.Now;

                var file = new FileInfo(Path.Combine(UserSettingsUtilities.TempStorageDirectory().FullName,
                    $"CleanAndResizeAllImageFiles-ErrorReport-{frozenNow:yyyy-MM-dd---HH-mm-ss}.htm"));

                var htmlString =
                    ($"<h1>Clean and Resize All Image Files Error Report - {frozenNow:yyyy-MM-dd---HH-mm-ss}</h1><br>" +
                     results.ToHtmlTable(new {@class = "pure-table pure-table-striped"}))
                    .ToHtmlDocumentWithPureCss("Clean and Resize Images Error Report", "body {margin: 12px;}");

                await File.WriteAllTextAsync(file.FullName, htmlString);

                var ps = new ProcessStartInfo(file.FullName) {UseShellExecute = true, Verb = "open"};

                Process.Start(ps);

                await StatusContext.ShowMessageWithOkButton("Image Clean and Resize All Errors",
                    $"There were {results.Count} errors while Cleaning and Resizing All Image Files - " +
                    $"an error file - {file.FullName} - has been generated and should open automatically in " +
                    "your browser.");
            }
        }


        private async Task CleanAndResizeAllPhotoFiles()
        {
            await ThreadSwitcher.ResumeBackgroundAsync();

            var results = await FileManagement.CleanAndResizeAllPhotoFiles(StatusContext.ProgressTracker());

            if (results.Any())
            {
                var frozenNow = DateTime.Now;

                var file = new FileInfo(Path.Combine(UserSettingsUtilities.TempStorageDirectory().FullName,
                    $"CleanAndResizeAllPhotoFiles-ErrorReport-{frozenNow:yyyy-MM-dd---HH-mm-ss}.htm"));

                var htmlString =
                    ($"<h1>Clean and Resize All Photo Files Error Report - {frozenNow:yyyy-MM-dd---HH-mm-ss}</h1><br>" +
                     results.ToHtmlTable(new {@class = "pure-table pure-table-striped"}))
                    .ToHtmlDocumentWithPureCss("Clean and Resize Photos Error Report", "body {margin: 12px;}");

                await File.WriteAllTextAsync(file.FullName, htmlString);

                var ps = new ProcessStartInfo(file.FullName) {UseShellExecute = true, Verb = "open"};

                Process.Start(ps);

                await StatusContext.ShowMessageWithOkButton("Photo Clean and Resize All Errors",
                    $"There were {results.Count} errors while Cleaning and Resizing All Photo Files - " +
                    $"an error file - {file.FullName} - has been generated and should open automatically in " +
                    "your browser.");
            }
        }

        private async Task CleanAndResizePictures()
        {
            await CleanAndResizeAllPhotoFiles();
            await CleanAndResizeAllImageFiles();
        }

        private async Task CleanupTemporaryFiles()
        {
            await ThreadSwitcher.ResumeBackgroundAsync();
            await FileManagement.CleanUpTemporaryFiles();
            await FileManagement.CleanupTemporaryHtmlFiles();
        }

        private async Task ConfirmAllFileContent()
        {
            await ThreadSwitcher.ResumeBackgroundAsync();

            var results = await FileManagement.ConfirmAllFileContentFilesArePresent(StatusContext.ProgressTracker());

            if (results.Any())
            {
                var frozenNow = DateTime.Now;

                var file = new FileInfo(Path.Combine(UserSettingsUtilities.TempStorageDirectory().FullName,
                    $"ConfirmFileContent-ErrorReport-{frozenNow:yyyy-MM-dd---HH-mm-ss}.htm"));

                var htmlString =
                    ($"<h1>Confirm All File Content Files Error Report - {frozenNow:yyyy-MM-dd---HH-mm-ss}</h1><br>" +
                     results.ToHtmlTable(new {@class = "pure-table pure-table-striped"}))
                    .ToHtmlDocumentWithPureCss("Confirm Files Error Report", "body {margin: 12px;}");

                await File.WriteAllTextAsync(file.FullName, htmlString);

                var ps = new ProcessStartInfo(file.FullName) {UseShellExecute = true, Verb = "open"};

                Process.Start(ps);

                await StatusContext.ShowMessageWithOkButton("File Content Files Check Errors",
                    $"There were {results.Count} errors while Confirming all File Content Files - " +
                    $"an error file - {file.FullName} - has been generated and should open automatically in " +
                    "your browser.");
            }
        }

        private async Task ConfirmAllImageContent()
        {
            await ThreadSwitcher.ResumeBackgroundAsync();

            var db = await Db.Context();

            var allItems = await db.ImageContents.ToListAsync();

            var loopCount = 1;
            var totalCount = allItems.Count;

            StatusContext.Progress($"Found {totalCount} Image to Confirm");

            foreach (var loopItem in allItems)
            {
                StatusContext.Progress($"Confirming Image Content for {loopItem.Title} - {loopCount} of {totalCount}");

                PictureAssetProcessing.ConfirmOrGenerateImageDirectoryAndPictures(loopItem,
                    StatusContext.ProgressTracker());

                loopCount++;
            }
        }

        private async Task ConfirmAllPhotoContent()
        {
            await ThreadSwitcher.ResumeBackgroundAsync();

            var db = await Db.Context();

            var allItems = await db.PhotoContents.ToListAsync();

            var loopCount = 1;
            var totalCount = allItems.Count;

            StatusContext.Progress($"Found {totalCount} Photos to Confirm");

            foreach (var loopItem in allItems)
            {
                StatusContext.Progress($"Confirming Photos for {loopItem.Title} - {loopCount} of {totalCount}");

                PictureAssetProcessing.ConfirmOrGeneratePhotoDirectoryAndPictures(loopItem,
                    StatusContext.ProgressTracker());

                loopCount++;
            }
        }

        private async Task ConfirmOrGenerateAllPhotosImagesFiles()
        {
            await ConfirmAllPhotoContent();
            await ConfirmAllImageContent();
            await ConfirmAllFileContent();

            StatusContext.ToastSuccess("All HTML Generation Finished");
        }

        private async Task GenerateAllHtml()
        {
            await ThreadSwitcher.ResumeBackgroundAsync();
            var generationResults = await HtmlGenerationGroups.GenerateAllHtml(StatusContext.ProgressTracker());

            if (generationResults.All(x => !x.HasError)) return;

            await Reports.InvalidBracketCodeContentIdsHtmlReport(generationResults);
        }

        private async Task GenerateChangedHtml()
        {
            await ThreadSwitcher.ResumeBackgroundAsync();
            var generationResults = await HtmlGenerationGroups.GenerateChangedToHtml(StatusContext.ProgressTracker());

            if (generationResults.All(x => !x.HasError)) return;

            await Reports.InvalidBracketCodeContentIdsHtmlReport(generationResults);
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
            settings.VerifyOrCreateAllTopLevelFolders(StatusContext.ProgressTracker());

            await settings.EnsureDbIsPresent(StatusContext.ProgressTracker());

            StatusContext.Progress("Setting up UI Controls");

            TabPostListContext = new PostListWithActionsContext(null);

            SettingsEditorContext =
                new UserSettingsEditorContext(StatusContext, UserSettingsSingleton.CurrentSettings());
            SoftwareComponentsHelpContext = new HelpDisplayContext(SoftwareUsedHelpMarkdown.HelpBlock);

            await ThreadSwitcher.ResumeForegroundAsync();

            MainTabControl.SelectedIndex = 0;

            StatusContext.RunFireAndForgetTaskWithUiToastErrorReturn(async () =>
                await EventLogContext.DeleteLogEntriesMoreThanMonthsOld(3));
        }

        private void LoadSelectedTabAsNeeded()
        {
            if (SelectedTab == null) return;

            if (SelectedTab.Header.ToString() == "Photos" && TabPhotoListContext == null)
                TabPhotoListContext = new PhotoListWithActionsContext(null);
            if (SelectedTab.Header.ToString() == "Images" && TabImageListContext == null)
                TabImageListContext = new ImageListWithActionsContext(null);
            if (SelectedTab.Header.ToString() == "Files" && TabFileListContext == null)
                TabFileListContext = new FileListWithActionsContext(null);
            if (SelectedTab.Header.ToString() == "Points" && TabPointListContext == null)
                TabPointListContext = new PointListWithActionsContext(null);
            if (SelectedTab.Header.ToString() == "Lines" && TabLineListContext == null)
                TabLineListContext = new LineListWithActionsContext(null);
            if (SelectedTab.Header.ToString() == "GeoJson" && TabGeoJsonListContext == null)
                TabGeoJsonListContext = new GeoJsonListWithActionsContext(null);
            if (SelectedTab.Header.ToString() == "Maps" && TabMapListContext == null)
                TabMapListContext = new MapComponentListWithActionsContext(null);
            if (SelectedTab.Header.ToString() == "Notes" && TabNoteListContext == null)
                TabNoteListContext = new NoteListWithActionsContext(null);
            if (SelectedTab.Header.ToString() == "Links" && TabLinkContext == null)
                TabLinkContext = new LinkListWithActionsContext(null);
            if (SelectedTab.Header.ToString() == "Tag Exclusions" && TabTagExclusionContext == null)
                TabTagExclusionContext = new TagExclusionEditorContext(null);
            if (SelectedTab.Header.ToString() == "Menu Links" && TabMenuLinkContext == null)
                TabMenuLinkContext = new MenuLinkEditorContext(null);
            if (SelectedTab.Header.ToString() == "Tags" && TabTagListContext == null)
                TabTagListContext = new TagListContext(null);
            if (SelectedTab.Header.ToString() == "File Log" && FilesWrittenContext == null)
                FilesWrittenContext = new FilesWrittenLogListContext(null, true);
        }

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        private async Task RemoveUnusedFilesFromMediaArchive()
        {
            await ThreadSwitcher.ResumeBackgroundAsync();
            await FileManagement.RemoveMediaArchiveFilesNotInDatabase(StatusContext.ProgressTracker());
        }

        private async Task RemoveUnusedFoldersAndFilesFromContent()
        {
            await ThreadSwitcher.ResumeBackgroundAsync();
            await FileManagement.RemoveContentDirectoriesAndFilesNotFoundInCurrentDatabase(
                StatusContext.ProgressTracker());
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

            if (fileList.Contains(UserSettingsUtilities.SettingsFileName))
                fileList.Remove(UserSettingsUtilities.SettingsFileName);

            fileList = new List<string> {UserSettingsUtilities.SettingsFileName}.Concat(fileList).ToList();

            if (fileList.Count > 10)
                fileList = fileList.Take(10).ToList();

            RecentSettingsFilesNames = string.Join("|", fileList);

            StatusContext.RunFireAndForgetBlockingTaskWithUiMessageReturn(LoadData);
        }

        private void SettingsFileChooserOnSettingsFileUpdatedEvent(object sender, (bool isNew, string userString) e)
        {
            StatusContext.RunFireAndForgetBlockingTaskWithUiMessageReturn(async () =>
                await SettingsFileChooserOnSettingsFileUpdated(e));
        }
    }
}