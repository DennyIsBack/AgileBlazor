namespace AgileBlazor.Shared.Models.Authentication
{
    public class appSettings
    {
        /// criar objeto para referenciar quando receber json
            public string reCaptchaPublicKey { get; set; }
            public string reCaptchaPrivateKey { get; set; }
            public bool clientUseReCaptcha { get; set; }
    }
}
