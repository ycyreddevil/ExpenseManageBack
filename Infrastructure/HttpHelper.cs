using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Security;
using System.Text;
using System.Threading.Tasks;

namespace ExpenseManageBack.Infrastructure
{
    public class HttpHelper
    {
        public static string Post(string url, NameValueCollection data)
        {
            string result;

            if (url.ToLower().IndexOf("https", System.StringComparison.Ordinal) > -1)
            {
                ServicePointManager.ServerCertificateValidationCallback = new RemoteCertificateValidationCallback((sender, certificate, chain, errors) => { return true; });
            }

            try
            {
                var wc = new WebClient();
                if (string.IsNullOrEmpty(wc.Headers["Content-Type"]))
                {
                    wc.Headers.Add(HttpRequestHeader.ContentType, "application/x-www-form-urlencoded");

                }

                //if (string.IsNullOrEmpty(wc.Headers["User-Agent"]))
                //{
                //    wc.Headers.Add("User-Agent", @"Mozilla/5.0 (Windows NT 6.1; WOW64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/69.0.3497.100 Safari/537.36");
                //}

                wc.Encoding = Encoding.UTF8;

                result = Encoding.UTF8.GetString(wc.UploadValues(url, "POST", data));
            }
            catch (Exception e)
            {
                throw e;
            }

            return result;
        }

        public static string Post(string url, NameValueCollection data, WebClient wc)
        {
            string result;

            if (url.ToLower().IndexOf("https", System.StringComparison.Ordinal) > -1)
            {
                ServicePointManager.ServerCertificateValidationCallback = new RemoteCertificateValidationCallback((sender, certificate, chain, errors) => { return true; });
            }

            try
            {
                if (string.IsNullOrEmpty(wc.Headers["Content-Type"]))
                {
                    wc.Headers.Add(HttpRequestHeader.ContentType, "application/x-www-form-urlencoded");
                }
                //wc.Headers.Add(HttpRequestHeader.Cookie, "JSESSIONIDFPCXQD121=QPPRlRhifIxXE9pYTvA4pLCbOyjz1GEcc_IPowmuQx2VYB50_PWl2N-BiHBT2vQKDIw9evpl41IUvJLSdE3sM7XLFLqu9Eh9m0XTKA**");
                //wc.Headers.Add(HttpRequestHeader.Cookie, "AntiLeech=2670161455");
                //if (string.IsNullOrEmpty(wc.Headers["User-Agent"]))
                //{
                //    wc.Headers.Add("User-Agent", @"Mozilla/5.0 (Windows NT 6.1; WOW64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/69.0.3497.100 Safari/537.36");
                //}
                wc.Encoding = Encoding.UTF8;

                result = Encoding.UTF8.GetString(wc.UploadValues(url, "POST", data));
            }
            catch (Exception e)
            {
                throw e;
            }

            return result;
        }

        public static string Post(string url, string paramData)
        {
            return Post(url, paramData, Encoding.UTF8);
        }

        public static string Post(string url, string paramData, Encoding encoding)
        {
            string result;

            if (url.ToLower().IndexOf("https", System.StringComparison.Ordinal) > -1)
            {
                ServicePointManager.ServerCertificateValidationCallback = new RemoteCertificateValidationCallback((sender, certificate, chain, errors) => { return true; });
            }

            try
            {
                var wc = new WebClient();
                if (string.IsNullOrEmpty(wc.Headers["Content-Type"]))
                {
                    wc.Headers.Add("Content-Type", "application/x-www-form-urlencoded");
                }
                wc.Encoding = encoding;

                result = wc.UploadString(url, "POST", paramData);
            }
            catch (Exception e)
            {
                throw e;
            }

            return result;
        }

        public static string Get(string url)
        {
            return Get(url, Encoding.UTF8);
        }

        public static string Get(string url, Encoding encoding)
        {
            try
            {
                var wc = new WebClient { Encoding = encoding };
                var readStream = wc.OpenRead(url);
                using (var sr = new StreamReader(readStream, encoding))
                {
                    var result = sr.ReadToEnd();
                    return result;
                }
            }
            catch (Exception e)
            {
                throw e;
            }
        }
    }
}
