using ReportingServices.OnDemandProcessing.Scalability;
using ReportingServices.ReportIntermediateFormat.Persistence;
using ReportingServices.ReportProcessing;
using System;
using System.Collections.Generic;

namespace ReportingServices.OnDemandProcessing.TablixProcessing
{
	[PersistedWithinRequestOnly]
	internal sealed class BTree : IStorable, IPersistable, IDisposable
	{
		private BTreeNode m_root;

		private ScalableList<BTreeNode> m_nodes;

		private static Declaration m_declaration = GetDeclaration();

		public int Size => ItemSizes.SizeOf(m_root) + ItemSizes.SizeOf(m_nodes);

		internal BTree()
		{
		}

		internal BTree(IHierarchyObj owner, OnDemandProcessingContext odpContext, int level)
		{
			m_nodes = new ScalableList<BTreeNode>(level, odpContext.TablixProcessingScalabilityCache);
			m_root = new BTreeNode(owner);
		}

		internal void NextRow(object keyValue, IHierarchyObj owner)
		{
			try
			{
				if (!m_root.SearchAndInsert(keyValue, m_nodes, owner, out BTreeNodeValue newSiblingValue, out int _, out int globalNewSiblingIndex))
				{
					int count = m_nodes.Count;
					m_nodes.Add(m_root);
					m_root = new BTreeNode(owner);
					m_root.SetFirstChild(m_nodes, count);
					m_root.Tuples.Add(new BTreeNodeTuple(newSiblingValue, globalNewSiblingIndex), m_nodes);
				}
			}
			catch (ReportProcessingException_SpatialTypeComparisonError)
			{
				throw new ReportProcessingException(owner.RegisterComparisonError("SortExpression"));
			}
			catch (ReportProcessingException_ComparisonError)
			{
				throw new ReportProcessingException(owner.RegisterComparisonError("SortExpression"));
			}
		}

		internal void Traverse(ProcessingStages operation, bool ascending, ITraversalContext traversalContext)
		{
			m_root.Traverse(operation, ascending, m_nodes, traversalContext);
		}

		public void Dispose()
		{
			m_nodes.Dispose();
			m_nodes = null;
		}

		void IPersistable.Serialize(IntermediateFormatWriter writer)
		{
			writer.RegisterDeclaration(m_declaration);
			while (writer.NextMember())
			{
				switch (writer.CurrentMember.MemberName)
				{
				case MemberName.Root:
					writer.Write(m_root);
					break;
				case MemberName.SortTreeNodes:
					writer.Write(m_nodes);
					break;
				default:
					Global.Tracer.Assert(condition: false);
					break;
				}
			}
		}

		void IPersistable.Deserialize(IntermediateFormatReader reader)
		{
			reader.RegisterDeclaration(m_declaration);
			while (reader.NextMember())
			{
				switch (reader.CurrentMember.MemberName)
				{
				case MemberName.Root:
					m_root = (BTreeNode)reader.ReadRIFObject();
					break;
				case MemberName.SortTreeNodes:
					m_nodes = reader.ReadRIFObject<ScalableList<BTreeNode>>();
					break;
				default:
					Global.Tracer.Assert(condition: false);
					break;
				}
			}
		}

		void IPersistable.ResolveReferences(Dictionary<ReportingServices.ReportIntermediateFormat.Persistence.ObjectType, List<MemberReference>> memberReferencesCollection, Dictionary<int, IReferenceable> referenceableItems)
		{
		}

		ReportingServices.ReportIntermediateFormat.Persistence.ObjectType IPersistable.GetObjectType()
		{
			return ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.BTree;
		}

		internal static Declaration GetDeclaration()
		{
			if (m_declaration == null)
			{
				List<MemberInfo> list = new List<MemberInfo>();
				list.Add(new MemberInfo(MemberName.Root, ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.BTreeNode));
				list.Add(new MemberInfo(MemberName.SortTreeNodes, ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ScalableList, ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.BTreeNode));
				return new Declaration(ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.BTree, ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.None, list);
			}
			return m_declaration;
		}
	}
}
