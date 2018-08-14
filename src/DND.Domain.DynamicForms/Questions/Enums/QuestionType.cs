using System.ComponentModel.DataAnnotations;

namespace DND.Domain.DynamicForms.Questions.Enums
{
    public enum QuestionType
    {
        [Display(Name = "Text")]
        Text,
        [Display(Name = "TextArea")]
        TextArea,
        [Display(Name = "Number")]
        Number,
        [Display(Name = "Slider")]
        Slider,
        [Display(Name = "Currency")]
        Currency,
        [Display(Name = "Date")]
        Date,
        [Display(Name = "Date Time")]
        DateTime,
        [Display(Name = "Phone Number")]
        PhoneNumber,
        [Display(Name = "Email")]
        Email,
        [Display(Name = "Checkbox")]
        Checkbox,
        [Display(Name = "Yes Button")]
        YesButton,
        [Display(Name = "Dropdown - Lookup Table")]
        Dropdown,
        [Display(Name = "Dropdown Many - Lookup Table")]
        DropdownMany,
        [Display(Name = "Radio List - Yes & No")]
        RadioListYesNo,
        [Display(Name = "Radio List - True & False")]
        RadioListTrueFalse,
        [Display(Name = "Radio List - Lookup Table")]
        RadioList,
        [Display(Name = "Radio List Buttons - Lookup Table")]
        RadioListButtons,
        [Display(Name = "Radio List Buttons - Yes & No")]
        RadioListButtonsYesNo,
        [Display(Name = "Radio List Buttons - True & False")]
        RadioListButtonsTrueFalse,
        [Display(Name = "Checkbox List - Lookup Table")]
        CheckboxList,
        [Display(Name = "Checkbox List Buttons - Lookup Table")]
        CheckboxListButtons,
        [Display(Name = "File")]
        File,
        [Display(Name = "File - Image")]
        FileImage,
        [Display(Name = "File - Video")]
        FileVideo,
        [Display(Name = "File - Audio")]
        FileAudio,
        [Display(Name = "File - Image or Video")]
        FileImageVideo,
        [Display(Name = "Multiple Files")]
        MultipleFiles,
        [Display(Name = "Multiple Files - Image")]
        MultipleFilesImage,
        [Display(Name = "Multiple Files - Video")]
        MultipleFilesVideo,
        [Display(Name = "Multiple Files - Audio")]
        MultipleFilesAudio,
        [Display(Name = "Multiple Files - Image or Video")]
        MultipleFilesImageVideo,
    }
}