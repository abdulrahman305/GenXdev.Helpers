namespace GenXdev.Helpers
{
    public class SimpleCustomTwoFactorAutentication
    {
        public static SimpleMachineWidePersistentStorage<SimpleAccessTokenList> Store { get; private set; } = new SimpleMachineWidePersistentStorage<SimpleAccessTokenList>("auth", new SimpleAccessTokenList());

        public string RequestAccessToken() {

            lock (Store)
            {
                var data = Store.Data;
                data.CleanUpExpired(TimeSpan.FromMinutes(10));

                try
                {
                    return data.CreateNewToken();
                }
                finally
                {
                    Store.Data = data;
                }
            }
        }

        public void AuthenticateAccessToken(string token)
        {
            lock (Store)
            {
                var data = Store.Data;
                data.CleanUpExpired(TimeSpan.FromMinutes(10));

                try
                {

                    if (data.TryGetToken(token, out SimpleAccessToken current))
                    {
                        current.IsAuthenticated = true;
                    }
                }
                finally
                {
                    Store.Data = data;
                }
            }
        }

        public bool CheckAccess(string token)
        {
            lock (Store)
            {
                var data = Store.Data;
                data.CleanUpExpired(TimeSpan.FromMinutes(10));

                try
                {

                    if (data.TryGetToken(token, out SimpleAccessToken current))
                    {
                        return current.IsAuthenticated;
                    }
                }
                finally
                {
                    Store.Data = data;
                }

                return false;
            }
        }
    }

    public class SimpleAccessToken
    {
        public string Token { get; set; } = Guid.NewGuid().ToString().Replace("-","").ToLowerInvariant();
        public System.DateTime PublishDate { get; set; } = System.DateTime.UtcNow;
        public bool IsAuthenticated { get; set; } = false;
    }

    public class SimpleAccessTokenList 
    {
        public List<SimpleAccessToken> Tokens { get; set; } = new List<SimpleAccessToken>();

        public string CreateNewToken()
        {
            var newToken = new SimpleAccessToken();

            Tokens.Add(newToken);

            return newToken.Token;
        } 

        public bool TryGetToken(string Token, out SimpleAccessToken Value)
        {
            foreach (var token in Tokens)
            {
                if (token.Token == Token)
                {
                    Value = token;
                    return true;
                }
            }

            Value = null;
            return false;
        }

        public void CleanUpExpired(TimeSpan eExpirationTime)
        {
            var now = System.DateTime.UtcNow;

            for (var i = Tokens.Count - 1; i >= 0; i--)
            {
                if (now - Tokens[i].PublishDate > eExpirationTime)
                {
                    Tokens.RemoveAt(i);
                }
            }
        }
    }
}
