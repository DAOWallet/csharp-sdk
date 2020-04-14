using System.Net;

namespace DAOWALLET_B2B_SDK
{
    /// <summary>
    /// Api Response
    /// </summary>
    public class ApiResponse
    {
        /// <summary>
        /// Default constructor
        /// </summary>
        public ApiResponse()
        {
        }

        /// <summary>
        /// Constructor
        /// </summary>
        public ApiResponse(HttpStatusCode statusCode, string result)
        {
            this.StatusCode = statusCode;
            this.Result = result;
        }

        /// <summary>
        /// Status code
        /// </summary>
        public HttpStatusCode StatusCode { get; private set; }

        /// <summary>
        /// Result
        /// </summary>
        public string Result { get; private set; }
    }
}
