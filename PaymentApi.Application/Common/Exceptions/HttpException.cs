using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace PaymentApi.Application.Common.Exceptions
{
    public class HttpException : ApplicationException
    {
        public const string DefaultErrorProperty = "raw";
        private HttpException(HttpStatusCode statusCode, string message, string property)
        {
            Property = property;
            StatusCode = statusCode;
            Errors = new List<string> { message };
        }
        public static void ThrowIf(bool condition, HttpStatusCode statusCode, string message, string property = DefaultErrorProperty)
        {
            if (condition)
            {
                Throw(statusCode, message, property);
            }
        }

        public static void Throw(HttpStatusCode statusCode, string message, string property = DefaultErrorProperty)
        {
            throw Create(statusCode, message, property);
        }
        public static HttpException Create(HttpStatusCode statusCode, string message, string property)
        {
            return new HttpException(statusCode, message, property);
        }
        public string Property { get; }
        public HttpStatusCode StatusCode { get; }
        public List<string> Errors { get; }
    }
}
