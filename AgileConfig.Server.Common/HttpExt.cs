﻿using Microsoft.AspNetCore.Http;
using System;
using System.Linq;
using System.Text;

namespace AgileConfig.Server.Common
{
    public static class HttpExt
    {
        /// <summary>
        /// 从request 的 Authorization 头部中解析出 username, password
        /// </summary>
        /// <param name="httpRequest"></param>
        /// <returns></returns>
        public static (string, string) GetUserNamePasswordFromBasicAuthorization(this HttpRequest httpRequest)
        {
            var authorization = httpRequest.Headers["Authorization"];
            if (string.IsNullOrEmpty(authorization))
            {
                return ("", "");
            }
            var authStr = authorization.First();
            //去掉basic_
            if (!authStr.StartsWith("Basic "))
            {
                return ("", "");
            }
            authStr = authStr.Substring(6, authStr.Length - 6);
            byte[] base64Decode = null;
            try
            {
                base64Decode = Convert.FromBase64String(authStr);
            }
            catch
            {
                return ("", "");
            }
            var base64Str = Encoding.UTF8.GetString(base64Decode);

            if (string.IsNullOrEmpty(base64Str))
            {
                return ("", "");
            }

            var userName = "";
            var password = "";
            var baseAuthArr = base64Str.Split(':');

            if (baseAuthArr.Length > 0)
            {
                userName = baseAuthArr[0];
            }
            if (baseAuthArr.Length > 1)
            {
                password = baseAuthArr[1];
            }

            return (userName, password);
        }


        public static string GetUserNameFromClaim(this HttpContext httpContext)
        {
            var name = httpContext.User?.FindFirst("username")?.Value;

            return name;
        }

        public static string GetUserIdFromClaim(this HttpContext httpContext)
        {
            return httpContext.User?.FindFirst("id")?.Value;
        }

        /// <summary>
        /// 获取客户端IP
        /// </summary>
        /// <param name="httpRequest"></param>
        /// <returns></returns>
        public static System.Net.IPAddress GetRemoteIp(HttpRequest httpRequest)
        {
            System.Net.IPAddress ip;
            var headers = httpRequest.Headers.ToList();
            if (headers.Exists((kvp) => kvp.Key == "X-Forwarded-For"))
            {
                // when running behind a load balancer you can expect this header
                var header = headers.First((kvp) => kvp.Key == "X-Forwarded-For").Value.ToString();
                System.Net.IPAddress.TryParse(header, out ip);
            }
            else
            {
                // this will always have a value (running locally in development won't have the header)
                ip = httpRequest.HttpContext.Connection.RemoteIpAddress;
            }

            return ip;
        }

    }
}
