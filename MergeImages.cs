using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json.Linq;
using PowerAppsFunction.Entities;
using PowerAppsFunction.HttpHelpers;
using PowerAppsFunction.ValueObject;

namespace PowerAppsFunction {
    public static class MergeImages
    {
        [FunctionName("MergeImages")]
       public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Function, "get", "post", Route = null)] HttpRequest req,
            ILogger log){

            log.LogInformation("C# HTTP trigger function processed a request.");

            //バリデーション
            HttpRequestBody<NurieImageEntity> requestBody;
            try {
                requestBody = await req.GetBodyAsync<NurieImageEntity>();
            }
            catch (Exception e) {
                return new BadRequestObjectResult(e.Message);
            }
            
            if (!requestBody.IsValid) {
                return new BadRequestObjectResult(requestBody.ValidationResults.First().ErrorMessage);
            }

            NurieImageEntity entity = requestBody.Value;

            //ビットマップへの変換
            Bitmap fgBitmap, bgBitmap;
            try {
                fgBitmap = entity.image_fg_datauri.ReadAsBitmap();
            }
            catch (Exception) {
                return new BadRequestObjectResult($"{nameof(entity.image_fg_datauri)}の変換に失敗しました。");
            }

            try {
                bgBitmap = entity.image_bg_datauri.ReadAsBitmap();
            }
            catch (Exception) {
                return new BadRequestObjectResult($"{nameof(entity.image_bg_datauri)}の変換に失敗しました。");
            }
            //ペン入力ツールバーのクロップ有無
            var cropFgToolbar = false;
            if (entity.image_fg_crop == "true") cropFgToolbar = true;


            //画像の合成
            var nurieBitmap = MergeTwoImages(fgBitmap, bgBitmap, cropFgToolbar);
            var dataUri = new ImageDataUri(ImageFormat.Png, nurieBitmap);

            object resultJson = new
            {
                data = dataUri.ToString()
            };

            log.LogInformation("MergeImages function ran successfully.");
            return new JsonResult(resultJson);

        }

        /// <summary>
        /// 背景画像に前景画像を描画してビットマップを返します。
        /// </summary>
        /// <param name="fg">前景画像</param>
        /// <param name="bg">背景画像</param>
        /// <param name="cropToolbar">ツールバーの60pxを切り抜くかどうか</param>
        /// <returns></returns>
        public static Bitmap MergeTwoImages(Bitmap fg, Bitmap bg, bool cropToolbar = false) {

            Bitmap bitmapBase = new Bitmap(bg.Width, bg.Height);

            //ビットマップに背景画像を描画
            using (var g = Graphics.FromImage(bitmapBase)) {
                //透過背景の場合に対応して一旦白背景に
                g.FillRectangle(Brushes.White, g.VisibleClipBounds);
                g.DrawImage(bg, g.VisibleClipBounds);
            }

            //ツールバーの60pxを切り抜き
            Bitmap fgBitmap;
            if (cropToolbar) {
                fgBitmap = fg.Clone(new Rectangle(0, 0, fg.Width, fg.Height - 60), fg.PixelFormat);
            }
            else {
                fgBitmap = fg;
            }

            //ビットマップに前景画像を描画
            using (var g = Graphics.FromImage(bitmapBase)) {
                g.DrawImage(fgBitmap, g.VisibleClipBounds);
            }

            return bitmapBase;
        }

    }


}

