﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Windows;
using JetBrains.Annotations;
using Microsoft.EntityFrameworkCore;
using MvvmHelpers.Commands;
using Omu.ValueInjecter;
using PointlessWaymarksCmsData;
using PointlessWaymarksCmsData.FileHtml;
using PointlessWaymarksCmsData.Models;
using PointlessWaymarksCmsWpfControls.FileContentEditor;
using PointlessWaymarksCmsWpfControls.Status;
using PointlessWaymarksCmsWpfControls.Utility;

namespace PointlessWaymarksCmsWpfControls.FileList
{
    public class FileListWithActionsContext : INotifyPropertyChanged
    {
        private Command _deleteSelectedCommand;
        private Command _editSelectedContentCommand;
        private Command _fileDownloadLinkCodesToClipboardForSelectedCommand;
        private Command _firstPagePreviewFromPdfToCairoCommand;
        private Command _generateSelectedHtmlCommand;
        private FileListContext _listContext;
        private Command _newContentCommand;
        private Command _openUrlForSelectedCommand;
        private List<(object, string)> _pdfPreviewGenerationErrorOutput = new List<(object, string)>();

        private List<(object, string)> _pdfPreviewGenerationProgress = new List<(object, string)>();
        private Command _photoPageLinkCodesToClipboardForSelectedCommand;
        private StatusControlContext _statusContext;

        public FileListWithActionsContext(StatusControlContext statusContext)
        {
            StatusContext = statusContext ?? new StatusControlContext();

            StatusContext.RunFireAndForgetBlockingTaskWithUiMessageReturn(LoadData);
        }

        public Command DeleteSelectedCommand
        {
            get => _deleteSelectedCommand;
            set
            {
                if (Equals(value, _deleteSelectedCommand)) return;
                _deleteSelectedCommand = value;
                OnPropertyChanged();
            }
        }

        public Command EditSelectedContentCommand
        {
            get => _editSelectedContentCommand;
            set
            {
                if (Equals(value, _editSelectedContentCommand)) return;
                _editSelectedContentCommand = value;
                OnPropertyChanged();
            }
        }

        public Command FileDownloadLinkCodesToClipboardForSelectedCommand
        {
            get => _fileDownloadLinkCodesToClipboardForSelectedCommand;
            set
            {
                if (Equals(value, _fileDownloadLinkCodesToClipboardForSelectedCommand)) return;
                _fileDownloadLinkCodesToClipboardForSelectedCommand = value;
                OnPropertyChanged();
            }
        }

        public Command FilePageLinkCodesToClipboardForSelectedCommand
        {
            get => _photoPageLinkCodesToClipboardForSelectedCommand;
            set
            {
                if (Equals(value, _photoPageLinkCodesToClipboardForSelectedCommand)) return;
                _photoPageLinkCodesToClipboardForSelectedCommand = value;
                OnPropertyChanged();
            }
        }

        public Command FirstPagePreviewFromPdfToCairoCommand
        {
            get => _firstPagePreviewFromPdfToCairoCommand;
            set
            {
                if (Equals(value, _firstPagePreviewFromPdfToCairoCommand)) return;
                _firstPagePreviewFromPdfToCairoCommand = value;
                OnPropertyChanged();
            }
        }


        public Command GenerateSelectedHtmlCommand
        {
            get => _generateSelectedHtmlCommand;
            set
            {
                if (Equals(value, _generateSelectedHtmlCommand)) return;
                _generateSelectedHtmlCommand = value;
                OnPropertyChanged();
            }
        }

        public FileListContext ListContext
        {
            get => _listContext;
            set
            {
                if (Equals(value, _listContext)) return;
                _listContext = value;
                OnPropertyChanged();
            }
        }

        public Command NewContentCommand
        {
            get => _newContentCommand;
            set
            {
                if (Equals(value, _newContentCommand)) return;
                _newContentCommand = value;
                OnPropertyChanged();
            }
        }

        public Command OpenUrlForSelectedCommand
        {
            get => _openUrlForSelectedCommand;
            set
            {
                if (Equals(value, _openUrlForSelectedCommand)) return;
                _openUrlForSelectedCommand = value;
                OnPropertyChanged();
            }
        }

        public Command RefreshDataCommand { get; set; }

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

        public event PropertyChangedEventHandler PropertyChanged;

        private async Task Delete()
        {
            await ThreadSwitcher.ResumeBackgroundAsync();

            var selected = ListContext.SelectedItems;

            if (selected == null || !selected.Any())
            {
                StatusContext.ToastError("Nothing Selected?");
                return;
            }

            if (selected.Count > 1)
            {
                StatusContext.ToastError("Sorry - please delete one at a time");
                return;
            }

            var selectedItem = selected.Single();

            if (selectedItem.DbEntry == null || selectedItem.DbEntry.Id < 1)
            {
                StatusContext.ToastError("Entry is not saved?");
                return;
            }

            var settings = UserSettingsSingleton.CurrentSettings();

            var possibleContentDirectory = settings.LocalSiteFileContentDirectory(selectedItem.DbEntry, false);
            if (possibleContentDirectory.Exists) possibleContentDirectory.Delete(true);

            var context = await Db.Context();

            var toHistoric = await context.FileContents.Where(x => x.ContentId == selectedItem.DbEntry.ContentId)
                .ToListAsync();

            foreach (var loopToHistoric in toHistoric)
            {
                var newHistoric = new HistoricFileContent();
                newHistoric.InjectFrom(loopToHistoric);
                newHistoric.Id = 0;
                await context.HistoricFileContents.AddAsync(newHistoric);
                context.FileContents.Remove(loopToHistoric);
            }

            await context.SaveChangesAsync(true);

            await LoadData();
        }

        private async Task EditSelectedContent()
        {
            await ThreadSwitcher.ResumeBackgroundAsync();

            if (ListContext.SelectedItems == null || !ListContext.SelectedItems.Any())
            {
                StatusContext.ToastError("Nothing Selected?");
                return;
            }

            var context = await Db.Context();
            var frozenList = ListContext.SelectedItems;

            foreach (var loopSelected in frozenList)
            {
                var refreshedData =
                    context.FileContents.SingleOrDefault(x => x.ContentId == loopSelected.DbEntry.ContentId);

                if (refreshedData == null)
                {
                    StatusContext.ToastError(
                        $"{loopSelected.DbEntry.Title} is no longer active in the database? Can not edit - " +
                        "look for a historic version...");
                    continue;
                }

                await ThreadSwitcher.ResumeForegroundAsync();

                var newContentWindow = new FileContentEditorWindow(refreshedData);

                newContentWindow.Show();

                await ThreadSwitcher.ResumeBackgroundAsync();
            }
        }

        private void ErrorOutputHandler(object sendingProcess, DataReceivedEventArgs outLine)
        {
            //* Do your stuff with the output (write to console/log/StringBuilder)
            _pdfPreviewGenerationErrorOutput.Add((sendingProcess, outLine.Data));
        }

        private async Task FileDownloadLinkCodesToClipboardForSelected()
        {
            await ThreadSwitcher.ResumeBackgroundAsync();

            if (ListContext.SelectedItems == null || !ListContext.SelectedItems.Any())
            {
                StatusContext.ToastError("Nothing Selected?");
                return;
            }

            var finalString = string.Empty;

            foreach (var loopSelected in ListContext.SelectedItems)
                finalString +=
                    @$"{{{{filedownloadlink {loopSelected.DbEntry.ContentId}; {loopSelected.DbEntry.Title}}}}}{Environment.NewLine}";

            await ThreadSwitcher.ResumeForegroundAsync();

            Clipboard.SetText(finalString);

            StatusContext.ToastSuccess($"To Clipboard {finalString}");
        }

        private async Task FilePageLinkCodesToClipboardForSelected()
        {
            await ThreadSwitcher.ResumeBackgroundAsync();

            if (ListContext.SelectedItems == null || !ListContext.SelectedItems.Any())
            {
                StatusContext.ToastError("Nothing Selected?");
                return;
            }

            var finalString = string.Empty;

            foreach (var loopSelected in ListContext.SelectedItems)
                finalString +=
                    @$"{{{{filelink {loopSelected.DbEntry.ContentId}; {loopSelected.DbEntry.Title}}}}}{Environment.NewLine}";

            await ThreadSwitcher.ResumeForegroundAsync();

            Clipboard.SetText(finalString);

            StatusContext.ToastSuccess($"To Clipboard {finalString}");
        }

        private async Task FirstPagePreviewFromPdfToCairo()
        {
            await ThreadSwitcher.ResumeBackgroundAsync();

            var selected = ListContext.SelectedItems;

            if (selected == null || !selected.Any())
            {
                StatusContext.ToastError("Nothing Selected?");
                return;
            }

            var popplerDirectory = UserSettingsSingleton.CurrentSettings().PdfToCairoExeDirectory;
            if (string.IsNullOrWhiteSpace(popplerDirectory))
            {
                StatusContext.ToastError(
                    "Sorry - this function requires that pdftocairo.exe be on the system - please set the directory... ");
                return;
            }

            var popplerDirectoryInfo = new DirectoryInfo(popplerDirectory);
            if (!popplerDirectoryInfo.Exists)
            {
                StatusContext.ToastError(
                    $"{popplerDirectoryInfo.FullName} doesn't exist? Check your pdftocairo bin directory setting.");
                return;
            }

            var pdfToCairoExe = new FileInfo(Path.Combine(popplerDirectoryInfo.FullName, "pdftocairo.exe"));
            if (!pdfToCairoExe.Exists)
                if (!popplerDirectoryInfo.Exists)
                {
                    StatusContext.ToastError(
                        $"{pdfToCairoExe.FullName} doesn't exist? Check your pdftocairo bin directory setting.");
                    return;
                }

            foreach (var loopSelected in selected)
            {
                var targetFile = new FileInfo(Path.Combine(
                    UserSettingsSingleton.CurrentSettings().LocalSiteFileContentDirectory(loopSelected.DbEntry)
                        .FullName, loopSelected.DbEntry.OriginalFileName));

                if (!targetFile.Extension.ToLower().Contains("pdf"))
                    StatusContext.ToastError(
                        $"Can only generate previews from PDFs - {loopSelected.DbEntry.OriginalFileName} skipped...");

                var destinationFile = new FileInfo(Path.Combine(
                    UserSettingsSingleton.CurrentSettings().LocalSiteFileContentDirectory(loopSelected.DbEntry)
                        .FullName, $"{Path.GetFileNameWithoutExtension(targetFile.Name)}-FirstPage.jpg"));

                if (destinationFile.Exists)
                {
                    destinationFile.Delete();
                    destinationFile.Refresh();
                }

                //https://stackoverflow.com/questions/4291912/process-start-how-to-get-the-output
                //* Create your Process
                var process = new Process
                {
                    StartInfo =
                    {
                        FileName = pdfToCairoExe.FullName,
                        Arguments = $"-jpeg -singlefile {targetFile.FullName} {destinationFile.FullName}",
                        UseShellExecute = false,
                        RedirectStandardOutput = true,
                        RedirectStandardError = true,
                        CreateNoWindow = true
                    }
                };
                //* Set ONLY ONE handler here.
                process.ErrorDataReceived += ErrorOutputHandler;
                process.OutputDataReceived += OutputHandler;
                //* Start process
                process.Start();
                process.WaitForExit();


                var anyProgressOutput = string.Join(Environment.NewLine,
                    _pdfPreviewGenerationProgress.Where(x => x.Item1 == process).Select(x => x.Item2));

                if (!string.IsNullOrWhiteSpace(anyProgressOutput))
                {
                    StatusContext.Progress(anyProgressOutput);
                    await EventLogContext.TryWriteDiagnosticMessageToLog(anyProgressOutput,
                        StatusContext.StatusControlContextId.ToString());
                }

                _pdfPreviewGenerationProgress.RemoveAll(x => x.Item1 == process);


                var anyErrorOutput = string.Join(Environment.NewLine,
                    _pdfPreviewGenerationErrorOutput.Where(x => x.Item1 == process).Select(x => x.Item2));

                if (!string.IsNullOrWhiteSpace(anyErrorOutput))
                {
                    await StatusContext.ShowMessageWithOkButton("Error Generating Preview", anyErrorOutput);
                    await EventLogContext.TryWriteDiagnosticMessageToLog(anyErrorOutput,
                        StatusContext.StatusControlContextId.ToString());
                }

                _pdfPreviewGenerationErrorOutput.RemoveAll(x => x.Item1 == process);
            }
        }

        private async Task GenerateSelectedHtml()
        {
            await ThreadSwitcher.ResumeBackgroundAsync();

            if (ListContext.SelectedItems == null || !ListContext.SelectedItems.Any())
            {
                StatusContext.ToastError("Nothing Selected?");
                return;
            }

            var loopCount = 1;
            var totalCount = ListContext.SelectedItems.Count;

            foreach (var loopSelected in ListContext.SelectedItems)
            {
                StatusContext.Progress(
                    $"Generating Html for {loopSelected.DbEntry.Title}, {loopCount} of {totalCount}");

                var htmlContext = new SingleFilePage(loopSelected.DbEntry);

                htmlContext.WriteLocalHtml();

                StatusContext.ToastSuccess($"Generated {htmlContext.PageUrl}");

                loopCount++;
            }
        }

        private async Task LoadData()
        {
            await ThreadSwitcher.ResumeBackgroundAsync();

            ListContext = new FileListContext(StatusContext);

            GenerateSelectedHtmlCommand = new Command(() => StatusContext.RunBlockingTask(GenerateSelectedHtml));
            EditSelectedContentCommand = new Command(() => StatusContext.RunBlockingTask(EditSelectedContent));
            FilePageLinkCodesToClipboardForSelectedCommand = new Command(() =>
                StatusContext.RunBlockingTask(FilePageLinkCodesToClipboardForSelected));
            FileDownloadLinkCodesToClipboardForSelectedCommand = new Command(() =>
                StatusContext.RunBlockingTask(FileDownloadLinkCodesToClipboardForSelected));
            OpenUrlForSelectedCommand = new Command(() => StatusContext.RunNonBlockingTask(OpenUrlForSelected));
            NewContentCommand = new Command(() => StatusContext.RunNonBlockingTask(NewContent));
            RefreshDataCommand = new Command(() => StatusContext.RunBlockingTask(ListContext.LoadData));
            DeleteSelectedCommand = new Command(() => StatusContext.RunBlockingTask(Delete));
            FirstPagePreviewFromPdfToCairoCommand =
                new Command(() => StatusContext.RunBlockingTask(FirstPagePreviewFromPdfToCairo));
        }

        private async Task NewContent()
        {
            await ThreadSwitcher.ResumeForegroundAsync();

            var newContentWindow = new FileContentEditorWindow(null);

            newContentWindow.Show();
        }

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        private async Task OpenUrlForSelected()
        {
            await ThreadSwitcher.ResumeBackgroundAsync();

            if (ListContext.SelectedItems == null || !ListContext.SelectedItems.Any())
            {
                StatusContext.ToastError("Nothing Selected?");
                return;
            }

            var settings = UserSettingsSingleton.CurrentSettings();

            foreach (var loopSelected in ListContext.SelectedItems)
            {
                var url = $@"http://{settings.FilePageUrl(loopSelected.DbEntry)}";

                var ps = new ProcessStartInfo(url) {UseShellExecute = true, Verb = "open"};
                Process.Start(ps);
            }
        }

        private void OutputHandler(object sendingProcess, DataReceivedEventArgs outLine)
        {
            //* Do your stuff with the output (write to console/log/StringBuilder)
            _pdfPreviewGenerationProgress.Add((sendingProcess, outLine.Data));
        }
    }
}