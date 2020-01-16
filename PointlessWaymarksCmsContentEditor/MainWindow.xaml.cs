﻿using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using GalaSoft.MvvmLight.CommandWpf;
using JetBrains.Annotations;
using PointlessWaymarksCmsData;
using PointlessWaymarksCmsData.IndexHtml;
using PointlessWaymarksCmsData.Models;
using PointlessWaymarksCmsWpfControls.ContentList;
using PointlessWaymarksCmsWpfControls.FileContentEditor;
using PointlessWaymarksCmsWpfControls.FileList;
using PointlessWaymarksCmsWpfControls.ImageContentEditor;
using PointlessWaymarksCmsWpfControls.ImageList;
using PointlessWaymarksCmsWpfControls.PhotoContentEditor;
using PointlessWaymarksCmsWpfControls.PhotoList;
using PointlessWaymarksCmsWpfControls.PostContentEditor;
using PointlessWaymarksCmsWpfControls.PostList;
using PointlessWaymarksCmsWpfControls.Status;
using PointlessWaymarksCmsWpfControls.Utility;

namespace PointlessWaymarksCmsContentEditor
{
    /// <summary>
    ///     Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : INotifyPropertyChanged
    {
        private ContentListContext _contextListContext;
        private RelayCommand _fileListWindowCommand;
        private RelayCommand _imageListWindowCommand;
        private RelayCommand _newFileContentCommand;
        private RelayCommand _newImageContentCommand;
        private StatusControlContext _statusContext;

        public MainWindow()
        {
            InitializeComponent();

            DataContext = this;

            StatusContext = new StatusControlContext();

            GenerateIndexCommand = new RelayCommand(() => StatusContext.RunNonBlockingTask(GenerateIndex));

            PhotoListWindowCommand = new RelayCommand(() => StatusContext.RunNonBlockingTask(NewPhotoList));
            NewPhotoContentCommand = new RelayCommand(() => StatusContext.RunNonBlockingTask(NewPhotoContent));

            PostListWindowCommand = new RelayCommand(() => StatusContext.RunNonBlockingTask(NewPostList));
            NewPostContentCommand = new RelayCommand(() => StatusContext.RunNonBlockingTask(NewPostContent));

            ImageListWindowCommand = new RelayCommand(() => StatusContext.RunNonBlockingTask(NewImageList));
            NewImageContentCommand = new RelayCommand(() => StatusContext.RunNonBlockingTask(NewImageContent));

            FileListWindowCommand = new RelayCommand(() => StatusContext.RunNonBlockingTask(NewFileList));
            NewFileContentCommand = new RelayCommand(() => StatusContext.RunNonBlockingTask(NewFileContent));

            StatusContext.RunFireAndForgetTaskWithUiToastErrorReturn(LoadData);
        }

        public ContentListContext ContextListContext
        {
            get => _contextListContext;
            set
            {
                if (Equals(value, _contextListContext)) return;
                _contextListContext = value;
                OnPropertyChanged();
            }
        }

        public RelayCommand FileListWindowCommand
        {
            get => _fileListWindowCommand;
            set
            {
                if (Equals(value, _fileListWindowCommand)) return;
                _fileListWindowCommand = value;
                OnPropertyChanged();
            }
        }

        public RelayCommand GenerateIndexCommand { get; set; }

        public RelayCommand ImageListWindowCommand
        {
            get => _imageListWindowCommand;
            set
            {
                if (Equals(value, _imageListWindowCommand)) return;
                _imageListWindowCommand = value;
                OnPropertyChanged();
            }
        }

        public RelayCommand NewFileContentCommand
        {
            get => _newFileContentCommand;
            set
            {
                if (Equals(value, _newFileContentCommand)) return;
                _newFileContentCommand = value;
                OnPropertyChanged();
            }
        }

        public RelayCommand NewImageContentCommand
        {
            get => _newImageContentCommand;
            set
            {
                if (Equals(value, _newImageContentCommand)) return;
                _newImageContentCommand = value;
                OnPropertyChanged();
            }
        }

        public RelayCommand NewPhotoContentCommand { get; set; }

        public RelayCommand NewPostContentCommand { get; set; }

        public RelayCommand PhotoListWindowCommand { get; set; }

        public RelayCommand PostListWindowCommand { get; set; }

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

        private async Task EditPhotoContent()
        {
            await ThreadSwitcher.ResumeBackgroundAsync();
            if (ContextListContext.SelectedItem == null)
            {
                StatusContext.ToastWarning("Nothing Selected?");
                return;
            }

            if (ContextListContext.SelectedItem.ContentType != "Photo")
            {
                StatusContext.ToastWarning("Photo not Selected.");
                return;
            }

            var photo = (PhotoContent) ContextListContext.SelectedItem.SummaryInfo;

            await ThreadSwitcher.ResumeForegroundAsync();

            var newContentWindow = new PhotoContentEditorWindow(photo) {Left = Left + 4, Top = Top + 4};
            newContentWindow.Show();
        }

        private async Task EditPostContent()
        {
            await ThreadSwitcher.ResumeBackgroundAsync();
            if (ContextListContext.SelectedItem == null)
            {
                StatusContext.ToastWarning("Nothing Selected?");
                return;
            }

            if (ContextListContext.SelectedItem.ContentType != "Post")
            {
                StatusContext.ToastWarning("Post not Selected.");
                return;
            }

            var post = (PostContent) ContextListContext.SelectedItem.SummaryInfo;

            await ThreadSwitcher.ResumeForegroundAsync();

            var newContentWindow = new PostContentEditorWindow(post) {Left = Left + 4, Top = Top + 4};
            newContentWindow.Show();
        }

        private async Task GenerateIndex()
        {
            await ThreadSwitcher.ResumeBackgroundAsync();

            var index = new IndexPage();
            index.WriteLocalHtml();

            StatusContext.ToastSuccess($"Generated {index.PageUrl}");
        }

        private async Task LoadData()
        {
            await ThreadSwitcher.ResumeBackgroundAsync();

            UserSettingsUtilities.VerifyAndCreate();

            var db = await Db.Context();
            //db.Database.EnsureDeleted();
            db.Database.EnsureCreated();

            ContextListContext = new ContentListContext(StatusContext);
        }

        private async Task NewFileContent()
        {
            await ThreadSwitcher.ResumeForegroundAsync();

            var newContentWindow = new FileContentEditorWindow(null) {Left = Left + 4, Top = Top + 4};
            newContentWindow.Show();
        }

        private async Task NewFileList()
        {
            await ThreadSwitcher.ResumeForegroundAsync();

            var newContentWindow = new FileListWindow {Left = Left + 4, Top = Top + 4};
            newContentWindow.Show();
        }

        private async Task NewImageContent()
        {
            await ThreadSwitcher.ResumeForegroundAsync();

            var newContentWindow = new ImageContentEditorWindow(null) {Left = Left + 4, Top = Top + 4};
            newContentWindow.Show();
        }

        private async Task NewImageList()
        {
            await ThreadSwitcher.ResumeForegroundAsync();

            var newContentWindow = new ImageListWindow {Left = Left + 4, Top = Top + 4};
            newContentWindow.Show();
        }

        private async Task NewPhotoContent()
        {
            await ThreadSwitcher.ResumeForegroundAsync();

            var newContentWindow = new PhotoContentEditorWindow(null) {Left = Left + 4, Top = Top + 4};
            newContentWindow.Show();
        }

        private async Task NewPhotoList()
        {
            await ThreadSwitcher.ResumeForegroundAsync();

            var newContentWindow = new PhotoListWindow {Left = Left + 4, Top = Top + 4};
            newContentWindow.Show();
        }

        private async Task NewPostContent()
        {
            await ThreadSwitcher.ResumeForegroundAsync();

            var newContentWindow = new PostContentEditorWindow(null) {Left = Left + 4, Top = Top + 4};
            newContentWindow.Show();
        }

        private async Task NewPostList()
        {
            await ThreadSwitcher.ResumeForegroundAsync();

            var newContentWindow = new PostListWindow {Left = Left + 4, Top = Top + 4};
            newContentWindow.Show();
        }

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public event PropertyChangedEventHandler PropertyChanged;
    }
}