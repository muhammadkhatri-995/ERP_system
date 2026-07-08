// auth-guard.js
// Runs immediately when loaded — must be the first script in <head>.
(function () {
    const token = localStorage.getItem("erp_token") || sessionStorage.getItem("erp_token");
    const role = localStorage.getItem("erp_role") || sessionStorage.getItem("erp_role");

    if (!token) {
        window.location.replace("/login.html");
        return;
    }

    const allowedRoles = window.ALLOWED_ROLES;
    if (allowedRoles && !allowedRoles.includes(role)) {
        window.location.replace("/unauthorized.html");
        return;
    }
})();

function getToken() {
    return localStorage.getItem("erp_token") || sessionStorage.getItem("erp_token");
}

function authHeaders(extraHeaders = {}) {
    return { ...extraHeaders, "Authorization": `Bearer ${getToken()}` };
}

function logout() {
    localStorage.removeItem("erp_token");
    localStorage.removeItem("erp_userId");
    localStorage.removeItem("erp_name");
    localStorage.removeItem("erp_email");
    localStorage.removeItem("erp_role");
    sessionStorage.clear();
    window.location.href = "/login.html";
}