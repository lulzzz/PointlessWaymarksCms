﻿using System;
using System.Collections.Generic;
using System.Text;

namespace PointlessWaymarksCmsData.Database.PointDetailModels
{
    public class TrailJunction
    {
        public const string DataTypeIdentifier = "TrailJunction";
        public bool Sign { get; set; }
        public string NotesContentFormat { get; set; }
        public string Notes { get; set; }
    }
}
