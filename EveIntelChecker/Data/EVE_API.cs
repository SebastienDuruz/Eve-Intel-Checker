using ESI.NET;
using ESI.NET.Enumerations;
using ESI.NET.Models.SSO;
using Microsoft.Extensions.Options;

namespace EveIntelChecker.Data
{
    public class EVE_API
    {
        public IEsiClient _client;
        private string _state = "ttkgpa";
        public string _ssoURL;

        private IOptions<EsiConfig> _config = Options.Create(new EsiConfig()
        {
            EsiUrl = "https://esi.evetech.net/",
            DataSource = DataSource.Tranquility,
            ClientId = "****",
            SecretKey = "****",
            CallbackUrl = "https://localhost:5001/",
            UserAgent = "Merk Ataru"
        });

        public EVE_API() 
        {
            _client = new EsiClient(_config);
            _ssoURL = _client.SSO.CreateAuthenticationUrl(null, _state);
        }

        public async Task APIAuth(string code)
        {
            bool isAuth = false;
            while(isAuth != true)
            {
                try
                {
                    SsoToken token = await _client.SSO.GetToken(GrantType.AuthorizationCode, code);
                    AuthorizedCharacterData auth_char = await _client.SSO.Verify(token);
                    _client.SetCharacterData(auth_char);
                    isAuth = true;
                }
                catch (Exception)
                {

                }
            }
        }
    }
}
