using Microsoft.AspNetCore.Mvc.ModelBinding.Metadata;
using DND.Common.Infrastructure;
using DND.Common.Interfaces.Models;
using DND.Common.ModelMetadataCustom.Interfaces;
using System;
using System.IO;
using System.Linq;

namespace DND.Common.ModelMetadataCustom.DisplayAttributes
{
    public class FolderDropdownAttribute : DropdownAttribute
    {
        public FolderDropdownAttribute(string folderId, Boolean nullable = false)
            : this(folderId, nameof(DirectoryInfo.FullName), nameof(DirectoryInfo.LastWriteTime), DND.Common.ModelMetadataCustom.DisplayAttributes.OrderByType.Descending, nullable)
        {

        }

        public FolderDropdownAttribute(string folderId, string valueProperty, string orderByProperty, string orderByType, Boolean nullable = false)
            : base(folderId, "", valueProperty, orderByProperty, orderByType, nullable)
        {
        }
    }

    public class FileDropdownAttribute : DropdownAttribute
    {
        public FileDropdownAttribute(string folderId, Boolean nullable = false)
            : this(folderId, nameof(DirectoryInfo.Name), nameof(DirectoryInfo.LastWriteTime),DND.Common.ModelMetadataCustom.DisplayAttributes.OrderByType.Descending, nullable)
        {

        }

        public FileDropdownAttribute(string folderId, string valueProperty, string orderByProperty, string orderByType, Boolean nullable = false)
            : base("", folderId, valueProperty, orderByProperty, orderByType, nullable)
        {
        }
    }

    public class DropdownAttribute : Attribute, IMetadataAttribute
    {
        public Type ModelType { get; set; }
        public string KeyProperty { get; set; }
        public string ValueProperty { get; set; }
        public string OrderByProperty { get; set; }
        public string OrderByType { get; set; }

        public string FolderFolderId { get; set; }
        public string PhysicalFolderPath { get; set; }

        public string FileFolderId { get; set; }
        public string PhysicalFilePath { get; set; }

        public Boolean Nullable { get; set; }

        public DropdownAttribute(string folderFolderId, string fileFolderId, string valueProperty, string orderByProperty, string orderByType, Boolean nullable = false)
        {
            FolderFolderId = folderFolderId;
            FileFolderId = fileFolderId;

            ValueProperty = valueProperty;
            OrderByProperty = orderByProperty;
            OrderByType = orderByType;

            Nullable = nullable;
        }

        //typeof
        //nameof
        public DropdownAttribute(Type modelType, string valueProperty)
            :this(modelType, "Id", valueProperty, "Id", DND.Common.ModelMetadataCustom.DisplayAttributes.OrderByType.Descending)
        {

        }

        public DropdownAttribute(Type modelType, string valueProperty, string orderByProperty, string orderByType)
           : this(modelType, "Id", valueProperty, orderByProperty, orderByType)
        {

        }

        //typeof
        //nameof
        public DropdownAttribute(Type modelType, string keyProperty, string valueProperty, string orderByProperty, string orderByType, Boolean nullable = false)
        {
            if (!modelType.GetInterfaces().Contains(typeof(IBaseEntity)))
            {
                throw new ApplicationException("modelType must implement IBaseEntity");
            }

            ModelType = modelType;
            KeyProperty = keyProperty;
            ValueProperty = valueProperty;

            OrderByProperty = orderByProperty;
            OrderByType = orderByType;
            Nullable = nullable;
        }

        public void TransformMetadata(DisplayMetadataProviderContext context)
        {
            var propertyAttributes = context.Attributes;
            var modelMetadata = context.DisplayMetadata;
            var propertyName = context.Key.Name;

            modelMetadata.DataTypeName = "ModelDropdown";

            modelMetadata.AdditionalValues["DropdownModelType"] = ModelType;
            modelMetadata.AdditionalValues["DropdownKeyProperty"] = KeyProperty;
            modelMetadata.AdditionalValues["DropdownValueProperty"] = ValueProperty;

            modelMetadata.AdditionalValues["DropdownOrderByProperty"] = OrderByProperty;
            modelMetadata.AdditionalValues["DropdownOrderByType"] = OrderByType;

            if (!string.IsNullOrEmpty(FolderFolderId))
            {
                PhysicalFolderPath = Server.GetWwwFolderPhysicalPathById(FolderFolderId);
            }

            modelMetadata.AdditionalValues["DropdownPhysicalFolderPath"] = PhysicalFolderPath;

            if (!string.IsNullOrEmpty(FileFolderId))
            {
                PhysicalFilePath = Server.GetWwwFolderPhysicalPathById(FileFolderId);
            }

            modelMetadata.AdditionalValues["DropdownPhysicalFilePath"] = PhysicalFilePath;
            modelMetadata.AdditionalValues["DropdownNullable"] = Nullable;
        }
    }

    public static class OrderByType
    {
        public const string Descending = "desc";
        public const string Ascending = "asc";
    }

}