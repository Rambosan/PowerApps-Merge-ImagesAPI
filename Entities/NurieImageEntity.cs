using Newtonsoft.Json;
using PowerAppsFunction.ValueObject;
using System;
using System.ComponentModel.DataAnnotations;

namespace PowerAppsFunction.Entities {
    class NurieImageEntity {

        /// <summary>
        /// NurieImageを格納するEntity
        /// </summary>
        /// <param name="image_fg_datauri">前景画像のdatauriをテキストで指定します。</param>
        /// <param name="image_bg_datauri">背景画像のdatauriをテキストで指定します。</param>
        /// <param name="image_fg_crop">前景画像(ペン入力)のツールバーをクロップするかどうかを指定します。</param>
        /// <exception cref="ArgumentException"></exception>
        [JsonConstructor]
        public NurieImageEntity(string image_fg_datauri, string image_bg_datauri, string image_fg_crop) {

            try {
                this.image_fg_datauri = new ImageDataUri(image_fg_datauri);
            }
            catch (ArgumentNullException) {
                this.image_fg_datauri = null;
            }
            catch (Exception) {
                throw new ArgumentException($"パラメータ：{nameof(image_fg_datauri)}がdatauri形式ではありません。");
            }

            try {
                this.image_bg_datauri = new ImageDataUri(image_bg_datauri);
            }
            catch (ArgumentNullException) {
                this.image_bg_datauri = null;
            }
            catch (Exception) {
                throw new ArgumentException($"パラメータ：{nameof(image_bg_datauri)}がdatauri形式ではありません。");
            }
            
            this.image_fg_crop = image_fg_crop;
        }

        [Required]
        [ImageDataUriMaxLength(2000000)]
        public ImageDataUri image_fg_datauri { get; set; }


        [Required]
        [ImageDataUriMaxLength(2000000)]
        public ImageDataUri image_bg_datauri { get; set; }

        [RegularExpression("true|false")]
        public string image_fg_crop { get; set; }

    }
}
