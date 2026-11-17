using System;

using System.ComponentModel;



namespace Reporting.Map.WebForms

{

	internal class SymbolFieldPropertyDescriptor : PropertyDescriptor

	{

		private Field field;



		public override Type ComponentType => typeof(Symbol);



		public override string DisplayName => this.field.Name;



		public override bool IsBrowsable => true;



		public override bool IsReadOnly => this.field.IsTemporary;



		public override Type PropertyType => this.field.Type;



		public SymbolFieldPropertyDescriptor(Field field)

			: base(field.Name, new Attribute[2]

			{

				new CategoryAttribute(SR.CategoryAttribute_SymbolFields),

				new DescriptionAttribute(SR.DescriptionAttributeSymbol_Fields(field.Name))

			})

		{

			this.field = field;

		}



		public override bool CanResetValue(object component)

		{

			return false;

		}



		public override object GetValue(object component)

		{

			try

			{

				return ((Symbol)component)[this.field.Name];

			}

			catch (Exception)

			{

				throw;

			}

		}



		public override void ResetValue(object component)

		{

		}



		public override bool ShouldSerializeValue(object component)

		{

			return true;

		}



		public override void SetValue(object component, object value)

		{

			((Symbol)component)[this.field.Name] = value;

		}

	}

}

