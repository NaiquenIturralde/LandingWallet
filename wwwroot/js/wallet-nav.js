/* ==========================================================================
   wallet-nav — controles de navegación de la App Wallet.

   1. Atrás / Adelante: Blazor no expone el historial real del navegador
      (solo rutas de la app), así que estas funciones replican exactamente el
      comportamiento de los botones nativos, incluido el caso en que no hay
      entrada anterior/siguiente (el navegador no hace nada).
   2. Sidebar de desktop: el estado de la navegación contraída vive en
      localStorage para sobrevivir a la recarga y a los cambios de vista —el
      shell se vuelve a montar en cada ruta—, y la clase en <html> es la que
      reduce --ws-sidebar-w (tokens.css): la columna de navegación y la reserva
      de la layout bajan juntas, así el contenido adopta el ancho liberado.

   El módulo se carga una sola vez desde App.razor, antes de blazor.web.js,
   para que el shell pueda invocarlo apenas monte.
   ========================================================================== */

const WALLET_NAV_COLLAPSED_KEY = 'wallet.navCollapsed';

window.walletNav = {
    back: () => window.history.back(),
    forward: () => window.history.forward(),

    // Estado persistido de la sidebar. Sin storage (modo privado, cookies
    // bloqueadas) el estado vive solo en la sesión actual de la aplicación.
    getNavCollapsed: () => {
        try {
            return window.localStorage.getItem(WALLET_NAV_COLLAPSED_KEY) === '1';
        } catch {
            return false;
        }
    },

    setNavCollapsed: (collapsed) => {
        const isCollapsed = collapsed === true;
        document.documentElement.classList.toggle('wallet-nav-collapsed', isCollapsed);

        try {
            window.localStorage.setItem(WALLET_NAV_COLLAPSED_KEY, isCollapsed ? '1' : '0');
        } catch {
            // Sin storage el estado vive solo en la sesión actual.
        }
    },
};
