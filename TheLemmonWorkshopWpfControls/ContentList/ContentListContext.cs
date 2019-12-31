﻿using JetBrains.Annotations;
using Microsoft.EntityFrameworkCore;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using TheLemmonWorkshopData.Models;
using TheLemmonWorkshopWpfControls.ControlStatus;
using TheLemmonWorkshopWpfControls.Utility;

namespace TheLemmonWorkshopWpfControls.ContentList
{
    public class ContentListContext : INotifyPropertyChanged
    {
        private StatusControlContext _statusContext;

        public ContentListContext(StatusControlContext statusContext)
        {
            StatusContext = statusContext ?? new StatusControlContext();

            StatusContext.RunFireAndForgetTaskWithUiToastErrorReturn(LoadAllContent);
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public ObservableCollection<ContentListItem> Items { get; set; }

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

        public async Task LoadAllContent()
        {
            await ThreadSwitcher.ResumeBackgroundAsync();

            var db = await Db.Context();

            var rawList = (await db.LineContents.ToListAsync()).Select(x =>
                new ContentListItem {ContentType = "Line", SummaryInfo = (ITitleSummarySlugFolder) x}).ToList();
            rawList.AddRange((await db.PhotoContents.ToListAsync()).Select(x =>
                new ContentListItem {ContentType = "Photo", SummaryInfo = (ITitleSummarySlugFolder) x}).ToList());
            rawList.AddRange((await db.PointContents.ToListAsync()).Select(x =>
                new ContentListItem {ContentType = "Point", SummaryInfo = (ITitleSummarySlugFolder) x}).ToList());
            rawList.AddRange((await db.PostContents.ToListAsync()).Select(x =>
                new ContentListItem {ContentType = "Post", SummaryInfo = (ITitleSummarySlugFolder) x}).ToList());

            Items = new ObservableCollection<ContentListItem>(rawList);
        }

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}