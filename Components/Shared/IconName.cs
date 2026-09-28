namespace LandingWallet.Components.Shared;

/// <summary>
/// Conjunto de iconos disponibles en <see cref="Icon"/>.
/// Abstracción tipada sobre Google Material Symbols Outlined: cada valor se
/// resuelve internamente al nombre oficial del símbolo (por ejemplo,
/// <see cref="Home"/> → "home"). Los consumidores usan el enum y nunca
/// escriben spans de Material Symbols directamente.
/// </summary>
public enum IconName
{
    // --- Base de la landing ---
    Wallet,
    Transfer,
    Account,
    Security,
    Analytics,
    Help,
    Check,
    CheckCircle,
    Warning,
    Error,
    Info,
    Refresh,
    Arrow,
    Currency,
    /// <summary>Chevron hacia abajo (expand_more) — acordeones y filtros.</summary>
    ExpandMore,

    // --- Navegación y acciones de la App Wallet ---
    Home,
    Send,
    Receive,
    Transactions,
    Person,
    Add,
    Search,
    Filter,
    Settings,
    Profile,
    Notifications,
    CurrencyExchange,
    TrendingUp,
    TrendingDown,
    TrendingFlat,
    Close,
    ArrowBack,
    ArrowForward,
    More,

    // --- Tipos de movimiento de la App Wallet ---
    Deposit,
    Withdrawal,
    Subscription,
    Purchase,
    ServicePayment,
    Salary,
    Refund,

    // --- Admin ---
    Dashboard,
    Accounts,
    Users,
    Audit,
    Logout,
}
