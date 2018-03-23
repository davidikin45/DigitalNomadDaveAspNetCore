﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace DND.Common.Implementation.Validation
{
    public class Result
    {
        public IEnumerable<ValidationResult> ObjectValidationErrors { get; }
        public bool IsSuccess { get; }
        public ErrorType? ErrorType { get; private set; }
        public bool IsFailure => !IsSuccess;

        protected Result(bool isSuccess, ErrorType? errorType, IEnumerable<ValidationResult> ObjectValidationErrors)
        {
            if (isSuccess && errorType != null)
                throw new InvalidOperationException();
            if (!isSuccess && errorType == null)
                throw new InvalidOperationException();

            IsSuccess = isSuccess;
            ErrorType = errorType;
            ObjectValidationErrors = ObjectValidationErrors;
        }

        public static Result<T> ObjectValidationFail<T>(string errorMessage)
        {
            var list = new List<ValidationResult>();
            list.Add(new ValidationResult(errorMessage));
            return ObjectValidationFail<T>(list);
        }

        public static Result<T> ObjectValidationFail<T>(string errorMessage, IEnumerable<string> memberNames)
        {
            var list = new List<ValidationResult>();
            list.Add(new ValidationResult(errorMessage, memberNames));
            return ObjectValidationFail<T>(list);
        }

        public static Result ObjectValidationFail(IEnumerable<ValidationResult> ObjectValidationErrors)
        {
            return new Result(false, Validation.ErrorType.ObjectValidationFailed, ObjectValidationErrors);
        }

        public static Result<T> ObjectValidationFail<T>(IEnumerable<ValidationResult> ObjectValidationErrors)
        {
            return new Result<T>(default(T), false, Validation.ErrorType.ObjectValidationFailed, ObjectValidationErrors);
        }

        public static Result Fail(ErrorType errorType)
        {
            return new Result(false, errorType, new List<ValidationResult>());
        }

        public static Result<T> Fail<T>(ErrorType errorType)
        {
            return new Result<T>(default(T), false, errorType, new List<ValidationResult>());
        }

        public static Result Ok()
        {
            return new Result(true, null, new List<ValidationResult>());
        }

        public static Result<T> Ok<T>(T value)
        {
            return new Result<T>(value, true, null, new List<ValidationResult>());
        }

        public static Result Combine(params Result[] results)
        {
            foreach (Result result in results)
            {
                if (result.IsFailure)
                    return result;
            }

            return Ok();
        }
    }


    public class Result<T> : Result
    {
        private readonly T _value;
        public T Value
        {
            get
            {
                if (!IsSuccess)
                    throw new InvalidOperationException();

                return _value;
            }
        }

        protected internal Result(T value, bool isSuccess, ErrorType? errorType, IEnumerable<ValidationResult> ObjectValidationErrors)
            : base(isSuccess, errorType, ObjectValidationErrors)
        {
            _value = value;
        }
    }

    public enum ErrorType
    {
        ObjectValidationFailed
    }
}
