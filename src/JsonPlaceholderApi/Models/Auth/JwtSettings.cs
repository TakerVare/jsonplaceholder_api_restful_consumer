namespace JsonPlaceholderApi.Models.Auth;

/// <summary>
/// Configuracion para JWT extraida de appsettings.json.
///
/// POR QUE EXTERNALIZAR LA CONFIGURACION:
/// 1. Permite cambiar valores sin recompilar
/// 2. Diferentes valores por ambiente (Dev, Staging, Prod)
/// 3. Secrets pueden manejarse con User Secrets o Azure Key Vault
/// </summary>
public class JwtSettings
{
    /// <summary>
    /// Clave secreta para firmar los tokens.
    /// IMPORTANTE: En produccion usar una clave fuerte y almacenarla de forma segura.
    /// Minimo 32 caracteres para HMAC-SHA256.
    /// </summary>
    public string SecretKey { get; set; } = string.Empty;

    /// <summary>
    /// Emisor del token (nuestra API).
    /// POR QUE VALIDAR ISSUER:
    /// Garantiza que el token fue emitido por nuestra API y no por otro sistema.
    /// </summary>
    public string Issuer { get; set; } = string.Empty;

    /// <summary>
    /// Audiencia del token (quien puede usarlo).
    /// POR QUE VALIDAR AUDIENCE:
    /// Permite restringir tokens a aplicaciones especificas.
    /// </summary>
    public string Audience { get; set; } = string.Empty;

    /// <summary>
    /// Tiempo de expiracion del token en minutos.
    /// POR QUE TOKENS EXPIRAN:
    /// - Limita el tiempo de exposicion si un token es comprometido
    /// - Fuerza renovacion periodica de credenciales
    /// - Balance entre seguridad (corto) y usabilidad (largo)
    /// </summary>
    public int ExpirationMinutes { get; set; } = 60;
}
