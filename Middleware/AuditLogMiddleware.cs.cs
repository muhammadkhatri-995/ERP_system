using ERP_sys.Attributes;
using ERP_sys.Models;
using ERP_sys.Repositories;
using Microsoft.AspNetCore.Http;
using System;
using System.Linq;
using System.Reflection;
using System.Security.Claims;
using System.Threading.Tasks;

namespace ERP_sys.Middleware
{
    public class AuditLogMiddleware
    {
        private readonly RequestDelegate _next;

        public AuditLogMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context, IAuditLogRepository auditLogRepository)
        {
            await _next(context);

            if (!context.Request.Path.StartsWithSegments("/api"))
                return;

            try
            {
                var user = context.User;
                int? userId = null;
                string? userName = null;
                string? userRole = null;

                if (user?.Identity != null && user.Identity.IsAuthenticated)
                {
                    var idClaim = user.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                    if (int.TryParse(idClaim, out int parsedId))
                        userId = parsedId;

                    userName = user.FindFirst("name")?.Value ?? user.FindFirst(ClaimTypes.Email)?.Value;
                    userRole = user.FindFirst(ClaimTypes.Role)?.Value;
                }

                var ipAddress = context.Connection.RemoteIpAddress?.ToString();
                if (context.Request.Headers.TryGetValue("X-Forwarded-For", out var forwardedFor))
                {
                    ipAddress = forwardedFor.ToString().Split(',')[0].Trim();
                }

                string action = GetActionDescription(context);

                var log = new AuditLogs
                {
                    UserId = userId,
                    UserName = userName,
                    UserRole = userRole,
                    IpAddress = ipAddress,
                    Action = action,
                    HttpMethod = context.Request.Method,
                    Path = context.Request.Path.Value ?? "",
                    StatusCode = context.Response.StatusCode,
                    UserAgent = context.Request.Headers["User-Agent"].ToString(),
                    Timestamp = DateTime.Now
                };

                await auditLogRepository.InsertAsync(log);
            }
            catch
            {
                // Never let logging failures break the actual request
            }
        }

        private string GetActionDescription(HttpContext context)
        {
            // 1. Try to find an [AuditAction] attribute on the endpoint that handled this request
            var endpoint = context.GetEndpoint();
            var actionDescriptor = endpoint?.Metadata?.GetMetadata<AuditActionAttribute>();

            if (actionDescriptor != null)
            {
                return actionDescriptor.Description;
            }

            // 2. Fallback — build a generic description from method + path
            var method = context.Request.Method;
            var path = context.Request.Path.Value ?? "";

            return method switch
            {
                "GET" => $"Viewed {path}",
                "POST" => $"Created record via {path}",
                "PUT" => $"Updated record via {path}",
                "DELETE" => $"Deleted record via {path}",
                _ => $"{method} {path}"
            };
        }
    }
}