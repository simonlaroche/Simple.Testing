﻿extern alias resharper;
using System;
using System.Drawing;

using resharper::JetBrains.CommonControls;
using resharper::JetBrains.ReSharper.Features.Common.TreePsiBrowser;
using resharper::JetBrains.ReSharper.UnitTestExplorer;
using resharper::JetBrains.ReSharper.UnitTestFramework;
using resharper::JetBrains.ReSharper.UnitTestFramework.UI;
using resharper::JetBrains.TreeModels;
using resharper::JetBrains.UI.TreeView;

namespace Simple.Testing.ReSharperRunner.Presentation
{
  class Presenter : TreeModelBrowserPresenter
  {
    public Presenter()
    {
      Present<ContextElement>(PresentContext);
      Present<FieldElement>(PresentSpecification);
    }

    protected virtual void PresentContext(ContextElement element,
                                          IPresentableItem item,
                                          TreeModelNode modelNode,
                                          PresentationState state)
    {
      PresentItem(item, element, state, UnitTestElementImage.TestContainer);
    }

    protected virtual void PresentSpecification(FieldElement element,
                                                IPresentableItem item,
                                                TreeModelNode modelNode,
                                                PresentationState state)
    {
      PresentItem(item, element, state, UnitTestElementImage.Test);
    }

    static void PresentItem(IPresentableItem item, Element element, PresentationState state, UnitTestElementImage type)
    {
      item.RichText = element.GetPresentation();

      SetTextColor(item, element);
    }

    static void SetTextColor(IPresentableItem item, Element element)
    {
      if (element.Explicit)
      {
        item.RichText.SetForeColor(SystemColors.GrayText);
      }

      item.RichText.SetForeColor(SystemColors.GrayText, 0, element.GetTitlePrefix().Length);
    }

    protected override bool IsNaturalParent(object parentValue, object childValue)
    {
      var @namespace = parentValue as UnitTestNamespace;
      var context = childValue as ContextElement;

      if (context != null && @namespace != null)
      {
        return @namespace.Equals(context.GetNamespace());
      }

      return base.IsNaturalParent(parentValue, childValue);
    }

    protected override object Unwrap(object value)
    {
      var specification = value as FieldElement;
      if (specification != null)
      {
        value = specification.GetDeclaredElement();
      }

      var context = value as ContextElement;
      if (context != null)
      {
        value = context.GetDeclaredElement();
      }
      return base.Unwrap(value);
    }

  }
}