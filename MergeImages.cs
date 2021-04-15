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

            //�o���f�[�V����
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

            //�r�b�g�}�b�v�ւ̕ϊ�
            Bitmap fgBitmap, bgBitmap;
            try {
                fgBitmap = entity.image_fg_datauri.ReadAsBitmap();
            }
            catch (Exception) {
                return new BadRequestObjectResult($"{nameof(entity.image_fg_datauri)}�̕ϊ��Ɏ��s���܂����B");
            }

            try {
                bgBitmap = entity.image_bg_datauri.ReadAsBitmap();
            }
            catch (Exception) {
                return new BadRequestObjectResult($"{nameof(entity.image_bg_datauri)}�̕ϊ��Ɏ��s���܂����B");
            }
            //�y�����̓c�[���o�[�̃N���b�v�L��
            var cropFgToolbar = false;
            if (entity.image_fg_crop == "true") cropFgToolbar = true;


            //�摜�̍���
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
        /// �w�i�摜�ɑO�i�摜��`�悵�ăr�b�g�}�b�v��Ԃ��܂��B
        /// </summary>
        /// <param name="fg">�O�i�摜</param>
        /// <param name="bg">�w�i�摜</param>
        /// <param name="cropToolbar">�c�[���o�[��60px��؂蔲�����ǂ���</param>
        /// <returns></returns>
        public static Bitmap MergeTwoImages(Bitmap fg, Bitmap bg, bool cropToolbar = false) {

            Bitmap bitmapBase = new Bitmap(bg.Width, bg.Height);

            //�r�b�g�}�b�v�ɔw�i�摜��`��
            using (var g = Graphics.FromImage(bitmapBase)) {
                //���ߔw�i�̏ꍇ�ɑΉ����Ĉ�U���w�i��
                g.FillRectangle(Brushes.White, g.VisibleClipBounds);
                g.DrawImage(bg, g.VisibleClipBounds);
            }

            //�c�[���o�[��60px��؂蔲��
            Bitmap fgBitmap;
            if (cropToolbar) {
                fgBitmap = fg.Clone(new Rectangle(0, 0, fg.Width, fg.Height - 60), fg.PixelFormat);
            }
            else {
                fgBitmap = fg;
            }

            //�r�b�g�}�b�v�ɑO�i�摜��`��
            using (var g = Graphics.FromImage(bitmapBase)) {
                g.DrawImage(fgBitmap, g.VisibleClipBounds);
            }

            return bitmapBase;
        }

    }


}

