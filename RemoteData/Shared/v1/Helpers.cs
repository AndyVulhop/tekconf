using System.Text.RegularExpressions;

namespace TekConf.RemoteData.v1
{
	public static class Helpers
	{
		public static string GenerateSlug(this string phrase)
		{
			if (string.IsNullOrWhiteSpace(phrase))
				return string.Empty;

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
			byte[] bytes = System.Text.Encoding.GetEncoding("Cyrillic").GetBytes(txt);
			return System.Text.Encoding.UTF8.GetString(bytes, 0, bytes.Length);
		}
	}
}
