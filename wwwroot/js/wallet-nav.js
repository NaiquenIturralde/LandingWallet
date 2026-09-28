/* ==========================================================================
   wallet-nav — controles Atrás / Adelante de la App Wallet.

   Blazor no expone el historial real del navegador (solo rutas de la app), así
   que este módulo es el único punto de contacto con history.back() y
   history.forward(). Se carga una sola vez desde App.razor, antes de
   blazor.web.js, para que el shell pueda invocarlo apenas monte.

   Las funciones no reciben argumentos ni devuelven nada: replican exactamente
   el comportamiento de los botones nativos del navegador, incluido el caso en
   que no hay entrada anterior/siguiente (el navegador no hace nada).
   ========================================================================== */

window.walletNav = {
    back: () => window.history.back(),
    forward: () => window.history.forward(),
};
