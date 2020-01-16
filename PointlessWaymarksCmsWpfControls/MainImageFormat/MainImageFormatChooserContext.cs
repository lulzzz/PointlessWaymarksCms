﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using JetBrains.Annotations;
using PointlessWaymarksCmsData;

namespace PointlessWaymarksCmsWpfControls.MainImageFormat
{
    public class MainImageFormatChooserContext : INotifyPropertyChanged
    {
        private List<MainImageContentFormatEnum> _contentFormatChoices;

        private MainImageContentFormatEnum _selectedContentFormat;

        public MainImageFormatChooserContext()
        {
            ContentFormatChoices = Enum.GetValues(typeof(MainImageContentFormatEnum)).Cast<MainImageContentFormatEnum>()
                .ToList();
            SelectedContentFormat = ContentFormatChoices.First();
        }

        public List<MainImageContentFormatEnum> ContentFormatChoices
        {
            get => _contentFormatChoices;
            set
            {
                if (Equals(value, _contentFormatChoices)) return;
                _contentFormatChoices = value;
                OnPropertyChanged();
            }
        }

        public MainImageContentFormatEnum SelectedContentFormat
        {
            get => _selectedContentFormat;
            set
            {
                if (value == _selectedContentFormat) return;
                _selectedContentFormat = value;
                OnPropertyChanged();

                OnSelectedValueChanged?.Invoke(this,
                    Enum.GetName(typeof(MainImageContentFormatEnum), SelectedContentFormat));
            }
        }

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public event EventHandler<string> OnSelectedValueChanged;
    }
}