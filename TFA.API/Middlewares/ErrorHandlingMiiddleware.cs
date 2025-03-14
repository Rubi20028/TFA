using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using TFA.Domain.Authorization;
using TFA.Domain.Exceptions;

namespace TFA.API.Middlewares;

public class ErrorHandlingMiiddleware
{
    private readonly RequestDelegate next;

    public ErrorHandlingMiiddleware(RequestDelegate next)
    { 
        this.next = next;
    }
    
    public async Task InvokeAsync(
        HttpContext httpcontext,
        ILogger<ErrorHandlingMiiddleware> logger,
        ProblemDetailsFactory problemDetailsFactory)
    {
        try
        {
            logger.LogError("Error handling started for request in path {RequestPath}", httpcontext.Request.Path.Value );
            await next.Invoke(httpcontext);
        }
        
        catch (Exception exception)
        {
            logger.LogError(
                exception, 
                "Error has happened with {RequestPath}, the message is {ErrorMessage}",
                httpcontext.Request.Path.Value, exception.Message);

            ProblemDetails problemDetails;
            switch (exception)
            {
                case IntentionManagerException intentionManagerException:
                    problemDetails = problemDetailsFactory.CreateFrom(httpcontext, intentionManagerException);
                    break;
                case ValidationException validationException:
                    problemDetails = problemDetailsFactory.CreateFrom(httpcontext, validationException);
                    break;
                case DomainException domainException:
                    problemDetails = problemDetailsFactory.CreateFrom(httpcontext, domainException);
                    logger.LogError(domainException, "Domain exception occured");
                    break;
                default:
                    problemDetails = problemDetailsFactory.CreateProblemDetails(
                        httpcontext, StatusCodes.Status500InternalServerError, "Unhandled error");
                    logger.LogError(exception, "Unhandled exception occured");
                    break;
            }
            
            httpcontext.Response.StatusCode = problemDetails.Status ?? StatusCodes.Status500InternalServerError;
            await httpcontext.Response.WriteAsJsonAsync(problemDetails, problemDetails.GetType());
        }
    }
}