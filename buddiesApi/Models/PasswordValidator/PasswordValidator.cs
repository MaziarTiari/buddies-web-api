using System;
using System.ComponentModel.DataAnnotations;
using System.IO;

namespace buddiesApi.Models {
    public class ValidPasswordAttribute : ValidationAttribute {
        protected override ValidationResult IsValid(
            object value,
            ValidationContext validationContext
        ) {
            string line;
            var model = (IWithValidPassword)validationContext.ObjectInstance;
            if (model.Password.Length < 8) {
                return new ValidationResult("Short");
            }
            var path = Path.Combine(Directory.GetCurrentDirectory(), "PasswordList_Top_1000.txt");
            StreamReader passwordList = new StreamReader(path);
            while ((line = passwordList.ReadLine()) != null) {
                if (model.Password == line) {
                    return new ValidationResult("Banned");
                }
            }
            return ValidationResult.Success;
        }
    }
}
