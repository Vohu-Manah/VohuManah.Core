using ReportingServices.OnDemandReportRendering;
using ReportingServices.ReportProcessing;
using System;
using System.Collections.Specialized;
using System.Globalization;

namespace ReportingServices.ReportRendering
{
	internal sealed class TextBox : ReportItem
	{
		private SimpleTextBoxInstanceInfo m_simpleInstanceInfo;

		private string m_value;

		private ActionInfo m_actionInfo;

		private object m_originalValue;

		internal SimpleTextBoxInstanceInfo SimpleInstanceInfo
		{
			get
			{
				if (base.IsCustomControl)
				{
					throw new RenderingObjectModelException(ProcessingErrorCode.rsInvalidOperation);
				}
				if (base.ReportItemInstance == null)
				{
					return null;
				}
				if (m_simpleInstanceInfo == null)
				{
					ReportingServices.ReportProcessing.TextBoxInstance textBoxInstance = (ReportingServices.ReportProcessing.TextBoxInstance)base.ReportItemInstance;
					m_simpleInstanceInfo = textBoxInstance.GetSimpleInstanceInfo(base.RenderingContext.ChunkManager, base.RenderingContext.InPageSection);
				}
				return m_simpleInstanceInfo;
			}
		}

		public Report.DataElementStyles DataElementStyle
		{
			get
			{
				if (!((ReportingServices.ReportProcessing.TextBox)base.ReportItemDef).DataElementStyleAttribute)
				{
					return Report.DataElementStyles.ElementNormal;
				}
				return Report.DataElementStyles.AttributeNormal;
			}
		}

		public bool CanGrow => ((ReportingServices.ReportProcessing.TextBox)base.ReportItemDef).CanGrow;

		public bool CanShrink => ((ReportingServices.ReportProcessing.TextBox)base.ReportItemDef).CanShrink;

		public string Value
		{
			get
			{
				_ = base.RenderingContext;
				string text = m_value;
				if (m_value == null)
				{
					ReportingServices.ReportProcessing.TextBox textBox = (ReportingServices.ReportProcessing.TextBox)base.ReportItemDef;
					if (textBox.Value.Type == ExpressionInfo.Types.Constant)
					{
						text = textBox.Value.Value;
					}
					else if (base.ReportItemInstance == null)
					{
						text = null;
					}
					else if (textBox.IsSimpleTextBox(base.RenderingContext.IntermediateFormatVersion))
					{
						text = SimpleInstanceInfo.FormattedValue;
						if (text == null)
						{
							text = (SimpleInstanceInfo.OriginalValue as string);
						}
					}
					else
					{
						TextBoxInstanceInfo textBoxInstanceInfo = (TextBoxInstanceInfo)base.InstanceInfo;
						text = textBoxInstanceInfo.FormattedValue;
						if (text == null)
						{
							text = (textBoxInstanceInfo.OriginalValue as string);
						}
					}
					if (base.RenderingContext.CacheState)
					{
						m_value = text;
					}
				}
				return text;
			}
		}

		public ReportUrl HyperLinkURL
		{
			get
			{
				ActionInfo actionInfo = m_actionInfo;
				if (actionInfo == null)
				{
					actionInfo = ActionInfo;
				}
				return actionInfo?.Actions[0].HyperLinkURL;
			}
		}

		public ReportUrl DrillthroughReport
		{
			get
			{
				ActionInfo actionInfo = m_actionInfo;
				if (actionInfo == null)
				{
					actionInfo = ActionInfo;
				}
				return actionInfo?.Actions[0].DrillthroughReport;
			}
		}

		public NameValueCollection DrillthroughParameters
		{
			get
			{
				ActionInfo actionInfo = m_actionInfo;
				if (actionInfo == null)
				{
					actionInfo = ActionInfo;
				}
				return actionInfo?.Actions[0].DrillthroughParameters;
			}
		}

		public string BookmarkLink
		{
			get
			{
				ActionInfo actionInfo = m_actionInfo;
				if (actionInfo == null)
				{
					actionInfo = ActionInfo;
				}
				return actionInfo?.Actions[0].BookmarkLink;
			}
		}

		public ActionInfo ActionInfo
		{
			get
			{
				ActionInfo actionInfo = m_actionInfo;
				if (actionInfo == null)
				{
					ReportingServices.ReportProcessing.Action action = ((ReportingServices.ReportProcessing.TextBox)base.ReportItemDef).Action;
					if (action != null)
					{
						ReportingServices.ReportProcessing.ActionInstance actionInstance = null;
						string ownerUniqueName = base.UniqueName;
						if (base.ReportItemInstance != null)
						{
							actionInstance = ((TextBoxInstanceInfo)base.InstanceInfo).Action;
							if (base.RenderingContext.InPageSection)
							{
								ownerUniqueName = base.ReportItemInstance.UniqueName.ToString(CultureInfo.InvariantCulture);
							}
						}
						else if (base.RenderingContext.InPageSection && m_intUniqueName != 0)
						{
							ownerUniqueName = m_intUniqueName.ToString(CultureInfo.InvariantCulture);
						}
						actionInfo = new ActionInfo(action, actionInstance, ownerUniqueName, base.RenderingContext);
						if (base.RenderingContext.CacheState)
						{
							m_actionInfo = actionInfo;
						}
					}
				}
				return actionInfo;
			}
		}

		public bool Duplicate
		{
			get
			{
				if (!HideDuplicates)
				{
					return false;
				}
				if (base.ReportItemInstance != null)
				{
					return ((TextBoxInstanceInfo)base.InstanceInfo).Duplicate;
				}
				return false;
			}
		}

		public bool HideDuplicates => ((ReportingServices.ReportProcessing.TextBox)base.ReportItemDef).HideDuplicates != null;

		public string Formula => ((ReportingServices.ReportProcessing.TextBox)base.ReportItemDef).Formula;

		public object OriginalValue
		{
			get
			{
				object obj = m_originalValue;
				if (m_originalValue == null)
				{
					ReportingServices.ReportProcessing.TextBox textBox = (ReportingServices.ReportProcessing.TextBox)base.ReportItemDef;
					obj = ((textBox.Value.Type == ExpressionInfo.Types.Constant) ? textBox.Value.Value : ((base.ReportItemInstance == null) ? null : ((!textBox.IsSimpleTextBox(base.RenderingContext.IntermediateFormatVersion)) ? ((TextBoxInstanceInfo)base.InstanceInfo).OriginalValue : SimpleInstanceInfo.OriginalValue)));
					if (base.RenderingContext.CacheState)
					{
						m_originalValue = obj;
					}
				}
				return obj;
			}
		}

		public TypeCode SharedTypeCode => ((ReportingServices.ReportProcessing.TextBox)base.ReportItemDef).ValueType;

		public override bool Hidden
		{
			get
			{
				if (base.ReportItemInstance == null)
				{
					return RenderingContext.GetDefinitionHidden(base.ReportItemDef.Visibility);
				}
				if (base.ReportItemDef.Visibility == null)
				{
					return false;
				}
				if (base.ReportItemDef.Visibility.Toggle != null)
				{
					return base.RenderingContext.IsItemHidden(base.ReportItemInstance.UniqueName, potentialSender: true);
				}
				return base.InstanceInfo.StartHidden;
			}
		}

		public bool IsToggleParent
		{
			get
			{
				if (base.ReportItemInstance == null)
				{
					return false;
				}
				if (IsSharedToggleParent)
				{
					return base.RenderingContext.IsToggleParent(base.ReportItemInstance.UniqueName);
				}
				return false;
			}
		}

		public bool IsSharedToggleParent => ((ReportingServices.ReportProcessing.TextBox)base.ReportItemDef).IsToggle;

		public bool ToggleState
		{
			get
			{
				if (base.ReportItemInstance == null)
				{
					return false;
				}
				if (IsSharedToggleParent)
				{
					if (base.RenderingContext.IsToggleStateNegated(base.ReportItemInstance.UniqueName))
					{
						return !((TextBoxInstanceInfo)base.InstanceInfo).InitialToggleState;
					}
					return ((TextBoxInstanceInfo)base.InstanceInfo).InitialToggleState;
				}
				return false;
			}
		}

		public bool CanSort => ((ReportingServices.ReportProcessing.TextBox)base.ReportItemDef).UserSort != null;

		public SortOptions SortState
		{
			get
			{
				if (base.IsCustomControl)
				{
					return SortOptions.None;
				}
				return base.RenderingContext.GetSortState(m_intUniqueName);
			}
		}

		internal TextBox(string uniqueName, int intUniqueName, ReportingServices.ReportProcessing.TextBox reportItemDef, ReportingServices.ReportProcessing.TextBoxInstance reportItemInstance, RenderingContext renderingContext)
			: base(uniqueName, intUniqueName, reportItemDef, reportItemInstance, renderingContext)
		{
		}

		internal override bool Search(SearchContext searchContext)
		{
			if (base.SkipSearch)
			{
				return false;
			}
			return SearchTextBox(searchContext.FindValue);
		}

		private bool SearchTextBox(string findValue)
		{
			string value = Value;
			if (value != null && value.IndexOf(findValue, 0, StringComparison.OrdinalIgnoreCase) >= 0)
			{
				return true;
			}
			return false;
		}
	}
}
