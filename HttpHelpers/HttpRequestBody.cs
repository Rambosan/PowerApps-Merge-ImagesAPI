using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace PowerAppsFunction.HttpHelpers {

    /// <summary>
    /// The container of validation result.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class HttpRequestBody<T> {
        public bool IsValid { get; set; } = false;
        public T Value { get; set; }

        public IEnumerable<ValidationResult> ValidationResults { get; set; }
    }
}
