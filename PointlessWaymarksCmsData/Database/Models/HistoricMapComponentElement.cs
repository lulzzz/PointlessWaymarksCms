﻿using System;

namespace PointlessWaymarksCmsData.Database.Models
{
    public class HistoricMapComponentElement
    {
        public Guid ContentId { get; set; }
        public int Id { get; set; }
        public bool InitialFocus { get; set; }
        public DateTime LastUpdateOn { get; set; }
        public Guid MapComponentContentId { get; set; }
    }
}