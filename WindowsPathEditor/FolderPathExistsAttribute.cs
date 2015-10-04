using System;
using System.ComponentModel.DataAnnotations;
using System.IO;

namespace WindowsPathEditor
{
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field, AllowMultiple = false)]
    public class FolderPathExistsAttribute : ValidationAttribute
    {
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            if (value == null)
                return InvalidPathResult();

            if (!Directory.Exists(value.ToString()))
                return InvalidPathResult();

            return null;
        }

        private ValidationResult InvalidPathResult()
        {
            return new ValidationResult("Invalid path");
        }
    }
}