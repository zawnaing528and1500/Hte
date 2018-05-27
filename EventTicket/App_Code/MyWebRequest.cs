﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Net;

namespace EventTicket.App_Code
{
    public class MyWebRequest
    {
        private System.Net.WebRequest request;
        private System.IO.Stream dataStream;

        private string status;
        public String Status
        {
            get
            {
                return status;
            }
            set
            {
                status = value;
            }
        }

        public MyWebRequest(string url)
        {
            // Create a request using a URL that can receive a post.

            request = WebRequest.Create(url);
        }

        public MyWebRequest(string url, string method)
            : this(url)
        {

            if (method.Equals("GET") || method.Equals("POST"))
            {
                // Set the Method property of the request to POST.
                request.Method = method;
            }
            else
            {
                throw new Exception("Invalid Method Type");
            }
        }

        public MyWebRequest(string url, string method, string data)
            : this(url, method)
        {

            // Create POST data and convert it to a byte array.
            string postData = data;
            byte[] byteArray = System.Text.Encoding.UTF8.GetBytes(postData);

            // Set the ContentType property of the WebRequest.
            request.ContentType = "application/x-www-form-urlencoded";

            // Set the ContentLength property of the WebRequest.
            request.ContentLength = byteArray.Length;

            // Get the request stream.
            dataStream = request.GetRequestStream();

            // Write the data to the request stream.
            dataStream.Write(byteArray, 0, byteArray.Length);

            // Close the Stream object.
            dataStream.Close();

        }

        public string GetResponse()
        {
            // Get the original response.
            WebResponse response = request.GetResponse();

            this.Status = ((HttpWebResponse)response).StatusCode.ToString();

            //// Get the stream containing all content returned by the requested server.
            //dataStream = response.GetResponseStream();

            //// Open the stream using a StreamReader for easy access.
            //StreamReader reader = new StreamReader(dataStream);

            //// Read the content fully up to the end.
            //string responseFromServer = reader.ReadToEnd();

            //// Clean up the streams.
            //reader.Close();
            //dataStream.Close();
            //response.Close();

            return Status;
        }
    }
}