using System;
using System.Collections.Generic;
using System.Net;
using System.Text;
using System.IO;
using System.Linq;

namespace Server
{
    abstract class Handler
    {
        public Handler Successor { get; set; }
        public abstract void HandlerRequest(HttpListenerContext context);
    }

    class StreamHandler : Handler
    {
        public override void HandlerRequest(HttpListenerContext context)
        {
            HttpListenerRequest request = context.Request;
            string query = request.QueryString.GetKey(0);

            if (query == "text")
            {
                HttpListenerResponse response = context.Response;

                string bodyText = $"<html><head><meta charset='utf8'></head><body>{request.QueryString.Get(query)}</body></html>";

                byte[] byteBodyText = Encoding.UTF8.GetBytes(bodyText);
                response.ContentLength64 = byteBodyText.Length;

                using(Stream output = response.OutputStream)
                {
                    output.Write(byteBodyText, 0, byteBodyText.Length);
                }
            }
            else if (Successor != null)
            {
                Successor.HandlerRequest(context);
            }
        }
    }

    class ConvertHandler : Handler
    {
        public override void HandlerRequest(HttpListenerContext context)
        {
            HttpListenerRequest request = context.Request;
            string query = request.QueryString.GetKey(0);

            if (query == "convert")
            {
                HttpListenerResponse response = context.Response;
                string valueQueryString = request.QueryString.Get(query);
                string[] bytesString = Encoding.UTF8.GetBytes(valueQueryString).Select((byte item) => $"{item}").ToArray();

                string bodyText = $"<html><head><meta charset='utf8'></head><body>{String.Join("", bytesString)}</body></html>";
                byte[] byteBodyText = Encoding.UTF8.GetBytes(bodyText);

                response.ContentLength64 = byteBodyText.Length;

                using (Stream output = response.OutputStream)
                {
                    output.Write(byteBodyText, 0, byteBodyText.Length);
                }
            }
            else if (Successor != null)
            {
                Successor.HandlerRequest(context);
            }
        }
    }
}
