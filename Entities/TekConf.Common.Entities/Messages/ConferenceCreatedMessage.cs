namespace TekConf.Common.Entities
{
	public class ConferenceCreatedMessage : TinyMessageBase
	{
		public string ConferenceSlug { get; set; }
		public string ConferenceName { get; set; }

		public override bool Equals(object obj)
		{
			if (obj == null)
				return false;

			var p = obj as ConferenceCreatedMessage;
			if (p == null)
				return false;

			// Return true if the fields match:
			return (this.ConferenceName == p.ConferenceName) && (this.ConferenceSlug == p.ConferenceSlug);
		}

		public override int GetHashCode()
		{
			return this.ConferenceName.GetHashCode() + this.ConferenceSlug.GetHashCode();
		}
	}
}