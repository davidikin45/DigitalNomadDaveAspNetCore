using Microsoft.AspNetCore.Mvc.ModelBinding.Metadata;
using DND.Common.Infrastructure;
using DND.Common.Interfaces.Models;
using DND.Common.ModelMetadataCustom.Interfaces;
using System;
using System.IO;
using System.Linq;
using DND.Common.Interfaces.Dtos;

namespace DND.Common.ModelMetadataCustom.DisplayAttributes
{
    public class FolderDropdownAttribute : DataboundAttribute
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

    public class FileDropdownAttribute : DataboundAttribute
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

    public class RepeaterAttribute : DataboundAttribute
    {
        public RepeaterAttribute(string valueProperty)
            :base(valueProperty)
        {

        }
    }

     public class DropdownAttribute : DataboundAttribute
    {
        public DropdownAttribute(Type modelType, string valueProperty)
            : base(modelType, nameof(IBaseEntity.Id), valueProperty, nameof(IBaseEntity.Id), DND.Common.ModelMetadataCustom.DisplayAttributes.OrderByType.Descending, false, null)
        {

        }

        public DropdownAttribute(Type modelType, string valueProperty, string orderByProperty, string orderByType)
           : base(modelType, nameof(IBaseEntity.Id), valueProperty, orderByProperty, orderByType, false, null)
        {

        }

        public DropdownAttribute(Type modelType, string valueProperty, string orderByProperty, string orderByType, string bindingProperty)
          : base(modelType, nameof(IBaseEntity.Id), valueProperty, orderByProperty, orderByType, false, bindingProperty)
        {

        }
    }

    public abstract class DataboundAttribute : Attribute, IMetadataAttribute
    {
        public Type ModelType { get; set; }
        public string KeyProperty { get; set; }
        public string BindingProperty { get; set; }
        public string ValueProperty { get; set; }
        public string OrderByProperty { get; set; }
        public string OrderByType { get; set; }

        public string FolderFolderId { get; set; }
        public string PhysicalFolderPath { get; set; }

        public string FileFolderId { get; set; }
        public string PhysicalFilePath { get; set; }

        public Boolean Nullable { get; set; }

        public string DataTypeName { get; set; }

        public DataboundAttribute(string folderFolderId, string fileFolderId, string valueProperty, string orderByProperty, string orderByType, Boolean nullable = false)
        {
            DataTypeName = "ModelDropdown";

            FolderFolderId = folderFolderId;
            FileFolderId = fileFolderId;

            ValueProperty = valueProperty;
            OrderByProperty = orderByProperty;
            OrderByType = orderByType;

            Nullable = nullable;
        }

        //typeof
        //nameof
        public DataboundAttribute(Type modelType, string keyProperty, string valueProperty, string orderByProperty, string orderByType, Boolean nullable = false, string bindingProperty = null)
        {
            if (!modelType.GetInterfaces().Contains(typeof(IBaseEntity)))
            {
                throw new ApplicationException("modelType must implement IBaseEntity");
            }

            DataTypeName = "ModelDropdown";

            ModelType = modelType;
            KeyProperty = keyProperty;
            ValueProperty = valueProperty;

            BindingProperty = bindingProperty;

            OrderByProperty = orderByProperty;
            OrderByType = orderByType;
            Nullable = nullable;
        }

        public DataboundAttribute(string valueProperty)
        {
            DataTypeName = "ModelRepeater";

            ValueProperty = valueProperty;

            KeyProperty = nameof(IBaseEntity.Id);
            BindingProperty = nameof(IBaseDtoWithId.Id);
        }

        public void TransformMetadata(DisplayMetadataProviderContext context)
        {
            var propertyAttributes = context.Attributes;
            var modelMetadata = context.DisplayMetadata;
            var propertyName = context.Key.Name;

            modelMetadata.DataTypeName = DataTypeName;

            modelMetadata.AdditionalValues["ModelType"] = ModelType;
            modelMetadata.AdditionalValues["KeyProperty"] = KeyProperty;
            modelMetadata.AdditionalValues["ValueProperty"] = ValueProperty;

            modelMetadata.AdditionalValues["BindingProperty"] = BindingProperty;

            modelMetadata.AdditionalValues["OrderByProperty"] = OrderByProperty;
            modelMetadata.AdditionalValues["OrderByType"] = OrderByType;

            if (!string.IsNullOrEmpty(FolderFolderId))
            {
                PhysicalFolderPath = Server.GetWwwFolderPhysicalPathById(FolderFolderId);
            }

            modelMetadata.AdditionalValues["PhysicalFolderPath"] = PhysicalFolderPath;

            if (!string.IsNullOrEmpty(FileFolderId))
            {
                PhysicalFilePath = Server.GetWwwFolderPhysicalPathById(FileFolderId);
            }

            modelMetadata.AdditionalValues["PhysicalFilePath"] = PhysicalFilePath;
            modelMetadata.AdditionalValues["Nullable"] = Nullable;
        }
    }

    public static class OrderByType
    {
        public const string Descending = "desc";
        public const string Ascending = "asc";
    }

}