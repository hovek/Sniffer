using System;
using System.Collections.Generic;
using System.Text;
using System.Reflection;
using Sniffer.Common;

namespace Sniffer.Sniffs.Dvoznak
{
	public class Tip : ICloneable
	{
		public static readonly Dictionary<string, PropertyInfo> PropertyInfos = new Dictionary<string, PropertyInfo>();
		public static readonly Dictionary<string, Field> EnumFields = new Dictionary<string, Field>();

		private static Type stringType = typeof(string);

		static Tip()
		{
			Type fieldType = typeof(Field);
			Array fields = Enum.GetValues(fieldType);
			foreach (Field field in fields)
			{
				EnumFields.Add(Enum.GetName(fieldType, field), field);
			}

			foreach (PropertyInfo pi in typeof(Tip).GetProperties())
			{
				if (EnumFields.ContainsKey(pi.Name))
				{
					PropertyInfos.Add(pi.Name, pi);
				}
			}
		}

		public enum Field
		{
			Liga,
			Dogadjaj,
			DogadjajUrl,
			TipOdigravanja,
			Odigravanje,
			Opis,
			Tipster,
			TipsterUrl,
			TipsterBrojOklada,
			TipsterBrojPogodaka,
			TipsterBrojPovrata,
			TipsterUlog,
			TipsterPovrat,
			Koeficijent,
			Ulog,
			MinKoef,
			Objavljeno,
			Kladionica,
			KladionicaUrl,
			KladionicaImgUrl,
			Rezultat,
			Sport,
			SportImgUrl,
			Zarada,
			ZastavaClass,
			Uspjesnost,
			UspjesnostTitle,
			Premium,
			IsMargina,
			Misc
		}

		public enum Outcome
		{
			[EnumStringValue("u")]
			Unfinished,
			[EnumStringValue("u0")]
			Loss,
			[EnumStringValue("u1")]
			Won,
			[EnumStringValue("u2")]
			Won2
		}

		public int Id { get; set; }
		public string Liga { get; set; }
		public string Dogadjaj { get; set; }
		public string DogadjajUrl { get; set; }
		public string TipOdigravanja { get; set; }
		public DateTime Odigravanje { get; set; }
		public string Opis { get; set; }
		public string Tipster { get; set; }
		public string TipsterUrl { get; set; }
		public int TipsterBrojOklada { get; set; }
		public int TipsterBrojPogodaka { get; set; }
		public int TipsterBrojPovrata { get; set; }
		public decimal TipsterUlog { get; set; }
		public decimal TipsterPovrat { get; set; }
		public decimal Koeficijent { get; set; }
		public int Ulog { get; set; }
		public decimal? MinKoef { get; set; }
		public DateTime Objavljeno { get; set; }
		public string Kladionica { get; set; }
		public string KladionicaUrl { get; set; }
		public string KladionicaImgUrl { get; set; }
		public string Rezultat { get; set; }
		public string Sport { get; set; }
		public string SportImgUrl { get; set; }
		public decimal? Zarada { get; set; }
		public string ZastavaClass { get; set; }
		public Outcome Uspjesnost { get; set; }
		public string UspjesnostTitle { get; set; }
		public bool Premium { get; set; }
		public string IsMargina { get; set; }
		public string Misc { get; set; }

		public decimal TipsterROI
		{
			get
			{
				return TipsterUlog == 0 ? 0 : (TipsterPovrat - TipsterUlog) / TipsterUlog * 100;
			}
		}

		public List<Field> GetFieldsThatDiffer(Tip other)
		{
			List<Field> fieldsThatDiffer = new List<Field>();

			foreach (PropertyInfo pi in PropertyInfos.Values)
			{
				dynamic thisVal = pi.GetValue(this, null) ?? (pi.PropertyType == stringType ? "" : Activator.CreateInstance(pi.PropertyType));
				dynamic otherVal = pi.GetValue(other, null) ?? (pi.PropertyType == stringType ? "" : Activator.CreateInstance(pi.PropertyType));

				if (thisVal is string)
				{
					if (Common.Common.RemoveAllWhiteSpace(thisVal.ToString()) != Common.Common.RemoveAllWhiteSpace(otherVal.ToString()))
					{
						fieldsThatDiffer.Add(EnumFields[pi.Name]);
					}
				}
				else if (thisVal != otherVal)
				{
					fieldsThatDiffer.Add(EnumFields[pi.Name]);
				}
			}

			return fieldsThatDiffer;
		}

		public static List<Tip> FindSimilar(List<Tip> list, Tip toFind)
		{
			List<Tip> similarTips = new List<Tip>();
			foreach (Tip prognoza in list)
			{
				if (Common.Common.RemoveAllWhiteSpace(prognoza.Tipster) == Common.Common.RemoveAllWhiteSpace(toFind.Tipster)
					&& Common.Common.RemoveAllWhiteSpace(prognoza.Dogadjaj) == Common.Common.RemoveAllWhiteSpace(toFind.Dogadjaj)
					&& Common.Common.RemoveAllWhiteSpace(prognoza.TipOdigravanja) == Common.Common.RemoveAllWhiteSpace(toFind.TipOdigravanja))
				{
					similarTips.Add(prognoza);
				}
			}

			return similarTips;
		}

		public bool IsEqualTo(Tip other)
		{
			foreach (PropertyInfo pi in PropertyInfos.Values)
			{
				dynamic thisVal = pi.GetValue(this, null) ?? (pi.PropertyType == stringType ? "" : Activator.CreateInstance(pi.PropertyType));
				dynamic otherVal = pi.GetValue(other, null) ?? (pi.PropertyType == stringType ? "" : Activator.CreateInstance(pi.PropertyType));

				if (thisVal is string)
				{
					if (Common.Common.RemoveAllWhiteSpace(thisVal.ToString()) != Common.Common.RemoveAllWhiteSpace(otherVal.ToString()))
					{
						return false;
					}
				}
				else if (thisVal != otherVal)
				{
					return false;
				}
			}

			return true;
		}

		public object GetPropertyValue(string propertyName)
		{
			return PropertyInfos[propertyName].GetValue(this, null);
		}

		public void SetPropertyValue(string propertyName, object value)
		{
			PropertyInfos[propertyName].SetValue(this, value, null);
		}

		public object Clone()
		{
			return MemberwiseClone();
		}
	}
}