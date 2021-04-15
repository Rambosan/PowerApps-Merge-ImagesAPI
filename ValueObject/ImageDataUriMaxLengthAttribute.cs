
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

            //指定バイト数を1.33倍してbase64の長さに変換
            double base64length = maxByteCount * (4 / 3);

            if (base64length > int.MaxValue) {
                MaxBase64Length = int.MaxValue;
            }else{
                MaxBase64Length = (int)(base64length);
            }

            base.ErrorMessage = "パラメータ：{0}が制限サイズを超えています。";
        }


        protected override ValidationResult IsValid(object value,ValidationContext validationContext) {

            ImageDataUri dataUri = value as ImageDataUri;
            if (dataUri == null) {
                return new ValidationResult($"バリデーションルールが正しく設定されていません。");
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
