using System.Security.Principal;

namespace placeToBe.Model.Entities
{
    public class BasicAuthenticationIdentity : GenericIdentity
    {
        public BasicAuthenticationIdentity(string name, string password):base(name, "Basic")
        {
            this.password = password;
        }

        /// <summary>
        /// Basic Auth Password for custom authentication
        /// </summary>
        public string password { get; set; }
    }
}