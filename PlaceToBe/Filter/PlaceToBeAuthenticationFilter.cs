using System;
using System.Net;
using System.Net.Http;
using System.Security.Principal;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Http.Controllers;
using System.Web.Http.Filters;
using placeToBe.Filter;
using placeToBe.Model.Entities;
using placeToBe.Model.Repositories;
using placeToBe.Services;

/// <summary>
///  Basic Authentication and Facebook Token/ID based filter that checks for basic authentication
/// headers and challenges for authentication if no authentication is provided
/// Sets the Thread Principle with a GenericAuthenticationPrincipal.
/// 
/// You can override the OnAuthorize method for custom auth logic that
/// might be application specific.    
/// </summary>
/// <remarks>Always remember that Basic Authentication passes username and passwords
/// from client to server in plain text, so make sure SSL is used with basic auth
/// to encode the Authorization header on all requests (not just the login).
/// </remarks>
[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = false)]
public class PlaceToBeAuthenticationFilter : AuthorizationFilterAttribute
{
    private readonly AccountService acc = new AccountService();
    private readonly UserRepository userRepo = new UserRepository();
    private readonly FacebookUserVerification fbVerification = new FacebookUserVerification();

    bool Active = true;

    public PlaceToBeAuthenticationFilter()
    { }

    /// <summary>
    /// Overriden constructor to allow explicit disabling of this
    /// filter's behavior. Pass false to disable (same as no filter
    /// but declarative)
    /// </summary>
    /// <param name="active"></param>
    public PlaceToBeAuthenticationFilter(bool active)
    {
        Active = active;
    }


    /// <summary>
    /// Override to Web API filter method to handle Basic Auth check
    /// </summary>
    /// <param name="actionContext"></param>
    /// 
    public override async Task OnAuthorizationAsync(HttpActionContext actionContext, CancellationToken cancellationToken)
    {
        if (Active)
        {
            var identity = ParseAuthorizationHeader(actionContext);
            if (identity == null)
            {
                denyAccess(actionContext);
                return;
            }


            if (! await OnAuthorizeUser(identity.Name, identity.password, actionContext))
            {
                denyAccess(actionContext);
                return;
            }

            var principal = new GenericPrincipal(identity, null);

            Thread.CurrentPrincipal = principal;

            // inside of ASP.NET this is required
            //if (HttpContext.Current != null)
            //    HttpContext.Current.User = principal;

            base.OnAuthorization(actionContext);
        }
    }

    /// <summary>
    /// Base implementation for user authentication - you probably will
    /// want to override this method for application specific logic.
    /// 
    /// The base implementation merely checks for username and password
    /// present and set the Thread principal.
    /// 
    /// Override this method if you want to customize Authentication
    /// and store user data as needed in a Thread Principle or other
    /// Request specific storage.
    /// </summary>
    /// <param name="username"></param>
    /// <param name="password"></param>
    /// <param name="actionContext"></param>
    /// <returns></returns>
    protected virtual Task<bool> OnAuthorizeUser(string usrEmailOrId, string usrTknOrPass, HttpActionContext actionContext)
    {
        //if (string.IsNullOrEmpty(usrEmailOrId) || string.IsNullOrEmpty(usrTknOrPass)) return false;

        //if usrEmailOrId is only numbers --> fbID
        string regx = @"^[0-9]*$";
        if (Regex.IsMatch(usrEmailOrId, regx)) {
           return  fbVerification.authorizeRequest(usrEmailOrId, usrTknOrPass);
        }

        //contains Authorization header --> perform checkPassword
        return checkPtbPassword(usrEmailOrId, usrTknOrPass);
        
    }


    /// <summary>
    /// Checks the password from the local database user against the provided password
    /// </summary>
    /// <param name="userEmail"></param>
    /// <param name="userPassword"></param>
    /// <returns></returns>
    private async Task<bool> checkPtbPassword(string userEmail, string userPassword)
    {
        try
        {
            var userPasswordInBytes = Encoding.UTF8.GetBytes(userPassword);
            var user = userRepo.GetByEmailAsync(userEmail);
            if (user == null) return false;
            var salt = user.salt;
            var passwordSalt = acc.GenerateSaltedHash(userPasswordInBytes, salt);
            var comparePasswords = acc.CompareByteArrays(passwordSalt, user.passwordSalt);

            //statement: if users password is correct and status is activated          
            if (comparePasswords && user.status) return true;
            if (comparePasswords && user.status == false)
            {
                //Please activate your acc.
                return false;
            }
            //Authentification failed.
            return false;
        }
        catch (Exception e)
        {
            //Something go wrong.
            return false;
        }
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
    /// Send a 401 back
    /// </summary>
    /// <param name="message"></param>
    /// <param name="actionContext"></param>
    void denyAccess(HttpActionContext actionContext)
    {
        actionContext.Response = actionContext.Request.CreateResponse(HttpStatusCode.Unauthorized);
    }


    

}