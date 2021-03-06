﻿#nullable enable
using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using JetBrains.Annotations;
using PointlessWaymarksCmsData.Database.Models;

namespace PointlessWaymarksCmsWpfControls.MapComponentEditor
{
    public class MapElementListPointItem : INotifyPropertyChanged, IMapElementListItem
    {
        private PointContentDto? _dbEntry;
        private bool _inInitialView;
        private bool _isFeaturedElement;
        private bool _showInitialDetails;
        private string _smallImageUrl = string.Empty;

        public PointContentDto? DbEntry
        {
            get => _dbEntry;
            set
            {
                if (Equals(value, _dbEntry)) return;
                _dbEntry = value;
                OnPropertyChanged();
            }
        }

        public string SmallImageUrl
        {
            get => _smallImageUrl;
            set
            {
                if (value == _smallImageUrl) return;
                _smallImageUrl = value;
                OnPropertyChanged();
            }
        }

        public bool InInitialView
        {
            get => _inInitialView;
            set
            {
                if (value == _inInitialView) return;
                _inInitialView = value;
                OnPropertyChanged();
            }
        }

        public bool IsFeaturedElement
        {
            get => _isFeaturedElement;
            set
            {
                if (value == _isFeaturedElement) return;
                _isFeaturedElement = value;
                OnPropertyChanged();
            }
        }

        public bool ShowInitialDetails
        {
            get => _showInitialDetails;
            set
            {
                if (value == _showInitialDetails) return;
                _showInitialDetails = value;
                OnPropertyChanged();
            }
        }

        public Guid? ContentId()
        {
            return DbEntry?.ContentId;
        }

        public event PropertyChangedEventHandler? PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}