namespace JobSearch.Infrastructure.Validations
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Security.Cryptography;
    using System.Text;
    using FluentValidation;
    using JobSearch.Infrastructure.Logging;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.AspNetCore.Mvc.Filters;
    using Newtonsoft.Json;

    public class ValidatorActionFilter : IActionFilter
    {
        private List<object> _sentObjects = new List<object>();

        public void OnActionExecuting(ActionExecutingContext context)
        {
            _sentObjects = GetSentObjects(context);

            if (!context.ModelState.IsValid)
            {
                var errors = context.ModelState.Values.SelectMany(x => x.Errors).ToList();
                var errorList = new List<ErrorMessage>();

                foreach (var error in errors)
                {
                    if (error.Exception == null)
                    {
                        errorList.Add(new ErrorMessage { Message = error.ErrorMessage });
                    }
                    else
                    {
                        errorList.Add(new ErrorMessage { Message = error.Exception.Message });
                    }
                }

                context.Result = new BadRequestObjectResult(new ErrorResponse { Errors = errorList });
                var errorsJson = JsonConvert.SerializeObject(errors);
                Logger.Instance.Error(
                    "Validation Error in Request {Errors} - Path {Path} - Sent Objects {SentObjects}",
                    errorsJson,
                    context.HttpContext.Request.Path.Value,
                    JsonConvert.SerializeObject(_sentObjects));
            }
        }

        public void OnActionExecuted(ActionExecutedContext context)
        {
            if (context.Exception != null)
            {
                var exception = context.Exception;

                if (exception is AggregateException ae)
                {
                    var overrideError = false;
                    var allValidationErrors = true;
                    var errors = new List<ErrorMessage>();

                    foreach (var ex in ae.InnerExceptions)
                    {
                        switch (ex)
                        {
                            case FileNotFoundException fe:
                                if (overrideError)
                                {
                                    break;
                                }

                                overrideError = true;

                                if (!string.IsNullOrWhiteSpace(fe.Message))
                                {
                                    context.Result = new NotFoundObjectResult(new ErrorResponse { Errors = new List<ErrorMessage> { new ErrorMessage { Message = fe.Message } } });
                                }
                                else
                                {
                                    context.Result = new NotFoundResult();
                                }

                                Logger.Instance.Error(
                                    "Resource not found - Path: {Path} - Sent Objects {SentObjects}",
                                    context.HttpContext.Request.Path.Value,
                                    JsonConvert.SerializeObject(_sentObjects));

                                break;
                            case UnauthorizedAccessException ue:
                                if (overrideError)
                                {
                                    break;
                                }

                                overrideError = true;

                                if (!string.IsNullOrWhiteSpace(ue.Message))
                                {
                                    context.Result = new NotFoundObjectResult(new ErrorResponse { Errors = new List<ErrorMessage> { new ErrorMessage { Message = ue.Message } } });
                                }
                                else
                                {
                                    context.Result = new NotFoundResult();
                                }

                                break;
                            case ValidationException ve:
                                if (overrideError)
                                {
                                    break;
                                }

                                if (!ve.Errors.Any())
                                {
                                    errors.Add(new ErrorMessage { Message = ve.Message });
                                }
                                else
                                {
                                    foreach (var error in ve.Errors)
                                    {
                                        errors.Add(new ErrorMessage { Message = error.ErrorMessage });
                                    }
                                }

                                context.Result = new BadRequestObjectResult(new ErrorResponse { Errors = errors });
                                break;
                            default:
                                if (overrideError)
                                {
                                    break;
                                }

                                allValidationErrors = false;
                                var exceptionId = MD5(exception.ToString());

                                Logger.Instance.Error(
                                    "Exception ID: {ExceptionId} - Path: {Path} - Sent Objects {SentObjects} - Exception: {Exception}",
                                    exceptionId,
                                    context.HttpContext.Request.Path.Value,
                                    JsonConvert.SerializeObject(_sentObjects),
                                    exception);
                                errors.Add(new ErrorMessage { Message = $"An internal error occurred with ID {exceptionId}" });
                                break;
                        }
                    }

                    if (allValidationErrors && _sentObjects.Count > 0)
                    {
                        context.Result = new BadRequestObjectResult(new ErrorResponse { Errors = errors });
                        var errorsJson = JsonConvert.SerializeObject(errors);
                        Logger.Instance.Error(
                            "Validation Error in Request {Errors} - Path {Path} - Sent Objects {SentObjects}",
                            errorsJson,
                            context.HttpContext.Request.Path.Value,
                            JsonConvert.SerializeObject(_sentObjects));
                    }
                    else
                    {
                        context.Result = new ObjectResult(new ErrorResponse { Errors = errors }) { StatusCode = 500 };
                    }
                }
                else
                {
                    switch (exception)
                    {
                        case FileNotFoundException fe when !string.IsNullOrWhiteSpace(fe.Message):
                            context.Result = new NotFoundObjectResult(new ErrorResponse { Errors = new List<ErrorMessage> { new ErrorMessage { Message = fe.Message } } });

                            Logger.Instance.Error(
                                "Resource not found - Path: {Path} - Sent Objects {SentObjects}",
                                context.HttpContext.Request.Path.Value,
                                JsonConvert.SerializeObject(_sentObjects));

                            break;
                        case FileNotFoundException _:
                            context.Result = new NotFoundResult();

                            Logger.Instance.Error(
                                "Resource not found - Path: {Path} - Sent Objects {SentObjects}",
                                context.HttpContext.Request.Path.Value,
                                JsonConvert.SerializeObject(_sentObjects));

                            break;
                        case ValidationException ve:
                            var errors = new List<ErrorMessage>();

                            if (!ve.Errors.Any())
                            {
                                errors.Add(new ErrorMessage { Message = ve.Message });
                            }
                            else
                            {
                                foreach (var error in ve.Errors)
                                {
                                    errors.Add(new ErrorMessage { Message = error.ErrorMessage });
                                }
                            }

                            context.Result = new BadRequestObjectResult(new ErrorResponse { Errors = errors });
                            break;
                        default:
                            var exceptionId = MD5(exception.ToString());
                            Logger.Instance.Error(
                                "Exception ID: {ExceptionId} -  Path: {Path} - Sent Objects {SentObjects} - Exception: {Exception}",
                                exceptionId,
                                context.HttpContext.Request.Path.Value,
                                JsonConvert.SerializeObject(_sentObjects),
                                exception);
                            context.Result = new ObjectResult(new ErrorResponse { Errors = new List<ErrorMessage> { new ErrorMessage { Message = $"An internal error occurred with ID {exceptionId}" } } }) { StatusCode = 500 };
                            break;
                    }
                }

                context.ExceptionHandled = true;
            }
        }

        private static List<object> GetSentObjects(ActionExecutingContext context)
        {
            var sentObjects = new List<object>();

            foreach (var value in context.ActionArguments.Values)
            {
                if (value == null)
                {
                    continue;
                }

                try
                {
                    JsonConvert.SerializeObject(value);
                    sentObjects.Add(value);
                }
                catch (Exception ex)
                {
                    // This is just to continue if the current value isn't serializable by the JsonConverter
                    Logger.Instance.Error("An exception happened while serializing sent objects: {Exception}", ex);
                }
            }

            return sentObjects;
        }

        private static string MD5(string text)
        {
            string result;

            using (var cryptoServiceProvider = new MD5CryptoServiceProvider())
            {
                result = GenerateHashString(cryptoServiceProvider, text);
            }

            return result;
        }

        private static string GenerateHashString(HashAlgorithm algorithm, string text)
        {
            // Compute hash from text parameter
            algorithm.ComputeHash(Encoding.UTF8.GetBytes(text));

            // Get has value in array of bytes
            var result = algorithm.Hash;

            // Return as hexadecimal string
            return string.Join(
                string.Empty,
                result.Select(x => x.ToString("x2")));
        }
    }
}
