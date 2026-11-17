using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.VisualBasic;
using Microsoft.VisualBasic;
using System;
using System.CodeDom;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;

namespace ReportingServices.RdlExpressions
{
	internal sealed class VBExpressionCodeProvider : VBCodeProvider
	{
		private const string LineContinuation = " _";

		private const string LineTooLongError = "BC30494";

		private const int MaxLineLength = 65535;

		private void CheckAndAddReference(List<MetadataReference> roslynReferences, string file)
		{
			if (String.IsNullOrEmpty(file)) return;
			if (!File.Exists(file)) return;
			roslynReferences.Add(MetadataReference.CreateFromFile(file));
		}

		// Assembly.Location is empty for any assembly bundled into a single-file publish
		// (there's no separate on-disk DLL to point at), which made CheckAndAddReference
		// silently skip it via its IsNullOrEmpty guard - the expression compiler would then
		// fail with real "type not found" Roslyn errors for any report containing a
		// non-trivial expression. AssemblyExtensions.TryGetRawMetadata reads the assembly's
		// PE metadata directly out of the loaded module in memory, which works whether the
		// assembly came from a normal file, a single-file bundle, or any other loader that
		// doesn't leave a `.Location` behind.
		private unsafe MetadataReference CreateReferenceFromLoadedAssembly(Assembly assembly)
		{
			if (assembly == null) return null;
			if (!System.Reflection.Metadata.AssemblyExtensions.TryGetRawMetadata(assembly, out byte* blob, out int length))
			{
				return null;
			}
			var moduleMetadata = ModuleMetadata.CreateFromMetadata((IntPtr)blob, length);
			return AssemblyMetadata.Create(moduleMetadata).GetReference();
		}

		private void CheckAndAddReference(List<MetadataReference> roslynReferences, Assembly assembly)
		{
			if (assembly == null) return;
			string location = assembly.Location;
			if (!String.IsNullOrEmpty(location) && File.Exists(location))
			{
				roslynReferences.Add(MetadataReference.CreateFromFile(location));
				return;
			}
			MetadataReference reference = CreateReferenceFromLoadedAssembly(assembly);
			if (reference != null)
			{
				roslynReferences.Add(reference);
			}
		}

		public override CompilerResults CompileAssemblyFromDom(CompilerParameters options, params CodeCompileUnit[] compilationUnits)
		{
			var writer = new StringWriter();
			GenerateCodeFromCompileUnit(compilationUnits[0], writer, new CodeGeneratorOptions());
			var roslynTree = SyntaxFactory.ParseSyntaxTree(writer.ToString(), null, "");
			var roslynOptions = new VisualBasicCompilationOptions(OutputKind.DynamicallyLinkedLibrary);
			var roslynReferences = new List<MetadataReference>();
			CheckAndAddReference(roslynReferences, Assembly.Load("System.Private.CoreLib"));
			CheckAndAddReference(roslynReferences, Assembly.Load("Microsoft.VisualBasic.Core"));
			CheckAndAddReference(roslynReferences, Assembly.Load("System.Runtime"));
			CheckAndAddReference(roslynReferences, Assembly.Load("System.Text.RegularExpressions"));
#if NETSTANDARD2_0_OR_GREATER
			CheckAndAddReference(roslynReferences, Assembly.Load("netstandard"));
#endif
			// ReportObjectModelProxy (Fields!/ReportItems!/etc.) is the RDL expression host's core
			// proxy type - every non-trivial expression needs it. Referenced directly here (rather
			// than via options.ReferencedAssemblies, a plain path StringCollection its callers used
			// to populate with typeof(ReportObjectModelProxy).Assembly.Location) so the
			// Assembly-typed CheckAndAddReference overload's single-file-publish fallback applies:
			// .Location is empty for an assembly bundled into a single-file publish, and by the time
			// that had collapsed to an empty/unresolvable path string, the live Assembly needed to
			// recover it was already gone.
			CheckAndAddReference(roslynReferences, typeof(ReportingServices.RdlExpressions.ExpressionHostObjectModel.ReportObjectModelProxy).Assembly);
			foreach (var assembly in options.ReferencedAssemblies)
			{
				if (assembly == "System.dll") continue;
				CheckAndAddReference(roslynReferences, assembly);
			}
			var roslynCompilation = VisualBasicCompilation.Create(Path.GetFileNameWithoutExtension(options.OutputAssembly), new[] { roslynTree }, options: roslynOptions, references: roslynReferences);
			var roslynAssembly = new MemoryStream();
			var result = roslynCompilation.Emit(roslynAssembly);
			if (!result.Success)
			{
				var error = result.Diagnostics.Where(e => e.Severity == DiagnosticSeverity.Error).FirstOrDefault();
				if (error != null) throw new InvalidOperationException(error.ToString());
				throw new Exception();
			}
			var assemblyFile = Path.GetTempFileName();
			File.WriteAllBytes(assemblyFile, roslynAssembly.ToArray());
			var compilerResults = new CompilerResults(new TempFileCollection()) { PathToAssembly = assemblyFile };
			return compilerResults;
		}

		private CompilerResults CompileAssemblyFromDomWithRetry(CompilerParameters options, CodeCompileUnit compilationUnit)
		{
			options.TempFiles.KeepFiles = true;
			CompilerResults compilerResults = base.CompileAssemblyFromDom(options, compilationUnit);
			TempFileCollection tempFiles = compilerResults.TempFiles;
			try
			{
				if (compilerResults.Errors.HasErrors)
				{
					options.TempFiles = new TempFileCollection();
					return RetryCompile(options, compilerResults.Errors) ?? compilerResults;
				}
				return compilerResults;
			}
			finally
			{
				foreach (object item in tempFiles)
				{
					string text = item as string;
					if (text != null)
					{
						try
						{
							File.Delete(text);
						}
						catch
						{
						}
					}
				}
			}
		}

		private CompilerResults RetryCompile(CompilerParameters options, CompilerErrorCollection compilerErrorCollection)
		{
			CompilerResults result = null;
			string text = null;
			List<int> list = new List<int>();
			foreach (CompilerError item in compilerErrorCollection)
			{
				if (string.Equals(item.ErrorNumber, "BC30494", StringComparison.OrdinalIgnoreCase))
				{
					list.Add(item.Line);
					if (text == null)
					{
						text = item.FileName;
					}
				}
			}
			if (text != null)
			{
				List<string> list2 = File.ReadAllLines(text).ToList();
				SplitLongLines(list2, list);
				File.WriteAllLines(text, list2.ToArray());
				options.TempFiles = new TempFileCollection();
				result = base.CompileAssemblyFromFile(options, text);
			}
			return result;
		}

		private static void SplitLongLines(List<string> lines, List<int> longLineNumbers)
		{
			if (longLineNumbers.Count > 0)
			{
				longLineNumbers.Sort();
			}
			for (int num = longLineNumbers.Count - 1; num >= 0; num--)
			{
				int num2 = longLineNumbers[num] - 1;
				string text = lines[num2];
				lines.RemoveAt(num2);
				while (text.Length > 65535)
				{
					int num3 = FindSafeSplitPosition(text);
					if (num3 <= 0)
					{
						lines.Insert(num2, text);
						return;
					}
					string item = text.Substring(0, num3) + " _";
					lines.Insert(num2, item);
					num2++;
					text = text.Substring(num3 + 1);
				}
				lines.Insert(num2, text);
			}
		}

		private static int FindSafeSplitPosition(string line)
		{
			int num = line.LastIndexOf(' ', 65535 - " _".Length);
			bool flag = true;
			while (flag && num > 80)
			{
				int num2 = line.LastIndexOf('"', num, 80);
				flag = (num2 != -1);
				if (flag)
				{
					num = line.LastIndexOf(' ', num2);
				}
			}
			if (flag)
			{
				return -1;
			}
			return num;
		}
	}
}
