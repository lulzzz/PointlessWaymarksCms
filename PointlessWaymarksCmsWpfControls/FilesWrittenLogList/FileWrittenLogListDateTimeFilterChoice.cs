﻿using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using JetBrains.Annotations;

namespace PointlessWaymarksCmsWpfControls.FilesWrittenLogList
{
    public class FileWrittenLogListDateTimeFilterChoice : INotifyPropertyChanged
    {
        private DateTime? _filterDateTimeUtc;
        private string _displayText = string.Empty;

        public DateTime? FilterDateTimeUtc
        {
            get => _filterDateTimeUtc;
            set
            {
                if (Nullable.Equals(value, _filterDateTimeUtc)) return;
                _filterDateTimeUtc = value;
                OnPropertyChanged();
            }
        }

        public string DisplayText
        {
            get => _displayText;
            set
            {
                if (value == _displayText) return;
                _displayText = value;
                OnPropertyChanged();
            }
        }

        public event PropertyChangedEventHandler? PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}