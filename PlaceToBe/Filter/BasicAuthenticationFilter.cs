﻿using placeToBe.Model.Entities;
using System;
using System.Net;
using System.Net.Http;
using System.Security.Principal;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http.Controllers;
using System.Web.Http.Filters;

/// <summary>
/// Generic Basic Authentication filter that checks for basic authentication
/// headers and challenges for authentication if no authentication is provided
/// Sets the Thread Principle with a GenericAuthenticationPrincipal.
/// </summary>

[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = false)]
public class BasicAuthenticationFilter : AuthorizationFilterAttribute
{
    bool Active = true;

    public BasicAuthenticationFilter()
    { }

    /// <summary>
    /// Overriden constructor to allow explicit disabling of this
    /// filter's behavior. Pass false to disable (same as no filter
    /// but declarative)
    /// </summary>
    /// <param name="active"></param>
    public BasicAuthenticationFilter(bool active)
    {
        Active = active;
    }


    /// <summary>
    /// Override to Web API filter method to handle Basic Auth check
    /// </summary>
    /// <param name="actionContext"></param>
    public async override void OnAuthorization(HttpActionContext actionContext)
    {
        if (Active)
        {
            
            
            var identity = ParseAuthorizationHeader(actionContext);
            if (identity == null)
            {
                //changed since we also want to listen to FB auth headers
                identity = new BasicAuthenticationIdentity("", "");
                Challenge(actionContext);
            }


            if (! await OnAuthorizeUser(identity.Name, identity.password, actionContext))
            {
                Challenge(actionContext);
                return;
            }

            var principal = new GenericPrincipal(identity, null);

            Thread.CurrentPrincipal = principal;

            if (HttpContext.Current != null)
                HttpContext.Current.User = principal;

            base.OnAuthorization(actionContext);
        }
    }

    /// <summary>
    /// Base implementation for user authentication
    /// The base implementation merely checks for username and password
    /// present and set the Thread principal.
    /// Override this method if you want to customize Authentication
    /// and store user data as needed in a Thread Principle or other
    /// Request specific storage.
    /// </summary>
    /// <param name="username"></param>
    /// <param name="password"></param>
    /// <param name="actionContext"></param>
    /// <returns></returns>
    protected virtual Task<bool> OnAuthorizeUser(string username, string password, HttpActionContext actionContext)
    {
       return null;
    }

    /// <summary>
    /// Parses the Authorization header and creates user credentials
    /// </summary>
    /// <param name="actionContext"></param>
    protected virtual BasicAuthenticationIdentity ParseAuthorizationHeader(HttpActionContext actionContext)
    {
        string authHeader = null;
        var auth = actionContext.Request.Headers.Authorization;
        if (auth != null && auth.Scheme == "Basic")
            authHeader = auth.Parameter;

        if (string.IsNullOrEmpty(authHeader))
            return null;

        authHeader = Encoding.Default.GetString(Convert.FromBase64String(authHeader));

        var tokens = authHeader.Split(':');
        if (tokens.Length < 2)
            return null;

        return new BasicAuthenticationIdentity(tokens[0], tokens[1]);
    }


    /// <summary>
    /// Send the Authentication Challenge request
    /// </summary>
    /// <param name="message"></param>
    /// <param name="actionContext"></param>
    void Challenge(HttpActionContext actionContext)
    {
        var host = actionContext.Request.RequestUri.DnsSafeHost;
        actionContext.Response = actionContext.Request.CreateResponse(HttpStatusCode.Unauthorized);
        actionContext.Response.Headers.Add("WWW-Authenticate", string.Format("Basic realm=\"{0}\"", host));
    }

}