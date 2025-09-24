using System;

namespace AdvantShop.Core.Primitives
{
    public class Error
    {
        public string Message { get; }

        public Error(string message)
        {
            Message = message;
        }

        public static Error None => new Error("");
    }

    public class Result
    {
        internal Result(bool isSuccess, Error error)
        {
            IsSuccess = isSuccess;
            Error = error;
        }

        public bool IsSuccess { get; }

        public Error Error { get; }

        public static Result Success() => new Result(true, Error.None);

        public static Result<TValue> Success<TValue>(TValue value) => new Result<TValue>(value, true, Error.None);

        public static Result Failure(Error error) => new Result(false, error);

        public static Result<TValue> Failure<TValue>(Error error) => new Result<TValue>(default, false, error);

        public static Result<TValue> Create<TValue>(TValue value, Error error) where TValue : class
            => value is null
                ? Failure<TValue>(error)
                : Success(value);
    }

    public class Result<TValue> : Result
    {
        private readonly TValue _value;

        protected internal Result(TValue value, bool isSuccess, Error error) : base(isSuccess, error)
        {
            _value = value;
        }
        
        public TValue Value => IsSuccess
            ? _value
            : throw new InvalidOperationException("The value of failure result can not be accessed.");

        public static implicit operator Result<TValue>(TValue value) => Success(value);
    }
    
    // public class Result
    // {
    //     internal Result(bool isSuccess, IEnumerable<string> errors)
    //     {
    //         IsSuccess = isSuccess;
    //         Errors = errors.ToArray();
    //     }
    //
    //     public bool IsSuccess { get; set; }
    //
    //     public string[] Errors { get; set; }
    //
    //     public static Result Success()
    //     {
    //         return new Result(true, Array.Empty<string>());
    //     }
    //
    //     public static Result Failure(IEnumerable<string> errors)
    //     {
    //         return new Result(false, errors);
    //     }
    // }
}