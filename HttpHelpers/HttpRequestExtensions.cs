using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Newtonsoft.Json;

namespace PowerAppsFunction.HttpHelpers {
    public static class HttpRequestExtensions {

        /// <summary>
        /// validate request body and return validate result as HttpRequestBody
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="request"></param>
        /// <returns></returns>
        public static async Task<HttpRequestBody<T>> GetBodyAsync<T>(this HttpRequest request) {

            var body = new HttpRequestBody<T>();

            //content-typeチェック
            if (request.ContentType != "application/json") {
                body.IsValid = false;
                body.ValidationResults = new List<ValidationResult>(){
                    new ValidationResult("Content-typeはapplication/jsonで指定して下さい。")                
                };
                return body;
            }

            var bodyString = await request.ReadAsStringAsync();
            body.Value = JsonConvert.DeserializeObject<T>(bodyString);

            //validate 
            var results = new List<ValidationResult>();
            body.IsValid = Validator.TryValidateObject(body.Value, new ValidationContext(body.Value, null, null), results, true);
            body.ValidationResults = results;

            return body;
        }

    }
}
