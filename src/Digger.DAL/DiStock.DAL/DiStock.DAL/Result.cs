using System;
using System.Collections.Generic;
using System.Net;
using System.Text;

namespace DiStock.DAL
{
    public class Result<TSuccess> : Result
    {
        public Result(HttpStatusCode status, TSuccess success, string errorMessage)
            : base(status, errorMessage)
        {
            if (!HasError) Content = success;
        }

        public TSuccess Content { get; }
    }

    public class Result
    {
        public Result(HttpStatusCode status)
            : this(status, string.Empty)
        {
        }

        public Result(HttpStatusCode status, string errorMessage)
        {
            Status = status;
            if (errorMessage == null) errorMessage = string.Empty;
            ErrorMessage = errorMessage;
        }

        public HttpStatusCode Status { get; }

        public bool HasError => ErrorMessage != string.Empty;

        public string ErrorMessage { get; }

        public static Result<T> Success<T>(T content) => Success(HttpStatusCode.OK, content);

        public static Result<T> Success<T>(HttpStatusCode status, T content) => new Result<T>(status, content, null);

        public static Result<T> Failure<T>(HttpStatusCode status, string errorMessage)
        {
            if (string.IsNullOrEmpty(errorMessage)) throw new ArgumentException("The error message must be not null nor whitespace.", nameof(errorMessage));
            return new Result<T>(status, default(T), errorMessage);
        }

        public static Result Success() => new Result(HttpStatusCode.OK);

        public static Result Success(HttpStatusCode status) => new Result(status);

        public static Result Failure(HttpStatusCode status, string errorMessage) => new Result(status, errorMessage);
    }
}
