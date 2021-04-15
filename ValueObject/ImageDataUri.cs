using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace PowerAppsFunction.ValueObject {
    class ImageDataUri {

        public ImageFormat ImageFormat { get; }
        public string Base64 { get; }

        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="dataUri"></param>
        /// <exception cref="FormatException"></exception>
        public ImageDataUri(string dataUri) {

            if (string.IsNullOrWhiteSpace(dataUri)) {
                throw new ArgumentNullException();
            }

            if (dataUri.Substring(0, 5) != "data:") {
                ThrowFormatException();
            }

            //image型uriのみ許容
            var match = Regex.Match(dataUri, @"data:image/(?<type>\w{3,4});base64,(?<data>.+)");
            if (!match.Success) {
                ThrowFormatException();
            }

            var imageFormat = GetImageFormat(match.Groups["type"].Value);
            if (imageFormat == null) {
                ThrowFormatException("Image format in the data uri isn't valid format.");
            }

            this.Base64 = match.Groups["data"].Value;
            this.ImageFormat = imageFormat;
        }

        public ImageDataUri(ImageFormat imageFormat,Bitmap bitmap) {

            string base64;
            using (var stream = new MemoryStream()) {
                bitmap.Save(stream, imageFormat);
                base64 = Convert.ToBase64String(stream.GetBuffer());
            }

            this.Base64 = base64;
            this.ImageFormat = imageFormat;
        }

        public ImageDataUri(ImageFormat imageFormat,string base64) {
            this.Base64 = base64;
            this.ImageFormat = imageFormat;
        }

        public override string ToString() {
            var format = this.ImageFormat.ToString().ToLower();
            return $"data:image/{format};base64,{this.Base64}";
        }

        /// <summary>
        /// base64データをビットマップとして返します。
        /// </summary>
        /// <returns></returns>
        public Bitmap ReadAsBitmap() {

            byte[] binData = Convert.FromBase64String(this.Base64);

            Bitmap bitmap;
            using (var stream = new MemoryStream(binData)) {
                bitmap = new Bitmap(stream);
            }

            return bitmap;
        }


        void ThrowFormatException(string message = "Argument is not a valid DataUri format.") {
            throw new FormatException(message);
        }

        /// <summary>
        /// 指定した拡張子に応じたImageFormatを返します。
        /// </summary>
        /// <param name="fileExt"></param>
        /// <returns></returns>
        private ImageFormat GetImageFormat(string fileExt) {

            ImageFormat result = null;

            var property = typeof(ImageFormat).GetProperties().Where(
                x => x.PropertyType == typeof(ImageFormat)
                && x.Name.Equals(fileExt, StringComparison.InvariantCultureIgnoreCase)).FirstOrDefault();

            if (property != null) {
                result = property.GetValue(property) as ImageFormat;
            }

            return result;
        }

    }
}
