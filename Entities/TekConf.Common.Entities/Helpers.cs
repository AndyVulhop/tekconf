﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace TekConf.Common.Entities
{
    public static class Helpers
    {
        public static string GenerateSlug(this string phrase)
        {
            string slug = phrase.RemoveAccent().ToLower();
            // invalid chars           
            slug = Regex.Replace(slug, @"[^a-z0-9\s-]", "");
            // convert multiple spaces into one space   
            slug = Regex.Replace(slug, @"\s+", " ").Trim();
            // cut and trim 
            slug = slug.Substring(0, slug.Length <= 45 ? slug.Length : 45).Trim();
            slug = Regex.Replace(slug, @"\s", "-"); // hyphens   
            return slug;
        }

        public static string RemoveAccent(this string txt)
        {
	        if (!string.IsNullOrWhiteSpace(txt))
	        {
		        byte[] bytes = Encoding.GetEncoding("Cyrillic").GetBytes(txt);
		        return Encoding.ASCII.GetString(bytes);
	        }
	        else
	        {
		        return string.Empty;
	        }
        }
    }
}
