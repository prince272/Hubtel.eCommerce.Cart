using Hubtel.eCommerce.Cart.Core.Exceptions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using System.Collections.Generic;
using System.Collections;
using System.Net;
using System;
using System.Linq;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.WebUtilities;
using System.Diagnostics;

namespace Hubtel.eCommerce.Cart.Api.Controllers
{
    [Route("/[controller]")]
    [ApiController]
    public class ErrorsController : ControllerBase
    {
        [AllowAnonymous]
        [ApiExplorerSettings(IgnoreApi = true)]
        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        [Route("/[controller]/{statusCode}")]
        public IActionResult Index()
        {
            var httpContext = HttpContext;
            IDictionary<string, TValue> ApplyDictionaryKeyPolicy<TValue>(IDictionary<string, TValue> dictionary)
            {
                var serializerOptions = httpContext.RequestServices.GetService<IOptions<JsonOptions>>()?.Value?.JsonSerializerOptions;
                dictionary = dictionary.ToDictionary(pair => serializerOptions?.DictionaryKeyPolicy?.ConvertName(pair.Key) ?? pair.Key, pair => pair.Value);
                return dictionary;
            }

            int statusCode = Enum.TryParse<HttpStatusCode>(httpContext.GetRouteValue(nameof(statusCode))?.ToString(), out var status) ? (int)status : (int)HttpStatusCode.NotFound;
            var statusCodeFeature = httpContext.Features.Get<IStatusCodeReExecuteFeature>();
            var exceptionFeature = httpContext.Features.Get<IExceptionHandlerFeature>();

            StatusCodeException exception;
            string instance;

            if (exceptionFeature is null)
            {
                instance = statusCodeFeature?.OriginalPath ?? httpContext.Request.Path;

                exception = statusCode switch
                {
                    StatusCodes.Status400BadRequest => new BadRequestException(),
                    StatusCodes.Status404NotFound => new NotFoundException(),
                    _ => new StatusCodeException(statusCode)
                };
            }
            else
            {
                instance = null;

                if (exceptionFeature.Error is StatusCodeException statusCodeException)
                {
                    exception = statusCodeException;
                    statusCode = statusCodeException.StatusCode;
                }
                else
                {
                    exception = new StatusCodeException(statusCode, innerException: exceptionFeature?.Error);
                }
            }

            var title = exception.Title;
            var detail = exception.Message;
            var extensions = ApplyDictionaryKeyPolicy(exception.Data.Cast<DictionaryEntry>().ToDictionary(entry => entry.Key.ToString()!, entry => entry.Value));

            if (exception is BadRequestException)
            {
                var errors = ApplyDictionaryKeyPolicy(((BadRequestException)exception).Errors);
                return ValidationProblem(errors, detail: detail, instance: instance, statusCode: statusCode, title: title, type: null, extensions: extensions);
            }
            else
            {
                return Problem(detail: detail, instance: instance, statusCode: statusCode, title: title, type: null);
            }
        }

        /// <summary>
        /// Creates an Microsoft.AspNetCore.Mvc.BadRequestObjectResult that produces a Microsoft.AspNetCore.Http.StatusCodes.Status400BadRequest.
        /// </summary>
        /// <param name="errors">One or more validation errors.</param>
        /// <param name="detail">The value for <see cref="ProblemDetails.Detail" />.</param>
        /// <param name="instance">The value for <see cref="ProblemDetails.Instance" />.</param>
        /// <param name="statusCode">The status code.</param>
        /// <param name="title">The value for <see cref="ProblemDetails.Title" />. Defaults to "One or more validation errors occurred."</param>
        /// <param name="type">The value for <see cref="ProblemDetails.Type" />.</param>
        /// <param name="extensions">The value for <see cref="ProblemDetails.Extensions" />.</param>
        /// <returns>The created <see cref="IResult"/> for the response.</returns>
        [NonAction]
        public IActionResult ValidationProblem(
        IDictionary<string, string[]> errors,
        string detail = null,
        string instance = null,
        int? statusCode = null,
        string title = null,
        string type = null,
        IDictionary<string, object> extensions = null)
        {
            var problemDetailsFactory = (HttpProblemDetailsFactory)HttpContext.RequestServices.GetServices<ProblemDetailsFactory>().First(_ => _.GetType() == typeof(HttpProblemDetailsFactory));
            return ValidationProblem(problemDetailsFactory.CreateValidationProblemDetails(HttpContext, errors, statusCode, title, type, detail, instance, extensions));
        }
    }

    /// <summary>
    /// Based on Microsoft's DefaultProblemDeatilsFactory
    /// https://github.com/aspnet/AspNetCore/blob/2e4274cb67c049055e321c18cc9e64562da52dcf/src/Mvc/Mvc.Core/src/Infrastructure/DefaultProblemDetailsFactory.cs
    /// </summary>
    public sealed class HttpProblemDetailsFactory : ProblemDetailsFactory
    {
        private readonly ApiBehaviorOptions _options;

        public HttpProblemDetailsFactory(IOptions<ApiBehaviorOptions> options)
        {
            _options = options?.Value ?? throw new ArgumentNullException(nameof(options));
        }

        public override ProblemDetails CreateProblemDetails(
            HttpContext httpContext,
            int? statusCode = null,
            string title = null,
            string type = null,
            string detail = null,
            string instance = null)
        {
            statusCode ??= 500;

            var problemDetails = new ProblemDetails
            {
                Status = statusCode,
                Title = title,
                Type = type,
                Detail = detail,
                Instance = instance,
            };

            ApplyProblemDetailsDefaults(httpContext, problemDetails, statusCode.Value);

            return problemDetails;
        }

        public override ValidationProblemDetails CreateValidationProblemDetails(
            HttpContext httpContext,
            ModelStateDictionary modelStateDictionary,
            int? statusCode = null,
            string title = null,
            string type = null,
            string detail = null,
            string instance = null)
        {
            if (modelStateDictionary == null)
            {
                throw new ArgumentNullException(nameof(modelStateDictionary));
            }

            statusCode ??= 400;

            var problemDetails = new ValidationProblemDetails(modelStateDictionary)
            {
                Status = statusCode,
                Type = type,
                Detail = detail,
                Instance = instance,
            };

            if (title != null)
            {
                // For validation problem details, don't overwrite the default title with null.
                problemDetails.Title = title;
            }

            ApplyProblemDetailsDefaults(httpContext, problemDetails, statusCode.Value);

            return problemDetails;
        }

        public ValidationProblemDetails CreateValidationProblemDetails(
            HttpContext httpContext,
            IDictionary<string, string[]> errors,
            int? statusCode = null,
            string title = null,
            string type = null,
            string detail = null,
            string instance = null,
            IDictionary<string, object> extensions = null)
        {
            if (errors == null)
            {
                throw new ArgumentNullException(nameof(errors));
            }

            string ResolvePropertyNamingPolicy(string propertyName)
                => (httpContext.RequestServices.GetRequiredService<IOptions<JsonOptions>>()?.Value)?.JsonSerializerOptions.DictionaryKeyPolicy?.ConvertName(propertyName) ?? propertyName;

            errors = errors.ToDictionary(k => ResolvePropertyNamingPolicy(k.Key), k => k.Value);

            statusCode ??= 400;

            var problemDetails = new ValidationProblemDetails(errors)
            {
                Status = statusCode,
                Type = type,
                Detail = detail,
                Instance = instance,
            };

            if (title != null)
            {
                // For validation problem details, don't overwrite the default title with null.
                problemDetails.Title = title;
            }

            if (extensions != null)
            {
                foreach (var extension in extensions)
                {
                    problemDetails.Extensions.Add(ResolvePropertyNamingPolicy(extension.Key), extension.Value);
                }
            }

            ApplyProblemDetailsDefaults(httpContext, problemDetails, statusCode.Value);

            return problemDetails;
        }

        private void ApplyProblemDetailsDefaults(HttpContext httpContext, ProblemDetails problemDetails, int statusCode)
        {
            problemDetails.Status ??= statusCode;

            if (_options.ClientErrorMapping.TryGetValue(statusCode, out var clientErrorData))
            {
                problemDetails.Type ??= clientErrorData.Link;
            }

            var traceId = Activity.Current?.Id ?? httpContext?.TraceIdentifier;
            if (traceId != null)
            {
                problemDetails.Extensions["traceId"] = traceId;
            }

            problemDetails.Extensions["reason"] = ReasonPhrases.GetReasonPhrase(statusCode);
        }
    }
}