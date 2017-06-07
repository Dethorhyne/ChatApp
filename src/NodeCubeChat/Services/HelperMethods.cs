using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Security.Cryptography;
using System.ComponentModel;
using System.Reflection;
using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Net;
using System.IO;

public static class HelperMethods
{
    public static string BotImageUrl
    {
        get
        {
            return "/images/Avatar/Bot.jpg";
        }
    }
    public static string Md5Hash(string emailAddress)
    {
        emailAddress = emailAddress.Trim().ToLower();
        return GetMd5Hash(emailAddress);
    }

    /// <summary>
    /// Generates an MD5 hash of the given string
    /// </summary>
    private static string GetMd5Hash(string input)
    {
        byte[] data = MD5.Create().ComputeHash(Encoding.UTF8.GetBytes(input));
        StringBuilder sBuilder = new StringBuilder();
        for (int i = 0; i < data.Length; i++)
            sBuilder.Append(data[i].ToString("x2"));
        return sBuilder.ToString();
    }

}