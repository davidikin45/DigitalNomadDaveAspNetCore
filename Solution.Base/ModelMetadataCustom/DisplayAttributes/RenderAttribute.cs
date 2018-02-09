﻿using System;
using Microsoft.AspNetCore.Mvc.ModelBinding.Metadata;
using Solution.Base.ModelMetadataCustom.Interfaces;

namespace Solution.Base.ModelMetadataCustom.DisplayAttributes
{
    public class RenderAttribute : Attribute, IMetadataAttribute
    {
        public bool ShowForGrid { get; set; }
        public bool AllowSortForGrid { get; set; }
        public bool ShowForEdit { get; set; }
        private bool _showForCreateSet;
        private bool _showForCreate;
        public bool ShowForCreate
        {
            get { return _showForCreate; }
            set
            {
                _showForCreateSet = true;
                _showForCreate = value;
            }
        }
        public bool ShowForDisplay { get; set; }

        public RenderAttribute()
        {
            ShowForGrid = true;
            AllowSortForGrid = true;
            ShowForEdit = true;
            _showForCreate = true;
            ShowForDisplay = true;
        }

        public void TransformMetadata(DisplayMetadataProviderContext context)
        {
            var propertyAttributes = context.Attributes;
            var modelMetadata = context.DisplayMetadata;
            var propertyName = context.Key.Name;

            modelMetadata.AdditionalValues["ShowForGrid"] = ShowForGrid;
            modelMetadata.AdditionalValues["AllowSortForGrid"] = AllowSortForGrid;
            modelMetadata.ShowForEdit = ShowForEdit;

            if (!_showForCreateSet)
            {
                _showForCreate = ShowForEdit;
            }

            modelMetadata.AdditionalValues["ShowForCreate"] = _showForCreate;
            modelMetadata.ShowForDisplay = ShowForDisplay;
        }
    }
}