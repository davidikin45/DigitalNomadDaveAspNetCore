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
    //Composition Properties (1-To-Many, child cannot exist independent of the parent) 
    public class RepeaterAttribute : DataboundAttribute
    {
        public RepeaterAttribute(string displayExpression)
            : base(displayExpression)
        {

        }
    }

    //Aggregation relationshiships(child can exist independently of the parent, reference relationship)
    public class FolderDropdownAttribute : DataboundAttribute
    {
        public FolderDropdownAttribute(string folderId, Boolean nullable = false)
            : this(folderId, nameof(DirectoryInfo.FullName), nameof(DirectoryInfo.LastWriteTime), DND.Common.ModelMetadataCustom.DisplayAttributes.OrderByType.Descending, nullable)
        {

        }

        public FolderDropdownAttribute(string folderId, string displayExpression, string orderByProperty, string orderByType, Boolean nullable = false)
            : base(folderId, "", displayExpression, orderByProperty, orderByType, nullable)
        {
        }
    }

    //Aggregation relationshiships(child can exist independently of the parent, reference relationship)
    public class FileDropdownAttribute : DataboundAttribute
    {
        public FileDropdownAttribute(string folderId, Boolean nullable = false)
            : this(folderId, nameof(DirectoryInfo.Name), nameof(DirectoryInfo.LastWriteTime), DND.Common.ModelMetadataCustom.DisplayAttributes.OrderByType.Descending, nullable)
        {

        }

        public FileDropdownAttribute(string folderId, string displayExpression, string orderByProperty, string orderByType, Boolean nullable = false)
            : base("", folderId, displayExpression, orderByProperty, orderByType, nullable)
        {
        }
    }

    //Aggregation relationshiships(child can exist independently of the parent, reference relationship)
    public class DropdownAttribute : DataboundAttribute
    {

        public DropdownAttribute(Type dropdownModelType, string displayExpression)
            : base(dropdownModelType, nameof(IBaseEntity.Id), displayExpression, nameof(IBaseEntity.Id), DND.Common.ModelMetadataCustom.DisplayAttributes.OrderByType.Descending, false, null)
        {

        }

        public DropdownAttribute(Type dropdownModelType, string displayExpression, string orderByProperty, string orderByType)
           : base(dropdownModelType, nameof(IBaseEntity.Id), displayExpression, orderByProperty, orderByType, false, null)
        {

        }

        public DropdownAttribute(Type dropdownModelType, string displayExpression, string orderByProperty, string orderByType, string bindingProperty)
          : base(dropdownModelType, nameof(IBaseEntity.Id), displayExpression, orderByProperty, orderByType, false, bindingProperty)
        {

        }
    }

    public abstract class DataboundAttribute : Attribute, IMetadataAttribute
    {
        public Type DropdownModelType { get; set; }
        public string KeyProperty { get; set; }
        public string BindingProperty { get; set; }
        public string DisplayExpression { get; set; }
        public string OrderByProperty { get; set; }
        public string OrderByType { get; set; }

        public string FolderFolderId { get; set; }
        public string PhysicalFolderPath { get; set; }

        public string FileFolderId { get; set; }
        public string PhysicalFilePath { get; set; }

        public Boolean Nullable { get; set; }

        public string DataTypeName { get; set; }

        public DataboundAttribute(string folderFolderId, string fileFolderId, string displayExpression, string orderByProperty, string orderByType, Boolean nullable = false)
        {
            DataTypeName = "ModelDropdown";

            FolderFolderId = folderFolderId;
            FileFolderId = fileFolderId;

            DisplayExpression = displayExpression;
            OrderByProperty = orderByProperty;
            OrderByType = orderByType;

            Nullable = nullable;
        }

        //typeof
        //nameof
        public DataboundAttribute(Type dropdownModelType, string keyProperty, string displayExpression, string orderByProperty, string orderByType, Boolean nullable = false, string bindingProperty = null)
        {
            if (!dropdownModelType.GetInterfaces().Contains(typeof(IBaseEntity)))
            {
                throw new ApplicationException("modelType must implement IBaseEntity");
            }

            DataTypeName = "ModelDropdown";

            DropdownModelType = dropdownModelType;
            KeyProperty = keyProperty;
            DisplayExpression = displayExpression;

            BindingProperty = bindingProperty;

            OrderByProperty = orderByProperty;
            OrderByType = orderByType;
            Nullable = nullable;
        }

        public DataboundAttribute(string displayExpression)
        {
            DataTypeName = "ModelRepeater";

            DisplayExpression = displayExpression;

            KeyProperty = nameof(IBaseEntity.Id);
            BindingProperty = nameof(IBaseDtoWithId.Id);
        }

        public void TransformMetadata(DisplayMetadataProviderContext context)
        {
            var propertyAttributes = context.Attributes;
            var modelMetadata = context.DisplayMetadata;
            var propertyName = context.Key.Name;

            modelMetadata.DataTypeName = DataTypeName;

            modelMetadata.AdditionalValues["IsDatabound"] = true;

            //Select from Db
            modelMetadata.AdditionalValues["DropdownModelType"] = DropdownModelType;
            modelMetadata.AdditionalValues["OrderByProperty"] = OrderByProperty;
            modelMetadata.AdditionalValues["OrderByType"] = OrderByType;

            modelMetadata.AdditionalValues["KeyProperty"] = KeyProperty; //Used for dropdown 
            modelMetadata.AdditionalValues["DisplayExpression"] = DisplayExpression; //Used for Dropdown and Display Text

            modelMetadata.AdditionalValues["BindingProperty"] = BindingProperty;

            modelMetadata.AdditionalValues["Nullable"] = Nullable;

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
        }
    }

    public static class OrderByType
    {
        public const string Descending = "desc";
        public const string Ascending = "asc";
    }

}