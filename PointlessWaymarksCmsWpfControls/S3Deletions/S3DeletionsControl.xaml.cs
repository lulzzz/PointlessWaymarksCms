﻿using System.Collections.Generic;
using System.Linq;
using System.Windows.Controls;

namespace PointlessWaymarksCmsWpfControls.S3Deletions
{
    /// <summary>
    ///     Interaction logic for S3DeletionsControl.xaml
    /// </summary>
    public partial class S3DeletionsControl : UserControl
    {
        public S3DeletionsControl()
        {
            InitializeComponent();
        }

        private void Selector_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (DataContext == null) return;
            var viewmodel = (S3DeletionsContext) DataContext;
            viewmodel.SelectedItems = ItemsListBox?.SelectedItems.Cast<S3DeletionsItem>().ToList() ??
                                      new List<S3DeletionsItem>();
        }
    }
}