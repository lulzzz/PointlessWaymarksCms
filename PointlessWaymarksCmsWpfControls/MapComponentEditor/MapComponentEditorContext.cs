﻿#nullable enable
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using GongSolutions.Wpf.DragDrop;
using JetBrains.Annotations;
using Microsoft.EntityFrameworkCore;
using MvvmHelpers.Commands;
using PointlessWaymarksCmsData;
using PointlessWaymarksCmsData.Content;
using PointlessWaymarksCmsData.Database;
using PointlessWaymarksCmsData.Database.Models;
using PointlessWaymarksCmsData.Html.CommonHtml;
using PointlessWaymarksCmsWpfControls.ContentIdViewer;
using PointlessWaymarksCmsWpfControls.CreatedAndUpdatedByAndOnDisplay;
using PointlessWaymarksCmsWpfControls.Status;
using PointlessWaymarksCmsWpfControls.StringDataEntry;
using PointlessWaymarksCmsWpfControls.UpdateNotesEditor;
using PointlessWaymarksCmsWpfControls.Utility;
using PointlessWaymarksCmsWpfControls.Utility.ChangesAndValidation;
using PointlessWaymarksCmsWpfControls.Utility.ThreadSwitcher;

namespace PointlessWaymarksCmsWpfControls.MapComponentEditor
{
    public class MapComponentEditorContext : INotifyPropertyChanged, IHasChanges, IHasValidationIssues,
        ICheckForChangesAndValidation, IDropTarget
    {
        private ContentIdViewerControlContext? _contentId;
        private CreatedAndUpdatedByAndOnDisplayContext? _createdUpdatedDisplay;
        private List<MapElement> _dbElements = new();
        private MapComponent? _dbEntry;
        private bool _hasChanges;
        private bool _hasValidationIssues;
        private ObservableCollection<IMapElementListItem>? _mapElements;
        private Command _saveAndCloseCommand;
        private Command _saveCommand;

        private StatusControlContext _statusContext;
        private StringDataEntryContext? _summaryEntry;
        private StringDataEntryContext? _titleEntry;
        private UpdateNotesEditorContext? _updateNotes;
        private Command _userGeoContentIdInputToMapCommand;
        private string _userGeoContentInput = string.Empty;

        public EventHandler? RequestContentEditorWindowClose;

        private MapComponentEditorContext(StatusControlContext statusContext)
        {
            _statusContext = statusContext;

            _userGeoContentIdInputToMapCommand = StatusContext.RunBlockingTaskCommand(UserGeoContentIdInputToMap);
            _saveCommand = StatusContext.RunBlockingTaskCommand(async () => await SaveAndGenerateHtml(false));
            _saveAndCloseCommand = StatusContext.RunBlockingTaskCommand(async () => await SaveAndGenerateHtml(true));
        }

        public ContentIdViewerControlContext? ContentId
        {
            get => _contentId;
            set
            {
                if (Equals(value, _contentId)) return;
                _contentId = value;
                OnPropertyChanged();
            }
        }

        public CreatedAndUpdatedByAndOnDisplayContext? CreatedUpdatedDisplay
        {
            get => _createdUpdatedDisplay;
            set
            {
                if (Equals(value, _createdUpdatedDisplay)) return;
                _createdUpdatedDisplay = value;
                OnPropertyChanged();
            }
        }

        public List<MapElement> DbElements
        {
            get => _dbElements;
            set
            {
                if (Equals(value, _dbElements)) return;
                _dbElements = value;
                OnPropertyChanged();
            }
        }

        public MapComponent? DbEntry
        {
            get => _dbEntry;
            set
            {
                if (Equals(value, _dbEntry)) return;
                _dbEntry = value;
                OnPropertyChanged();
            }
        }

        public ObservableCollection<IMapElementListItem>? MapElements
        {
            get => _mapElements;
            set
            {
                if (Equals(value, _mapElements)) return;
                _mapElements = value;
                OnPropertyChanged();
            }
        }

        public Command SaveAndCloseCommand
        {
            get => _saveAndCloseCommand;
            set
            {
                if (Equals(value, _saveAndCloseCommand)) return;
                _saveAndCloseCommand = value;
                OnPropertyChanged();
            }
        }

        public Command SaveCommand
        {
            get => _saveCommand;
            set
            {
                if (Equals(value, _saveCommand)) return;
                _saveCommand = value;
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

        public StringDataEntryContext? SummaryEntry
        {
            get => _summaryEntry;
            set
            {
                if (Equals(value, _summaryEntry)) return;
                _summaryEntry = value;
                OnPropertyChanged();
            }
        }

        public StringDataEntryContext? TitleEntry
        {
            get => _titleEntry;
            set
            {
                if (Equals(value, _titleEntry)) return;
                _titleEntry = value;
                OnPropertyChanged();
            }
        }

        public UpdateNotesEditorContext? UpdateNotes
        {
            get => _updateNotes;
            set
            {
                if (Equals(value, _updateNotes)) return;
                _updateNotes = value;
                OnPropertyChanged();
            }
        }

        public Command UserGeoContentIdInputToMapCommand
        {
            get => _userGeoContentIdInputToMapCommand;
            set
            {
                if (Equals(value, _userGeoContentIdInputToMapCommand)) return;
                _userGeoContentIdInputToMapCommand = value;
                OnPropertyChanged();
            }
        }

        public string UserGeoContentInput
        {
            get => _userGeoContentInput;
            set
            {
                if (value == _userGeoContentInput) return;
                _userGeoContentInput = value;
                OnPropertyChanged();
            }
        }

        public void CheckForChangesAndValidationIssues()
        {
            HasChanges = PropertyScanners.ChildPropertiesHaveChanges(this);
            HasValidationIssues = PropertyScanners.ChildPropertiesHaveValidationIssues(this);
        }

        public void DragOver(IDropInfo dropInfo)
        {
            dropInfo.Effects = DragDropEffects.Copy;
        }

        public async void Drop(IDropInfo dropInfo)
        {
            var data = dropInfo.Data;

            if (data is DataObject dataObject)
            {
                var dataFormat = DataFormats.GetDataFormat(DataFormats.Serializable);
                data = dataObject.GetDataPresent(dataFormat.Name) ? dataObject.GetData(dataFormat.Name) : data;
            }

            var wrapper = data as SerializableContentCommonIdList;
            if (wrapper?.ContentIdList == null || !wrapper.ContentIdList.Any())
            {
                StatusContext.ToastWarning("Couldn't add - no items?");
                return;
            }

            foreach (var loopGuids in wrapper.ContentIdList) await TryAddSpatialType(loopGuids);
        }

        public bool HasChanges
        {
            get => _hasChanges;
            set
            {
                if (value == _hasChanges) return;
                _hasChanges = value;
                OnPropertyChanged();
            }
        }

        public bool HasValidationIssues
        {
            get => _hasValidationIssues;
            set
            {
                if (value == _hasValidationIssues) return;
                _hasValidationIssues = value;
                OnPropertyChanged();
            }
        }

        public event PropertyChangedEventHandler? PropertyChanged;

        private async Task TryAddSpatialType(Guid toAdd)
        {
            var db = await Db.Context();

            if (db.PointContents.Any(x => x.ContentId == toAdd))
            {
                await AddPoint(await Db.PointAndPointDetails(toAdd), guiNotificationWhenAdded: true);
                return;
            }

            if (db.GeoJsonContents.Any(x => x.ContentId == toAdd))
            {
                await AddGeoJson(await db.GeoJsonContents.SingleAsync(x => x.ContentId == toAdd),
                    guiNotificationWhenAdded: true);
                return;
            }

            if (db.LineContents.Any(x => x.ContentId == toAdd))
            {
                await AddLine(await db.LineContents.SingleAsync(x => x.ContentId == toAdd),
                    guiNotificationWhenAdded: true);
                return;
            }

            StatusContext.ToastError("Item isn't a spatial type or isn't in the db?");
        }

        private async Task AddGeoJson(GeoJsonContent possibleGeoJson, MapElement? loopContent = null,
            bool guiNotificationWhenAdded = false)
        {
            await ThreadSwitcher.ResumeBackgroundAsync();

            if (MapElements == null) return;

            if (MapElements.Any(x => x.ContentId() == possibleGeoJson.ContentId))
            {
                StatusContext.ToastWarning($"GeoJson {possibleGeoJson.Title} is already on the map");
                return;
            }

            await ThreadSwitcher.ResumeForegroundAsync();

            MapElements.Add(new MapElementListGeoJsonItem
            {
                DbEntry = possibleGeoJson,
                SmallImageUrl = GetSmallImageUrl(possibleGeoJson),
                InInitialView = loopContent?.IncludeInDefaultView ?? true,
                ShowInitialDetails = loopContent?.ShowDetailsDefault ?? false
            });

            if (guiNotificationWhenAdded) StatusContext.ToastSuccess($"Added Point - {possibleGeoJson.Title}");
        }

        private async Task AddLine(LineContent possibleLine, MapElement? loopContent = null,
            bool guiNotificationWhenAdded = false)
        {
            await ThreadSwitcher.ResumeBackgroundAsync();

            if (MapElements == null) return;

            if (MapElements.Any(x => x.ContentId() == possibleLine.ContentId))
            {
                StatusContext.ToastWarning($"Line {possibleLine.Title} is already on the map");
                return;
            }

            await ThreadSwitcher.ResumeForegroundAsync();

            MapElements.Add(new MapElementListLineItem
            {
                DbEntry = possibleLine,
                SmallImageUrl = GetSmallImageUrl(possibleLine),
                InInitialView = loopContent?.IncludeInDefaultView ?? true,
                ShowInitialDetails = loopContent?.ShowDetailsDefault ?? false
            });

            if (guiNotificationWhenAdded) StatusContext.ToastSuccess($"Added Point - {possibleLine.Title}");
        }

        private async Task AddPoint(PointContentDto possiblePoint, MapElement? loopContent = null,
            bool guiNotificationWhenAdded = false)
        {
            await ThreadSwitcher.ResumeBackgroundAsync();

            if (MapElements == null) return;

            if (MapElements.Any(x => x.ContentId() == possiblePoint.ContentId))
            {
                StatusContext.ToastWarning($"Point {possiblePoint.Title} is already on the map");
                return;
            }

            await ThreadSwitcher.ResumeForegroundAsync();

            MapElements.Add(new MapElementListPointItem
            {
                DbEntry = possiblePoint,
                SmallImageUrl = GetSmallImageUrl(possiblePoint),
                InInitialView = loopContent?.IncludeInDefaultView ?? true,
                ShowInitialDetails = loopContent?.ShowDetailsDefault ?? false
            });

            if (guiNotificationWhenAdded) StatusContext.ToastSuccess($"Added Point - {possiblePoint.Title}");
        }

        public static async Task<MapComponentEditorContext> CreateInstance(StatusControlContext statusContext,
            MapComponent mapComponent)
        {
            var newControl = new MapComponentEditorContext(statusContext);
            await newControl.LoadData(mapComponent);
            return newControl;
        }

        public MapComponentDto CurrentStateToContent()
        {
            var newEntry = new MapComponent();

            if (DbEntry == null || DbEntry.Id < 1)
            {
                newEntry.ContentId = Guid.NewGuid();
                newEntry.CreatedOn = DateTime.Now;
            }
            else
            {
                newEntry.ContentId = DbEntry.ContentId;
                newEntry.CreatedOn = DbEntry.CreatedOn;
                newEntry.LastUpdatedOn = DateTime.Now;
                newEntry.LastUpdatedBy =
                    CreatedUpdatedDisplay?.UpdatedByEntry.UserValue.TrimNullToEmpty() ?? string.Empty;
            }

            newEntry.Summary = SummaryEntry?.UserValue.TrimNullToEmpty() ?? string.Empty;
            newEntry.Title = TitleEntry?.UserValue.TrimNullToEmpty() ?? string.Empty;
            newEntry.CreatedBy = CreatedUpdatedDisplay?.CreatedByEntry.UserValue.TrimNullToEmpty() ?? string.Empty;
            newEntry.UpdateNotes = UpdateNotes?.UpdateNotes.TrimNullToEmpty();
            newEntry.UpdateNotesFormat = UpdateNotes?.UpdateNotesFormat.SelectedContentFormatAsString ?? string.Empty;

            var currentElementList = MapElements?.ToList() ?? new List<IMapElementListItem>();
            var finalElementList = currentElementList.Select(x => new MapElement
            {
                MapComponentContentId = newEntry.ContentId,
                ElementContentId = x.ContentId() ?? Guid.Empty,
                IsFeaturedElement = x.IsFeaturedElement,
                IncludeInDefaultView = x.InInitialView,
                ShowDetailsDefault = x.ShowInitialDetails
            }).ToList();

            return new MapComponentDto(newEntry, finalElementList);
        }

        public string GetSmallImageUrl(IMainImage content)
        {
            if (content.MainPicture == null) return string.Empty;

            string smallImageUrl;

            try
            {
                smallImageUrl =
                    PictureAssetProcessing.ProcessPictureDirectory(content.MainPicture.Value).SmallPicture?.File
                        .FullName ?? string.Empty;
            }
            catch
            {
                smallImageUrl = string.Empty;
            }

            return smallImageUrl;
        }

        public async Task LoadData(MapComponent toLoad)
        {
            await ThreadSwitcher.ResumeBackgroundAsync();

            DbEntry = toLoad;

            if (DbEntry.Id > 0)
            {
                var context = await Db.Context();
                DbElements = await context.MapComponentElements.Where(x => x.MapComponentContentId == DbEntry.ContentId)
                    .ToListAsync();
            }

            TitleEntry = StringDataEntryContext.CreateInstance();
            TitleEntry.Title = "Title";
            TitleEntry.HelpText = "Title Text";
            TitleEntry.ReferenceValue = DbEntry.Title.TrimNullToEmpty();
            TitleEntry.UserValue = DbEntry.Title.TrimNullToEmpty();
            TitleEntry.ValidationFunctions = new List<Func<string, (bool passed, string validationMessage)>>
            {
                CommonContentValidation.ValidateTitle
            };

            SummaryEntry = StringDataEntryContext.CreateInstance();
            SummaryEntry.Title = "Summary";
            SummaryEntry.HelpText = "A short text entry that will show in Search and short references to the content";
            SummaryEntry.ReferenceValue = DbEntry.Summary ?? string.Empty;
            SummaryEntry.UserValue = StringHelpers.NullToEmptyTrim(DbEntry.Summary);
            SummaryEntry.ValidationFunctions = new List<Func<string, (bool passed, string validationMessage)>>
            {
                CommonContentValidation.ValidateSummary
            };

            CreatedUpdatedDisplay = await CreatedAndUpdatedByAndOnDisplayContext.CreateInstance(StatusContext, DbEntry);
            ContentId = await ContentIdViewerControlContext.CreateInstance(StatusContext, DbEntry);
            UpdateNotes = await UpdateNotesEditorContext.CreateInstance(StatusContext, DbEntry);

            PropertyScanners.SubscribeToChildHasChangesAndHasValidationIssues(this, CheckForChangesAndValidationIssues);

            var db = await Db.Context();

            await ThreadSwitcher.ResumeForegroundAsync();

            MapElements ??= new ObservableCollection<IMapElementListItem>();

            MapElements.Clear();

            await ThreadSwitcher.ResumeBackgroundAsync();

            foreach (var loopContent in DbElements)
            {
                var elementContent = await db.ContentFromContentId(loopContent.ElementContentId);

                switch (elementContent)
                {
                    case GeoJsonContent gj:
                        await AddGeoJson(gj, loopContent);
                        break;
                    case LineContent l:
                        await AddLine(l, loopContent);
                        break;
                    case PointContentDto p:
                        await AddPoint(p, loopContent);
                        break;
                }
            }
        }

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));

            if (string.IsNullOrWhiteSpace(propertyName)) return;

            if (!propertyName.Contains("HasChanges") && !propertyName.Contains("Validation"))
                CheckForChangesAndValidationIssues();
        }

        public async Task SaveAndGenerateHtml(bool closeAfterSave)
        {
            await ThreadSwitcher.ResumeBackgroundAsync();

            var (generationReturn, newContent) =
                await MapComponentGenerator.SaveAndGenerateData(CurrentStateToContent(), null,
                    StatusContext.ProgressTracker());

            if (generationReturn.HasError || newContent == null)
            {
                await StatusContext.ShowMessageWithOkButton("Problem Saving and Generating Html",
                    generationReturn.GenerationNote);
                return;
            }

            await LoadData(newContent.Map);

            if (closeAfterSave)
            {
                await ThreadSwitcher.ResumeForegroundAsync();
                RequestContentEditorWindowClose?.Invoke(this, new EventArgs());
            }
        }

        public async Task UserGeoContentIdInputToMap()
        {
            await ThreadSwitcher.ResumeBackgroundAsync();

            if (string.IsNullOrWhiteSpace(UserGeoContentInput))
            {
                StatusContext.ToastError("Nothing to add?");
                return;
            }

            var codes = new List<Guid>();

            var regexObj = new Regex(@"(?:(\()|(\{))?\b[A-F0-9]{8}(?:-[A-F0-9]{4}){3}-[A-F0-9]{12}\b(?(1)\))(?(2)\})",
                RegexOptions.IgnoreCase | RegexOptions.IgnorePatternWhitespace | RegexOptions.Singleline);
            Match matchResult = regexObj.Match(UserGeoContentInput);
            while (matchResult.Success)
            {
                codes.Add(Guid.Parse(matchResult.Value));
                matchResult = matchResult.NextMatch();
            }

            codes.AddRange(BracketCodeCommon.BracketCodeContentIds(UserGeoContentInput));

            codes = codes.Distinct().ToList();

            if (!codes.Any())
            {
                StatusContext.ToastError("No Content Ids found?");
                return;
            }

            var db = await Db.Context();

            foreach (var loopCode in codes)
            {
                var possiblePoint = await db.PointContents.SingleOrDefaultAsync(x => x.ContentId == loopCode);
                if (possiblePoint != null)
                {
                    var pointDto = await Db.PointAndPointDetails(possiblePoint.ContentId, db);
                    await AddPoint(pointDto);
                    continue;
                }

                var possibleLine = await db.LineContents.SingleOrDefaultAsync(x => x.ContentId == loopCode);
                if (possibleLine != null)
                {
                    await AddLine(possibleLine);
                    continue;
                }

                var possibleGeoJson = await db.GeoJsonContents.SingleOrDefaultAsync(x => x.ContentId == loopCode);
                if (possibleGeoJson != null)
                {
                    await AddGeoJson(possibleGeoJson);
                    continue;
                }

                StatusContext.ToastWarning(
                    $"ContentId {loopCode} doesn't appear to be a valid point, line or GeoJson content for the map?");
            }

            UserGeoContentInput = string.Empty;
        }
    }
}