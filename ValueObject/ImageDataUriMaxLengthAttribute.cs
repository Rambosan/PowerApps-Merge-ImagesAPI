
using PowerAppsFunction.ValueObject;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Globalization;
using System.Text;

namespace PowerAppsFunction.ValueObject {
    public class ImageDataUriMaxLengthAttribute : ValidationAttribute {

        public int MaxBase64Length { get; private set; }

        public ImageDataUriMaxLengthAttribute(int maxByteCount) {

            if(maxByteCount * 1.33 > int.MaxValue) {
                MaxBase64Length = int.MaxValue;
            }else{
                MaxBase64Length = (int)(maxByteCount * 1.33);
            }

            base.ErrorMessage = "パラメータ：{0}が制限サイズを超えています。";
        }


        protected override ValidationResult IsValid(object value,ValidationContext validationContext) {

            ImageDataUri dataUri = value as ImageDataUri;
            if (dataUri == null) {
                return new ValidationResult("バリデーションルールが正しく設定されていません。");
            }

            //サイズ確認
            if(dataUri.Base64.Length >= MaxBase64Length) {
                return new ValidationResult(FormatErrorMessage(validationContext.DisplayName));
            }

            //
            return ValidationResult.Success;
        }

        public override string FormatErrorMessage(string name) {
            return String.Format(CultureInfo.CurrentCulture, base.ErrorMessage, name);
        }
    }
}
