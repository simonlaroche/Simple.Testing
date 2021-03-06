﻿extern alias resharper;
using System.Collections.Generic;

using resharper::JetBrains.ReSharper.Psi.Tree;
#if RESHARPER_5 || RESHARPER_6
using resharper::JetBrains.ReSharper.UnitTestFramework;
#else
using resharper::JetBrains.ReSharper.UnitTestExplorer;
#endif

namespace Simple.Testing.ReSharperRunner.Explorers.ElementHandlers
{
	internal interface IElementHandler
	{
#if RESHARPER_6
    bool Accepts(ITreeNode element);
    IEnumerable<UnitTestElementDisposition> AcceptElement(ITreeNode element, IFile file);
    void Cleanup(ITreeNode element);
#else
		bool Accepts(IElement element);
		IEnumerable<UnitTestElementDisposition> AcceptElement(IElement element, IFile file);
#endif
	}
}