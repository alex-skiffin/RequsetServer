﻿using System;
using System.IO;
using System.Net;
using System.Text;
using Server.DataBase;

namespace Server
{
    class ServerProcessor
    {
        readonly HttpListener _listener;
        public ServerProcessor(int port)
        {
            _listener = new HttpListener();
            int sslPort = port + 10000 > 65000 ? 17070 : port + 10000;
            _listener.Prefixes.Add(string.Format("http://*:{0}/", port));
            _listener.Prefixes.Add(string.Format("https://*:{0}/", sslPort));
            Console.WriteLine("Listening on port {0}...", port);
            Console.WriteLine("Listening on SSL port {0}...", sslPort);
        }

        public void Start()
        {
            var reqpro = new RequestProcessor();

            _listener.Start();

            while (true)
            {
                try
                {
                    HttpListenerContext context = _listener.GetContext();
                    HttpListenerRequest request = context.Request;
                    HttpListenerResponse response = context.Response;
                    //Создаем ответ
                    Stream inputStream = request.InputStream;
                    Encoding encoding = request.ContentEncoding;
                    StreamReader reader = new StreamReader(inputStream, encoding);
                    var requestBody = reader.ReadToEnd();

                    Console.WriteLine("{0} request was caught: {1}",
                        request.HttpMethod, request.Url);
                    var urlParts = request.Url.AbsolutePath.Split('/');
                    string msg;
                    
                    try
                    {
                        if (urlParts[1] != "register")
                            DbProcessor.CheckAuth(urlParts[2], urlParts[3]);
                        msg = reqpro.GetResponseData(request.HttpMethod, urlParts, requestBody);
                        response.StatusCode = (int)HttpStatusCode.OK;
                    }
                    catch (Exception e)
                    {
                        msg = "Request error! " + e.Message;
                        Console.WriteLine(msg);
                        response.StatusCode = (int)HttpStatusCode.BadRequest;
                    }


                    byte[] b = Encoding.UTF8.GetBytes(msg);
                    context.Response.ContentLength64 = b.Length;
                    context.Response.OutputStream.Write(b, 0, b.Length);
                }
                catch (Exception exception)
                {
                    Console.WriteLine("Net trouble\r\n" + exception.Message);
                }
            }
        }

        public void Stop()
        {
            if (_listener != null)
            {
                _listener.Stop();
            }
        }

        ~ServerProcessor()
        {
            Stop();
        }
    }
}